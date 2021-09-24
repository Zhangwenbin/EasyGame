using UnityEngine;
using UnityEditor;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using ActivateData = System.Collections.Generic.List<System.Tuple<UnityEngine.GameObject, bool>>;

namespace EG
{
    public partial class EditorWindow_Event : EditorWindow
    {
        partial class TimelineInfo
        {
            public class TrackList
            {
                public EventTrack   event_track;
                public Rect         event_rect;
            }
            
            
            static uint                     s_UniqueId          = 0;
            
            public List<List<TrackList>>    Tracks              = new List<List<TrackList>>();
            public List<System.Type>        TrackTypes          = new List<Type>();
            
            public  EventPlayer             m_EventPlayer       = null;
            EventPlayerMachine              m_EventPlayerMcn    = null;
            EventParam                      m_EventParam        = null;
            int                             m_AnmIdx            = 0;
            string                          m_AddedAnmKey       = "";
            
            // 
            private List<EventClassInfo>    m_BaseEventInfos    = new List<EventClassInfo>();
            private List<EventClassInfo>    m_DispEventInfos    = new List<EventClassInfo>();
            private EditorWindow_Event      m_ParentWindow      = null;
            
            // 
            public  Vector2                 ScrollPosTimeline   = Vector2.zero;
            int                             m_TimelineHeight;
            
            uint                            m_UniqueId          = 0;
            GameObject                      m_PreviewRoot       = null;
            public  GameObject              PreviewObject;
            bool                            m_IsChangedParent   = false;
            
            float                           m_Time              = 0;
            float                           m_PreTime           = 0;
            float                           m_DeltaTime         = 0;
            bool                            m_IsSeekMove        = false;
            bool                            m_IsSeekMoveFirst   = true;
            bool                            m_IsDragSeekBar     = false;
            bool                            m_IsLooped          = false;
            bool                            m_IsEnded           = false;
            
            ActivateData                    m_ActivateDataTbl   = new ActivateData();
            
            EditClipInfo                    m_EditClipInfo      = null;
            
            public EventParam               event_player_parent;                      // 
            
            
            public EventParam   EventParam          { get { return m_EventParam;                            } }
            public int          TimelineHeight      { get { return m_TimelineHeight * TIME_LINE_HEIGHT;     } }
            public float        Time                { get { return m_Time;                                  } }
            public float        EventParamLength
            {
                get
                {
                    if( m_EventParam != null )
                    {
                        return m_EventParam.GetEndTime();
                    }
                    return 0;
                }
            }
            public float        AnmLength           { get { return EventParamLength + OFFSET_TIMELINE_LENGTH;       } }
            
            public bool         IsDragSeekBar       { get { return m_IsDragSeekBar; } set { m_IsDragSeekBar = value; } }
            public string       AddedAnmKey         { get { return m_AddedAnmKey; } }
            
            public string       DisplayTime
            {
                get
                {
                    return Time.ToString( "F2" ) + " (" + ( int )( Time / ( 1.0f / 30.0f ) + 0.5f ) + ")";
                }
            }
            

            public TimelineInfo( EditorWindow_Event parent, List<EventClassInfo> baseEvClasses )
            {
                m_UniqueId = ++s_UniqueId;
                
                m_ParentWindow = parent;
                m_BaseEventInfos.AddRange( baseEvClasses );
                m_DispEventInfos.AddRange( baseEvClasses );
                
                if( m_ParentWindow.IsPlayMode == false )
                {
                    m_PreviewRoot = new GameObject( "PreviewRoot" + m_UniqueId.ToString( "00" ) );
                    if( m_ParentWindow != null )
                    {
                        m_ParentWindow.AddChildToRootObject( m_PreviewRoot.transform );
                    }
                    SetObjectNotEditable( m_PreviewRoot, true );
                    
                    PreviewObject = null;
                }
                
                m_EventParam = null;
                Tracks = new List<List<TrackList>>(32);
                m_TimelineHeight = 1;
                
                m_EventPlayer = null;
                m_EventPlayerMcn = null;
                
                event_player_parent = null;
            }
            
            public void Release()
            {
                m_ActivateDataTbl.Clear();
                
                _DestroyEventPlayer();
                if( m_EventParam != null )
                {
                    if( m_EventParam.CheckPreLoad() )
                    {
                        m_EventParam.EditorUnloadPreLoad();
                    }
                }
                m_EventParam = null;
                event_player_parent = null;
                
                m_ParentWindow = null;
                m_BaseEventInfos.Clear();
                m_DispEventInfos.Clear();
                
                DestroyPreviewObject();
                
                if( m_PreviewRoot != null )
                {
                    DestroyImmediate( m_PreviewRoot );
                    m_PreviewRoot = null;
                }
                
                if( m_EditClipInfo != null )
                {
                    m_EditClipInfo.Release();
                    m_EditClipInfo = null;
                }
            }
            
            public void DestroyPreviewObject()
            {
                if( PreviewObject != null )
                {
                    SetPreviewObject( null );
                }
            }
            
            public void ElapseTime( float dt )
            {
                m_PreTime = m_Time;
                m_DeltaTime = dt;
                m_Time += m_DeltaTime;
                m_IsLooped = false;
                m_IsEnded = false;
                
                if( EventParam != null )
                {
                    if( m_Time >= EventParamLength )
                    {
                        if( m_ParentWindow.IsForceLoop )
                        {
                            m_Time -= EventParamLength;
                            m_IsLooped = true;
                        }
                        else
                        {
                            m_Time = EventParamLength;
                            m_DeltaTime = 0;
                            m_ParentWindow.NotifyEndStop();
                            m_IsEnded = true;
                        }
                    }
                }
            }
            
