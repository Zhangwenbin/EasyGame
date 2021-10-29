using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace EG
{
    [AddComponentMenu("Scripts/Application/Common/Effect/EffectDataEvent.cs")]
    public class EffectDataEvent : EffectData
    {
        const string        EVENT_KEY       = "EffectEv";
        
        
        EventPlayer         m_EvPlayer    = null;
        
        public override void Initialize()
        {
            if( isInitialized )
                return;
            
            base.Initialize();
            
            // ----------------------------------------
            
            m_EvPlayer = gameObject.AddComponent<EventPlayer>();
        }
        
        public override void Release()
        {
            if( isInitialized == false )
                return;
            
            
            // ----------------------------------------
            
            base.Release( );
        }
        

        protected override bool CheckDestroy( float dt )
        {
            if( base.CheckDestroy( dt ) )
            {
                return true;
            }
            
            if( m_EvPlayer.GetRemainingTime( EVENT_KEY ) == 0 )
            {
                return true;
            }

            return false;
        }


        public void SetEventParam( EventParam evParam )
        {
            if( m_EvPlayer != null )
            {
                m_EvPlayer.AddEvent( EVENT_KEY, evParam );
            }
        }

        
        protected override void OnPlay()
        {
            base.OnPlay();

            // ----------------------------------------
            
            m_EvPlayer.PlayEvent( EVENT_KEY );
        }
        

        protected override void OnStop( bool isImmediate )
        {

            // ----------------------------------------
            
            base.OnStop( isImmediate );
        }
        

        protected override void OnStopDestroy( )
        {
            
            // ----------------------------------------
            
            base.OnStopDestroy();
        }
        

        protected override void OnPause()
        {
            base.Pause();
            
            // ----------------------------------------
            
        }
        

        protected override void OnResume()
        {
            
            // ----------------------------------------
            
            base.Resume();
        }
        
    }
}
