using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;

namespace EG
{
    public class AnimationPlayer
    {

        private static readonly int MIXER_COUNT = 3;
        public enum EFlag
        {
            None=0,
            SamePlay=1<<0,
            BlendTimeOverride=1<<1,
        }
       
        public class PlayStatus
        {
            public enum State
            {
                Keep,
                BlendUp,
                BlendDown,
            }

            public State _state;
            public AnimationClip _clip;
            public AnimationClipPlayable _clip_playable;
            public float _weight;
            public short _mixer_layer = -1;
            public short _mixer_id = -1;

            public bool IsActive()
            {
                return (_state == State.Keep || _state == State.BlendUp);
            }

            public bool IsLoop()
            {
                return (_clip != null && _clip.isLooping);
            }
        }

        private Animator _animator;
        private PlayableGraph _graph;
        private AnimationLayerMixerPlayable _mixer_layer;
        private AnimationMixerPlayable[] _mixer;
        private float _blend_time;
        public float _speed = 1;
        private List<PlayStatus> _play_statuss;
        private BitFlag<EFlag>                  m_Flag          = new BitFlag<EFlag>( );
        private float                           m_BlendTime     = 0;
        public Animator Animator { get { return _animator; } }
        public PlayableGraph Graph { get { return _graph; } }

        public void Create(Animator animator, int layer_num = 1,
            DirectorUpdateMode updateMode = DirectorUpdateMode.GameTime)
        {
            
        }

        public void Destroy()
        {
            _play_statuss = null;
            if (_graph.IsValid())
            {
                _graph.Destroy();
            }
        }
    }
}