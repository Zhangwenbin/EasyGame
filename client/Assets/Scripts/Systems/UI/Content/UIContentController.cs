#if UNITY_EDITOR
using UnityEditor;
#endif

using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Buffers;
using System.Diagnostics.CodeAnalysis;

namespace EG
{

    [AddComponentMenu("Scripts/System/UI/Content/UIContentController")]
    [ExecuteInEditMode()]
    public partial class UIContentController : AppMonoBehaviour
    {
        public enum EFlag
        {
            LOCAL_CACHE     = (1<<0),
            DO_NOT_SCROLL   = (1<<1),
            NOT_REMOVENODE  = (1<<2),
            CALC_PADDINGGRID = (1<<3),
            KEEP_ACTIVE = (1<<4),
        }
        
        #if UNITY_EDITOR
        public static Utility.EnumArray.Element[] EFlagNames = new Utility.EnumArray.Element[]
        {
            new Utility.EnumArray.Element( EFlag.LOCAL_CACHE       , "ローカルキャッシュを有効にする" ),
            new Utility.EnumArray.Element( EFlag.DO_NOT_SCROLL     , "スクロールしない" ),
            new Utility.EnumArray.Element( EFlag.NOT_REMOVENODE    , "ノードを削除しない" ),
            new Utility.EnumArray.Element( EFlag.CALC_PADDINGGRID  , "セルの内包計算でパディングの影響を考慮する." ),
            new Utility.EnumArray.Element( EFlag.KEEP_ACTIVE       , "ノードのActive切替を可能な限り減らす" ),
        };
        #endif
        
        public enum Constraint
        {
            Flexible            ,   // 自動
            FixedColumnCount    ,   // 横指定
            FixedRowCount       ,   // 縦指定
            FixedCount          ,   // 横縦指定
            FullColumnCount     ,   // 横指定フル
            FullRowCount        ,   // 縦指定フル
        }
        
        static readonly Vector2 FarAway = new Vector2(-65000, -65000);
        
        [UnityEngine.Serialization.FormerlySerializedAs("m_Cache")]
        public int                          m_Flag                 = 0;
        public bool                         m_WidthLoop            = false;
        public bool                         m_HeightLoop           = false;
                                           
        public UIContentScroller            m_Scroller             = null;
        public UIContentNode                m_Node                 = null;
        public float                        m_PaddingLeft          = 0.0f;
        public float                        m_PaddingRight         = 0.0f;
        public float                        m_PaddingTop           = 0.0f;
        public float                        m_PaddingBottom        = 0.0f;
        public Vector2                      m_CellSize             = Vector2.zero;
        public Vector2                      m_Spacing              = Vector2.zero;
        public Constraint                   m_Constraint           = Constraint.Flexible;
        public int                          m_ConstraintCount      = 0;
        public int                          m_ConstraintCountSub   = 0;
        

        public bool                         m_PeriodicZeroBoundary = false;

        public bool                         m_KeepsNodeIndexOrder  = false;
        

        public bool                         m_NotMoveRefresh       = false;
        
        RectTransform                       m_RectTransform        = null;
        UIContentSource                     m_Source               = null;
        
        bool                                m_NodeStatic           = false;
        Dictionary<int,UIContentNode>       m_NodeUsed             = new Dictionary<int,UIContentNode>();
        List<UIContentNode>                 m_NodeEmpty            = new List<UIContentNode>();
        List<UIContentNode>                 m_RemoveList           = new List<UIContentNode>();
        Dictionary<int,List<UIContentNode>> m_NodeCache            = new Dictionary<int,List<UIContentNode>>();
        
        Toggle[]                            m_PageToggle           = null;
        
        Vector2                             m_PageSize             = Vector2.zero;
        int                                 m_PageNodeWidthNum     = 1;
        int                                 m_PageNodeHeightNum    = 1;
        int                                 m_NodeWidthNum         = 1;
        int                                 m_NodeHeightNum        = 1;
        int                                 m_ViewWidthNum         = 1;
        int                                 m_ViewHeightNum        = 1;
        int                                 m_SelectNode           = -1;
                                            
        float                               m_MoveRefreshTime      = 0.0f;
        bool                                m_MoveRefresh          = false;
        object                              m_Work                 = null;
        
        bool                                m_ForceUpdate          = false;
        
        
        public UIContentScroller Scroller
        {
            get
            {
                if( IsFlag( UIContentController.EFlag.DO_NOT_SCROLL ) == false )
                {
                    if( m_Scroller == null )
                    {
                        m_Scroller = gameObject.GetComponentInParent<UIContentScroller>();
                    }
                }
                return m_Scroller;
            }
        }
                                            
        public bool                         isScrollHorizontal  { get { return m_Scroller != null ? m_Scroller.horizontal: false; } }
        public bool                         isScrollVertical    { get { return m_Scroller != null ? m_Scroller.vertical: false; } }
        
        public bool                         isNodeStatic        { get { return m_NodeStatic;    } }
        
        public Vector2 AnchoredPosition
        {
            set
            {
                rectTransform.anchoredPosition = value;
            }
            get
            {
                return rectTransform.anchoredPosition;
            }
        }
        
        public Vector2 Size
        {
            get
            {
                return rectTransform.rect.size;
            }
        }
        
