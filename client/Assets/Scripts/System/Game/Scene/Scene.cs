using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace EG
{
    public class Scene : AppMonoBehaviour
    {

        protected SceneManager m_SceneManager = null;
        protected ViewController m_ViewController = null;

        bool m_Activate = false;
        bool m_Startup = false;
        protected StateMachine m_State = new StateMachine();
        protected string m_StateNext = "";
        protected ViewBase m_RootView = null;
        protected CachePool m_CachePool = null;

        // ����I��
        private bool m_BGMContinue = false;                    // ���`��ޤ�����BGM��ֹͣ���뤫�ɤ���

        #endregion

        // �ץ�ѥƥ�
        #region �ץ�ѥƥ�

        new public string name { get { return ""; } }

        public ViewController ViewController { get { return m_ViewController; } }

        public StateMachine.State CurrentState { get { return m_State.GetCurrentState(); } }

        public CachePool CachePool { get { return m_CachePool; } }

        public bool BgmContinue
        {
            get { return m_BGMContinue; }
            set { m_BGMContinue = value; }
        }

        #endregion

        /// ***********************************************************************

        /// ***********************************************************************
        public static Scene Current()
        {
            return SceneManager.Current;
        }

        /// ***********************************************************************
        /// <summary>
        /// �����ȥ��`���ȡ��
        /// </summary>
        /// ***********************************************************************
        public static T Current<T>() where T : Scene
        {
            return SceneManager.Current as T;
        }

        //=========================================================================
        //. ����`����
        //=========================================================================
        #region ����`����

        /// ***********************************************************************
        /// <summary>
        /// ����`�����_ʼ
        /// </summary>
        /// ***********************************************************************
        public static Coroutine StartExecuter(IEnumerator routine)
        {
            Scene scene = Current();
            if (scene != null)
            {
                return scene.StartCoroutine(routine);
            }
            return null;
        }

        /// ***********************************************************************
        /// <summary>
        /// ����`�����_ʼ
        /// </summary>
        /// ***********************************************************************
        public static Coroutine StartExecuter(IEnumerator routine, System.Action callback)
        {
            Scene scene = Current();
            if (scene != null)
            {
                return scene.StartCoroutine(WorkCoroutine(routine, callback));
            }
            return null;
        }
        // ��`���`����`����( �K�˕r�˥��`��Хå��v����g�Ф��� )
        private static IEnumerator WorkCoroutine(IEnumerator target, System.Action callback)
        {
            yield return target;
            callback();
        }

        /// ***********************************************************************
        /// <summary>
        /// ����`����ֹͣ
        /// </summary>
        /// ***********************************************************************
        public static void StopExecuter(Coroutine coroutine)
        {
            Scene scene = Current();
            if (scene != null)
            {
                scene.StopCoroutine(coroutine);
            }
        }

        /// ***********************************************************************
        /// <summary>
        /// ����`����ȫֹͣ
        /// </summary>
        /// ***********************************************************************
        public static void StopAllExecuter()
        {
            Scene scene = Current();
            if (scene != null)
            {
                scene.StopAllCoroutines();
            }
        }

        /// ***********************************************************************
        /// <summary>
        /// �ǥ��쥤����������`���`���_ʼ
        /// </summary>
        /// ***********************************************************************
        public static Coroutine StartDelayExecuter(float delay, System.Action<object> action, object value = null)
        {
            Scene scene = Current();
            if (scene != null)
            {
                return scene.StartCoroutine(DelayExecuter(delay, action, value));
            }
            return null;
        }
        public static IEnumerator DelayExecuter(float delay, System.Action<object> action, object value)
        {
            while (delay > 0)
            {
                delay -= TimerManager.DeltaTime;
                yield return null;
            }
            if (action != null) action.Invoke(value);
        }

        #endregion

        //=========================================================================
        //. ����å���
        //=========================================================================
        #region ����å���

        /// ***********************************************************************
        /// <summary>
        /// ����å����׷��
        /// </summary>
        /// ***********************************************************************
        public static void AddCache(GameObject gobj)
        {
            Scene scene = Current();
            if (scene != null)
            {
                CachePool cachePool = scene.CachePool;
                if (cachePool != null)
                {
                    cachePool.AddCache(gobj);
                }
            }
        }

        /// ***********************************************************************
        /// <summary>
        /// ָ�����`�Υ���å����ȫ�Ɨ�
        /// </summary>
        /// <param name="isVague">�����ޤ��������S�ɤ��뤫</param>
        /// ***********************************************************************
        public static void RemoveCache(string key, bool isVague = false)
        {
            Scene scene = Current();
            if (scene != null)
            {
                CachePool cachePool = scene.CachePool;
                if (cachePool != null)
                {
                    cachePool.Remove(key, isVague);
                }
            }
        }

        /// ***********************************************************************
        /// <summary>
        /// ����å����ȫ�Ɨ�
        /// </summary>
        /// ***********************************************************************
        public static void RemoveCacheAll()
        {
            Scene scene = Current();
            if (scene != null)
            {
                CachePool cachePool = scene.CachePool;
                if (cachePool != null)
                {
                    cachePool.RemoveAll();
                }
            }
        }

        /// ***********************************************************************
        /// <summary>
        /// ָ�����`�Υ���å����S�ɤ���
        /// </summary>
        /// <param name="isVague">�����ޤ��������S�ɤ��뤫</param>
        /// ***********************************************************************
        public static void EnableCache(string key, bool isVague = false)
        {
            Scene scene = Current();
            if (scene != null)
            {
                CachePool cachePool = scene.CachePool;
                if (cachePool != null)
                {
                    cachePool.Enable(key, isVague);
                }
            }
        }

        /// ***********************************************************************
        /// <summary>
        /// ȫ�ƤΥ���å����S�ɤ���
        /// </summary>
        /// ***********************************************************************
        public static void EnableCacheAll()
        {
            Scene scene = Current();
            if (scene != null)
            {
                CachePool cachePool = scene.CachePool;
                if (cachePool != null)
                {
                    cachePool.EnableAll();
                }
            }
        }

        /// ***********************************************************************
        /// <summary>
        /// ָ�����`�Υ���å����S�ɤ��ʤ�
        /// </summary>
        /// <param name="isVague">�����ޤ��������S�ɤ��뤫</param>
        /// ***********************************************************************
        public static void DisableCache(string key, bool isVague = false)
        {
            Scene scene = Current();
            if (scene != null)
            {
                CachePool cachePool = scene.CachePool;
                if (cachePool != null)
                {
                    cachePool.Disable(key, isVague);
                }
            }
        }

        /// ***********************************************************************
        /// <summary>
        /// ȫ�ƤΥ���å����S�ɤ��ʤ�
        /// </summary>
        /// ***********************************************************************
        public static void DisableCacheAll()
        {
            Scene scene = Current();
            if (scene != null)
            {
                CachePool cachePool = scene.CachePool;
                if (cachePool != null)
                {
                    cachePool.DisableAll();
                }
            }
        }

        #endregion

        #endregion

        //=========================================================================
        //. ���ڻ�
        //=========================================================================
        #region ���ڻ�

        // �I���
        //  Awake()
        //  IEnumerator Start()
        //  {
        //      Preparation();
        //      
        //      while( m_Activate == false ) yield return null;
        //      
        //      while( OnStart() == false ) yield return null;
        //      
        //      Load();
        //      LoadPost();
        //
        //      Initialize();
        //  }

        /// ***********************************************************************
        /// <summary>
        /// ���ӕr�˺��Ф��
        /// </summary>
        /// ***********************************************************************
        protected virtual void Awake()
        {
            // �������ꥢ
            m_Startup = false;

            // ���`��ޤ�����BGM��ֹͣ����
            m_BGMContinue = false;

            // �꥽�`����ȫ���Ɨ�
            Resources.UnloadUnusedAssets();
            // ���٩`�����쥯��
            System.GC.Collect();

            // �ӥ�`����ȥ�`���ȡ��
            m_ViewController = gameObject.FindChildComponentAll<ViewController>("view");
        }

        /// ***********************************************************************
        /// <summary>
        /// �����ƥ��ʂ�
        /// </summary>
        /// ***********************************************************************
        protected virtual IEnumerator Preparation()
        {
            // �����ƥ�ޥͩ`����γ��ڻ�
            while (SystemManager.Instance.isInitialized == false)
            {
                SystemManager.Instance.Initialize();
                yield return null;
            }

            // ���`��ޥͩ`����γ��ڻ�����
            while (GameManager.Instance.isInitialized == false)
            {
                GameManager.Instance.Initialize();
                yield return null;
            }

            // ���`��ޥͩ`����ȡ��
            m_SceneManager = GameManager.Instance.SceneManager;
        }

        /// ***********************************************************************
        /// <summary>
        /// ��`��
        /// </summary>
        /// ***********************************************************************
        protected virtual IEnumerator Load()
        {
            yield break;
        }
        protected virtual IEnumerator LoadPost()
        {
            yield break;
        }

        /// ***********************************************************************
        /// <summary>
        /// ����ǰ���ڻ�
        /// </summary>
        /// ***********************************************************************
        protected virtual IEnumerator Start()
        {
            // �����ƥ��ʂ�
            yield return StartCoroutine(Preparation());

            // �����ƥ��٩`�ȴ���
            while (m_Activate == false)
            {
                yield return null;
            }

            // ���`��Ȥδ���
            while (OnStart() == false)
            {
                yield return null;
            }

            // ��`��
            yield return StartCoroutine(Load());
            yield return StartCoroutine(LoadPost());

            // ���ڻ�
            Initialize();
        }

        /// ***********************************************************************
        /// <summary>
        /// ���`����_ʼ�v��
        /// </summary>
        /// ***********************************************************************
        protected virtual bool OnStart()
        {
            // ����å���ש`���ȡ��/����
            m_CachePool = GetComponentInChildren<CachePool>();
            if (m_CachePool == null)
            {
                GameObject cachePool = new GameObject("cache_pool");
                cachePool.transform.SetParent(gameObject.transform, false);
                m_CachePool = cachePool.AddComponent<CachePool>();
            }

            // ���ڻ�
            m_CachePool.Initialize();

            return true;
        }

        /// ***********************************************************************
        /// <summary>
        /// ���ڻ�
        /// </summary>
        /// ***********************************************************************
        public override void Initialize()
        {
            if (isInitialized) return;

            // ���פγ��ڻ�
            base.Initialize();

            // ------------------------------------------------

            // �ӥ�`����ȥ�`��`���ڻ�
            if (m_ViewController != null)
            {
                m_ViewController.Initialize();
            }

            // ���Ʃ`���O��
            m_State.Initialize();
            m_State.SetChangeCallback(StateChange);
            m_State.AddState("idle", StateIdle_Begin, StateIdle, StateIdle_End, StateLateIdle);
            m_State.AddState("setup", StateSetup_Begin, StateSetup, StateSetup_End, null);

            // ���ڥ��Ʃ`��
            m_State.Reset("setup");
        }

        /// ***********************************************************************
        /// <summary>
        /// �Ɨ�
        /// </summary>
        /// ***********************************************************************
        public override void Release()
        {
            // �ӥ�`����ȥ�`��`�Ɨ�
            if (m_ViewController != null)
            {
                m_ViewController.Release();
            }
            m_ViewController = null;

            // �ӥ�`�������Ƥ���ƥ����Ȥ�ȫ�Ɨ�
            ViewManager.UnloadTextAll();

            // ����å����Ɨ�
            if (m_CachePool != null)
            {
                m_CachePool.Release();
                m_CachePool = null;
            }

            // ------------------------------------------------

            // ���פ��Ɨ�
            base.Release();
        }

        #endregion

        //=========================================================================
        //. ����
        //=========================================================================
        #region ����

        /// ***********************************************************************
        /// <summary>
        /// ���ե�`�ॳ�`�뤵���
        /// </summary>
        /// ***********************************************************************
        protected virtual void Update()
        {
            if (isInitialized == false)
            {
                return;
            }

            // ���ä��������������ޤ��������Σ�ʤ����ʤΤǥ�����
            // if( ErrorHandler.IsBusy( ) ) return;

            // �����`�ȥ��åפ��Ƥ�����Ϥϥ��`���Ф��椨���k������ȄI�����¤��ʤ�
            if (IsStartup())
            {
                if (SceneManager.isBusy) return;
            }

#if UNITY_EDITOR
            if (FlowNode.EditorPauser != null)
            {
                FlowNode.EditorPauser.Activate();
            }
#endif

            // ���Ʃ`�ȥޥ������
            m_State.Update();

            // �ӥ�`����ȥ�`��`����
            if (m_ViewController != null)
            {
                m_ViewController.Process();
            }
        }

        /// ***********************************************************************
        /// <summary>
        /// ���ե�`�ॳ�`�뤵���(��I��)
        /// </summary>
        /// ***********************************************************************
        protected virtual void LateUpdate()
        {
            if (isInitialized == false)
            {
                return;
            }

            // ���ä��������������ޤ��������Σ�ʤ����ʤΤǥ�����
            // if( ErrorHandler.IsBusy( ) ) return;

            // �����`�ȥ��åפ��Ƥ�����Ϥϥ��`���Ф��椨���k������ȄI�����¤��ʤ�
            if (IsStartup())
            {
                if (SceneManager.isBusy) return;
            }

            // ���Ʃ`�ȥޥ������
            m_State.LateUpdate();
        }

        #endregion

        //=========================================================================
        //. �ӥ�`
        //=========================================================================

        //=========================================================================
        //. �O��/ȡ��
        //=========================================================================
        #region �O��/ȡ��

        /// ***********************************************************************
        /// <summary>
        /// ���`�󥢥��ƥ��٩`��
        /// </summary>
        /// ***********************************************************************
        public void Activate()
        {
            m_Activate = true;
        }

        /// ***********************************************************************
        /// <summary>
        /// �����`�ȥ��å�
        /// </summary>
        /// ***********************************************************************
        protected void Startup()
        {
            m_Startup = true;
        }

        /// ***********************************************************************
        /// <summary>
        /// ���Ʃ`�ȥꥻ�å�
        /// </summary>
        /// ***********************************************************************
        public void ResetState(string stateName, string nextState)
        {
            m_StateNext = nextState;
            m_State.Reset(stateName);
        }

        /// ***********************************************************************
        /// <summary>
        /// ���Ʃ`�ȥꥻ�å�
        /// </summary>
        /// ***********************************************************************
        public void ResetState(string stateName)
        {
            m_StateNext = "";
            m_State.Reset(stateName);
        }

        /// ***********************************************************************
        /// <summary>
        /// ���Ʃ`�ȥꥯ������
        /// </summary>
        /// ***********************************************************************
        public void RequestState(string stateName, string nextState)
        {
            m_StateNext = nextState;
            m_State.RequestState(stateName);
        }

        /// ***********************************************************************
        /// <summary>
        /// ���Ʃ`�ȥꥯ������
        /// </summary>
        /// ***********************************************************************
        public void RequestState(string stateName)
        {
            m_StateNext = "";
            m_State.RequestState(stateName);
        }

        #endregion

        //=========================================================================
        //. �_�J
        //=========================================================================
        #region �_�J

        /// ***********************************************************************
        /// <summary>
        /// �����`�ȥ��åץ����å�
        /// </summary>
        /// ***********************************************************************
        public bool IsStartup()
        {
            return m_Startup;
        }

        /// ***********************************************************************
        /// <summary>
        /// �����ɥ륹�Ʃ`�ȥ����å�
        /// </summary>
        /// ***********************************************************************
        public bool IsIdle()
        {
            if (m_State.EqualState("idle"))
            {
                return true;
            }
            return false;
        }

        /// ***********************************************************************
        /// <summary>
        /// ���åȥ��åץ��Ʃ`�ȥ����å�
        /// </summary>
        /// ***********************************************************************
        public bool IsSetup()
        {
            if (m_State.EqualState("setup"))
            {
                return true;
            }
            return false;
        }

        /// ***********************************************************************
        /// <summary>
        /// ���`�����Ф������å�
        /// </summary>
        /// ***********************************************************************
        public bool IsSceneChange()
        {
            return false;
        }

        #endregion

        //=========================================================================
        //. ���٥��
        //=========================================================================
        #region ���٥��

        #endregion

        //=========================================================================
        //. ���Ʃ`��
        //=========================================================================
        #region ���Ʃ`��

        /// ***********************************************************************
        /// <summary>
        /// ���Ʃ`�����Ф��椨�r�˺��Ф��
        /// </summary>
        /// ***********************************************************************
        protected virtual void StateChange(StateMachine state)
        {
        }

        #endregion

        //=========================================================================
        //. �����ɥ�
        //=========================================================================

        #region �����ɥ�

        // �_ʼ
        protected virtual void StateIdle_Begin(StateMachine state)
        {
        }

        // �ᥤ��
        protected virtual void StateIdle(StateMachine state)
        {
        }
        // �ᥤ��(��)
        protected virtual void StateLateIdle(StateMachine state)
        {
        }

        // �K��
        protected virtual void StateIdle_End(StateMachine state)
        {
        }

        #endregion

        //=========================================================================
        //. ���åȥ��å�
        //=========================================================================

        #region ���åȥ��å�

        // �_ʼ
        protected virtual void StateSetup_Begin(StateMachine state)
        {
        }

        // �ᥤ��
        protected virtual void StateSetup(StateMachine state)
        {
            if (m_ViewController == null)
            {
                RequestState("idle");
                return;
            }

            // �ӥ�`�΄I������
            if (m_ViewController.IsBusy() == false)
            {
                RequestState("idle");
            }
        }

        // �K��
        protected virtual void StateSetup_End(StateMachine state)
        {
            // �_ʼ
            Startup();
        }

        #endregion
    }
}