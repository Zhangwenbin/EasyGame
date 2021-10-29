using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System;
using System.Collections;
using UnityEngine.AddressableAssets;
using UnityEngine.AddressableAssets.ResourceLocators;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;

namespace EG
{
    #region 定义

    public enum ReleaseType
    {
        OnNewScene,
        Manual,
        NoneRelease
    }

    public enum LoadPriority
    {
        Default = 0, //默认加载 异步 队列
        Prior = 10, // 异步  不队列
    }

    public enum AssetStatus
    {
        Init,
        CheckUpdate,
        ConfirmUpdate,
        Update,
        Ready,
        Preload
    }

    public class LoadRequest : IEnumerator
    {
        public enum AssetFrom
        {
            None,
            GameobjectPool,
            RefPool,
            LoadGameobjectNotInstantiate,
            LoadGameobjectInstantiate,
            LoadRefAsset
        }

        public string key;
        private bool startLoad;
        public bool isSprite;
        public bool useCache;

        public AssetFrom aasetFrom;

        public AssetFrom AasetFrom
        {
            get { return aasetFrom; }
        }

        /// <summary>
        /// for load
        /// </summary>
        public AsyncOperationHandle handle;

        public void Load()
        {
            aasetFrom = AssetFrom.None;

            var item = GameObjectPool.GetGameObject(key);
            if (item != null)
            {
                item.SetActive(true);
                this.useCache = true;
                this.result = item;
                aasetFrom = AssetFrom.GameobjectPool;
                return;
            }

            var refItem = RefPool.GetObject(key);
            if (refItem != null)
            {
                this.useCache = true;
                this.result = refItem;
                aasetFrom = AssetFrom.RefPool;
                return;
            }

            useCache = false;
            if (isSprite)
            {
                handle = Addressables.LoadAssetAsync<Sprite>(key);
            }
            else
            {
                handle = Addressables.LoadAssetAsync<Object>(key);
            }

            startLoad = true;
        }

        /// <summary>
        /// True if the operation is complete.
        /// </summary>
        public bool IsDone
        {
            get { return startLoad && (useCache || handle.IsDone); }
        }


        /// <summary>
        /// Check if the internal operation is not null and has the same version of this handle.
        /// </summary>
        /// <returns>True if valid.</returns>
        public bool IsValid
        {
            get { return startLoad? useCache || handle.IsValid():true; }
        }


        /// <summary>
        /// The progress of the internal operation.
        /// This is evenly weighted between all sub-operations. For example, a LoadAssetAsync call could potentially
        /// be chained with InitializeAsync and have multiple dependent operations that download and load content.
        /// In that scenario, PercentComplete would reflect how far the overal operation was, and would not accurately
        /// represent just percent downloaded or percent loaded into memory.
        /// For accurate download percentages, use GetDownloadStatus(). 
        /// </summary>
        public float PercentComplete
        {
            get { return startLoad? useCache ? 1 : handle.PercentComplete:0; }
        }

        private Object result;

        /// <summary>
        /// The result object of the operations.
        /// </summary>
        public Object Result
        {
            get
            {
                if (IsDone)
                {
                    if (Status == AsyncOperationStatus.Succeeded)
                    {
                        if (result == null)
                        {
                            GameObject go = handle.Result as GameObject;
                            if (go)
                            {
                                // if (EngineDelegate.PrefabUnInstantiateRule != null &&
                                //     EngineDelegate.PrefabUnInstantiateRule(go))
                                // {
                                //     result = go;
                                //     aasetFrom =LoadRequest.AssetFrom.LoadGameobjectNotInstantiate;
                                // }
                                // else
                                {
                                    var instance = UnityEngine.GameObject.Instantiate(go);
                                    result = instance;
                                    AssetManager.Instance.handleDic[handle] = instance;
                                    aasetFrom = LoadRequest.AssetFrom.LoadGameobjectInstantiate;
                                }
                            }
                            else
                            {
                                aasetFrom = LoadRequest.AssetFrom.LoadRefAsset;
                                RefPool.OnLoadObject(key, handle);
                                result = handle.Result as Object;
                            }
                        }
                    }
                    else
                    {
                        Debug.LogError("加载资源失败 :" + key);
                    }
                }

                return result;
            }
        }