        public UIContentSource Source                   { get { return m_Source; } }
        public RectTransform rectTransform              { get { if( m_RectTransform == null ) m_RectTransform = gameObject.GetComponent<RectTransform>(); return m_RectTransform; } }
        public Dictionary<int, UIContentNode> NodeUsed  { get { return m_NodeUsed; } }
        public List<UIContentNode> RemoveList           { get { return m_RemoveList; } }
        public List<UIContentNode> NodeEmpty            { get { return m_NodeEmpty; } }
        public Vector2 PageSize                         { get { return m_PageSize; } }
        public int NodeWidthNum                         { get { return m_NodeWidthNum; }    set { m_NodeWidthNum = value; } }
        public int NodeHeightNum                        { get { return m_NodeHeightNum; }   set { m_NodeHeightNum = value; } }
        public int ViewWidthNum                         { get { return m_ViewWidthNum; }    set { m_ViewWidthNum = value; } }
        public int ViewHeightNum                        { get { return m_ViewHeightNum; }   set { m_ViewHeightNum = value; } }
        public int SelectNode                           { get { return m_SelectNode; }      set { m_SelectNode = value; } }

        
        private void Awake( )
        {
            #if UNITY_EDITOR
            if( Application.isPlaying == false )
            {
                return;
            }
            #endif
            
            // 
            if( IsFlag( UIContentController.EFlag.DO_NOT_SCROLL ) == false )
            {
                if( m_Scroller == null )
                {
                    m_Scroller = gameObject.GetComponentInParent<UIContentScroller>();
                }
            }
            
            // 
            if( m_Node != null )
            {
                m_NodeStatic = false;
                m_Node.gameObject.SetActive( false );
            }
            else
            {
                m_NodeStatic = true;
                
                Initialize( null, Vector2.zero );
            }
        }
        

        public virtual void Initialize( UIContentSource source, Vector2 pos )
        {
            base.Initialize( );
            
            if( IsFlag( EFlag.LOCAL_CACHE ) )
            {
                m_NodeCache.Clear( );
                for( int i = 0, max = rectTransform.childCount; i < max; ++i )
                {
                    Transform child = rectTransform.GetChild( i );
                    UIContentNode node = child.GetComponent<UIContentNode>( );
                    if( node != null )
                    {
                        List<UIContentNode> list = null;
                        if( m_NodeCache.TryGetValue( node.nodeHash, out list ) == false )
                        {
                            list = new List<UIContentNode>( );
                            m_NodeCache.Add( node.nodeHash, list );
                        }

                        if (IsFlag(EFlag.KEEP_ACTIVE))
                        {
                            node.RectTransform.anchoredPosition = FarAway;
                            node.enabled = false;
                        }
                        else
                        {
                            node.SetActiveSafe( false );
                        }
                        list.Add( node );
                    }
                }
            }
            

            InitializeParam( );
            
            if( m_NodeStatic == false )
            {
                // 
                AnchoredPosition = pos;
                
                // 
                if( source != null )
                {
                    SetCurrentSource( source );
                }
                 
                // 
                Resize( );
                
                // 
                CreateNode( );
            }
            else
            {
                List<UIContentNode> list = GetNodeChilds();
                
                // 
                AnchoredPosition = pos;
                
                // 
                if( source != null )
                {
                    SetCurrentSource( source );
                }
                
                // 
                Resize( list.Count );
                
                // 
                CreateStaticNode( list );
            }
        }
        
        private void InitializeParam( )
        {
            // 
            if( m_RectTransform == null )
            {
                m_RectTransform = gameObject.GetComponent<RectTransform>();
            }

            if( m_RectTransform != null )
            {
                // 
                m_RectTransform.anchorMin    = new Vector2( 0.0f, 1.0f );
                m_RectTransform.anchorMax    = new Vector2( 0.0f, 1.0f );
                m_RectTransform.pivot        = new Vector2( 0.0f, 1.0f );
                
                // 
                if( m_Scroller != null && m_Scroller.viewport != null )
                {
                    m_PageSize.x = m_Scroller.viewport.rect.width;
                    m_PageSize.y = m_Scroller.viewport.rect.height;
                }
                else
                {
                    m_PageSize.x = m_RectTransform.rect.width;
                    m_PageSize.y = m_RectTransform.rect.height;
                }
                
                // 
                CalcNodeViewSize();
            }
        }
        

        public virtual void CalcNodeViewSize( )
        {
         
            var expandsByPadding = m_Scroller?.viewport == null || ReferenceEquals(m_Scroller.viewport, m_RectTransform);
            Vector2 size = m_PageSize;
            if( m_Constraint == Constraint.Flexible )
            {
                // 
                if (expandsByPadding)
                {
                    size.x -= (m_PaddingLeft + m_PaddingRight);
                    size.y -= (m_PaddingTop + m_PaddingBottom);
                }
                m_NodeWidthNum = Mathf.CeilToInt((size.x + m_Spacing.x) / (m_CellSize.x + m_Spacing.x));
                m_NodeHeightNum = Mathf.CeilToInt((size.y + m_Spacing.y) / (m_CellSize.y + m_Spacing.y));
            }
            else if( m_Constraint == Constraint.FixedColumnCount || m_Constraint == Constraint.FullColumnCount )
            {
                // 
                if (expandsByPadding)
                {
                    size.y -= (m_PaddingTop + m_PaddingBottom);
                }
                m_NodeWidthNum = m_ConstraintCount;
                m_NodeHeightNum = Mathf.CeilToInt((size.y + m_Spacing.y) / (m_CellSize.y + m_Spacing.y));
            }
            else if( m_Constraint == Constraint.FixedRowCount || m_Constraint == Constraint.FullRowCount )
            {
                // 
                if (expandsByPadding)
                {
                    size.x -= (m_PaddingLeft + m_PaddingRight);
                }
                m_NodeWidthNum = Mathf.CeilToInt((size.x + m_Spacing.x) / (m_CellSize.x + m_Spacing.x)) + 1;
                m_NodeHeightNum = m_ConstraintCount;
            }
            else if( m_Constraint == Constraint.FixedCount )
            {
                // 
                m_NodeWidthNum = m_ConstraintCount;
                m_NodeHeightNum = m_ConstraintCountSub;
            }
            
            m_PageNodeWidthNum = m_NodeWidthNum;
            m_PageNodeHeightNum = m_NodeHeightNum;
        }
        

        public override void Release()
        {
            base.Release( );
            
  
            if( m_Source != null )
            {
                m_Source.OnDisable( );
                m_Source.Release();
                m_Source = null;
            }
            
            // 
            DestroyNode( );
            
            // 
            m_NodeCache.Clear( );
        }
        

        protected virtual void Update()
        {
            #if UNITY_EDITOR
            if( Application.isPlaying == false )
            {
                UpdateNodeEditor( );
                return;
            }
            #endif
            
            // 
            UpdateNode();
            
            // 
            if( m_Source != null )
            {
                m_Source.Update();
            }
            
            // 
            m_ForceUpdate = false;
        }
        

