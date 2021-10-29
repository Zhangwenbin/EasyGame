using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using System;
using System.Collections;
using System.Collections.Generic;
using EG;

public class EditorWindow_EffectViewer : EditorWindow
{

    const string    RESOURCES_EFFECT_FOLDER   = "Assets/" + AssetManager.RESOURCE_ASSETBUNDLE_PATH + "Effects/";
    
    readonly int HASH = "EditorWindow_EffectViewerHASH".GetHashCode();
    
    enum EMode
    {
        Preview,
        Create,
    }
    
    readonly string[] MODE_TEXT_TBL = new string[]
    {
        "预览",
        "EffectParam制作",
    };
    
    struct StringTuple
    {
        public string Val1;
        public string Val2;
        
        public StringTuple( string v1, string v2 )
        {
            Val1 = v1;
            Val2 = v2;
        }
    };
    

    bool                    m_DockOnce              = false;
    UnityEditor.SceneView   m_AdditionSceneView     = null;
    bool                    m_IsPlaying             = false;
    bool                    m_IsForceLoop           = true;
    System.DateTime         m_LastUpdateTime;
    float                   m_SpeedRate             = 1.0f;

    float                   m_ElapseTime            = 0f;
    //float                   m_PreTime               = 0f;
    float                   m_DeltaTime             = 0f;
    float                   m_MaxTime               = 10f;
    
    bool                    m_IsSettingLoop         = false;
    bool                    m_HaveLoopInParticle    = false;
    bool                    m_DontPreviewPlay       = false;
    
    EMode                   m_Mode                  = EMode.Preview;
    
    List<StringTuple>       m_EffectList            = new List<StringTuple>();
    EffectData              m_PreviewEffect         = null;
    Vector2                 m_ScrollPosEffList      = Vector2.zero;
    int                     m_SelectIdx             = -1;
    string                  m_Filter                = "";
    
    // UIParticle用Canvas
    GameObject              m_CanvasObj             = null;
    
    GameObject              m_Prefab                = null;
    EventParam              m_EvParam               = null;
    
    GUIStyle                m_ListItemStyle         = null;
    
    
    [MenuItem( "EG/Game/EffectPreview", false, 301 )]
    public static EditorWindow_EffectViewer Init()
    {
        EditorWindow_EffectViewer window = GetWindow<EditorWindow_EffectViewer>();
        window.titleContent = new GUIContent("EffectPreview");
        return window;
    }
    
    public static void Init( EffectParam effParam )
    {
        EditorWindow_EffectViewer window = Init();
        if( window != null )
        {
            window.SetEffectParam( effParam );
        }
    }
    
    void OnEnable()
    {
        m_AdditionSceneView = SceneView.CreateInstance( "UnityEditor.SceneView" ) as SceneView;
        m_AdditionSceneView.Show();
        
        EditorApplication.update += OnUpdate;
        
        // Canvas作成
        CreateCanvas();
        
        SetupEffectList();
    }
    
    void OnDisable()
    {
        DestroyCanvas();
        
        EditorApplication.update -= OnUpdate;
        
        if( m_AdditionSceneView != null )
        {
            m_AdditionSceneView.Close();
            m_AdditionSceneView = null;
        }
        
        if( m_PreviewEffect != null )
        {
            DestroyImmediate( m_PreviewEffect.gameObject );
            m_PreviewEffect = null;
        }
    }
    
    void OnDestroy()
    {
    }

    void OnGUI()
    {
        Event e = Event.current;
        int id = GUIUtility.GetControlID( HASH, FocusType.Passive );
        EventType type = e.GetTypeForControl(id);
        
        if( type == EventType.KeyDown )
        {
            if( e.keyCode == KeyCode.Escape )
            {
                Close();
                return;
            }
        }
        
        InitStyle();
        
        _OnGUI();
        
        if( m_DockOnce == false )
        {
            m_DockOnce = true;
            DockSceneView();
        }
    }
    
    
    private void OnUpdate()
    {
        System.DateTime currentTime = System.DateTime.Now;
        //m_PreTime = m_ElapseTime;
        m_DeltaTime = 0;
        
        if( m_IsPlaying )
        {
            bool isLoop = false;
            m_DeltaTime = ( float )( currentTime - m_LastUpdateTime ).TotalSeconds * m_SpeedRate;
            m_ElapseTime += m_DeltaTime;
            if( m_ElapseTime >= m_MaxTime )
            {
                if( m_IsForceLoop )
                {
                    m_ElapseTime -= m_MaxTime;
                    if( m_HaveLoopInParticle == false )
                    {
                        isLoop = true;
                    }
                }
                else
                {
                    m_IsPlaying = false;
                }
            }
            
            if( m_PreviewEffect != null )
            {
                m_PreviewEffect.SimulateNew( m_ElapseTime, m_DeltaTime, isLoop );
            }
            
            Repaint();
            //SceneView.RepaintAll();
        }
        
        m_LastUpdateTime = currentTime;
    }
    
