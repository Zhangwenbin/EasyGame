using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using GFSYS;

namespace EG
{
    public class UIContentListTemplate
    {
        public abstract class ItemAccessorBase
        {
            UIContentNode       m_Node      = null;
            
            public UIContentNode Node { get { return m_Node; } }

            public abstract bool IsValid { get; }

            public ItemAccessorBase()
            {
            }
            

            //public void Initialize( StatusBase statusParam, StatusParamDisplay.EParamType paramType )
            //{
            //}
            
  
            public void Release()
            {
                Clear();
                m_Node = null;
            }
            
  
            public void Bind( UIContentNode node )
            {
                m_Node = node;

                OnBind();
            }
            
   
            protected virtual void OnBind()
            {
            
            }
            

            public void Clear()
            {
                OnClear();
                
                if( m_Node != null )
                {
                    m_Node.SetParam( null );
                    m_Node = null;
                }
            }
            
            protected virtual void OnClear()
            {
            }
            

            public virtual void ForceUpdate()
            {
                if( m_Node != null )
                {
                    
                }
            }
            

            public virtual void LateUpdate()
            {
            }
            

            
            protected SerializeValueList GetValueList()
            {
                if( Node == null )
                    return null;
                
                return GetValueList( Node.gameObject );
            }
            
            protected SerializeValueList GetValueList( GameObject gobj )
            {
                if( gobj == null )
                    return null;
                
                SerializeValueBehaviour valueBehaviour = gobj.GetComponent<SerializeValueBehaviour>();
                if( valueBehaviour == null )
                    return null;
                
                return valueBehaviour.list;
            }
            
        }
        

        public class ItemParameterBase : UIContentSource.Param
        {
            ItemAccessorBase m_Accessor = null;
            
            public ItemAccessorBase Accessor
            {
                get { return m_Accessor; }
                protected set { m_Accessor = value; }
            }
            
            public T GetAccessor<T>() where T : ItemAccessorBase
            {
                return m_Accessor as T;
            }
            
            protected void CreateAccessor<T>() where T : ItemAccessorBase, new ()
            {
                m_Accessor = new T();
            }
            
            public override void Release()
            {
                m_Accessor?.Release();
            }
            

            public override void LateUpdate()
            {
                m_Accessor?.LateUpdate();
            }
            

            public override void OnEnable( UIContentNode node )
            {
                base.OnEnable( node );
                if( m_Accessor != null )
                {
                    m_Accessor.Bind( node );
                    m_Accessor.ForceUpdate();
                }
            }
            

            public override void OnDisable( UIContentNode node )
            {
                m_Accessor?.Clear();
                
                base.OnDisable( node );
            }
            
   
            public override bool IsValid()
            {
                return m_Accessor?.IsValid ?? false;
            }
        }
        
        
        public class ItemSourceBase : UIContentSource
        {
            List<ItemParameterBase>     m_Params            = new List<ItemParameterBase>();
            Vector2                     m_BaseAnchoredPos   = Vector2.zero;
            Vector2                     m_AnchorPos         = Vector2.zero;
            int                         m_Focus             = -1;
            
            Vector2                     m_FocusLerpSrc      = Vector2.zero;
            Vector2                     m_FocusLerpDst      = Vector2.zero;
            float                       m_FocusLerpTime     = 0f;
            float                       m_FocusLerpElapse   = 0f;
            
            private UIContentLayout     m_ContentLayout     = null;
            
            protected List<ItemParameterBase> Params
            {
                get { return m_Params; }
            }
            
            public int ParamCount
            {
                get { return m_Params.Count; }
            }
            
            public UIContentLayout      ContentLayout
            {
                get { return m_ContentLayout; }
            }
            
            public override void Initialize( UIContentController controller )
            {
                base.Initialize( controller );
                
                Setup();
            }
            
            public override void Release()
            {
                m_Params.Clear();
                
                // ----------------------------------------
                
                base.Release();
            }
            
 
            public override void Update()
            {
                if( m_FocusLerpElapse < m_FocusLerpTime )
                {
                    m_FocusLerpElapse += TimerManager.DeltaTime;
                    _UpdateFocus();
                }
            }
            

            public void Rebuild( )
            {
                for( int i = 0; i < m_Params.Count; ++i )
                {
                    m_Params[i].Rebuild( );
                }
            }
            

            public int Add( ItemParameterBase param )
            {
                m_Params.Add( param );
                return m_Params.Count;
            }
            

            public void AnchorPos( Vector2 pos )
            {
                m_AnchorPos = pos;
            }
            

            public void FocusIndex( int idx )
            {
                m_Focus = idx;
            }
            

            public void Setup()
            {
                if( ContentLayout != null )
                {
                    RectTransform rect = ContentLayout.transform as RectTransform;
                    if( rect != null )
                    {
                        m_BaseAnchoredPos = rect.anchoredPosition;
                    }
                }
                
                Clear();
                
                SetTable( m_Params.ToArray() );
                
                if( ContentController != null )
                {
                    _SetupContentController();
                }
                else if( ContentLayout != null )
                {
                    _SetupContentLayout();
                }
            }
            
  
            void _SetupContentController()
            {
                ContentController.Resize();
                
                FocusImmediate( m_Focus );
                
                {
                    bool isreset = false;
                    Vector2 pos = ContentController.AnchoredPosition;
                    Vector2 lastpos = ContentController.GetLastPageAnchorePos();
                    if( pos.x < lastpos.x )
                    {
                        isreset = true;
                        pos.x = lastpos.x;
                    }
                    if( pos.y > lastpos.y )
                    {
                        isreset = true;
                        pos.y = lastpos.y;
                    }
                    if( isreset )
                    {
                        ContentController.AnchoredPosition = pos;
                    }
                }
                
                if( ContentController.Scroller != null )
                {
                    ContentController.Scroller.StopMovement( );
                }
                
                m_AnchorPos = Vector2.zero;
                m_Focus = -1;
            }
            

