using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections;
using System.Collections.Generic;

namespace EG
{
    public partial class EditorWindow_Event : EditorWindow
    {
        
        const int       MOUSE_BUTTON_LEFT       = 0;
        const int       MOUSE_BUTTON_RIGHT      = 1;
        const int       MOUSE_BUTTON_MIDDLE     = 2;
        
        
        static private string GetTempPath()
        {
            return EditorHelp.GetTempPath( true ) + "/EventEditor/";
        }
        
        struct EventClassInfo
        {
            public System.Type type;
            public string name;
            public string listName;
            public EventTrack.ECategory category;
            
            public EventClassInfo( string _name )
            {
                type = null;
                name = _name;
                listName = _name;
                category = EventTrack.ECategory.None;
            }
            
            public EventClassInfo( System.Type _type )
            {
                type = _type;
                System.Reflection.PropertyInfo propInfo = _type.GetProperty( "ClassName", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.GetProperty );
                if( propInfo != null )
                {
                    name = ( string )propInfo.GetValue( null, null );
                }
                else
                {
                    name = _type.Name;
                }
                
                var track = ScriptableObject.CreateInstance( _type ) as EventTrack;
                listName = EventTrack.GetCategoryText( track ) + "/" + name;
                
                category = EventTrack.ECategory.None;
                if( track != null )
                {
                    category = track.Category;
                }
            }
        }
        
        private void RefreshEventClasses()
        {
            mBaseEventClasses.Clear();
            mBaseEventClasses.Add( new EventClassInfo( "[Create New Track]" ) );
            
            foreach( System.Reflection.Assembly assembly in System.AppDomain.CurrentDomain.GetAssemblies() )
            {
                if( assembly.FullName.StartsWith( "Mono.Cecil" ) ) continue;
                if( assembly.FullName.StartsWith( "UnityScript" ) ) continue;
                if( assembly.FullName.StartsWith( "Boo.Lan" ) ) continue;
                if( assembly.FullName.StartsWith( "System" ) ) continue;
                if( assembly.FullName.StartsWith( "I18N" ) ) continue;
                if( assembly.FullName.StartsWith( "UnityEngine" ) ) continue;
                if( assembly.FullName.StartsWith( "UnityEditor" ) ) continue;
                if( assembly.FullName.StartsWith( "mscorlib" ) ) continue;
                
                System.Type[] types = assembly.GetTypes();
                
                foreach( System.Type type in types )
                {
                    if( type.IsClass && !type.IsAbstract && type.IsSubclassOf( typeof( EventTrack ) ) )
                    {
                        mBaseEventClasses.Add( new EventClassInfo( type ) );
                    }
                }
            }
            
            if( mTimelineInfo != null && mTimelineInfo.Count > 0 )
            {
                for( int i = 0, max = mTimelineInfo.Count; i < max; ++i )
                {
                    mTimelineInfo[ i ].SetEventTrackClassInfo( mBaseEventClasses );
                }
            }
        }
        
        private string GetClassName( System.Type type )
        {
            for( int i = 1; i < mBaseEventClasses.Count; ++i )
            {
                if( mBaseEventClasses[ i ].type == type )
                {
                    return mBaseEventClasses[ i ].name;
                }
            }
            return "";
        }
        
        //todo lightsetting
        // void CreateLightObject()
        // {
        //     if( m_LightObject != null )
        //     {
        //         return;
        //     }
        //     
        //     m_LightObject = new GameObject( "evLight" );
        //     LightSetting lightSetting = m_LightObject.AddComponent<LightSetting>();
        //     GameObject lightObj = new GameObject( "Directional light" );
        //     {
        //         lightObj.transform.SetParent( m_LightObject.transform, false );
        //         Light light = lightObj.AddComponent<Light>();
        //         Transform lightTrans = light.transform;
        //         {
        //             light.type = LightType.Directional;
        //             lightTrans.position = Vector3.up * 100.0f;
        //             lightTrans.rotation = Quaternion.Euler( 0, 510, 0 );
        //         }
        //         lightSetting.LightDir = light;
        //     }
        //     AddChildToRootObject( m_LightObject.transform );
        // }
        //
        // void DestroyLightObject()
        // {
        //     if( m_LightObject != null )
        //     {
        //         GameObject.DestroyImmediate( m_LightObject );
        //         m_LightObject = null;
        //     }
        // }
        

        
        void CreateCamera()
        {
            if( m_CameraObject != null  )
                return;
            
            m_CameraObject = CreateCameraObject();
            AddChildToRootObject( m_CameraObject.transform );
        }
        
