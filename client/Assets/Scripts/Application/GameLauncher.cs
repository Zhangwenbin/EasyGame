using System;
using System.Collections;
using System.Collections.Generic;
using EG;
using UnityEngine;
using MotionFramework;
using MotionFramework.Console;
using MotionFramework.Resource;
using MotionFramework.Event;
using MotionFramework.Config;
using MotionFramework.Audio;
using MotionFramework.Patch;
using MotionFramework.Scene;
using MotionFramework.Pool;
using MotionFramework.Window;
using UnityEngine.Networking;

public class GameLauncher : MonoBehaviour
{
    [Tooltip("在编辑器下模拟运行")]
    public bool SimulationOnEditor = true;

    public float TargetNum = 0f;

    public bool Wait = true;

    public bool IsGM = false;

    private float startupTime;

    void Awake()
    {
#if !UNITY_EDITOR
		SimulationOnEditor = false;
#endif

        startupTime = Time.realtimeSinceStartup;

        // 初始化应用
        InitAppliaction();

        // 初始化控制台
        if (Application.isEditor || Debug.isDebugBuild)
            DeveloperConsole.Initialize();

        // 初始化框架
        MotionEngine.Initialize(this, HandleMotionFrameworkLog);
    }
    void Start()
    {
        // 创建游戏模块
        StartCoroutine(CreateGameModules());
    }
    void Update()
    {
        // 更新框架
        MotionEngine.Update();
    }
    void OnGUI()
    {
        if (TargetNum < 1)
            return;

        // 绘制控制台
        if (Application.isEditor || Debug.isDebugBuild)
            DeveloperConsole.Draw();
    }

    /// <summary>
    /// 初始化应用
    /// </summary>
    private void InitAppliaction()
    {
        Application.runInBackground = true;
        Application.backgroundLoadingPriority = ThreadPriority.High;

        // 设置最大帧数
        float mSize = SystemInfo.systemMemorySize / 1024f;
        if (mSize <= 3)
        {
            Application.targetFrameRate = 45;
        }
        else
        {
            Application.targetFrameRate = 60;
        }

        // 屏幕不休眠
        Screen.sleepTimeout = SleepTimeout.NeverSleep;

        //关闭多点触控
        Input.multiTouchEnabled = false;

        string deviceName = SystemInfo.deviceModel;
        if (deviceName.Contains("OnePlus"))
        {
            QualitySettings.vSyncCount = 1;
        }
    }

    /// <summary>
    /// 监听框架日志
    /// </summary>
    private void HandleMotionFrameworkLog(ELogLevel logLevel, string log)
    {
        if (logLevel == ELogLevel.Log)
        {
            UnityEngine.Debug.Log(log);
        }
        else if (logLevel == ELogLevel.Error)
        {
            UnityEngine.Debug.LogError(log);
        }
        else if (logLevel == ELogLevel.Warning)
        {
            UnityEngine.Debug.LogWarning(log);
        }
        else if (logLevel == ELogLevel.Exception)
        {
            UnityEngine.Debug.LogError(log);
        }
        else
        {
            throw new NotImplementedException($"{logLevel}");
        }
    }

    /// <summary>
    /// 创建游戏模块
    /// </summary>
    private IEnumerator CreateGameModules()
    {
        //MotionEngine.CreateModule<SDKManager>(IsGM);

        // 创建事件管理器
        MotionEngine.CreateModule<EventManager>();
        

        TargetNum = 0.1f;

        // 加载配置文件
        MotionEngine.CreateModule<ConfigManager>();
        List<ConfigManager.LoadPair> loadPairs = new List<ConfigManager.LoadPair>();
        // foreach (int v in System.Enum.GetValues(typeof(EConfigType)))
        // {
        //     string name = System.Enum.GetName(typeof(EConfigType), v);
        //     System.Type type = System.Type.GetType("Cfg" + name);
        //     if (type == null)
        //         throw new System.Exception($"Not found class {name}");
        //
        //     ConfigManager.LoadPair loadPair = new ConfigManager.LoadPair(type, "Config/" + name);
        //     loadPairs.Add(loadPair);
        // }

        float targetProcess = 0.5f;
        yield return ConfigManager.Instance.LoadConfigs(loadPairs, this, targetProcess);
        TargetNum = targetProcess;
        

        // 创建音频管理器
        MotionEngine.CreateModule<AudioManager>();

        // 创建场景管理器
        MotionEngine.CreateModule<SceneManager>();

        // 创建对象池管理器
        MotionEngine.CreateModule<GameObjectPoolManager>();

        // 最后创建游戏业务逻辑模块
        MotionEngine.CreateModule<WindowManager>();
        //MotionEngine.CreateModule<FsmManager>();

        //MotionEngine.CreateModule<GuideManager>();
        // NotificationManager.CancelAllNotifications();
        // NotificationManager.Update(NotificationManager.EPushWay.DATE);
        
        TargetNum = 1;

        while (Wait)
            yield return 0;

        // 走到这里肯定同意了隐私协议
       // SDKManager.Instance.UserAgreed();

        // 开始游戏逻辑
        //FsmManager.Instance.StartGame();
    }
}