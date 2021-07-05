using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace EG
{
    public class SceneManager : StateMachine
    {
        static readonly string SCENE_PATH_NAME = "01_Streamed/Active/";
        const int SCENE_STACK_MAX = 16;
        const int ERROR_PROC_NUM = 10000;

        // 
        public class SceneState : State
        {
            public SceneState(int id, StateParam param) : base(id, param)
            {
            }

            public override bool CanChange(State state)
            {
                if (Equals(state.name) == false)
                {
                    // 相同场景不能切换
                    return true;
                }
                return false;
            }
        }

        string[] m_SceneStack;
        int m_SceneStackTop = -1;
        int m_SceneStackLast = SCENE_STACK_MAX;

        bool m_RequestSame;
        string m_RequestScene;
        Scene m_Current;

        string m_SceneName;
        int m_SceneProc;

        AsyncOperationHandle loadHandle;


        public Scene Startup { set {  m_Current = value; if (m_Current != null) m_Current.Activate(); } }

        public string ActiveName
        {
            get
            {
                return m_SceneName;
            }
        }

        public string SceneName
        {
            get
            {
                return UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
            }
        }

        //
        public string SceneNamePrev
        {
            get
            {
                return m_SceneStackTop >= 0 && m_SceneStackTop < m_SceneStack.Length ? m_SceneStack[m_SceneStackTop] : "";
            }
        }

        // 
        public bool IsSceneChange()
        {
            return string.IsNullOrEmpty(m_RequestScene) == false || GetCurrentStateName() != "check" ? true : false;
        }

        static SceneManager m_Instance = null;

        // 
        public static SceneManager Instance
        {
            get
            {
                return m_Instance;
            }
        }

        public static bool HasInstance()
        {
            return (m_Instance != null);
        }

        public static bool isBusy
        {
            get
            {
                Scene scene = Current;
                if (scene != null)
                {
                    bool result = scene.IsSceneChange();
                    result |= m_Instance != null && m_Instance.IsSceneChange();
                    result |= !scene.IsStartup();
                    return result;
                }
                return false;
            }
        }

        public static Scene Current
        {
            get
            {
                if (m_Instance != null)
                {
                    return m_Instance.m_Current;
                }
                return null;
            }
        }

        public static bool IsCurrent<T>() where T : Scene
        {
            if (m_Instance != null)
            {
                if (m_Instance.m_Current != null)
                {
                    return m_Instance.m_Current is T;
                }
            }
            return false;
        }


        // 
        public SceneManager()
        {
            m_Instance = this;
        }

        /// ***********************************************************************

        /// ***********************************************************************
        new public void Initialize()
        {
            base.Initialize();

            m_SceneStack = new string[SCENE_STACK_MAX];
            m_SceneStackTop = -1;
            m_SceneStackLast = SCENE_STACK_MAX;

            m_RequestSame = false;
            m_RequestScene = "";
            m_Current = null;

            // 
            AddState("check", SceneCheck_Begin, SceneCheck_Main, SceneCheck_End);
            AddState("fadein", SceneFadein_Begin, SceneFadein_Main, SceneFadein_End);
            AddState("fadeout", SceneFadeout_Begin, SceneFadeout_Main, SceneFadeout_End);
            AddState("change", SceneChange_Begin, SceneChange_Main, SceneChange_End);

            // 
            Reset("check");

            // 
            StartupScene();
        }

        /// ***********************************************************************

        /// ***********************************************************************
        public void StartupScene()
        {
            // 
            if (m_Current == null)
            {
                GameObject sceneObj = GameObject.Find("scene");
                if (sceneObj != null)
                {
                    UITouchInputModule.ResetInput();
                    this.Startup = sceneObj.GetComponent<Scene>();
                }
            }
        }




        /// ***********************************************************************

        /// ***********************************************************************
        public override void Update()
        {
            // 
            base.Update();
        }

        /// ***********************************************************************

        /// ***********************************************************************
        protected override State CreateState(int id, StateParam param)
        {
            return new SceneState(id, param);
        }

        /// ***********************************************************************

        /// ***********************************************************************
        public void ResetScene(string name, bool sameFlag)
        {
            m_SceneStackTop = -1;
            m_SceneStackLast = SCENE_STACK_MAX;
            m_RequestScene = name;
            m_RequestSame = sameFlag;
        }
        /// ***********************************************************************

        /// ***********************************************************************
        public void ResetScene(string name)
        {
            ResetScene(name, false);
        }

        /// ***********************************************************************

        /// ***********************************************************************
        public void PushScene(string name, bool sameFlag)
        {
            m_RequestScene = name;
            m_RequestSame = sameFlag;
        }
        /// ***********************************************************************

        /// ***********************************************************************
        public void PushScene(string name)
        {
            PushScene(name, false);
        }

        /// ***********************************************************************

        /// ***********************************************************************
        public void PopScene()
        {
            if (m_SceneStackTop != -1 && m_SceneStackTop != m_SceneStackLast)
            {
                m_RequestScene = m_SceneStack[m_SceneStackTop];
                m_RequestSame = true;

                --m_SceneStackTop;
                if (m_SceneStackTop < 0)
                {
                    m_SceneStackTop += SCENE_STACK_MAX;
                }
            }
        }

        /// ***********************************************************************

        /// ***********************************************************************
        public string PeekScene()
        {
            if (m_SceneStackTop != -1 && m_SceneStackTop != m_SceneStackLast)
            {
                return m_SceneStack[m_SceneStackTop];
            }
            return null;
        }


        // 
        void SceneCheck_Begin(StateMachine state)
        {
        }

        // 更新
        void SceneCheck_Main(StateMachine state)
        {
            if (m_RequestScene != "")
            {
                if (m_RequestSame || UnityEngine.SceneManagement.SceneManager.GetActiveScene().name != m_RequestScene)
                {
                    if (RequestState("fadeout"))
                    {
                        // 
                        UITouchInputModule.LockInput();
                        // 
                        m_SceneName = m_RequestScene;
                    }
                }
                m_RequestScene = "";
                m_RequestSame = false;
            }
        }

        // 
        void SceneCheck_End(StateMachine state)
        {
        }



        // 
        void SceneFadein_Begin(StateMachine state)
        {
            //
            //FadeManager.In( sysFadeManager.defaultInTime );
        }

        // 更新
        void SceneFadein_Main(StateMachine state)
        {
            //
            //if( FadeManager.IsIdle() )
            {
                RequestState("check");
            }
        }

        //
        void SceneFadein_End(StateMachine state)
        {
            // 
            UITouchInputModule.UnlockInput(true);
            // 
            ErrorHandler.ResetLock();
        }



        // 
        void SceneFadeout_Begin(StateMachine state)
        {
            // 
            if (Current != null && Current.BgmContinue == false)
            {
                SoundManager.Instance.FadeoutBGM(0.3f);
            }
            // 
            if (LWARS.FadeView.IsOut() == false)
            {
                LWARS.FadeView.Out(ViewManager.Type.FADE_DEFAULT, Color.white, 0.3f);
            }
        }

        // 更新
        void SceneFadeout_Main(StateMachine state)
        {
            // 
            if (LWARS.FadeView.IsIdle())
            {
                RequestState("change");
            }
        }

        // 
        void SceneFadeout_End(StateMachine state)
        {
        }


        // 
        void SceneChange_Begin(StateMachine state)
        {
            m_SceneProc = 0;

            // 
            ErrorHandler.Clear();
            ErrorHandler.Lock();

#if DEBUG_TIMELOG
            m_Time = UnityEngine.Time.realtimeSinceStartup;
#endif
        }

        // 更新
        void SceneChange_Main(StateMachine state)
        {
            string sceneName = m_SceneName;
            int proc = m_SceneProc;

            switch (proc)
            {
                case 0:
                    // 
                    if (SystemManager.Instance.isBusy) break;

                    // 
                    if (SoundManager.Instance.IsFadeoutBGM())
                    {
                        break;
                    }

                    // 
                    while (ThreadManager.Instance.OnSceneChange() == false)
                    {
                        return;
                    }
                    // 
                    //while( Network.instance.OnSceneChange() == false )
                    //{
                    //    return;
                    //}
                    // 
                    while (AssetManager.Instance.OnSceneChange() == false)
                    {
                        return;
                    }

                    // 
                    SoundManager.Instance.StopAllSE();
                    SoundManager.Instance.StopAllVOICE();

                    // 
                    Scene.StopAllExecuter();

                    // GC
                    System.GC.Collect();

                    ++proc;

                    break;

                case 1:

                    // 
                    if (m_Current != null)
                    {
                        ++m_SceneStackTop;
                        if (m_SceneStackTop == m_SceneStackLast)
                        {
                            ++m_SceneStackLast;
                        }

                        if (m_SceneStackTop >= SCENE_STACK_MAX)
                        {
                            m_SceneStackTop -= SCENE_STACK_MAX;
                        }
                        if (m_SceneStackLast > SCENE_STACK_MAX)
                        {
                            m_SceneStackLast -= SCENE_STACK_MAX;
                        }

                        m_SceneStack[m_SceneStackTop] = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
                    }

                    // 
                    if (m_Current != null)
                    {
                        m_Current.Release();
                    }

                    // 
                    Object[] objs = GameObject.FindObjectsOfType(typeof(EG.AppMonoBehaviour));
                    foreach (Object obj in objs)
                    {
                        MonoBehaviour mono = obj as MonoBehaviour;
                        if (mono != null)
                        {
                            mono.StopAllCoroutines();
                        }
                    }


                    m_Current = null;

                    ++proc;

                    break;

                case 2:

                    // 
                    if (SystemManager.Instance.DestroyAll() == false)
                    {
                        break;
                    }

                    // 
                    UIButtonEvent.ResetLock();

                    loadHandle=AssetManager.Instance.LoadSceneAsync(sceneName,null,UnityEngine.SceneManagement.LoadSceneMode.Single,true);


                    ++proc;

                    break;

                case 3:

                    // 
                    while (!loadHandle.IsDone)
                    {
                        return;
                    }

                    ++proc;

                    break;

                case 4:

                    // 
                    if (m_Current == null)
                    {

                        GameObject sceneObj = GameObject.Find("scene");
                        if (sceneObj != null)
                        {
                            m_Current = sceneObj.GetComponent<Scene>();
                            m_Current.Activate();
                            ErrorHandler.ResetLock();
                        }
                    }
                    else
                    {
                        // 
                        if (m_RequestScene != "" && (m_RequestSame || m_RequestScene != sceneName))
                        {
                            RequestState("check");
                            break;
                        }

                        // 
                        if (m_Current.IsStartup())
                        {
                            proc = 10;
                        }
                        else
                        {
                            if (ErrorHandler.IsBusy())
                            {
                                // 
                                UITouchInputModule.UnlockInput(true);
                            }
                        }
                    }

                    break;

                case 10:
                    // 
                    RequestState("fadein");
                    break;
            }

            m_SceneProc = proc;
        }

        // 
        void SceneChange_End(StateMachine state)
        {
            m_SceneProc = 0;

        }

    }
}