        /// <summary>
        /// The status of the internal operation.
        /// </summary>
        public AsyncOperationStatus Status
        {
            get { return useCache ? AsyncOperationStatus.Succeeded : handle.Status; }
        }


        object IEnumerator.Current
        {
            get { return Result; }
        }

        /// <summary>
        /// Overload for <see cref="IEnumerator.MoveNext"/>.
        /// </summary>
        /// <returns>Returns true if the enumerator can advance to the next element in the collectin. Returns false otherwise.</returns>
        bool IEnumerator.MoveNext()
        {
            return !IsDone;
        }

        /// <summary>
        /// Overload for <see cref="IEnumerator.Reset"/>.
        /// </summary>
        void IEnumerator.Reset()
        {
        }

        public Action<string, UnityEngine.Object> callback;

        public void CallBack()
        {
            if (callback != null)
            {
                //委托的执行顺序问题
                var list = callback.GetInvocationList();
                for (int i = 0; i < list.Length; i++)
                {
                    Debug.Log(list[i].Method.Name);
                    list[i].DynamicInvoke(key, Result);
                }
            }

        }

        public LoadRequest(string key)
        {
            this.key = key;
            startLoad = false;
        }

        public void Release()
        {
            AssetManager.Instance.ReleaseAsset(key, result);
        }
    }

    #endregion

    public class AssetManager : MonoSingleton<AssetManager>
    {
        #region Public Interface

        public override void Initialize()
        {
            StartCoroutine(InitializeAsync());
        }

        public void PreloadToPool(string name)
        {
           GetAsset(name, (key, obj) =>
           {
               ReleaseAsset(key,obj,true);
           });
        }

        /// <summary>
        /// 从assetbundle中加载asset
        /// </summary>
        /// <param name="name"></param>
        /// <param name="callback"></param>
        /// <param name="priority"></param>
        public virtual void GetAsset(string name, Action<string, UnityEngine.Object> callback,
            LoadPriority priority = LoadPriority.Default, ReleaseType type = ReleaseType.Manual)
        {
            // resourceMgr.AssetBundleGroup.GetAsset(name, callback, priority, type);
            if (string.IsNullOrEmpty(name))
            {
                callback(name, null);
                return;
            }

            if (priority == LoadPriority.Default)
            {
                LoadAssetAsyncQueue(name).callback += callback;
            }
            else
            {
                LoadAssetAsync(name).callback += callback;
            }
        }

        public void LoadSpriteAsync(string name, Action<string, UnityEngine.Sprite> callback)
        {
            LoadAssetAsync(name, true).callback = (key, res) => { callback(key, res as Sprite); };
        }


        /// <summary>
        /// 注册不需要加载的资源
        /// </summary>
        /// <param name="key"></param>
        /// <param name="obj"></param>
        public virtual void RegisterAsset(string key, GameObject obj)
        {
            if (obj != null)
            {
                GameObjectPool.RecycleGameObject(key, obj);
            }
        }

        //load scene
        public virtual AsyncOperationHandle GetScene(string name, Action callBack = null)
        {
            return LoadSceneAsync(name + ".unity", (res) =>
            {
                if (callBack != null)
                {
                    callBack();
                }
            });
        }


        /// <summary>
        /// 加载二进制文件
        /// </summary>
        /// <param name="name"></param>
        /// <param name="callback"></param>
        public virtual void GetBytes(string name, Action<string, byte[]> callback)
        {
            LoadAssetAsync(name).callback =
                (key, res) =>
                {
                    var txt = res as TextAsset;
                    callback(name, txt.bytes);
                };
        }

        public byte[] GetLuaBytes(string key)
        {
            if (luaBytes.ContainsKey(key))
            {
                return luaBytes[key];
            }

            return null;
        }

        /// <summary>
        /// 从assetbundle中读取字符串
        /// </summary>
        /// <param name="name"></param>
        /// <param name="callback"></param>
        /// <returns></returns>
        public virtual void GetString(string name, Action<string, string> callback)
        {
            LoadAssetAsync(name).callback =
                (key, res) =>
                {
                    var txt = res as TextAsset;
                    callback(name, txt.text);
                }
                ;
        }


