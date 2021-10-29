using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace EG
{

    public partial class EditorWindow_Event : EditorWindow
    {

        #region 定数
        
        const string    PREVIEW_OBJECT_NAME             = "EventEditorPreview";
        const string    PREVIEW_ACTIVEMANAGER_OBJECT    = "ActiveManagerObject";
        
        const string    EVENT_PARM_PLAY_MODE_PATH_INFO  = "EventParamPlayModePathInfo.txt";
        const string    AEE_PLAY_MODE_PATH_INFO         = "AEEPlayModePathInfo.txt";
        
        const string    EVENT_PARAM_HISTORY             = "EventParamHistory.txt";
        const string    PREFAB_HISTORY                  = "PrefabHistory.txt";
        
        const int       OFFSET_EVENT_PANE_HEIGHT        = 128;
        const int       OFFSET_TIMELINE_HEIGHT          = 48;
        const int       OFFSET_SCROLLBAR_WIDTH          = 15;
        const int       TIME_LINE_HEIGHT                = 20;
        
        const float     OFFSET_TIMELINE_LENGTH          = 5f;
        const float     TRACK_HEIGHT                    = 20f;
        const float     FPS_DEF                         = 30f;
        const float     TYPE_COLUMN_WIDTH               = 140f;   //100f;  // 項目表示幅調整(ticket/6650)
        const float     PROPERTY_PANE_WIDTH             = 300f;
        const float     TOOLBAR_HEIGHT                  = 28f;
        const float     SZING_KNOB_SIZE                 = 8f;
        const float     MAX_STRIDE                      = 32f;
        const float     MIN_STRIDE                      = 4f;
        
        readonly Color  LINE_COLOR_H                    = new Color( 0, 0, 0, 0.4f );
        readonly Color  LINE_COLOR_M                    = new Color( 0, 0, 0, 0.25f );
        readonly Color  LINE_COLOR_S                    = new Color( 0, 0, 0, 0.1f );
        
        #endregion 定数
        
        enum SelectEventTriggerState
        {
            None,           // 
            DecideRange,    //  
            SelectEvent,    //  
        }
        
        enum DragTargets
        {
            Position,
            StartTime,
            EndTime
        }
        

        class SelectedEvent
        {
            public int TimelineIdx;
            public EventTrack EvTrack;
            
            public SelectedEvent( ) : this( null, -1 )
            {
            }
            
            public SelectedEvent( EventTrack track, int tlIdx )
            {
                EvTrack = track;
                TimelineIdx = tlIdx;
            }
        }
        
        class MouseDragMovePos
        {
            public float prev_pos_x = 0f;
            public float offset_pos_x = 0f;
            public bool drag_flag = false;
            public int time_line_idx = -1;
        }
        
        
        
        bool        mInitialized;
        
        Texture2D   mEventBGTex;
        GUIStyle    mEventStyle;
        GUIStyle    mSelectEventStyle;
        GUIStyle    mSizingKnobStyleL;
        GUIStyle    mSizingKnobStyleR;
        GUIStyle    mFillStyle;
        GUIStyle    mSelectRangeStyle;
        
        DragTargets mDragTarget;
        float       mDragOffset;
        bool        mCanDragEvent;
        bool        mPositionChanged;
        bool        mMouseDownDragEvent;
        
        static float mStride = 8.0f;
        
        List<EventClassInfo> mBaseEventClasses = new List<EventClassInfo>();
        
        bool        mUpddatePreviewObject;
        
        List<TimelineInfo> mTimelineInfo = new List<TimelineInfo>();
        
        List<SelectedEvent> mSelectedEventList = new List<SelectedEvent>();
        
        EventParam mLookEventParam = null;
        
        SerializedObject mSerializedEvent;
        
        float mTimelineScrollPosY;
        
        SelectEventTriggerState mSelectEventTriggerState;
        Rect mRectSelectEventTrigger;
        
        bool mSlidebarFlag = true;
        float mSlidebarPositionX = 0f;
        
        MouseDragMovePos mMouseDragMovePos = new MouseDragMovePos();
        
        
        Vector2 mPropertyScrollPos;
        
        UseHistory      mPrefabHistory  = new UseHistory( PREFAB_HISTORY, 8 );
        UseHistory      mEvParamHistory = new UseHistory( EVENT_PARAM_HISTORY, 8 );
        

        bool                                mDockOnce               = false;
        UnityEditor.SceneView               mAdditionSceneView      = null;
        EditorWindow_PreviewController      mAnmCtrlWindow          = null;
        EditorWindow_FindEventPlayer        mFindEvPlayer           = null;
        bool                                mIsPlaying              = false;
        bool                                m_IsForceLoop           = true;
        System.DateTime                     mLastUpdateTime;
        float                               mSpeedRate              = 1.0f;
        
        PreviewUnit m_PreviewUnit      = null;
        private GameObject m_PreviewUnitObject = null;

        GameObject                          m_LightObject           = null;
        
        GameObject                          m_RootObject            = null;
        Vector2                             m_ScrollPosPreviewCtrl  = Vector2.zero;
        bool                                m_IsPlayMode            = false;
        

        Camera                              m_CameraObject          = null;
        

        static bool                         s_IsRepaint             = false;
        
        
        
        public bool     IsPlaying   { get { return mIsPlaying;      } }
        public bool     IsPlayMode  { get { return m_IsPlayMode;    } }
        public bool     IsForceLoop { get { return m_IsForceLoop;   } }
        

        
        
        [MenuItem("EG/Tools/event",false,102)]
        public static EditorWindow_Event AllocateWindow( )
        {
            EditorWindow_Event window = GetWindow<EditorWindow_Event>( );
            window.SetTitle( "Event" );
            return window;
        }
        

        public static EditorWindow_Event Init( EventParam event_param )
        {
            EditorWindow_Event window = AllocateWindow( );
            window.ClearAllTimeline();
            window.AddEventParam( event_param );
            return window;
        }
        
        public static EditorWindow_Event Init( EventPlayerMachine machine, EventParam evParam )
        {
            if( machine == null )
            {
                return Init( evParam );
            }
            
            Selection.activeGameObject = machine.gameObject;
            EditorWindow_Event window = AllocateWindow( );
            window.ClearAllTimeline();
            window.SetPreviewFromSelection( machine.Player, evParam );
            return window;
        }
        

        
        private void Initialize( )
        {
            if( Application.isPlaying == false )
            {
                //todo 非运行模式下的声音播放
                // if (!FmodManager.Instance.IsInitialized())
                // {
                //     FmodManager.Instance.EditorInitialize();
                // }
            }

            m_RootObject = new GameObject( "EventEditorRoot" );
            SetObjectNotEditable( m_RootObject, true );
            
            // mAdditionSceneView = SceneView.CreateInstance( "UnityEditor.SceneView" ) as SceneView;
            // mAdditionSceneView.Show();
            
            mAnmCtrlWindow = GetWindow<EditorWindow_PreviewController>();
            mAnmCtrlWindow.Setup( DrawPreviewController );
            mAnmCtrlWindow.Show();
            
            CreateDataPathLog( );
            
            mEvParamHistory.Initialize();
            mPrefabHistory.Initialize();
            

            autoRepaintOnSceneChange = true;
            
            mEventBGTex = EditorGUIUtility.Load( "eventbg.png" ) as Texture2D;
            
            CreateGUIStyle();
            

            RefreshEventClasses( );
            
            EditorApplication.playModeStateChanged += OnChangePlayModeState;   
            EditorApplication.update += OnUpdate;                           
            SceneView.onSceneGUIDelegate += OnSceneGUI;
            
            //SetPlayModeData( );
            
            if( mTimelineInfo.Count == 0 )
            {
                TimelineInfo timeline_info = new TimelineInfo( this, mBaseEventClasses );
                mTimelineInfo.Add( timeline_info );
                mLookEventParam = timeline_info.EventParam;
            }
            
            mSelectEventTriggerState = SelectEventTriggerState.None;
            
            CreateCamera();
            
            m_IsPlayMode = false;
            mInitialized = true;
        }
        

        private void Release( )
        {
            mSerializedEvent = null;
            
            DestroyCamera();

            EditorApplication.playModeStateChanged -= OnChangePlayModeState;
            EditorApplication.update -= OnUpdate;
            SceneView.onSceneGUIDelegate -= OnSceneGUI;
            SceneView.RepaintAll( );
            
            if( mTimelineInfo.Count > 0 )
            {
                for( int i = 0, max = mTimelineInfo.Count; i < max; ++i )
                {
                    mTimelineInfo[i].Release();
                }
            }
            
            if( EditorApplication.isPlayingOrWillChangePlaymode == Application.isPlaying )
            {
                mEvParamHistory.Release();
                mPrefabHistory.Release();
                ClearPlayModePathInfo( );
            }
            else
            {
                MemoryPlayModePathInfo( );
            }
            
            ReleaseEditorWindow();
            
            if( m_PreviewUnit != null )
            {
                m_PreviewUnit.Release();
                m_PreviewUnit = null;
            }
            
            if( m_RootObject != null )
            {
                DestroyImmediate( m_RootObject );
                m_RootObject = null;
            }
            
            if( FmodManager.HasInstance() )
            {
                DestroyImmediate( FmodManager.Instance.gameObject );
                
                GameObject singletonDir = GameObject.FindWithTag( "SINGLETON_DIR" );
                if( singletonDir != null )
                {
                    DestroyImmediate( singletonDir );
                }
            }
            
            AssetDatabase.SaveAssets( );
        }
        
        

        private void Draw( )
        {
            Undo.RecordObject( this, "Undo" );
            
            if( mSerializedEvent != null )
            {
                mSerializedEvent.Update();
            }
            
            int last_idx = mTimelineInfo.Count - 1;
            if( CheckAddEventParam( mTimelineInfo[ last_idx ].EventParam ) )
            {

            }
            else if( mTimelineInfo[ last_idx ].EventParam != null )
            {
                mTimelineInfo[ last_idx ].SetEventParam( null );
            }
            
            float timeline_bottom_value = CalcTimeLineBottomY();
            
            // GUI
            EditorGUILayout.BeginHorizontal( );
            {
                if( m_IsPlayMode )
                {
                    DrawToolbarOnPlaying( new Rect( 0, 0, position.width - PROPERTY_PANE_WIDTH, TOOLBAR_HEIGHT ) );
                }
                else
                {
                    DrawToolbar( new Rect( 0, 0, position.width - PROPERTY_PANE_WIDTH, TOOLBAR_HEIGHT ) );
                }
            }
            EditorGUILayout.EndHorizontal( );
            
 
            DrawPropertyPane();
            
            EditorGUILayout.BeginHorizontal( );
            {
                GUILayout.BeginArea( new Rect( 0, TOOLBAR_HEIGHT, position.width - PROPERTY_PANE_WIDTH, position.height - TOOLBAR_HEIGHT ) );
                {
                    for( int idx = 0; idx < mTimelineInfo.Count; idx++ )
                    {
                        DrawEventPane( idx );
                    }
                    
                    UpdateRectSelect();
                    
                    GUI.backgroundColor = Color.white;
                    mTimelineScrollPosY = GUI.VerticalScrollbar(
                        new Rect( position.width - PROPERTY_PANE_WIDTH - OFFSET_SCROLLBAR_WIDTH, 0, 16, position.height - TOOLBAR_HEIGHT ),
                        mTimelineScrollPosY,
                        24f,
                        0f,
                        timeline_bottom_value );
                }
                GUILayout.EndArea( );
            }
            EditorGUILayout.EndHorizontal( );
            
            _InputEditorRoot();
            
            if( mSerializedEvent != null )
            {
                mSerializedEvent.ApplyModifiedProperties( );
            }
        }
        

        private void DrawToolbar( Rect rc )
        {
            GUILayout.BeginArea( rc, GUIStyle.none );
            {
                GUILayout.BeginHorizontal( );
                {
                    GUI.backgroundColor = Color.white;
                    
                    GUI.enabled = false;
                    mSlidebarFlag = GUILayout.Toggle( mSlidebarFlag, "幻灯片栏同步", GUILayout.Width( 160 ) );
                    GUI.enabled = true;
                    
                    if( GUILayout.Button("添加空时间线", GUILayout.Width( 160 ) ) )
                    {
                        if( mTimelineInfo[ 0 ].EventParam != null )
                        {
                            bool isEmpty = false;
                            foreach( TimelineInfo info in mTimelineInfo )
                            {
                                if( info.EventParam == null )
                                {
                                    isEmpty = true;
                                    break;
                                }
                            }
                            if( isEmpty == false )
                            {
                                TimelineInfo add_timeline = new TimelineInfo( this, mBaseEventClasses );
                                mTimelineInfo.Add( add_timeline );
                            }
                        }
                    }
                    m_PreviewUnitObject = EditorGUILayout.ObjectField("charactor", m_PreviewUnitObject, typeof(GameObject)) as GameObject;
                    if( GUILayout.Button("角色选择", GUILayout.Width( 120 ) ) ) {
                        var previewUnit = new PreviewUnit();
                        previewUnit.Setup(m_PreviewUnitObject);
                        CallbackCharaList(previewUnit);
                    }
                }
                GUILayout.EndHorizontal( );
            }
            GUILayout.EndArea( );
        }
        

        private void DrawPreviewController()
        {
            m_ScrollPosPreviewCtrl = GUILayout.BeginScrollView( m_ScrollPosPreviewCtrl );
            {
                GUILayout.BeginHorizontal( );
                {
                    int idx = 0;
                    TimelineInfo info = mTimelineInfo[idx];
                    bool isBtnEnable = false;
                    if( info != null && info.PreviewObject != null && info.EventParam != null )
                    {
                        isBtnEnable = true;
                    }
                    
                    GUI.enabled = isBtnEnable;
                    
                    if( mIsPlaying && isBtnEnable == false )
                    {
                        mIsPlaying = false;
                    }
                    string txt = ( mIsPlaying ) ? "停止" : "播放";
                    GUI.backgroundColor = Color.green;
                    if( GUILayout.Button( txt, GUILayout.Width( 120 ) ) )
                    {
                        mIsPlaying = !mIsPlaying;
                    }
                    GUI.backgroundColor = Color.white;
                    GUI.enabled = true;
                    
                    txt = ( m_IsForceLoop ) ? "循环" : "不循环";
                    if( GUILayout.Button( txt, GUILayout.Width( 120 ) ) )
                    {
                        m_IsForceLoop = !m_IsForceLoop;
                    }
                    
                    if( GUILayout.Button( "回到开始", GUILayout.Width( 120 ) ) )
                    {
                        for( int i = 0, max = mTimelineInfo.Count; i < max; ++i )
                        {
                            mTimelineInfo[i].SeekTime( 0 );
                        }
                        mUpddatePreviewObject = true;
                    }
                }
                GUILayout.EndHorizontal();
                
                if( m_PreviewUnit != null )
                {
                    m_PreviewUnit.DrawGUIAnmController( ref mSpeedRate );
                }
            }
            GUILayout.EndScrollView();
        }
        
        void NotifyEndStop()
        {
            mIsPlaying = false;
        }
        

        private void DrawPropertyPane( )
        {
            GUILayout.BeginArea( new Rect( position.width - PROPERTY_PANE_WIDTH, 0, PROPERTY_PANE_WIDTH, position.height ) );
            {
                EditorGUIUtility.labelWidth = PROPERTY_PANE_WIDTH * 0.35f;
                
                GUI.backgroundColor = Color.white;
                
                if( mSelectedEventList.Count == 1 )
                {
                    //GUI.enabled = EditorApplication.isPlaying == false;
                    _DrawGUITrackProperty();
                    //GUI.enabled = true;
                }
                else
                {
                    GUILayout.Label( "操作方法" );
                    GUILayout.Label("Movement/long adjustment: Mouse right");
                    GUILayout.Label("缩放：按住Alt键并按住鼠标滚轮");
                }
            }
            GUILayout.EndArea( );
        }
        
        void _DrawGUITrackProperty()
        {
            mPropertyScrollPos = GUILayout.BeginScrollView( mPropertyScrollPos, false, true,
                                            GUIStyle.none,
                                            GUI.skin.verticalScrollbar,
                                            GUI.skin.box,
                                            GUILayout.Width( PROPERTY_PANE_WIDTH ) );
            {
                if( mSelectedEventList[ 0 ].EvTrack != null && mSerializedEvent != null )
                {
                    int timeline_index = mSelectedEventList[ 0 ].TimelineIdx;
                    if( timeline_index >= 0 && timeline_index < mTimelineInfo.Count )
                    {
                        EventPlayer.CurrentEditorPlayer = mTimelineInfo[ timeline_index ].m_EventPlayer;
                    }
                    else
                    {
                        EventPlayer.CurrentEditorPlayer = null;
                    }
                    
                    if( mSelectedEventList[ 0 ].EvTrack.isCustomInspector == false )
                    {
                        SerializedProperty prop = mSerializedEvent.GetIterator();
                        if( prop.NextVisible( true ) )
                        {
                            do
                            {
                                if( prop.name == "m_Script" )
                                    continue;
                                EditorGUILayout.BeginHorizontal();
                                try
                                {
                                    EditorGUILayout.PropertyField( prop, new GUIContent( prop.displayName ), true );
                                }
                                catch( System.Exception e )
                                {
                                    Debug.LogError( e.Message );
                                }
                                EditorGUILayout.EndHorizontal();
                            }
                            while( prop.NextVisible( false ) );
                        }
                    }
                    else
                    {
                        mSelectedEventList[ 0 ].EvTrack.OnInspectorGUI( this.position, mSerializedEvent, PROPERTY_PANE_WIDTH );
                    }
                }
                EventPlayer.CurrentEditorPlayer = null;
            }
            GUILayout.EndScrollView();
        }
        
        
        private void _SetEventParam( TimelineInfo info, EventParam evParam,bool force=false )
        {
            if( info == null || evParam == null )
                return;
            if (force) {
                if (evParam.Events == null)
                {
                    evParam.Events = new EventTrack[0];
                }
                info.SetEventParam( evParam );
                    
                mEvParamHistory.Add( evParam );
                SortEventParamEvents( evParam );
                    
                SetNewSelectedEv( null, -1 );
                return;
            }
            
            if( evParam != info.EventParam && evParam != null )
            {
                if( CheckEntrySameEventParam( evParam ) == false )
                {
                    if (evParam.Events == null)
                    {
                        evParam.Events = new EventTrack[0];
                    }
                    info.SetEventParam( evParam );
                    
                    mEvParamHistory.Add( evParam );
                    SortEventParamEvents( evParam );
                    
                    SetNewSelectedEv( null, -1 );
                }
            }
        }
        

        public void StopPlaying()
        {
            mIsPlaying = false;
        }
        

        EventTrack GetSelectedEvTrack()
        {
            if( mSelectedEventList.Count == 0 )
                return null;
            
            return mSelectedEventList[0].EvTrack;
        }
        


        bool CheckSelectedEvTrack( System.Type trackType = null )
        {
            if( mSelectedEventList.Count == 0 )
                return false;
            
            EventTrack evTrack = mSelectedEventList[0].EvTrack;
            if( evTrack == null )
                return false;
            
            if( trackType == null )
                return true;
            
            return evTrack.GetType() == trackType;
        }
        

        private bool CheckEntrySameEventParam( EventParam event_param )
        {
            if( event_param != null )
            {
                foreach( TimelineInfo info in mTimelineInfo )
                {
                    if( info.EventParam != null && info.EventParam == event_param )
                    {
                        return true;
                    }
                }
            }
            return false;
        }
        
        
        static private void SetObjectNotEditable( GameObject go, bool not_editable )
        {
            if( go == null )
            {
                return;
            }
            
            if( not_editable )
            {
                go.hideFlags = HideFlags.DontSave | HideFlags.NotEditable;
            }
            else
            {
                go.hideFlags = HideFlags.DontSave;
            }
            
            foreach( Transform child in go.GetComponentsInChildren<Transform>(true) )
            {
                if( child.gameObject == go )
                {
                    continue;
                }
                
                child.gameObject.hideFlags |= HideFlags.DontSave | HideFlags.NotEditable;// | HideFlags.HideInHierarchy | HideFlags.HideInInspector;
            }
        }
        

        public void AddChildToRootObject( Transform trans )
        {
            if( trans == null )
                return;
            
            if( m_RootObject != null )
            {
                trans.SetParent( m_RootObject.transform, false );
            }
        }
        
        
        
        void ReleaseEditorWindow()
        {
            if( mAdditionSceneView != null )
            {
                mAdditionSceneView.Close();
                mAdditionSceneView = null;
            }
            
            if( mAnmCtrlWindow != null )
            {
                mAnmCtrlWindow.Close();
                mAnmCtrlWindow = null;
            }
            
            
            if( mFindEvPlayer != null )
            {
                mFindEvPlayer.Close();
                mFindEvPlayer = null;
            }
            
        }



        private void RepaintAll( )
        {
            Repaint();
            UnityEditorInternal.InternalEditorUtility.RepaintAllViews();
        }
        

        private void OnChangePlayModeState( PlayModeStateChange obj )
        {

            ReleaseEditorWindow();
            
            mAdditionSceneView = SceneView.CreateInstance( "UnityEditor.SceneView" ) as SceneView;
            mAdditionSceneView.Show();
            
            mAnmCtrlWindow = GetWindow<EditorWindow_PreviewController>();
            mAnmCtrlWindow.Setup( DrawPreviewController );
            mAnmCtrlWindow.Show();
            
            mDockOnce = false;
            
        }


        private void OnEnable( )
        {
            if( EditorApplication.isPlaying )
            {
                InitializeOnPlaying();
            }
            else
            {
                Initialize();
            }
        }
        

        private void OnDisable( )
        {
            if( EditorApplication.isPlaying )
            {
                ReleaseOnPlaying();
            }
            else
            {
                Release();
            }
        }
        

        private void OnUpdate( )
        {
            //todo  SoundManager.Instance.UpdateManual
            // if( SoundManager.HasInstance() )
            // {
            //     SoundManager.Instance.UpdateManual();
            // }
            //
            
            System.DateTime currentTime = System.DateTime.Now;
            float dt = 0.033333f;
            
            if( mIsPlaying )
            {
                dt = (float)(currentTime - mLastUpdateTime).TotalSeconds * mSpeedRate;
                
                for( int idx = 0; idx < mTimelineInfo.Count; idx++ )
                {
                    mTimelineInfo[idx].ElapseTime( dt );
                }
            }
            
            if( mUpddatePreviewObject || mIsPlaying )
            {
                for( int idx = 0; idx < mTimelineInfo.Count; idx++ )
                {
                    mTimelineInfo[ idx ].UpdatePreviewObject();
                }
                
                mUpddatePreviewObject = false;
            }
            
            if( m_PreviewUnit != null )
            {
                m_PreviewUnit.Update( dt );
            }
            
            mLastUpdateTime = currentTime;
            //mLastUpdateTime.AddSeconds(dt);
            
            if( mIsPlaying )
            {
                RepaintAll();
            }
            
            //todo SoundManager.Instance.LateUpdateManual
            // if( SoundManager.HasInstance() )
            // {
            //     SoundManager.Instance.LateUpdateManual();
            // }
        }
        

        private void OnGUI( )
        {
            if( mInitialized == false )
            {
                return;
            }
            
            Draw( );
            
            
            if( s_IsRepaint )
            {
                Repaint();
                s_IsRepaint = false;
            }
        }
        

        private void OnSceneGUI( SceneView sceneView )
        {
            
            if( GUI.changed && mLookEventParam != null )
            {
                EditorUtility.SetDirty( mLookEventParam );
            }
            
            if( EditorApplication.isPlaying == false )
            {
                if( mTimelineInfo.Count > 0 && mTimelineInfo[0] != null )
                {
                    Handles.BeginGUI();
                    {
                        GUI.color = new Color( 0, 0, 0, 0.5f );
                        GUI.Box( new Rect( 3, 3, 184, 30 ), "", new GUIStyle( "Dopesheetkeyframe" ) );
                        GUI.color = Color.white;
                        GUI.Label( new Rect( 5, 5, 180, 30 ), "時間：" + mTimelineInfo[ 0 ].DisplayTime );
                    }
                    Handles.EndGUI();
                }
            }
        }


        void OnSelectionChange()
        {
            if( Selection.activeGameObject == null )
                return;
            
            if( AssetDatabase.Contains( Selection.activeGameObject ) )
                return;
            
            EventPlayer evPlayer = Selection.activeGameObject.GetComponent<EventPlayer>();
            if( evPlayer == null )
                return;
            
            for( int i = 0, max = mTimelineInfo.Count; i < max; ++i )
            {
                if( mTimelineInfo[i].IsDuplicatedEventPlayer( evPlayer ) )
                {
                    return;
                }
            }
            
            SetPreviewFromSelection( evPlayer );
        }
        
        
        public void SetNewSelectedEv( EventTrack track, int tlIdx )
        {
            ClearSelectedEv();
            
            if( track != null && tlIdx != -1 )
            {
                mSelectedEventList.Add( new SelectedEvent( track, tlIdx ) );
                
                 mSerializedEvent = new SerializedObject( track );
            }
            
            mCanDragEvent = false;
            mPositionChanged = false;
            
            s_IsRepaint = true;
        }
        

        public bool CheckSameSelectEventTrack( EventTrack track )
        {
            if( mSelectedEventList.Count > 0 )
            {
                for( int i = 0, max = mSelectedEventList.Count; i < max; ++i )
                {
                    if( mSelectedEventList[i] != null && mSelectedEventList[i].EvTrack == track )
                    {
                        return true;
                    }
                }
            }
            
            return false;
        }
        

        public bool CheckMoveSelectEventTrack( EventTrack track )
        {
            if( mSelectedEventList.Count <= 1 )
                return true;
            
            for( int i = 0, max = mSelectedEventList.Count; i < max; ++i )
            {
                if( mSelectedEventList[i] != null && mSelectedEventList[i].EvTrack == track )
                {
                    return false;
                }
            }
            
            return true;
        }
        

        public void ClearSelectedEv()
        {
            mSelectedEventList.Clear();
            mSerializedEvent = null;
        }

 

        private void SetSoundVoice2Layout( )
        {
        }
        
        private void SearchAndAddEventParam( EventParam event_param, int set_idx, bool setting_preview = true )
        {
            
        }
        
        private void ClearAllTimeline()
        {
            if( mTimelineInfo.Count == 0 )
                return;
            
            SetNewSelectedEv( null, -1 );
            
            while( mTimelineInfo.Count > 0 )
            {
                if( DeleteTimelineInfo( 0 ) )
                {
                    break;
                }
            }
        }
        
        private void AddEventParam( EventParam event_param )
        {
            if( event_param == null || CheckEntrySameEventParam( event_param ) )
            {
                return;
            }
            
            if( mTimelineInfo.Count == 1 && mTimelineInfo[ 0 ].EventParam == null )
            {
                mTimelineInfo[ 0 ].SetEventParam( event_param );
            }
            else
            {
                foreach( TimelineInfo info in mTimelineInfo )
                {
                    if( info.EventParam == null )
                    {
                        info.SetEventParam( event_param );
                        return; // 
                    }
                }
                
                {
                    TimelineInfo add_timeline = new TimelineInfo( this, mBaseEventClasses );
                    add_timeline.SetEventParam( event_param );
                    mTimelineInfo.Add( add_timeline );
                }
            }
        }
        
        private void DeleteChild( int idx )
        {
            if( idx + 1 < mTimelineInfo.Count )
            {
                EventParam parent = mTimelineInfo[ idx ].EventParam;

                for( int tl_idx = idx + 1; tl_idx < mTimelineInfo.Count; tl_idx++ )
                {
                    if( parent == mTimelineInfo[ tl_idx ].event_player_parent )
                    {
                        DeleteTimelineInfo( tl_idx );
                        tl_idx--;
                    }
                }
            }
        }

        private bool DeleteTimelineInfo( int idx )
        {
            DeleteChild( idx );
            
            if( idx < mTimelineInfo.Count )
            {
                mTimelineInfo[idx].Release();
                mTimelineInfo.Remove( mTimelineInfo[ idx ] );
            }

            if( mTimelineInfo.Count == 0 )
            {
                TimelineInfo add_info = new TimelineInfo( this, mBaseEventClasses );
                mTimelineInfo.Add( add_info );
                return true;
            }
            return false;
        }

        private void SetPreviewObject( int idx )
        {
            
        }
        
        private void MemoryPlayModePathInfo()
        {
            if (mTimelineInfo == null || mTimelineInfo.Count == 0 || mTimelineInfo[0].EventParam == null)
            {
                return;
            }

            StreamWriter animdef_writer = new StreamWriter(GetTempPath() + EVENT_PARM_PLAY_MODE_PATH_INFO, false);
            StreamWriter aee_writer = new StreamWriter(GetTempPath() + AEE_PLAY_MODE_PATH_INFO, false);

            string animdef_path = "";
            string prefab_path = "";
            foreach (TimelineInfo info in mTimelineInfo)
            {
                //  
                //if (info.add_update)
                //    continue;

                animdef_path = AssetDatabase.GetAssetPath(info.EventParam );
                //prefab_path = AssetDatabase.GetAssetPath(info.preview_prefab);
                if (string.IsNullOrEmpty(animdef_path) || string.IsNullOrEmpty(prefab_path))
                    continue;

                animdef_writer.WriteLine(animdef_path);
                aee_writer.WriteLine(prefab_path);

                animdef_writer.Flush();
                aee_writer.Flush();
            }

            animdef_writer.Close();
            aee_writer.Close();
        }

        //  
        private void ClearPlayModePathInfo()
        {
            FileInfo anim_fi = new FileInfo(GetTempPath() + EVENT_PARM_PLAY_MODE_PATH_INFO);
            if (!anim_fi.Exists)
                return;
            using (StreamWriter animdef_writer = new StreamWriter(GetTempPath() + EVENT_PARM_PLAY_MODE_PATH_INFO, false))
            {
                animdef_writer.WriteLine("");
            }

            FileInfo aee_fi = new FileInfo(GetTempPath() + AEE_PLAY_MODE_PATH_INFO);
            if (!aee_fi.Exists)
                return;
            using (StreamWriter aee_writer = new StreamWriter(GetTempPath() + AEE_PLAY_MODE_PATH_INFO, false))
            {
                aee_writer.WriteLine("");
            }
        }

        //  
        private void SetPlayModeData()
        {
            List<string> animdef_list = new List<string>();
            List<string> prefab_list = new List<string>();

            string line;
            using (StreamReader reader = new StreamReader(GetTempPath() + EVENT_PARM_PLAY_MODE_PATH_INFO))
            {
                while ((line = reader.ReadLine()) != null)
                {
                    if (line == "")
                        continue;
                    animdef_list.Add(line);
                }
            }
            using (StreamReader reader = new StreamReader(GetTempPath() + AEE_PLAY_MODE_PATH_INFO))
            {
                while ((line = reader.ReadLine()) != null)
                {
                    if (line == "")
                        continue;
                    prefab_list.Add(line);
                }
            }

            //  
            if (animdef_list.Count != prefab_list.Count)
            {
                Debug.LogWarning("MisMatch animdef_list and prefab_list");
                return;
            }

            //  
            if (animdef_list.Count == 0)
            {
                return;
            }

            for (int idx = 0; idx < animdef_list.Count; idx++)
            {
                TimelineInfo add_info = new TimelineInfo( this, mBaseEventClasses );
                mTimelineInfo.Add(add_info);

                int timeline_idx = mTimelineInfo.Count - 1;

                mTimelineInfo[timeline_idx].SetEventParam( AssetDatabase.LoadAssetAtPath(animdef_list[idx], typeof(EventParam)) as EventParam );

                //  
                //mTimelineInfo[timeline_idx].set_preview_flag = true;
                SearchAndAddEventParam(mTimelineInfo[timeline_idx].EventParam, timeline_idx + 1, false);
            }

            // 
            //mPlayingFlag = true;
        }
    }
    
}
