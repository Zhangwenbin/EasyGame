using UnityEngine;
using UnityEditor;
using System;
using System.Collections;
using System.Collections.Generic;

namespace EG
{
    public class EditorWindow_FindEventPlayer : EditorWindow
    {
        
        public delegate void Callback( EventPlayer evPlayer );
        
        class Data
        {
            EventPlayer         m_EvPlayer      = null;
            bool                m_IsSelected    = false;
            
            public EventPlayer  EvPlayer
            {
                get { return m_EvPlayer; }
            }
            
            public Data( EventPlayer evPlayer, bool isSelected )
            {
                m_EvPlayer = evPlayer;
                m_IsSelected = isSelected;
            }
            
            public bool DrawGUI()
            {
                bool ret = false;
                
                GUILayout.BeginHorizontal( "box" );
                {
                    if( GUILayout.Button( m_EvPlayer.gameObject.name, GUI.skin.label, GUILayout.Width( 120 ) ) )
                    {
                        Selection.activeGameObject = m_EvPlayer.gameObject;
                    }
                    
                    string btnTxt = "select";
                    GUI.backgroundColor = Color.green;
                    if( m_IsSelected )
                    {
                        GUI.backgroundColor = Color.white;
                        btnTxt = "selected";
                    }
                    
                    GUI.enabled = m_IsSelected == false;
                    if( GUILayout.Button( btnTxt, GUILayout.Width( 80 ) ) )
                    {
                        ret = true;
                    }
                    GUI.backgroundColor = Color.white;
                    GUI.enabled = true;
                }
                GUILayout.EndHorizontal();
                
                return ret;
            }
        }
        

        Callback                    m_Callback      = null;
        List<EventPlayer>           m_SelectedList  = new List<EventPlayer>();
        List<Data>                  m_Datas         = new List<Data>();
        Vector2                     m_ScrollPos     = Vector2.zero;
        
        
        public static EditorWindow_FindEventPlayer Create( Callback callback, List<EventPlayer> selected )
        {
            EditorWindow_FindEventPlayer window = GetWindow<EditorWindow_FindEventPlayer>();
            window.SetCallback( callback, selected );
            return window;
        }
        
        private void OnGUI()
        {
            _OnGUI();
        }
        
        private void _OnGUI()
        {
            if( m_Datas.Count == 0 )
            {
                EditorGUILayout.HelpBox("没有EventPlayer", MessageType.Warning );
                return;
            }
            
            m_ScrollPos = GUILayout.BeginScrollView( m_ScrollPos );
            {
                for( int i = 0, max = m_Datas.Count; i < max; ++i )
                {
                    if( m_Datas[i].DrawGUI() )
                    {
                        if( m_Callback != null )
                        {
                            m_Callback( m_Datas[i].EvPlayer );
                        }
                        
                        Close();
                    }
                }
            }
            GUILayout.EndScrollView();
        }
        
        public void SetCallback( Callback callback, List<EventPlayer> selected )
        {
            m_Callback = callback;
            m_SelectedList = selected;
            
            GatherEventPlayer();
        }
        
        void GatherEventPlayer()
        {
            m_Datas.Clear();
            
            EventPlayer[] evPlayers = GameObject.FindObjectsOfType<EventPlayer>();
            if( evPlayers == null || evPlayers.Length == 0 )
                return;
            
            for( int i = 0, max = evPlayers.Length; i < max; ++i )
            {
                m_Datas.Add( new Data( evPlayers[i], m_SelectedList.Contains( evPlayers[i] ) ) );
            }
        }
    }
}