            void _SetupContentLayout()
            {
                //ContentLayout.Resize();
                
                if( ContentLayout != null )
                {
                    FocusImmediate( m_Focus );
                }
                
                //{
                //    bool isreset = false;
                //    Vector2 pos = ContentLayout.AnchoredPosition;
                //    Vector2 lastpos = ContentLayout.GetLastPageAnchorePos();
                //    if( pos.x < lastpos.x )
                //    {
                //        isreset = true;
                //        pos.x = lastpos.x;
                //    }
                //    if( pos.y > lastpos.y )
                //    {
                //        isreset = true;
                //        pos.y = lastpos.y;
                //    }
                //    if( isreset )
                //    {
                //        ContentController.AnchoredPosition = pos;
                //    }
                //}
                
                //if( ContentLayout.Scroller != null )
                //{
                //    ContentLayout.Scroller.StopMovement();
                //}
                
                //m_AnchorPos = Vector2.zero;
                m_Focus = -1;
            }
            
            
            public void FocusLerp( int idx, float lerpTime, bool over_index = false )
            {
                m_FocusLerpElapse = 0;
                m_FocusLerpTime   = lerpTime;
                
                if( ContentController != null )
                {
                    m_FocusLerpSrc  =
                    m_FocusLerpDst  = ContentController.AnchoredPosition;
                    m_FocusLerpDst  = _GetFocusPosContentController( idx, over_index );
                }
                else if( ContentLayout != null )
                {
                    m_FocusLerpSrc  =
                    m_FocusLerpDst  = ContentLayout.AnchoredPosition;
                    m_FocusLerpDst  = _GetFocusPosContentLayout( idx );
                }
                
                _UpdateFocus();
            }
            
            public void FocusImmediate( int idx )
            {
                FocusLerp( idx, 0 );
            }
            
            void _UpdateFocus()
            {
                Vector2 focusPos = m_FocusLerpDst;
                if( m_FocusLerpTime > 0 )
                {
                    focusPos = Vector2.Lerp( m_FocusLerpSrc, m_FocusLerpDst, Mathf.Clamp01( m_FocusLerpElapse / m_FocusLerpTime ) );
                }
                
                if( ContentController != null )
                {
                    ContentController.AnchoredPosition = focusPos;
                }
                else if( ContentLayout != null )
                {
                    ContentLayout.AnchoredPosition = focusPos;
                }
            }
            
            Vector2 _GetFocusPosContentController( int idx, bool over_index = false )
            {
                if( (idx <= 0 || idx > m_Params.Count) && !over_index)
                    return m_AnchorPos;

                int param_index = idx;
                int table_count = GetCount();
                if (over_index)
                {
                    if (idx >= 0)
                    {
                        param_index = idx % table_count;
                    }
                    else
                    {
                        param_index = table_count - Mathf.Abs(idx) % table_count;
                        if (param_index == table_count)
                        {
                            param_index = 0;
                        }
                    }
                }

                Vector2 pos = Vector2.zero;
                Param param = GetParam( param_index );
                if( param != null )
                {
                    UIContentGrid grid = ContentController.GetGrid( param.Id );
                    pos = ContentController.GetAnchorePosFromGrid( grid.x, grid.y );

                    if(over_index)
                    {
                        Vector2 _pos      = Vector2.zero;
                        Vector2 _prev_pos = Vector2.zero;
                        if (GetCount() >= 2)
                        {
                            Param         _param = GetParam(0);
                            UIContentGrid _grid  = ContentController.GetGrid(_param.Id);
                            Vector2       pos_1  = ContentController.GetAnchorePosFromGrid(_grid.x, _grid.y);

                            _param = GetParam(1);
                            _grid = ContentController.GetGrid(_param.Id);
                            Vector2       pos_2  = ContentController.GetAnchorePosFromGrid(_grid.x, _grid.y);

                            _pos = pos_2 - pos_1;
                            _pos *= GetCount();
                        }

                        int pow_index = 0;
                        if(idx < 0)
                        {
                            pow_index = Math.Abs(idx+1) / GetCount();
                            pos.x += ContentController.GetSpacing().x + _pos.x * (pow_index+1) * -1;
                            pos.y -= ContentController.GetSpacing().y + _pos.y * (pow_index+1) * -1;
                        }
                        else
                        {
                            pow_index = Math.Abs(idx) / GetCount();
                            pos.x += ContentController.GetSpacing().x + _pos.x * pow_index;
                            pos.y -= ContentController.GetSpacing().y + _pos.y * pow_index;
                        }
                    }
                    else
                    {
                        pos.x += ContentController.GetSpacing().x;
                        pos.y -= ContentController.GetSpacing().y;
                    }
                }
                
                return pos;
            }
            
       
            Vector2 _GetFocusPosContentLayout( int idx )
            {
                if( idx <= 0 || idx > m_Params.Count )
                    return m_AnchorPos + m_BaseAnchoredPos;
                
                Vector2 pos = Vector2.zero;
                
                Param param = GetParam( idx );
                if( param != null )
                {
                    UIContentGrid grid = ContentLayout.GetGrid( param.Id );
                    pos = ContentLayout.GetAnchorePosFromGrid( grid.x, grid.y );
                    pos.x += ContentLayout.GetSpacing().x;
                    pos.y -= ContentLayout.GetSpacing().y;
                }
                
                return pos + m_BaseAnchoredPos;
            }
            
     
            public void SetContentLayout( UIContentLayout layout )
            {
                m_ContentLayout = layout;
            }
        }
    }
}
