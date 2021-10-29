using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using Object = UnityEngine.Object;
#if UNITY_EDITOR
using Bolt;
using UnityEditor;
#endif

namespace EG
{
    public class EventTrackShaderProperty : EventTrack
    #if UNITY_EDITOR
    , CustomFieldInterface
    #endif
    {
        public enum EPropertyType
        {
            None        ,
            Background  ,
        }
        
        #if UNITY_EDITOR
        public static Utility.EnumArray.Element[] EPropertyTypeNames =
        {
            new Utility.EnumArray.Element( EPropertyType.None        , "输入值"      ),
            new Utility.EnumArray.Element( EPropertyType.Background  , "背景切换"    ),
        };
        #endif
        
        public class Status : EventTrackStatus
        {
 
            private EventTrackShaderProperty    m_Self              = null;
            private ObjectCache                 m_Cache             = default(ObjectCache);
            
            private MaterialClone               m_MaterialClone     = null;
            
            
            public GameObject                   gameObject          { get { return m_Cache.gameObject; } }
            

            public Status( EventPlayerStatus owner ) : base( owner )
            {
            }
            

            public override void Initialize( AppMonoBehaviour behaviour, EventTrack track )
            {
                base.Initialize( behaviour, track );
                
                m_Self = Track as EventTrackShaderProperty;
            }
            
   
            public override void Release( AppMonoBehaviour behaviour )
            {
                
                base.Release( behaviour );
            }
            
            protected void RefreshMaterial( )
            {
                // 
                if( m_MaterialClone == null )
                {
                    GameObject gobj = gameObject;
                    if( gobj != null )
                    {
                        m_MaterialClone = gobj.RequireComponent<MaterialClone>( );
                        #if UNITY_EDITOR
                        if( Application.isPlaying == false && m_MaterialClone.Materials.Count > 0 )
                        {
                            // 
                            return;
                        }
                        #endif
                        //Debug.LogWarning( "cache material >> " + m_MaterialClone.Materials.Count + " " + gameObject.name + " @ " + gameObject.GetInstanceID() );
                        m_MaterialClone.Refresh( );
                    }
                }
            }
            
            private int Sample( ECurveType curveType, int from, int to, float t, AnimationCurve curve )
            {
                int result = 0;
                result = (int)CurveUtility.Sample( curveType, from, to, t, curve );
                return result;
            }
            
            private float Sample( ECurveType curveType, float from, float to, float t, AnimationCurve curve )
            {
                float result = 0;
                result = CurveUtility.Sample( curveType, from, to, t, curve );
                return result;
            }
            
            private Vector2 Sample( ECurveType curveType, Vector2 from, Vector2 to, float t, AnimationCurve curve )
            {
                Vector2 result = Vector2.zero;
                result.x = CurveUtility.Sample( curveType, from.x, to.x, t, curve );
                result.y = CurveUtility.Sample( curveType, from.y, to.y, t, curve );
                return result;
            }
            
            private Vector3 Sample( ECurveType curveType, Vector3 from, Vector3 to, float t, AnimationCurve curve )
            {
                Vector3 result = Vector3.zero;
                result.x = CurveUtility.Sample( curveType, from.x, to.x, t, curve );
                result.y = CurveUtility.Sample( curveType, from.y, to.y, t, curve );
                result.z = CurveUtility.Sample( curveType, from.z, to.z, t, curve );
                return result;
            }
            
            private Vector4 Sample( ECurveType curveType, Vector4 from, Vector4 to, float t, AnimationCurve curve )
            {
                Vector4 result = Vector4.zero;
                result.x = CurveUtility.Sample( curveType, from.x, to.x, t, curve );
                result.y = CurveUtility.Sample( curveType, from.y, to.y, t, curve );
                result.z = CurveUtility.Sample( curveType, from.z, to.z, t, curve );
                result.w = CurveUtility.Sample( curveType, from.w, to.w, t, curve );
                return result;
            }
            
            private Color Sample( ECurveType curveType, Color from, Color to, float t, AnimationCurve curve )
            {
                Color result = Color.white;
                result.r = CurveUtility.Sample( curveType, from.r, to.r, t, curve );
                result.g = CurveUtility.Sample( curveType, from.g, to.g, t, curve );
                result.b = CurveUtility.Sample( curveType, from.b, to.b, t, curve );
                result.a = CurveUtility.Sample( curveType, from.a, to.a, t, curve );
                return result;
            }
            
            protected override void OnStart( AppMonoBehaviour behaviour )
            {
                CacheTarget( behaviour, m_Self.TargetId, ref m_Cache );
            }
            
            protected override void OnUpdate( AppMonoBehaviour behaviour, float time )
            {
                if( string.IsNullOrEmpty( m_Self.Key ) ) return;
                
                // 
                RefreshMaterial( );
                
                // 
                if( m_MaterialClone == null ) return;
                
                // 
                if( m_MaterialClone.Material.HasProperty( m_Self.Key ) == false ) return;
                
                // 
                if( m_Self.PropertyType == EPropertyType.None )
                {
                    
                    switch( m_Self.From.type )
                    {
                    case SerializeValue.Type.Int:
                        m_MaterialClone.Material.SetInt( m_Self.Key, Sample( m_Self.CurveType, m_Self.From.v_Int, m_Self.To.v_Int, m_Self.GetTimeScale( time ), m_Self.Curve ) );
                        break;
                    case SerializeValue.Type.Long:
                        m_MaterialClone.Material.SetInt( m_Self.Key, Sample( m_Self.CurveType, m_Self.From.v_Int, m_Self.To.v_Int, m_Self.GetTimeScale( time ), m_Self.Curve ) );
                        break;
                    case SerializeValue.Type.Float:
                        m_MaterialClone.Material.SetFloat( m_Self.Key, Sample( m_Self.CurveType, m_Self.From.v_Float, m_Self.To.v_Float, m_Self.GetTimeScale( time ), m_Self.Curve ) );
                        break;
                    case SerializeValue.Type.Vector2:
                        m_MaterialClone.Material.SetVector( m_Self.Key, Sample( m_Self.CurveType, m_Self.From.v_Vector2, m_Self.To.v_Vector2, m_Self.GetTimeScale( time ), m_Self.Curve ) );
                        break;
                    case SerializeValue.Type.Vector3:
                        m_MaterialClone.Material.SetVector( m_Self.Key, Sample( m_Self.CurveType, m_Self.From.v_Vector3, m_Self.To.v_Vector3, m_Self.GetTimeScale( time ), m_Self.Curve ) );
                        break;
                    case SerializeValue.Type.Vector4:
                        m_MaterialClone.Material.SetVector( m_Self.Key, Sample( m_Self.CurveType, m_Self.From.v_Vector4, m_Self.To.v_Vector4, m_Self.GetTimeScale( time ), m_Self.Curve ) );
                        break;
                    case SerializeValue.Type.Color:
                        m_MaterialClone.Material.SetColor( m_Self.Key, Sample( m_Self.CurveType, m_Self.From.v_Color, m_Self.To.v_Color, m_Self.GetTimeScale( time ), m_Self.Curve ) );
                        break;
                    }
                }
                else if( m_Self.PropertyType == EPropertyType.Background )
                {
                  
                }
            }
            
            protected override void OnEnd( AppMonoBehaviour behaviour )
            {
            }
            
            #if UNITY_EDITOR

            protected override void ReCacheTarget( AppMonoBehaviour behaviour )
            {
                m_Cache = default( ObjectCache );
                OnStart( behaviour );
            }
            
            public override void EditorRelease()
            {
                if( Application.isPlaying == false )
                {
                    if( m_MaterialClone != null )
                    {
                        m_MaterialClone.Release( );
                        Component.DestroyImmediate( m_MaterialClone );
                        m_MaterialClone = null;
                    }
                }
                

                base.EditorRelease();
            }
            
            #endif //UNITY_EDITOR
        }
        

        [CustomFieldGroup("设定")]
        [CustomFieldAttribute("TargetId",CustomFieldAttribute.Type.Custom)]
        public string                       TargetId    = null;
        
        [CustomFieldGroup("设定")]
        [CustomFieldAttribute("PropertyType",CustomFieldAttribute.Type.Enum,typeof(EPropertyType),typeof(EventTrackShaderProperty))]
        public EPropertyType                PropertyType= EPropertyType.None;
        
        [CustomFieldGroup("设定")]
        [CustomFieldDispCond("PropertyType","None",true)]
        [CustomFieldAttribute("CurveType",CustomFieldAttribute.Type.Enum,typeof( ECurveType ) )]
        public ECurveType                   CurveType   = ECurveType.None;
        
        [CustomFieldGroup("设定")]
        [CustomFieldDispCond("PropertyType","None",true)]
        [CustomFieldAttribute("Curve",CustomFieldAttribute.Type.Custom)]
        public AnimationCurve               Curve       = new AnimationCurve( new Keyframe( 0, 0 ), new Keyframe( 1, 1 ) );
        
        [CustomFieldGroup("设定")]
        [CustomFieldAttribute("Key",CustomFieldAttribute.Type.String)]
        public string                       Key;
        
        
        [CustomFieldGroup("设定")]
        [CustomFieldAttribute("From",CustomFieldAttribute.Type.Custom)]
        public SerializeValue              From;
        [CustomFieldAttribute("To",CustomFieldAttribute.Type.Custom)]
        [CustomFieldGroup("设定")]
        public SerializeValue              To;
        


        public override ECategory Category
        {
            get { return ECategory.Common; }
        }
        
        
        public override EventTrackStatus CreateStatus( EventPlayerStatus owner )
        {
            return new Status( owner );
        }
        

        #if UNITY_EDITOR
        
        public static string        ClassName  { get { return "Shader/property change"; } }
        public override Color       TrackColor { get { return new Color32( 255, 174, 201, 255 ); } }
        

        public void OnCustomProperty( CustomFieldAttribute attr, UnityEditor.SerializedProperty prop, float width )
        {
            if( prop.name == "TargetId" )
            {
                EditorGUI.BeginChangeCheck();
                if( EventTrackStatus.OnCustomProperty_TargetField( attr, prop, width ) )
                {
                    RequestTrackStatusUpdate();
                    TargetId = prop.stringValue;    // 
                }
                if( EditorGUI.EndChangeCheck() )
                {
                    EditorUtility.SetDirty( this );
                }
            }
            else if( prop.name == "From" )
            {
                EditorGUI.BeginChangeCheck();
                From.OnGUIInspect( null, null, "From", width );
                if( EditorGUI.EndChangeCheck() )
                {
                    EditorUtility.SetDirty( this );
                }
            }
            else if( prop.name == "To" )
            {
                EditorGUI.BeginChangeCheck();
                To.OnGUIInspect( null, null, "To", width );
                if( EditorGUI.EndChangeCheck() )
                {
                    EditorUtility.SetDirty( this );
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