        void LateUpdate()
        {
#if UNITY_EDITOR
            if( Application.isPlaying == false )
            {
                return;
            }
            
            bool isTouch = Input.GetMouseButton(0);
#else
            bool isTouch = Input.touchCount > 0;
#endif
            
            if( m_ForceUpdate )
            {
                // 
                UpdateNode();
                
                // 
                if( m_Source != null )
                {
                    m_Source.Update();
                }
                
                m_ForceUpdate = false;
            }
            

            if( m_Scroller != null )
            {
                if( isTouch == false && !m_NotMoveRefresh )
                {
                    if( m_MoveRefresh == false )
                    {
                        // 
                        if( m_Scroller.velocity.magnitude < 0.01f )
                        {
                            // 
                            m_MoveRefreshTime += Time.deltaTime;
                            if( m_MoveRefreshTime > 0.1f )
                            {
                                // 
                                m_MoveRefresh = true;
                                // 
                                MoveRefresh( );
                                // 
                                m_Scroller.StopMovement( );
                            }
                        }
                    }
                }
                else
                {
                    m_MoveRefresh = false;
                    m_MoveRefreshTime = 0.0f;
                }
            }
        }
        

        public virtual void UpdateNode()
        {
            // 
            if( m_NodeStatic == false )
            {
                CheckActiveNode( );
            }
            else
            {
                List<UIContentNode> list = GetNodeChilds( );
                if( list.Count != m_NodeUsed.Count )
                {
                    Resize( list.Count );
                    CreateStaticNode( list );
                }
            }
            
            if( m_Scroller != null )
            {
                UpdatePageToggle( );
                
                RectTransform viewport = m_Scroller.viewport;
                if( viewport != null )
                {
                    Rect viewportRect = viewport.rect;

                    foreach( var pair in m_NodeUsed )
                    {
                        UIContentNode node = pair.Value;
                        if( node != null )
                        {
                            Vector2 localPoint = viewport.InverseTransformPoint( node.RectTransform.position );
                            localPoint = node.GetPivotAnchoredPosition( localPoint );
                            
                            float lx = localPoint.x - node.SizeX * 0.5f + 2.5f;
                            float rx = localPoint.x + node.SizeX * 0.5f - 2.5f;
                            float ty = localPoint.y + node.SizeY * 0.5f - 2.5f;
                            float by = localPoint.y - node.SizeY * 0.5f + 2.5f;
                            
                            if( rx > viewportRect.x && lx < viewportRect.x + viewportRect.width && 
                                ty > viewportRect.y && by < viewportRect.y + viewportRect.height )
                            {
                                node.OnViewIn( localPoint );
                            }
                            else
                            {
                                node.OnViewOut( localPoint );
                            }
                            
                            if( node.Index == m_SelectNode )
                            {
                                node.OnSelectOn();
                            }
                            else
                            {
                                node.OnSelectOff();
                            }
                        }
                    }
                }
            }
            else
            {
                foreach( var pair in m_NodeUsed )
                {
                    UIContentNode node = pair.Value;
                    if( node != null )
                    {
                        node.OnViewIn( node.GetPivotAnchoredPosition( node.RectTransform.anchoredPosition ) );
                        
                        if( node.Index == m_SelectNode )
                        {
                            node.OnSelectOn();
                        }
                        else
                        {
                            node.OnSelectOff();
                        }
                    }
                }
            }
        }


        public bool MoveRefresh()
        {
            Vector2 pos = AnchoredPosition;
            bool refresh = false;
            
            if( m_WidthLoop )
            {
                float w = m_RectTransform.sizeDelta.x - (m_PaddingLeft + m_PaddingRight);
                if( pos.x > m_PageSize.x )
                {
                    while( pos.x > 0.0f )
                    {
                        pos.x -= w;
                    }
                    refresh = true;
                }
                else if( pos.x < -m_RectTransform.sizeDelta.x )
                {
                    while( pos.x < -w )
                    {
                        pos.x += w;
                    }
                    refresh = true;
                }
            }
            
            if( m_HeightLoop )
            {
                float h = m_RectTransform.sizeDelta.y - (m_PaddingTop + m_PaddingBottom);
                if( pos.y > m_PageSize.y )
                {
                    while( pos.y > 0.0f )
                    {
                        pos.y -= h;
                    }
                    refresh = true;
                }
                else if( pos.y < -m_RectTransform.sizeDelta.y )
                {
                    while( pos.y < -h )
                    {
                        pos.y += h;
                    }
                    refresh = true;
                }
            }
            
            if( refresh )
            {
                AnchoredPosition = pos;
                
                UpdateNode();
            }
            
            return refresh;
        }
        
        #if UNITY_EDITOR
        public void UpdateNodeEditor( )
        {
            List<UIContentNode> list = GetNodeChilds( );
            InitializeParam( );
            Resize( list.Count );
            for( int i = 0; i < list.Count; ++i )
            {
                int x = i % m_ViewWidthNum;
                int y = i / m_ViewWidthNum;
                Vector2 pos = GetNodePos( x, y );
                list[ i ].Initialize( this );
                list[ i ].RectTransform.anchoredPosition = list[ i ].PosToLocalPos( pos );
            }
        }
        #endif
        
        public void CreateStaticNode( List<UIContentNode> list )
        {
            // 
            DestroyNode();
            
            // 
            for( int i = 0; i < list.Count; ++i )
            {
                UIContentNode node = list[i];
                
                int x = i % m_ViewWidthNum;
                int y = i / m_ViewWidthNum;
                
                int index = GetParamIndex( x, y );
                
                node.Initialize( this );
                node.Setup( index, x, y, GetParam( index ) );
                node.Attach( );
                node.SetActive( true );
                
                m_NodeUsed.Add( GetNodeKey( x, y ), node );
            }
            
            // 
            UpdateNode();
            
            // 
            if( m_KeepsNodeIndexOrder )
            {
                SortNodeByIndex();
            }
            
            // 
            if( m_Scroller != null )
            {
                m_Scroller.Refresh( );
            }
            
            // 
            m_SelectNode = -1;
        }
        
