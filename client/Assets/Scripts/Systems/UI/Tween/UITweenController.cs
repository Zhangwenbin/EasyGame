using UnityEngine;
using System.Collections;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;
using System.Linq;
#endif

namespace EG
{
    [AddComponentMenu("Scripts/System/UI/Tween/UITweenController")]
    public class UITweenController : UnityEngine.MonoBehaviour
    {
        [System.Serializable]
        public class Param
        {
            public string               key             = "";
            public bool                 includeInactive = false;
            public UITweener.GroupType  groupType       = UITweener.GroupType.Free;
            public string[]             groups          = new string[0];
        }
        
        public class Info
        {
            public Param                param           = null;
            public UITweenList          list            = new UITweenList( );
            
            public void Initialize( GameObject root )
            {
                if( param != null )
                {
                    list.Initialize( root, param.includeInactive, param.groupType, param.groups );
                }
                else
                {
                    list.Clear( );
                }
            }
        }
        
 
        [SerializeField] List<Param>    m_Params        = new List<Param>( );
        
        private Dictionary<string,Info> m_Infos         = new Dictionary<string,Info>( );
        
        public List<Param>              list            { get { return m_Params;        } }
        public int                      count           { get { return m_Params.Count;  } }
        
        public UITweenList              this[string key]
        {
            get
            {
                if( string.IsNullOrEmpty( key ) == false )
                {
                    #if UNITY_EDITOR
                    if( Application.isPlaying == false )
                    {
                        for( int i = 0; i < m_Params.Count; ++i )
                        {
                            if( m_Params[i].key == key )
                            {
                                UITweenList result = new UITweenList( );
                                result.Initialize( gameObject, m_Params[i].groupType, m_Params[i].groups );
                                return result;
                            }
                        }
                    }
                    #endif
                    Info info = null;
                    if( m_Infos.TryGetValue( key, out info ) )
                    {
                        return info.list;
                    }
                }
                return null;
            }
        }
        
 
        #if UNITY_EDITOR
        
        string m_EditorKey = "";
        string m_EditorPlayKey = "";
        

        public void OnGUIInspect( )
        {
            if( gameObject == null ) return;
            
            GameObject gobj = gameObject;
            
            EditorGUIUtility.labelWidth = 60f;
            
            EditorGUILayout.BeginVertical( "box" );
            {
                EditorGUILayout.BeginHorizontal( );
                {
                    bool hasKey = GetParam( m_EditorKey ) != null;
                    
                    if( hasKey ) GUI.contentColor = Color.red;
                    else         GUI.contentColor = Color.white;
                    m_EditorKey = EditorGUILayout.TextField( m_EditorKey );
                    
                    GUI.contentColor = Color.white;
                    GUI.enabled = !hasKey;
                    if( GUILayout.Button( "追加", GUILayout.Width( 60f ) ) )
                    {
                        if( gobj != null ) UnityEditor.Undo.RecordObjects( new UnityEngine.Object[]{ gobj }, "TweenController" );
                        Add( m_EditorKey );
                        if( gobj != null ) EditorUtility.SetDirty( gobj );
                        EditorHelp.ClearKeyControll( );
                    }
                    GUI.enabled = true;
                }
                EditorGUILayout.EndHorizontal( );
            }
            EditorGUILayout.EndVertical();
            
            if( count == 0 )
            {
                return;
            }
            
            EditorGUILayout.BeginVertical( "box" );
            {
                UITweenController.Param[] array = list.ToArray( );
                for( int i = 0; i < array.Length; ++i )
                {
                    EditorGUILayout.BeginHorizontal();
                    {
                        {
                            if( i == 0 ) GUI.enabled = false;
                            else         GUI.enabled = true;
                            if( GUILayout.Button( "↑", GUILayout.Width( 15 ) ) )
                            {
                                if( gobj != null ) UnityEditor.Undo.RecordObjects( new UnityEngine.Object[]{ gobj }, "TweenController" );
                                UITweenController.Param bak = array[ i - 1 ];
                                array[ i - 1 ] = array[ i ];
                                array[ i ] = bak;
                                list.Clear( );
                                list.AddRange( array );
                                if( gobj != null ) EditorUtility.SetDirty( gobj );
                                return;
                            }
                            if( i == array.Length - 1 ) GUI.enabled = false;
                            else                        GUI.enabled = true;
                            if( GUILayout.Button( "↓", GUILayout.Width( 15 ) ) )
                            {
                                if( gobj != null ) UnityEditor.Undo.RecordObjects( new UnityEngine.Object[]{ gobj }, "TweenController" );
                                UITweenController.Param bak = array[ i + 1 ];
                                array[ i + 1 ] = array[ i ];
                                array[ i ] = bak;
                                list.Clear( );
                                list.AddRange( array );
                                if( gobj != null ) EditorUtility.SetDirty( gobj );
                                return;
                            }
                            GUI.enabled = true;
                        }
                        GUI.contentColor = Color.white;
                        OnGUIInspect( array[i] );
                    }
                    GUI.enabled = true;
                    EditorGUILayout.EndHorizontal();
                }
            }
            EditorGUILayout.EndVertical();
        }
        
