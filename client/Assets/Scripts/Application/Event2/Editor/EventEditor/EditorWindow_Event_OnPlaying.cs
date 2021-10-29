using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace EG
{
    public partial class EditorWindow_Event : EditorWindow
    {
        
        private void InitializeOnPlaying( )
        {
            mEventBGTex = EditorGUIUtility.Load( "eventbg.png" ) as Texture2D;
            
            CreateGUIStyle();

            RefreshEventClasses( );
            
            EditorApplication.playModeStateChanged += OnChangePlayModeState;    // 
            EditorApplication.update += OnUpdateOnPlaying;                               // 
            SceneView.onSceneGUIDelegate += OnSceneGUI;
            
            // 
            TimelineInfo timeline_info = new TimelineInfo( this, mBaseEventClasses );
            mTimelineInfo.Add( timeline_info );
            
            m_IsPlayMode = true;
            mInitialized = true;
        }
        

        private void ReleaseOnPlaying( )
        {
            mSerializedEvent = null;
            
            // 
            EditorApplication.playModeStateChanged -= OnChangePlayModeState;
            EditorApplication.update -= OnUpdateOnPlaying;
            SceneView.onSceneGUIDelegate -= OnSceneGUI;
            SceneView.RepaintAll( );
            
            if( mTimelineInfo.Count > 0 )
            {
                for( int i = 0, max = mTimelineInfo.Count; i < max; ++i )
                {
                    mTimelineInfo[i].Release();
                }
            }
            
            //  .
            if( EditorApplication.isPlayingOrWillChangePlaymode == Application.isPlaying )
            {
                mEvParamHistory.Release();
                mPrefabHistory.Release();
                ClearPlayModePathInfo( );
            }
        }
        
        
        private void DrawToolbarOnPlaying( Rect rc )
        {
            GUILayout.BeginArea( rc, GUIStyle.none );
            {
                GUILayout.BeginHorizontal();
                {
                    GUI.contentColor = Color.red;
                    GUILayout.Label("OnPlay模式：", GUILayout.Width( 90 ) );
                    GUI.contentColor = Color.white;
                    
                    // 
                    if( GUILayout.Button( "选择EventPlayer", GUILayout.Width( 120 ) ) )
                    {
                        List<EventPlayer> selected = new List<EventPlayer>();
                        if( mTimelineInfo.Count > 0 )
                        {
                            for( int i = 0, max = mTimelineInfo.Count; i < max; ++i )
                            {
                                if( mTimelineInfo[ i ].m_EventPlayer != null )
                                {
                                    selected.Add( mTimelineInfo[ i ].m_EventPlayer );
                                }
                            }
                        }
                        mFindEvPlayer = EditorWindow_FindEventPlayer.Create( ( player ) =>
                        {
                            if( player != null )
                            {
                                SetPreviewFromSelection( player );
                                mSerializedEvent = null;
                            }
                        }, selected );
                    }
                }
                GUILayout.EndHorizontal();
            }
            GUILayout.EndArea();
        }
        

        private void OnUpdateOnPlaying()
        {
            for( int idx = 0; idx < mTimelineInfo.Count; ++idx )
            {
                mTimelineInfo[ idx ].UpdatePreviewObjectOnPlaying();
            }
            
            Repaint();
        }
        

        private bool DrawTimelineToolbarOnPlaying( Rect rc, int idx )
        {
            TimelineInfo info = mTimelineInfo[ idx ];
            
            GUILayout.BeginArea( rc, GUIStyle.none );
            GUILayout.BeginHorizontal();
            {
                GUILayout.Label( "Time:" + info.Time.ToString( "F2" ) + " (" + ( int )( info.Time / ( 1.0f / 30.0f ) + 0.5f ) + ")", GUILayout.Width( 120 ) );
                GUILayout.FlexibleSpace();
            }
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            {
                GUILayout.Label( "EventPlayer:", GUILayout.Width( 80 ) );
                
                string txt = "未选中";
                if( info.m_EventPlayer != null )
                {
                    txt = info.m_EventPlayer.gameObject.name;
                }
                
                if( info.EventParam != null )
                {
                    txt += " [" + info.EventParam.name + "]";
                }
                
                GUILayout.Label( txt, GUILayout.Width( 320 ) );
            }
            GUILayout.EndHorizontal();
            GUILayout.EndArea();
            
            return false;
        }
        
   
        private void DrawGUITimelineOnPlaying( Rect rc, TimelineInfo info, int idx )
        {
            Rect rcTimelineToolbar = rc;
            rcTimelineToolbar.height = TOOLBAR_HEIGHT * 2;
            
            float rect_y = rc.y + rcTimelineToolbar.height;
            rc = new Rect( 0, rect_y, position.width - PROPERTY_PANE_WIDTH - 16, 0 );
            rc.yMax = rect_y + info.TimelineHeight + OFFSET_TIMELINE_HEIGHT;
            
            Rect rcTypeColumn = rc;
            rcTypeColumn.width = TYPE_COLUMN_WIDTH;
            
            Rect rcTimelineColumn = rc;
            rcTimelineColumn.xMin = rcTypeColumn.xMax;
            
            if( DrawTimelineToolbarOnPlaying( rcTimelineToolbar, idx ) == false )
            {
                DrawTimelinePane( rcTimelineColumn, idx );
                info.DrawTypePane( rcTypeColumn, idx );
            }
        }


        partial class TimelineInfo
        {
            public void UpdatePreviewObjectOnPlaying()
            {
                if( m_EventPlayer == null )
                    return;
                
                EventPlayerStatus evStatus = m_EventPlayer.GetPlayingEventStatus();
                if( evStatus != null )
                {
                    m_EventParam = evStatus.Param;
                    m_Time = evStatus.Time;
                }
            }
            
            public void SetPreviewObjectOnPlaying( EventPlayer evPlayer )
            {
                m_EventPlayer = evPlayer;
            }

        }
    }
}
