using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace EG
{
    public abstract class EventTrackTweener : EventTrack
    #if UNITY_EDITOR
    , CustomFieldInterface
    #endif
    {
        #if UNITY_EDITOR
        public static string[] ECurveTypeNames = new string[]
        {
            "使用自定义曲线",
            "线性",
            "球形线性",
            "弹簧",
            "InSin: Cos ( 1,0 = start,end )",
            "OutSin: Sin ( 0,1 = start,end )",
            "InOutSin: Cos ( 1,0,-1 = start,(start+end)*0.5,end )",
        };
        #endif
        
        public class StatusBase : EventTrackStatus
        {

            protected EventTrackTweener     m_Tweener           = null;
            
            protected ObjectCache           m_Cache             = default(ObjectCache);
            protected RectTransform         m_Transform         = null;
            protected CanvasGroup           m_CanvasGroup       = null;
            protected Graphic               m_Graphic           = null;
            protected Graphic[]             m_Graphics          = null;
            protected Vector2               m_DefaultPosition   = Vector2.zero;
            
            
            public GameObject               gameObject          { get { return m_Cache.gameObject;                                                          } }
            public RectTransform            rectTransform       { get { if( m_Transform == null ) m_Transform = GetTargetTranform( ); return m_Transform;   } }
            public CanvasGroup              canvasGroup         { get { if( m_CanvasGroup == null ) m_CanvasGroup = GetCanvasGroup( ); return m_CanvasGroup;} }
            public Graphic                  graphic             { get { if( m_Graphic == null ) m_Graphic = GetGraphic( ); return m_Graphic;                } }
            public Graphic[]                graphics            { get { if( m_Graphics == null ) m_Graphics = GetGraphics( ); return m_Graphics;            } }
            
  
            
            public StatusBase( EventPlayerStatus owner ) : base( owner )
            {
            }
            
 
            public override void Initialize( AppMonoBehaviour behaviour, EventTrack track )
            {
                base.Initialize( behaviour, track );

                m_Tweener = Track as EventTrackTweener;
            }
            
            public override void Release( AppMonoBehaviour behaviour )
            {

                base.Release( behaviour );
            }
            

            protected RectTransform GetTargetTranform( )
            {
                GameObject gobj = gameObject;
                if( gobj != null )
                {
                    UIObject uiobj = gobj.GetComponent<UIObject>( );
                    if( uiobj != null )
                    {
                        m_DefaultPosition = uiobj.DefaultAnchoredPosition;
                        return uiobj.RectTransform;
                    }
                    else
                    {
                        RectTransform result = gobj.GetComponent<RectTransform>( );
                        if( result != null ) m_DefaultPosition = result.anchoredPosition;
                        return result;
                    }
                }
                return null;
            }
            
            protected CanvasGroup GetCanvasGroup( )
            {
                GameObject gobj = gameObject;
                if( gobj != null )
                {
                    return gobj.GetComponent<CanvasGroup>( );
                }
                return null;
            }
            

            protected Graphic GetGraphic( )
            {
                GameObject gobj = gameObject;
                if( gobj != null )
                {
                    return gobj.GetComponent<Graphic>( );
                }
                return null;
            }
            
 
            protected Graphic[] GetGraphics( )
            {
                GameObject gobj = gameObject;
                if( gobj != null )
                {
                    return gobj.GetComponentsInChildren<Graphic>( );
                }
                return null;
            }
            
            protected override void OnStart( AppMonoBehaviour behaviour )
            {
                CacheTarget( behaviour, m_Tweener.TargetId, ref m_Cache );
                
                base.OnStart( behaviour );
            }
            
 
            protected override void OnUpdate( AppMonoBehaviour behaviour, float time )
            {
                base.OnUpdate( behaviour, time );
            }
            
 
            protected override void OnEnd( AppMonoBehaviour behaviour )
            {
                base.OnEnd( behaviour );
            }
            

            protected override void OnBackground( AppMonoBehaviour behaviour, float time )
            {
                base.OnBackground( behaviour, time );
            }
            

            #if UNITY_EDITOR
        

            protected override void ReCacheTarget( AppMonoBehaviour behaviour )
            {
                m_Cache = default( ObjectCache );
                CacheTarget( behaviour, m_Tweener.TargetId, ref m_Cache );
                m_Transform = null;
            }
            
            #endif //UNITY_EDITOR

        }
        

        
        [CustomFieldGroup("设置")]
        [CustomFieldAttribute("目标", CustomFieldAttribute.Type.Custom)]
        public string                   TargetId            = "";
        
        [CustomFieldGroup("设置")]
        [CustomFieldAttribute("曲线类型", CustomFieldAttribute.Type.Enum,typeof( ECurveType ) )]
        public ECurveType               CurveType           = ECurveType.None;
        
        [CustomFieldGroup("设置")]
        [CustomFieldAttribute("自定义曲线", CustomFieldAttribute.Type.Custom)]
        public AnimationCurve           Curve               = new AnimationCurve( new Keyframe( 0, 0 ), new Keyframe( 1, 1 ) );
        
        
        
        public override ECategory Category
        {
            get { return ECategory.UI; }
        }
        

        public override EventTrackStatus CreateStatus( EventPlayerStatus owner )
        {
            return new StatusBase( owner );
        }
        

        public virtual float Sample( float from, float to, float t )
        {
            return CurveUtility.Sample( CurveType, from, to, t, Curve );
        }
        

        public virtual Vector2 Sample( Vector2 from, Vector2 to, float t )
        {
            Vector2 result = Vector2.zero;
            result.x = CurveUtility.Sample( CurveType, from.x, to.x, t, Curve );
            result.y = CurveUtility.Sample( CurveType, from.y, to.y, t, Curve );
            return result;
        }
        

        public virtual Vector3 Sample( Vector3 from, Vector3 to, float t )
        {
            Vector3 result = Vector3.zero;
            result.x = CurveUtility.Sample( CurveType, from.x, to.x, t, Curve );
            result.y = CurveUtility.Sample( CurveType, from.y, to.y, t, Curve );
            result.z = CurveUtility.Sample( CurveType, from.z, to.z, t, Curve );
            return result;
        }
        

        public virtual Color Sample( Color from, Color to, float t )
        {
            Color result = Color.white;
            result.r = CurveUtility.Sample( CurveType, from.r, to.r, t, Curve );
            result.g = CurveUtility.Sample( CurveType, from.g, to.g, t, Curve );
            result.b = CurveUtility.Sample( CurveType, from.b, to.b, t, Curve );
            result.a = CurveUtility.Sample( CurveType, from.a, to.a, t, Curve );
            return result;
        }
        
        
        #if UNITY_EDITOR
        
        public static string        ClassName           { get { return "Tween/Tweener"; } }
        

        public override void OnSceneGUI( SceneView sceneView, GameObject gobj, float time )
        {
        }
        

        public void OnCustomProperty( CustomFieldAttribute attr, UnityEditor.SerializedProperty prop, float width )
        {
            if( prop.name == "TargetId" )
            {
                if( EventTrackStatus.OnCustomProperty_TargetField( attr, prop, width ) )
                {
                    RequestTrackStatusUpdate();
                    TargetId = prop.stringValue;    // 
                }
            }
            else if( prop.name == "Curve" )
            {
                EditorGUILayout.BeginHorizontal( );
                {
                    EditorGUILayout.LabelField( attr.text, GUILayout.Width( width * 0.35f ) );
                    EditorGUI.BeginChangeCheck();
                    Curve = EditorGUILayout.CurveField( Curve );
                    if( EditorGUI.EndChangeCheck() )
                    {
                        EditorUtility.SetDirty( this );
                    }
                }
                EditorGUILayout.EndHorizontal( );
            }
        }
        
        #endif
    }

}