        private void OnGUIInspect( UITweenController.Param param )
        {
            GameObject gobj = gameObject;
            
            EditorGUILayout.BeginVertical( "box" );
            {
                EditorHelp.HeadStart( param.key );
                 
                EditorGUILayout.BeginHorizontal();
                {
                    EditorGUILayout.LabelField( "includeInactive" );
                    bool includeInactive = EditorGUILayout.ToggleLeft( "", param.includeInactive );
                    if( includeInactive != param.includeInactive )
                    {
                        if( gobj != null ) UnityEditor.Undo.RecordObjects( new UnityEngine.Object[]{ gobj }, "TweenController" );
                        param.includeInactive = includeInactive;
                    }
                }
                EditorGUILayout.EndHorizontal( );
                
                EditorGUILayout.BeginHorizontal();
                {
                    EditorGUILayout.LabelField( "GroupType" );
                    UITweener.GroupType groupType = (UITweener.GroupType)EditorGUILayout.EnumPopup( param.groupType );
                    if( groupType != param.groupType )
                    {
                        if( gobj != null ) UnityEditor.Undo.RecordObjects( new UnityEngine.Object[]{ gobj }, "TweenController" );
                        param.groupType = groupType;
                    }
                }
                EditorGUILayout.EndHorizontal( );
                
                EditorGUILayout.BeginVertical( "box" );
                {
                    List<string> groupList = new List<string>( param.groups );
                    EditorGUILayout.BeginHorizontal();
                    {
                        EditorGUILayout.LabelField( "groupList" );
                        if( GUILayout.Button( "追加" ) )
                        {
                            if( gobj != null ) UnityEditor.Undo.RecordObjects( new UnityEngine.Object[]{ gobj }, "TweenController" );
                            groupList.Add( "" );
                        }
                    }
                    EditorGUILayout.EndHorizontal( );
                    int removeIndex = -1;
                    for( int i = 0; i < groupList.Count; ++i )
                    {
                        EditorGUILayout.BeginHorizontal();
                        {
                            string groupName = EditorGUILayout.TextField( groupList[ i ] );
                            if( groupName != groupList[ i ] )
                            {
                                groupList[ i ] = groupName;
                            }
                            if( GUILayout.Button( "×", GUILayout.Width( 30f ) ) )
                            {
                                if( gobj != null ) UnityEditor.Undo.RecordObjects( new UnityEngine.Object[]{ gobj }, "TweenController" );
                                removeIndex = i;
                            }
                        }
                        EditorGUILayout.EndHorizontal( );
                    }
                    if( removeIndex != -1 ) groupList.RemoveAt( removeIndex );
                    param.groups = groupList.ToArray( );
                }
                EditorGUILayout.EndVertical();
                
                if( Application.isPlaying )
                {
                    EditorGUILayout.BeginHorizontal();
                    
                    UITweenList list = this[ param.key ];
                    if( list != null )
                    {
                        switch( list.GroupType )
                        {
                        case UITweener.GroupType.Free:
                            {
                                string[] names = list.Groups.Select( ( prop ) => prop.id ).ToArray( );
                                int groupValue = ArrayUtility.FindIndex<string>( names, ( prop ) => prop == m_EditorPlayKey );
                                if( groupValue == -1 ){ groupValue = 0; m_EditorPlayKey = names[ 0 ]; }
                                int next = EditorGUILayout.Popup( "", groupValue, names );
                                if( next != groupValue )
                                {
                                    m_EditorPlayKey = names[ next ];
                                }
                            }
                            break;
                        case UITweener.GroupType.Window:
                            {
                                string[] names = System.Enum.GetNames( typeof( UITweener.WindowGroup ) );
                                int groupValue = ArrayUtility.FindIndex<string>( names, ( prop ) => prop == m_EditorPlayKey );
                                if( groupValue == -1 ){ groupValue = 0; m_EditorPlayKey = names[ 0 ]; }
                                int next = EditorGUILayout.Popup( "", groupValue, names );
                                if( next != groupValue )
                                {
                                    m_EditorPlayKey = names[ next ];
                                }
                            }
                            break;
                        case UITweener.GroupType.Event:
                            {
                                string[] names = System.Enum.GetNames( typeof(UITweener.EventGroup) );
                                int groupValue = ArrayUtility.FindIndex<string>( names, ( prop ) => prop == m_EditorPlayKey );
                                if( groupValue == -1 ){ groupValue = 0; m_EditorPlayKey = names[ 0 ]; }
                                int next = EditorGUILayout.Popup( "", groupValue, names );
                                if( next != groupValue )
                                {
                                    m_EditorPlayKey = names[ next ];
                                }
                            }
                            break;
                        }

                        if( GUILayout.Button( "Play", GUILayout.Width( 70f ) ) )
                        {
                            list.Play( m_EditorPlayKey, true, 1.0f );
                        }
                    }
                    
                    EditorGUILayout.EndHorizontal( );
                }
                
                EditorHelp.HeadEnd( );
            }
            EditorGUILayout.EndVertical();
        }
        
