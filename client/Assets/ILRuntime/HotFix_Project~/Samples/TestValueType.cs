using System;
using System.Collections.Generic;
using UnityEngine;
using System.Text;

namespace HotFix_Project
{
    class TestValueType
    {
        public static void RunTest()
        {
            System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
            //Debug.Log("测试Vector3的各种运算");
            Vector3 a = new Vector3(1, 2, 3);
            Vector3 b = Vector3.one;

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

        public static void RunTest2()
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

        public static void RunTest3()
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

        public static void RunTestInLocal()
        {
            ValueTypeBindingDemo vtbd = new ValueTypeBindingDemo();
            vtbd.RunTestLocal();
        }

        public static void RunTest2InLocal()
        {

            ValueTypeBindingDemo vtbd = new ValueTypeBindingDemo();
            vtbd.RunTest2Local();

        }
        public static void RunTest3InLocal()
        {

            ValueTypeBindingDemo vtbd = new ValueTypeBindingDemo();
            vtbd.RunTest3Local();
      
        }
    }
}
