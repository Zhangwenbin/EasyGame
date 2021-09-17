using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using ILRuntime.CLR.TypeSystem;
using ILRuntime.CLR.Method;
using ILRuntime.Runtime.Enviorment;

public class ValueTypeBindingDemo : MonoBehaviour
{
    //AppDomain是ILRuntime的入口，最好是在一个单例类中保存，整个游戏全局就一个，这里为了示例方便，每个例子里面都单独做了一个
    //大家在正式项目中请全局只创建一个AppDomain
    AppDomain appdomain;
    System.IO.MemoryStream fs;
    System.IO.MemoryStream p;

    void Start()
    {
        StartCoroutine(LoadHotFixAssembly());
    }

    IEnumerator LoadHotFixAssembly()
    {
        //首先实例化ILRuntime的AppDomain，AppDomain是一个应用程序域，每个AppDomain都是一个独立的沙盒
        appdomain = new ILRuntime.Runtime.Enviorment.AppDomain();
        //正常项目中应该是自行从其他地方下载dll，或者打包在AssetBundle中读取，平时开发以及为了演示方便直接从StreammingAssets中读取，
        //正式发布的时候需要大家自行从其他地方读取dll

        //!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
        //这个DLL文件是直接编译HotFix_Project.sln生成的，已经在项目中设置好输出目录为StreamingAssets，在VS里直接编译即可生成到对应目录，无需手动拷贝
#if UNITY_ANDROID
        WWW www = new WWW(Application.streamingAssetsPath + "/HotFix_Project.dll");
#else
        WWW www = new WWW("file:///" + Application.streamingAssetsPath + "/HotFix_Project.dll");
#endif
        while (!www.isDone)
            yield return null;
        if (!string.IsNullOrEmpty(www.error))
            UnityEngine.Debug.LogError(www.error);
        byte[] dll = www.bytes;
        www.Dispose();

        //PDB文件是调试数据库，如需要在日志中显示报错的行号，则必须提供PDB文件，不过由于会额外耗用内存，正式发布时请将PDB去掉，下面LoadAssembly的时候pdb传null即可
#if UNITY_ANDROID
        www = new WWW(Application.streamingAssetsPath + "/HotFix_Project.pdb");
#else
        www = new WWW("file:///" + Application.streamingAssetsPath + "/HotFix_Project.pdb");
#endif
        while (!www.isDone)
            yield return null;
        if (!string.IsNullOrEmpty(www.error))
            UnityEngine.Debug.LogError(www.error);
        byte[] pdb = www.bytes;
        fs = new MemoryStream(dll);
        p = new MemoryStream(pdb);
        appdomain.LoadAssembly(fs, p, new ILRuntime.Mono.Cecil.Pdb.PdbReaderProvider());

        InitializeILRuntime();
        yield return new WaitForSeconds(0.5f);
        RunTest();
        yield return new WaitForSeconds(0.5f);
        RunTestToLocal();
        yield return new WaitForSeconds(0.5f);
        RunTestLocal();
        yield return new WaitForSeconds(0.5f);
        RunTest2();
        yield return new WaitForSeconds(0.5f);
        RunTest2ToLocal();
        yield return new WaitForSeconds(0.5f);
        RunTest2Local();
        yield return new WaitForSeconds(0.5f);
        RunTest3();
        yield return new WaitForSeconds(0.5f);
        RunTest3ToLocal();
        yield return new WaitForSeconds(0.5f);
        RunTest3Local();
        
    }

    void InitializeILRuntime()
    {
#if DEBUG && (UNITY_EDITOR || UNITY_ANDROID || UNITY_IPHONE)
        //由于Unity的Profiler接口只允许在主线程使用，为了避免出异常，需要告诉ILRuntime主线程的线程ID才能正确将函数运行耗时报告给Profiler
        appdomain.UnityMainThreadID = System.Threading.Thread.CurrentThread.ManagedThreadId;
#endif
        //这里做一些ILRuntime的注册，这里我们注册值类型Binder，注释和解注下面的代码来对比性能差别
        appdomain.RegisterValueTypeBinder(typeof(Vector3), new Vector3Binder());
        appdomain.RegisterValueTypeBinder(typeof(Quaternion), new QuaternionBinder());
        appdomain.RegisterValueTypeBinder(typeof(Vector2), new Vector2Binder());
    }

