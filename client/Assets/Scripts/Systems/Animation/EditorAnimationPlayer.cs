using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.Playables;
using UnityEngine.Animations;

namespace EG
{
#if UNITY_EDITOR
    public class EditorAnimationPlayer : EventPlayer
    {
        
        public void _SetPlayableTime( float time )
        {
            if (AnimationPlayer.Graph.IsValid() == false)
                return;

            int outputCnt = AnimationPlayer.Graph.GetOutputCount();
            if (outputCnt == 0)
                return;

            for (int i = 0; i < outputCnt; ++i)
            {
                PlayableOutput output = AnimationPlayer.Graph.GetOutput(i);
                if (output.IsOutputValid() == false)
                    continue;

                _SetTimeToPlayable(output.GetSourcePlayable(), time);
            }
        }
        
        void _SetTimeToPlayable( Playable playable, float time )
        {
            if( playable.IsValid() == false )
                return ;
            
            playable.SetTime( time );
            
            int inputCnt = playable.GetInputCount();
            if( inputCnt > 0 )
            {
                for( int i = 0; i < inputCnt; ++i )
                {
                    _SetTimeToPlayable( playable.GetInput( i ), time );
                }
            }
        }
        
        [UnityEditor.CustomEditor(typeof(EditorAnimationPlayer))]
        public class EditorInspector_EditorAnimationPlayer: EditorInspector_EventPlayer
        {
            public override void OnInspectorGUI()
            {
                base.OnInspectorGUI( );
            }
        }
    }
#endif
    
}