        //ɾ����Դ����
        public virtual void ReleaseAsset(string name, UnityEngine.Object obj, bool recycle = false)
        {
            if (obj is GameObject)
            {
                if (recycle)
                {
                    GameObjectPool.RecycleGameObject(name, obj as GameObject);
                }
                else
                {
                    GameObject.Destroy(obj);
                }
            }
            else
            {
                ReleaseRefAsset(name);
            }
        }

        public void ReleaseRefAsset(string key)
        {
            RefPool.ReleaseObject(key);
        }


        public string GetLocalVersionStr()
        {
            return "1.0.0";
        }

        public void ReleaseCallBack(string key, Action<string, UnityEngine.Object> callback)
        {
            for (int i = 0; i < comingRequests.Count; i++)
            {
                var req = comingRequests[i];
                if (req.key == key && req.callback.Equals(callback))
                {
                    comingRequests.Remove(req);
                    return;
                }
            }

            var cancelReq = new LoadRequest(key);
            cancelReq.callback = callback;
            if (cancelRequests.TryGetValue(key, out List<LoadRequest> list))
            {
                list.Add(cancelReq);
            }
            else
            {
                list = new List<LoadRequest>();
                list.Add(cancelReq);
                cancelRequests.Add(key, list);
            }
        }

        public void PreLoadAssets(Action callback, bool showProgress = false)
        {
            StartCoroutine(PreLoadAssetsIe(callback, showProgress));
        }

        #endregion


        private AssetStatus _status;
        private bool enableHotUpdate = false;

        public static AsyncOperationHandle defaultHandle;

        internal Dictionary<AsyncOperationHandle, GameObject> handleDic =
            new Dictionary<AsyncOperationHandle, GameObject>();

        private Dictionary<string, List<LoadRequest>> cancelRequests = new Dictionary<string, List<LoadRequest>>();
        private List<LoadRequest> comingRequests = new List<LoadRequest>();
        private List<LoadRequest> loadingRequests = new List<LoadRequest>();
        private object lockObj = new object();
        private int maxLoadingCount = 10;


        private void SetStatus(AssetStatus status)
        {
            _status = status;
            Events<AssetStatus>.Broadcast(EventsType.assetStatusChange,status);
        }
        private void SetProgress(float progress)
        {
            Events<float>.Broadcast(EventsType.assetProgressChange,progress);
        }
        IEnumerator InitializeAsync()
        {
            var inithandle = Addressables.InitializeAsync();
            inithandle.Completed += InitialCompleted;
            SetStatus(AssetStatus.Init);
            while (!inithandle.IsDone)
            {
                SetProgress(inithandle.PercentComplete);
                yield return null;
            }

            yield return null;
        }

        private void InitialCompleted(AsyncOperationHandle<IResourceLocator> obj)
        {
            if (!enableHotUpdate)
            {
                SetStatus(AssetStatus.Ready);
                SetProgress(1);
                m_Initialize = true;
                return;
            }
            SetStatus(AssetStatus.CheckUpdate);
            SetProgress(0);
            Addressables.CheckForCatalogUpdates(true).Completed += CheckComplete;
        }

        private void CheckComplete(AsyncOperationHandle<List<string>> obj)
        {
            if (obj.Status == AsyncOperationStatus.Failed)
            {
                Debug.LogError(" 检查 catalog失败");
                m_Initialize = true;
                SetStatus(AssetStatus.Ready);
                SetProgress(1);
                return;
            }

            Debug.Log(obj.Result.Count);
            if (obj.Result.Count == 0)
            {
                CalcDownLoadSize();
                return;
            }

            Addressables.UpdateCatalogs(obj.Result).Completed += UpdateCatalogsComplete;
        }


        private void UpdateCatalogsComplete(AsyncOperationHandle<List<IResourceLocator>> obj)
        {
            Debug.Log(obj.Status);
            if (obj.Status == AsyncOperationStatus.Failed)
            {
                m_Initialize = true;
                SetStatus(AssetStatus.Ready);
                SetProgress(1);
                return;
            }

            CalcDownLoadSize(obj.Result);
        }

