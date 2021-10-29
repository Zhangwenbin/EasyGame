using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

namespace EG
{
    public struct UIContentGrid
    {
        public static UIContentGrid  zero { get { return new UIContentGrid( 0, 0 ); } }
        
        public int x;
        public int y;
        
        public float fx { set { x = FloatToInt( value ); } }
        public float fy { set { y = FloatToInt( value ); } }
        
        public UIContentGrid( int _ix, int _iy )
        {
            x = _ix;
            y = _iy;
        }
        public UIContentGrid( float _fx, float _fy )
        {
            x = FloatToInt( _fx );
            y = FloatToInt( _fy );
        }
        
        public static int FloatToInt( float value )
        {
            return Mathf.FloorToInt( value );
        }
        
        public override string ToString()
        {
            return string.Format("[Grid: " + x + ", " + y + "]" );
        }
    }
    
    [AddComponentMenu("Scripts/System/UI/Content/UIContentNode")]
    public class UIContentNode : AppMonoBehaviour
    {
        public enum EventType
        {
            SETUP                   ,
            ENABLE                  ,
            DISABLE                 ,
        }
        
        static string[]             s_NameCache;
        [CustomFieldAttribute("タイプ",CustomFieldAttribute.Type.String)]
        public string               nodeType                = "";
        
        int                         m_HashCode              = -1;
        RectTransform               m_RectTransform         = null;
        UIContentController         m_UIContentController   = null;
        
        int                         m_Index                 = -1;
        int                         m_ParamHash             = 0;
        UIContentSource.Param       m_Param                 = null;
        UIContentGrid               m_Grid                  = UIContentGrid.zero;
        Vector2                     m_Pos                   = Vector2.zero;
        
        bool                        m_ViewIn                = false;

        LayoutElement               m_LayoutElement         = null;
        
        
        public RectTransform        RectTransform
        {
            get
            {
                if( m_RectTransform == null )
                {
                    m_RectTransform = gameObject.GetComponent<RectTransform>();
                }
                return m_RectTransform;
            }
        }

        public LayoutElement LayoutElement
        {
            get
            {
                return m_LayoutElement ?? (m_LayoutElement = gameObject.RequireComponent<LayoutElement>());
            }
        }
        
        public int                  nodeHash            { get { if( m_HashCode == -1 ) m_HashCode = nodeType.GetHashCode( ); return m_HashCode; } }
        public int                  paramHash           { set { m_ParamHash = value; } get { return m_ParamHash; } }
        
        public UIContentController  ContentController   { get { return m_UIContentController;            } }
        
        public int                  Index               { get { return m_Index;                        } }
        
        public float                SizeX               { get { return RectTransform.sizeDelta.x;    } }
        public float                SizeY               { get { return RectTransform.sizeDelta.y;    } }
        
        public float                PosX                { get { return m_Pos.x;                        } }
        public float                PosY                { get { return m_Pos.y;                        } }
        
        public int                  GridX               { get { return m_Grid.x;                    } }
        public int                  GridY               { get { return m_Grid.y;                    } }
        
        private void Awake( )
        {
            
        }
        
        public virtual void Initialize( UIContentController controller )
        {
            base.Initialize( );
            
            // ----------------------------------------
            
            m_UIContentController = controller;
            
            RectTransform transform = RectTransform;
            if( transform != null )
            {
                transform.anchorMin = new Vector2( 0.0f, 1.0f );
                transform.anchorMax = new Vector2( 0.0f, 1.0f );
            }
            
            m_ViewIn = false;
        }
        
        public override void Release()
        {
            Detach( );
            
            m_UIContentController = null;
             

            base.Release( );
        }
        

        public virtual void Copy( UIContentNode src )
        {
            m_UIContentController = src.m_UIContentController;
            m_Index             = src.m_Index;
            m_Param             = src.m_Param;
            m_Grid              = src.m_Grid;
            m_Pos               = src.m_Pos;
        }
        

