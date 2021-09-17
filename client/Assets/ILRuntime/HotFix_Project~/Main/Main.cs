using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using MotionFramework.Config;
using System.Collections;

namespace HotFix_Project
{
    class Main
    {
        private GameObject mainRoot;
        public void Start(MonoBehaviour g)
        {
            Debug.Log("start");
            mainRoot = g.gameObject;
            g.StartCoroutine(LoadConfig());
        }

        IEnumerator LoadConfig()
        {
            List<ConfigManager.LoadPair> loadPairs = new List<ConfigManager.LoadPair>();
            foreach (int v in System.Enum.GetValues(typeof(EConfigType)))
            {
                string name = System.Enum.GetName(typeof(EConfigType), v);
                System.Type type = System.Type.GetType("Cfg" + name);
                if (type == null)
                    throw new System.Exception($"Not found class {name}");

                ConfigManager.LoadPair loadPair = new ConfigManager.LoadPair(type, "Config/" + name);
                loadPairs.Add(loadPair);
            }

            yield return ConfigManager.Instance.LoadConfigs(loadPairs);

            var test = GetLanguage("UILogin1");
            Debug.Log("start");
            Debug.LogError(test);
        }

        public static string GetLanguage(string key, params object[] args)
        {
            var cfgLanguage = ConfigManager.Instance.GetConfig<CfgLanguage>();
            var table = cfgLanguage.GetTable(key.GetHashCode()) as CfgLanguageTable;
            if (table != null)
            {
                return string.Format(table.Lang, args);
            }
            return key;
        }

        public void Update()
        {
            Debug.Log("update");
        
        }

   
    }
}