        static public Camera CreateCameraObject()
        {
            Camera cam = new GameObject( "evCamera" ).AddComponent<Camera>();
            {
                cam.gameObject.tag = "MainCamera";
                cam.cullingMask = ( 1 << LayerMask.NameToLayer( "BG" ) );
                cam.cullingMask |= ( 1 << LayerMask.NameToLayer( "CH" ) );
                cam.cullingMask |= ( 1 << LayerMask.NameToLayer( "EFFECT" ) );
                cam.cullingMask |= ( 1 << LayerMask.NameToLayer( "Default" ) );
                cam.fieldOfView = 30.9427f;
                cam.nearClipPlane = 0.1f;
                cam.allowHDR = false;
                cam.allowMSAA = false;
            }
            
            return cam;
        }
        
     
        void DestroyCamera()
        {
            if( m_CameraObject == null )
                return;
            
            DestroyImmediate( m_CameraObject.gameObject );
            m_CameraObject = null;
        }
        
        class UseHistory
        {
            List<string>        m_History       = new List<string>();
            string              m_FileName      = "history";
            int                 m_MaxNum        = 8;
            
            public UseHistory( string saveFileName, int maxNum )
            {
                m_FileName = saveFileName;
                m_MaxNum = maxNum;
                m_History.Clear();
            }
            
            public void Initialize()
            {
                Load();
            }
            
            public void Release()
            {
                m_History.Clear();
            }
            
            public void Add( Object data )
            {
                if( data == null )
                    return;
                
                string path = AssetDatabase.GetAssetPath( data );
                if( string.IsNullOrEmpty( path ) == false )
                {
                    m_History.Remove( path );
                    m_History.Insert( 0, path );    // 
                    if( m_History.Count > m_MaxNum )
                    {
                        m_History.RemoveAt( m_History.Count - 1 );
                    }
                }
                
                Save();
            }
            

            public void RemoveAll()
            {
                m_History.Clear();
                Save();
            }
            

            public T DrawGUI<T>() where T : class
            {
                List<string> dispHistory = new List<string>();
                dispHistory.Add( "" );
                for( int i = 0; i < m_History.Count; ++i )
                {
                    dispHistory.Add( System.IO.Path.GetFileName( m_History[ i ] ) );
                }
                
                int selectedHistoryItem = EditorGUILayout.Popup( 0, dispHistory.ToArray(), GUILayout.Width( 20 ) ) - 1;
                if( selectedHistoryItem >= 0 )
                {
                    return AssetDatabase.LoadAssetAtPath( m_History[ selectedHistoryItem ], typeof( T ) ) as T;
                }
                
                return null;
            }
            

            public void Load()
            {
                string path = GetTempPath() + m_FileName;
                
                FileInfo fi = new FileInfo( path );
                if( fi == null || fi.Exists == false )
                    return;
                
                StreamReader reader = new StreamReader( path );
                {
                    while( reader.Peek() > -1 )
                    {
                        string txt = reader.ReadLine();
                        if( string.IsNullOrEmpty( txt ) == false )
                        {
                            m_History.Add( txt );
                        }
                    }
                    
                    reader.Close();
                    reader.Dispose();
                }
            }
            