        public virtual void Update()
        {
            if( m_Param != null )
            {
                m_Param.Update( );
            }
        }
        
   
        public virtual void LateUpdate()
        {
            if( m_Param != null )
            {
                m_Param.LateUpdate( );
            }
        }
        
     
        public virtual void Setup( int index, Vector2 pos, UIContentSource.Param param )
        {
            m_Index         = index;
            m_Param         = param;
            m_Pos           = pos;

            s_NameCache = s_NameCache ?? new string[(m_Index + 1) * 2];
            if (m_Index >= s_NameCache.Length)
            {
                System.Array.Resize(ref s_NameCache, m_Index * 2);
            }
            var nodeName = s_NameCache[m_Index];
            if (nodeName == null)
            {
                nodeName = "Node_" + m_Index;
                s_NameCache[m_Index] = nodeName;
            }
            name = nodeName;
            
            if( m_UIContentController != null )
            {
                RectTransform.anchoredPosition = PosToLocalPos( m_Pos );
            }
            
            m_ViewIn = false;
            
        }
        

        public virtual void Setup( int index, int x, int y, UIContentSource.Param param )
        {
     
            m_Grid = new UIContentGrid( x, y );
            
            Vector2 nodePos = Vector2.zero;
            if( m_UIContentController != null )
            {
                nodePos = m_UIContentController.GetNodePos( x, y );
            }
            Setup( index, nodePos, param );
        }
        
        public virtual void Attach( )
        {
            if( m_Param != null )
            {
                m_Param.OnAttach( this );
            }
        }
        
    
        public virtual void Detach( )
        {
            OnSelectOff( );
            
            if( m_Param != null )
            {
                m_Param.OnDetach( this );
            }
        }
        

        public void SetActive( bool value )
        {
            gameObject.SetActive( value );
        }
        

        public void SetParam( UIContentSource.Param param )
        {
            m_Param = param;
        }
        

        public UIContentSource.Param GetParam( )
        {
            return m_Param;
        }
        public T GetParam<T>( ) where T : UIContentSource.Param
        {
            return m_Param as T;
        }
        

        public void SetGrid( int x, int y )
        {
            m_Grid.x = x;
            m_Grid.y = y;
        }
        public void SetGrid( UIContentGrid grid )
        {
            m_Grid = grid;
        }
        

        public void SetPos( float x, float y )
        {
            m_Pos.x = x;
            m_Pos.y = y;
        }
        public void SetPos( Vector2 pos )
        {
            m_Pos = pos;
        }
        

        public Vector2 PosToLocalPos( Vector2 pos )
        {
            Vector2 pivot = RectTransform.pivot;
            Vector2 size = RectTransform.sizeDelta;
            pos.x += pivot.x * size.x;
            pos.y -= ( 1.0f - pivot.y ) * size.y;
            return pos;
        }
        

        public Vector2 LocalPosToPos( Vector2 pos )
        {
            Vector2 pivot = RectTransform.pivot;
            Vector2 size = RectTransform.sizeDelta;
            pos.x -= pivot.x * size.x;
            pos.y += ( 1.0f - pivot.y ) * size.y;
            return pos;
        }
        
 
        public void UpdateLocalPos( Vector2 pos )
        {
            m_Pos = pos;
            RectTransform.localPosition = pos;
        }
        public void UpdateAnchoredPos( Vector2 pos )
        {
            m_Pos = pos;
            RectTransform.anchoredPosition = pos;
        }
        
        public Vector2 GetPivotAnchoredPosition( Vector2 pos )
        {
            Vector2 pivot = RectTransform.pivot;
            Vector2 size = RectTransform.sizeDelta;
            pos.x += ( 0.5f - pivot.x ) * size.x;
            pos.y += ( 0.5f - pivot.y ) * size.y;
            return pos;
        }
        
