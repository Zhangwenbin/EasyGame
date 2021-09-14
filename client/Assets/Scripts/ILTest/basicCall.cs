using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EG
{
    public class basicCall : MonoBehaviour
    {
        public void Add(int a, int b)
        {
            Debug.Log(a+b);
        }

        public string Add(string a, string b)
        {
            return a + b;
        }
        
    }
}