            public void SeekTime( float dstTime )
            {
                m_Time = Mathf.Clamp( dstTime, 0, AnmLength );
                m_IsSeekMove = true;
            }
            
            public void SeekTimeOffset( float offset )
            {
                SeekTime( m_Time + offset );
            }
            
            public bool InputMoveSeekbar()
            {
                bool isNeedRepaint = false;
                
                if( m_ParentWindow.IsPlayMode )
                    return isNeedRepaint;
                
                if( EventParam == null )
                    return isNeedRepaint;
                
                float timeDragPos = 0;
                if( Event.current.type == EventType.MouseDown )
                {
                    m_IsDragSeekBar = true;
                    timeDragPos = m_Time;
                    m_ParentWindow.StopPlaying();
                }
                else if( Event.current.type == EventType.MouseUp )
                {
                    m_IsDragSeekBar = false;
                }
                
                if( m_IsDragSeekBar )
                {
                    timeDragPos = Pos2Time( Event.current.mousePosition.x + ScrollPosTimeline.x );
                    timeDragPos = Mathf.Clamp( timeDragPos, 0, AnmLength );
                    float nextTime = SnapTime( timeDragPos );
                    if( m_Time != nextTime )
                    {
                        m_IsSeekMove = true;
                    }
                    m_Time = nextTime;
                    
                    isNeedRepaint = true;
                }
                
                Event.current.Use();
                
                return isNeedRepaint;
            }
            
            public void UpdateEventPlayer( float time, float dt, bool isSeek = false )
            {
                if( m_EventPlayer != null )
                {
                    m_EventPlayer.UpdateManual( time, dt, m_IsLooped, m_IsEnded, isSeek );
                }
            }
            
            public void SetPreviewObject( GameObject obj, bool isChangeParent = true )
            {
                if( PreviewObject == obj )
                    return;

                _DestroyEventPlayer();
                    
                if( PreviewObject != null )
                {
                    if( m_IsChangedParent )
                    {
                        DestroyImmediate( PreviewObject );
                    }
                    PreviewObject = null;
                }
                
                PreviewObject = obj;
                m_IsChangedParent = isChangeParent;
                if( PreviewObject != null )
                {
                    m_EventPlayer = PreviewObject.GetComponentInChildren<EventPlayer>();
                    if( m_EventPlayer == null )
                    {
                        m_EventPlayer = PreviewObject.AddComponent<EventPlayer>();
                    }
                        
                    if( isChangeParent )
                    {
                        PreviewObject.transform.SetParent( m_PreviewRoot.transform, false );
                    }
                }
                    
                // EventPlayerMachine
                if( m_EventPlayer != null )
                {
                    m_EventPlayerMcn = m_EventPlayer.gameObject.GetComponent<EventPlayerMachine>();
                    if( m_EventPlayerMcn != null )
                    {
                        m_EventPlayerMcn.EditorCreateAnim();
                        
                        _GatherActivateData();
                    }
                    
                    if( m_EventPlayer.AnimationPlayer != null )
                    {
                        m_EventPlayer.AnimationPlayer.ChangeTimeUpdateMode( UnityEngine.Playables.DirectorUpdateMode.Manual );
                    }
                }
            }
            
            public void RevertPrefab()
            {
                if( PreviewObject == null )
                    return;

#pragma warning disable CS0618 // 
                GameObject root = PrefabUtility.FindPrefabRoot( PreviewObject );
#pragma warning restore CS0618 // 
                if( root != null )
                {
#pragma warning disable CS0618 // 
                    PrefabUtility.RevertPrefabInstance( PreviewObject );
#pragma warning restore CS0618 // 
                }
            }
            
            
            public void CreateNewEventParam()
            {
                string path = EditorUtility.SaveFilePanel("选择要创建文件的位置", Application.dataPath + "/ResourcesDLC/Unit/ANIM/", "", "asset" );
                if( string.IsNullOrEmpty( path ) == false )
                {
                    string assetPath = path.Substring( Application.dataPath.Length );
                    assetPath = "Assets" + assetPath;
                    EventParam evParam = AssetDatabase.LoadAssetAtPath<EventParam>( assetPath );
                    if( evParam == null )
                    {
                        if( System.IO.File.Exists( path ) )
                        {
                            Debug.LogError("无法载入 >> " + assetPath );
                        }
                        else
                        {
                            ScriptableObject obj = EditorInspector_EventParam.CreateAsset( null );
                            AssetDatabase.CreateAsset( obj, assetPath );
                            EditorUtility.SetDirty( obj );
                            AssetDatabase.SaveAssets();
                            AssetDatabase.Refresh();
                            
                            evParam = AssetDatabase.LoadAssetAtPath<EventParam>( assetPath );
                        }
                    }
                    
                    if( evParam != null )
                    {
                        m_ParentWindow._SetEventParam( this, evParam );
                    }
                }
            }
            
            public void SetEventParam( EventParam evParam )
            {
                if( m_ParentWindow.CheckEntrySameEventParam( evParam ) )
                {
                    return;
                }
                
                if( evParam != null )
                {
                    evParam.ClearEmptyEventSlots();
                }
                
                if( m_EventParam != evParam )
                {
                    _AddEventParamToPlayer( evParam );
                    
                    m_PreTime = 0;
                    m_Time = 0;
                    m_IsSeekMoveFirst = true;
                }
            }
            
