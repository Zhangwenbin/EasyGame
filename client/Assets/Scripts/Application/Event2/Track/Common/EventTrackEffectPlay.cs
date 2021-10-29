using UnityEngine;
using System.Collections;

#if UNITY_EDITOR
using Bolt;
using UnityEditor;
#endif

namespace EG
{
    public class EventTrackEffectPlay : EventTrackWithTarget
#if UNITY_EDITOR
    , CustomFieldInterface
#endif
    {
        public class Status : EventTrackStatus
        {

            protected EventTrackEffectPlay m_Tweener = null;

            protected ObjectCache       m_Cache         = default( ObjectCache );

            EffectDataParticle          m_EffectData    = null;
            

            public GameObject gameObjec             { get { return m_Cache.gameObject;  } }
            public EffectDataParticle EffectData   { get { return m_EffectData;        } }
            
            
            public Status( EventPlayerStatus owner ) : base( owner )
            {
            }
            
            public override void Initialize( AppMonoBehaviour behaviour, EventTrack track )
            {
                base.Initialize( behaviour, track );
                
                m_Tweener = Track as EventTrackEffectPlay;

                CacheTarget( behaviour, m_Tweener.TargetId, ref m_Cache );
                if( m_Cache.gameObject != null )
                {
                    bool isActive = m_Cache.gameObject.activeSelf;
                    m_Cache.gameObject.SetActive( true );
                    m_EffectData = m_Cache.gameObject.RequireComponent<EffectDataParticle>();
                    m_EffectData.SetDontDestroy();
                    m_Cache.gameObject.SetActive( isActive );
                }
            }
            
            public override void Release( AppMonoBehaviour behaviour )
            {
                if( m_EffectData != null )
                {
#if UNITY_EDITOR
                    if( Application.isPlaying == false )
                    {
                        m_EffectData.Release();
                        DestroyImmediate( m_EffectData );
                    }
                    #endif
                    m_EffectData = null;
                }


                base.Release( behaviour );
            }


            protected override void OnStart( AppMonoBehaviour behaviour )
            {
                base.OnStart( behaviour );
            }
            
            #if UNITY_EDITOR
            
            protected override void ReCacheTarget( AppMonoBehaviour behaviour )
            {
                m_Cache = default( ObjectCache );
                CacheTarget( behaviour, m_Tweener.TargetId, ref m_Cache );
                if( m_Cache.gameObject != null )
                {
                    bool isActive = m_Cache.gameObject.activeSelf;
                    m_Cache.gameObject.SetActive( true );
                    m_EffectData = m_Cache.gameObject.RequireComponent<EffectDataParticle>();
                    m_EffectData.SetDontDestroy();
                    m_Cache.gameObject.SetActive( isActive );
                }
            }

            #endif //UNITY_EDITOR

        }
        
        
        [CustomFieldGroup( "设置" )]
        [CustomFieldAttribute( "TargetId", CustomFieldAttribute.Type.Custom )]
        public string TargetId = null;
        
        [CustomFieldGroup("效果")]
        [CustomFieldAttribute("PlayMode",CustomFieldAttribute.Type.Enum, typeof( EffectData.EPlayMode ))]
        public EffectData.EPlayMode         PlayMode = EffectData.EPlayMode.OneShot;
        
        
        public override ECategory Category
        {
            get { return ECategory.Common; }
        }
        
        
        public override EventTrackStatus CreateStatus( EventPlayerStatus owner )
        {
            return new Status( owner );
        }
        

        
        public override void OnStart(AppMonoBehaviour behaviour )
        {
            Status status = CurrentStatus as Status;
            if( status != null && status.EffectData != null )
            {
                #if UNITY_EDITOR
                if( Application.isPlaying == false )
                {
                    EventPlayer evPlayer = behaviour as EventPlayer;
                    if( evPlayer != null && evPlayer.IsManualUpdate )
                    {
                        EditorCacheEffect( status.EffectData );
                        status.EffectData.EditorResetPhase();
                    }
                }
                #endif
                
                if( status.EffectData.IsPlay )
                {
                    status.EffectData.Stop( true );
                }
                status.EffectData.Play();
            }
            
            return;
        }
        

        protected virtual void OnGenerate( EffectBase effect )
        {
        }
        

        #if UNITY_EDITOR

        new public static string    ClassName           { get { return "Placement effect control";                    } }
        public override Color       TrackColor          { get { return new Color32( 0xb3, 0xb3, 0x00, 0xff );   } }
        //public override bool        IsHaveObjectCache   { get { return true;                                    } }

        EffectData                  m_EditorEffectData      = null;
        Transform                   m_EditorCacheTrans      = null;
        Transform                   m_EditorParent          = null;
        Vector3                     m_EditorCacheLPos       = Vector3.zero;
        Quaternion                  m_EditorCacheRot        = Quaternion.identity;
        string                      m_ErrorLog              = "";


        public override void EditorRelease()
        {
            base.EditorRelease();

            // ----------------------------------------

            _EditorReleaseEffectObject();
        }

        void _EditorReleaseEffectObject()
        {
            if( m_EditorEffectData != null )
            {
                m_EditorEffectData.EditorResetPhase();

                m_EditorCacheTrans = null;
                m_EditorParent     = null;
                m_EditorEffectData = null;
            }
        }


        public override void EditorPreProcess( AppMonoBehaviour behaviour, float time, float dt, bool isLooped, bool isEnded )
        {
            base.EditorPreProcess( behaviour, time, dt, isLooped, isEnded );

            // ----------------------------------------
        }


        public override void EditorPostProcess( AppMonoBehaviour behaviour, float time, float dt, bool isLooped, bool isEnded )
        {
            base.EditorPostProcess( behaviour, time, dt, isLooped, isEnded );

            // ----------------------------------------

            // 
            if( time < Start || ( PlayMode == EffectData.EPlayMode.Loop && End <= time ) )
            {
                _EditorReleaseEffectObject();
            }

            if( m_EditorEffectData != null )
            {
                m_EditorEffectData.SimulateFromEventEditor( time, dt, Start, End, false, dt == 0, this );

                // 
                m_EditorCacheTrans.SetParent( m_EditorParent );
                m_EditorCacheTrans.localPosition = m_EditorCacheLPos;
                m_EditorCacheTrans.localRotation = m_EditorCacheRot;
            }
        }


        void EditorCacheEffect( EffectData effData )
        {
            // 
            _EditorReleaseEffectObject();

            m_EditorEffectData = effData;
            m_EditorEffectData.EditorAwake();

            m_EditorCacheTrans = m_EditorEffectData.transform;
            m_EditorParent = m_EditorCacheTrans.parent;

            m_EditorCacheLPos = m_EditorCacheTrans.localPosition;
            m_EditorCacheRot = m_EditorCacheTrans.localRotation;
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

            GUI.enabled = string.IsNullOrEmpty( TargetId ) == false;
            if( GUILayout.Button( "长度自动调整" ) )
            {
                m_ErrorLog = "";

                EventPlayer player = EventPlayer.CurrentEditorPlayer;
                if( player != null )
                {
                    // EventTrackStatus::CacheTarget() 
                    GameObject gobj = null;
                    SerializeValueBehaviour valueBehaviour = player.GetComponent<SerializeValueBehaviour>();
                    if( valueBehaviour != null )
                    {
                        gobj = valueBehaviour.list.GetGameObject( TargetId );
                    }
                    else
                    {
                        gobj = player.gameObject.FindChildAll( TargetId, true );
                    }

                    if( gobj != null )
                    {
                        float calcTime = 0f;
                        UIParticleSystem[] particles = gobj.GetComponentsInChildren<UIParticleSystem>(true);
                        if( particles != null && particles.Length > 0 )
                        {
                            for( int i = 0, max = particles.Length; i < max; ++i )
                            {
                                calcTime = Mathf.Max( calcTime, particles[i].duration + particles[i].startLifetime.Max );
                            }
                        }

                        End = Start;
                        End += calcTime;
                    }
                    else
                    {
                        
                        m_ErrorLog += "没有找到目标 ";
                        Debug.LogError( m_ErrorLog+TargetId );
                    }
                }
            }

            if( string.IsNullOrEmpty( m_ErrorLog ) == false )
            {
                GUI.contentColor = Color.red;
                GUILayout.Label( m_ErrorLog );
                GUI.contentColor = Color.white;
            }

            GUI.enabled = true;
        }

        #endif
    }
}