    void _OnGUI()
    {
        m_Mode = (EMode)GUILayout.Toolbar( (int)m_Mode, MODE_TEXT_TBL );
        if( Application.isPlaying )
        {
            m_Mode = EMode.Preview;
            GUI.enabled = false;
        }
        
        // if( GUILayout.Button( "check_soundを開く" ) )
        // {
        //     if( GameObject.FindObjectOfType<LWARS.Scene>() != null )
        //     {
        //         UnityEditor.SceneManagement.EditorSceneManager.OpenScene( "Assets/Scenes/01_Streamed/Debug/check_sound.unity" );
        //     }
        //     EditorApplication.isPlaying = true;
        // }
        
        switch( m_Mode )
        {
            case EMode.Preview: DrawGUIPreview(); break;
            case EMode.Create: DrawGUICreateScriptableObject(); break;
        }
        
        GUI.enabled = true;
    }
    
    void DrawGUIPreview()
    {
        DrawGUIPreviewController();
        
        GUI.enabled = true; // 
        
        GUILayout.BeginVertical( GUI.skin.box );
        {
            DrawGUIEffectList();
        }
        GUILayout.EndVertical();
    }
    
    void DrawGUIEffectList()
    {
        if( m_EffectList.Count == 0 )
        {
            GUILayout.Label( "没有EffectParam。" );
            return;
        }
        
        GUILayout.BeginHorizontal();
        {
            GUILayout.Label( "请从以下列表中选择要预览的效果。  " );
            if( GUILayout.Button( "列表更新" ) )
            {
                SetupEffectList();
            }
        }
        GUILayout.EndHorizontal();
        
        GUILayout.BeginHorizontal();
        {
            GUILayout.Label( "Filter: ", GUILayout.Width( 60 ) );
            m_Filter = GUILayout.TextField( m_Filter );
        }
        GUILayout.EndHorizontal();
        
        m_ScrollPosEffList = GUILayout.BeginScrollView( m_ScrollPosEffList, GUI.skin.box );
        {
            bool isFilterOn = ( string.IsNullOrEmpty( m_Filter ) == false );
            
            for( int i = 0, max = m_EffectList.Count; i < max; ++i )
            {
                if( isFilterOn )
                {
                    if( m_EffectList[i].Val2.Contains( m_Filter ) == false )
                        continue;
                }
                
                GUIStyle style = GUI.skin.label;
                if( m_SelectIdx == i )
                {
                    style = m_ListItemStyle;
                }
                
                if( GUILayout.Button( m_EffectList[i].Val2, style ) )
                {
                    m_SelectIdx = i;
                    SetPreviewEffect( m_EffectList[i].Val1 );
                }
            }
        }
        GUILayout.EndScrollView();
        
        Event ev = Event.current;
        if( ev != null && ev.isKey )
        {
            switch( ev.type )
            {
                case EventType.KeyDown:
                    if( ev.keyCode == KeyCode.DownArrow )
                    {
                        int preIdx = m_SelectIdx;
                        ++m_SelectIdx;
                        if( m_SelectIdx >= m_EffectList.Count )
                            m_SelectIdx = m_EffectList.Count - 1;
                        
                        if( preIdx != m_SelectIdx )
                        {
                            SetPreviewEffect( m_EffectList[m_SelectIdx].Val1 );
                            Repaint();
                        }
                    }
                    else if( ev.keyCode == KeyCode.UpArrow )
                    {
                        int preIdx = m_SelectIdx;
                        --m_SelectIdx;
                        if( m_SelectIdx < 0 )
                            m_SelectIdx = 0;
                        
                        if( preIdx != m_SelectIdx )
                        {
                            SetPreviewEffect( m_EffectList[m_SelectIdx].Val1 );
                            Repaint();
                        }
                    }
                    break;
            }
        }
    }
    
