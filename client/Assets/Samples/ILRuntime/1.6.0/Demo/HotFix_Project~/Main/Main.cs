using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace HotFix_Project
{
    class Main
    {
        private GameObject mainRoot;
        public void Start(GameObject g)
        {
            Debug.Log("start");
            mainRoot = g;
            TestCall();
        }

        public void Update()
        {
            Debug.Log("update");
        
        }

        /// <summary>
        /// 调用主工程方法,需要生成绑定代码,绑定注册
        /// </summary>
        public void TestCall()
        {
            EG.basicCall basicCall = mainRoot.GetComponent<EG.basicCall>();
            basicCall.Add(3, 5);
        }
    }
}
