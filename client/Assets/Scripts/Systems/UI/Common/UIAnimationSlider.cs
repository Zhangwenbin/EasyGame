using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;

namespace EG
{
    [AddComponentMenu("Scripts/Application/UIAnimationSlider.cs")]
    public class UIAnimationSlider : AppMonoBehaviour
    {
        
        public Slider                       Slider              = null;
        
        // --------------------------------------------
        
        private Action<UIAnimationSlider>   m_ActionUpdate      = null;
        private Action<UIAnimationSlider>   m_ActionReset       = null;
        private int                         m_StartValue        = 0;
        private int                         m_Value             = 0;
        private float                       m_ValueF            = 0;
        private int                         m_EndValue          = 0;
        private float                       m_AnimateTime       = 0;
        private float                       m_CurrentTime       = 0;

        private bool                        m_IsFull            = false;
        

        private int                         m_SliderMax         = 0;
        private int                         m_SliderMin         = 0;

        private bool                        m_Direction         = true;

        public float                        RangeValue          { get { return Slider.value; } }
        public float                        RangeMinValue       { get { return Slider.minValue; } }
        public float                        RangeMaxValue       { get { return Slider.maxValue; } }
        
        public float                        Value               { get { return m_ValueF; } }
        public int                          ValueInt            { get { return m_Value;  } }
        public float                        MinValue            { get { return m_StartValue; } }
        public float                        MaxValue            { get { return m_EndValue; } }
        public float                        Rate
        {
            get
            {
                float t = m_AnimateTime == 0 ? 1: Mathf.Clamp01( m_CurrentTime / m_AnimateTime );
                t = Mathf.Sin( t * Mathf.Deg2Rad * 90.0f );
                return t;
            }
        }
        
        public bool                         isDone              { get { return m_CurrentTime >= m_AnimateTime; } }
        

        public override void Initialize( )
        {
            base.Initialize( );
            
            // --------------------------------------------
            
            m_CurrentTime = 0;
            m_AnimateTime = 1;
        }
        
        public override void Release( )
        {
            // --------------------------------------------
            
            base.Release( );
        }
        
        private void Update( )
        {
            if( m_IsFull == false )
            {
                if((m_Direction && m_Value >= m_SliderMax) || (!m_Direction && m_Value <= m_SliderMin))
                {
                    if( m_ActionReset != null )
                    {
                        m_ActionReset.Invoke( this );
                        m_ActionReset = null;
                    }
                    else
                    {
                        m_CurrentTime = m_AnimateTime;
                    }
                    return;
                }
            }
            
            if( isDone == false )
            {
                m_CurrentTime += Time.deltaTime;
                
                Refresh( );
            }
        }
        
        public void Refresh( )
        {
            float t = m_AnimateTime == 0 ? 1: Mathf.Clamp01( m_CurrentTime / m_AnimateTime );
            t = Mathf.Sin( t * Mathf.Deg2Rad * 90.0f );
            if( t >= 1.0f )
            {
                m_Value = m_EndValue;
                m_ValueF = m_EndValue;
            }
            else
            {
                m_ValueF = Mathf.Lerp( m_StartValue, m_EndValue, t );
                m_Value  = (int)m_ValueF;
            }
            
            if( m_IsFull )
            {
                Slider.value = Slider.maxValue;
            }
            else
            {
                Slider.value = m_ValueF;
            }
            
            if( m_ActionUpdate != null )
            {
                m_ActionUpdate.Invoke( this );
            }
        }
        
        public void SetRange( int min, int max )
        {
            m_SliderMin = min;
            Slider.minValue = min;
            m_SliderMax = max;
            Slider.maxValue = max;
        }
        
        public void SetValue( float value )
        {
            Slider.value = value;
        }
        
        public void SetActionReset( Action<UIAnimationSlider> action )
        {
            m_ActionReset = action;
        }
        
        public void SetActionUpdate( Action<UIAnimationSlider> action )
        {
            m_ActionUpdate = action;
        }
        
        public void Play( int start, int end, float time, bool isFull = false )
        {
            m_StartValue = start;
            m_EndValue = end;
            m_AnimateTime = time;
            m_CurrentTime = 0;
            m_IsFull = isFull;
            if( m_IsFull )
            {
                SetDispFull();
                m_StartValue = m_EndValue;
            }
            m_Direction = m_EndValue >= m_StartValue;
            
            Refresh( );
        }
        
        public void SetDispFull()
        {
            SetRange( 0, 1 );
            SetValue( 1 );
        }
    }
}
