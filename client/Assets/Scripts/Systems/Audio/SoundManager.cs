using System.Collections;
using System.Collections.Generic;
using EG;
using UnityEngine;

public class SoundManager : MonoSingleton<SoundManager>
{
    public enum ESe
    {
        TAP                     ,
        OK                      ,
        CANCEL                  ,
        SELECT                  ,
        BUZZER                  ,
        SWIPE                   ,
        SCROLL_LIST             ,
        WINDOW_POP              ,
        WINDOW_CLOSE            ,
        CUSTOM_POWERSAVE_OFF    ,
    }
}