            public void ResetEventParam( EventParam evParam )
            {
                if( m_EventPlayer != null && string.IsNullOrEmpty( m_AddedAnmKey ) == false )
                {
                    if( m_EventPlayerMcn != null )
                    {
                        EventPlayer.CurrentEditorPlayer = m_EventPlayer;
                        m_EventPlayer.ReleaseObjectOnPlayTrack();
                        EventPlayer.CurrentEditorPlayer = null;
                    }
                    
                    m_EventPlayer.StopEvent( m_AddedAnmKey );
                    m_EventPlayer.RemoveEvent( m_AddedAnmKey );
                    
                    m_EventParam = null;
                    m_AddedAnmKey = "";
                    _AddEventParamToPlayer( evParam );
                }
            }
            
            void _AddEventParamToPlayer( EventParam evParam )
            {
                if( evParam == null )
                    return;
                
                EventParam preParam = m_EventParam;
                
                m_EventParam = evParam;
                RefreshEventTrackList();
                if( m_EventParam != null )
                {
                    if( m_EventParam.CheckPreLoad() )
                    {
                        m_EventParam.EditorStartPreLoad();
                    }
                }
                
                if( m_EventPlayer != null )
                {
                    string key = m_EventPlayer.IsContained( evParam );
                    if( string.IsNullOrEmpty( key ) )
                    {
                        ++m_AnmIdx;
                        m_AddedAnmKey = "evEdit" + m_UniqueId.ToString( "00" ) + "_" + m_AnmIdx.ToString( "00" );
                        m_EventPlayer.AddEvent( m_AddedAnmKey, evParam );
                    }
                    else
                    {
                        m_AddedAnmKey = key;
                    }
                    
                    m_EventPlayer.SetManualFlag( true );
                    m_EventPlayer.PlayEvent( m_AddedAnmKey );
                    m_EventPlayer.SetManualFlag( false );
                }
                
                if( preParam != null )
                {
                    if( preParam.CheckPreLoad() )
                    {
                        preParam.EditorUnloadPreLoad();
                    }
                    EditorUtility.SetDirty( preParam );
                    AssetDatabase.SaveAssets( );
                    AssetDatabase.ImportAsset( AssetDatabase.GetAssetPath( preParam ) );
                }
            }
            
            void _DestroyEventPlayer()
            {
                m_EventPlayerMcn = null;
                
                if( m_EventPlayer == null )
                    return;
                
                m_AddedAnmKey = null;
                
                m_EventPlayer.ReleaseOnEditor();
                m_EventPlayer = null;
            }
            

            
            public void SetEventTrackClassInfo( List<EventClassInfo> baseEvClasses )
            {
                m_BaseEventInfos = baseEvClasses;
                
                RefreshEventTrackList();
            }
            

            public void RefreshEventTrackList()
            {
                m_DispEventInfos.Clear();
                
                if( m_EventParam == null )
                    return;
                
                if( m_BaseEventInfos.Count == 0 )
                    return;
                
                EventTrack.ECategory ownCategory = m_EventParam.Category;
                for( int i = 0, max = m_BaseEventInfos.Count; i < max; ++i )
                {
                    if( EventTrack.IsDispPopupList( m_EventParam.Category, m_BaseEventInfos[i].category ) )
                    {
                        m_DispEventInfos.Add( m_BaseEventInfos[i] );
                    }
                }
            }
            

            public void DrawTrackList()
            {
                if( m_EventParam != null )
                {
                    GenericMenu menu = new GenericMenu( );
                    for( int class_item_idx = 1; class_item_idx < m_BaseEventInfos.Count; class_item_idx++ )
                    {
                        int idx = class_item_idx;
                        menu.AddItem( new GUIContent( m_BaseEventInfos[ idx ].listName ), false, ( ) =>
                            {
                                AddNewTrack( m_BaseEventInfos[ idx ].type );
                            } );
                    }
                    menu.ShowAsContext( );
                    Event.current.Use( );
                }
            }
            

            public void DrawNewEventComboBox()
            {
                EditorGUILayout.BeginVertical( GUILayout.Width( 200 ) );
                GUILayout.Space( 4 );
                
                GUI.backgroundColor = Color.white;
                int newEventIndex = EditorGUILayout.Popup( 0, m_DispEventInfos.Select( ( prop ) => prop.listName ).ToArray() );
                
                if( newEventIndex > 0 && m_EventParam != null )
                {
                    AddNewTrack( m_DispEventInfos[newEventIndex].type );
                }
                
                EditorGUILayout.EndVertical( );
            }
            

            private void AddNewTrack( System.Type addType )
            {
                Undo.RecordObject( m_EventParam, "Add event" );
                
                EventTrack newTrack = (EventTrack)CreateInstance( addType );
                newTrack.name = System.Guid.NewGuid( ).ToString( );
                newTrack.Start = m_Time;
                newTrack.End = newTrack.Start;
                newTrack.hideFlags = HideFlags.HideInHierarchy;
                
                if( m_ParentWindow != null )
                {
                    m_ParentWindow.SetNewSelectedEv( newTrack, 0 );
                }
                
                List<EventTrack> events = new List<EventTrack>( m_EventParam.Events );
                events.Add( newTrack );
                m_EventParam.Events = events.ToArray( );
                
                AssetDatabase.AddObjectToAsset( newTrack, m_EventParam );
                
                // AssetDatabase.SaveAssets( );
                
                AssetDatabase.ImportAsset( AssetDatabase.GetAssetPath( newTrack ) );
                
                ResetEventParam( m_EventParam );
                
                SortEventParamEvents( m_EventParam );
            }
            

