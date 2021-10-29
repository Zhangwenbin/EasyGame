using UnityEngine;
using System.Collections;

namespace EG
{
    public enum UIDirection
    {
        Reverse = -1,
        Toggle = 0,
        Forward = 1
    }
    
    public enum UITrigger
    {
        OnPointerEnter,
        OnPointerDown,
        OnPointerClick,
        OnPointerUp,
        OnPointerExit,
    }
}