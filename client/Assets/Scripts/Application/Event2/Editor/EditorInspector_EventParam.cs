using UnityEngine;
using System.Collections.Generic;
using System.IO;
using UnityEditor;

namespace EG
{
    [CustomEditor(typeof(EventParam))]
    [CanEditMultipleObjects]
    public class EditorInspector_EventParam : Editor
    {
        [MenuItem("Assets/Create/ScriptableObject/EventParam",false,1)]
        public static ScriptableObject CreateAssetMenu()
        {
            return CreateAsset( DoCreateAction );
        }

        public static ScriptableObject CreateAsset( System.Action<ScriptableObject, string, string> createAct )
        {
            return EditorHelp.CreateScriptableObject<EventParam>( createAct );
        }
        
        private static void DoCreateAction( ScriptableObject sobj, string pathName, string resourceFile )
        {
            AssetDatabase.SetLabels( sobj, new string[] { "Data", "ScriptableObject", "EventParam" } );
        }
        
        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            if (targets.Length == 1)
            {
                EventParam player = (EventParam)target;
                
                EventTrack.ECategory preCategory = player.Category;
                player.Category = (EventTrack.ECategory)EditorGUILayout.EnumPopup( "ECategory", player.Category );
                if( preCategory != player.Category )
                {
                    EditorUtility.SetDirty(player);
                }
                
                bool loop = player.Loop;
                player.Loop = EditorGUILayout.Toggle( "Loop", player.Loop );
                if( loop != player.Loop )
                {
                    EditorUtility.SetDirty(player);
                }
                
                if( player.Loop )
                {
                    float loopstart = player.LoopStart;
                    player.LoopStart = EditorGUILayout.FloatField("LoopStart", player.LoopStart);
                    if (loopstart != player.LoopStart)
                    {
                        EditorUtility.SetDirty(player);
                    }
                }
                
                float length = player.Length;
                player.Length = EditorGUILayout.FloatField("Length", player.Length);
                if (length != player.Length)
                {
                    EditorUtility.SetDirty(player);
                }
                
                if( GUILayout.Button( "CalcLength" ) )
                {
                    player.Length = player.CalcLength( );
                    EditorUtility.SetDirty( player );
                }
                
                EditorGUI.BeginDisabledGroup(true);
                bool haveNullTrack = false;
                
                if( player != null && player.Events != null )
                {
                    EditorGUILayout.BeginHorizontal( );
                    EditorGUILayout.IntField( "Track", player.Events.Length );
                    EditorGUILayout.EndHorizontal( );
                    bool isTrackFoldout = EditorPrefs.GetBool( "eventparam_tracks", false );
                    isTrackFoldout = EditorGUILayout.Foldout( isTrackFoldout, "isTrackFoldout" );
                    if( isTrackFoldout )
                    {
                        foreach( var i in player.Events )
                        {
                            EditorGUILayout.BeginHorizontal();
                            GUILayout.Space( 10 );
                            if( i == null )
                            {
                                GUI.contentColor = Color.red;
                                EditorGUILayout.LabelField( "null reference" );
                                GUI.contentColor = Color.white;
                                haveNullTrack = true;
                            }
                            else
                            {
                                EditorGUILayout.LabelField( i.GetType().FullName );
                            }
                            EditorGUILayout.EndHorizontal();
                        }
                    }
                    EditorPrefs.SetBool( "eventparam_tracks", isTrackFoldout );
                    
                    bool isFoldout = EditorPrefs.GetBool( "eventparam_dependencies", false );
                    isFoldout = EditorGUILayout.Foldout( isFoldout, "isDependenciesFoldout" );
                    if( isFoldout )
                    {
                        string[] depends = AssetDatabase.GetDependencies( AssetDatabase.GetAssetPath( player ) );
                        System.Array.Sort<string>( depends, ( p1, p2 ) => p1.CompareTo( p2 ) );
                        EditorGUILayout.IntField( "Dependencies", depends.Length );
                        foreach (var i in depends)
                        {
                            if( i.IndexOf( "EventTrack" ) == -1 ) GUI.color = Color.white;
                            else GUI.color = Color.yellow;
                            EditorGUILayout.BeginHorizontal( );
                            GUILayout.Space( 10 );
                            EditorGUILayout.LabelField( System.IO.Path.GetFileName( i ) );
                            EditorGUILayout.EndHorizontal( );
                        }
                        GUI.color = Color.white;
                    }
                    EditorPrefs.SetBool( "eventparam_dependencies", isFoldout );
                }
                
                EditorGUI.EndDisabledGroup();
                
                GUI.enabled = haveNullTrack;
                if( GUILayout.Button( "ClearEmptyEventSlots" ) )
                {
                    ( ( EventParam )target ).ClearEmptyEventSlots();
                }
                GUI.enabled = true;
            }
            
            serializedObject.ApplyModifiedProperties();
            
            GUILayout.Space( 5 );
            
            if( GUILayout.Button( "Confirm" ) )
            {
                EditorWindow_Event.Init( ( EventParam )target );
            }
        }
    }
}