            public void Save()
            {
                string path = GetTempPath() + m_FileName;
                StreamWriter writer = null;
                
                // 
                FileInfo fi = new FileInfo( path );
                if( fi.Exists )
                {
                    writer = new StreamWriter( path );
                }
                else
                {
                    writer = File.CreateText( path );
                }
                
                if( m_History.Count == 0 )
                {
                    writer.WriteLine( "" );
                }
                else
                {
                    for( int i = 0; i < m_History.Count; ++i )
                    {
                        writer.WriteLine( m_History[ i ] );
                    }
                }
                
                writer.Flush();
                writer.Close();
                writer.Dispose();
            }
        }


        static private float SnapTime( float time )
        {
            float snapSize = 1.0f / FPS_DEF;
            return Mathf.Floor( time / snapSize ) * snapSize;
        }

        // 
        static private float SnapPos( float pos )
        {
            float snapSize = mStride;
            return Mathf.Floor( pos / snapSize ) * snapSize;
        }
        
        // 
        static private float Time2Pos( float t )
        {
            return t * FPS_DEF * mStride;
        }

        // 
        static private float Pos2Time( float t )
        {
            return t / ( FPS_DEF * mStride );
        }
        
        // 
        private void AdjustTime( ref float x1, ref float x2, float dragOfs )
        {
            float t;
            switch( mDragTarget )
            {
                case DragTargets.StartTime:
                    x1 = SnapTime( x1 + dragOfs );
                    x1 = Mathf.Min( x1, x2 );
                    break;
                case DragTargets.EndTime:
                    x2 = SnapTime( x2 + dragOfs );
                    x2 = Mathf.Max( x1, x2 );
                    break;
                case DragTargets.Position:
                default:
                    t = x1 + dragOfs;
                    t = SnapTime( t );
                    x2 += t - x1;
                    x1 = t;
                    break;
            }
        }
        
        
        void CreateGUIStyle()
        {
            mEventStyle = new GUIStyle();
            mEventStyle.normal.textColor = Color.black;
            mEventStyle.normal.background = mEventBGTex;
            mEventStyle.border = new RectOffset( 1, 1, 1, 1 );
            mEventStyle.padding = new RectOffset( 2, 2, 2, 2 );
            
            mSelectEventStyle = new GUIStyle();
            mSelectEventStyle.normal.textColor = new Color32( 255, 255, 32, 255 );
            mSelectEventStyle.normal.background = mEventBGTex;
            mSelectEventStyle.border = new RectOffset( 1, 1, 1, 1 );
            mSelectEventStyle.padding = new RectOffset( 2, 2, 2, 2 );
            
            mSizingKnobStyleL = new GUIStyle();
            mSizingKnobStyleL.normal.background = EditorGUIUtility.Load( "event_leftarrow.png" ) as Texture2D;
            
            mSizingKnobStyleR = new GUIStyle();
            mSizingKnobStyleR.normal.background = EditorGUIUtility.Load( "event_rightarrow.png" ) as Texture2D;
            
            mFillStyle = new GUIStyle();
            mFillStyle.normal.background = Texture2D.whiteTexture;
            
            mSelectRangeStyle = new GUIStyle();
            mSelectRangeStyle.normal.background = Texture2D.whiteTexture;
        }
        
        
        private void UpdateRectSelect()
        {
            //if( m_IsPlayMode )
            //{
            //    mSelectEventTriggerState = SelectEventTriggerState.None;
            //    return;
            //}
            
            switch( mSelectEventTriggerState )
            {
                case SelectEventTriggerState.None:
                    if( Event.current.type == EventType.MouseDown
                    && mCanDragEvent == false
                    && Event.current.button == MOUSE_BUTTON_LEFT )
                    {
                        if( CheckMousePosition() )
                        {
                            mRectSelectEventTrigger = new Rect();
                            mRectSelectEventTrigger.min = Event.current.mousePosition;
                            mRectSelectEventTrigger.size = Vector2.one;
                            mSelectEventTriggerState = SelectEventTriggerState.DecideRange;
                        }
                    }
                    break;
                case SelectEventTriggerState.DecideRange:
                    mRectSelectEventTrigger.max = Event.current.mousePosition;
                    if( Event.current.type == EventType.MouseUp || Event.current.type == EventType.Ignore )
                    {
                        if( AddSelectEvent() )
                        {
                            mSelectEventTriggerState = SelectEventTriggerState.SelectEvent;
                        }
                        else
                        {
                            mSelectEventTriggerState = SelectEventTriggerState.None;
                        }
                    }
                    else
                    {
                        GUI.backgroundColor = new Color( 0, 1, 0, 0.25f );
                        GUI.Box( mRectSelectEventTrigger, "", mSelectRangeStyle );
                        GUI.backgroundColor = Color.white;
                        Repaint();
                    }
                    break;
                case SelectEventTriggerState.SelectEvent:
                    if( ( Event.current.type == EventType.Ignore || Event.current.type == EventType.MouseDown || Event.current.type == EventType.MouseUp ) && !mCanDragEvent && !mMouseDownDragEvent )
                    {
                        mSelectEventTriggerState = SelectEventTriggerState.None;
                        ClearSelectedEv();
                    }
                    break;
                default:
                    //  .
                    Debug.LogWarning( " : " + mSelectEventTriggerState );
                    break;
            }
        }
        

