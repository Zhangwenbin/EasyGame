using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using ILRuntime.CLR.Method;
using ILRuntime.CLR.TypeSystem;
using UnityEngine;
using ILRuntime.Runtime.Enviorment;
using MotionFramework.Config;

namespace EG
{
    public class Main : MonoBehaviour
    {
        private FsmMachine _fsmMachine;
        private void Start()
        {
            DontDestroyOnLoad(gameObject);
            _fsmMachine = new FsmMachine();
            _fsmMachine.Initialize("InitState",this);
        }

        private void Update()
        {
            _fsmMachine.Update();
        }
    }
}