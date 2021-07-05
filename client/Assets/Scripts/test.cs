using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
namespace EG
{
    [CreateAssetMenu(fileName = "Data", menuName = "CreateSO/test", order = 1)]
    public class test : ScriptableObject
    {
        [Serializable]
        public class testData
        {
            public int id;
            public string name;
            public List<testDataChild> child;
        }
        [Serializable]
        public class testDataChild
        {
            public int id;
            public string name;
        }
        public List<testData> list;

    }
}