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

        }

        public void Update()
        {
            Debug.Log("update");
        
        }

   
    }
}