        public virtual void CreateNode()
        {
            if( m_Node == null )
            {
                Debug.LogError( "ベースノードが設定されていません > " + gameObject.name );
                return;
            }
            
            DestroyNode();
            
            // 生成
            int nodeWidthNum = m_NodeWidthNum;
            int nodeHeightNum = m_NodeHeightNum;
            
            if( m_Constraint != Constraint.FixedCount && m_Constraint != Constraint.FullColumnCount && m_Constraint != Constraint.FullRowCount )
            {
                if( isScrollHorizontal ) nodeWidthNum += 2;
                if( isScrollVertical ) nodeHeightNum += 2;
            }
            
            // 生成
            for( int y = 0; y < nodeHeightNum; ++y )
            {
                for( int x = 0; x < nodeWidthNum; ++x )
                {
                    UIContentNode node = AddNode( );
                    if( node != null )
                    {
                        m_NodeEmpty.Add( node );
                    }
                }
            }
            
            UpdateNode();
            
            m_SelectNode = -1;
        }
        
        public void DestroyNode()
        {
            for( int i = 0; i < m_NodeEmpty.Count; ++i )
            {
                RemoveNode( m_NodeEmpty[i] );
            }
            m_NodeEmpty.Clear();
            
            foreach( var pair in m_NodeUsed )
            {
                UIContentNode node = pair.Value;
                if( node != null )
                {
                    if( m_NodeStatic == false )
                    {
                        RemoveNode( node );
                    }
                    else
                    {
                        node.Release( );
                        node.gameObject.SetActive( false );
                    }
                }
            }
            m_NodeUsed.Clear();
        }

        public UIContentNode AddNode( )
        {
            UIContentNode result = null;
            
            if( IsFlag( EFlag.LOCAL_CACHE ) )
            {
                List<UIContentNode> list = null;
                if( m_NodeCache.TryGetValue( m_Node.nodeHash, out list ) )
                {
                    if( list.Count > 1 )
                    {
                        result = list[1];
                        list.RemoveAt( 1 );
                    }
                }
            }
            
            if( result == null )
            {
                if( m_Source != null )
                {
                    result = m_Source.Instantiate( m_Node, m_RectTransform, false );
                }
                else
                {
                    GameObject obj = GameObject.Instantiate( m_Node.gameObject, m_RectTransform, false );
                    if( obj != null )
                    {
                        result = obj.GetComponent<UIContentNode>();
                    }
                }
            }
            
            if( result != null )
            {
                result.Initialize( this );
                if (IsFlag(EFlag.KEEP_ACTIVE))
                {
                    result.RectTransform.anchoredPosition = FarAway;
                    result.enabled = false;
                }
                else
                {
                    result.SetActiveSafe( false );
                }
            }
            
            return result;
        }
        

        public void RemoveNode( UIContentNode node )
        {
            if( node == null ) return;
            
            if( IsFlag( EFlag.LOCAL_CACHE ) )
            {
                List<UIContentNode> list = null;
                if( m_NodeCache.TryGetValue( node.nodeHash, out list ) )
                {
                    node.Release( );
                    node.SetActiveSafe( false );
                    list.Add( node );
                }
            }
            else
            {
                GameObject.Destroy( node );
            }
        }
        

        public virtual void ClearActiveNode()
        {
            foreach( var pair in m_NodeUsed )
            {
                UIContentNode node = pair.Value;
                if( node != null )
                {
                    node.Detach( );
                    node.SetActive( false );
                    m_NodeEmpty.Add( node );
                }
            }
            
            m_NodeUsed.Clear( );
        }
        
 
        public virtual void CheckActiveNode()
        {
            UIContentGrid grid = GetGrid();
            
            // 
            if( IsFlag( UIContentController.EFlag.NOT_REMOVENODE ) == false )
            {
                m_RemoveList.Clear( );
                
                foreach( var pair in m_NodeUsed )
                {
                    UIContentNode node = pair.Value;
                    if( node != null )
                    {
                        if( node.IsReMake() || node.IsValid() == false || 
                            node.GridX < grid.x-1 || node.GridX > grid.x + m_PageNodeWidthNum ||
                            node.GridY < grid.y-1 || node.GridY > grid.y + m_PageNodeHeightNum )
                        {
                            m_RemoveList.Add( node );
                        }
                    }
                }
                
                for( int i = 0; i < m_RemoveList.Count; ++i )
                {
                    UIContentNode node = m_RemoveList[i];
                    if( node != null )
                    {
                        node.Detach( );
                        if (IsFlag(EFlag.KEEP_ACTIVE))
                        {
                            node.RectTransform.anchoredPosition = FarAway;
                            node.enabled = false;
                        }
                        else
                        {
                            node.SetActive( false );
                        }
                        m_NodeUsed.Remove( GetNodeKey( node.GridX, node.GridY ) );
                        m_NodeEmpty.Add( node );
                    }
                }
            }
            
            var dirtyOrder = false;

            // 
            {
                int minX = grid.x;
                int minY = grid.y;
                int maxX = grid.x + m_PageNodeWidthNum;
                int maxY = grid.y + m_PageNodeHeightNum;
                
                if( isScrollHorizontal )
                {
                    minX -= 1;
                    maxX += 1;
                }
                if( isScrollVertical )
                {
                    minY -= 1;
                    maxY += 1;
                }
                
                for( int y = minY; y < maxY; ++y )
                {
                    for( int x = minX; x < maxX; ++x )
                    {
                        if( ( m_WidthLoop || x >= 0 && x < m_ViewWidthNum ) && ( m_HeightLoop || y >= 0 && y < m_ViewHeightNum ) )
                        {
                            UIContentNode node = GetNodeUsed( x, y );
                            if( node == null )
                            {
                                int index = GetParamIndex( x, y );
                                UIContentSource.Param param = GetParam( index );
                                if( param != null )
                                {
                                    node = GetNodeEmpty( index );
                                    if( node != null )
                                    {
                                        node.Setup( index, x, y, param );
                                        node.Attach( );
                                        if (IsFlag(EFlag.KEEP_ACTIVE) && node.gameObject.activeSelf)
                                        {
                                            node.enabled = true;
                                        }
                                        else
                                        {
                                            node.SetActive( true );
                                        }
                                        m_NodeUsed.Add( GetNodeKey( x, y ), node );
                                        dirtyOrder = true;
                                    }
                                    else
                                    {
                                        Debug.LogError( "ノードが不足しています" );
                                    }
                                }
                            }
                        }
                    }
                }
            }
            
            if( dirtyOrder && m_KeepsNodeIndexOrder )
            {
                SortNodeByIndex( );
            }
        }
        