        long updateSize;

        public long GetDownloadSize()
        {
            return updateSize;
        }

       private Dictionary<IResourceLocator, long> needDownLoadDic = new Dictionary<IResourceLocator, long>();
        public void CalcDownLoadSize(List<IResourceLocator> obj = null)
        {
            StartCoroutine(CalcDownLoadSizeIe(obj));
        }

        public IEnumerator CalcDownLoadSizeIe(List<IResourceLocator> obj = null)
        {
            needDownLoadDic.Clear();
            long tempSize = 0;
            updateSize = 0;
            var locators = Addressables.ResourceLocators;
            if (obj != null)
            {
                locators = obj;
            }
            
            float index = 0;
            int count = obj == null ? 2 : obj.Count;
            foreach (var item in locators)
            {
                var handle = Addressables.GetDownloadSizeAsync(item.Keys);
                yield return handle;
                if (handle.Result > 0)
                {
                    tempSize += handle.Result;
                    needDownLoadDic.Add(item, handle.Result);
                }
                Addressables.Release(handle);
            }

            updateSize = tempSize;
            SetStatus(AssetStatus.ConfirmUpdate);
            SetProgress(0);
        }
        
         public void DownLoadResource()
        {
            StartCoroutine(DownLoadResourceIe());
        }

        public IEnumerator DownLoadResourceIe()
        {
            SetStatus(AssetStatus.Update);
            SetProgress(0);
            float currentDownloadSize = 0;
            float totalDownLoadSize = 0;
            float downloadPercent = 0;
            if (updateSize > 0)
            {
                foreach (var item in needDownLoadDic)
                {
                    Debug.Log("download " + item.Key);
                    var downloadHandle =
                        Addressables.DownloadDependenciesAsync(item.Key.Keys, Addressables.MergeMode.Union);
                    while (!downloadHandle.IsDone)
                    {
                        currentDownloadSize = downloadHandle.PercentComplete * item.Value;
                        downloadPercent = (totalDownLoadSize + currentDownloadSize) / updateSize;
                        SetProgress(downloadPercent);
                        yield return null;
                    }

                    totalDownLoadSize += currentDownloadSize;
                    Addressables.Release(downloadHandle);
                }
                SetStatus(AssetStatus.Ready);
                SetProgress(1);
                m_Initialize = true;
            }
            else
            {
                SetStatus(AssetStatus.Ready);
                m_Initialize = true;
            }
        }



        private IEnumerator PreLoadAssetsIe(Action callback, bool showProgress = false)
        {
            if (showProgress)
            {
               SetStatus(AssetStatus.Preload);
            }

            var handle = Addressables.LoadAssetsAsync<Object>((IEnumerable) new List<string>() {".bytes", "lua"}, null,
                Addressables.MergeMode.Intersection);
            while (!handle.IsDone)
            {
                if (showProgress)
                {
                    SetProgress(handle.PercentComplete);
                }

                yield return null;
            }

            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                var list = handle.Result as List<Object>;
                for (int i = 0; i < list.Count; i++)
                {
                    var txt = list[i] as TextAsset;
                    if (txt)
                    {
                        luaBytes[txt.name + ".bytes"] = txt.bytes;
                    }
                }
            }

            ReleaseHandle(handle, true);


            if (callback != null)
            {
                callback();
            }
        }

        private Dictionary<string, byte[]> luaBytes = new Dictionary<string, byte[]>();

        public const string RESOURCE_ASSETBUNDLE_PATH = "ResDLC";


        public LoadRequest LoadAssetAsyncQueue(string key, bool isSprite = false)
        {
            var loadingInfo = IsLoading(key);
            if (loadingInfo.Item1)
            {
                return loadingInfo.Item2;
            }
            var req = new LoadRequest(key);
            req.isSprite = isSprite;
            comingRequests.Add(req);
            return req;
        }