        private bool CheckMousePosition()
        {
            if( Event.current.mousePosition.x < position.width - PROPERTY_PANE_WIDTH - OFFSET_SCROLLBAR_WIDTH )
            {
                return true;
            }
            
            return false;
        }
        

        private bool AddSelectEvent()
        {
            ClearSelectedEv();

            for( int idx = 0, max = mTimelineInfo.Count; idx < max; ++idx )
            {
                TimelineInfo info = mTimelineInfo[idx];
                if( info == null || info.EventParam == null )
                {
                    continue;
                }
                
                foreach( List<TimelineInfo.TrackList> track_list in info.Tracks )
                {
                    foreach( TimelineInfo.TrackList track in track_list )
                    {
                        if( track.event_track == null )
                        {
                            continue;
                        }
                        
                        bool clickedEvent = mRectSelectEventTrigger.Overlaps( track.event_rect, true );
                        if( clickedEvent )
                        {
                            Undo.RecordObject( this, "Selection Change" );
                            
                            SelectedEvent select_event = new SelectedEvent( track.event_track, idx );
                            mSelectedEventList.Add( select_event );
                        }
                    }
                }
            }
            
            if( mSelectedEventList.Count > 1 )
            {
                mCanDragEvent = true;
                mPositionChanged = false;
                return true;
            }
            
            return false;
        }
        
    
        private bool CheckAddEventParam( EventParam event_param )
        {
            if( event_param == null )
            {
                return false;
            }
            
            for( int idx = 0; idx < mTimelineInfo.Count - 1; idx++ )
            {
                //  .
                if( mTimelineInfo[ idx ].EventParam == event_param )
                {
                    return false;
                }
            }
            
            //  .
            return true;
        }
        

        
        float CalcTimeLineBottomY()
        {
            float val = 0f;
            for( int idx = 0; idx < mTimelineInfo.Count; idx++ )
            {
                TimelineInfo info = mTimelineInfo[idx];
                
                // 
                info.TrackTypes = new List<System.Type>( 32 );
                if( info.EventParam != null )
                {
                    if( info.EventParam.Events != null )
                    {
                        for( int i = 0; i < info.EventParam.Events.Length; ++i )
                        {
                            EventTrack e = info.EventParam.Events[ i ];
                            if( e == null )
                            {
                                continue;
                            }
                            
                            if( info.TrackTypes.Contains( e.GetType() ) )
                            {
                                continue;
                            }
                            
                            info.TrackTypes.Add( e.GetType() );
                        }
                    }
                }
                
                val += info.TimelineHeight + OFFSET_EVENT_PANE_HEIGHT;
            }
            
            return val;
        }
        

        static private void SortEventParamEvents( Object data )
        {
            if( data == null )
            {
                return;
            }
            
            //  
            EventParam anim_def = data as EventParam;
            if( anim_def == null )
            {
                return;
            }
            
            if( anim_def.Events != null )
            {
                System.Array.Sort( anim_def.Events, ( a, b ) => ComparedAnimLength( a, b ) );
            }
        }
        

