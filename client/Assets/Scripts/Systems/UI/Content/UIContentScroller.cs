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

    [AddComponentMenu("Scripts/System/UI/Content/UIContentScroller")]
    public class UIContentScroller : ScrollRect
    {

        public  float            PAGE_SCROLL_TIME      = 0.3f;        
        
        public enum EFlag
        {
            ScrollCheckViewActive   ,  
        }
        
        public enum EScrollPosition
        {
            StartTop        ,       // 初期位置 TOP
            StartBottom     ,       // 初期位置 BOTTOM
            StartLeft       ,       // 初期位置 LEFT
            StartRight      ,       // 初期位置 RIGHT
            StartPosH       ,       // 初期位置 PG任意(横)
            StartPosV       ,       // 初期位置 PG任意(縦)
            FixTop          ,       // 固定 TOP
            FixBottom       ,       // 固定 BOTTOM
            FixLeft         ,       // 固定 LEFT
            FixRight        ,       // 固定 RIGHT
            StartCustomH    ,       // Start at Custom position for x
            StartCustomV    ,       // Start at Custom position for y
        }
        
        #if UNITY_EDITOR
        public static Utility.EnumArray.Element[] EFlagNames =
        {
            new Utility.EnumArray.Element( EFlag.ScrollCheckViewActive      , "ビューが非アクティブ中はスクロールを受け付けない" ),
        };
        public static Utility.EnumArray.Element[] EScrollPositionNames =
        {
            new Utility.EnumArray.Element( EScrollPosition.StartTop         , "初期位置 TOP" ),
            new Utility.EnumArray.Element( EScrollPosition.StartBottom      , "初期位置 BOTTOM" ),
            new Utility.EnumArray.Element( EScrollPosition.StartLeft        , "初期位置 LEFT" ),
            new Utility.EnumArray.Element( EScrollPosition.StartRight       , "初期位置 RIGHT" ),
            new Utility.EnumArray.Element( EScrollPosition.StartPosH        , "初期位置 PG任意(横)" ),
            new Utility.EnumArray.Element( EScrollPosition.StartPosV        , "初期位置 PG任意(縦)" ),
            new Utility.EnumArray.Element( EScrollPosition.FixTop           , "固定 TOP" ),
            new Utility.EnumArray.Element( EScrollPosition.FixBottom        , "固定 BOTTOM" ),
            new Utility.EnumArray.Element( EScrollPosition.FixLeft          , "固定 LEFT" ),
            new Utility.EnumArray.Element( EScrollPosition.FixRight         , "固定 RIGHT" ),
            new Utility.EnumArray.Element( EScrollPosition.StartCustomH     , "Start Custom Horizontal" ),
            new Utility.EnumArray.Element( EScrollPosition.StartCustomV     , "Start Custom Vertical" ),
        };
        #endif
        
        
        // [CustomFieldAttribute("フラグ",CustomFieldAttribute.Type.BitFlag,typeof(EFlag),typeof(UIContentScroller))]
        // public BitFlag              Flag                = new BitFlag( );
        
        [CustomFieldAttribute("スクロール制御",CustomFieldAttribute.Type.BitFlag,typeof(EScrollPosition),typeof(UIContentScroller))]
        [UnityEngine.Serialization.FormerlySerializedAs("m_ScrollPosition")]
        public BitFlag              ScrollPosition      = new BitFlag( );
        
        [CustomFieldAttribute("ページ吸着",CustomFieldAttribute.Type.Bool)]
        public bool                 IsAdsorption        = false;
        
        [CustomFieldAttribute("ページトグル",CustomFieldAttribute.Type.Component,typeof(ToggleGroup))]
        [CustomFieldDispCond("IsAdsorption","True",true)]
        public ToggleGroup          PageToggleGroup     = null;
        
        public Selectable           PrevPageButton      = null;
        public Selectable           NextPageButton      = null;
        
        public string               PrevPageSE          = "";
        public string               NextPageSE          = "";
        
        private UIContentController m_ContentController = null;
        private RectTransform       m_RectTransform     = null;
        private int                 m_Refresh           = 0;
        private Vector2             m_RefreshPGNormPos  = Vector2.zero;     // EScrollPosition.StartPos で使用する任意位置
        //[CE][LWWW-2824] Hard code in the value for now, variable can't be exposed, to prevent being saved in prefab
        private Vector2             m_CustomNormPos     = new Vector2(0.3f, 0.0f);
        bool                        m_IsScroll          = false;
        float                       m_ScrollElapse      = 0;
        Vector2                     m_ScrollStart       = Vector2.zero;
        Vector2                     m_ScrollEnd         = Vector2.zero;
        
        private Vector2             m_Pos               = Vector2.zero;
        private Vector2             m_PosPrev           = Vector2.zero;
        private Vector2             m_DragMove          = Vector2.zero;
        private Vector2             m_DragMovePow       = Vector2.zero;
        private bool                m_Flick             = false;
        private Vector2             m_FlickMove         = Vector2.zero;
        
        // private ViewBase            m_ViewRoot          = null;
        // private bool                m_HorizontalFlag    = false;
        // private bool                m_VerticalFlag      = false;
        

        public UIContentController ContentController
        {
            get
            {
                if( m_ContentController == null )
                {
                    if( content != null )
                    {
                        m_ContentController = content.GetComponent<UIContentController>();
                    }
                }
                return m_ContentController;
            }
        }
        
        public RectTransform rectTransform
        {
            get
            {
                if( m_RectTransform == null )
                {
                    m_RectTransform = transform as RectTransform;
                }
                return m_RectTransform;
            }
        }
        
        Vector2 ScrollDir
        {
            get { return ( vertical ) ? -Vector2.up : Vector2.right; }
        }
        
        public Action<PointerEventData> OnBeginDragAction   { get; set; } = null;
        public Action<PointerEventData> OnDragAction        { get; set; } = null;
        public Action<PointerEventData> OnEndDragAction     { get; set; } = null;
        
        public bool                     isStartTop          { get { return ScrollPosition.HasValue( (int)EScrollPosition.StartTop ); } }
        public bool                     isStartBottom       { get { return ScrollPosition.HasValue( (int)EScrollPosition.StartBottom ); } }
        public bool                     isStartLeft         { get { return ScrollPosition.HasValue( (int)EScrollPosition.StartLeft ); } }
        public bool                     isStartRight        { get { return ScrollPosition.HasValue( (int)EScrollPosition.StartRight ); } }
        public bool                     isStartPosH         { get { return ScrollPosition.HasValue( (int)EScrollPosition.StartPosH ); } }
        public bool                     isStartPosV         { get { return ScrollPosition.HasValue( (int)EScrollPosition.StartPosV ); } }
        public bool                     isFixTop            { get { return ScrollPosition.HasValue( (int)EScrollPosition.FixTop ); } }
        public bool                     isFixBottom         { get { return ScrollPosition.HasValue( (int)EScrollPosition.FixBottom ); } }
        public bool                     isFixLeft           { get { return ScrollPosition.HasValue( (int)EScrollPosition.FixLeft ); } }
        public bool                     isFixRight          { get { return ScrollPosition.HasValue( (int)EScrollPosition.FixRight ); } }
        public bool                     isStartCustomH      { get { return ScrollPosition.HasValue( (int)EScrollPosition.StartCustomH); } }
        public bool                     isStartCustomV      { get { return ScrollPosition.HasValue( (int)EScrollPosition.StartCustomV); } }

        public bool                     IsScroll            { get { return m_IsScroll; } }
        
        protected override void Awake( )
        {
            base.Awake( );

            if( content != null )
            {
                m_ContentController = content.GetComponent<UIContentController>();
            }
            
            if( PrevPageButton != null )
            {
                var btn = PrevPageButton as Button;
                if( btn != null )
                {
                    btn.onClick.AddListener( () => { ScrollPrevPage( 1.0f ); } );
                }
            }
            
            if( NextPageButton != null )
            {
                var btn = NextPageButton as Button;
                if( btn != null )
                {
                    btn.onClick.AddListener( () => { ScrollNextPage( 1.0f ); } );
                }
            }
            
            // m_ViewRoot = gameObject.GetComponentInParent<ViewBase>( );
            // m_HorizontalFlag = this.horizontal;
            // m_VerticalFlag = this.vertical;
        }
        
        protected override void Start( )
        {
            base.Start( );
            
            if( ScrollPosition.flag > 0 )
            {
                Refresh();
            }
        }
        
        protected void Update( )
        {
            if( m_IsScroll )
            {
                m_ScrollElapse += Time.deltaTime;
                
                float rate = 1.0f;
                if( PAGE_SCROLL_TIME > 0 )
                {
                    rate = Mathf.Clamp01( m_ScrollElapse / PAGE_SCROLL_TIME );
                    rate = Mathf.Sin( rate * Mathf.PI * 0.5f );
                }

                SetAnchoreNormalizePos( Vector2.Lerp( m_ScrollStart, m_ScrollEnd, rate ) );
                if( rate >= 1.0f )
                {
                    m_IsScroll = false;
                }
            }
        }
        
        protected override void LateUpdate( )
        {
            base.LateUpdate( );

            if (Application.isPlaying == false)
                return;

            if( m_Refresh > 0 )
            {
                if( isStartTop )
                {
                    this.verticalNormalizedPosition = 1;
                }
                else if( isStartBottom )
                {
                    this.verticalNormalizedPosition = 0;
                }
                if( isStartLeft )
                {
                    this.horizontalNormalizedPosition = 0;
                }
                else if( isStartRight )
                {
                    this.horizontalNormalizedPosition = 1;
                }
                
                if( isStartPosH )
                {
                    this.horizontalNormalizedPosition = m_RefreshPGNormPos.x;
                }
                else if (isStartCustomH)
                {
                    this.horizontalNormalizedPosition = m_CustomNormPos.x;
                }
                if ( isStartPosV )
                {
                    this.verticalNormalizedPosition = m_RefreshPGNormPos.y;
                }
                else if (isStartCustomV)
                {
                    this.verticalNormalizedPosition = m_CustomNormPos.y;
                }

                --m_Refresh;
            }
            
            if( isFixTop )
            {
                this.verticalNormalizedPosition = 1;
            }
            else if( isFixBottom )
            {
                this.verticalNormalizedPosition = 0;
            }
            if( isFixLeft )
            {
                this.horizontalNormalizedPosition = 0;
            }
            else if( isFixRight )
            {
                this.horizontalNormalizedPosition = 1;
            }
            
            if( PrevPageButton != null || NextPageButton != null )
            {
                _UpdatePageButtonInteractable();
            }
            
            {
                m_DragMovePow.x *= 0.5f;
                if( Mathf.Abs( m_DragMovePow.x ) < 1 ) m_DragMovePow.x = 0;
                m_DragMovePow.y *= 0.5f;
                if( Mathf.Abs( m_DragMovePow.y ) < 1 ) m_DragMovePow.y = 0;
                
                if( m_Flick )
                {
                    m_FlickMove.x *= 0.95f;
                    if( Mathf.Abs( m_FlickMove.x ) < 1 ) m_FlickMove.x = 0;
                    m_FlickMove.y *= 0.95f;
                    if( Mathf.Abs( m_FlickMove.y ) < 1 ) m_FlickMove.y = 0;
                    
                    if( m_FlickMove.magnitude < 0.1f )
                    {
                        m_Flick = false;
                    }
                }
            }
            
            if( IsAdsorption )
            {
                StopMovement( );
            }
            
            // if( Flag.HasValue( (int)EFlag.ScrollCheckViewActive ) )
            // {
            //     if( m_ViewRoot ==null || m_ViewRoot.isActive )
            //     {
            //         if( this.horizontalScrollbar != null ) this.horizontalScrollbar.interactable = m_HorizontalFlag;
            //         if( this.verticalScrollbar != null ) this.verticalScrollbar.interactable = m_VerticalFlag;
            //         this.horizontal = m_HorizontalFlag;
            //         this.vertical = m_VerticalFlag;
            //     }
            //     else
            //     {
            //         if( this.horizontalScrollbar != null ) this.horizontalScrollbar.interactable = false;
            //         if( this.verticalScrollbar != null ) this.verticalScrollbar.interactable = false;
            //         this.horizontal = false;
            //         this.vertical = false;
            //         StopMovement( );
            //     }
            // }
        }
        
        void _UpdatePageButtonInteractable()
        {
            Vector2 scrollDir = horizontal ? ScrollDir : -ScrollDir;

            float scrollPos = Vector2.Dot( normalizedPosition, scrollDir );
            //scrollPos = Mathf.Abs( scrollPos );
            
            if( content != null )
            {
                float scrollSize;
                float contentSize;
                _CalcRectSize( out scrollSize, out contentSize );
                if( horizontal )
                {
                    scrollPos = 1.0f - scrollPos;
                }
                
                if( PrevPageButton != null )
                {
                    PrevPageButton.interactable = scrollPos < 0.999f && scrollSize < contentSize;
                }
                
                if( NextPageButton != null )
                {
                    NextPageButton.interactable = scrollPos > 0.001f && scrollSize < contentSize;
                }
            }
        }
    
        public void ScrollPrevPage( float delta )
        {
            _PlaySEOnTapButton( PrevPageSE );
            
            if( IsAdsorption )
            {
                ScrollAdsorption( true, new Vector2( 1, -1 ) );
            }
            else
            {
                Scroll( -delta );
            }
        }
        
        public void ScrollNextPage( float delta )
        {
            _PlaySEOnTapButton( NextPageSE );
            
            if( IsAdsorption )
            {
                ScrollAdsorption( true, new Vector2( -1, 1 ) );
            }
            else
            {
                Scroll( delta );
            }
        }
        
        public void _PlaySEOnTapButton( string seKey )
        {
            if( m_IsScroll )
                return;
            
            if( string.IsNullOrEmpty( seKey ) )
                return;
            
            // SoundManager.Instance.PlaySEOneShot( seKey );
        }
        
        public void SetScrollPos( float normPos )
        {
            Vector2 dstPos = ScrollDir;
            dstPos.x = Mathf.Abs( dstPos.x ) * normPos;
            dstPos.y = Mathf.Abs( dstPos.y ) * normPos;
            normalizedPosition = dstPos;
            
            m_IsScroll = false;
        }

        
        public void ScrollTo( float normPos )
        {
           m_ScrollStart = normalizedPosition;
           m_ScrollEnd = ScrollDir * normPos;
            
           _StartScroll();
        }
        
        public void Scroll( float delta )
        {
            Vector2 dir = ScrollDir;
            
            float scrollSize;
            float contentSize;
            _CalcRectSize( out scrollSize, out contentSize );
            
            float scrollAreaSize = contentSize - scrollSize;
            if( scrollAreaSize <= 0f )
            {
                return;
            }
            
            float stride = scrollSize * delta;
            float normDeltaSize = stride / scrollAreaSize;
            
            Vector2 normPos = normalizedPosition;
            normPos += dir * normDeltaSize;
            normPos.x = Mathf.Clamp01( normPos.x );
            normPos.y = Mathf.Clamp01( normPos.y );
            
            m_ScrollStart = normalizedPosition;
            m_ScrollEnd   = normPos;
            _StartScroll();
        }
        
        public void ScrollAdsorption( bool isFlick, Vector3 flickMove )
        {
            UIContentController controller = ContentController;
            if( controller == null ) return;
            
            Vector2 pageSize = controller.GetPageSize( );
            Vector2 contentSize = controller.Size;
            
            Vector2 scrollArea = contentSize - pageSize;
            scrollArea.x = Mathf.Max( 0, scrollArea.x );
            scrollArea.y = Mathf.Max( 0, scrollArea.y );
            
            UIContentGrid page = controller.GetPageGrid( pageSize * 0.5f, this );
            if( isFlick )
            {
                Vector2Int lastPage = controller.GetPageNum( );
                // DebugUtility.Log( "FLICK > " + m_FlickMove + ", " + page );
                if( horizontal )
                {
                    if( flickMove.x > 0.0f )
                    {
                        UIContentGrid nextPage = controller.GetPageGrid( new Vector2( -10.0f, 0.0f ), this );
                        page.x = nextPage.x;
                        if( page.x < 0 ) page.x = 0;
                    }
                    else
                    {
                        UIContentGrid nextPage = controller.GetPageGrid( new Vector2( 10.0f, 0.0f ), this );
                        page.x = nextPage.x + 1;
                        if( page.x >= lastPage.x ) page.x = lastPage.x - 1;
                    }
                }
                if( vertical )
                {
                    if( flickMove.y < 0.0f )
                    {
                        UIContentGrid nextPage = controller.GetPageGrid( new Vector2( 0.0f, -10.0f ), this );
                        page.y = nextPage.y;
                        if( page.y < 0 ) page.y = 0;
                    }
                    else
                    {
                        UIContentGrid nextPage = controller.GetPageGrid( new Vector2( 0.0f, 10.0f ), this );
                        page.y = nextPage.y + 1;
                        if( page.y >= lastPage.y ) page.y = lastPage.y - 1;
                    }
                }
            }
            else
            {
                // DebugUtility.Log( "STOP > " + page );
            }
            
            m_ScrollStart   = normalizedPosition;
            // m_ScrollEnd     = new Vector2( scrollArea.x > 0 ? pageSize.x * page.x / scrollArea.x: 0, ( scrollArea.y > 0 ? 1 - ( pageSize.y * page.y / scrollArea.y ): 1 ) );
            m_ScrollEnd     = new Vector2( scrollArea.x > 0 ? pageSize.x * page.x / scrollArea.x: 0, scrollArea.y > 0 ? pageSize.y * page.y / scrollArea.y: 0 );
            m_ScrollEnd.x   = Math.Max( 0.0f, Mathf.Min( 1.0f, m_ScrollEnd.x ) );
            m_ScrollEnd.y   = Math.Max( 0.0f, Mathf.Min( 1.0f, m_ScrollEnd.y ) );
            _StartScroll( );
        }
        
        private void _CalcRectSize( out float scrollSize, out float contentSize )
        {
            scrollSize = 0;
            contentSize = 0;
            
            RectTransform scrollRectTrans = transform as RectTransform;
            RectTransform contentRectTrans = content.transform as RectTransform;
            
            scrollSize = Mathf.Abs( Vector2.Dot( scrollRectTrans.rect.size, ScrollDir ) );
            contentSize = Mathf.Abs( Vector2.Dot( contentRectTrans.rect.size, ScrollDir ) );
        }

        
        private void _StartScroll()
        {
            Vector2 div = m_ScrollStart - m_ScrollEnd;
            if( Mathf.Abs( div.x ) > 0.0001f || Mathf.Abs( div.y ) > 0.0001f )
            {
                m_IsScroll = true;
                m_ScrollElapse = 0;
            }
        }
        
       
        public Vector2 GetSize( )
        {
            Vector2 orgSizeDelta = new Vector2( rectTransform.rect.width, rectTransform.rect.height );
            Vector2 sizeDelta = ContentController.rectTransform.sizeDelta - orgSizeDelta;
            if( sizeDelta.x < orgSizeDelta.x ) sizeDelta.x = orgSizeDelta.x;
            if( sizeDelta.y < orgSizeDelta.y ) sizeDelta.y = orgSizeDelta.y;
            return sizeDelta;
        }
        
        public Vector2 GetAnchorePos( )
        {
            return ContentController.GetAnchorePos( );
        }
        
        public void SetAnchoreNormalizePos( Vector2 normalizePos )
        {
            normalizedPosition = normalizePos;
        }
        
        public void Refresh()
        {
            m_Refresh = 2;
        }
        
        public void Refresh( Vector2 normPos )
        {
            Refresh();
            m_RefreshPGNormPos = normPos;
        }
        
        public void SetHorizontalScrollPos( int id )
        {
            Vector2 pos = Vector2.zero;
            
            UIContentGrid grid = m_ContentController.GetGrid( id );
            pos = m_ContentController.GetAnchorePosFromGrid( grid.x, grid.y );
            pos.x += m_ContentController.GetSpacing( ).x;
            pos.y -= m_ContentController.GetSpacing( ).y;
            
            Vector2 lastpos = m_ContentController.GetLastPageAnchorePos();
            if( pos.x > 0 )              pos.x = 0;
            else if( pos.x < lastpos.x ) pos.x = lastpos.x;
            else if( pos.x < 0 )         pos.x = 0;
            if( pos.y < 0 )              pos.y = 0;
            else if( pos.y > lastpos.y ) pos.y = lastpos.y;
            
            StopMovement( );

            if( this.horizontalScrollbar != null )
            {
                float tx = 0.0f;
                
                tx = pos.x / lastpos.x;
                if( this.horizontalScrollbar.direction == Scrollbar.Direction.RightToLeft ) tx = 1 - tx;
                
                ScrollPosition.Clear( );
                ScrollPosition.SetValue( UIContentScroller.EScrollPosition.StartPosH, true );
                
                Refresh( new Vector2( tx, 0 ) );
            }
            else
            {
                m_ContentController.AnchoredPosition = pos;
            }
        }
        
        public void SetHorizontalScrollPos( float x )
        {
            Vector2 pos = Vector2.zero;
            
            pos.x = x;
            
            Vector2 lastpos = m_ContentController.GetLastPageAnchorePos();
            if( pos.x > 0 )              pos.x = 0;
            else if( pos.x < lastpos.x ) pos.x = lastpos.x;
            else if( pos.x < 0 )         pos.x = 0;
            if( pos.y < 0 )              pos.y = 0;
            else if( pos.y > lastpos.y ) pos.y = lastpos.y;
            
            StopMovement( );
            
            if( this.horizontalScrollbar != null )
            {
                float tx = 0.0f;
                
                tx = pos.x / lastpos.x;
                if( this.horizontalScrollbar.direction == Scrollbar.Direction.RightToLeft ) tx = 1 - tx;
                
                ScrollPosition.Clear( );
                ScrollPosition.SetValue( UIContentScroller.EScrollPosition.StartPosH, true );
                
                Refresh( new Vector2( tx, 0 ) );
            }
            else
            {
                m_ContentController.AnchoredPosition = pos;
            }
        }
        
        public void SetVerticalScrollPos( int id )
        {
            if( m_ContentController == null ) return;
            
            Vector2 pos = Vector2.zero;
            
            UIContentGrid grid = m_ContentController.GetGrid( id );
            pos = m_ContentController.GetAnchorePosFromGrid( grid.x, grid.y );
            pos.x += m_ContentController.GetSpacing( ).x;
            pos.y -= m_ContentController.GetSpacing( ).y;
            
            Vector2 lastpos = m_ContentController.GetLastPageAnchorePos();
            if( pos.x > 0 )              pos.x = 0;
            else if( pos.x < lastpos.x ) pos.x = lastpos.x;
            else if( pos.x < 0 )         pos.x = 0;
            if( pos.y < 0 )              pos.y = 0;
            else if( pos.y > lastpos.y ) pos.y = lastpos.y;

            StopMovement( );
            
            if( this.verticalScrollbar != null )
            {
                float ty = 0.0f;
                
                ty = pos.y / lastpos.y;
                if( this.verticalScrollbar.direction == Scrollbar.Direction.BottomToTop ) ty = 1 - ty;
                
                ScrollPosition.Clear( );
                ScrollPosition.SetValue( UIContentScroller.EScrollPosition.StartPosV, true );
                
                Refresh( new Vector2( 0, ty ) );
            }
            else
            {
                m_ContentController.AnchoredPosition = pos;
            }
        }
        
        public void SetVerticalScrollPos( float y )
        {
            Vector2 pos = Vector2.zero;
            
            pos.y = y;
            
            Vector2 lastpos = m_ContentController.GetLastPageAnchorePos();
            if( pos.x > 0 )              pos.x = 0;
            else if( pos.x < lastpos.x ) pos.x = lastpos.x;
            else if( pos.x < 0 )         pos.x = 0;
            if( pos.y < 0 )              pos.y = 0;
            else if( pos.y > lastpos.y ) pos.y = lastpos.y;
            
            StopMovement( );
            
            if( this.verticalScrollbar != null )
            {
                float ty = 0.0f;
                
                ty = pos.y / lastpos.y;
                if( this.verticalScrollbar.direction == Scrollbar.Direction.BottomToTop ) ty = 1 - ty;
                
                ScrollPosition.Clear( );
                ScrollPosition.SetValue( UIContentScroller.EScrollPosition.StartPosV, true );
                
                Refresh( new Vector2( 0, ty ) );
            }
            else
            {
                m_ContentController.AnchoredPosition = pos;
            }
        }
        
        
        public override void OnBeginDrag( PointerEventData eventData )
        {
            base.OnBeginDrag( eventData );
            
            m_Pos = m_PosPrev = eventData.position;
            m_DragMove = Vector2.zero;
            m_DragMovePow = Vector2.zero;
            m_Flick = false;
            m_FlickMove = Vector2.zero;
            
            OnBeginDragAction?.Invoke( eventData );
        }
        
        public override void OnDrag( PointerEventData eventData )
        {
            base.OnDrag( eventData );
            
            m_PosPrev = m_Pos;
            m_Pos = eventData.position;
            m_DragMove = m_Pos - m_PosPrev;
            m_DragMovePow += m_DragMove;
            
            OnDragAction?.Invoke( eventData );
        }
        

        public override void OnEndDrag( PointerEventData eventData )
        {
            base.OnEndDrag( eventData );
            
            if( m_DragMovePow.magnitude > 1.0f )
            {
                m_FlickMove = m_DragMovePow;
                m_Flick = true;
            }
            else
            {
                m_FlickMove = Vector2.zero;
                m_Flick = false;
            }
            
            if( IsAdsorption )
            {
                ScrollAdsorption( m_Flick, m_FlickMove );
            }

            OnEndDragAction?.Invoke( eventData );
        }
        
    }
}
