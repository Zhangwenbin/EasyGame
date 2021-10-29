using System;
using System.Collections;
using System.Collections.Generic;
using EG.SceneState;
using UnityEngine;

namespace EG
{
    public class SceneManager :  MonoSingleton<SceneManager>
    {
        private FsmMachine _fsmMachine;
        public override void Initialize()
        {
            _fsmMachine = new FsmMachine();
            _fsmMachine.Initialize(RunningState.name,this,"EG.SceneState");
        }

        public void LoadScene(string name)
        {
            _fsmMachine.GoTo(LoadingState.name,name);
        }

        private void Update()
        {
            _fsmMachine.Update();
        }

        void Awake()
        {
            Initialize();
            UnityEngine.SceneManagement.SceneManager.sceneLoaded += OnSceneLoaded;
        }

        public virtual void OnSceneLoaded( UnityEngine.SceneManagement.Scene sceneName, UnityEngine.SceneManagement.LoadSceneMode loadSceneMode )
        {
            Events<string>.Broadcast(EventsType.onSceneLoaded,sceneName.name);
        }
    }

}