        #endif

        void Awake( )
        {
            Setup( );
        }
        
        public void Setup( )
        {
            if( m_Infos.Count > 0 ) return;
            
            for( int i = 0; i < m_Params.Count; ++i )
            {
                Info info = new Info( );
                info.param = m_Params[ i ];
                info.Initialize( gameObject );
                m_Infos.Add( info.param.key, info );
            }
        }
        
        public void Clear( )
        {
            m_Params.Clear( );
        }
        

        public Param Add( string key )
        {
            Param param = new Param( );
            param.key = key;
            m_Params.Add( param );
            return param;
        }
        
      
        public void Remove( string key )
        {
            Param param = GetParam( key );
            if( param != null )
            {
                m_Params.Remove( param );
            }
        }
        
    
        public Param GetParam( string key )
        {
            return m_Params.Find( ( prop ) => prop.key == key );
        }
        

        public void Reset( string grpId )
        {
            foreach( var pair in m_Infos )
            {
                pair.Value.list.Reset( grpId );
            }
        }
        public void Reset( UITweener.WindowGroup grpId )
        {
            foreach( var pair in m_Infos )
            {
                pair.Value.list.Reset( grpId );
            }
        }
        public void Reset( UITweener.EventGroup grpId )
        {
            foreach( var pair in m_Infos )
            {
                pair.Value.list.Reset( grpId );
            }
        }
        
    
        public void Play( string grpId, bool forward, float speed )
        {
            foreach( var pair in m_Infos )
            {
                pair.Value.list.Play( grpId, forward, speed );
            }
        }
        public void Play( UITweener.WindowGroup grpId, bool forward, float speed )
        {
            foreach( var pair in m_Infos )
            {
                pair.Value.list.Play( grpId, forward, speed );
            }
        }
        public void Play( UITweener.EventGroup grpId, bool forward, float speed )
        {
            foreach( var pair in m_Infos )
            {
                pair.Value.list.Play( grpId, forward, speed );
            }
        }
        
  
        public void Stop( bool reset )
        {
            foreach( var pair in m_Infos )
            {
                pair.Value.list.Stop( reset );
            }
        }
        
        public bool IsEnable( string grpId )
        {
            bool result = true;
            foreach( var pair in m_Infos )
            {
                result |= pair.Value.list.IsEnable( grpId );
            }
            return result;
        }
        public bool IsEnable( UITweener.WindowGroup grpId )
        {
            bool result = false;
            foreach( var pair in m_Infos )
            {
                result |= pair.Value.list.IsEnable( grpId );
            }
            return result;
        }
        public bool IsEnable( UITweener.EventGroup grpId )
        {
            bool result = true;
            foreach( var pair in m_Infos )
            {
                result |= pair.Value.list.IsEnable( grpId );
            }
            return result;
        }
        