    void DrawGUIPreviewController()
    {
        GUILayout.BeginVertical( GUI.skin.box );
        {
            GUILayout.BeginHorizontal();
            {
                string txt = "没有对象被读取。";
                if( m_PreviewEffect != null )
                {
                    txt  = "[ " + m_PreviewEffect.gameObject.name + " ]";
                }
                GUILayout.Label( txt + " ", GUILayout.Width( 240 ) );
                
                GUILayout.Space( 10 );
                
                if( m_DontPreviewPlay )
                {
                    GUI.contentColor = Color.red;
                    GUILayout.Label( "因为Event附带的效果，这里不能再生。 请在事件编辑器中确认  " );
                    GUI.contentColor = Color.white;
                }
                else if( m_PreviewEffect != null )
                {
                    GUILayout.Label( "长度: " + m_MaxTime, GUILayout.Width( 80 ) );
                    
                    GUILayout.Space( 5 );
                    
                    bool isDiff = m_IsSettingLoop != m_HaveLoopInParticle;
                    
                    GUI.contentColor = ( isDiff ) ? Color.red : Color.green;
                    GUILayout.Label( ( m_IsSettingLoop ? "param loop" : "no param loop" ), GUILayout.Width( 90 ) );
                    GUI.contentColor = Color.white;
                    
                    GUI.contentColor = ( isDiff ) ? Color.red : Color.cyan;
                    GUILayout.Label( ( m_HaveLoopInParticle ? "LoopInParticle" : "no LoopInParticle" ), GUILayout.Width( 50 ) );
                    GUI.contentColor = Color.white;
                }
            }
            GUILayout.EndHorizontal();
            
            bool isCtrlEnable = ( m_DontPreviewPlay == false ) && ( Application.isPlaying == false );
            
            GUILayout.BeginHorizontal();
            {
                bool isEnable = ( m_PreviewEffect != null );
                
                if( m_IsPlaying && isEnable == false )
                {
                    m_IsPlaying = false;
                }
                
                GUI.enabled = isEnable && isCtrlEnable;
                string txt = ( m_IsPlaying ) ? "停止" : "再生";
                GUI.backgroundColor = Color.green;
                if( GUILayout.Button( txt, GUILayout.Width( 120 ) ) )
                {
                    m_IsPlaying = !m_IsPlaying;
                }
                GUI.backgroundColor = Color.white;
                GUI.enabled = true && isCtrlEnable;
                
                txt = ( m_IsForceLoop ) ? "Loop" : "NoLoop";
                if( GUILayout.Button( txt, GUILayout.Width( 120 ) ) )
                {
                    m_IsForceLoop = !m_IsForceLoop;
                }
                
                if( GUILayout.Button( "start", GUILayout.Width( 120 ) ) )
                {
                    m_ElapseTime = 0;
                    m_PreviewEffect.SimulateNew( 0, 0, true );
                }
                
                GUILayout.Label( "speed :", GUILayout.Width( 60 ) );
                m_SpeedRate = EditorGUILayout.Slider( m_SpeedRate, 0.1f, 3.0f );
            }
            GUILayout.EndHorizontal();
            
            GUILayout.BeginHorizontal();
            {
                GUILayout.Label( "位置 :", GUILayout.Width( 60 ) );
                float slideVal = EditorGUILayout.Slider( m_ElapseTime, 0f, m_MaxTime );
                if( m_ElapseTime != slideVal )
                {
                    m_IsPlaying = false;
                    m_ElapseTime = slideVal;
                    if( m_PreviewEffect != null )
                    {
                        m_PreviewEffect.SimulateNew( m_ElapseTime, 0, true );
                        SceneView.RepaintAll();
                    }
                }
            }
            GUILayout.EndHorizontal();
        }
        GUILayout.EndVertical();
        
        GUI.enabled = true;
    }
    