        public Vector2 GetPivotAnchoredPosition( )
        {
            Vector2 pivot = RectTransform.pivot;
            Vector2 size = RectTransform.sizeDelta;
            Vector2 pos = RectTransform.anchoredPosition;
            pos.x += ( 0.5f - pivot.x ) * size.x;
            pos.y += ( 0.5f - pivot.y ) * size.y;
            return pos;
        }
        

        public Vector2 GetWorldPos( )
        {
            Vector2 pos = m_UIContentController.AnchoredPosition;
            pos.x = pos.x + m_Pos.x;
            pos.y = pos.y + m_Pos.y;
            return pos;
        }
        
        public bool IsValid( )
        {
            if( m_Param != null )
            {
                return m_Param.IsValid();
            }
            return false;
        }
        
        public bool IsInvalid( )
        {
            if( m_Param != null )
            {
                return !m_Param.IsValid();
            }
            return true;
        }
        

        public bool IsLock( )
        {
            if( m_Param != null )
            {
                return m_Param.IsLock();
            }
            return true;
        }
        
        public bool IsReMake( )
        {
            if( m_Param != null )
            {
                return m_Param.IsReMake();
            }
            return true;
        }

        public bool IsViewIn( )
        {
            return m_ViewIn;
        }
        

        public bool IsViewOut( )
        {
            return !m_ViewIn;
        }
        

        public virtual void OnEnable()
        {
            if( m_Param != null )
            {
                if( m_Param.IsValid( ) == false )
                {
                    m_Param = null;
                }
                else
                {
                    m_Param.Wakeup( this );
                    m_Param.OnEnable( this );
                }
            }
            else
            {
                if( m_UIContentController == null || m_UIContentController.isNodeStatic == false )
                {
                    SetActive( false );
                }
            }
        }
        
        public virtual void OnDisable()
        {
            if( m_Param == null || m_Param.Node != this ) return;
            
            UIContentSource.Param param = m_Param;
            m_Param = null;
            
            param.OnDisable( this );
            param.Sleep( );
        }
        
        public virtual void OnViewIn( Vector2 pivotViewPosition )
        {
            m_ViewIn = true;

            if( m_Param != null )
            {
                m_Param.OnViewIn( this, pivotViewPosition );
            }

        }
        
    
        public virtual void OnViewOut( Vector2 pivotViewPosition )
        {
            m_ViewIn = false;
            
            if( m_Param != null )
            {
                m_Param.OnViewOut( this, pivotViewPosition );
            }
            //DebugUtility.Log("out>" + name);
        }
        
   
        public virtual void OnSelectOn( )
        {
            if( m_Param != null )
            {
                m_Param.OnSelectOn( this );
            }
            //DebugUtility.Log("pagefit>" + name);
        }
        
        public virtual void OnSelectOff( )
        {
            if( m_Param != null )
            {
                m_Param.OnSelectOff( this );
            }
            //DebugUtility.Log("pagefit>" + name);
        }
        
        public override void OnCacheEnable()
        {
            Detach( );
        }
        
        public override void OnCacheDisable()
        {
            // LWARS.ParamDisplay.OnCacheDisableAll( gameObject );
        }
        
        public virtual SerializeValueList Invoke( string key, SerializeValueList receiveValueList )
        {
            if( m_Param != null )
            {
                return m_Param.Invoke( this, key, receiveValueList );
            }
            return null;
        }

    }


    #if UNITY_EDITOR
    
    [UnityEditor.CustomEditor( typeof( UIContentNode ) )]
    public class EditorInspector_UIContentNode : UnityEditor.Editor
    {
        public override void OnInspectorGUI( )
        {
            CustomFieldAttribute.OnInspectorGUI( target.GetType(), serializedObject );
            
            // ----------------------------------------
            
            UIContentNode node = target as UIContentNode;
            
            UnityEditor.EditorGUILayout.LabelField( "Index", node.Index.ToString( ) );
        }
    }
        
    #endif
}
