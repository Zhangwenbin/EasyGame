using System.Collections;
using MotionFramework.Config;
using UnityEngine;

namespace EG
{
    public class InitState:IGameState
    {
        public const string name = "init";
        public override string Name
        {
            get { return name; }
        }

        public override IEnumerator OnEnter()
        {
            //初始化资源
            AssetManager.Instance.Initialize();
            while (!AssetManager.Instance.isInitialized)
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
            
            //todo 初始化音频
            
            
            //进入主场景
            Debug.Log("init finish");
            var loading= WindowManager.Instance.OpenWindow<UILoading>();
            while (!loading.IsDone)
            {
                yield return null;
            }
            LoadingScreen.Instance.gameObject.SetActive(false);
            Game.Goto("home");
            
        }

        public override void OnUpdate()
        {
            
        }

        public override void OnExit()
        {
            
        }
    }
}