using UnityEngine;
using UnityEditor;
using UnityEngine.Playables;
using System.Collections.Generic;

namespace EG
{
    [UnityEditor.CustomEditor(typeof(EventPlayerMachine))]
    public class EditorInspector_EventPlayerMachine : UnityEditor.Editor
    {
        int m_Select = 0;
        
        public override void OnInspectorGUI( )
        {
            CustomFieldEditorButtonAttribute.EditorButton[0] = CustomButton_OpenEventWindow;
            CustomFieldAttribute.OnInspectorGUI( target.GetType( ), serializedObject );
            CustomFieldEditorButtonAttribute.EditorButton[0] = null;
            
            // ----------------------------------------
            
            if( Application.isPlaying )
            {
                EventPlayerMachine machine = target as EventPlayerMachine;
                EditorGUILayout.LabelField( "播放中", machine.Current );
                
                EditorGUILayout.BeginHorizontal( "box" );
                string[] keys = machine.Keys;
                int next = EditorGUILayout.Popup( m_Select, keys );
                if( next != m_Select )
                {
                    m_Select = next;
                }
                if( m_Select >= keys.Length ) m_Select = keys.Length-1;
                GUI.enabled = m_Select != -1 ? true: false;
                if( GUILayout.Button( "播放", GUILayout.Width(100) ) )
                {
                    machine.StopAll( );
                    machine.Play( keys[m_Select] );
                }
                GUI.enabled = machine.IsPlaying() ? true: false;
                if( GUILayout.Button( "停止", GUILayout.Width(100) ) )
                {
                    machine.StopAll( );
                }
                GUI.enabled = true;
                EditorGUILayout.EndHorizontal( );
            }
        }
        
        public void CustomButton_OpenEventWindow( SerializedProperty prop )
        {
            EventParam value = (EventParam)prop.objectReferenceValue;
            if( value != null )
            {
                EditorWindow_Event.Init( target as EventPlayerMachine, value );
            }
        }
    }
}
