using System;
using UnityEngine;

public class LoadingScreen:MonoBehaviour
{
    public static LoadingScreen Instance;
    private void Awake()
    {
        Instance = this;
    }

    public static void SetTipsStatic(int v)
    {
        
    }

    public static void SetProgress(float v)
    {
       
    }
}