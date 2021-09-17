//--------------------------------------------------
// Motion Framework
// Copyright©2018-2020 何冠峰
// Licensed under the MIT license
//--------------------------------------------------
using System;
using System.Collections;
using System.Collections.Generic;
using EG;
using UnityEngine;

namespace MotionFramework.Config
{
    /// <summary>
    /// 配表管理器
    /// </summary>
    public sealed class ConfigManager : MonoSingleton<ConfigManager>
    {
        public class LoadPair
        {
            public Type ClassType;
            public string Location;
            public LoadPair(Type classType, string location)
            {
                ClassType = classType;
                Location = location;
            }
        }

        private readonly Dictionary<string, AssetConfig> _configs = new Dictionary<string, AssetConfig>();

        public override void Initialize()
        {
            StartCoroutine(LoadConfig());
        }

        public  IEnumerator LoadConfig()
        {
            List<ConfigManager.LoadPair> loadPairs = new List<ConfigManager.LoadPair>();
            foreach (int v in System.Enum.GetValues(typeof(EConfigType)))
            {
                string name = System.Enum.GetName(typeof(EConfigType), v);
                System.Type type = System.Type.GetType("Cfg" + name);
                if (type == null)
                    throw new System.Exception($"Not found class {name}");

                ConfigManager.LoadPair loadPair = new ConfigManager.LoadPair(type,  name+".bytes");
                loadPairs.Add(loadPair);
            }

            yield return LoadConfigs(loadPairs);
        }

        /// <summary>
        /// 按照列表顺序批量加载配表
        /// </summary>
        public IEnumerator LoadConfigs(List<LoadPair> loadPairs)
        {
            float tmp = 0f;
            
            for (int i = 0; i < loadPairs.Count; i++)
            {
                Type type = loadPairs[i].ClassType;
                string location = loadPairs[i].Location;
                AssetConfig config = LoadConfig(type, location);
                
                yield return config;
            }

            m_Initialize = true;
        }

        public AssetConfig LoadConfig(Type configType, string location)
        {
            string configName = configType.FullName;

            // 防止重复加载
            if (_configs.ContainsKey(configName))
            {
                 Debug.LogError($"Config {configName} is already existed.");
                return null;
            }

            AssetConfig config;
            config = Activator.CreateInstance(configType) as AssetConfig;

            if (config == null)
            {
                Debug.LogError($"Config {configName} create instance  failed.");
            }
            else
            {
                config.Load(location);
                _configs.Add(configName, config);
            }

            return config;
        }

        /// <summary>
        /// 获取配表
        /// </summary>
        public T GetConfig<T>() where T : AssetConfig
        {
            return GetConfig(typeof(T)) as T;
        }
        public AssetConfig GetConfig(Type configType)
        {
            string configName = configType.FullName;
            foreach (var pair in _configs)
            {
                if (pair.Key == configName)
                    return pair.Value;
            }

             Debug.LogError($"Not found config {configName}");
            return null;
        }

        /// <summary>
        /// 获取配表
        /// </summary>
        public AssetConfig GetConfig(string configName)
        {
            if (_configs.ContainsKey(configName))
            {
                return _configs[configName];
            }

             Debug.LogError($"Not found config {configName}");
            return null;
        }
    }
}