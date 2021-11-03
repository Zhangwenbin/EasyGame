﻿using System.Collections;
using MotionFramework.Config;
using UnityEngine;

namespace EG.GameState
{
    public class InitState:IState
    {
        public const string name = "InitState";
        public override string Name
        {
            get { return name; }
        }

        public override IEnumerator OnEnter(FsmMachine fsmMachine)
        {
           yield return base.OnEnter(fsmMachine);
            //初始化资源
            AssetManager.Instance.Initialize();
            while (!AssetManager.Instance.IsInitialized())
            {
                yield return null;
            }
            
            //初始化配置
            yield return ConfigManager.Instance.LoadConfig();
            
            // 初始化UI
            var uiRoot = WindowManager.Instance.CreateUIRoot<CanvasRoot>("UIRoot.prefab");
            yield return uiRoot;
            uiRoot.Go.transform.position = new Vector3(1000, 0, 1);
            
            //初始化数据
            DataManager.Instance.Initialize();
            
            // AudioManager.Instance.Initialize();
            FmodManager.Instance.Initialize();
            
            //进入主场景
            Debug.Log("init finish");
            var loading= WindowManager.Instance.OpenWindow<UILoading>();
            while (!loading.IsDone)
            {
                yield return null;
            }
            LoadingScreen.Instance.gameObject.SetActive(false);
            GotoState(HomeState.name);
            
        }

        public override void OnUpdate()
        {
            
        }

        public override void OnExit()
        {
            
        }

        public override bool CanGoto(IState next)
        {
            if (next is InitState)
            {
                return false;
            }

            return true;
        }
    }
}