    void DrawGUICreateScriptableObject()
    {
        string err = "";
        bool isEnable = true;
        GUILayout.BeginVertical( GUI.skin.box );
        {
            GUILayout.Label( "ScriptableObject 创建" );
            
            GUILayout.BeginHorizontal();
            {
                GUILayout.Label( "Prefab ", GUILayout.Width( 80 ) );
                m_Prefab = EditorGUILayout.ObjectField( m_Prefab, typeof( GameObject ), true ) as GameObject;
            }
            GUILayout.EndHorizontal();
            
            GUILayout.BeginHorizontal();
            {
                GUILayout.Label( "EventParam ", GUILayout.Width( 80 ) );
                m_EvParam = EditorGUILayout.ObjectField( m_EvParam, typeof( EventParam ), true ) as EventParam;
            }
            GUILayout.EndHorizontal();
            
            isEnable = IsEnableCreateScriptableObject( m_Prefab, m_EvParam, out err );
            GUI.enabled = isEnable;
            GUI.backgroundColor = Color.green;
            if( GUILayout.Button( "new", GUILayout.Width( 80 ) ) )
            {
                CreateScriptableObjectFromEffectBase( m_Prefab, m_EvParam );
                m_Prefab = null;
            }
            GUI.backgroundColor = Color.white;
            GUI.enabled = true;
        }
        GUILayout.EndVertical();
        
        GUI.contentColor = ( isEnable ) ? Color.white : Color.red;
        GUILayout.Label( err );
        GUI.contentColor = Color.white;
    }
    
    
    static public void CreateScriptableObjectFromEffectBase( GameObject prefab, EventParam evParam )
    {
        if( prefab == null && evParam == null )
            return;
        
        EffectBase effBase = null;
        if( prefab != null )
        {
            prefab.GetComponent<EffectBase>();
        }

        string path = "";
        string defName = "";
        if( prefab != null )
        {
            defName = prefab.name;
        }
        else if( evParam != null )
        {
            defName = evParam.name;
        }
        path = EditorUtility.SaveFilePanel( "选择保存地址", RESOURCES_EFFECT_FOLDER, defName, "asset");
        if( string.IsNullOrEmpty( path ) == false )
        {
            path = path.Replace( Application.dataPath, "" );
            path = "Assets" + path;
            
            EffectParam effParam = ScriptableObject.CreateInstance<EffectParam>();
            EffectData.Param param = new EffectData.Param();
            param.PlayMode = EffectData.EPlayMode.OneShot;
            param.AliveTime = 0;
            param.GenOption = new BitFlag();

            if( effBase != null )
            {
                param.PlayMode  = (EffectData.EPlayMode)(int)effBase.PlayMode;
                param.AliveTime = effBase.m_AliveTime;
                
                if( effBase.Flag.HasValue( EffectBase.GenerateOption.OneShot ) )
                {
                    param.PlayMode = EffectData.EPlayMode.OneShot;
                }
                if( effBase.Flag.HasValue( EffectBase.GenerateOption.Mirror ) )
                {
                    param.GenOption.SetValue( EffectData.EGenerateOption.Mirror );
                }
            }
            effParam.SetParam( prefab, evParam, ref param );
            
            AssetDatabase.CreateAsset( effParam, path );
            AssetDatabase.Refresh();
        }
    }
    
    bool IsEnableCreateScriptableObject( GameObject prefab, EventParam evParam, out string err )
    {
        err = "是可以创建的。";
        
        if( prefab != null )
        {
            if( prefab.GetComponent<EffectBase>() == null )
            {
                err = "因为没有EffectBase，所以是默认值。  ";
            }
            
            return true;
        }
        
        if( evParam != null )
        {
            return true;
        }
        
        err = "请指定效果的Prefab或EventParam  ";
        return false;
    }
    
    void SetupEffectList()
    {
        m_EffectList.Clear();
        
        string[] guids = AssetDatabase.FindAssets( "t:EffectParam", null );
        if( guids != null && guids.Length > 0 )
        {
            for( int i = 0, max = guids.Length; i < max; ++i )
            {
                string path = AssetDatabase.GUIDToAssetPath( guids[i] );
                string fileName = System.IO.Path.GetFileNameWithoutExtension( path );
                
                m_EffectList.Add( new StringTuple( path, fileName ) );
            }
            
            if( m_EffectList.Count > 2 )
            {
                m_EffectList.Sort( ( a, b )=> { return string.Compare( a.Val2, b.Val2 ); } );
            }
        }
    }
    
    void SetEffectParam( EffectParam effParam )
    {
        if( effParam == null )
            return;
        
        if( m_EffectList.Count > 0 )
        {
            for( int i = 0, max = m_EffectList.Count; i < max; ++i )
            {
                if( effParam.name == m_EffectList[i].Val2 )
                {
                    m_SelectIdx = i;
                    SetPreviewEffect( effParam );
                    break;
                }
            }
        }
    }
    