        public bool IsDisable( string grpId )
        {
            return !IsEnable( grpId );
        }
        public bool IsDisable( UITweener.WindowGroup grpId )
        {
            return !IsEnable( grpId );
        }
        public bool IsDisable( UITweener.EventGroup grpId )
        {
            return !IsEnable( grpId );
        }
        
    }
    
    #if UNITY_EDITOR
    [UnityEditor.CustomEditor(typeof(UITweenController))]
    public class EditorInspector_UITweenController : UnityEditor.Editor
    {
        string m_EditorKey = "";
        string m_EditorPlayKey = "";
        
        public override void OnInspectorGUI( )
        {
            UITweenController controller = target as UITweenController;
            GameObject gobj = controller.gameObject;
            
            EditorGUIUtility.labelWidth = 60f;
            
            EditorGUILayout.BeginVertical( "box" );
            {
                // 
                EditorGUILayout.BeginHorizontal( );
                {
                    bool hasKey = controller.GetParam( m_EditorKey ) != null;
                    
                    if( hasKey ) GUI.contentColor = Color.red;
                    else         GUI.contentColor = Color.white;
                    m_EditorKey = EditorGUILayout.TextField( m_EditorKey );
                    
                    GUI.contentColor = Color.white;
                    GUI.enabled = !hasKey;
                    if( GUILayout.Button( "追加", GUILayout.Width( 60f ) ) )
                    {
                        if( gobj != null ) UnityEditor.Undo.RecordObjects( new UnityEngine.Object[]{ gobj }, "TweenController" );
                        controller.Add( m_EditorKey );
                        if( gobj != null ) EditorUtility.SetDirty( gobj );
                        EditorHelp.ClearKeyControll( );
                    }
                    GUI.enabled = true;
                }
                EditorGUILayout.EndHorizontal( );
            }
            EditorGUILayout.EndVertical();
            
            if( controller.count == 0 )
            {
                return;
            }
            
            EditorGUILayout.BeginVertical( "box" );
            {
                UITweenController.Param[] array = controller.list.ToArray( );
                for( int i = 0; i < array.Length; ++i )
                {
                    EditorGUILayout.BeginHorizontal();
                    {
                        {
                            if( i == 0 ) GUI.enabled = false;
                            else         GUI.enabled = true;
                            if( GUILayout.Button( "↑", GUILayout.Width( 15 ) ) )
                            {
                                if( gobj != null ) UnityEditor.Undo.RecordObjects( new UnityEngine.Object[]{ gobj }, "TweenController" );
                                UITweenController.Param bak = array[ i - 1 ];
                                array[ i - 1 ] = array[ i ];
                                array[ i ] = bak;
                                controller.list.Clear( );
                                controller.list.AddRange( array );
                                if( gobj != null ) EditorUtility.SetDirty( gobj );
                                return;
                            }
                            if( i == array.Length - 1 ) GUI.enabled = false;
                            else                        GUI.enabled = true;
                            if( GUILayout.Button( "↓", GUILayout.Width( 15 ) ) )
                            {
                                if( gobj != null ) UnityEditor.Undo.RecordObjects( new UnityEngine.Object[]{ gobj }, "TweenController" );
                                UITweenController.Param bak = array[ i + 1 ];
                                array[ i + 1 ] = array[ i ];
                                array[ i ] = bak;
                                controller.list.Clear( );
                                controller.list.AddRange( array );
                                if( gobj != null ) EditorUtility.SetDirty( gobj );
                                return;
                            }
                            GUI.enabled = true;
                        }
                        GUI.contentColor = Color.white;
                        OnGUIInspect( gobj, array[i] );
                    }
                    GUI.enabled = true;
                    EditorGUILayout.EndHorizontal();
                }
            }
            EditorGUILayout.EndVertical();
        }
        
