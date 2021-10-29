using UnityEngine;
using UnityEngine.Playables;
using System;
using System.Collections;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace EG
{
    public class EventTrackAnimation : EventTrack
    {
        public class Status : EventTrackStatus
        {

            private EventPlayer             m_Player            = null;
            private EventTrackAnimation     m_Self              = null;
            private AnimationPlayer         m_AnimPlayer        = null;
            
            private ObjectCache             m_Cache             = default(ObjectCache);
            private GameObject[]            m_CurveObj          = null;
            

            
            public EventPlayer              Player              { get { return m_Player;        } }
            public AnimationPlayer          AnimPlayer          { get { return m_AnimPlayer;    } }
            
            public GameObject               gameObject          { get { return m_Cache.gameObject; } }
            
            
            public Status( EventPlayerStatus owner ) : base( owner )
            {
            }
            
            public override void Initialize( EG.AppMonoBehaviour behaviour, EventTrack track )
            {
                base.Initialize( behaviour, track );
                
                m_Self = Track as EventTrackAnimation;
                CacheTarget(behaviour,m_Self._targetId,ref m_Cache);
                CheckPlayer( behaviour, gameObject );
                
                CreateCurveObject( );
            }
            
            public override void Release( AppMonoBehaviour behaviour )
            {
                if( m_AnimPlayer != null )
                {
                    m_AnimPlayer.Destroy( );
                    m_AnimPlayer = null;
                }
                
                DestroyCurveObject( );
                

                base.Release( behaviour );
            }
            
            
            protected void CheckPlayer( AppMonoBehaviour behaviour, GameObject gobj )
            {
                if( m_Player == null )
                {
                    m_Player = behaviour != null ? behaviour.GetComponent<EventPlayer>( ) : null;
                    if( m_Player == null )
                    {
                        Debug.LogError( "EventPlayer EventTrackAnimation: not found behaviour" );
                    }
                }
                if( m_Player.AnimationPlayer == null && m_AnimPlayer == null )
                {
                    m_AnimPlayer = new AnimationPlayer( );
                    if( gobj != null )
                    {
                        Animator animator = gobj.RequireComponent<Animator>( );
                        if( animator != null )
                        {
                            m_AnimPlayer.Create( animator, 1, m_Self._manual_controll ? DirectorUpdateMode.Manual: DirectorUpdateMode.GameTime );
                        }
                        else
                        {
                            Debug.LogError( "AnimationPlayer EventTrackAnimation: not found AnimationPlayer" );
                        }
                    }
                }
            }
            

            public override GameObject GetObject( string name )
            {
                if( m_CurveObj != null )
                {
                    for( int i = 0; i < m_CurveObj.Length; ++i )
                    {
                        if( m_CurveObj[i] != null && m_CurveObj[i].name == name )
                        {
                            return m_CurveObj[i];
                        }
                    }
                }
                
                return null;
            }
            
            public void CreateCurveObject( )
            {
                if( m_Self._curve_create == false ) return;
                
                EventPlayer player = Player;
                if( player == null ) return;

                if( m_Self._curve_names.Count > 0 )
                {
                    m_CurveObj = new GameObject[ m_Self._curve_names.Count ];
                    
                    for( int i = 0; i < m_Self._curve_names.Count; ++i )
                    {
                        if( string.IsNullOrEmpty( m_Self._curve_names[i] ) == false )
                        {
                            m_CurveObj[i] = new GameObject();
                            m_CurveObj[i].name = m_Self._curve_names[i];
                            if( player.AnimationPlayer != null )
                            {
                                m_CurveObj[ i ].transform.parent = player.AnimationPlayer.Animator.gameObject.transform;
                            }
                        }
                    }
                }
            }
            

            public void DestroyCurveObject( )
            {
                if( m_CurveObj != null )
                {
                    for( int i = 0; i < m_CurveObj.Length; ++i )
                    {
                        if( m_CurveObj[i] != null )
                        {
                            #if UNITY_EDITOR
                            GameObject.DestroyImmediate( m_CurveObj[i] );
                            #else
                            GameObject.Destroy( m_CurveObj[i] );
                            #endif
                            m_CurveObj[i] = null;
                        }
                    }
                    m_CurveObj = null;
                }
            }
            
            protected override void OnStart( AppMonoBehaviour behaviour )
            {
                base.OnStart( behaviour );
            }
            

            protected override void OnUpdate( AppMonoBehaviour behaviour, float time )
            {
                base.OnUpdate( behaviour, time );
            }
            

            protected override void OnEnd( AppMonoBehaviour behaviour )
            {
                base.OnEnd( behaviour );
            }
            

            protected override void OnBackground( AppMonoBehaviour behaviour, float time )
            {
                base.OnBackground( behaviour, time );
            }
            

            #if UNITY_EDITOR
            
            protected override void ReCacheTarget( AppMonoBehaviour behaviour )
            {
                CacheTarget(behaviour,m_Self._targetId,ref m_Cache);
                CheckPlayer( behaviour, gameObject );
            }
            
            #endif //UNITY_EDITOR
        }
        
        [System.Serializable]
        public class AnmSet
        {
            public string           Key;
            public AnimationClip    Clip;
            
            public AnmSet( string key )
            {
                Key = key;
            }
        }
        
        
        [SerializeField] private string                 _targetId       = null;
        [SerializeField] private AnimationClip          _animation      = null;
        [SerializeField] private bool                   _manual_controll= false;
        [SerializeField] private bool                   _keep           = true;
        [SerializeField] private float                  _blend_time     = 0.0f;
        [SerializeField] private int                    _layer_id       = 0;
        
        [SerializeField] private AnmSet[]               _subAnms        = new AnmSet[0];
        

        [SerializeField] private bool                   _curve_create = false;
        [SerializeField] private List<string>           _curve_names;
        

        
        public override ECategory Category
        {
            get { return ECategory.Common; }
        }
        
        public AnimationClip    Animation       { get { return _animation; } set { _animation = value;} }
        public List<string>     CurveNames      { get { return _curve_names; } set { _curve_names = value;} }
        
        public float Length
        {
            get
            {
                return _animation.length;
            }
        }
        
        
        public override EventTrackStatus CreateStatus( EventPlayerStatus owner )
        {
            return new Status( owner );
        }
        
        private string GetSubClipKey( EG.AppMonoBehaviour behaviour )
        {
            return "";
        }
        
        AnimationClip GetSubClip( string key )
        {
            if( string.IsNullOrEmpty( key ) ) return _animation;
            if( _subAnms.Length == 0 ) return _animation;
            
            for( int i = 0, max = _subAnms.Length; i < max; ++i )
            {
                AnmSet set = _subAnms[i];
                if( set == null ) continue;
                
                if( set.Key == key )
                {
                    return ( set.Clip != null ) ? set.Clip : _animation;
                }
            }
            
            return _animation;
        }
        
        
        public override void OnStart( EG.AppMonoBehaviour behaviour )
        {
            Status status = CurrentStatus as Status;
            if( status != null )
            {
                AnimationPlayer animPlayer = null;
                
                if( status.AnimPlayer != null )
                {
                    animPlayer = status.AnimPlayer;
                }
                else
                {
                    EventPlayer player = status.Player;
                    if( player == null || player.AnimationPlayer == null) return;
                    animPlayer = player.AnimationPlayer;
                }
                
                if( animPlayer != null )
                {
                    string subClipKey = GetSubClipKey( behaviour );
                    if( string.IsNullOrEmpty( subClipKey ) )
                    {
                        animPlayer.Play( _animation, _blend_time, _layer_id );
                    }
                    else
                    {
                        #if UNITY_EDITOR
                        if( Application.isPlaying == false )
                        {
                            animPlayer.Play( _animation, 0, _layer_id );
                            animPlayer.Play( GetSubClip( subClipKey ), _blend_time, _layer_id );
                        }
                        else
                        {
                            animPlayer.Play( GetSubClip( subClipKey ), _blend_time, _layer_id );
                        }
                        #else
                        animPlayer.Play( GetSubClip( subClipKey ), _blend_time, _layer_id );
                        #endif
                    }
                }
            }
        }
        

        public override void OnUpdate( EG.AppMonoBehaviour behaviour, float time )
        {
            Status status = CurrentStatus as Status;
            if( status != null )
            {
                AnimationPlayer animPlayer = null;
                
                if( status.AnimPlayer != null )
                {
                    animPlayer = status.AnimPlayer;
                    animPlayer.Update( EventTrack.DeltaTime );
                }
                else
                {
                    EventPlayer player = status.Player;
                    if( player == null || player.AnimationPlayer == null) return;
                    animPlayer = player.AnimationPlayer;
                }
                
                if( animPlayer != null )
                {
                    EventTrackAnimation self = status.Track as EventTrackAnimation;
                    if( self != null && self._manual_controll || status.Owner.isImmidiate )
                    {
                        animPlayer.SetTimeToLastAnim( time );
                    }
                }
            }
        }
        
        public override void OnEnd( EG.AppMonoBehaviour behaviour )
        {
            Status status = CurrentStatus as Status;
            if( status != null )
            {
                if( status.AnimPlayer != null )
                {
                    if( _keep == false )
                    {
                        status.AnimPlayer.Stop( _animation );
                    }
                }
                else
                {
                    EventPlayer player = status.Player;
                    if( player == null || player.AnimationPlayer == null) return;
                    
                    if( _keep == false )
                    {
                        player.AnimationPlayer.Stop( _animation );
                    }
                }
            }
        }
        
        #if UNITY_EDITOR
        
        GUIStyle    mBorderStyle;
        bool        m_FoldSubAnm = false;
        string      m_AddKeyName = "";
        
        public static string    ClassName  { get { return "动画";                        } }
        public override Color   TrackColor { get { return new Color32( 0xe6, 0xe6, 0x00, 0xff );   } }
        

        void OnEnable()
        {
            mBorderStyle = new GUIStyle();
            mBorderStyle.normal.background = (Texture2D)EditorGUIUtility.Load("box1.png");
            mBorderStyle.border = new RectOffset(8, 8, 8, 8);
            mBorderStyle.padding = new RectOffset(6, 6, 6, 6);
        }
        

        public string GetTargetIdOnEditor()
        {
            return _targetId;
        }
        
        bool _ContainsSubAnm( string key )
        {
            if( string.IsNullOrEmpty( key ) )
                return false;
            
            if( _subAnms.Length == 0 )
                return false;
            
            for( int i = 0, max = _subAnms.Length; i < max; ++i )
            {
                if( _subAnms[i].Key == key )
                    return true;
            }
            return false;
        }
        

        public override void OnInspectorGUI( Rect position, SerializedObject serializeObject, float width )
        {
            EventPlayer player = EventPlayer.CurrentEditorPlayer;
            AnimationClip animationOld = _animation;
            float start = Start;

            // 
            EditorGUILayout.BeginHorizontal( );
            {
                Start = EditorGUILayout.FloatField( "Start", Start );
                if( start != Start )
                {
                    OnReimport( );
                    //CacheCurves();
                    EditorUtility.SetDirty( this );
                }
            }
            EditorGUILayout.EndHorizontal( );
            
            // 
            EditorGUILayout.BeginHorizontal( );
            {
                _animation = (AnimationClip)EditorGUILayout.ObjectField( "AnimClip", _animation, typeof(AnimationClip), false );
                if( animationOld != _animation )
                {
                    OnReimport();
                    //CacheCurves();
                    EditorUtility.SetDirty( this );
                }
            }
            EditorGUILayout.EndHorizontal( );
            
            EditorGUILayout.BeginHorizontal( );
            {
                string preTargetId = _targetId;
                _targetId = EditorGUILayout.TextField( "_targetId", _targetId );
                if( player != null )
                {
                    SerializeValueBehaviour beheviour = player.GetComponentInChildren<SerializeValueBehaviour>( );
                    if( beheviour != null )
                    {
                        SerializeValueList valueList = beheviour.list;
                        string selectName = _targetId;
                        SerializeValue[] gobjs = valueList.GetFields<GameObject>( );
                        List<string> names = new List<string>( );
                        for( int i = 0; i < gobjs.Length; ++i ) names.Add( gobjs[ i ].key );
                        int value = names.FindIndex( ( p ) => p == selectName );
                        GUI.contentColor = value == -1 ? Color.red: Color.green;
                        int nextValue = UnityEditor.EditorGUILayout.Popup( "", value, names.ToArray( ), GUILayout.Width( 15f ) );
                        if( value != nextValue )
                        {
                            _targetId = names[ nextValue ];
                        }
                        GameObject gobj = valueList.GetGameObject( selectName );
                        GUIStyle style = new GUIStyle( "toolbarButton" );
                        style.alignment = TextAnchor.MiddleCenter;
                        GUI.enabled = gobj != null ? true: false;
                        if( GUILayout.Button( "sel", style, GUILayout.Width( 30f ) ) )
                        {
                            UnityEditor.Selection.activeGameObject = gobj;
                        }
                        GUI.enabled = true;
                        GUI.contentColor = Color.white;
                    }
                }
                
                if( preTargetId != _targetId )
                {
                    RequestTrackStatusUpdate();
                    OnReimport();
                    EditorUtility.SetDirty( this );
                }
            }
            EditorGUILayout.EndHorizontal( );
            
            // Layer ID

            
            // 
            bool self_controll = _manual_controll;
            _manual_controll = EditorGUILayout.Toggle("_manual_controll", _manual_controll);
            if (self_controll != _manual_controll)
            {
                OnReimport();
                EditorUtility.SetDirty(this);
            }
            
            // 
            bool keep = _keep;
            _keep = EditorGUILayout.Toggle("Clip Keep", _keep);
            if (keep != _keep)
            {
                OnReimport();
                EditorUtility.SetDirty(this);
            }
            
            // 
            float blend_time = _blend_time;
            _blend_time = EditorGUILayout.FloatField("Blend Time", _blend_time);
            if (blend_time != _blend_time)
            {
                OnReimport();
                EditorUtility.SetDirty(this);
            }
            
            // 
            if( GUILayout.Button( "Animationlength" ) )
            {
                End = Start;
                if( _animation != null )
                {
                    End += _animation.length;
                }
                else
                {
                    Debug.LogError( "_animation==null" );
                }
                
                OnReimport();
                EditorUtility.SetDirty( this );
            }
            
            // 
            bool curve_create = _curve_create;
            _curve_create = EditorGUILayout.Toggle("_curve_create", _curve_create);
            if( curve_create != _curve_create )
            {
                OnReimport( );
            }
            
            // 
            EditorGUI.BeginDisabledGroup(true);
            
            EditorGUILayout.LabelField( "Animation Clip:" );
            if (_animation != null)
            {
                EditorGUILayout.BeginHorizontal( );
                GUILayout.Space( 10 );
                EditorGUILayout.FloatField( "Length", _animation.length );
                EditorGUILayout.EndHorizontal( );
                
                EditorGUILayout.BeginHorizontal( );
                GUILayout.Space( 10 );
                EditorGUILayout.Toggle( "Loop", _animation.isLooping );
                EditorGUILayout.EndHorizontal( );
                
                // 
                if (CurveNames != null)
                {
                    EditorGUILayout.BeginHorizontal( );
                    GUILayout.Space( 10 );
                    EditorGUILayout.IntField("Curve Count", CurveNames.Count);
                    EditorGUILayout.EndHorizontal( );
                    
                    foreach (var i in CurveNames)
                    {
                        EditorGUILayout.BeginHorizontal( );
                        GUILayout.Space( 10 );
                        EditorGUILayout.TextField(i);
                        EditorGUILayout.EndHorizontal( );
                    }
                }
            }
            
            EditorGUI.EndDisabledGroup();
        }
        
        public void OnReimport()
        {
            if( _curve_create )
            {
                CurveNames = new List<string>( );
                End = Start;
                if( _animation != null )
                {
                    End += _animation.length;
                    
                    EditorCurveBinding[] curves = AnimationUtility.GetCurveBindings( _animation );
                    
                    for( int i = 0; i < curves.Length; ++i )
                    {
                        string path = curves[ i ].path;
                        string curveName = extractBoneName( path );
                        if( curveName != "" && !CurveNames.Contains( curveName ) && curveName.IndexOf(".") < 0 )
                        {
                            if( curveName.IndexOf( "Camera" ) == -1 && curveName.IndexOf( "" ) == -1 ) continue;
                            CurveNames.Add( curveName );
                        }
                    }
                }
                EditorUtility.SetDirty( this );
            }
            else
            {
                CurveNames = null;
            }
        }
        

        private string extractBoneName(string path)
        {
            if (path == null) return "";
            string[] nodes = path.Split('/');
            string name = nodes[nodes.Length - 1];
            return name;
        }
        
        public override void EditorRelease()
        {
            base.EditorRelease();
            
            EventPlayer player = EventPlayer.CurrentEditorPlayer;
            if( player != null && player.AnimationPlayer != null )
            {
                player.AnimationPlayer.Stop( _animation );
            }
        }
        
        public override void EditorPreProcess( EG.AppMonoBehaviour behaviour, float time, float dt, bool isLooped, bool isEnded )
        {
            base.EditorPreProcess( behaviour, time, dt, isLooped, isEnded );

            if( behaviour == null )
                return;
            
            EventPlayer evPlayer = (EventPlayer)behaviour;
            if( evPlayer == null || evPlayer.AnimationPlayer == null )
                return;
            
            SetAnimationTime( evPlayer.AnimationPlayer, time - Start );
        }
        
        void SetAnimationTime( AnimationPlayer anmPlayer, float time )
        {
            if( _animation == null )
                return;
            
            if( anmPlayer == null )
                return;
            
            AnimationPlayer.PlayStatus status = anmPlayer.FindStatus( _animation );
            if( status != null )
            {
                SetTimeToPlayable( anmPlayer.Graph, status._clip_playable.GetHandle(), time );
            }
        }
        
        void SetTimeToPlayable( PlayableGraph graph, PlayableHandle handle, float time )
        {
            if( handle == PlayableHandle.Null )
                return;
            
            int outputCnt = graph.GetOutputCount();
            if( outputCnt == 0 )
                return;
            
            for( int i = 0; i < outputCnt; ++i )
            {
                UnityEngine.Playables.PlayableOutput output = graph.GetOutput(i);
                if( output.IsOutputValid() == false )
                    continue;
                
                _SetTimeToPlayable( output.GetSourcePlayable(), handle, time );
            }
        }
        
        void _SetTimeToPlayable( Playable playable, PlayableHandle handle, float time )
        {
            if( playable.IsValid() == false )
                return ;
            
            if( playable.GetHandle().Equals( handle ) )
            {
                playable.SetTime( time );
            }
            
            int inputCnt = playable.GetInputCount();
            if( inputCnt > 0 )
            {
                for( int i = 0; i < inputCnt; ++i )
                {
                    _SetTimeToPlayable( playable.GetInput( i ), handle, time );
                }
            }
        }
        
        #endif
    }
}
