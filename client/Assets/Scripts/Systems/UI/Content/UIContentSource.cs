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
    public class UIContentSource
    {
        public class Param
        {
            public UIContentNode    Node    = null;
            private int             _id     = -1;
            private int             _idprev = int.MinValue;
            public int          Id { set { _id = value; } get { return _id; } }
            public void         Rebuild( ){ _idprev = -1; }
            public void         Wakeup( UIContentNode node ){ _idprev = Id; Node = node; }
            public void         Sleep( ){ if( Node != null ){ _idprev = int.MinValue; Node.SetParam( null ); Node = null; } }
            public virtual void Initialize( UIContentSource source ){}
            public virtual void Release( ){ }
            public virtual bool IsValid( ){ return true; }
            public virtual bool IsLock( ){ return false;    }
            public virtual bool IsReMake( ){ return Id != _idprev; }
            public virtual void Update( ){}
            public virtual void LateUpdate( ){}
            //public virtual void OnSetup( UIContentNode node ){}
            public virtual void OnAttach( UIContentNode node ){}
            public virtual void OnDetach( UIContentNode node ){}
            public virtual void OnEnable( UIContentNode node ){ }
            public virtual void OnDisable( UIContentNode node ){ }
            public virtual void OnViewIn( UIContentNode node, Vector2 pivotViewPosition ){}
            public virtual void OnViewOut( UIContentNode node, Vector2 pivotViewPosition ){}
            public virtual void OnPageFit( UIContentNode node ){}
            public virtual void OnSelectOn( UIContentNode node ){}
            public virtual void OnSelectOff( UIContentNode node ){}
            public virtual SerializeValueList Invoke( UIContentNode node, string key, SerializeValueList receiveValueList ){ return null; }
            public virtual bool Equal( string value ){ return this.ToString() == value; }
        }
        
        public class ParamAction<T> : Param
        {
            public T obj;
            public System.Action<ParamAction<T>,UIContentSource>            InitializeAction;
            public System.Action<ParamAction<T>>                            ReleaseAction;
            public System.Action<ParamAction<T>>                            UpdateAction;
            public System.Action<ParamAction<T>>                            LateUpdateAction;
            public System.Action<ParamAction<T>,UIContentNode>              SetupAction;
            public System.Action<ParamAction<T>,UIContentNode>              AttachAction;
            public System.Action<ParamAction<T>,UIContentNode>              DetachAction;
            public System.Action<ParamAction<T>,UIContentNode>              EnableAction;
            public System.Action<ParamAction<T>,UIContentNode>              DisableAction;
            public System.Action<ParamAction<T>,UIContentNode,Vector2>      ViewInAction;
            public System.Action<ParamAction<T>,UIContentNode,Vector2>      ViewOutAction;
            public System.Func<ParamAction<T>,UIContentNode,SerializeValueList, SerializeValueList> ClickAction;
            public System.Func<ParamAction<T>,UIContentNode,string,SerializeValueList,SerializeValueList> InvokeAction;
            public override void Initialize( UIContentSource source )   { base.Initialize( source ); if( InitializeAction != null ) InitializeAction.Invoke( this, source );    }
            public override void Release( )                             { if( ReleaseAction != null ) ReleaseAction.Invoke( this ); base.Release( ); }
            public override void Update( )                              { base.Update( ); if( UpdateAction != null ) UpdateAction.Invoke( this ); }
            public override void LateUpdate( )                          { base.LateUpdate( ); if( LateUpdateAction != null ) LateUpdateAction.Invoke( this ); }
            //public override void OnSetup( UIContentNode node )          { base.OnSetup( node ); if( SetupAction != null ) SetupAction.Invoke( this, node ); }
            public override void OnAttach( UIContentNode node )         { base.OnAttach( node ); if( AttachAction != null ) AttachAction.Invoke( this, node ); }
            public override void OnDetach( UIContentNode node )         { if( DetachAction != null ) DetachAction.Invoke( this, node ); base.OnDetach( node ); }
            public override void OnEnable( UIContentNode node )         { base.OnEnable( node ); if( EnableAction != null ) EnableAction.Invoke( this, node ); }
            public override void OnDisable( UIContentNode node )        { if( DisableAction != null ) DisableAction.Invoke( this, node ); base.OnDisable( node ); }
            public override void OnViewIn( UIContentNode node, Vector2 pivotViewPosition ){ if( ViewInAction != null ) ViewInAction.Invoke( this, node, pivotViewPosition ); base.OnViewIn( node, pivotViewPosition ); }
            public override void OnViewOut( UIContentNode node, Vector2 pivotViewPosition ){ if( ViewOutAction != null ) ViewOutAction.Invoke( this, node, pivotViewPosition ); base.OnViewOut( node, pivotViewPosition ); }
            public override SerializeValueList Invoke( UIContentNode node, string key, SerializeValueList receiveValueList ){ if( InvokeAction != null ) return InvokeAction( this, node, key, receiveValueList ); return base.Invoke( node, key, receiveValueList ); }
        }
        
        public class ParamAction<T1,T2> : Param
        {
            public T1 obj;
            public T2 value;
            public System.Action<ParamAction<T1,T2>,UIContentSource>        InitializeAction;
            public System.Action<ParamAction<T1,T2>>                        ReleaseAction;
            public System.Action<ParamAction<T1,T2>>                        UpdateAction;
            public System.Action<ParamAction<T1,T2>>                        LateUpdateAction;
            public System.Action<ParamAction<T1,T2>,UIContentNode>          SetupAction;
            public System.Action<ParamAction<T1,T2>,UIContentNode>          AttachAction;
            public System.Action<ParamAction<T1,T2>,UIContentNode>          DetachAction;
            public System.Action<ParamAction<T1,T2>,UIContentNode>          EnableAction;
            public System.Action<ParamAction<T1,T2>,UIContentNode>          DisableAction;
            public System.Action<ParamAction<T1,T2>,UIContentNode,Vector2>  ViewInAction;
            public System.Action<ParamAction<T1,T2>,UIContentNode,Vector2>  ViewOutAction;
            public System.Func<ParamAction<T1,T2>,UIContentNode,string,SerializeValueList,SerializeValueList> InvokeAction;
            public override void Initialize( UIContentSource source )   { base.Initialize( source ); if( InitializeAction != null ) InitializeAction.Invoke( this, source );    }
            public override void Release( )                             { if( ReleaseAction != null ) ReleaseAction.Invoke( this ); base.Release( ); }
            public override void Update( )                              { base.Update( ); if( UpdateAction != null ) UpdateAction.Invoke( this ); }
            public override void LateUpdate( )                          { base.LateUpdate( ); if( LateUpdateAction != null ) LateUpdateAction.Invoke( this ); }
            public override void OnAttach( UIContentNode node )         { base.OnAttach( node ); if( AttachAction != null ) AttachAction.Invoke( this, node ); }
            public override void OnDetach( UIContentNode node )         { if( DetachAction != null ) DetachAction.Invoke( this, node ); base.OnDetach( node ); }
            public override void OnEnable( UIContentNode node )         { base.OnEnable( node ); if( EnableAction != null ) EnableAction.Invoke( this, node ); }
            public override void OnDisable( UIContentNode node )        { if( DisableAction != null ) DisableAction.Invoke( this, node ); base.OnDisable( node ); }
            public override void OnViewIn( UIContentNode node, Vector2 pivotViewPosition ){ if( ViewInAction != null ) ViewInAction.Invoke( this, node, pivotViewPosition ); base.OnViewIn( node, pivotViewPosition ); }
            public override void OnViewOut( UIContentNode node, Vector2 pivotViewPosition ){ if( ViewOutAction != null ) ViewOutAction.Invoke( this, node, pivotViewPosition ); base.OnViewOut( node, pivotViewPosition ); }
            public override SerializeValueList Invoke( UIContentNode node, string key, SerializeValueList receiveValueList ){ if( InvokeAction != null ) return InvokeAction( this, node, key, receiveValueList ); return base.Invoke( node, key, receiveValueList ); }
        }
        
  
        public System.Action<UIContentSource>   InitializeAction;
        public System.Action<UIContentSource>   ReleaseAction;
        
        private Param[]                         m_Table                 = null;
        private UIContentController             m_ContentController     = null;
        
        private bool                            m_ForcusFlag            = false;
        private Vector2                         m_ForcusAnchorePos      = Vector2.zero;
        private int                             m_ForcusIndex           = -1;
        
 
        public UIContentController              ContentController       { get { return m_ContentController; } }
        
        
        public virtual void Initialize( UIContentController controller )
        {
            m_ContentController = controller;
            
            for( int i = 0, max = GetCount(); i < max; ++i )
            {
                Param param = GetParam( i );
                if( param != null )
                {
                    param.Initialize( this );
                }
            }
            
            if( InitializeAction != null )
            {
                InitializeAction( this );
            }
        }
        
        public virtual void Release( )
        {
            if( ReleaseAction != null )
            {
                ReleaseAction( this );
            }
            
            Clear( );
        }
        
        public virtual void Clear( )
        {
            for( int i = 0, max = GetCount(); i < max; ++i )
            {
                Param param = GetParam( i );
                if( param != null )
                {
                    if( param.Node != null )
                    {
                        param.Node.Detach( );
                        param.OnDisable( param.Node );
                        param.Sleep( );
                    }
                    param.Release();
                }
            }
            m_Table = null;
        }
        
        public virtual void Update()
        {
        }
        
        public virtual void ForceUpdate()
        {
            for( int i = 0, max = GetCount(); i < max; ++i )
            {
                Param param = GetParam( i );
                if( param != null )
                {
                    param.Update( );
                }
            }
        }
        
        public virtual void ForceLateUpdate()
        {
            for( int i = 0, max = GetCount(); i < max; ++i )
            {
                Param param = GetParam( i );
                if( param != null )
                {
                    param.LateUpdate( );
                }
            }
        }
        
        public virtual void UpdateForcus( )
        {
            if( m_ForcusFlag == false ) return;
            
            if( m_ForcusIndex != -1 )
            {
                if( ContentController.isScrollHorizontal )
                {
                    ContentController.Scroller.SetHorizontalScrollPos( m_ForcusIndex );
                }
                if( ContentController.isScrollVertical )
                {
                    ContentController.Scroller.SetVerticalScrollPos( m_ForcusIndex );
                }
            }
            else
            {
                if( ContentController.isScrollHorizontal )
                {
                    ContentController.Scroller.SetHorizontalScrollPos( m_ForcusAnchorePos.x );
                }
                if( ContentController.isScrollVertical )
                {
                    ContentController.Scroller.SetVerticalScrollPos( m_ForcusAnchorePos.y );
                }
            }
            
            if( ContentController.Scroller != null )
            {
                ContentController.Scroller.StopMovement( );
            }
            
            m_ForcusFlag = false;
            m_ForcusAnchorePos = Vector2.zero;
            m_ForcusIndex = -1;
        }
        
        public virtual UIContentNode Instantiate( UIContentNode res )
        {
            if( res == null ) return null;
            GameObject gobj = GameObject.Instantiate( res.gameObject );
            if( gobj != null )
            {
                return gobj.GetComponent<UIContentNode>();
            }
            return null;
        }
        
        public virtual UIContentNode Instantiate( UIContentNode res, RectTransform parent, bool instantiateInWorldSpace )
        {
            if( res == null ) return null;
            GameObject gobj = GameObject.Instantiate( res.gameObject, parent, instantiateInWorldSpace );
            if( gobj != null )
            {
                return gobj.GetComponent<UIContentNode>();
            }
            return null;
        }
        
        public void SetTable( Param[] values )
        {
            Clear( );
            
            if( values != null )
            {
                for( int i = 0; i < values.Length; ++i )
                {
                    values[i].Id = i;
                }
                m_Table = values;
            }
            else
            {
                m_Table = null;
            }
        }
        
        public virtual Param GetParam( int index )
        {
            if( m_Table != null )
            {
                if( index >= 0 && index < m_Table.Length )
                {
                    return m_Table[ index ];
                }
            }
            return null;
        }
        public virtual T GetParam<T>( int index ) where T : Param
        {
            if( m_Table != null )
            {
                if( index >= 0 && index < m_Table.Length )
                {
                    return m_Table[ index ] as T;
                }
            }
            return null;
        }
        
        public virtual Param GetParam( string value )
        {
            if( m_Table != null )
            {
                for( int i = 0; i < m_Table.Length; ++i )
                {
                    Param param = m_Table[ i ];
                    if( param != null )
                    {
                        if( param.Equal( value ) )
                        {
                            return param;
                        }
                    }
                }
            }
            return null;
        }
        public virtual T GetParam<T>( string value ) where T : Param
        {
            if( m_Table != null )
            {
                for( int i = 0; i < m_Table.Length; ++i )
                {
                    T param = m_Table[ i ] as T;
                    if( param != null )
                    {
                        if( param.Equal( value ) )
                        {
                            return param;
                        }
                    }
                }
            }
            return null;
        }
        
        public virtual Param GetParam( System.Func<Param,bool> equal )
        {
            if( m_Table != null )
            {
                for( int i = 0; i < m_Table.Length; ++i )
                {
                    Param param = m_Table[ i ];
                    if( param != null )
                    {
                        if( equal( param ) )
                        {
                            return param;
                        }
                    }
                }
            }
            return null;
        }
        public virtual T GetParam<T>( System.Func<T,bool> equal ) where T : Param
        {
            if( m_Table != null )
            {
                for( int i = 0; i < m_Table.Length; ++i )
                {
                    T param = m_Table[ i ] as T;
                    if( param != null )
                    {
                        if( equal( param ) )
                        {
                            return param;
                        }
                    }
                }
            }
            return null;
        }
        
        public virtual int GetCount()
        {
            if( m_Table != null )
            {
                return m_Table.Length;
            }
            return 0;
        }
        
        public void AddValueTable(Param value)
        {
            if (m_Table != null && value != null)
            {
                List<Param> list = new List<Param>(m_Table);
                list.Add(value);
                m_Table = list.ToArray();
            }
        }
        
        public void RemoveValueTable(Param value)
        {
            if (m_Table != null && value != null)
            {
                List<Param> list = new List<Param>(m_Table);
                list.Remove(value);
                m_Table = list.ToArray();
            }
        }
        
        public UIContentController GetContentController( )
        {
            return m_ContentController;
        }
        
        public virtual void SetForcusIndex( int index )
        {
            m_ForcusFlag = true;
            m_ForcusIndex = index;
        }
        
        public virtual void SetForcusAnchorePos( Vector2 pos )
        {
            m_ForcusFlag = true;
            m_ForcusAnchorePos = pos;
        }
        
        public virtual void OnEnable( )
        {
            UpdateForcus( );
        }
        
        public virtual void OnDisable( )
        {
        }
    }
}
