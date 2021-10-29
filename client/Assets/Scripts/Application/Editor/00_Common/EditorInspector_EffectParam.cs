using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using GFSYS;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.ProjectWindowCallback;
#endif

namespace EG
{
#if UNITY_EDITOR
    [UnityEditor.CustomEditor( typeof( EffectParam ), true )]
    public class EditorInspector_EffectParam : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            CustomFieldAttribute.OnInspectorGUI( typeof( EffectParam ), serializedObject );
            
            GUILayout.Space( 5 );
            
            EffectParam effParam = target as EffectParam;
            
            GUI.enabled = ( effParam.m_Prefab != null || effParam.m_EvParam != null );
            if( GUILayout.Button( "preview" ) )
            {
                if( effParam.m_Prefab != null )
                {
                    EditorWindow_EffectViewer.Init( effParam );
                }
                else if( effParam.m_EvParam )
                {
                    EditorWindow_Event.Init( effParam.m_EvParam );
                }
            }
            GUI.enabled = true;
            
            string txt = "没有设置效果Prefab或EventParam。  ";
            Color clr = Color.red;
            
            if( effParam.m_Prefab != null )
            {
                txt = "效果查看器就会打开。";
                clr = Color.green;
            }
            else if( effParam.m_EvParam != null )
            {
                txt = "打开事件编辑器。";
                clr = Color.green;
            }
            
            GUI.contentColor = clr;
            GUILayout.Label( txt );
            GUI.contentColor = Color.white;
        }
     }
#endif
}
