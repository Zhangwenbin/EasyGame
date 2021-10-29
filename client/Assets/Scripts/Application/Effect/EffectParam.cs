using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.ProjectWindowCallback;
#endif

namespace EG
{
    public class EffectParam : ScriptableObject
    {
        
        [CustomFieldAttribute( "Prefab", CustomFieldAttribute.Type.GameObject )]
        public GameObject                   m_Prefab        = null;
        
        [CustomFieldAttribute( "EffectData.Param", CustomFieldAttribute.Type.Object, typeof( EffectData.Param ) )]
        public EffectData.Param             m_Param;
        
        [CustomFieldAttribute( "EventParam", CustomFieldAttribute.Type.Component, typeof(EventParam) )]
        public EventParam                   m_EvParam       = null;
        
        [CustomFieldAttribute( "AttachedUIPrefab", CustomFieldAttribute.Type.GameObject )]
        public GameObject                   m_AttachedUIPrefab      = null; 
        
        
        public EffectData.Param Param
        {
            get { return m_Param; }
        }


        public EffectData CreateEffect( Vector3 pos, Quaternion rot )
        {
            if( m_Prefab != null )
                return CreateEffect<EffectDataParticle>( pos, rot );
            
            if( m_EvParam != null )
                return CreateEffect<EffectDataEvent>( pos, rot );
            
            return null;
        }

        public EffectData CreateEffect( ref EffectData.Param param, Vector3 pos, Quaternion rot )
        {
            if( m_Prefab != null )
                return CreateEffect<EffectDataParticle>( ref param, pos, rot );
            
            if( m_EvParam != null )
                return CreateEffect<EffectDataEvent>( ref param, pos, rot );
            
            return null;
        }

        public EffectData CreateEffect( ref GameUtility.AttachParam attachParam )
        {
            if( m_Prefab != null )
                return CreateEffect<EffectDataParticle>( ref m_Param, ref attachParam );
            
            if( m_EvParam != null )
                return CreateEffect<EffectDataEvent>( ref m_Param, ref attachParam );
            
            return null;
        }

        public EffectData CreateEffect( ref EffectData.Param param, ref GameUtility.AttachParam attachParam )
        {
            if( m_Prefab != null )
                return CreateEffect<EffectDataParticle>( ref param, ref attachParam );
            
            if( m_EvParam != null )
                return CreateEffect<EffectDataEvent>( ref param, ref attachParam );
            
            return null;
        }
        
        public EffectData CreateEffectParticle( Vector3 pos, Quaternion rot )
        {
            return CreateEffect<EffectDataParticle>( pos, rot );
        }
        
        public EffectData CreateEffectParticle( ref EffectData.Param param, Vector3 pos, Quaternion rot )
        {
            return CreateEffect<EffectDataParticle>( ref param, pos, rot );
        }
        
        public EffectData CreateEffectParticle( ref GameUtility.AttachParam attachParam )
        {
            return CreateEffect<EffectDataParticle>( ref m_Param, ref attachParam );
        }
        
        public EffectData CreateEffectParticle( ref EffectData.Param param, ref GameUtility.AttachParam attachParam )
        {
            return CreateEffect<EffectDataParticle>( ref param, ref attachParam );
        }


        public EffectData CreateEffectEvent( Vector3 pos, Quaternion rot )
        {
            return CreateEffect<EffectDataEvent>( pos, rot );
        }

        public EffectData CreateEffectEvent( ref EffectData.Param param, Vector3 pos, Quaternion rot )
        {
            return CreateEffect<EffectDataEvent>( ref param, pos, rot );
        }

        public EffectData CreateEffectEvent( ref GameUtility.AttachParam attachParam )
        {
            return CreateEffect<EffectDataEvent>( ref m_Param, ref attachParam );
        }

        public EffectData CreateEffectEvent( ref EffectData.Param param, ref GameUtility.AttachParam attachParam )
        {
            return CreateEffect<EffectDataEvent>( ref param, ref attachParam );
        }


        public T CreateEffect<T>( Vector3 pos, Quaternion rot ) where T : EffectData
        {
            return CreateEffect<T>( ref m_Param, pos, rot );
        }
        
        public T CreateEffect<T>( ref GameUtility.AttachParam attachParam ) where T : EffectData
        {
            return CreateEffect<T>( ref m_Param, ref attachParam );
        }
        
        public T CreateEffect<T>( ref EffectData.Param param, Vector3 pos, Quaternion rot ) where T : EffectData
        {
            if( m_Prefab == null && m_EvParam == null )
                return null;
            
            GameObject gobj = null;
            if( m_Prefab != null )
            {
                gobj = GameObject.Instantiate( m_Prefab, pos, rot ) as GameObject;
            }
            else if( m_EvParam != null )
            {
                gobj = new GameObject( m_EvParam.name );
                gobj.transform.position = pos;
                gobj.transform.localRotation = rot;
            }
            T effData = gobj.RequireComponent<T>();
            effData.SetParam( ref param );
            if( effData is EffectDataEvent )
            {
                EffectDataEvent effEv = effData as EffectDataEvent;
                if( effEv != null )
                {
                    effEv.SetEventParam( m_EvParam );
                }
            }
            
            
            return effData;
        }
        
        public T CreateEffect<T>( ref EffectData.Param param, ref GameUtility.AttachParam attachParam ) where T : EffectData
        {
            if( m_Prefab == null && m_EvParam == null )
                return null;
            
            Vector3 spawnPos;
            Quaternion spawnRot;
            GameUtility.CalcAttachPosition( ref attachParam, m_Prefab, out spawnPos, out spawnRot );
            
            GameObject gobj = null;
            if( m_Prefab != null )
            {
                gobj = GameObject.Instantiate( m_Prefab, spawnPos, spawnRot ) as GameObject;
            }
            else if( m_EvParam != null )
            {
                gobj = new GameObject( m_EvParam.name );
                gobj.transform.position = spawnPos;
                gobj.transform.localRotation = spawnRot;
            }
            T effData = gobj.RequireComponent<T>();
            effData.SetParam( ref param );
            if( effData is EffectDataEvent )
            {
                EffectDataEvent effEv = effData as EffectDataEvent;
                if( effEv != null )
                {
                    effEv.SetEventParam( m_EvParam );
                }
            }
            
            if( attachParam.AttachTarget != null )
            {
                effData.Attach( attachParam.AttachTarget );
            }
            
            return effData;
        }
        
        #if UNITY_EDITOR
        
        public void SetParam( GameObject prefab, EventParam evParam, ref EffectData.Param param )
        {
            m_Prefab = prefab;
            m_EvParam = evParam;
            m_Param = param;
        }
        
        #endif
    }
}