        private void UpdateLoadQueue()
        {
            var count = comingRequests.Count;
            var loadingCount = loadingRequests.Count;
            while (loadingCount < maxLoadingCount && count > 0)
            {
                var req = comingRequests[0];
                req.Load();
                comingRequests.Remove(req);
                loadingRequests.Add(req);
                count = comingRequests.Count;
                loadingCount = loadingRequests.Count;
            }

            while (loadingCount > 0)
            {
                var loadingReq = loadingRequests[0];
                if (RemoveCancel(loadingReq.key, loadingReq.callback))
                {
                    loadingRequests.Remove(loadingReq);
                    loadingCount = loadingRequests.Count;
                    switch (loadingReq.aasetFrom)
                    {
                        case LoadRequest.AssetFrom.None:
                            ReleaseHandle(loadingReq.handle, true);
                            break;
                        case LoadRequest.AssetFrom.GameobjectPool:
                            GameObjectPool.RecycleGameObject(loadingReq.key, loadingReq.Result as GameObject);
                            break;
                        case LoadRequest.AssetFrom.RefPool:
                            RefPool.ReleaseObject(loadingReq.key);
                            break;
                        case LoadRequest.AssetFrom.LoadGameobjectInstantiate:
                            GameObjectPool.RecycleGameObject(loadingReq.key, loadingReq.Result as GameObject);
                            break;
                        case LoadRequest.AssetFrom.LoadGameobjectNotInstantiate:
                            ReleaseHandle(loadingReq.handle, true);
                            break;
                        case LoadRequest.AssetFrom.LoadRefAsset:
                            RefPool.ReleaseObject(loadingReq.key);
                            break;
                        default:
                            break;
                    }

                    continue;
                }

                if (loadingReq.IsDone)
                {
                    loadingReq.CallBack();
                    loadingRequests.Remove(loadingReq);
                    loadingCount = loadingRequests.Count;
                    continue;
                }

                break;
            }
        }


        public LoadRequest LoadAssetAsync(string key, bool isSprite = false, LoadRequest req = null)
        {
            var loadingInfo = IsLoading(key);
            if (loadingInfo.Item1)
            {
                return loadingInfo.Item2;
            }
            if (req == null)
            {
                req = new LoadRequest(key);
            }

            req.isSprite = isSprite;
            lock (loadingRequests)
            {
                loadingRequests.Insert(0, req);
            }

            req.Load();
            return req;
        }

        private (bool, LoadRequest) IsLoading(string key)
        {
            //不行
            // foreach (var loading in loadingRequests)
            // {
            //     if (!loading.IsDone&&loading.key==key)
            //     {
            //         return (true, loading);
            //     }
            // }

            return (false, null);
        }


        public AsyncOperationHandle LoadSceneAsync(string key, Action<string> callback,
            LoadSceneMode loadSceneMode = LoadSceneMode.Single,
            bool activateOnLoad = true, int priority = 100)
        {
           var handle= Addressables.LoadSceneAsync(key, loadSceneMode, activateOnLoad, priority);
               handle.Completed += (res) =>
            {
                if (callback != null)
                {
                    callback(key);
                }
            };
               return handle;
        }

        public void ReleaseHandle(AsyncOperationHandle handle, bool forece = false)
        {
            if (handle.Equals(defaultHandle))
            {
                return;
            }

            if (forece)
            {
                Addressables.Release(handle);
                return;
            }

            GameObject go;
            if (handleDic.TryGetValue(handle, out go))
            {
                if (go != null)
                {
                    //加载的资源还在使用,不能释放,只能update释放
                    return;
                }
                else
                {
                    handleDic.Remove(handle);
                }
            }

            Addressables.Release(handle);
        }


        private void UpdateHandles()
        {
            foreach (var item in handleDic)
            {
                if (item.Value == null)
                {
                    handleDic.Remove(item.Key);
                    ReleaseHandle(item.Key);
                    return;
                }
            }
        }


        private bool RemoveCancel(string key, Action<string, UnityEngine.Object> callback)
        {
            if (cancelRequests.TryGetValue(key, out List<LoadRequest> list))
            {
                for (int i = 0; i < list.Count; i++)
                {
                    var req = list[i];
                    if (req.callback.Equals(callback))
                    {
                        list.Remove(req);
                        return true;
                    }
                }
            }

            return false;
        }

        void Update()
        {
            UpdateHandles();
            UpdateLoadQueue();
        }
    }
}