    void RunTest()
    {
        Debug.Log("Vector3等Unity常用值类型如果不做任何处理，在ILRuntime中使用会产生较多额外的CPU开销和GC Alloc");
        Debug.Log("我们通过值类型绑定可以解决这个问题，只有Unity主工程的值类型才需要此处理，热更DLL内定义的值类型不需要任何处理");        
        Debug.Log("请注释或者解注InitializeILRuntime里的代码来对比进行值类型绑定前后的性能差别");
        //调用无参数静态方法，appdomain.Invoke("类名", "方法名", 对象引用, 参数列表);
        appdomain.Invoke("HotFix_Project.TestValueType", "RunTest", null, null);
    }

    void RunTest2()
    {
        Debug.Log("=======================================");
        Debug.Log("Quaternion测试");
        //调用无参数静态方法，appdomain.Invoke("类名", "方法名", 对象引用, 参数列表);
        appdomain.Invoke("HotFix_Project.TestValueType", "RunTest2", null, null);
    }

    void RunTest3()
    {
        Debug.Log("=======================================");
        Debug.Log("Vector2测试");
        //调用无参数静态方法，appdomain.Invoke("类名", "方法名", 对象引用, 参数列表);
        appdomain.Invoke("HotFix_Project.TestValueType", "RunTest3", null, null);
    }
    
    void RunTestToLocal()
    {
        //调用无参数静态方法，appdomain.Invoke("类名", "方法名", 对象引用, 参数列表);
        appdomain.Invoke("HotFix_Project.TestValueType", "RunTestInLocal", null, null);
    }

    void RunTest2ToLocal()
    {
        //调用无参数静态方法，appdomain.Invoke("类名", "方法名", 对象引用, 参数列表);
        appdomain.Invoke("HotFix_Project.TestValueType", "RunTest2InLocal", null, null);
    }

    void RunTest3ToLocal()
    {
        //调用无参数静态方法，appdomain.Invoke("类名", "方法名", 对象引用, 参数列表);
        appdomain.Invoke("HotFix_Project.TestValueType", "RunTest3InLocal", null, null);
    }
    
    
      public  void RunTestLocal()
        {
            System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
            //Debug.Log("测试Vector3的各种运算");
            Vector3 a = new Vector3(1, 2, 3);
            Vector3 b = Vector3.one;

            //Debug.Log("a + b = " + (a + b));
            //Debug.Log("a - b = " + (a - b));
            //Debug.Log("a * 2 = " + (a * 2));
            //Debug.Log("2 * a = " + (2 * a));
            //Debug.Log("a / 2 = " + (a / 2));
            //Debug.Log("-a = " + (-a));
            //Debug.Log("a == b = " + (a == b));
            //Debug.Log("a != b = " + (a != b));
            //Debug.Log("a dot b = " + Vector3.Dot(a, b));
            //Debug.Log("a cross b = " + Vector3.Cross(a, b));
            //Debug.Log("a distance b = " + Vector3.Distance(a, b));
            //Debug.Log("a.magnitude = " + a.magnitude);
            //Debug.Log("a.normalized = " + a.normalized);
            //Debug.Log("a.sqrMagnitude = " + a.sqrMagnitude);

            sw.Start();
            float dot = 0;
            for(int i = 0; i < 100000; i++)
            {
                a += Vector3.one;
                dot += Vector3.Dot(a, Vector3.zero);
            }
            sw.Stop();

            Debug.LogFormat("Value: a={0},dot={1}, time = {2}ms", a, dot, sw.ElapsedMilliseconds);
        }

        public  void RunTest2Local()
        {
            System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
            //Debug.Log("测试Vector3的各种运算");
            int a = 4;
            int b = 3;

            sw.Start();
            float dot = 0;
            for (int i = 0; i < 100000; i++)
            {
                a *= b;
            }
            sw.Stop();

            Debug.LogFormat("Value: a={0},dot={1}, time = {2}ms", a, dot, sw.ElapsedMilliseconds);
        }

        public  void RunTest3Local()
        {
            System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
            //Debug.Log("测试Vector2的各种运算");
            StringBuilder sb = new StringBuilder();
            string a = "dsfsf";
            string b = "";
            sb.Append(a);

            sw.Start();
            float dot = 0;
            for (int i = 0; i < 100000; i++)
            {
                sb.Append(b);
           
            }
            sw.Stop();

            Debug.LogFormat("Value: a={0},dot={1}, time = {2}ms", sb.ToString(), dot, sw.ElapsedMilliseconds);
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
}