        static private int ComparedAnimLength( EventTrack a, EventTrack b )
        {
            if( ( a.End - a.Start ) > ( b.End - b.Start ) )
            {
                return 1;
            }
            
            if( ( a.End - a.Start ) < ( b.End - b.Start ) )
            {
                return -1;
            }
            
            return 0;
        }
        
        //
        private void CreateDataPathLog()
        {
            if( !Directory.Exists( GetTempPath() ) )
            {
                Directory.CreateDirectory( GetTempPath() );
            }

            FileInfo fi = new FileInfo( GetTempPath() + EVENT_PARM_PLAY_MODE_PATH_INFO );
            if( !fi.Exists )
            {
                StreamWriter fs = File.CreateText( GetTempPath() + EVENT_PARM_PLAY_MODE_PATH_INFO );
                fs.Close();
            }
            fi = new FileInfo( GetTempPath() + AEE_PLAY_MODE_PATH_INFO );
            if( !fi.Exists )
            {
                StreamWriter fs = File.CreateText( GetTempPath() + AEE_PLAY_MODE_PATH_INFO );
                fs.Close();
            }
        }
        
        
        void DrawGUIVerticalLine( Rect rc, float scrollPosX )
        {
            float x = -scrollPosX % mStride;
            int tick = Mathf.FloorToInt( scrollPosX / mStride );
            while( x < rc.width )
            {
                float offset;
                if( tick % 10 == 0 )
                {
                    GUI.backgroundColor = LINE_COLOR_H;
                    offset = 8;
                }
                else if( tick % 5 == 0 )
                {
                    GUI.backgroundColor = LINE_COLOR_M;
                    offset = 12;
                }
                else
                {
                    GUI.backgroundColor = LINE_COLOR_S;
                    offset = 16;
                }
                
                if( 0 <= x && x < rc.width )
                {
                    GUI.Box( new Rect( x, offset, 1, rc.height - offset ), "", mFillStyle );
                }
                ++tick;
                x += mStride;
            }
        }
        
        class EditClipInfo
        {
            const int TARGET_DISPLAY_IDX        = 1;
            
            public EditorWindow     AddGameWindow   = null;
            public EditorWindow     AddAnimWindow   = null;
            public GameObject       DuplicatedObj   = null;
            public AnimationClip    TargetClip      = null;
            public GameObject       TargetGObj      = null;
            public Camera           AdditiveCam     = null;
            
            public bool IsValid
            {
                get { return DuplicatedObj != null && TargetClip != null; }
            }
            
            public EditClipInfo( GameObject gobj, AnimationClip clip, string TargetId )
            {
#pragma warning disable CS0618 // 
                DuplicatedObj = Instantiate( PrefabUtility.FindPrefabRoot( gobj ) );
#pragma warning restore CS0618 // 
                EventPlayer evPlayer = DuplicatedObj.GetComponentInChildren<EventPlayer>();
                if( evPlayer != null )
                {
                    //todo
                    //TargetGObj = EventTrackStatus.CacheTargetOnEditor( evPlayer, TargetId );
                }
                TargetClip      = clip;
            }
            

            public void Release()
            {
                TargetGObj = null;
                
                if( AdditiveCam != null )
                {
                    DestroyImmediate( AdditiveCam.gameObject );
                    AdditiveCam = null;
                }
                
                if( AddGameWindow != null )
                {
                    AddGameWindow.Close();
                    AddGameWindow = null;
                }
                
                if( AddAnimWindow != null )
                {
                    AddAnimWindow.Close();
                    AddAnimWindow = null;
                }
                
                if( DuplicatedObj != null )
                {
                    DestroyImmediate( DuplicatedObj );
                    DuplicatedObj = null;
                }
            }
            