        public void SortNodeByIndex()
        {
            var keys = ArrayPool<int>.Shared.Rent(m_NodeUsed.Count);
            var offset = 0;
            foreach (var pair in m_NodeUsed)
            {
                keys[offset++] = pair.Key;
            }
            Array.Sort(keys, 0, offset);

            for (var i = 0; i < offset; ++i)
            {
                var node = m_NodeUsed[keys[i]];
                node.RectTransform.SetAsLastSibling();
            }
            ArrayPool<int>.Shared.Return(keys);
        }

 
        public int GetNodeKey( int x, int y )
        {
            return ( ( x & 0xFFFF ) << 16 ) | ( y & 0xFFFF );
        }
        
        
        public UIContentNode GetNodeUsed( int x, int y )
        {
            UIContentNode result = null;
            m_NodeUsed.TryGetValue( GetNodeKey( x, y ), out result );
            return result;
        }
        

        public UIContentNode GetNodeEmpty( int index )
        {
            if( m_NodeEmpty.Count > 0 )
            {
                UIContentNode result = null;
                int high = -1;
                for( int i = 0; i < m_NodeEmpty.Count; ++i )
                {
                    if( m_NodeEmpty[i].Index == index )
                    {
                        result = m_NodeEmpty[i];
                        m_NodeEmpty.RemoveAt(i);
                        return result;
                    }
                    else if( high == -1 && m_NodeEmpty[i].Index == -1 )
                    {
                        high = i;
                    }
                }
                if( high != -1 )
                {
                    result = m_NodeEmpty[high];
                    m_NodeEmpty.RemoveAt(high);
                    return result;
                }
                result = m_NodeEmpty[0];
                m_NodeEmpty.RemoveAt(0);
                return result;
            }
            return null;
        }
        

        public int GetNodeCount( )
        {
            return m_NodeEmpty.Count + m_NodeUsed.Count;
        }
        

        public List<UIContentNode> GetNodeAll( )
        {
            List<UIContentNode> result = new List<UIContentNode>();
            result.AddRange( m_NodeEmpty );
            result.AddRange( m_NodeUsed.Values.ToArray() );
            return result;
        }
        

        public List<UIContentNode> GetNodeChilds( bool activeOnly = true )
        {
            List<UIContentNode> result = new List<UIContentNode>();
            for( int i = 0; i < transform.childCount; ++i )
            {
                Transform child = transform.GetChild(i);
                //if( child != null && child.gameObject.activeSelf )
                if( child != null )
                {
                    if( activeOnly && child.gameObject.activeSelf == false ) continue;
                    UIContentNode node = child.GetComponent<UIContentNode>();
                    if( node != null )
                    {
                        result.Add( node );
                    }
                }
            }
            return result;
        }
        
  
        public void SetFlag( EFlag value, bool sw )
        {
            if( sw )
                m_Flag |= (int)value;
            else
                m_Flag &= ~(int)value;
        }
        

        public bool IsFlag( EFlag value )
        {
            return ( m_Flag & (int)value ) != 0;
        }
        

        public int GetParamIndex( int x, int y )
        {
            UIContentGrid grid = GetNormalizeGrid( x, y );
            if( m_Constraint == Constraint.FixedRowCount )
            {
                return grid.x * m_ViewHeightNum + grid.y;
            }
            return grid.y * m_ViewWidthNum + grid.x;
        }
        

        public UIContentSource.Param GetParam( int x, int y )
        {
            if( m_Source != null )
            {
                return m_Source.GetParam( GetParamIndex( x, y ) );
            }
            return null;
        }
        public UIContentSource.Param GetParam( int index )
        {
            if( m_Source != null )
            {
                return m_Source.GetParam( index );
            }
            return null;
        }

        
        public Vector2 GetPageSize( )
        {
            Vector2 size = m_PageSize;
            if( size.x == 0.0f )
            {
                size.x = m_Node.GridX;
            }
            if( size.y == 0.0f )
            {
                size.y = m_Node.GridY;
            }
            return size;
        }
        

        public Vector2Int GetPageNum( )
        {
            Vector2 pageSize = GetPageSize( );
            Vector2 size = m_RectTransform.sizeDelta;
            return new Vector2Int( MathUtility.RoundUpToInt( size.x / m_PageSize.x ), MathUtility.RoundUpToInt( size.y / m_PageSize.y ) );
        }
        
        public UIContentGrid GetPage( Vector2 pos )
        {
            Vector2 pageSize = GetPageSize( );
            UIContentGrid grid = GetGrid( pos );
            if( pageSize.x != 0.0f )
            {
                if( pos.x < 0 ) pos.x -= pageSize.x * 0.5f;
                else            pos.x += pageSize.x * 0.5f;
                grid.x = UIContentGrid.FloatToInt( -pos.x / ( pageSize.x + m_Spacing.x ) );
            }
            if( pageSize.y != 0.0f )
            {
                if( pos.y < 0 ) pos.y -= pageSize.y * 0.5f;
                else            pos.y += pageSize.y * 0.5f;
                grid.y = UIContentGrid.FloatToInt( pos.y / ( pageSize.y + m_Spacing.y ) );
            }
            return grid;
        }
        public UIContentGrid GetPageGrid( Vector2 offset, UIContentScroller scroller = null )
        {
            if( scroller == null ) scroller = m_Scroller;
            if( scroller != null )
            {
                // 
                Vector2 pageSize = GetPageSize( );
                Vector2 contentSize = Size;
                
                // 
                Vector2 scrollArea = contentSize - pageSize;
                scrollArea.x = Mathf.Max( 0, scrollArea.x );
                scrollArea.y = Mathf.Max( 0, scrollArea.y );
                
                // 
                Vector2 anchorPos = scrollArea * scroller.normalizedPosition;
                Vector2 pageAnchorPos = anchorPos + offset;
                
                // 
                return GetGrid( pageAnchorPos * new Vector2( -1, 1 ) );
            }
            return UIContentGrid.zero;
        }
        

