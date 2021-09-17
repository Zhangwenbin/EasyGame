using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using ILRuntime.CLR.Method;
using ILRuntime.CLR.TypeSystem;
using UnityEngine;
using ILRuntime.Runtime.Enviorment;

namespace EG
{
    public class Main : MonoBehaviour
    {
        IEnumerator Start()
        {
            AssetManager.Instance.Initialize();
            while (!AssetManager.Instance.isInitialized)
            {
                yield return null;
            }
        }


        void Update()
        {
        }
    }
}