            public bool RemoveTrack( List<SelectedEvent> selectedEvents, int idx )
            {
                List<EventTrack> events = new List<EventTrack>( EventParam.Events );
                bool result = false;
                
                Undo.RecordObject( EventParam, "Delete event" );
                
                foreach( SelectedEvent select_event in selectedEvents )
                {
                    if( select_event.TimelineIdx == idx )
                    {
                        select_event.EvTrack.EditorRelease();
                        events.Remove( select_event.EvTrack );
                        UnityEngine.Object.DestroyImmediate( select_event.EvTrack, true );
                        result = true;
                    }
                }
                EventParam.Events = events.ToArray();
                
                EditorUtility.SetDirty( EventParam );
                // AssetDatabase.SaveAssets( );
                AssetDatabase.ImportAsset( AssetDatabase.GetAssetPath( EventParam ) );
                
                return result;
            }

            
            public void UpdatePreviewObject()
            {
                if( PreviewObject == null )
                {
                    return;
                }
                
                if( m_EventParam == null )
                {
                    return;
                }
                
                if( m_ParentWindow.IsPlaying )
                {
                    UpdateEventPlayer( m_PreTime, m_DeltaTime );
                }
                else if( m_IsSeekMove )
                {
                    if( m_IsSeekMoveFirst )
                    {
                        UpdateEventPlayer( m_PreTime, 0.1f );   // 将这里设为0.1f，会产生效果问题，设为0.001f，会产生动作问题  
                        m_IsSeekMoveFirst = false;
                    }
                    UpdateEventPlayer( m_Time, 0, true );
                    m_IsSeekMove = false;
                }
                
                // 没有这个的话最初的第一次以后平方根的位置以外不被更新…  
                PreviewObject.SetActive( false );
                PreviewObject.SetActive( true );
                
                SceneView.RepaintAll();
                UnityEditorInternal.InternalEditorUtility.RepaintAllViews();
            }
            
            
            public void DrawTypePane( Rect rc, int idx )
            {
                GUI.BeginGroup( rc );
                
                int i = 0, height = 0;
                while( i < Tracks.Count )
                {
                    if( Tracks[ i ].Count <= 0 )
                    {
                        break;
                    }
                    
                    int j = i + 1;
                    while( j < Tracks.Count )
                    {
                        if( Tracks[ j ].Count <= 0 || Tracks[ i ][ 0 ].event_track.GetType() != Tracks[ j ][ 0 ].event_track.GetType() )
                        {
                            break;
                        }
                        ++j;
                    }
                    
                    GUIStyle style = m_ParentWindow.mEventStyle;
                    if( m_ParentWindow.CheckSameSelectEventTrack( Tracks[ i ][ 0 ].event_track ) )
                    {
                        style = m_ParentWindow.mSelectEventStyle;
                    }
                    
                    GUI.backgroundColor = Color.Lerp( Tracks[ i ][ 0 ].event_track.TrackColor, Color.grey, 0.5f );
                    
                    if( GUI.Button( new Rect( 0, TIME_LINE_HEIGHT + i * TRACK_HEIGHT, rc.width, ( j - i ) * TRACK_HEIGHT ), m_ParentWindow.GetClassName( Tracks[ i ][ 0 ].event_track.GetType() ), style ) )
                    {
                        m_ParentWindow.SetNewSelectedEv( Tracks[ i ][ 0 ].event_track, idx );
                    }
                    
                    i = j;
                    
                    if( height <= i )
                    {
                        height = i;
                    }
                }
                
                m_TimelineHeight = height + 1;
                
                GUI.contentColor = Color.white;
                GUI.backgroundColor = Color.black;
                GUI.Box( new Rect( rc.width - 1, -m_ParentWindow.mTimelineScrollPosY, 1, rc.height ), "", m_ParentWindow.mFillStyle );
                
                GUI.EndGroup();
            }
            