        public UIContentGrid GetLastPageGrid( )
        {
            Vector2 pos = GetLastPageAnchorePos( );
            UIContentGrid result = UIContentGrid.zero;
            pos.x *= -1;
            result.fx = ( pos.x / ( m_CellSize.x + m_Spacing.x ) );
            result.fy = ( pos.y / ( m_CellSize.y + m_Spacing.y ) );
            return result;
        }
        
  
        public Vector2 _test = Vector2.zero;
        public UIContentGrid GetGrid( )
        {
            if( m_RectTransform != null )
            {
                return GetGrid( AnchoredPosition );
            }
            return UIContentGrid.zero;
        }
        public virtual UIContentGrid GetGrid( Vector2 pos )
        {
            UIContentGrid result = UIContentGrid.zero;
            if( m_RectTransform != null )
            {
                pos.x *= -1;
                if (IsFlag(EFlag.CALC_PADDINGGRID))
                {
                    pos.x += m_PaddingLeft;
                    pos.y -= m_PaddingTop;
                }
                result.fx = ( pos.x / ( m_CellSize.x + m_Spacing.x ) );
                result.fy = ( pos.y / ( m_CellSize.y + m_Spacing.y ) );
                //pos.x += m_PaddingLeft - m_Spacing.x;
                //result.fx = ( -pos.x / ( m_CellSize.x + m_Spacing.x ) );
                //pos.y -= m_PaddingTop - m_Spacing.y;
                //result.fy = ( pos.y / ( m_CellSize.y + m_Spacing.y ) );
                UIContentGrid lastGrid = GetLastPageGrid( );
                if( m_WidthLoop == false )
                {
                    if( result.x < 0 )
                    {
                        result.x = 0;
                    }
                    else if( result.x > lastGrid.x )
                    {
                        result.x = lastGrid.x;
                    }
                }
                if( m_HeightLoop == false )
                {
                    if( result.y < 0 )
                    {
                        result.y = 0;
                    }
                    else if( result.y > lastGrid.y )
                    {
                        result.y = lastGrid.y;
                    }
                }
            }
            return result;
        }
        public UIContentGrid GetGrid( int index )
        {
            UIContentGrid result = UIContentGrid.zero;
            if( m_Constraint == Constraint.Flexible || m_Constraint == Constraint.FixedRowCount || m_Constraint == Constraint.FixedRowCount )
            {
                result.x = index / m_ViewHeightNum;
                result.y = index % m_ViewHeightNum;
            }
            else if( m_Constraint == Constraint.FixedColumnCount || m_Constraint == Constraint.FullColumnCount )
            {
                result.x = index % m_ViewWidthNum;
                result.y = index / m_ViewWidthNum;
            }
            else if( m_Constraint == Constraint.FixedCount )
            {
                if( isScrollHorizontal )
                {
                    result.x = index / m_ViewHeightNum;
                    result.y = index % m_ViewHeightNum;
                }
                else
                {
                    result.x = index % m_ViewWidthNum;
                    result.y = index / m_ViewWidthNum;
                }
            }
            return result;
        }
        
   
        public UIContentGrid GetNormalizeGrid(int x, int y)
        {
            int ix, iy;
            
            if( x < 0 )
            {
                if( m_PeriodicZeroBoundary )
                {
                    ix = (m_ViewWidthNum + (x % m_ViewWidthNum)) % m_ViewWidthNum;
                }
                else
                {
                    ix = (m_ViewWidthNum - (x % m_ViewWidthNum)) % m_ViewWidthNum;
                }
            }
            else
            {
                ix = x % m_ViewWidthNum;
            }
            
            if( y < 0 )
            {
                if( m_PeriodicZeroBoundary )
                {
                    iy = (m_ViewHeightNum + (y % m_ViewHeightNum)) % m_ViewHeightNum;
                }
                else
                {
                    iy = (m_ViewHeightNum - (y % m_ViewHeightNum)) % m_ViewHeightNum;
                }
            }
            else
            {
                iy = y % m_ViewHeightNum;
            }
            
            return new UIContentGrid( ix, iy );
        }
        

        public virtual Vector2 GetNodePos( int x, int y )
        {
            return new Vector2( m_PaddingLeft + x * ( m_CellSize.x + m_Spacing.x ), - ( m_PaddingTop + y * ( m_CellSize.y + m_Spacing.y ) ) );
        }
        

        public Vector2 GetLastPageAnchorePos()
        {
            Vector2 size = m_RectTransform.sizeDelta;
            size.x -= m_PageSize.x;
            if( size.x < 0 ) size.x = 0; 
            size.y -= m_PageSize.y;
            if( size.y < 0 ) size.y = 0; 
            return new Vector2( -size.x, size.y );
        }
        

        public Vector2 GetAnchorePosFromGrid( int x, int y )
        {
            Vector2 result = Vector2.zero;
            if( isScrollHorizontal )
            {
                if( x >= 0 )
                {
                    result.x = -( x * ( m_CellSize.x + m_Spacing.x ) ) - m_PaddingLeft;
                }
                else
                {
                }
            }
            if( isScrollVertical )
            {
                if( y >= 0 )
                {
                    result.y = y * ( m_CellSize.y + m_Spacing.y ) + m_PaddingTop;
                }
                else
                {
                }
            }
            return result;
        }
        

