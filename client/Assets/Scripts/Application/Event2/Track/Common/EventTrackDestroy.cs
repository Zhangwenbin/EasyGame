using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace EG
{
    public class EventTrackDestroy : EventTrack
    #if UNITY_EDITOR
    , CustomFieldInterface
    #endif
    {
        public class Status : EventTrackStatus
        {
            private EventTrackDestroy           m_Self              = null;
            private ObjectCache                 m_Cache             = default(ObjectCache);
            
            private bool                        m_Destroy           = false;
            
            public GameObject                   gameObject          { get { return m_Cache.gameObject; } }
            

            public Status( EventPlayerStatus owner ) : base( owner )
            {
            }
            
            public override void Initialize( AppMonoBehaviour behaviour, EventTrack track )
            {
                base.Initialize( behaviour, track );
                

                m_Self = Track as EventTrackDestroy;
            }
            
    
            public override void Release( AppMonoBehaviour behaviour )
            {
                // ------------------------------------
                
                base.Release( behaviour );
            }
            
            protected override void OnStart( AppMonoBehaviour behaviour )
            {
                CacheTarget( behaviour, m_Self.TargetId, ref m_Cache );

                m_Destroy = true;
            }
            
            protected override void OnUpdate(AppMonoBehaviour behaviour, float time )
            {
            }
            

            protected override void OnEnd( AppMonoBehaviour behaviour )
            {
            }
            
            protected override void OnBackground(AppMonoBehaviour behaviour, float time )
            {
                if( m_Destroy )
                {
                    #if UNITY_EDITOR
                    if( Application.isPlaying )
                    {
                        gameObject.SafeDestroy( );
                    }
                    #else
                    {
                        gameObject.SafeDestroy( );
                    }
                    #endif
                    m_Destroy = false;
                }
            }
            
            #if UNITY_EDITOR
            
            protected override void ReCacheTarget( AppMonoBehaviour behaviour )
            {
                m_Cache = default( ObjectCache );
                CacheTarget( behaviour, m_Self.TargetId, ref m_Cache );
            }
            
            #endif //UNITY_EDITOR
        }
        

        [CustomFieldGroup("设置")]
        [CustomFieldAttribute("TargetId",CustomFieldAttribute.Type.Custom)]
        public string   TargetId    = null;
        
        
        public override ECategory Category
        {
            get { return ECategory.Common; }
        }
        


        public override EventTrackStatus CreateStatus( EventPlayerStatus owner )
        {
            return new Status( owner );
        }
        

        #if UNITY_EDITOR
        
        public static string        ClassName  { get { return "Game Object/Delete"; } }
        public override Color       TrackColor { get { return new Color32( 153, 217, 234, 255 ); } }
        

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