            public void Setup()
            {
                // 
                if( AddGameWindow == null )
                {
                    AddGameWindow = _CreateGameWindow();
                    
                    // 
                    AdditiveCam = new GameObject( "_EditorCam" ).AddComponent<Camera>();
                    AdditiveCam.targetDisplay = TARGET_DISPLAY_IDX;
                }
                
                AddGameWindow.Show();
                

                if( ExistAnimationWindow() == false )
                {
                    AddAnimWindow = GetAnimationWindow();
                }
                
                // 
                if( DuplicatedObj != null )
                {
                    Canvas canvas = DuplicatedObj.GetComponentInChildren<Canvas>();
                    if( canvas != null )
                    {
                        canvas.targetDisplay = TARGET_DISPLAY_IDX;
                    }
                    
                    Animator[] anms = DuplicatedObj.GetComponentsInChildren<Animator>();
                    if( anms != null && anms.Length > 0 )
                    {
                        GameObject selectObj = anms[0].gameObject;
                        
                        for( int i = 0, max = anms.Length; i < max; ++i )
                        {
                            GameObject gobj = anms[i].gameObject;
                            DestroyImmediate( anms[i] );
                            anms[i] = null;
                            
                            Animation newAnm = gobj.AddComponent<Animation>();
                            AnimationUtility.SetAnimationClips( newAnm, new AnimationClip[] { TargetClip } );
                            newAnm.clip = TargetClip;
                            
                            if( gobj == TargetGObj )
                            {
                                selectObj = gobj;
                            }
                        }
                        
                        Selection.activeObject = selectObj;
                    }
                }
            }
            

            public bool IsSameEventPlayer( EventPlayer evPlayer )
            {
                if( evPlayer == null )
                    return false;
                
                if( DuplicatedObj == null )
                    return false;
                
                EventPlayer[] evs = DuplicatedObj.GetComponentsInChildren<EventPlayer>( true );
                if( evs != null && evs.Length > 0 )
                {
                    for( int i = 0, max = evs.Length; i < max; ++i )
                    {
                        if( evs[i] == evPlayer )
                            return true;
                    }
                }
                return false;
            } 
            

            EditorWindow _CreateGameWindow()
            {
                EditorWindow window = CreateGameView();
                
                // 
                var tdField = window.GetType().GetField( "m_TargetDisplay", System.Reflection.BindingFlags.GetField | System.Reflection.BindingFlags.SetField | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance );
                if( tdField != null )
                {
                    tdField.SetValue( window, TARGET_DISPLAY_IDX );
                }
                return window;
            }
        }
        

        static EditClipInfo SetupEditAnimationClip( EventPlayer evPlayer, EventTrack track, GameObject previewObj )
        {
            if( evPlayer == null )
            {
                Debug.LogWarning("没有EventPlayer @ SetupEditAnimationClip()");
                return null;
            }
            
            if( track == null )
            {
                Debug.LogWarning("没有EventPlayer @ SetupEditAnimationClip()");
                return null;
            }
            
            EventTrackAnimation trackAnm = track as EventTrackAnimation;
            if( trackAnm == null )
            {
                Debug.LogWarning( "EventTrackAnimationtrackAnm == null" );
                return null;
            }
            
            if( previewObj == null )
            {
                Debug.LogWarning("没有可预览的对象 @ SetupEditAnimationClip()");
                return null;
            }
            
            EditClipInfo info = new EditClipInfo( previewObj, trackAnm.Animation, trackAnm.GetTargetIdOnEditor() );
            info.Setup();
            
            return info;
        }


        const string GAMEVIEW_TYPE = "UnityEditor.GameView";
        
        static EditorWindow GetGameView()
        {
            return EditorWindow.GetWindow( GetGameViewType() );
        }
        
        static EditorWindow CreateGameView()
        {
            return SceneView.CreateInstance( GAMEVIEW_TYPE ) as EditorWindow;
        }
        
        static System.Type GetGameViewType()
        {
            var assembly = typeof( EditorWindow ).Assembly;
            return assembly.GetType( GAMEVIEW_TYPE );
        }
        

        const string ANIMWINDOW_TYPE = "UnityEditor.AnimationWindow";
        