            public void DrawGUIEventParam()
            {
                EditorGUILayout.BeginVertical( GUILayout.Width( 400  ) );
                {
                    GUILayout.Space( 4 );
                    
                    EditorGUILayout.BeginHorizontal();
                    {
                        EditorGUILayout.LabelField( "EventParam:", GUILayout.Width( 82 ) );
                        
                        EventParam getEventParam = ( EventParam )EditorGUILayout.ObjectField( m_EventParam, typeof( EventParam ), true, GUILayout.Width( 140 ) );
                        if( m_EventParam != getEventParam )
                        {
                            m_ParentWindow._SetEventParam( this, getEventParam );
                        }
                        
                        {
                            EventParam fromHistory = m_ParentWindow.mEvParamHistory.DrawGUI<EventParam>();
                            if( fromHistory != null )
                            {
                                getEventParam = fromHistory;
                                if( m_ParentWindow.CheckEntrySameEventParam( getEventParam ) == false )
                                {
                                    SetEventParam( getEventParam );
                                    m_ParentWindow.SetNewSelectedEv( null, -1 );
                                }
                            }
                        }
                        
                        if( getEventParam != m_EventParam )
                        {
                            m_ParentWindow.mEvParamHistory.Add( m_EventParam );
                            SortEventParamEvents( m_EventParam );
                        }
                        
                        if( m_EventParam != null  )
                        {
                            GUILayout.Space( 5 );
                            GUILayout.Label("长度: ", GUILayout.Width( 40 ) );
                            float evLen = m_EventParam.Length;
                            evLen = EditorGUILayout.FloatField( evLen, GUILayout.Width( 40 ) );
                            
                            if( GUILayout.Button("自动调整长度", GUILayout.Width( 80 ) ) )
                            {
                                evLen = m_EventParam.CalcLength();
                            }
                            
                            if( evLen != m_EventParam.Length )
                            {
                                m_EventParam.Length = evLen;
                                EditorUtility.SetDirty( m_EventParam );
                            }
                        }
                    }
                    EditorGUILayout.EndHorizontal();
                    
                    // EventPlayerMachine用
                    EditorGUILayout.BeginHorizontal();
                    {
                        GUILayout.Label( "选择: ", GUILayout.Width( 40 ) );
                        
                        if( m_EventPlayerMcn != null )
                        {
                            EventParam evParam = m_EventPlayerMcn.DrawGUISelect( m_EventParam );
                            if( evParam != m_EventParam )
                            {
                                m_ParentWindow.mIsPlaying = false;
                                m_Time = 0;
                                ResetEventParam( evParam );

                                UpdateEventPlayer( 0, 0 );
                                _ResetActivate();
                                m_ParentWindow.ClearSelectedEv();
                            }
                        }
                        else
                        {
                            GUILayout.Label( "none" );
                        }
                        
                        if( m_EditClipInfo == null )
                        {
                            GUI.backgroundColor = Color.green;
                            GUI.enabled = m_ParentWindow.CheckSelectedEvTrack( typeof( EventTrackAnimation ) );
                            if( GUILayout.Button( "SetupEditAnimationClip", GUILayout.Width( 90 ) ) )
                            {
                                if( m_EditClipInfo == null )
                                {
                                    m_EditClipInfo = SetupEditAnimationClip( m_EventPlayer, m_ParentWindow.GetSelectedEvTrack(), PreviewObject );
                                }
                            }
                            GUI.enabled = true;
                            GUI.backgroundColor = Color.white;
                        }
                        else
                        {
                            GUI.backgroundColor = Color.red;
                            GUI.enabled = m_EditClipInfo != null;
                            if( GUILayout.Button( "ClipRelease", GUILayout.Width( 90 ) ) )
                            {
                                if( m_EditClipInfo != null )
                                {
                                    m_EditClipInfo.Release();
                                    m_EditClipInfo = null;
                                }
                            }
                            GUI.enabled = true;
                            GUI.backgroundColor = Color.white;
                        }
                    }
                    EditorGUILayout.EndHorizontal();
                }
                EditorGUILayout.EndVertical();
            }
            

            void _GatherActivateData()
            {
                m_ActivateDataTbl.Clear();
                
                if( PreviewObject == null )
                    return;
                
                __AddActivateData( PreviewObject.transform );
            }
            
            void __AddActivateData( Transform trans )
            {
                if( trans == null )
                    return;
                
                m_ActivateDataTbl.Add( new Tuple<GameObject, bool>( trans.gameObject, trans.gameObject.activeSelf ) );
                
                if( trans.childCount == 0 )
                    return;
                
                for( int i = 0, max = trans.childCount; i < max; ++i )
                {
                    __AddActivateData( trans.GetChild(i) );
                }
            }
            
            void _ResetActivate()
            {
                if( m_ActivateDataTbl.Count == 0 )
                    return;
                
                for( int i = 0, max = m_ActivateDataTbl.Count; i < max; ++i )
                {
                    m_ActivateDataTbl[i].Item1.SetActive( m_ActivateDataTbl[i].Item2 );
                }
            }
            

            public bool IsDuplicatedEventPlayer( EventPlayer evPlayer )
            {
                if( m_EditClipInfo == null )
                    return false;
                
                return m_EditClipInfo.IsSameEventPlayer( evPlayer );
            }
            
        }
        

        private void DrawEventPane( int idx )
        {
            TimelineInfo info = mTimelineInfo[idx];
            
            float pos_y = 0f;
            if( idx > 0 )
            {
                for( int i = 1; i <= idx; i++ )
                {
                    pos_y += mTimelineInfo[ i - 1 ].TimelineHeight + OFFSET_EVENT_PANE_HEIGHT;
                }
            }
            
            float start_pos_y = pos_y + TOOLBAR_HEIGHT * idx - mTimelineScrollPosY;
            
            Rect rc = new Rect( 0, start_pos_y, position.width - PROPERTY_PANE_WIDTH - 16, 0 );
            
            if( EditorApplication.isPlaying )
            {
                DrawGUITimelineOnPlaying( rc, info, idx );
            }
            else
            {
                DrawGUITimeline( rc, info, idx );
            }
        }
        

        private void DrawGUITimeline( Rect rc, TimelineInfo info, int idx )
        {
            Rect rcTimelineToolbar = rc;
            rcTimelineToolbar.height = TOOLBAR_HEIGHT * 3;
            
            float rect_y = rc.y + rcTimelineToolbar.height;
            rc = new Rect( 0, rect_y, position.width - PROPERTY_PANE_WIDTH - 16, 0 );
            rc.yMax = rect_y + info.TimelineHeight + OFFSET_TIMELINE_HEIGHT;
            
            Rect rcTypeColumn = rc;
            rcTypeColumn.width = TYPE_COLUMN_WIDTH;
            
            Rect rcTimelineColumn = rc;
            rcTimelineColumn.xMin = rcTypeColumn.xMax;
            
            if( DrawTimelineToolbar( rcTimelineToolbar, idx ) == false )
            {
                DrawTimelinePane( rcTimelineColumn, idx );
                info.DrawTypePane( rcTypeColumn, idx );
            }
        }
        

