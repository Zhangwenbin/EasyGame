using UnityEngine;
using System.Collections;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace EG
{

    public class EventTrackBattleEffectPlay : EventTrack
#if UNITY_EDITOR
    , CustomFieldInterface
#endif
    {
        public class Status : EventTrackStatus
        {

            protected EventTrackBattleEffectPlay            m_EventTrack    = null;
            
            protected ObjectCache                           m_Cache         = default(ObjectCache);
            protected ParticleSystem                        m_Particle      = null;
            protected ParticleSystem[]                      m_Particles     = null;

            public GameObject                   gameObject          { get { return m_Cache.gameObject; } }
            public ParticleSystem               particle            { get { if( m_Particle == null ) m_Particle = GetParticle( ); return m_Particle;            } }
            public ParticleSystem[]             particles           { get { if( m_Particles == null ) m_Particles = GetParticles( ); return m_Particles;        } }
            
            public void Play()
            {
                if( m_EventTrack.InChild == false )
                {
                    var part = particle;
                    if( part != null )
                    {
                        part.Play();
                    }
                }
                else
                {
                    var parts = particles;
                    if( parts != null && parts.Length > 0 )
                    {
                        for( int i = 0; i < parts.Length; ++i )
                        {
                            parts[i]?.Play();
                        }
                    }
                }
            }
            

            public Status( EventPlayerStatus owner ) : base( owner )
            {
            }
            
            public override void Initialize( EG.AppMonoBehaviour behaviour, EventTrack track )
            {
                base.Initialize( behaviour, track );
                

                m_EventTrack = Track as EventTrackBattleEffectPlay;
            }
            

            public override void Release( EG.AppMonoBehaviour behaviour )
            {
                // ------------------------------------
                
                base.Release( behaviour );
            }
            
            
            protected override void OnStart( EG.AppMonoBehaviour behaviour )
            {
                CacheTarget( behaviour, m_EventTrack.TargetId, ref m_Cache );
                
                base.OnStart( behaviour );
            }


            protected ParticleSystem GetParticle( )
            {
                GameObject gobj = gameObject;
                if( gobj != null )
                {
                    return gobj.GetComponent<ParticleSystem>();
                }
                return null;
            }
            

            protected ParticleSystem[] GetParticles( )
            {
                GameObject gobj = gameObject;
                if( gobj != null )
                {
                    return gobj.GetComponentsInChildren<ParticleSystem>( true );
                }
                return null;
            }
            

            #if UNITY_EDITOR
            
            protected override void ReCacheTarget( EG.AppMonoBehaviour behaviour )
            {
                m_Cache = default( ObjectCache );
                CacheTarget( behaviour, m_EventTrack.TargetId, ref m_Cache );
            }
            
            #endif //UNITY_EDITOR

        }
        

        
        [CustomFieldGroup("设定")]
        [CustomFieldAttribute("TargetId",CustomFieldAttribute.Type.Custom)]
        public string   TargetId    = null;
        
        [CustomFieldGroup("设定")]
        [CustomFieldAttribute("InChild",CustomFieldAttribute.Type.Bool)]
        public bool     InChild     = false;
        
        static public bool          s_IsStopPlay    = false;
        
        
        public override ECategory Category
        {
            get { return ECategory.Common; }
        }
        

        public override EventTrackStatus CreateStatus( EventPlayerStatus owner )
        {
            return new Status( owner );
        }
        
        
        public override void OnStart( EG.AppMonoBehaviour behaviour )
        {
            if( s_IsStopPlay )
                return;
            
            Status status = CurrentStatus as Status;
            if( status != null )
            {
                status.Play();
            }
        }
        
        
        #if UNITY_EDITOR

        public static string    ClassName           { get { return "战斗特效";                    } }


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