        public void DisablePageToggle( )
        {
            if( m_Scroller == null ) return;
            
            // 
            if( m_Scroller.PageToggleGroup != null )
            {
                m_Scroller.PageToggleGroup.SetActiveSafe( false );
            }
            
            // 
            if( m_PageToggle != null )
            {
                for( int i = 1; i < m_PageToggle.Length; ++i )
                {
                    Toggle toggle = m_PageToggle[i];
                    if( toggle == null ) continue;
                    GameObject.Destroy( toggle );
                }
                m_PageToggle = null;
            }
        }
        

        public void UpdatePageToggle( )
        {
            if( m_Scroller.IsAdsorption == false || m_Scroller.PageToggleGroup == null ) return;
            
            ToggleGroup toggleGroup = m_Scroller.PageToggleGroup;
            
            // 生成
            int sourceCount = m_Source.GetCount( );
            if( m_PageToggle == null || sourceCount > 0 && m_PageToggle.Length != sourceCount )
            {
                m_PageToggle = new Toggle[ m_Source.GetCount() ];
                
                SerializeValueBehaviour valueBehaviour = toggleGroup.GetComponent<SerializeValueBehaviour>( );
                if( valueBehaviour != null )
                {
                    Toggle toggle = valueBehaviour.list.GetComponent<Toggle>( "toggle" );
                    if( toggle != null )
                    {
                        toggle.isOn = false;
                        m_PageToggle[0] = toggle;
                        for( int i = 1; i < m_PageToggle.Length; ++i )
                        {
                            GameObject gobj = GameObject.Instantiate( toggle.gameObject );
                            if( gobj != null )
                            {
                                m_PageToggle[i] = gobj.GetComponent<Toggle>( );
                                m_PageToggle[i].transform.SetParent( toggleGroup.transform, false );
                            }
                        }
                        toggleGroup.SetAllTogglesOff( );
                        toggleGroup.SetActiveSafe( true );
                    }
                }
            }
            
            // 
            if( toggleGroup.gameObject.activeSelf )
            {
                UIContentGrid page = GetPageGrid( GetPageSize( ) * 0.5f );
                if( m_Scroller.horizontal )
                {
                    if( page.x >= 0 && page.x < m_PageToggle.Length && m_PageToggle[ page.x ] != null )
                    {
                        m_PageToggle[ page.x ].isOn = true;
                    }
                }
                else if( m_Scroller.vertical )
                {
                    if( page.y >= 0 && page.y < m_PageToggle.Length && m_PageToggle[ page.y ] != null )
                    {
                        m_PageToggle[ page.y ].isOn = true;
                    }
                }
            }
        }
        

        public virtual void Resize( int count = 0 )
        {
            // 
            if( count == 0 )
            {
                if( m_Source == null || m_RectTransform == null )
                {
                    return;
                }
                // 
                count = m_Source.GetCount();
            }
            
            // 
            Vector2 size = m_PageSize;
            if( m_Constraint == Constraint.Flexible )
            {
                // 
                m_ViewWidthNum = Mathf.CeilToInt( (float)count / (float)m_NodeHeightNum );
                m_ViewHeightNum = m_NodeHeightNum;
            }
            else if( m_Constraint == Constraint.FixedColumnCount )
            {
                // 
                m_ViewWidthNum = m_NodeWidthNum;
                m_ViewHeightNum = Mathf.CeilToInt( (float)count / (float)m_NodeWidthNum );
            }
            else if( m_Constraint == Constraint.FixedRowCount )
            {
                // 
                m_ViewWidthNum = Mathf.CeilToInt( (float)count / (float)m_NodeHeightNum );
                m_ViewHeightNum = m_NodeHeightNum;
            }
            else if( m_Constraint == Constraint.FixedCount )
            {
                // 
                m_ViewWidthNum = m_NodeWidthNum;
                m_ViewHeightNum = m_NodeHeightNum;
            }
            else if( m_Constraint == Constraint.FullColumnCount )
            {
                // 
                m_NodeWidthNum = m_ViewWidthNum = m_ConstraintCount;
                m_NodeHeightNum = m_ViewHeightNum = Mathf.CeilToInt( (float)count / (float)m_ConstraintCount );
            }
            else if( m_Constraint == Constraint.FullRowCount )
            {
                // 
                m_NodeWidthNum = m_ViewWidthNum = Mathf.CeilToInt( (float)count / (float)m_ConstraintCount );
                m_NodeHeightNum = m_ViewHeightNum = m_ConstraintCount;
            }
            
            if( m_ViewWidthNum == 0 ) m_ViewWidthNum = 1;
            if( m_ViewHeightNum == 0 ) m_ViewHeightNum = 1;
            
            // 
            //size.x = m_ViewWidthNum * m_CellSize.x + ( m_ViewWidthNum - 1 ) * m_Spacing.x + m_PaddingLeft + m_PaddingRight;
            //size.y = m_ViewHeightNum * m_CellSize.y + ( m_ViewHeightNum - 1 ) * m_Spacing.y + m_PaddingTop + m_PaddingBottom;
            size.x = m_ViewWidthNum * ( m_CellSize.x + m_Spacing.x ) + m_PaddingLeft + m_PaddingRight;
            size.y = m_ViewHeightNum * ( m_CellSize.y + m_Spacing.y ) + m_PaddingTop + m_PaddingBottom;
            m_RectTransform.sizeDelta = size;
        }
        

        public void SetCurrentSource( UIContentSource source )
        {
            // 
            if( m_Source != null )
            {
                m_Source.OnDisable( );
                m_Source.Release();
            }
            
            // 
            m_Source = source;
            
            // 
            if( m_Source != null )
            {
                m_Source.Initialize( this );
                m_Source.OnEnable( );
            }
            
            // 
            DisablePageToggle( );
            
            // 
            m_ForceUpdate = true;
        }

        public UIContentSource GetCurrentSource( )
        {
            return m_Source;
        }
        
   
        public void SetWork( object value )
        {
            m_Work = value;
        }

        public object GetWork( )
        {
            return m_Work;
        }
        

        public void SetSelect( int index )
        {
            m_SelectNode = index;
        }
    
