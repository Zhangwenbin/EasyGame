﻿using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace EG
{
    public class EventTrackTweenRotation : EventTrackTweener
    {
        public class Status : StatusBase
        {
            public float Value
            {
                set
                {
                    RectTransform trs = rectTransform;
                    if( trs != null )
                    {
                        trs.localRotation = Quaternion.Euler( 0, 0, value );
                    }
                }
                get
                {
                    RectTransform trs = rectTransform;
                    if( trs != null )
                    {
                        return trs.eulerAngles.z;
                    }
                    return 0;
                }
            }
            
            public Status( EventPlayerStatus owner ) : base( owner )
            {
            }
        }
        
        [CustomFieldGroup("设置")]
        [CustomFieldAttribute("开始值",CustomFieldAttribute.Type.Float)]
        public float                From;
        
        [CustomFieldGroup("设置")]
        [CustomFieldAttribute("结束值",CustomFieldAttribute.Type.Float)]
        public float                To;
        
        public override EventTrackStatus CreateStatus( EventPlayerStatus owner )
        {
            return new Status( owner );
        }
        
        public override void OnStart( AppMonoBehaviour behaviour )
        {
            Status status = CurrentStatus as Status;
            if( status != null )
            {
                status.Value = Sample( From, To, 0 );
            }
        }
        
  
        public override void OnUpdate( AppMonoBehaviour behaviour, float time )
        {
            Status status = CurrentStatus as Status;
            if( status != null )
            {
                float t = GetTimeScale( time );
                status.Value = Sample( From, To, t );
            }
        }
        
        public override void OnEnd( AppMonoBehaviour behaviour )
        {
            Status status = CurrentStatus as Status;
            if( status != null )
            {
                status.Value = Sample( From, To, 1 );
            }
        }
        
        #if UNITY_EDITOR
        
        new public static string    ClassName   { get { return "Tween/旋转";     } }
        public override Color       TrackColor  { get { return Color.yellow; } }
        
        #endif
    }
}