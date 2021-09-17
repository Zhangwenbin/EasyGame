using System.Collections;
using MotionFramework.Config;
using UnityEngine;

namespace EG
{
    public class InitState:IGameState
    {
        public override string Name
        {
            get { return "InitState"; }
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
            // 加载UIRoot
            var uiRoot = WindowManager.Instance.CreateUIRoot<CanvasRoot>("UIRoot.prefab");
            yield return uiRoot;
            uiRoot.Go.transform.position = new Vector3(1000, 0, 1);
            
            // 常驻面板加载
            WindowManager.Instance.OpenWindow<UITest>();
            Debug.Log(GetLanguage("UILogin1"));
            
        }
        public  string GetLanguage(string key, params object[] args)
        {
            var cfgLanguage = ConfigManager.Instance.GetConfig<CfgLanguage>();
            var table = cfgLanguage.GetTable(key.GetHashCode()) as CfgLanguageTable;
            if (table != null)
            {
                return string.Format(table.Lang, args);
            }
            return key;
        }

        public override void OnUpdate()
        {
            
        }

        public override void OnExit()
        {
            
        }
    }
}