    void SetPreviewEffect( string assetPath )
    {
        if( string.IsNullOrEmpty( assetPath ) )
            return;
        
        EffectParam effParam = AssetDatabase.LoadAssetAtPath<EffectParam>( assetPath );
        SetPreviewEffect( effParam );
    }
    
    void SetPreviewEffect( EffectParam param )
    {
        if( m_PreviewEffect != null )
        {
            m_PreviewEffect.Release();
            DestroyImmediate( m_PreviewEffect.gameObject );
            m_PreviewEffect = null;
        }
        
        if( param == null )
            return;
        
        m_IsPlaying = false;
        m_ElapseTime = 0;
        //m_PreTime = 0;
        m_IsSettingLoop = param.Param.PlayMode == EffectData.EPlayMode.Loop;
        
        m_PreviewEffect = param.CreateEffect( Vector3.zero, Quaternion.identity );
        if( m_PreviewEffect != null )
        {
            bool isUI = false;
            Vector3 camPos = Vector3.zero;
            
            m_PreviewEffect.EditorAwake();
            
            EffectDataParticle effParticle = m_PreviewEffect as EffectDataParticle;
            if( effParticle != null )
            {
                effParticle.GetParticleInfo( out m_MaxTime, out m_HaveLoopInParticle );
                
                if( effParticle.UIParticleCount > 0 )
                {
                    if( effParticle.GetComponentInChildren<Canvas>() == null )
                    {
                        effParticle.transform.SetParent( m_CanvasObj.transform, false );
                    }
                    
                    camPos = m_CanvasObj.transform.position;
                    camPos.z = 500;
                    isUI = true;
                }
            }
            
            m_DontPreviewPlay = m_PreviewEffect is EffectDataEvent;
            
            m_PreviewEffect.Play();
            
            if( isUI )
            {
                ResetSceneViewCamera( camPos );
            }
            else
            {
                ResetSceneViewCamera();
            }
        }
    }
    
    void InitStyle()
    {
        if( m_ListItemStyle == null )
        {
            m_ListItemStyle = new GUIStyle();
            m_ListItemStyle.normal.textColor = GUI.skin.label.normal.textColor;
            m_ListItemStyle.padding = new RectOffset( 0, 0, 0, 0 );
            m_ListItemStyle.onNormal.textColor = Color.black;
            m_ListItemStyle.onNormal.background = Texture2D.whiteTexture;
            
            m_ListItemStyle.normal.textColor = Color.black;
            m_ListItemStyle.normal.background = Texture2D.whiteTexture;
        }
    }
    
    void CreateCanvas()
    {
        if( m_CanvasObj != null )
            return;
        
        m_CanvasObj = new GameObject( "PreviewCanvas" );
        m_CanvasObj.AddComponent<RectTransform>();
        Canvas canvas = m_CanvasObj.AddComponent<Canvas>();
        {
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.pixelPerfect = false;
            canvas.sortingOrder = 0;
        }
        m_CanvasObj.AddComponent<GraphicRaycaster>();
        // m_CanvasObj.AddComponent<UICanvasScaler>();
    }
    
    void DestroyCanvas()
    {
        if( m_CanvasObj != null )
        {
            DestroyImmediate( m_CanvasObj );
            m_CanvasObj = null;
        }
    }
    
    
    void DockSceneView()
    {
        // m_AdditionSceneView.Focus();
        
        //if( mAnmCtrlWindow != null )
        //{
        //    mAnmCtrlWindow.Show();
        //}
        
        // Docker.Dock( this, m_AdditionSceneView, Docker.DockPosition.Right );
        //Docker.Dock( mAdditionSceneView, mAnmCtrlWindow, Docker.DockPosition.Bottom, new Vector2( this.position.width, 0 ) );
        
        ResetSceneViewCamera();
    }
    
    void ResetSceneViewCamera()
    {
        ResetSceneViewCamera( new Vector3( 0f, 0.5f, 0f ) );
    }
    
    void ResetSceneViewCamera( Vector3 wPos )
    {
        if( m_AdditionSceneView != null )
        {
            m_AdditionSceneView.LookAtDirect( wPos, Quaternion.Euler( 0, 180, 0 ), 1.5f );
            SceneView.RepaintAll();
        }
    }

}
