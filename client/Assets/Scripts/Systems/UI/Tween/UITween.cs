using UnityEngine;
using System.Collections;
using System;

namespace EG
{
    [AddComponentMenu("Scripts/System/UI/Tween/UITween")]
    public abstract class UITween<T> : UITweener
    {
        [HideInInspector]
        public T from;
        [HideInInspector]
        public T to;
        
        public virtual T value { get; set; }
        
        [ContextMenu("Set 'From' to current value")]
        public override void SetStartToCurrentValue()   { from = value;     }
        
        [ContextMenu("Set 'To' to current value")]
        public override void SetEndToCurrentValue()     { to = value;       }
        
        [ContextMenu("Assume value of 'From'")]
        public override void SetCurrentValueToStart()   { value = from;     }
        
        [ContextMenu("Assume value of 'To'")]
        public override void SetCurrentValueToEnd()     { value = to;       }
    }
}
