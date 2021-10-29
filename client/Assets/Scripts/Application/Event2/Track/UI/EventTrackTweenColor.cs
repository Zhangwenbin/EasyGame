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
    public class EventTrackTweenColor : EventTrackTweener
    {
        public class Status : StatusBase
        {
            EventTrackTweenColor m_Tween;
            
            public Color Value
            {
                set
                {
                    if( m_Tween.InChild == false )
                    {
                        Graphic graph = graphic;
                        if( graph != null )
                        {
                            graph.color = value;
                        }
                    }
                    else
                    {
                        Graphic[] graphs = graphics;
                        if( graphs != null )
                        {
                            for( int i = 0; i < graphs.Length; ++i )
                            {
                                graphs[i].color = value;
                            }
                        }
                    }
                }
                get
                {
                    Graphic graph = graphic;
                    if( graph != null )
                    {
                        return graph.color;
                    }
                    return Color.white;
                }
            }
            
            public Status( EventPlayerStatus owner ) : base( owner )
            {
            }
 
            public override void Initialize( AppMonoBehaviour behaviour, EventTrack track )
            {
                base.Initialize( behaviour, track );
                
                m_Tween = Track as EventTrackTweenColor;
            }
        }
        
        
        [CustomFieldGroup("设置")]
        [CustomFieldAttribute("包括孩子",CustomFieldAttribute.Type.Bool)]
        public bool                 InChild = false;
        
        [CustomFieldGroup("设置")]
        [CustomFieldAttribute("开始值",CustomFieldAttribute.Type.Color)]
        public Color                From;
        
        [CustomFieldGroup("设置")]
        [CustomFieldAttribute("结束值",CustomFieldAttribute.Type.Color)]
        public Color                To;
        


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
        
        new public static string    ClassName   { get { return "Tween/Color";      } }
        public override Color       TrackColor  { get { return Color.yellow;        } }
        
        #endif
    }
}
