using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace EG
{
    public class EventTrackToggleActive : EventTrack
    #if UNITY_EDITOR
    , CustomFieldInterface
    #endif
    {
        public class Status : EventTrackStatus
        {
            protected EventTrackToggleActive    m_Toggle            = null;
            
            protected ObjectCache               m_Cache             = default(ObjectCache);
            
            
            public GameObject                   gameObject          { get { return m_Cache.gameObject; } }
            
            public Status( EventPlayerStatus owner ) : base( owner )
            {
            }
            
            public override void Initialize( AppMonoBehaviour behaviour, EventTrack track )
            {
                base.Initialize( behaviour, track );
                
                m_Toggle = Track as EventTrackToggleActive;
            }
            
            public override void Release( AppMonoBehaviour behaviour )
            {

                base.Release( behaviour );
            }
            
            protected override void OnStart( AppMonoBehaviour behaviour )
            {
                CacheTarget( behaviour, m_Toggle.TargetId, ref m_Cache );
                
                base.OnStart( behaviour );
            }
            
            #if UNITY_EDITOR
            
            protected override void ReCacheTarget( AppMonoBehaviour behaviour )
            {
                m_Cache = default( ObjectCache );
                CacheTarget( behaviour, m_Toggle.TargetId, ref m_Cache );
            }
            
            #endif //UNITY_EDITOR
  
        }
        
        [CustomFieldGroup("设定")]
        [CustomFieldAttribute("目标", CustomFieldAttribute.Type.Custom)]
        public string   TargetId    = null;
        
        [CustomFieldGroup("设定")]
        [CustomFieldAttribute("ON/OFF",CustomFieldAttribute.Type.Bool)]
        public bool     Active      = false;
        
        [CustomFieldGroup("设定")]
        [CustomFieldAttribute("维持状态", CustomFieldAttribute.Type.Bool)]
        public bool     KeepActive  = false;
        
        
        public override ECategory Category
        {
            get { return ECategory.Common; }
        }
        

        public override EventTrackStatus CreateStatus( EventPlayerStatus owner )
        {
            return new Status( owner );
        }
        

        public override void OnStart( AppMonoBehaviour behaviour )
        {
            Status status = CurrentStatus as Status;
            if( status != null )
            {
                status.gameObject.SetActiveSafe( Active );
            }
        }
        

        public override void OnEnd( AppMonoBehaviour behaviour )
        {
            if( KeepActive == false )
            {
                Status status = CurrentStatus as Status;
                if( status != null )
                {
                    status.gameObject.SetActiveSafe( !Active );
                }
            }
        }
        
        #if UNITY_EDITOR
        
        public static string        ClassName  { get { return "游戏对象/显示隐藏"; } }
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
