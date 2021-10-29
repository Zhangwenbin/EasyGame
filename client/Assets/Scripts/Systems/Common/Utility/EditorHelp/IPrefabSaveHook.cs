using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System;
using System.Reflection;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace EG
{
    public interface IPrefabSaveSubscriber
    {
    }

    public interface IPrefabSaveHook : IPrefabSaveSubscriber
    {
        /// <summary>
        ///如果加上MonoBehaviour，在Editor上prefab save完成后就会被调用。  
        ///具体来说，编辑时场景上的prefab的apply的时机，将其放到ProjectWindow的时候，  
        ///在双击prefab打开的prefab阶段上改变值时被称为。  
        ///在git中掉落的时候，或者在文本编辑器中直接编辑值的时候等UnityEditor外的变更不被称为。  
        ///注意在这个方法内即使改变SerializeField的值也不会反映在文件中。  
        ///想把MonoBehaviour的设定值写在别的数据上的时候，  
        /// SerializeField用于设定值是否符合设想等的检查。  
        ///需要AssetDatabase.Refresh()时返回true  
        /// </summary>
        bool OnDidSavePrefab(string path, TypedContext context);
    }

    public interface IPrefabSaveNullValidation : IPrefabSaveSubscriber
    {
    }

    public class PrefabSaveHookContext
    {
        public bool IsNested { get; set; }
        public string PrefabPath { get; set; }
        public List<IPrefabSaveSubscriber> Subscribers { get; set; }

        (int, int, int) IndexOf(IPrefabSaveHook target)
        {
            var type = target.GetType();
            var targetIndex = -1;
            var firstIndex = -1;
            var lastIndex = -1;
            for (var i = 0; i < Subscribers.Count; ++i)
            {
                var item = Subscribers[i];
                if (item == null) continue;
                if (ReferenceEquals(item, target)) targetIndex = i;
                var itemType = item.GetType();
                if (firstIndex == -1 && itemType == type) firstIndex = i;
                if (itemType == type) lastIndex = i;
            }
            return (targetIndex, firstIndex, lastIndex);
        }

        public bool IsFirst(IPrefabSaveHook target)
        {
            if (target == null) throw new ArgumentNullException(nameof(target));
            var (targetIndex, firstIndex, _) = IndexOf(target);
            return targetIndex == firstIndex;
        }

        public bool IsLast(IPrefabSaveHook target)
        {
            if (target == null) throw new ArgumentNullException(nameof(target));
            var (targetIndex, _, lastIndex) = IndexOf(target);
            return targetIndex == lastIndex;
        }
    }
}