        public int GetSelect( )
        {
            return m_SelectNode;
        }
        
        
        public void SetSpacing( Vector2 value )
        {
            m_Spacing = value;
        }
  
        public Vector2 GetSpacing( )
        {
            return m_Spacing;
        }
        
    
        public Vector2 GetAnchorePos( )
        {
            if( m_RectTransform != null )
            {
                return AnchoredPosition;
            }
            return Vector2.zero;
        }
        

        public void SetAnchorePos( Vector2 pos )
        {
            if( m_RectTransform != null )
            {
                AnchoredPosition = pos;
            }
        }
        

        public void SetForcusAnchorePos( Vector2 pos )
        {
            m_Source.SetForcusAnchorePos( pos );
            m_Source.UpdateForcus( );
        }
        
    
        public void SetForcusIndex( int index )
        {
            m_Source.SetForcusIndex( index );
            m_Source.UpdateForcus( );
        }
        

        public void SetForcus<T>( System.Func<T,bool> contains ) where T : UIContentSource.Param
        {
            UIContentSource.Param param = m_Source.GetParam<T>( contains );
            if( param != null )
            {
                m_Source.SetForcusIndex( param.Id );
                m_Source.UpdateForcus( );
            }
            else
            {
                m_Source.SetForcusIndex( 0 );
                m_Source.UpdateForcus( );
            }
        }
    }
    

    #if UNITY_EDITOR
    
    [CustomEditor( typeof(UIContentController) )]
    public class EditorInspcetor_UIContentController : Editor
    {
        bool m_FoldoutPadding = false;
        
        public override void OnInspectorGUI()
        {
            var obj = target as UIContentController;
            if (obj == null)
            {
                return;
            }
            
            EditorGUI.BeginChangeCheck( );
            {
                #if UNITY_EDITOR
                if( GUILayout.Button( "ノード整列" ) )
                {
                    obj.UpdateNodeEditor( );
                }
                #endif
                
                obj.m_Scroller      = EditorHelp.ObjectField( obj, "ContentScroller", obj.m_Scroller, typeof( UIContentScroller ), true ) as UIContentScroller;
                obj.m_Node          = EditorHelp.ObjectField( obj, "Node", obj.m_Node, typeof( UIContentNode ), true, null ) as UIContentNode;
                
                GUILayout.BeginHorizontal( );
                {
                    Utility.EnumArray array = new Utility.EnumArray( UIContentController.EFlagNames );
                    int mask = 0;
                    for( int n = 0; n < array.elements.Length; ++n )
                    {
                        if( ( obj.m_Flag & (int)array.elements[n].index ) != 0 )
                        {
                            mask |= (1<<n);
                        }
                    }
                    mask = UnityEditor.EditorGUILayout.MaskField( "フラグ", mask, array.dispNames );
                    obj.m_Flag = 0;
                    for( int n = 0; n < array.elements.Length; ++n )
                    {
                        if( ( mask & (1<<n) ) != 0 )
                        {
                            obj.m_Flag |= (int)array.elements[n].index;
                        }
                    }
                }
                GUILayout.EndHorizontal( );
                
                obj.m_WidthLoop     = EditorHelp.ToggleLeft( obj, "Width Loop", obj.m_WidthLoop ); 
                obj.m_HeightLoop    = EditorHelp.ToggleLeft( obj, "Height Loop", obj.m_HeightLoop ); 
                
                m_FoldoutPadding = EditorGUILayout.Foldout( m_FoldoutPadding, "Padding" );
                if( m_FoldoutPadding )
                {
                    obj.m_PaddingLeft   = EditorHelp.FloatField( obj, "  Left", obj.m_PaddingLeft );
                    obj.m_PaddingRight  = EditorHelp.FloatField( obj, "  Right", obj.m_PaddingRight );
                    obj.m_PaddingTop    = EditorHelp.FloatField( obj, "  Top", obj.m_PaddingTop );
                    obj.m_PaddingBottom = EditorHelp.FloatField( obj, "  Bottom", obj.m_PaddingBottom );
                }
                
                EditorGUILayout.BeginHorizontal( );
                {
                    obj.m_CellSize = EditorHelp.Vector2Field( obj, "Cell Size", obj.m_CellSize ); 
                    
                }
                EditorGUILayout.EndHorizontal( );
                
                obj.m_Spacing = EditorHelp.Vector2Field( obj, "Spacing", obj.m_Spacing ); 
                
                obj.m_Constraint = (UIContentController.Constraint)EditorHelp.EnumPopup( obj, "Constraint", obj.m_Constraint ); 
                
                if( obj.m_Constraint != UIContentController.Constraint.Flexible )
                {
                    if( obj.m_Constraint == UIContentController.Constraint.FixedCount )
                    {
                        obj.m_ConstraintCount = EditorHelp.IntField( obj, "Width Count", obj.m_ConstraintCount );
                        obj.m_ConstraintCountSub = EditorHelp.IntField( obj, "Height Count", obj.m_ConstraintCountSub );
                    }
                    else
                    {
                        obj.m_ConstraintCount = EditorHelp.IntField( obj, "Constraint Count", obj.m_ConstraintCount );
                    }
                }
                
                obj.m_PeriodicZeroBoundary  = EditorHelp.ToggleLeft( obj, "0境界でのソースの並び順（falseで反射、trueで繰り返し）", obj.m_PeriodicZeroBoundary );
                obj.m_KeepsNodeIndexOrder   = EditorHelp.ToggleLeft( obj, "ノードの表示順序を常にソートする", obj.m_KeepsNodeIndexOrder );
                obj.m_NotMoveRefresh        = EditorHelp.ToggleLeft( obj, "Not Move Refresh", obj.m_NotMoveRefresh );
            }
            if( EditorGUI.EndChangeCheck( ) )
            {
                if( Application.isPlaying )
                {
                    Vector2 pos = obj.AnchoredPosition;
                    UIContentSource source = obj.GetCurrentSource( );
                    obj.Release();
                    obj.Initialize( source, pos );
                }
                EditorUtility.SetDirty( obj.gameObject );
            }
        }
    }
    
    #endif
}

