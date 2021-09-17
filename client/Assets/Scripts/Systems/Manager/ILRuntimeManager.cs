﻿/**************************************************************************/
/*@brief  简要描述   
  @author zwb
***************************************************************************/

using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using ILRuntime.CLR.Method;
using ILRuntime.CLR.TypeSystem;
using ILRuntime.Runtime.Enviorment;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;

#endif
namespace EG
{
    //=========================================================================
    //简要注释
    //=========================================================================
    public class ILRuntimeManager : MonoSingleton<ILRuntimeManager>
    {
        //=========================================================================
        //private var  私有变量
        //=========================================================================
        //AppDomain是ILRuntime的入口，最好是在一个单例类中保存，整个游戏全局就一个，这里为了示例方便，每个例子里面都单独做了一个
        //大家在正式项目中请全局只创建一个AppDomain
        AppDomain appdomain;
        System.IO.MemoryStream fs;

        System.IO.MemoryStream p;
        //=========================================================================
        //public var  公有变量
        //=========================================================================

        IEnumerator LoadHotFixAssembly()
        {
            //首先实例化ILRuntime的AppDomain，AppDomain是一个应用程序域，每个AppDomain都是一个独立的沙盒
            appdomain = new ILRuntime.Runtime.Enviorment.AppDomain();
            //正常项目中应该是自行从其他地方下载dll，或者打包在AssetBundle中读取，平时开发以及为了演示方便直接从StreammingAssets中读取，
            //正式发布的时候需要大家自行从其他地方读取dll

            //!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
            //这个DLL文件是直接编译HotFix_Project.sln生成的，已经在项目中设置好输出目录为StreamingAssets，在VS里直接编译即可生成到对应目录，无需手动拷贝
// #if UNITY_ANDROID
//         WWW www = new WWW(Application.streamingAssetsPath + "/HotFix_Project.dll");
// #else
//         WWW www = new WWW("file:///" + Application.streamingAssetsPath + "/HotFix_Project.dll");
// #endif

            void GetBytes(string key, byte[] dll)
            {
                fs = new MemoryStream(dll);
                // p = new MemoryStream(pdb);
                appdomain.LoadAssembly(fs, null, new ILRuntime.Mono.Cecil.Pdb.PdbReaderProvider());


                InitializeILRuntime();
                OnHotFixLoaded();
            }

            AssetManager.Instance.GetBytes("HotFix_Project.bytes", GetBytes);
            yield return null;

            //PDB文件是调试数据库，如需要在日志中显示报错的行号，则必须提供PDB文件，不过由于会额外耗用内存，正式发布时请将PDB去掉，下面LoadAssembly的时候pdb传null即可
// #if UNITY_ANDROID
//         www = new WWW(Application.streamingAssetsPath + "/HotFix_Project.pdb");
// #else
//         www = new WWW("file:///" + Application.streamingAssetsPath + "/HotFix_Project.pdb");
// #endif
            // while (!www.isDone)
            //     yield return null;
            // byte[] pdb = www.bytes;
        }

        void InitializeILRuntime()
        {
#if DEBUG && (UNITY_EDITOR || UNITY_ANDROID || UNITY_IPHONE)
            //由于Unity的Profiler接口只允许在主线程使用，为了避免出异常，需要告诉ILRuntime主线程的线程ID才能正确将函数运行耗时报告给Profiler
            appdomain.UnityMainThreadID = Thread.CurrentThread.ManagedThreadId;
#endif
            //这里做一些ILRuntime的注册

            //first 重定向注册
            LitJson.JsonMapper.RegisterILRuntimeCLRRedirection(appdomain);

            //second 绑定注册
            ILRuntime.Runtime.Generated.CLRBindings.Initialize(appdomain);
        }

        private object MainObj;
        private IMethod startMethod, updateMethod;
        private bool init;

        void OnHotFixLoaded()
        {
            Debug.Log("通过IMethod调用方法");
            //预先获得IMethod，可以减低每次调用查找方法耗用的时间
            IType type = appdomain.LoadedTypes["HotFix_Project.Main"];

            //第二种方式
            MainObj = ((ILType) type).Instantiate();
            //根据方法名称和参数个数获取方法
            startMethod = type.GetMethod("Start", 1);
            updateMethod = type.GetMethod("Update", 0);
            appdomain.Invoke(startMethod, MainObj, gameObject);
            init = true;
        }

        private void OnDestroy()
        {
            if (fs != null)
                fs.Close();
            if (p != null)
                p.Close();
            fs = null;
            p = null;
        }
        //=========================================================================
        //static var  静态变量
        //=========================================================================

        //=========================================================================
        //property  属性
        //=========================================================================


        //=========================================================================
        //init 初始化
        //=========================================================================

        #region 初始化

        #endregion

        //=========================================================================
        //update 更新
        //=========================================================================

        #region 更新

        #endregion

        //=========================================================================
        //get/set 设置/获取
        //=========================================================================

        #region 设置/获取

        #endregion

        //=========================================================================
        //Isxxx 确认 
        //=========================================================================

        #region 确认

        #endregion

        //=========================================================================
        //preload 预加载
        //=========================================================================

        #region 预加载

        #endregion

        //=========================================================================
        //Editor 编辑器
        //=========================================================================

        #region 编辑器

#if UNITY_EDITOR

#endif

        #endregion
    }
}