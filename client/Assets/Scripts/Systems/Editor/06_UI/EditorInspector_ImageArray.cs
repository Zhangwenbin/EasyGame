using UnityEngine;
using UnityEditor;
using UnityEditor.UI;
using System;
using System.Collections;
using System.Collections.Generic;

namespace EG
{
   // [AddComponentMenu("Scripts/Application/EditorInspector_ImageArray.cs")]
    [CustomEditor(typeof(UIImageArray), true)]
    [CanEditMultipleObjects]
    public class EditorInspector_ImageArray :   ImageEditor
    {
        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            base.OnInspectorGUI();

            GUILayout.Space(10);

            CustomFieldAttribute.OnInspectorGUI(typeof(UIImageArray), serializedObject);

            /*
            SerializedProperty itr = serializedObject.GetIterator();
            bool enterChildren = true;
            System.Type targetType = serializedObject.targetObject.GetType();
            while (itr.NextVisible(enterChildren))
            {
                enterChildren = false;

                if (targetType.GetField(itr.name, System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public) != null)
                {
                    EditorGUILayout.PropertyField(itr, true);
                }
            }

            serializedObject.ApplyModifiedProperties();
            */
        }
    }
}
