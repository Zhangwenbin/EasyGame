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

namespace EG
{
    [AddComponentMenu("Scripts/System/UI/Content/UIContentLayout")]
    [ExecuteInEditMode()]
    public class UIContentLayout : AppMonoBehaviour
    {
        public enum EFlag
        {
            LOCAL_CACHE,
        }

        #if UNITY_EDITOR
        public static Utility.EnumArray.Element[] EFlagNames = new[]
        {
            new Utility.EnumArray.Element( EFlag.LOCAL_CACHE , "ローカルキャッシュを有効にする" ),
        };
        #endif
        
        static readonly Vector2 FarAway = new Vector2(-65000, -65000);

        public UIContentNode                    m_Node              = null;

        [HideInInspector]
        public float                            m_PaddingLeft       = 0.0f;

        [HideInInspector]
        public float                            m_PaddingRight      = 0.0f;

        [HideInInspector]
        public float                            m_PaddingTop        = 0.0f;

        [HideInInspector]
        public float                            m_PaddingBottom     = 0.0f;

        [HideInInspector]
        public Vector2                          m_CellSize          = Vector2.zero;

        [HideInInspector]
        public Vector2                          m_Spacing           = Vector2.zero;

        [CustomField("フラグ", CustomFieldAttribute.Type.BitFlag, typeof(EFlag), typeof(UIContentLayout))]
        public int                              m_Flag              = 0;

        RectTransform                           m_RectTransform     = null;
                                            
        UIContentSource                         m_Source            = null;
                                            
        bool                                    m_NodeStatic        = false;
        Dictionary<int,UIContentNode>           m_NodeUsed          = new Dictionary<int,UIContentNode>();
        List<UIContentNode>                     m_NodeEmpty         = new List<UIContentNode>();
        List<UIContentNode>                     m_RemoveList        = new List<UIContentNode>();
        
        int                                     m_SelectNode        = -1;

        object                                  m_Work              = null;
        
        HorizontalOrVerticalLayoutGroup         m_LayoutGroup       = null;
        
        bool                                    m_ForceUpdate       = false;
        
        public Vector2 AnchoredPosition
        {
            set
            {
                m_RectTransform.anchoredPosition = value;
            }
            get
            {
                if( m_RectTransform!=null )
                {
                    return m_RectTransform.anchoredPosition;
                }
                return Vector2.zero;
            }
        }
        
        public bool IsHorizontal { get { return ( m_LayoutGroup != null ) ? m_LayoutGroup is HorizontalLayoutGroup : false; } }
        public bool IsVertical   { get { return ( m_LayoutGroup != null ) ? m_LayoutGroup is VerticalLayoutGroup : false;   } }
        
        private void Awake( )
        {
            #if UNITY_EDITOR
            if( Application.isPlaying == false )
            {
                return;
            }
            #endif
            
            // 
            m_RectTransform = gameObject.GetComponent<RectTransform>();
            m_LayoutGroup   = gameObject.GetComponent<HorizontalOrVerticalLayoutGroup>();
            
            _InitParam();
            
            // 
            if( m_Node != null )
            {
                m_NodeStatic = false;
                m_Node.gameObject.SetActive( false );
            }
            else
            {
                m_NodeStatic = true;
                
                //
                Initialize( null, Vector2.zero );
            }
        }
        

        void _InitParam()
        {
            if( m_LayoutGroup == null )
                return;
            
            m_PaddingLeft    = m_LayoutGroup.padding.left;
            m_PaddingRight   = m_LayoutGroup.padding.right;
            m_PaddingTop     = m_LayoutGroup.padding.top;
            m_PaddingBottom  = m_LayoutGroup.padding.bottom;
            
            if( IsHorizontal)
            {
                m_Spacing.x = m_LayoutGroup.spacing;
            }
            else if( IsVertical )
            {
                m_Spacing.y = m_LayoutGroup.spacing;
            }
            
            if( m_Node != null )
            {
                LayoutElement elem = m_Node.GetComponent<LayoutElement>();
                if( elem != null )
                {
                    m_CellSize.x = elem.preferredWidth;
                    m_CellSize.y = elem.preferredHeight;
                }
            }
        }
        

        public virtual void Initialize( UIContentSource source )
        {
            InitializeParam();
            
            Initialize( source, AnchoredPosition );
        }
        
        public virtual void Initialize( UIContentSource source, Vector2 pos )
        {
            base.Initialize( );
            
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
                CreateNode( );
            }
        }
        

        private void InitializeParam( )
        {
            // 
            if( m_RectTransform == null )
            {
                m_RectTransform = gameObject.GetComponent<RectTransform>();
            }
        }
        
   
        public override void Release()
        {
            base.Release( );
            
            if( m_Source != null )
            {
                m_Source.Release();
                m_Source = null;
            }
            
            DestroyNode();
        }
        
        protected virtual void Update()
        {
            UpdateNode();
            
            if( m_Source != null )
            {
                m_Source.Update();
            }

            m_ForceUpdate = false;
        }
        
