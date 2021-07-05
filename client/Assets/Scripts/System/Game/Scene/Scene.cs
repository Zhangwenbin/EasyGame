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

        // 例外処理
        private bool m_BGMContinue = false;                    // シーンまたぎでBGMを停止するかどうか

        #endregion

        // プロパティ
        #region プロパティ

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
        /// カレントシーンの取得
        /// </summary>
        /// ***********************************************************************
        public static T Current<T>() where T : Scene
        {
            return SceneManager.Current as T;
        }

        //=========================================================================
        //. コルーチン
        //=========================================================================
        #region コルーチン

        /// ***********************************************************************
        /// <summary>
        /// コルーチン開始
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
        /// コルーチン開始
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
        // ワーカーコルーチン( 終了時にコールバック関数を実行する )
        private static IEnumerator WorkCoroutine(IEnumerator target, System.Action callback)
        {
            yield return target;
            callback();
        }

        /// ***********************************************************************
        /// <summary>
        /// コルーチン停止
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
        /// コルーチン全停止
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
        /// ディレイエグゼキューターの開始
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
        //. キャッシュ
        //=========================================================================
        #region キャッシュ

        /// ***********************************************************************
        /// <summary>
        /// キャッシュの追加
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
        /// 指定キーのキャッシュを全破棄
        /// </summary>
        /// <param name="isVague">あいまい検索を許可するか</param>
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
        /// キャッシュを全破棄
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
        /// 指定キーのキャッシュ許可する
        /// </summary>
        /// <param name="isVague">あいまい検索を許可するか</param>
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
        /// 全てのキャッシュ許可する
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
        /// 指定キーのキャッシュ許可しない
        /// </summary>
        /// <param name="isVague">あいまい検索を許可するか</param>
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
        /// 全てのキャッシュ許可しない
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
        //. 初期化
        //=========================================================================
        #region 初期化

        // 処理順
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
        /// 起動時に呼ばれる
        /// </summary>
        /// ***********************************************************************
        protected virtual void Awake()
        {
            // 変数クリア
            m_Startup = false;

            // シーンまたぎでBGMを停止する
            m_BGMContinue = false;

            // リソースを全て破棄
            Resources.UnloadUnusedAssets();
            // ガベージコレクト
            System.GC.Collect();

            // ビューコントローラの取得
            m_ViewController = gameObject.FindChildComponentAll<ViewController>("view");
        }

        /// ***********************************************************************
        /// <summary>
        /// システム準備
        /// </summary>
        /// ***********************************************************************
        protected virtual IEnumerator Preparation()
        {
            // システムマネージャの初期化
            while (SystemManager.Instance.isInitialized == false)
            {
                SystemManager.Instance.Initialize();
                yield return null;
            }

            // ゲームマネージャの初期化待ち
            while (GameManager.Instance.isInitialized == false)
            {
                GameManager.Instance.Initialize();
                yield return null;
            }

            // シーンマネージャ取得
            m_SceneManager = GameManager.Instance.SceneManager;
        }

        /// ***********************************************************************
        /// <summary>
        /// ロード
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
        /// 更新前初期化
        /// </summary>
        /// ***********************************************************************
        protected virtual IEnumerator Start()
        {
            // システム準備
            yield return StartCoroutine(Preparation());

            // アクティベート待ち
            while (m_Activate == false)
            {
                yield return null;
            }

            // シーン側の待ち
            while (OnStart() == false)
            {
                yield return null;
            }

            // ロード
            yield return StartCoroutine(Load());
            yield return StartCoroutine(LoadPost());

            // 初期化
            Initialize();
        }

        /// ***********************************************************************
        /// <summary>
        /// シーンの開始関数
        /// </summary>
        /// ***********************************************************************
        protected virtual bool OnStart()
        {
            // キャッシュプールの取得/生成
            m_CachePool = GetComponentInChildren<CachePool>();
            if (m_CachePool == null)
            {
                GameObject cachePool = new GameObject("cache_pool");
                cachePool.transform.SetParent(gameObject.transform, false);
                m_CachePool = cachePool.AddComponent<CachePool>();
            }

            // 初期化
            m_CachePool.Initialize();

            return true;
        }

        /// ***********************************************************************
        /// <summary>
        /// 初期化
        /// </summary>
        /// ***********************************************************************
        public override void Initialize()
        {
            if (isInitialized) return;

            // 基底の初期化
            base.Initialize();

            // ------------------------------------------------

            // ビューコントローラー初期化
            if (m_ViewController != null)
            {
                m_ViewController.Initialize();
            }

            // ステート設定
            m_State.Initialize();
            m_State.SetChangeCallback(StateChange);
            m_State.AddState("idle", StateIdle_Begin, StateIdle, StateIdle_End, StateLateIdle);
            m_State.AddState("setup", StateSetup_Begin, StateSetup, StateSetup_End, null);

            // 初期ステート
            m_State.Reset("setup");
        }

        /// ***********************************************************************
        /// <summary>
        /// 破棄
        /// </summary>
        /// ***********************************************************************
        public override void Release()
        {
            // ビューコントローラー破棄
            if (m_ViewController != null)
            {
                m_ViewController.Release();
            }
            m_ViewController = null;

            // ビューが管理しているテキストを全破棄
            ViewManager.UnloadTextAll();

            // キャッシュ破棄
            if (m_CachePool != null)
            {
                m_CachePool.Release();
                m_CachePool = null;
            }

            // ------------------------------------------------

            // 基底の破棄
            base.Release();
        }

        #endregion

        //=========================================================================
        //. 更新
        //=========================================================================
        #region 更新

        /// ***********************************************************************
        /// <summary>
        /// 毎フレームコールされる
        /// </summary>
        /// ***********************************************************************
        protected virtual void Update()
        {
            if (isInitialized == false)
            {
                return;
            }

            // あった方が良いがいまさら入れると危なそうなのでコメント
            // if( ErrorHandler.IsBusy( ) ) return;

            // スタートアップしている場合はシーン切り替えが発生すると処理を更新しない
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

            // ステートマシン更新
            m_State.Update();

            // ビューコントローラー更新
            if (m_ViewController != null)
            {
                m_ViewController.Process();
            }
        }

        /// ***********************************************************************
        /// <summary>
        /// 毎フレームコールされる(後処理)
        /// </summary>
        /// ***********************************************************************
        protected virtual void LateUpdate()
        {
            if (isInitialized == false)
            {
                return;
            }

            // あった方が良いがいまさら入れると危なそうなのでコメント
            // if( ErrorHandler.IsBusy( ) ) return;

            // スタートアップしている場合はシーン切り替えが発生すると処理を更新しない
            if (IsStartup())
            {
                if (SceneManager.isBusy) return;
            }

            // ステートマシン更新
            m_State.LateUpdate();
        }

        #endregion

        //=========================================================================
        //. ビュー
        //=========================================================================

        //=========================================================================
        //. 設定/取得
        //=========================================================================
        #region 設定/取得

        /// ***********************************************************************
        /// <summary>
        /// シーンアクティベート
        /// </summary>
        /// ***********************************************************************
        public void Activate()
        {
            m_Activate = true;
        }

        /// ***********************************************************************
        /// <summary>
        /// スタートアップ
        /// </summary>
        /// ***********************************************************************
        protected void Startup()
        {
            m_Startup = true;
        }

        /// ***********************************************************************
        /// <summary>
        /// ステートリセット
        /// </summary>
        /// ***********************************************************************
        public void ResetState(string stateName, string nextState)
        {
            m_StateNext = nextState;
            m_State.Reset(stateName);
        }

        /// ***********************************************************************
        /// <summary>
        /// ステートリセット
        /// </summary>
        /// ***********************************************************************
        public void ResetState(string stateName)
        {
            m_StateNext = "";
            m_State.Reset(stateName);
        }

        /// ***********************************************************************
        /// <summary>
        /// ステートリクエスト
        /// </summary>
        /// ***********************************************************************
        public void RequestState(string stateName, string nextState)
        {
            m_StateNext = nextState;
            m_State.RequestState(stateName);
        }

        /// ***********************************************************************
        /// <summary>
        /// ステートリクエスト
        /// </summary>
        /// ***********************************************************************
        public void RequestState(string stateName)
        {
            m_StateNext = "";
            m_State.RequestState(stateName);
        }

        #endregion

        //=========================================================================
        //. 確認
        //=========================================================================
        #region 確認

        /// ***********************************************************************
        /// <summary>
        /// スタートアップチェック
        /// </summary>
        /// ***********************************************************************
        public bool IsStartup()
        {
            return m_Startup;
        }

        /// ***********************************************************************
        /// <summary>
        /// アイドルステートチェック
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
        /// セットアップステートチェック
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
        /// シーン変更中かチェック
        /// </summary>
        /// ***********************************************************************
        public bool IsSceneChange()
        {
            return false;
        }

        #endregion

        //=========================================================================
        //. イベント
        //=========================================================================
        #region イベント

        #endregion

        //=========================================================================
        //. ステート
        //=========================================================================
        #region ステート

        /// ***********************************************************************
        /// <summary>
        /// ステータス切り替え時に呼ばれる
        /// </summary>
        /// ***********************************************************************
        protected virtual void StateChange(StateMachine state)
        {
        }

        #endregion

        //=========================================================================
        //. アイドル
        //=========================================================================

        #region アイドル

        // 開始
        protected virtual void StateIdle_Begin(StateMachine state)
        {
        }

        // メイン
        protected virtual void StateIdle(StateMachine state)
        {
        }
        // メイン(後)
        protected virtual void StateLateIdle(StateMachine state)
        {
        }

        // 終了
        protected virtual void StateIdle_End(StateMachine state)
        {
        }

        #endregion

        //=========================================================================
        //. セットアップ
        //=========================================================================

        #region セットアップ

        // 開始
        protected virtual void StateSetup_Begin(StateMachine state)
        {
        }

        // メイン
        protected virtual void StateSetup(StateMachine state)
        {
            if (m_ViewController == null)
            {
                RequestState("idle");
                return;
            }

            // ビューの処理を待つ
            if (m_ViewController.IsBusy() == false)
            {
                RequestState("idle");
            }
        }

        // 終了
        protected virtual void StateSetup_End(StateMachine state)
        {
            // 開始
            Startup();
        }

        #endregion
    }
}