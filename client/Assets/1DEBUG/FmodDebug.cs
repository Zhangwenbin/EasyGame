using System;
using System.Collections;
using System.Collections.Generic;
using EG;
using FMODUnity;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class FmodDebug : MonoBehaviour
{
    
    [Range(0,100)]
    public float volume;


    [EventRef]
    public string[] ambient;

    private int amIndex;
    
    [EventRef]
    public string[] music;

    private int muIndex;

    [EventRef]
    public string sound;
    private void Awake()
    {
        FmodManager.Instance.Initialize();
    }
    

    private void Update()
    {
        if (Keyboard.current.pKey.isPressed)
        {
            FmodManager.Instance.PlayAmbient(ambient[(amIndex++)%2]);
            FmodManager.Instance.PlayMusic(music[(muIndex++) % 2]);
            FmodManager.Instance.PlaySound(sound);
        }
        if (Keyboard.current.oKey.isPressed)
        {
            FmodManager.Instance.StopAmbient();
            FmodManager.Instance.StopMusic();
            FmodManager.Instance.StopSound();
        }
    }
}
