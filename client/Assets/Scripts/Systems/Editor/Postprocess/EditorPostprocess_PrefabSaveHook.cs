using UnityEngine;
using UnityEditor;
using System.Collections;
using System.IO;
using System.Text;
using System.Collections.Generic;
using System;
using System.Runtime.Serialization;

#pragma warning disable

namespace EG
{
    public class EditorPostprocess_PrefabSaveHookModificationProcessor : UnityEditor.AssetModificationProcessor
    {
        static string[] OnWillSaveAssets(string[] paths)
        {
            if (Application.isBatchMode)
            {
                return paths;
            }

            foreach (var path in paths)
            {
                if (path.EndsWith(".prefab", true, null))
                {
                    EditorPostprocess_PrefabSaveHookPostProcessor.DidSavePrefabs.Add(path);
                }
            }

            return paths;
        }
    }

    public class EditorPostprocess_PrefabSaveHookPostProcessor : AssetPostprocessor
    {
        public static HashSet<string> DidSavePrefabs = new HashSet<string>();
        static List<IPrefabSaveSubscriber> buffer = new List<IPrefabSaveSubscriber>();
        static PrefabSaveHookContext sharedContext = new PrefabSaveHookContext();

        static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
        {
            if (Application.isBatchMode || importedAssets.Length == 0)
            {
                return;
            }

            var containsPrefab = false;
            var changed = false;

            foreach (var path in importedAssets)
            {
                if (path.EndsWith(".prefab", true, null))
                {
                    containsPrefab = true;
                    if (OnPostprocessPrefab(path))
                    {
                        changed = true;
                    }
                }
            }

            if (containsPrefab)
            {
                DidSavePrefabs.Clear();
            }

            if (changed)
            {
                AssetDatabase.Refresh();
            }
        }

        static bool OnPostprocessPrefab(string path)
        {
            if (DidSavePrefabs.Remove(path))
            {
                return OnDidSavePrefab(path);
            }
            return false;
        }

        static bool OnDidSavePrefab(string path)
        {
            var prefab = PrefabUtility.LoadPrefabContents(path);
            // var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
            if (prefab == null)
            {
                return false;
            }

            var changed = false;
            prefab.GetComponentsInChildren(true, buffer);
            if (buffer.Count > 0)
            {
                var context = TypedContext.Create();
                var hooks = buffer.RentList();
                sharedContext.Subscribers = hooks;
                sharedContext.PrefabPath = path;
                context.Set(sharedContext);

                foreach (var o in buffer)
                {
                    var comp = o as Component;
                    if (comp == null)
                    {
                        continue;
                    }

                    var root = PrefabUtility.GetNearestPrefabInstanceRoot(comp.gameObject);
                    sharedContext.IsNested = root != null;

                    try
                    {
                        if (PrefabSaveProcessor.Process(comp, path, context))
                        {
                            changed = true;
                        }
                    }
                    catch (Exception e)
                    {
                        Debug.LogError(e, comp);
                    }
                }
                buffer.Clear();
                ListPool.Return(ref hooks);
                context.Dispose();
            }
            PrefabUtility.UnloadPrefabContents(prefab);
            return changed;
        }

        static void ReprocessPrefabSaveHook(string pathPrefix)
        {
            var changed = false;
            var guids = AssetDatabase.FindAssets("t:prefab");
            var paths = new List<string>();
            foreach (var guid in guids)
            {
                var path = AssetDatabase.GUIDToAssetPath(guid);
                if (path.StartsWith(pathPrefix))
                {
                    paths.Add(path);
                }
            }

            try
            {
                for (var i = 0; i < paths.Count; ++i)
                {
                    var path = paths[i];
                    if (!path.StartsWith("Assets/ResDLC/UIPanel/"))
                    {
                        continue;
                    }

                    if (EditorUtility.DisplayCancelableProgressBar("", $"({i}/{paths.Count}) {Path.GetFileName(path)}", (float)i / paths.Count))
                    {
                        Debug.Log("canceled");
                        break;
                    }

                    if (OnDidSavePrefab(path))
                    {
                        changed = true;
                    }
                }
            }
            finally
            {
                EditorUtility.ClearProgressBar();
                if (changed)
                {
                    AssetDatabase.Refresh();
                }
            }
        }

        [MenuItem("GFSYS/Build/ReprocessPrefabSaveHook")]
        static void ReprocessPrefabSaveHookForView()
        {
            ReprocessPrefabSaveHook("Assets/ResDLC/UIPanel/");
        }
    }

    static class PrefabSaveProcessor
    {
        public static bool Process(Component comp, string path, TypedContext context)
        {
            if (comp is IPrefabSaveHook hook)
            {
                return hook.OnDidSavePrefab(path, context);
            }
            else if (comp is IPrefabSaveNullValidation nullValidation)
            {
                EditorHelp.ValidateNullableSerializeFields(comp as MonoBehaviour);
                return false;
            }

            return false;
        }
    }
}