        protected virtual void LateUpdate()
        {
            if( m_ForceUpdate )
            {
 
                UpdateNode();
                
                if( m_Source != null )
                {
                    m_Source.Update();
                }
                
                m_ForceUpdate = false;
            }
        }
        
        public void UpdateNode()
        {
            CheckActiveNode();
            
            foreach( var pair in m_NodeUsed )
            {
                UIContentNode node = pair.Value;
                if( node != null )
                {
                    node.OnViewIn( Vector2.zero );
                    
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
        
        public void CreateNode()
        {
            if( m_Node == null )
            {
                Debug.LogError( "ベースノードが設定されていません > " + gameObject.name );
                return;
            }

            DestroyNode(true);
            
            // 生成
            int nodeNum = m_Source.GetCount();
            var emptyCount = m_NodeEmpty.Count;
            
            // 生成
            for( int idx = 0; idx < nodeNum; ++idx )
            {
                UIContentNode node = null;

                if (idx >= emptyCount)
                {
                    if( m_Source != null )
                    {
                        node = m_Source.Instantiate( m_Node, m_RectTransform, false );
                    }
                    else
                    {
                        GameObject obj = GameObject.Instantiate( m_Node.gameObject, m_RectTransform, false );
                        if( obj != null )
                        {
                            node = obj.GetComponent<UIContentNode>();
                        }
                    }
                }
                
                if( node != null )
                {
                    node.Initialize( null );
                    if (IsFlag(EFlag.LOCAL_CACHE))
                    {
                        node.LayoutElement.ignoreLayout = true;
                        node.RectTransform.anchoredPosition = FarAway;
                        node.enabled = false;
                    }
                    else
                    {
                        node.SetActiveSafe( false );
                    }
                    m_NodeEmpty.Add( node );
                }
            }
            
            UpdateNode();
            
            m_SelectNode = -1;
        }
        
        public void DestroyNode(bool allowCache = false)
        {
            if (!(allowCache && IsFlag(EFlag.LOCAL_CACHE)))
            {
                for( int i = 0; i < m_NodeEmpty.Count; ++i )
                {
                    UIContentNode node = m_NodeEmpty[i];
                    if( node != null )
                    {
                        node.Release();
                        node.gameObject.SafeDestroy( );
                    }
                }
                m_NodeEmpty.Clear();
            }
            
            foreach( var (_, node) in m_NodeUsed )
            {
                if ( node == null )
                    continue;

                if (allowCache && IsFlag(EFlag.LOCAL_CACHE))
                {
                    node.Detach();
                    node.LayoutElement.ignoreLayout = true;
                    node.RectTransform.anchoredPosition = FarAway;
                    node.enabled = false;
                    m_NodeEmpty.Add(node);
                }
                else
                {
                    node.Release();
                    if( m_NodeStatic == false ) node.gameObject.SafeDestroy( );
                }
            }
            m_NodeUsed.Clear();
        }
        
        private void CheckActiveNode()
        {
            {
                m_RemoveList.Clear( );
                
                foreach( var pair in m_NodeUsed )
                {
                    UIContentNode node = pair.Value;
                    if( node != null )
                    {
                        if( node.IsReMake() || node.IsValid() == false )
                        {
                            m_RemoveList.Add( node );
                        }
                    }
                }
                
                for( int i = 0; i < m_RemoveList.Count; ++i )
                {
                    UIContentNode node = m_RemoveList[ i ];
                    if( node != null )
                    {
                        node.Detach( );
                        node.transform.SetSiblingIndex( 999 );
                        if (IsFlag(EFlag.LOCAL_CACHE))
                        {
                            node.RectTransform.anchoredPosition = FarAway;
                            node.LayoutElement.ignoreLayout = true;
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
            
            if( m_Source != null )
            {
                for( int index = 0, max = m_Source.GetCount(); index < max; ++index )
                {
                    UIContentNode node = GetNodeUsed( index, 0 );
                    if( node == null )
                    {
                        UIContentSource.Param param = GetParam( index );
                        if( param != null )
                        {
                            node = GetNodeEmpty();
                            if( node != null )
                            {
                                node.gameObject.transform.SetSiblingIndex(index);
                                node.Setup( index, index, 0, param );
                                node.Attach( );
                                if (IsFlag(EFlag.LOCAL_CACHE) && node.gameObject.activeSelf)
                                {
                                    node.LayoutElement.ignoreLayout = false;
                                    node.enabled = true;
                                }
                                else
                                {
                                    node.SetActive( true );
                                }
                                m_NodeUsed.Add( GetNodeKey( index, 0 ), node );
                            }
                            else
                            {
                                Debug.LogError( "ノードが不足しています >> " + gameObject.name );
                            }
                        }
                    }
                }
            }
        }
        
        int GetNodeKey( int x, int y )
        {
            return ( ( x & 0xFFFF ) << 16 ) | ( y & 0xFFFF );
        }
        

        UIContentNode GetNodeUsed( int x, int y )
        {
            UIContentNode result = null;
            m_NodeUsed.TryGetValue( GetNodeKey( x, y ), out result );
            return result;
        }
        

        UIContentNode GetNodeEmpty( )
        {
            if( m_NodeEmpty.Count > 0 )
            {
                UIContentNode result = m_NodeEmpty[0];
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
        

        public List<UIContentNode> GetNodeUsedAll( )
        {
            List<UIContentNode> result = ListPool<UIContentNode>.Rent();
            foreach (var pair in m_NodeUsed)
            {
                result.Add( pair.Value );
            }
            return result;
        }
        
  
        public List<UIContentNode> GetNodeChilds( )
        {
            List<UIContentNode> result = new List<UIContentNode>();
            for( int i = 0; i < transform.childCount; ++i )
            {
                Transform child = transform.GetChild(i);
                if( child != null && child.gameObject.activeSelf )
                {
                    UIContentNode node = child.GetComponent<UIContentNode>();
                    if( node != null )
                    {
                        result.Add( node );
                    }
                }
            }
            return result;
        }


        public void AddNode()
        {
            if (m_Node == null)
            {
                Debug.LogError("ベースノードが設定されていません > " + gameObject.name);
                return;
            }

            UIContentNode node = null;
            if (m_Source != null)
            {
                node = m_Source.Instantiate(m_Node);
            }
            else
            {
                GameObject obj = GameObject.Instantiate(m_Node.gameObject);
                if (obj != null)
                {
                    node = obj.GetComponent<UIContentNode>();
                }
            }

            if (node != null)
            {
                node.Initialize(null);
                node.gameObject.transform.SetParent(m_RectTransform, false);
                node.gameObject.SetActive(false);
                m_NodeEmpty.Add(node);
            }
        }
        
        public void RemoveNode(UIContentNode _node)
        {
            IDictionaryEnumerator enumrator = m_NodeUsed.GetEnumerator();
            while (enumrator.MoveNext())
            {
                UIContentNode node = (UIContentNode)enumrator.Value;
                if (node == _node)
                {
                    m_NodeUsed.Remove((int)enumrator.Key);
                    node.Release();
                    if (m_NodeStatic == false) node.gameObject.SafeDestroy();
                    
                    if(m_NodeUsed.Count > 0)
                    {
                        List<UIContentNode> node_list = m_NodeUsed.Values.ToList();
                        m_NodeUsed.Clear();
                        foreach(UIContentNode set_node in node_list)
                        {
                            m_NodeEmpty.Add(set_node);
                        }
                    }
                    break;
                }
            }
        }
        
        public UIContentSource.Param GetParam( int index )
        {
            if( m_Source != null )
            {
                return m_Source.GetParam( index );
            }
            return null;
        }
        

        public UIContentGrid GetGrid()
        {
            if( m_RectTransform != null )
            {
                return GetGrid( AnchoredPosition );
            }
            return UIContentGrid.zero;
        }
        public UIContentGrid GetGrid( Vector2 pos )
        {
            UIContentGrid result = UIContentGrid.zero;
            if( m_RectTransform != null )
            {
                pos.x += m_PaddingLeft - m_Spacing.x;
                result.fx = ( -pos.x / ( m_CellSize.x + m_Spacing.x ) );
                pos.y -= m_PaddingTop - m_Spacing.y;
                result.fy = ( pos.y / ( m_CellSize.y + m_Spacing.y ) );
            }
            return result;
        }
        public UIContentGrid GetGrid( int index )
        {
            UIContentGrid result = UIContentGrid.zero;
            {
                result.x = 0;
                result.y = 0;
                
                if( IsHorizontal )
                {
                    result.x = index;
                }
                else if( IsVertical )
                {
                    result.y = index;
                }
            }
            return result;
        }
        
        public Vector2 GetAnchorePosFromGrid( int x, int y )
        {
            Vector2 result = Vector2.zero;
            if( IsHorizontal )
            {
                if( x >= 0 )
                {
                    result.x = -( x * ( m_CellSize.x + m_Spacing.x ) ) - m_PaddingLeft;
                }
            }
            if( IsVertical )
            {
                if( y >= 0 )
                {
                    result.y = y * ( m_CellSize.y + m_Spacing.y ) + m_PaddingTop;
                }
            }
            return result;
        }
        
        public UIContentNode GetNode( Vector2 screenPos )
        {
          
            return null;
        }
        
        public bool IsFlag( EFlag value )
        {
            return ( m_Flag & (1 << (int)value) ) != 0;
        }

        public void SetCurrentSource( UIContentSource source )
        {
            if( m_Source != null )
            {
                m_Source.Release();
            }
            
            m_Source = source;
            
            if( m_Source != null )
            {
                m_Source.Initialize( null );
            }
            
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
        
    }
    
    #if UNITY_EDITOR
    
    [CustomEditor( typeof(UIContentLayout) )]
    public class EditorInspector_ContentLayout : Editor
    {
        SerializedProperty  m_Node          = null;
        
        public void OnEnable()
        {
            m_Node = serializedObject.FindProperty( "m_Node" );
        }
        
        public override void OnInspectorGUI()
        {
            CustomFieldAttribute.OnInspectorGUI( target.GetType( ), serializedObject );
        }
    }
    
    #endif
}


