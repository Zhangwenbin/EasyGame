using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace EG
{
    public class EventTrackTweenAlpha : EventTrackTweener
    {
        public class Status : StatusBase
        {
            EventTrackTweenAlpha m_Tween;
            
            public float Value
            {
                set
                {
                    CanvasGroup obj = canvasGroup;
                    if( obj != null )
                    {
                        obj.alpha = value;
                    }
                    else
                    {
                        if( m_Tween.InChild == false )
                        {
                            Graphic graph = graphic;
                            if( graph != null )
                            {
                                Color color = graph.color;
                                color.a = value;
                                graph.color = color;
                            }
                        }
                        else
                        {
                            Graphic[] graphs = graphics;
                            if( graphs != null )
                            {
                                for( int i = 0; i < graphs.Length; ++i )
                                {
                                    Color color = graphs[i].color;
                                    color.a = value;
                                    graphs[i].color = color;
                                }
                            }
                        }
                    }
                }
                get
                {
                    CanvasGroup obj = canvasGroup;
                    if( obj != null )
                    {
                        return obj.alpha;
                    }
                    else
                    {
                        Graphic graph = graphic;
                        if( graph != null )
                        {
                            return graph.color.a;
                        }
                    }
                    return 1;
                }
            }
            
            public Status( EventPlayerStatus owner ) : base( owner )
            {
            }
            
            public override void Initialize( AppMonoBehaviour behaviour, EventTrack track )
            {
                base.Initialize( behaviour, track );
                
                m_Tween = Track as EventTrackTweenAlpha;
            }
        }
        
        [CustomFieldGroup("设置")]
        [CustomFieldAttribute("包括孩子", CustomFieldAttribute.Type.Bool)]
        public bool                 InChild = false;
        
        [CustomFieldGroup("设置")]
        [CustomFieldAttribute("开始",CustomFieldAttribute.Type.Float)]
        public float                From;
        
        [CustomFieldGroup("设置")]
        [CustomFieldAttribute("结束",CustomFieldAttribute.Type.Float)]
        public float                To;
        

        
        public override EventTrackStatus CreateStatus( EventPlayerStatus owner )
        {
            return new Status( owner );
        }
        
        public override void OnStart(AppMonoBehaviour behaviour )
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
        
        new public static string    ClassName   { get { return "Tween/透明度";     } }
        public override Color       TrackColor  { get { return Color.yellow; } }
        
        #endif
    }
}
