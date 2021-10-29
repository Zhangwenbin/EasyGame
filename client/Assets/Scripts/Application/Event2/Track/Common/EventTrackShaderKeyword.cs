using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace EG
{

    public class EventTrackShaderKeyword : EventTrack
    #if UNITY_EDITOR
    , CustomFieldInterface
    #endif
    {
        public class Status : EventTrackStatus
        {

            private EventTrackShaderKeyword     m_Self              = null;
            private ObjectCache                 m_Cache             = default(ObjectCache);
            
            private MaterialClone               m_MaterialClone     = null;
            
            public GameObject                   gameObject          { get { return m_Cache.gameObject; } }
            

            public Status( EventPlayerStatus owner ) : base( owner )
            {
            }
            
            public override void Initialize( AppMonoBehaviour behaviour, EventTrack track )
            {
                base.Initialize( behaviour, track );

                m_Self = Track as EventTrackShaderKeyword;
            }
 
            public override void Release( AppMonoBehaviour behaviour )
            {
                // ------------------------------------
                
                base.Release( behaviour );
            }
            
            protected void RefreshMaterial( )
            {
                if( m_MaterialClone == null )
                {
                    m_MaterialClone = gameObject.RequireComponent<MaterialClone>( );
                    #if UNITY_EDITOR
                    if( Application.isPlaying == false && m_MaterialClone.Materials.Count > 0 )
                    {
                        return;
                    }
                    #endif
                    //Debug.LogWarning( "cache material >> " + m_MaterialClone.Materials.Count + " " + gameObject.name + " @ " + gameObject.GetInstanceID() );
                    m_MaterialClone.Refresh( );
                }
            }
            
            protected override void OnStart( AppMonoBehaviour behaviour )
            {
                CacheTarget( behaviour, m_Self.TargetId, ref m_Cache );
                
                if( string.IsNullOrEmpty( m_Self.Key ) ) return;
                
                RefreshMaterial( );
                
                if( m_MaterialClone == null ) return;
                
                if( m_Self.Value )  m_MaterialClone.Material.EnableKeyword( m_Self.Key );
                else                m_MaterialClone.Material.DisableKeyword( m_Self.Key );
            }
            
            protected override void OnUpdate( AppMonoBehaviour behaviour, float time )
            {
            }
            

            protected override void OnEnd( AppMonoBehaviour behaviour )
            {
                if( m_MaterialClone == null ) return;
                
                if( m_Self.Value )  m_MaterialClone.Material.DisableKeyword( m_Self.Key );
                else                m_MaterialClone.Material.EnableKeyword( m_Self.Key );
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
                
                // ----------------------------------------
                
                base.EditorRelease();
            }
            
            #endif //UNITY_EDITOR
        }
        
        [CustomFieldGroup("设定")]
        [CustomFieldAttribute("TargetId",CustomFieldAttribute.Type.Custom)]
        public string                       TargetId    = null;
        
        [CustomFieldGroup("设定")]
        [CustomFieldAttribute("Keyword",CustomFieldAttribute.Type.String)]
        public string                       Key;
        
        [CustomFieldGroup("设定")]
        [CustomFieldAttribute("ON/OFF",CustomFieldAttribute.Type.Bool)]
        public bool                         Value;
        
        
        public override ECategory Category
        {
            get { return ECategory.Common; }
        }
        
        
        public override EventTrackStatus CreateStatus( EventPlayerStatus owner )
        {
            return new Status( owner );
        }
        

        #if UNITY_EDITOR
        
        public static string        ClassName  { get { return "Shader/Keyword change"; } }
        public override Color       TrackColor { get { return new Color32( 255, 174, 201, 255 ); } }
        

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
        }
        
        #endif
    }
}
