using UnityEngine;
using UnityEditor;

namespace EG
{
    [CustomEditor(typeof(UITweener), true)]
    public class EditorInspector_UITweener : Editor
    {
        bool m_DefaultFoldout = false;
        bool m_TweenerFoldout = false;

        public override void OnInspectorGUI ()
        {
            GUILayout.Space(6f);

            EditorGUIUtility.labelWidth = 110f;
            
            DrawCommonProperties();
            
            m_DefaultFoldout = EditorHelp.HeadStart( ref m_DefaultFoldout, "Default" );
            if( m_DefaultFoldout )
            {
                base.OnInspectorGUI();
            }
            EditorHelp.HeadEnd( );
        }
        
        protected void DrawCommonProperties ()
        {
            UITweener tw = target as UITweener;
            
            tw.OnInspectorGUI();
            
            m_TweenerFoldout = EditorHelp.HeadStart( ref m_TweenerFoldout, "Tweener" );
            if( m_TweenerFoldout )
            {
                EditorGUIUtility.labelWidth = 110f;
                
                GUI.changed = false;
                
                UITweener.Style style = (UITweener.Style)EditorGUILayout.EnumPopup( "Play Style", tw.style );
                
                UIEaseType method = (UIEaseType)EditorGUILayout.EnumPopup("Play Method", tw.method);
                
                AnimationCurve curve = tw.animationCurve;
                if( tw.method == UIEaseType.none )
                {
                    curve = EditorGUILayout.CurveField( "Animation Curve", curve, GUILayout.Width(170f), GUILayout.Height(62f) );
                }
                
                GUILayout.BeginHorizontal();
                float dur = EditorGUILayout.FloatField( "Duration", tw.duration, GUILayout.Width(170f) );
                GUILayout.Label( "seconds" );
                GUILayout.EndHorizontal();
                
                GUILayout.BeginHorizontal();
                float del = EditorGUILayout.FloatField( "Start Delay", tw.delay, GUILayout.Width(170f) );
                GUILayout.Label( "seconds" );
                GUILayout.EndHorizontal();
                
                UITweener.GroupType groupType = tw.tweenGroupType;
                string groupName = tw.tweenGroup;
                EditorGUILayout.BeginHorizontal();
                {
                    groupType = (UITweener.GroupType)EditorGUILayout.EnumPopup( "Tween Group", groupType, GUILayout.Width(170f) );
                    
                    if( tw.tweenGroupType == UITweener.GroupType.Free )
                    {
                        groupName = EditorGUILayout.TextField( "", groupName );
                    }
                    else if( tw.tweenGroupType == UITweener.GroupType.Window )
                    {
                        string[] names = System.Enum.GetNames( typeof(UITweener.WindowGroup) );
                        int groupValue = ArrayUtility.FindIndex<string>( names, ( prop ) => prop == groupName );
                        //if( groupValue == -1 ){ groupValue = 0; groupName = names[ 0 ]; GUI.changed = true; }
                        int next = EditorGUILayout.Popup( "", groupValue, names );
                        if( next != groupValue )
                        {
                            groupName = names[ next ];
                        }
                    }
                    else if( tw.tweenGroupType == UITweener.GroupType.Event )
                    {
                        string[] names = System.Enum.GetNames( typeof(UITweener.EventGroup) );
                        int groupValue = ArrayUtility.FindIndex<string>( names, ( prop ) => prop == groupName );
                        //if( groupValue == -1 ){ groupValue = 0; groupName = names[ 0 ]; GUI.changed = true; }
                        int next = EditorGUILayout.Popup( "", groupValue, names );
                        if( next != groupValue )
                        {
                            groupName = names[ next ];
                        }
                    }
                }
                EditorGUILayout.EndHorizontal();
                
                if( GUI.changed )
                {
                    EditorHelp.Undo( tw );
                    tw.animationCurve = curve;
                    tw.method = method;
                    tw.style = style;
                    //tw.ignoreTimeScale = ts;
                    tw.tweenGroupType = groupType;
                    tw.tweenGroup = groupName;
                    tw.duration = dur;
                    tw.delay = del;
                    EditorUtility.SetDirty( tw );
                }
            }
            EditorHelp.HeadEnd( );
            
            EditorGUIUtility.labelWidth = 80f;
        }
    }
}