        static EditorWindow GetAnimationWindow()
        {
            return EditorWindow.GetWindow( GetAnimationWindowType() );
        }
        
        // 
        //static EditorWindow CreateAnimationWindow()
        //{
        //    return SceneView.CreateInstance( ANIMWINDOW_TYPE ) as EditorWindow;
        //}
        
        static System.Type GetAnimationWindowType()
        {
            var assembly = typeof( EditorWindow ).Assembly;
            return assembly.GetType( ANIMWINDOW_TYPE );
        }
        
        static bool ExistAnimationWindow()
        {
            System.Type type = GetAnimationWindowType();
            var tdField = type.GetField( "s_AnimationWindows", System.Reflection.BindingFlags.GetField | System.Reflection.BindingFlags.SetField | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Instance );
            if( tdField != null )
            {
                IList list = tdField.GetValue( null ) as IList;
                if( list != null )
                {
                    return list.Count > 0;
                }
            }
            return false;
        }
        
        
        void _InputEditorRoot()
        {
            switch( Event.current.type )
            {
                case EventType.ScrollWheel:
                    if( Event.current.alt )
                    {
                        if( Event.current.delta.y < 0.0f )
                        {
                            mStride -= 1.0f;
                        }
                        else if( Event.current.delta.y > 0.0f )
                        {
                            mStride += 1.0f;
                        }
                        mStride = Mathf.Clamp( mStride, MIN_STRIDE, MAX_STRIDE );
                    }
                    else
                    {
                        mTimelineScrollPosY += Event.current.delta.y * 3f;
                        if( Event.current.delta.y < 0.0f )
                        {
                            if( mTimelineScrollPosY < 0f )
                            {
                                mTimelineScrollPosY = 0f;
                            }
                        }
                    }
                    Repaint();
                    break;
                case EventType.KeyDown:
                    if( Event.current.keyCode == KeyCode.LeftArrow || Event.current.keyCode == KeyCode.RightArrow )
                    {
                        float timeOffset = 1.0f / FPS_DEF;
                        float offset = 0;
                        
                        if( Event.current.keyCode == KeyCode.LeftArrow )
                        {
                            offset = -timeOffset;
                        }
                        else if( Event.current.keyCode == KeyCode.RightArrow )
                        {
                            offset = timeOffset;
                        }
                        
                        // 
                        for( int idx = 0, max = mTimelineInfo.Count; idx < max; ++idx )
                        {
                            mTimelineInfo[idx].SeekTimeOffset( offset );
                        }
                            
                        mUpddatePreviewObject = true;
                        RepaintAll();
                            
                        Event.current.Use();
                    }
                    break;
            }
        }
        

        private void CallbackCharaList( PreviewUnit previewUnit)
        {
            int idx = 0;
            TimelineInfo info = mTimelineInfo[ idx ];
            if( info == null )
                return;
            
            m_PreviewUnit = previewUnit;
            if( m_PreviewUnit != null )
            {
                info.SetPreviewObject( m_PreviewUnit.BodyObj );
                info.UpdatePreviewObject();
            }
            var param = info.EventParam;
            var temp = new EventParam();
            _SetEventParam( info ,temp,true);
            _SetEventParam( info ,param,true);
            temp = null;
            
            Repaint();
        }
        

        private void SetPreviewFromSelection( EventPlayer evPlayer, EventParam evParam = null )
        {
            int idx = 0;
            TimelineInfo info = mTimelineInfo[ idx ];
            if( info == null )
                return;
            
            info.SetPreviewObject( evPlayer.gameObject, false );
            info.UpdatePreviewObject();
            
            EventPlayerMachine evPlayerMcn = evPlayer.GetComponent<EventPlayerMachine>();
            if( evPlayerMcn != null )
            {
                if( evParam == null )
                {
                    EventPlayerMachine.Param[] evParams = evPlayerMcn.Params;
                    if( evParams != null && evParams.Length > 0 )
                    {
                        evParam = evParams[0].param;
                    }
                }
                
                _SetEventParam( info, evParam );
            }
        }
        
    }
}