        private void DrawTimelinePane( Rect rc, int idx )
        {

            TimelineInfo info = mTimelineInfo[ idx ];
            float animLength = rc.width;
            
            for( int i = 0; i < info.Tracks.Count; ++i )
            {
                info.Tracks[ i ].Clear();
            }
            
            if( info.EventParam != null )
            {
                animLength = Time2Pos( info.EventParamLength );
            }
            
            GUI.BeginGroup( rc );
            
            //float start_time_offset = Time2Pos( info.start_time_offset );
            float start_time_offset = Time2Pos( 0 );
            if( animLength > 0.0f && info.EventParam != null )
            {
                GUI.backgroundColor = new Color( 1, 1, 1, 0.1f );
                GUI.Box( new Rect( -info.ScrollPosTimeline.x + start_time_offset, 0, animLength, rc.height ), "", mFillStyle );
            }
            
            Rect rcTimeline = new Rect( -1, 0, rc.width + 2, TIME_LINE_HEIGHT );
            GUI.backgroundColor = Color.gray;
            GUI.Box( new Rect( 0, 0, rcTimeline.width, rcTimeline.height ), "", mEventStyle );
            
            if( info.IsDragSeekBar && Event.current.type == EventType.Ignore )
            {
                info.IsDragSeekBar = false;
            }
            
            bool selectEventTrigger = false;
            if( mSelectEventTriggerState == SelectEventTriggerState.DecideRange )
            {
                selectEventTrigger = true;
            }
            
            Rect rcDragMove = new Rect( -1, 0, rc.width + 2, rc.height );
            
            if( selectEventTrigger == false )
            {
                if( Event.current.isMouse
                && ( rcTimeline.Contains( Event.current.mousePosition ) || info.IsDragSeekBar )
                && Event.current.button == MOUSE_BUTTON_LEFT )
                {
                    if( info.InputMoveSeekbar() )
                    {
                        Repaint();
                        mUpddatePreviewObject = true;
                    }
                }
                
                if( Event.current.isMouse
                &&  Event.current.type == EventType.MouseDown
                && ( rcDragMove.Contains( Event.current.mousePosition ) )
                && info.IsDragSeekBar == false
                && Event.current.button == MOUSE_BUTTON_MIDDLE )
                {
                    mMouseDragMovePos.drag_flag = true;
                    mMouseDragMovePos.prev_pos_x = Event.current.mousePosition.x;
                    mMouseDragMovePos.time_line_idx = idx;
                }
                else if( mMouseDragMovePos.drag_flag && ( Event.current.type == EventType.MouseUp || Event.current.type == EventType.Ignore && idx == mMouseDragMovePos.time_line_idx ) )
                {
                    mMouseDragMovePos.drag_flag = false;
                }
                if( mMouseDragMovePos.drag_flag && idx == mMouseDragMovePos.time_line_idx && Event.current.type == EventType.MouseDrag )
                {
                    mMouseDragMovePos.offset_pos_x = Event.current.mousePosition.x - mMouseDragMovePos.prev_pos_x;
                    mMouseDragMovePos.prev_pos_x = Event.current.mousePosition.x;
                }
            }
            
            DrawGUIVerticalLine( rc, info.ScrollPosTimeline.x );
            
            bool clearSelection = ( Event.current.type == EventType.MouseDown && Event.current.button == MOUSE_BUTTON_LEFT );
            
            // Events
            if( info.EventParam != null )
            {
                int trackStart = 0;
                float yStart = TIME_LINE_HEIGHT;
                
                for( int typeIndex = 0; typeIndex < info.TrackTypes.Count; ++typeIndex )
                {
                    System.Type trackType = info.TrackTypes[ typeIndex ];
                    
                    int maxTrack = trackStart;
                    
                    for( int i = 0; i < info.EventParam.Events.Length; ++i )
                    {
                        EventTrack e = info.EventParam.Events[ i ];
                        
                        if( e == null || e.GetType() != trackType )
                        {
                            continue;
                        }
                        
                        int track = trackStart;
                        float x1 = e.Start;// + info.start_time_offset;
                        float x2 = e.End;// + info.start_time_offset;
                        
                        foreach( SelectedEvent select in mSelectedEventList )
                        {
                            if( select.EvTrack == e && mCanDragEvent && mPositionChanged )
                            {
                                AdjustTime( ref x1, ref x2, mDragOffset );
                                break;
                            }
                        }
                        
                        x1 = Time2Pos( x1 );
                        x2 = Time2Pos( x2 );// + mStride;   // 
                        
                        bool trackChanged;
                        do
                        {
                            trackChanged = false;
                            
                            while( info.Tracks.Count <= track )
                            {
                                info.Tracks.Add( new List<TimelineInfo.TrackList>( 32 ) );
                            }
                            
                            for( int j = 0; j < info.Tracks[ track ].Count; ++j )
                            {
                                EventTrack f = info.Tracks[ track ][ j ].event_track;
                                float bx1 = f.Start;// + info.start_time_offset;
                                float bx2 = f.End;// + info.start_time_offset;
                                
                                // 
                                foreach( SelectedEvent select in mSelectedEventList )
                                {
                                    if( select.EvTrack == f && mCanDragEvent && mPositionChanged )
                                    {
                                        AdjustTime( ref bx1, ref bx2, mDragOffset );
                                        break;
                                    }
                                }
                                
                                bx1 = Time2Pos( bx1 );
                                bx2 = Time2Pos( bx2 );
                                
                                if( x1 - SZING_KNOB_SIZE + 1 <= bx2 + SZING_KNOB_SIZE - 1 && bx1 - SZING_KNOB_SIZE + 1 <= x2 + SZING_KNOB_SIZE - 1 )
                                {
                                    ++track;
                                    trackChanged = true;
                                    break;
                                }
                            }
                        }
                        while( trackChanged );
                        
                        float y = yStart + TRACK_HEIGHT * track;
                        
                        x1 -= info.ScrollPosTimeline.x;
                        x2 -= info.ScrollPosTimeline.x;
                        
                        Rect eventRect = new Rect( x1, y, x2 - x1, TRACK_HEIGHT );
                        Rect knobRectL = new Rect( x1 - SZING_KNOB_SIZE, y, SZING_KNOB_SIZE, TRACK_HEIGHT );
                        Rect knobRectR = new Rect( x2, y, SZING_KNOB_SIZE, TRACK_HEIGHT );
                        
                        TimelineInfo.TrackList track_list = new TimelineInfo.TrackList();
                        track_list.event_track = e;
                        track_list.event_rect = eventRect;
                        track_list.event_rect.x += rc.x;
                        track_list.event_rect.y += rc.y;
                        
                        info.Tracks[ track ].Add( track_list );
                        
                        bool select_flag = false;
                        foreach( SelectedEvent select in mSelectedEventList )
                        {
                            if( e == select.EvTrack )
                            {
                                if( Event.current.type == EventType.MouseDrag
                                &&  Event.current.button == MOUSE_BUTTON_RIGHT )
                                {
                                    // 
                                    if( mCanDragEvent )
                                    {
                                        float delta = Event.current.delta.x / ( FPS_DEF * mStride );
                                        if( delta != 0.0f )
                                        {
                                            mDragOffset += delta;
                                            s_IsRepaint = true;
                                        }
                                        
                                        mPositionChanged = true;
                                        
                                        Event.current.Use();
                                    }
                                }
                                else if( ( Event.current.type == EventType.MouseUp || ( Event.current.type == EventType.Ignore && mPositionChanged ) )
                                     &&  ( Event.current.button == MOUSE_BUTTON_LEFT || Event.current.button == MOUSE_BUTTON_RIGHT ) )
                                {
                                    // 
                                    if( mPositionChanged )
                                    {
     
                                        foreach( SelectedEvent select_event in mSelectedEventList )
                                        {
                                            Undo.RecordObject( select_event.EvTrack, "Change Event Position" );
                                            
                                            float startTime = select_event.EvTrack.Start;
                                            float endTime = select_event.EvTrack.End;
                                            
                                            AdjustTime( ref startTime, ref endTime, mDragOffset );
                                            
                                            if( startTime < 0 ) startTime = 0;
                                            if( endTime < 0 ) endTime = 0;
                                            
                                            select_event.EvTrack.Start = startTime;
                                            select_event.EvTrack.End   = endTime;

                                            //select_event.EvTrack.Start = Mathf.Clamp( startTime, 0, mTimelineInfo[ select_event.TimelineIdx ].EventParamLength );
                                            //select_event.EvTrack.End = Mathf.Clamp( endTime, 0, mTimelineInfo[ select_event.TimelineIdx ].EventParamLength );

                                            EditorUtility.SetDirty( mTimelineInfo[ select_event.TimelineIdx ].EventParam );
                                        }
                                        //--------------------------------------
                                        
                                        Event.current.Use();
                                        Repaint();
                                    }
                                    
                                    mCanDragEvent = false;
                                    mDragOffset = 0;
                                    mPositionChanged = false;
                                    mMouseDownDragEvent = false;
                                }
                                
                                GUI.backgroundColor = Color.Lerp( e.TrackColor, Color.grey, 0.5f ) + Color.white * 0.25f;
                                
                                select_flag = true;
                                break;
                            }
                        }
                        
                        if( select_flag == false )
                        {
                            GUI.backgroundColor = Color.Lerp( e.TrackColor, Color.grey, 0.5f );
                        }
                        
                        // 
                        if( Event.current.type == EventType.MouseDown )
                        {
                            Vector2 p = Event.current.mousePosition;
                            
                            bool clickedEvent = eventRect.Contains( p );
                            bool clickedLeftKnob = knobRectL.Contains( p );
                            bool clickedRightKnob = knobRectR.Contains( p );
                            
                            // 
                            if( clickedEvent || clickedLeftKnob || clickedRightKnob )
                            {
                                if( Event.current.button == MOUSE_BUTTON_LEFT || Event.current.button == MOUSE_BUTTON_RIGHT )
                                {
                                    if( CheckMoveSelectEventTrack( e ) )
                                    {
                                        Undo.RecordObject( this, "Selection Change" );
                                        
                                        SetNewSelectedEv( e, idx );
                                        mLookEventParam = info.EventParam;
                                        clearSelection = false;
                                        mCanDragEvent = true;
                                        mPositionChanged = false;
                                        
                                        // 
                                        if( clickedEvent )
                                        {
                                            mDragTarget = DragTargets.Position;
                                        }
                                        else if( clickedLeftKnob )
                                        {
                                            mDragTarget = DragTargets.StartTime;
                                        }
                                        else if( clickedRightKnob )
                                        {
                                            mDragTarget = DragTargets.EndTime;
                                        }
                                    }
                                    
                                    if( Event.current.button == MOUSE_BUTTON_RIGHT )
                                    {
                                        mMouseDownDragEvent = true;
                                    }
                                }
                            }
                            
                            mDragOffset = 0;
                        }
                        
                        // 
                        GUI.Box( eventRect, "", mEventStyle );
                        
                        // 
                        GUI.Box( knobRectL, "", mSizingKnobStyleL );
                        GUI.Box( knobRectR, "", mSizingKnobStyleR );
                        
                        maxTrack = Mathf.Max( maxTrack, track );
                    }
                    
                    trackStart = maxTrack + 1;
                    
                    // 
                    GUI.backgroundColor = LINE_COLOR_M;
                    GUI.Box( new Rect( 0, yStart + trackStart * TRACK_HEIGHT, rc.width, 1 ), "", mFillStyle );
                }
            }
            
            if( info.IsDragSeekBar == false
            &&  selectEventTrigger == false
            && mMouseDownDragEvent == false )
            {
                if( Event.current.isMouse
                &&  Event.current.type == EventType.MouseDown
                && ( rcDragMove.Contains( Event.current.mousePosition ) )
                && Event.current.button == MOUSE_BUTTON_RIGHT )
                {
                    info.DrawTrackList();
                }
            }
            
            if( clearSelection )
            {
                Undo.RecordObject( this, "Clear selection" );
                SetNewSelectedEv( null, -1 );

                mCanDragEvent = false;
            }

            //if( mSelectedEventList.Count > 0 )
            //{
            //    if( mNewSelectedEvent.EvTrack != mSelectedEventList[ 0 ].EvTrack )
            //    {
            //        Repaint();
            //        SceneView.RepaintAll();
            //    }
            //}
            
            // 
            GUI.backgroundColor = Color.red;
            GUI.Box( new Rect( -info.ScrollPosTimeline.x + Time2Pos( info.Time ), 0, 2, rc.height ), "", mFillStyle );
            
            // 
            GUI.backgroundColor = Color.white;
            
            //  .
            if( mSlidebarFlag )
            {
                if( mMouseDragMovePos.drag_flag && idx == mMouseDragMovePos.time_line_idx && Event.current.type == EventType.MouseDrag )
                {
                    mSlidebarPositionX -= mMouseDragMovePos.offset_pos_x / 1.5f;
                    Repaint();
                }
                
                info.ScrollPosTimeline.x = GUI.HorizontalScrollbar(
                    new Rect( 0, rc.height - GUI.skin.horizontalScrollbar.fixedHeight, rc.width, GUI.skin.horizontalScrollbar.fixedHeight ),
                    mSlidebarPositionX,
                    Time2Pos( info.AnmLength - OFFSET_TIMELINE_LENGTH / 2f ) * 0.2f,
                    0,
                    Time2Pos( info.AnmLength - OFFSET_TIMELINE_LENGTH / 2f ) );
                
                if( Mathf.Abs( info.ScrollPosTimeline.x - mSlidebarPositionX ) >= 0.001f )
                {
                    mSlidebarPositionX = info.ScrollPosTimeline.x;
                }
            }
            else
            {
                if( mMouseDragMovePos.drag_flag && idx == mMouseDragMovePos.time_line_idx && Event.current.type == EventType.MouseDrag )
                {
                    info.ScrollPosTimeline.x -= mMouseDragMovePos.offset_pos_x / 2f;
                    Repaint();
                }

                info.ScrollPosTimeline.x = GUI.HorizontalScrollbar(
                    new Rect( 0, rc.height - GUI.skin.horizontalScrollbar.fixedHeight, rc.width, GUI.skin.horizontalScrollbar.fixedHeight ),
                    info.ScrollPosTimeline.x,
                    animLength * 0.2f,
                    0,
                    animLength );
            }
            
            GUI.EndGroup();
        }
        
