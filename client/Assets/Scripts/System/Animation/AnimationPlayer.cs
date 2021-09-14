using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
                
            }
        }
    }
}