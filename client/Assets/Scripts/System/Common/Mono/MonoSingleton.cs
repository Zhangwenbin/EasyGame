using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace EG
{
    [AddComponentMenu("Scripts/System/Common/MonoSingleton")]
    public abstract class MonoSingleton<T> : UnityEngine.MonoBehaviour where T : MonoSingleton<T>
    {

        protected static T m_Instance = null;

        protected bool m_Initialize = false;

        public bool isInitialized { get { return m_Initialize; } }


        public static T Instance
        {
            get
            {
                if (m_Instance == null)
                {
                    m_Instance = GameObject.FindObjectOfType(typeof(T)) as T;

                    if (m_Instance == null)
                    {
                        GameObject obj = MonoSingletonUtility.CreateObject(typeof(T));
                        if (obj != null)
                        {
                            m_Instance = obj.GetComponent<T>();
                            if (m_Instance == null)
                            {
                                Application.Quit();
                            }
                        }
                    }
                }

                return m_Instance;
            }
        }

        public static bool HasInstance()
        {
            return m_Instance != null;
        }


        /// ***********************************************************************

        /// ***********************************************************************
        public virtual void OnSceneLoaded(UnityEngine.SceneManagement.Scene sceneName, UnityEngine.SceneManagement.LoadSceneMode loadSceneMode)
        {
        }

        /// ***********************************************************************
        /// ***********************************************************************
        protected virtual void OnCreate()
        {
            UnityEngine.SceneManagement.SceneManager.sceneLoaded += OnSceneLoaded;
        }

        /// ***********************************************************************
        /// ***********************************************************************


        /// ***********************************************************************

        /// ***********************************************************************
        protected virtual void Awake()
        {
            OnCreate();
        }

        /// ***********************************************************************

        /// ***********************************************************************
        protected virtual void Start()
        {

        }

        /// ***********************************************************************

        /// ***********************************************************************
        public virtual void Initialize()
        {
            m_Initialize = true;

        }

        /// ***********************************************************************

        /// ***********************************************************************
        public virtual void Release()
        {
            m_Initialize = false;
        }

        /// ***********************************************************************

        /// ***********************************************************************
        public virtual void Reset()
        {
        }

    }

    public static class MonoSingletonUtility
    {
        /// ***********************************************************************

        /// ***********************************************************************
        public static GameObject CreateObject(System.Type type)
        {
            GameObject result = null;
            string typeName = type.Name;

            UnityEngine.Object res = Resources.Load("Singleton/" + typeName);
            if (res == null)
            {
                Application.Quit();
                return null;
            }

            result = GameObject.Instantiate(res) as GameObject;
            if (result == null)
            {
                Application.Quit();
                return null;
            }

            GameObject obj = GameObject.FindGameObjectWithTag("SINGLETON_DIR");
            if (obj == null)
            {
                res = Resources.Load("Singleton/singleton");
                if (res != null)
                {
                    obj = GameObject.Instantiate(res) as GameObject;
                    if (obj != null)
                    {
#if UNITY_EDITOR
                        if (UnityEditor.EditorApplication.isPlaying)
#endif
                        {
                            GameObject.DontDestroyOnLoad(obj);
                        }

                        result.transform.parent = obj.transform;
                    }
                }
            }
            else
            {
                result.transform.parent = obj.transform;
            }

            return result;
        }

    }
}