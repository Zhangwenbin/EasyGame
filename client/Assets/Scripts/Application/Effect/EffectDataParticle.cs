using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace EG
{
    [AddComponentMenu("Scripts/Application/Common/Effect/EffectDataParticle.cs")]
    public class EffectDataParticle : EffectData
    {

        
        ParticleSystem[]        m_CacheParticles    = null;
        
        
         UIParticleSystem[]      m_CacheUIParticles  = null;


        AnimationPlayerSimple   m_AnimationPlayer   = null;
        
        //
        bool                    m_DontDestroy       = false;
        
        
        public int ParticleCount
        {
            get { return ( m_CacheParticles != null ) ? m_CacheParticles.Length : 0; }
        }
        
        public int UIParticleCount
        {
            get { return ( m_CacheUIParticles != null ) ? m_CacheUIParticles.Length : 0; }
        }
        

        public override void Initialize()
        {
            if( isInitialized )
                return;
            
            base.Initialize();
            
            // ----------------------------------------
            
            m_CacheParticles   = gameObject.GetComponentsInChildren<ParticleSystem>( true );
            m_CacheUIParticles = gameObject.GetComponentsInChildren<UIParticleSystem>( true );
            m_AnimationPlayer  = gameObject.GetComponent<AnimationPlayerSimple>();
        }
        

        public override void Release()
        {
            if( isInitialized == false )
                return;
            
            m_AnimationPlayer  = null;
            m_CacheParticles   = null;
            m_CacheUIParticles = null;
            
            // ----------------------------------------
            
            base.Release( );
        }
        
        
        protected override bool CheckDestroy( float dt )
        {
            if( m_DontDestroy )
                return false;
            
            if( base.CheckDestroy( dt ) )
            {
                return true;
            }
            
            if( PlayMode == EPlayMode.None || PlayMode == EPlayMode.OneShot )
            {
                if( m_CacheParticles != null && m_CacheParticles.Length > 0 )
                {
                    for( int i = m_CacheParticles.Length - 1; i >= 0; --i )
                    {
                        ParticleSystem ps = m_CacheParticles[ i ];
                        if( ps.main.loop )
                            continue;
                        
                        if( ps.IsAlive() )
                        {
                            return false;
                        }
                    }
                }
                
                if( m_CacheUIParticles != null && m_CacheUIParticles.Length > 0 )
                {
                    for( int i = m_CacheUIParticles.Length - 1; i >= 0; --i )
                    {
                        UIParticleSystem ps = m_CacheUIParticles[ i ];
                        if( ps.loop )
                            continue;
                        
                        if( ps.IsAlive )
                        {
                            return false;
                        }
                    }
                }
                
                if( m_AnimationPlayer != null )
                {
                    if( m_AnimationPlayer.IsPlay() )
                    {
                        return false;
                    }
                }
                
                return true;
            }
            
            return false;
        }
        
        
        public void SetDontDestroy()
        {
            m_DontDestroy = true;
        }
        

        protected override void OnPlay()
        {
            base.OnPlay();
            
            _StartEmitters();
            
            if( m_AnimationPlayer != null )
            {
                m_AnimationPlayer.enabled = true;
            }
        }


        protected override void OnStop( bool isImmediate )
        {
            if( m_AnimationPlayer != null )
            {
                m_AnimationPlayer.enabled = false;
            }
            
            _StopEmitters( false, isImmediate );
            

            base.OnStop( isImmediate );
        }
        

        protected override void OnStopDestroy( )
        {
            _StopEmitters( true, false );
            
            // ----------------------------------------
            
            base.OnStopDestroy();
        }
        

        protected override void OnPause()
        {
            base.Pause();
            

            _PauseEmitters();

            if( m_AnimationPlayer != null )
            {
                m_AnimationPlayer.enabled = false;
            }
        }


        protected override void OnResume()
        {
            if( m_AnimationPlayer != null )
            {
                m_AnimationPlayer.enabled = true;
            }
            
            _ResumeEmitters();
            

            base.Resume();
        }
        
        
        void _StartEmitters()
        {
            if( m_CacheParticles != null && m_CacheParticles.Length > 0 )
            {
                for( int i = 0, max = m_CacheParticles.Length; i < max; ++i )
                {
                    ParticleSystem ps = m_CacheParticles[i];
                    if( ps == null )
                        continue;
                    
                    ps.Play();
                    
                    ParticleSystem.EmissionModule module = ps.emission;
                    ParticleSystem.MainModule main = ps.main;
                    module.enabled = true;
                }
            }
            
            if( m_CacheUIParticles != null && m_CacheUIParticles.Length > 0 )
            {
                for( int i = 0, max = m_CacheUIParticles.Length; i < max; ++i )
                {
                    UIParticleSystem ps = m_CacheUIParticles[i];
                    if( ps == null )
                        continue;
                    
                    ps.ResetParticleSystem();
                }
            }
        }
        

        void _StopEmitters( bool isStopLoop, bool isImmediate )
        {
            if( m_CacheParticles != null && m_CacheParticles.Length > 0 )
            {
                for( int i = 0, max = m_CacheParticles.Length; i < max; ++i )
                {
                    ParticleSystem ps = m_CacheParticles[i];
                    if( ps == null )
                        continue;
                    
                    ps.Stop();
                    
                    if( isImmediate )
                    {
                        ps.Clear();
                    }
                    
                    if( isStopLoop )
                    {
                        ParticleSystem.MainModule main = ps.main;
                        main.loop = false;
                    }
                }
            }
            
            if( m_CacheUIParticles != null && m_CacheUIParticles.Length > 0 )
            {
                for( int i = 0, max = m_CacheUIParticles.Length; i < max; ++i )
                {
                    UIParticleSystem ps = m_CacheUIParticles[i];
                    if( ps == null )
                        continue;
                    
                    ps.StopEmitters();
                    
                    if( isImmediate )
                    {
                        ps.ResetEmitters();
                    }
                    
                    if( isStopLoop )
                    {
                        ps.loop = false;
                    }
                }
            }
        }
        

        void _PauseEmitters()
        {
            if( m_CacheParticles != null && m_CacheParticles.Length > 0 )
            {
                for( int i = 0, max = m_CacheParticles.Length; i < max; ++i )
                {
                    ParticleSystem ps = m_CacheParticles[i];
                    if( ps == null )
                        continue;
                    
                    if( ps.isPaused == false )
                    {
                        ps.Pause();
                    }
                }
            }
            
            if( m_CacheUIParticles != null && m_CacheUIParticles.Length > 0 )
            {
                for( int i = 0, max = m_CacheUIParticles.Length; i < max; ++i )
                {
                    UIParticleSystem ps = m_CacheUIParticles[i];
                    if( ps == null )
                        continue;
                    
                    ps.PauseEmitters();
                }
            }
        }
        

        void _ResumeEmitters()
        {
            if( m_CacheParticles != null && m_CacheParticles.Length > 0 )
            {
                for( int i = 0, max = m_CacheParticles.Length; i < max; ++i )
                {
                    ParticleSystem ps = m_CacheParticles[i];
                    if( ps == null )
                        continue;
                    
                    if( ps.isPaused )
                    {
                        ps.Play();
                    }
                }
            }
            
            if( m_CacheUIParticles != null && m_CacheUIParticles.Length > 0 )
            {
                for( int i = 0, max = m_CacheUIParticles.Length; i < max; ++i )
                {
                    UIParticleSystem ps = m_CacheUIParticles[i];
                    if( ps == null )
                        continue;
                    
                    ps.ResumeEmitters();
                }
            }
        }
        

        #if UNITY_EDITOR
        
        struct ParticleActiveInfo
        {
            public ParticleSystem   Particle;
            public bool             IsActivate;
            
            public ParticleActiveInfo( ParticleSystem particle )
            {
                Particle = particle;
                IsActivate = false;
                Update();
            }
            
            public void Update()
            {
                if( Particle != null )
                {
                    bool preActivate = IsActivate;
                    IsActivate = Particle.gameObject.activeInHierarchy;
                    
                    if( preActivate == false && IsActivate )
                    {
                        Particle.Simulate( 0, false, true );
                    }
                }
            }
        }
        
        ParticleActiveInfo[]        m_ParticleActiveInfos = new ParticleActiveInfo[0];
        

        public override void EditorResetPhase()
        {
            base.EditorResetPhase();
            

            if( m_CacheUIParticles != null && m_CacheUIParticles.Length > 0 )
            {
                for( int i = 0, max = m_CacheUIParticles.Length; i < max; ++i )
                {
                    UIParticleSystem ps = m_CacheUIParticles[ i ];
                    if( ps == null )
                        continue;
                    
                    ps.ResetEmitters();
                }
            }
        }


        public void GetParticleInfo( out float maxDuration, out bool haveLoop )
        {
            maxDuration = 0;
            haveLoop = false;
            
            if( m_CacheParticles != null && m_CacheParticles.Length > 0 )
            {
                for( int i = 0, max = m_CacheParticles.Length; i < max; ++i )
                {
                    ParticleSystem ps = m_CacheParticles[ i ];
                    if( ps == null )
                        continue;
                    
                    maxDuration = Mathf.Max( ps.main.duration, maxDuration );
                    haveLoop |= ps.main.loop;
                }
            }
            
            if( m_CacheUIParticles != null && m_CacheUIParticles.Length > 0 )
            {
                for( int i = 0, max = m_CacheUIParticles.Length; i < max; ++i )
                {
                    UIParticleSystem ps = m_CacheUIParticles[ i ];
                    if( ps == null )
                        continue;
                    
                    maxDuration = Mathf.Max( ps.duration, maxDuration );
                    haveLoop |= ps.loop;
                }
            }
            
            if( m_AnimationPlayer != null )
            {
                maxDuration = Mathf.Max( m_AnimationPlayer.Clip.length, maxDuration );
            }
        }
        

        protected override void OnEditorInitialize()
        {
            base.OnEditorInitialize();
            
            // ----------------------------------------
            
            if( m_AnimationPlayer != null )
            {
                m_AnimationPlayer.EditorAwake();
                
                if( m_CacheParticles != null && m_CacheParticles.Length > 0 )
                {
                    m_ParticleActiveInfos = new ParticleActiveInfo[ m_CacheParticles.Length ];
                    for( int i = 0, max = m_CacheParticles.Length; i < max; ++i )
                    {
                        m_ParticleActiveInfos[i] = new ParticleActiveInfo( m_CacheParticles[i] );
                    }
                }
            }
        }
        

        public override bool SimulateFromEventEditor( float t, float dt, float startTime, float endTime, bool isRestart, bool isManualMove, EventTrack caller )
        {
            bool ret = base.SimulateFromEventEditor( t, dt, startTime, endTime, isRestart, isManualMove, caller );
            if( ret == true )
            {
                return true;
            }

            if( m_CacheParticles != null && m_CacheParticles.Length > 0 )
            {
                for( int i = 0, max = m_CacheParticles.Length; i < max; ++i )
                {
                    ParticleSystem ps = m_CacheParticles[ i ];
                    if( ps == null )
                        continue;
                    
                    if( isManualMove )
                    {
                        ps.Simulate( t, false, true );
                    }
                    else
                    {
                        ps.Simulate( dt, false, isRestart );
                    }
                }
            }
            
            if( m_CacheUIParticles != null && m_CacheUIParticles.Length > 0 )
            {
                for( int i = 0, max = m_CacheUIParticles.Length; i < max; ++i )
                {
                    UIParticleSystem ps = m_CacheUIParticles[ i ];
                    if( ps == null )
                        continue;
                    
                    if( isRestart )
                    {
                        ps.Simulate( t, isRestart );
                    }
                    else
                    {
                        ps.Simulate( dt, isRestart );
                    }
                }
            }
            
            if( m_AnimationPlayer != null )
            {
                m_AnimationPlayer.UpdateManual( t, dt );
            }
            
            if( m_ParticleActiveInfos.Length > 0 )
            {
                for( int i = 0, max = m_ParticleActiveInfos.Length; i < max; ++i )
                {
                    m_ParticleActiveInfos[ i ].Update();
                }
            }
            
            return false;
        }
        
        public override void SimulateNew( float t, float dt, bool isRestart )
        {
            if( m_CacheParticles != null && m_CacheParticles.Length > 0 )
            {
                for( int i = 0, max = m_CacheParticles.Length; i < max; ++i )
                {
                    ParticleSystem ps = m_CacheParticles[ i ];
                    if( ps == null )
                        continue;
                    
                    if( isRestart )
                    {
                        ps.Simulate( t, false, isRestart );
                    }
                    else
                    {

                        ps.Simulate( dt, false, isRestart );
                    }
                }
            }
            
            if( m_CacheUIParticles != null && m_CacheUIParticles.Length > 0 )
            {
                for( int i = 0, max = m_CacheUIParticles.Length; i < max; ++i )
                {
                    UIParticleSystem ps = m_CacheUIParticles[ i ];
                    if( ps == null )
                        continue;
                    
                    if( isRestart )
                    {
                        ps.Simulate( t, isRestart );
                    }
                    else
                    {
                        ps.Simulate( dt, isRestart );
                    }
                }
            }
            
            if( m_AnimationPlayer != null )
            {
                m_AnimationPlayer.UpdateManual( t, dt );
            }
            
            if( m_ParticleActiveInfos.Length > 0 )
            {
                for( int i = 0, max = m_ParticleActiveInfos.Length; i < max; ++i )
                {
                    m_ParticleActiveInfos[ i ].Update();
                }
            }
        }

        #endif // UNITY_EDITOR
    }
}
