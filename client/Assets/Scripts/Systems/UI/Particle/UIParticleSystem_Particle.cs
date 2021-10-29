using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

namespace EG
{
    public partial class UIParticleSystem : MaskableGraphic
    {
        public class Particle
        {
            public Vector3 angularVelocity;
            public Vector3 axisOfRotation;
            public Color32 color;
            public float   lifetime;
            public Vector3 position;
            public int     randomSeed;
            public Vector3 rotation;
            public float   size;
            public float   startLifetime;
            public Vector3 startRotation;
            public Vector3 velocity;
            public Vector2 visualVelocity;
            

            float               m_StretchLengScl    = 0;                // 
            float               m_TexAnmStartFrame  = 0;                // 
            
            Vector3             m_CreatedPos        = Vector3.zero;     // 
            float               m_TotalDampen       = 1.0f;
            
            UIParticleSystem    m_Parent            = null;
            RectTransform       m_ParentRectTrans   = null;
            

            public float Fraction
            {
                get
                {
                    if( startLifetime > 0 )
                    {
                        return 1.0f - lifetime / startLifetime;
                    }
                    return 1.0f;
                }
            }
            
            public float StretchLengScl     { get { return m_StretchLengScl;    } }
            public float TexAnmStartFrame   { get { return m_TexAnmStartFrame;  } }
            
            
            public Particle( UIParticleSystem parent )
            {
                m_Parent            = parent;
                m_ParentRectTrans   = parent.rectTransform;
                m_StretchLengScl    = 0;
                m_TexAnmStartFrame  = 0;
            }
            
            public void Setup( ref Vector3 pos, ref Vector3 vel )
            {
                m_TotalDampen = 1.0f;
                
                randomSeed = Random.Range( 0, 0xFFFF );
                startLifetime =
                lifetime = m_Parent.startLifetime.Evaluate();
                color = m_Parent.startColor.Evaluate();
                rotation =
                startRotation = m_Parent.startRotation.Evaluate();
                size = m_Parent.startSize.Evaluate();
                
                // 
                angularVelocity = Vector3.zero;
                if( m_Parent.angularVelocityEnable )
                {
                    angularVelocity = m_Parent.angularVelocity.Evaluate();
                }
                
                position = pos;
                velocity = vel;
                
                velocity *= m_Parent.startSpeed.Evaluate();
                
                // 
                m_CreatedPos = m_ParentRectTrans.localPosition;
            }
            
            Vector3 m_CalcTmpVel    = Vector3.zero;
            
            // 
            public bool Update( float dt )
            {
                lifetime -= dt;
                if( lifetime <= 0 )
                {
                    return true;
                }
                
                float fraction = Fraction;
                

                m_CalcTmpVel = velocity;
                
                if( m_Parent.velocityOverLifetimeEnable )
                {

                    float vx = m_Parent.velocityOverLifetime.CalcX( fraction );
                    float vy = m_Parent.velocityOverLifetime.CalcY( fraction );
                    m_CalcTmpVel.x += vx;
                    m_CalcTmpVel.y += vy;
                }
                
                if( m_Parent.limitVelocityOverLifetimeEnable )
                {
                    // 
                    m_TotalDampen = m_Parent.limitVelocityOverLifetime.Calc( m_TotalDampen, m_CalcTmpVel.magnitude );
                    m_CalcTmpVel *= m_TotalDampen;
                }
                
                position += m_CalcTmpVel * dt;
                
                visualVelocity = m_CalcTmpVel;
                
                if( m_Parent.gravityMultipler > 0.0f )
                {
                    velocity += Physics.gravity * dt;
                }
                
                rotation += angularVelocity * dt;
                if( m_Parent.rotationOverLifetimeEnable )
                {
                    float frac = m_Parent.rotationOverLifetime.Evaluate( fraction );
                    rotation.x = startRotation.x + frac;
                    rotation.y = startRotation.y + frac;
                    rotation.z = startRotation.z + frac;
                }
                
                return false;
            }
            

            public void SetStretchLength( float length )
            {
                m_StretchLengScl = length;
            }
            
            public void SetTexAnmStartFrame( float frame )
            {
                m_TexAnmStartFrame = frame;
            }
            
            public Vector3 GetPos( SimulationSpace simSpace )
            {
                if( simSpace == SimulationSpace.World )
                {
                    return m_CreatedPos - m_ParentRectTrans.localPosition + position;
                }
                return position;
            }
        }
    }
}