        private void OnGUIInspect( GameObject gobj, UITweenController.Param param )
        {
            UITweenController controller = target as UITweenController;
            
            EditorGUILayout.BeginVertical( "box" );
            {
                EditorHelp.HeadStart( param.key );
                 
                EditorGUILayout.BeginHorizontal();
                {
                    EditorGUILayout.LabelField( "includeInactive" );
                    bool includeInactive = EditorGUILayout.ToggleLeft( "", param.includeInactive );
                    if( includeInactive != param.includeInactive )
                    {
                        if( gobj != null ) UnityEditor.Undo.RecordObjects( new UnityEngine.Object[]{ gobj }, "TweenController" );
                        param.includeInactive = includeInactive;
                    }
                }
                EditorGUILayout.EndHorizontal( );
                
                EditorGUILayout.BeginHorizontal();
                {
                    EditorGUILayout.LabelField( "GroupType" );
                    UITweener.GroupType groupType = (UITweener.GroupType)EditorGUILayout.EnumPopup( param.groupType );
                    if( groupType != param.groupType )
                    {
                        if( gobj != null ) UnityEditor.Undo.RecordObjects( new UnityEngine.Object[]{ gobj }, "TweenController" );
                        param.groupType = groupType;
                    }
                }
                EditorGUILayout.EndHorizontal( );
                
                EditorGUILayout.BeginVertical( "box" );
                {
                    List<string> groupList = new List<string>( param.groups );
                    EditorGUILayout.BeginHorizontal();
                    {
                        EditorGUILayout.LabelField( "groupList" );
                        if( GUILayout.Button( "追加" ) )
                        {
                            if( gobj != null ) UnityEditor.Undo.RecordObjects( new UnityEngine.Object[]{ gobj }, "TweenController" );
                            groupList.Add( "" );
                        }
                    }
                    EditorGUILayout.EndHorizontal( );
                    int removeIndex = -1;
                    for( int i = 0; i < groupList.Count; ++i )
                    {
                        EditorGUILayout.BeginHorizontal();
                        {
                            string groupName = EditorGUILayout.TextField( groupList[ i ] );
                            if( groupName != groupList[ i ] )
                            {
                                groupList[ i ] = groupName;
                            }
                            if( GUILayout.Button( "×", GUILayout.Width( 30f ) ) )
                            {
                                if( gobj != null ) UnityEditor.Undo.RecordObjects( new UnityEngine.Object[]{ gobj }, "TweenController" );
                                removeIndex = i;
                            }
                        }
                        EditorGUILayout.EndHorizontal( );
                    }
                    if( removeIndex != -1 ) groupList.RemoveAt( removeIndex );
                    param.groups = groupList.ToArray( );
                }
                EditorGUILayout.EndVertical();
                
                if( Application.isPlaying )
                {
                    EditorGUILayout.BeginHorizontal();
                    
                    UITweenList list = controller[ param.key ];
                    if( list != null )
                    {
                        switch( list.GroupType )
                        {
                        case UITweener.GroupType.Free:
                            {
                                string[] names = list.Groups.Select( ( prop ) => prop.id ).ToArray( );
                                int groupValue = ArrayUtility.FindIndex<string>( names, ( prop ) => prop == m_EditorPlayKey );
                                if( groupValue == -1 ){ groupValue = 0; m_EditorPlayKey = names[ 0 ]; }
                                int next = EditorGUILayout.Popup( "", groupValue, names );
                                if( next != groupValue )
                                {
                                    m_EditorPlayKey = names[ next ];
                                }
                            }
                            break;
                        case UITweener.GroupType.Window:
                            {
                                string[] names = System.Enum.GetNames( typeof( UITweener.WindowGroup ) );
                                int groupValue = ArrayUtility.FindIndex<string>( names, ( prop ) => prop == m_EditorPlayKey );
                                if( groupValue == -1 ){ groupValue = 0; m_EditorPlayKey = names[ 0 ]; }
                                int next = EditorGUILayout.Popup( "", groupValue, names );
                                if( next != groupValue )
                                {
                                    m_EditorPlayKey = names[ next ];
                                }
                            }
                            break;
                        case UITweener.GroupType.Event:
                            {
                                string[] names = System.Enum.GetNames( typeof(UITweener.EventGroup) );
                                int groupValue = ArrayUtility.FindIndex<string>( names, ( prop ) => prop == m_EditorPlayKey );
                                if( groupValue == -1 ){ groupValue = 0; m_EditorPlayKey = names[ 0 ]; }
                                int next = EditorGUILayout.Popup( "", groupValue, names );
                                if( next != groupValue )
                                {
                                    m_EditorPlayKey = names[ next ];
                                }
                            }
                            break;
                        }
                        
                        if( GUILayout.Button( "Play", GUILayout.Width( 70f ) ) )
                        {
                            list.Play( m_EditorPlayKey, true, 1.0f );
                        }
                    }
                    
                    EditorGUILayout.EndHorizontal( );
                }
                
                EditorHelp.HeadEnd( );
            }
            EditorGUILayout.EndVertical();
        }
    }
    #endif
}