        private bool DrawTimelineToolbar( Rect rc, int idx )
        {
            bool isDelete = false;
            
            TimelineInfo info = mTimelineInfo[ idx ];
            
            GUILayout.BeginArea( rc, GUIStyle.none );
            GUILayout.BeginHorizontal();
            {
                info.DrawNewEventComboBox();
                
                // 
                if( GUILayout.Button( "CreateNewEventParam", GUILayout.Width( 140 ) ) )
                {
                    info.CreateNewEventParam();
                }
                
                GUILayout.Label( "Time:" + info.DisplayTime, GUILayout.Width( 120 ) );
                
                // 
                if( info.EventParam != null )
                {
                    GUI.backgroundColor = Color.red;
                    if( GUILayout.Button("删除选中的Track", GUILayout.Width( 120 ) ) && mSelectedEventList[ 0 ] != null )
                    {
                        bool isRemove = info.RemoveTrack( mSelectedEventList, idx );
                        
                        mLookEventParam = info.EventParam;
                        
                        SetNewSelectedEv( null, -1 );
                        
                        if( isRemove )
                        {
                            mCanDragEvent = false;
                            mDragOffset = 0;
                            mPositionChanged = false;
                            mMouseDownDragEvent = false;
                            
                            mSelectEventTriggerState = SelectEventTriggerState.None;
                            ClearSelectedEv();
                        }
                        
                       info.ResetEventParam( mLookEventParam );
                    }
                    GUI.backgroundColor = Color.white;
                }
                
                GUILayout.FlexibleSpace();
            }
            GUILayout.EndHorizontal();
            
            GUILayout.BeginHorizontal();
            {
                if( idx <= mTimelineInfo.Count - 1 )
                {
                    info.DrawGUIEventParam();
                }
                
                if( ( idx == 0 && info.EventParam != null ) || idx >= 1 )
                {
                    if( GUILayout.Button("清除", GUILayout.Width( 50 ) ) )
                    {
                        DeleteTimelineInfo( idx );
                        
                        SetNewSelectedEv( null, -1 );
                        
                        Repaint();
                        SceneView.RepaintAll();
                        
                        isDelete = true;
                    }
                }
            }
            GUILayout.EndHorizontal();
            GUILayout.EndArea();
            
            return isDelete;
        }
    }
}
