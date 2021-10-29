using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace EG
{

    [AddComponentMenu("Scripts/Application/Common/Effect/EffectData.cs")]
    public class EffectData : AppMonoBehaviour
    {
        readonly System.Type[] GATHER_TYPES = new System.Type[]
        {
            typeof( TriggerCameraShake ),
            // typeof( CustomSound ),
        };
        
        public enum EPlayMode
        {
            None            ,
            OneShot         ,
            Loop            ,
        }
#if UNITY_EDITOR
        public static string[] PlayModeNames = new string[]
        {
            "None",
            "OneShot",
            "Loop",
        };
#endif
        
        public enum EFlag
        {
            UseAliveTime    ,
            Mirror          ,
        }
#if UNITY_EDITOR
        public static string[] EFlagNames = new string[]
        {
            "UseAliveTime",
            "Mirror",
        };
#endif
        
        public enum EGenerateOption
        {
            Mirror          ,       // 
        }
#if UNITY_EDITOR
        public static string[] EGenerateOptionNames = new string[]
        {
            "Mirror",
        };
#endif
        
        [System.Serializable]
        public struct Param
        {
            [CustomFieldAttribute("PlayMode",CustomFieldAttribute.Type.Enum,typeof( EPlayMode ) )]
            public EPlayMode                    PlayMode;               // 

            [CustomFieldAttribute("AliveTime",CustomFieldAttribute.Type.Float)]
            public float                        AliveTime;              // 

            [CustomFieldAttribute( "GenOption", CustomFieldAttribute.Type.BitFlag, typeof( EGenerateOption ), typeof( EffectData ) )]
            public BitFlag                      GenOption;              // 
        }
        
        enum Phase
        {
            Idle,
            Play,
            Stop,
            Pause,
            WaitEnd,
            End,
        };
        

        EPlayMode           m_PlayMode          = EPlayMode.OneShot;
        BitFlag<EFlag>      m_Flag              = new BitFlag<EFlag>();
        float               m_AliveTime         = 0;
        
        Vector3             m_LocalScale        = Vector3.one;      // 
        Phase               m_Phase             = Phase.Idle;
        
        Transform           m_CacheTrans        = null;
        
        List<Behaviour>     m_Behaviours        = new List<Behaviour>();
        
        
        public EPlayMode   PlayMode
        {
            get { return m_PlayMode; }
        }
        
        public bool IsEnd
        {
            get { return m_Phase == Phase.WaitEnd || m_Phase == Phase.End; }
        }
        
        public bool IsPlay
        {
            get { return m_Phase == Phase.Play; }
        }
        
        
        public bool IsPause
        {
            get { return m_Phase == Phase.Pause; }
        }
        
        protected virtual void Awake()
        {
            m_CacheTrans = transform;
            _GatherAttachedComponent();
            
            Initialize();
            
            // 
            OnStop( true );
        }
        

        public override void Initialize()
        {
            if( isInitialized )
                return;
            
            base.Initialize();
            
            // ----------------------------------------
            
            m_Phase = Phase.Idle;
            
            m_LocalScale = m_CacheTrans.localScale;
            
            if( gameObject.layer != GameUtility.LayerEffect )
            {
                GameUtility.SetLayer( this, GameUtility.LayerEffect, true );
            }
        }

        public override void Release()
        {
            if( isInitialized == false )
                return;
            
            m_CacheTrans = null;
            
            // ----------------------------------------
            
            base.Release( );
        }
        

        private void LateUpdate()
        {
            if( m_Phase == Phase.Idle )
                return;
            
            if( CheckDestroy( Time.deltaTime ) )
            {
                m_Phase = Phase.End;
            }
            
            if( m_Phase == Phase.End )
            {
                gameObject.SafeDestroy( );
            }
        }
        

        protected virtual bool CheckDestroy( float dt )
        {
            // 
            if( m_Flag.HasValue( EFlag.UseAliveTime ) )
            {
                m_AliveTime -= Time.deltaTime;
                if( m_AliveTime <= 0 )
                {
                    return true;
                }
            }
            
            return false;
        }


        public virtual void SetParam( ref Param param )
        {
            SetPlayMode( param.PlayMode );
            
            if( param.AliveTime > 0 )
            {
                SetAliveTime( param.AliveTime );
            }
            
            if( param.GenOption != null && param.GenOption.HasValue( (int)EGenerateOption.Mirror ) )
            {
                SetMirror( true );
            }
        }
        

        public void SetPlayMode( EPlayMode playMode )
        {
            m_PlayMode = playMode;
        }
        

        public void SetAliveTime( float aliveTime )
        {
            m_Flag.SetValue( EFlag.UseAliveTime, true );
            m_AliveTime = aliveTime;
        }
        
        public void SetMirror( bool isMirror )
        {
            if( m_Flag.HasValue( EFlag.Mirror ) != isMirror )
            {
                Vector3 newScl = m_LocalScale;
                if( isMirror )
                {
                    newScl.z *= -1;
                }

                if( m_CacheTrans == null )
                {
                    m_CacheTrans = transform;
                }

                m_CacheTrans.localScale = newScl;
                m_Flag.SetValue( EFlag.Mirror, isMirror );
            }
        }
        
        public void Attach( GameObject attachObj )
        {
            if( attachObj != null )
            {
                if( m_CacheTrans == null )
                {
                    m_CacheTrans = transform;
                }
                m_CacheTrans.SetParent( attachObj.transform );
            }
        }

        public void ResetAttach( )
        {
            if( m_CacheTrans != null )
            {
                m_CacheTrans.SetParent( null, true );
            }
        }
        

        public void Play()
        {
            if( m_Phase == Phase.Idle || m_Phase == Phase.Stop || m_Phase == Phase.Pause )
            {
                m_Phase = Phase.Play;
                OnPlay();
            }
        }
        
        protected virtual void OnPlay()
        {
            _EnableAttachedComponent();
        }
        

        public void Stop( bool isImmediate )
        {
            if( m_Phase == Phase.Play || m_Phase == Phase.Pause )
            {
                m_Phase = Phase.Stop;
                OnStop( isImmediate );
            }
        }
        
        protected virtual void OnStop( bool isImmediate )
        {
            _DisableAttachedComponent();
        }
        

        public void StopDestroy( )
        {
            if( m_Phase != Phase.End && m_Phase != Phase.WaitEnd )
            {
                m_Phase = Phase.WaitEnd;
                OnStopDestroy();
            }
        }
        
        protected virtual void OnStopDestroy( )
        {
            SetPlayMode( EPlayMode.OneShot );
        }
        
   
        public void Delete( )
        {
            if( m_Phase != Phase.End )
            {
                m_Phase = Phase.End;
            }
        }
        

        public void Pause()
        {
            if( m_Phase == Phase.Play )
            {
                m_Phase = Phase.Pause;
                OnPause();
            }
        }
        
        protected virtual void OnPause()
        {
        }
        

        public virtual void Resume()
        {
            if( m_Phase == Phase.Pause )
            {
                m_Phase = Phase.Play;
                OnResume();
            }
        }
        
        protected virtual void OnResume()
        {
        }
        

        void _GatherAttachedComponent()
        {
            for( int i = 0, max = GATHER_TYPES.Length; i < max; ++i )
            {
                Behaviour behaviour = GetComponent( GATHER_TYPES[i] ) as Behaviour;
                if( behaviour != null )
                {
                    m_Behaviours.Add( behaviour );
                }
            }
        }
        

        void _EnableAttachedComponent()
        {
            if( m_Behaviours.Count > 0 )
            {
                for( int i = 0, max = m_Behaviours.Count; i < max; ++i )
                {
                    if( m_Behaviours[i] != null )
                    {
                        m_Behaviours[i].enabled = true;
                    }
                }
            }
        }
        

        void _DisableAttachedComponent()
        {
            if( m_Behaviours.Count > 0 )
            {
                for( int i = 0, max = m_Behaviours.Count; i < max; ++i )
                {
                    if( m_Behaviours[i] != null )
                    {
                        m_Behaviours[i].enabled = false;
                    }
                }
            }
        }

        
        public void SetShaderKeyword( string key, bool value )
        {
            Renderer[] renderers = gameObject.GetComponentsInChildren<Renderer>( );
            for( int i = 0; i < renderers.Length; ++i )
            {
                if( value )
                    renderers[i].material.EnableKeyword( key );
                else
                    renderers[i].material.DisableKeyword( key );
            }
        }
        

        public void SetShaderProperty( string key, Color color )
        {
            Renderer[] renderers = gameObject.GetComponentsInChildren<Renderer>( );
            for( int i = 0; i < renderers.Length; ++i )
            {
                renderers[i].material.SetColor( key, color );
            }
        }
        

        private void OnEnable( )
        {
            if( IsPause )
            {
                Resume();
            }
        }
        

        private void OnDisable( )
        {
            if( IsPlay )
            {
                Pause();
            }
        }
        
        
        public void EditorAwake()
        {
            m_CacheTrans = transform;
            _GatherAttachedComponent();
            
            EditorInitialize();
        }


        private void EditorInitialize()
        {
            Initialize();
            
            OnEditorInitialize();
        }


        public virtual void EditorResetPhase()
        {
            m_Phase = Phase.Idle;
        }

        protected virtual void OnEditorInitialize()
        {
        }
        
    
        public virtual void SimulateNew( float t, float dt, bool isRestart )
        {
        }
        
        public virtual bool SimulateFromEventEditor( float t, float dt, float startTime, float endTime, bool isRestart, bool isManualMove, EventTrack caller )
        {
            if( CheckDestroy( dt ) )
            {
                return true;
            }
            
            return false;
        }
        
    }
}
