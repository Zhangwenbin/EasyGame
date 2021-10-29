#if DEBUG_BUILD && UNITY_EDITOR
#define STACK_HISTROY
#endif
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using TMPro;
#if STACK_HISTROY
using System.Runtime.CompilerServices;
#endif

#if UNITY_EDITOR
using UnityEditor;
using UnityEditorInternal;
#endif

namespace EG
{
    [System.Serializable]
    public class SerializeValueList
    {
        [System.Serializable]
        public class GroupDefine
        {
            public int                          id              = 0;
            public string                       name            = "";
            
            public GroupDefine Clone( )
            {
                GroupDefine result = new GroupDefine( );
                result.id   = id;
                result.name = name;
                return result;
            }
        }
        

        [SerializeField] List<SerializeValue>   m_Fields        = new List<SerializeValue>();
        [SerializeField] List<GroupDefine>      m_GroupDefine   = new List<GroupDefine>( );
        
        
        public SerializeValue                   this[ int i ]   { get { return m_Fields[ i ]; } }
        
        public List<SerializeValue>             list            { get { return m_Fields;        } }
        public int                              Count           { get { return m_Fields.Count;  } }
        
        public SerializeValue[]                 ToArray()       { return m_Fields.ToArray();    }
        
        public List<string>                     GetKeys()
        {
            List<string> result = new List<string>();
            for( int i = 0; i < m_Fields.Count; ++i )
            {
                if( result.FindIndex( prop => prop == m_Fields[ i ].key ) == -1 )
                {
                    result.Add( m_Fields[ i ].key );
                }
            }
            return result;
        }
        
        public List<int>                        GetGroups()
        {
            List<int> result = new List<int>();
            for( int i = 0; i < m_Fields.Count; ++i )
            {
                if( result.FindIndex( prop => prop == m_Fields[ i ].group ) == -1 )
                {
                    result.Add( m_Fields[ i ].group );
                }
            }
            return result;
        }
        
        public List<string>                     GetGroupDefineNames()
        {
            List<string> result = new List<string>();
            for( int i = 0; i < m_GroupDefine.Count; ++i )
            {
                result.Add( m_GroupDefine[ i ].name );
            }
            return result;
        }
        
        
        #if UNITY_EDITOR
        protected string                m_EditorKey = "";
        protected int                   m_EditorGroupId = 0;
        protected string                m_EditorGroupName = "";
        protected int                   m_EditorSelectGroup = 0;
        protected int                   m_EditorSelectGroupId = 0;
        protected int                   m_EditorList = 0;
        protected bool                  m_EditorMakeDefine = false;
        protected int                   m_EditorMakeDefineList = 0;
        protected SerializeValue.Type   m_EditorSelectValue = SerializeValue.Type.NONE;
        
        private ReorderableList         m_ReorderableList;
        private Rect                    m_HeaderRect;
        private List<SerializeValue>    m_DrawList = new List<SerializeValue>( );
        

        public void OnGUIInspect( UnityEngine.Object obj, bool isReadOnly )
        {
            OnGUIInspect( obj, null, isReadOnly );
        }
        public void OnGUIInspect( UnityEngine.Object obj )
        {
            OnGUIInspect( obj, null, false );
        }
        public void OnGUIInspect( UnityEngine.Object obj, string[] skipKeys, bool isReadOnly )
        {
            if( m_ReorderableList == null )
            {
                m_ReorderableList = new ReorderableList( m_Fields, typeof(SerializeValue), false, true, false, false );
                
                m_ReorderableList.showDefaultBackground = false;
                
                // 
                m_ReorderableList.drawHeaderCallback = ( Rect rect ) =>
                {
                    m_HeaderRect = rect;
                    GUI.contentColor = new Color( 0.9f, 0.9f, 0.9f );
                    EditorGUI.LabelField( rect, "变量列表", EditorHelp.GUIStyles.GroupSubTitleLeft );
                    GUI.contentColor = Color.white;
                    GUI.backgroundColor = Color.white;
                    GUI.color = Color.white;
                };
                
                // 
                m_ReorderableList.drawElementCallback = ( Rect rect, int index, bool isActive, bool isFocused ) => 
                {
                    if( index >= 0 && index < m_ReorderableList.list.Count )
                    {
                        SerializeValue value = m_ReorderableList.list[ index ] as SerializeValue;
                        GUI.enabled = !isReadOnly;
                        value.OnGUIInspect( rect, (SerializeValue.EditMode)this.m_EditorList, obj, this );
                        GUI.enabled = true;
                    }
                };
                
                // 
                m_ReorderableList.drawFooterCallback = ( rect ) =>
                {
                    // rect.y = m_HeaderRect.y + 3;
                    // ReorderableList.defaultBehaviours.DrawFooter( rect, m_ReorderableList );
                };
                
                // 
                m_ReorderableList.onReorderCallback = ( ReorderableList reorderableList ) =>
                {
                    IList list = reorderableList.list;
                    if( m_Fields.Count == list.Count )
                    {
                        for( int i = 0; i < list.Count; ++i )
                        {
                            m_Fields[i] = list[i] as SerializeValue;
                        }
                    }
                };
            }
            
            // ？
            if( isReadOnly == false )
            {
                EditorHelp.SubTitle( "属性变更" );
                
                List<int> groups = GetGroups( );
                
                // 
                EditorGUILayout.BeginHorizontal( );
                {
                    groups.Sort( ( p1, p2 ) => p1.CompareTo( p2 ) );
                    int currentIndex = groups.FindIndex( ( prop ) => prop == 0 );
                    if( currentIndex == -1 ){ currentIndex = 0; groups.Insert( 0, 0 ); }
                    List<string> groupNameList = GetGroupDefineNames( );
                    string[] groupName = new string[ groupNameList.Count+1 ];
                    groupName[ 0 ] = "all";
                    for( int i = 0; i < groupNameList.Count; ++i ) groupName[ 1+i ] = groupNameList[i];
                    EditorGUILayout.LabelField( "group", GUILayout.Width(50f) );
                    m_EditorSelectGroup = EditorGUILayout.Popup( m_EditorSelectGroup, groupName );
                    if( m_EditorSelectGroup >= groupName.Length ) m_EditorSelectGroup = 0;
                    GroupDefine def = GetGroupDefine( groupName[ m_EditorSelectGroup ] );
                    if( def != null ) m_EditorSelectGroupId = def.id;
                    else m_EditorSelectGroupId = 0;
                    if( GUILayout.Button( "定义", GUILayout.Width(80f) ) )
                    {
                        m_EditorMakeDefine = !m_EditorMakeDefine;
                    }
                    if( GUILayout.Button( "排序", GUILayout.Width(80f) ) )
                    {
                        if( obj != null ) UnityEditor.Undo.RecordObjects( new UnityEngine.Object[]{ obj }, "SerializeValueList" );
                        CollectionUtility.StableSort<SerializeValue>( m_Fields, ( p1, p2 ) => p1.group.CompareTo( p2.group ) );
                        if( obj != null ) EditorUtility.SetDirty( obj );
                        return;
                    }
                    GUI.enabled = true;
                }
                EditorGUILayout.EndHorizontal( );
                
                if( m_EditorMakeDefine )
                {
                    EditorGUILayout.BeginVertical( "box" );
                    {
                        EditorGUILayout.BeginHorizontal( );
                        {
                            EditorGUILayout.LabelField( "GroupId", GUILayout.Width(70f) );
                            m_EditorGroupId = EditorGUILayout.IntField( m_EditorGroupId );
                            GUI.enabled = m_GroupDefine.Count > 0;
                            m_EditorMakeDefineList = EditorGUILayout.Popup( m_EditorMakeDefineList, new string[]{ "隐藏", "显示" }, GUILayout.Width( 60f ) );
                            GUI.enabled = true;
                        }
                        EditorGUILayout.EndHorizontal( );
                        EditorGUILayout.BeginHorizontal( );
                        {
                            EditorGUILayout.LabelField( "GroupName", GUILayout.Width(70f) );
                            m_EditorGroupName = EditorGUILayout.TextField( m_EditorGroupName );
                            GUI.enabled = m_EditorGroupId != 0 && !HasGroupDefine( m_EditorGroupId );
                            if( GUILayout.Button( "建组", GUILayout.Width(80) ) )
                            {
                                if( obj != null ) UnityEditor.Undo.RecordObjects( new UnityEngine.Object[]{ obj }, "SerializeValueList" );
                                GroupDefine def = new GroupDefine( );
                                def.id = m_EditorGroupId;
                                def.name = m_EditorGroupName;
                                m_GroupDefine.Add( def );
                                m_GroupDefine.Sort( ( p1, p2 ) => p1.id.CompareTo( p2.id ) );
                                if( obj != null ) EditorUtility.SetDirty( obj );
                                m_EditorMakeDefineList = 1;
                            }
                            GUI.enabled = true;
                        }
                        EditorGUILayout.EndHorizontal( );
                        // 一覧
                        if( m_EditorMakeDefineList == 1 )
                        {
                            EditorGUILayout.BeginVertical( "box" );
                            {
                                for( int i = 0; i < m_GroupDefine.Count; ++i )
                                {
                                    EditorGUILayout.BeginHorizontal( );
                                    {
                                        EditorGUILayout.LabelField( "ID:"+m_GroupDefine[i].id, GUILayout.Width(50f) );
                                        string nextName = EditorGUILayout.TextField( m_GroupDefine[i].name );
                                        if( nextName != m_GroupDefine[ i ].name )
                                        {
                                            if( obj != null ) UnityEditor.Undo.RecordObjects( new UnityEngine.Object[]{ obj }, "SerializeValueList" );
                                            m_GroupDefine[ i ].name = nextName;
                                            if( obj != null ) EditorUtility.SetDirty( obj );
                                        }
                                        if( GUILayout.Button( "×", GUILayout.Width(30f) ) )
                                        {
                                            if( obj != null ) UnityEditor.Undo.RecordObjects( new UnityEngine.Object[]{ obj }, "SerializeValueList" );
                                            m_GroupDefine.RemoveAt( i );
                                            --i;
                                            if( obj != null ) EditorUtility.SetDirty( obj );
                                        }
                                    }
                                    EditorGUILayout.EndHorizontal( );
                                }
                            }
                            EditorGUILayout.EndVertical( );
                        }
                    }
                    EditorGUILayout.EndVertical( );
                }
            }
            
            // 
            if( isReadOnly == false )
            {
                EditorGUILayout.BeginHorizontal( );
                {
                    if( GetField( m_EditorKey ) != null ) GUI.contentColor = Color.red;
                    else                                  GUI.contentColor = Color.white;
                    m_EditorKey = EditorGUILayout.TextField( m_EditorKey );
                    GUI.contentColor = Color.white;
                    int nextValue = EditorGUILayout.Popup( m_EditorList, new string[]{ "简略", "详细" }, GUILayout.Width( 60f ) );
                    if( nextValue != m_EditorList )
                    {
                        m_EditorList = nextValue;
                    }
                }
                EditorGUILayout.EndHorizontal( );
                
                EditorGUILayout.BeginHorizontal( );
                {
                    m_EditorSelectValue = (SerializeValue.Type)EditorGUILayout.EnumPopup( m_EditorSelectValue );
                    if( m_EditorSelectValue == SerializeValue.Type.NONE )
                    {
                        GUI.enabled = false;
                    }
                    else
                    {
                        GUI.enabled = true;
                    }
                    if( GUILayout.Button( "添加" ) )
                    {
                        if( obj != null ) UnityEditor.Undo.RecordObjects( new UnityEngine.Object[]{ obj }, "SerializeValueList" );
                        SerializeValue value = NewField( m_EditorSelectValue, m_EditorKey );
                        value.group = m_EditorSelectGroupId;
                        if( obj != null ) EditorUtility.SetDirty( obj );
                        EditorHelp.ClearKeyControll( );
                    }
                    GUI.enabled = true;
                }
                EditorGUILayout.EndHorizontal( );
            }
            
            // 
            if( m_EditorSelectGroupId > 0 )
            {
                EditorGUILayout.BeginHorizontal( );
                {
                    EditorGUILayout.LabelField( "GameObejct", GUILayout.Width(90f) );
                    if( GUILayout.Button( "全ON", GUILayout.Width(50f) ) )
                    {
                        for( int i = 0; i < m_Fields.Count; ++i )
                        {
                            if( m_Fields[ i ].group == m_EditorSelectGroupId )
                            {
                                GameObject gobj = m_Fields[ i ].v_GameObject;
                                if( gobj != null )
                                {
                                    gobj.SetActive( true );
                                }
                            }
                        }
                    }
                    if( GUILayout.Button( "全OFF", GUILayout.Width(50f) ) )
                    {
                        for( int i = 0; i < m_Fields.Count; ++i )
                        {
                            if( m_Fields[ i ].group == m_EditorSelectGroupId )
                            {
                                GameObject gobj = m_Fields[ i ].v_GameObject;
                                if( gobj != null )
                                {
                                    gobj.SetActive( false );
                                }
                            }
                        }
                    }
                }
                EditorGUILayout.EndHorizontal( );
            }
            
            // 
            m_DrawList.Clear( );
            if( m_EditorSelectGroupId == 0 )
            {
                m_ReorderableList.draggable = isReadOnly == false;
                m_DrawList.AddRange( m_Fields );
            }
            else
            {
                m_ReorderableList.draggable = false;
                for( int i = 0; i < m_Fields.Count; ++i )
                {
                    if( m_Fields[ i ].group == m_EditorSelectGroupId )
                    {
                        m_DrawList.Add( m_Fields[i] );
                    }
                }
            }
            
            // 
            m_ReorderableList.list = m_DrawList;
            
            // 
            if( m_EditorList == 1 )
            {
                m_ReorderableList.elementHeight = EditorGUIUtility.singleLineHeight * 2 + 4;
            }
            else
            {
                m_ReorderableList.elementHeight = EditorGUIUtility.singleLineHeight * 1 + 4;
            }
            
            // 
            m_ReorderableList.DoLayoutList( );
        }
        
        #endif
        

        public SerializeValueList()
        {
        }
        // 
        public SerializeValueList( SerializeValueList list )
        {
            Write( list );
        }
        // 
        public SerializeValueList( List<SerializeValue> array )
        {
            m_Fields = array;
        }
        // 
        public SerializeValueList( SerializeValue[] array )
        {
            m_Fields.AddRange( array );
        }
        
        
        public void Initialize()
        {
            for( int i = 0; i < m_Fields.Count; ++i )
            {
                SerializeValue.Reset( m_Fields[i] );
            }
        }
        

        public void Release( )
        {
            Clear( );
        }
        

        public void Clear( )
        {
            m_Fields.Clear( );
        }
        

        public void Reset( )
        {
            for( int i = 0; i < m_Fields.Count; ++i )
            {
                SerializeValue.Reset( m_Fields[i] );
            }
        }
        

        public void Save( )
        {
            for( int i = 0; i < m_Fields.Count; ++i )
            {
                m_Fields[i].Save();
            }
        }
        
 
        public SerializeValueList Clone( )
        {
            SerializeValueList result = new SerializeValueList( );
            
            // 
            for( int i = 0; i < m_GroupDefine.Count; ++i )
            {
                result.m_GroupDefine.Add( m_GroupDefine[i].Clone( ) );
            }
            
            // 
            for( int i = 0; i < m_Fields.Count; ++i )
            {
                result.m_Fields.Add( SerializeValue.CreateCopy( m_Fields[i] ) );
            }
            
            return result;
        }
        
  
        public void Write( SerializeValueList src )
        {
            if( src.list != null )
            {
                List<SerializeValue> srcValues = src.list;
                for( int i = 0; i < srcValues.Count; ++i )
                {
                    SetField( srcValues[ i ] );
                }
            }
        }
        
  
        public SerializeValueList GetDiff( SerializeValueList dst )
        {
            if( dst == null )
            {
                return Clone( );
            }
            
            SerializeValueList result = new SerializeValueList( );
            
            for( int i = 0; i < m_Fields.Count; ++i )
            {
                SerializeValue value = dst.GetField( m_Fields[i].key );
                if( value == null || value != null && value.Equal( m_Fields[i] ) == false )
                {
                    result.m_Fields.Add( SerializeValue.CreateCopy( m_Fields[i] ) );
                }
            }
            
            return result;
        }
        
  
        public bool Equal( SerializeValueList src )
        {
            SerializeValue[] values = GetFields( );
            if( values != null && values.Length > 0 )
            {
                bool result = true;
                for( int i = 0; i < values.Length; ++i )
                {
                    result &= values[ i ].Equal( src.GetField( values[ i ].key ) );
                }
                return result;
            }
            return false;
        }
        

        public bool EqualGreater( SerializeValueList src )
        {
            SerializeValue[] values = GetFields( );
            if( values != null && values.Length > 0 )
            {
                bool result = true;
                for( int i = 0; i < values.Length; ++i )
                {
                    result &= values[ i ].EqualGreater( src.GetField( values[ i ].key ) );
                }
                return result;
            }
            return false;
        }
        
  
        public bool EqualLess( SerializeValueList src )
        {
            SerializeValue[] values = GetFields( );
            if( values != null && values.Length > 0 )
            {
                bool result = true;
                for( int i = 0; i < values.Length; ++i )
                {
                    result &= values[ i ].EqualLess( src.GetField( values[ i ].key ) );
                }
                return result;
            }
            return false;
        }
        

        public void RemoveFieldAt( int index )
        {
            m_Fields.RemoveAt( index );
        }
        
 
        public void RemoveField( SerializeValue value )
        {
            m_Fields.Remove( value );
        }
        

        public void RemoveField( string key )
        {
            int index = CollectionUtility.FindIndexEquals<SerializeValue,string>( m_Fields, ( o ) => o.key, key );
            if( index != -1 )
            {
                m_Fields.RemoveAt( index );
            }
        }
        

        public void RemoveObject<T>( )
        {
            RemoveField( typeof(T).Name );
        }
        public void RemoveObject( System.Type type )
        {
            RemoveField( type.Name );
        }
        

        private SerializeValue NewField( SerializeValue.Type type, string key )
        {
            SerializeValue field = SerializeValue.CreateType( type, key, null );
            m_Fields.Add( field );
            return field;
        }
        

        #if STACK_HISTROY
        public void Add( SerializeValueList list, [CallerFilePath] string _file = "", [CallerLineNumber] int _line = -1 )
        #else
        public void Add( SerializeValueList list )
        #endif
        {
            List<SerializeValue> values = list.m_Fields;
            if( values != null )
            {
                for( int i = 0; i < values.Count; ++i )
                {
                    #if STACK_HISTROY
                    SetField( values[i], _file, _line );
                    #else
                    SetField( values[i] );
                    #endif
                }
            }
        }
        
 
        #if STACK_HISTROY
        public SerializeValue AddField( SerializeValue value, [CallerFilePath] string _file = "", [CallerLineNumber] int _line = -1 )
        #else
        public SerializeValue AddField( SerializeValue value )
        #endif
        {
            SerializeValue field = GetField( value.key );
            if( field == null )
            {
                #if STACK_HISTROY
                field = SerializeValue.CreateCopy( value, _file, _line );
                #else
                field = SerializeValue.CreateCopy( value );
                #endif
                m_Fields.Add( field );
            }
            return field;
        }
        

        #if STACK_HISTROY
        public SerializeValue AddField( SerializeValue.Type type, string key, [CallerFilePath] string _file = "", [CallerLineNumber] int _line = -1 )
        #else
        public SerializeValue AddField( SerializeValue.Type type, string key )
        #endif
        {
            SerializeValue field = GetField( key );
            if( field == null )
            {
                #if STACK_HISTROY
                field = SerializeValue.CreateType( type, key, null, _file, _line );
                #else
                field = SerializeValue.CreateType( type, key, null );
                #endif
                m_Fields.Add( field );
            }
            return field;
        }
        

        #if STACK_HISTROY
        public SerializeValue AddField( string key, bool obj, [CallerFilePath] string _file = "", [CallerLineNumber] int _line = -1 )
        #else
        public SerializeValue AddField( string key, bool obj )
        #endif
        {
            SerializeValue field = GetField( key );
            if( field == null )
            {
                #if STACK_HISTROY
                field = SerializeValue.CreateBool( key, obj, _file, _line );
                #else
                field = SerializeValue.CreateBool( key, obj );
                #endif
                m_Fields.Add( field );
            }
            return field;
        }
        

        #if STACK_HISTROY
        public SerializeValue AddField( string key, int obj, [CallerFilePath] string _file = "", [CallerLineNumber] int _line = -1 )
        #else
        public SerializeValue AddField( string key, int obj )
        #endif
        {
            SerializeValue field = GetField( key );
            if( field == null )
            {
                #if STACK_HISTROY
                field = SerializeValue.CreateInt( key, obj, _file, _line );
                #else
                field = SerializeValue.CreateInt( key, obj );
                #endif
                m_Fields.Add( field );
            }
            return field;
        }
        

        #if STACK_HISTROY
        public SerializeValue AddField( string key, long obj, [CallerFilePath] string _file = "", [CallerLineNumber] int _line = -1 )
        #else
        public SerializeValue AddField( string key, long obj )
        #endif
        {
            SerializeValue field = GetField( key );
            if( field == null )
            {
                #if STACK_HISTROY
                field = SerializeValue.CreateLong( key, obj, _file, _line );
                #else
                field = SerializeValue.CreateLong( key, obj );
                #endif
                m_Fields.Add( field );
            }
            return field;
        }
        

        #if STACK_HISTROY
        public SerializeValue AddField( string key, float obj, [CallerFilePath] string _file = "", [CallerLineNumber] int _line = -1 )
        #else
        public SerializeValue AddField( string key, float obj )
        #endif
        {
            SerializeValue field = GetField( key );
            if( field == null )
            {
                #if STACK_HISTROY
                field = SerializeValue.CreateFloat( key, obj, _file, _line );
                #else
                field = SerializeValue.CreateFloat( key, obj );
                #endif
                m_Fields.Add( field );
            }
            return field;
        }
        

        #if STACK_HISTROY
        public SerializeValue AddField( string key, string obj, [CallerFilePath] string _file = "", [CallerLineNumber] int _line = -1 )
        #else
        public SerializeValue AddField( string key, string obj )
        #endif
        {
            SerializeValue field = GetField( key );
            if( field == null )
            {
                #if STACK_HISTROY
                field = SerializeValue.CreateString( key, obj, _file, _line );
                #else
                field = SerializeValue.CreateString( key, obj );
                #endif
                m_Fields.Add( field );
            }
            return field;
        }
        
        #if STACK_HISTROY
        public SerializeValue AddField( string key, Vector2 obj, [CallerFilePath] string _file = "", [CallerLineNumber] int _line = -1 )
        #else
        public SerializeValue AddField( string key, Vector2 obj )
        #endif
        {
            SerializeValue field = GetField( key );
            if( field == null )
            {
                #if STACK_HISTROY
                field = SerializeValue.CreateVector2( key, obj, _file, _line );
                #else
                field = SerializeValue.CreateVector2( key, obj );
                #endif
                m_Fields.Add( field );
            }
            return field;
        }
        

        #if STACK_HISTROY
        public SerializeValue AddField( string key, Vector3 obj, [CallerFilePath] string _file = "", [CallerLineNumber] int _line = -1 )
        #else
        public SerializeValue AddField( string key, Vector3 obj )
        #endif
        {
            SerializeValue field = GetField( key );
            if( field == null )
            {
                #if STACK_HISTROY
                field = SerializeValue.CreateVector3( key, obj, _file, _line );
                #else
                field = SerializeValue.CreateVector3( key, obj );
                #endif
                m_Fields.Add( field );
            }
            return field;
        }
        

        #if STACK_HISTROY
        public SerializeValue AddField( string key, Vector4 obj, [CallerFilePath] string _file = "", [CallerLineNumber] int _line = -1 )
        #else
        public SerializeValue AddField( string key, Vector4 obj )
        #endif
        {
            SerializeValue field = GetField( key );
            if( field == null )
            {
                #if STACK_HISTROY
                field = SerializeValue.CreateVector4( key, obj, _file, _line );
                #else
                field = SerializeValue.CreateVector4( key, obj );
                #endif
                m_Fields.Add( field );
            }
            return field;
        }
        

        #if STACK_HISTROY
        public SerializeValue AddField( string key, Color obj, [CallerFilePath] string _file = "", [CallerLineNumber] int _line = -1 )
        #else
        public SerializeValue AddField( string key, Color obj )
        #endif
        {
            SerializeValue field = GetField( key );
            if( field == null )
            {
                #if STACK_HISTROY
                field = SerializeValue.CreateColor( key, obj, _file, _line );
                #else
                field = SerializeValue.CreateColor( key, obj );
                #endif
                m_Fields.Add( field );
            }
            return field;
        }
        

        #if STACK_HISTROY
        public SerializeValue AddField( string key, GameObject obj, [CallerFilePath] string _file = "", [CallerLineNumber] int _line = -1 )
        #else
        public SerializeValue AddField( string key, GameObject obj )
        #endif
        {
            SerializeValue field = GetField( key );
            if( field == null )
            {
                #if STACK_HISTROY
                field = SerializeValue.CreateGameObject( key, obj, _file, _line );
                #else
                field = SerializeValue.CreateGameObject( key, obj );
                #endif
                m_Fields.Add( field );
            }
            return field;
        }
        

        #if STACK_HISTROY
        public SerializeValue AddField( string key, UnityEngine.UI.Text obj, [CallerFilePath] string _file = "", [CallerLineNumber] int _line = -1 )
        #else
        public SerializeValue AddField( string key, UnityEngine.UI.Text obj )
        #endif
        {
            SerializeValue field = GetField( key );
            if( field == null )
            {
                #if STACK_HISTROY
                field = SerializeValue.CreateLabel( key, obj, _file, _line );
                #else
                field = SerializeValue.CreateLabel( key, obj );
                #endif
                m_Fields.Add( field );
            }
            return field;
        }
        

        #if STACK_HISTROY
        public SerializeValue AddField( string key, UnityEngine.UI.Button obj, [CallerFilePath] string _file = "", [CallerLineNumber] int _line = -1 )
        #else
        public SerializeValue AddField( string key, UnityEngine.UI.Button obj )
        #endif
        {
            SerializeValue field = GetField( key );
            if( field == null )
            {
                #if STACK_HISTROY
                field = SerializeValue.CreateButton( key, obj, _file, _line );
                #else
                field = SerializeValue.CreateButton( key, obj );
                #endif
                m_Fields.Add( field );
            }
            return field;
        }
        

        #if STACK_HISTROY
        public SerializeValue AddField( string key, UnityEngine.UI.Toggle obj, [CallerFilePath] string _file = "", [CallerLineNumber] int _line = -1 )
        #else
        public SerializeValue AddField( string key, UnityEngine.UI.Toggle obj )
        #endif
        {
            SerializeValue field = GetField( key );
            if( field == null )
            {
                #if STACK_HISTROY
                field = SerializeValue.CreateToggle( key, obj, _file, _line );
                #else
                field = SerializeValue.CreateToggle( key, obj );
                #endif
                m_Fields.Add( field );
            }
            return field;
        }
        

        #if STACK_HISTROY
        public SerializeValue AddGlobal( string key, string fieldName, object obj, [CallerFilePath] string _file = "", [CallerLineNumber] int _line = -1 )
        #else
        public SerializeValue AddGlobal( string key, string fieldName, object obj )
        #endif
        {
            SerializeValue field = GetField( key );
            if( field == null )
            {
                #if STACK_HISTROY
                field = SerializeValue.CreateGlobal( key, fieldName, obj, _file, _line );
                #else
                field = SerializeValue.CreateGlobal( key, fieldName, obj );
                #endif
                m_Fields.Add( field );
            }
            return field;
        }
        

        #if STACK_HISTROY
        public SerializeValue AddObject( string key, object obj, [CallerFilePath] string _file = "", [CallerLineNumber] int _line = -1 )
        #else
        public SerializeValue AddObject( string key, object obj )
        #endif
        {
            SerializeValue field = GetField( key );
            if( field == null )
            {
                #if STACK_HISTROY
                field = SerializeValue.CreateObject( key, obj, _file, _line );
                #else
                field = SerializeValue.CreateObject( key, obj );
                #endif
                m_Fields.Add( field );
            }
            return field;
        }
        
        #if STACK_HISTROY
        public void SetField( SerializeValue value, [CallerFilePath] string _file = "", [CallerLineNumber] int _line = -1 )
        #else
        public void SetField( SerializeValue value )
        #endif
        {
            SerializeValue field = GetField( value.key );
            if( field != null )
            {
                #if STACK_HISTROY
                SerializeValue.Write( field, value, _file, _line );
                #else
                SerializeValue.Write( field, value );
                #endif
            }
            else
            {
                #if STACK_HISTROY
                AddField( value, _file, _line );
                #else
                AddField( value );
                #endif
            }
        }
        
  
        #if STACK_HISTROY
        public void SetField( string key, bool value, [CallerFilePath] string _file = "", [CallerLineNumber] int _line = -1 )
        #else
        public void SetField( string key, bool value )
        #endif
        {
            SerializeValue field = GetField( key );
            if( field != null )
            {
                #if STACK_HISTROY
                SerializeValue.SetBool( field, value, _file, _line );
                #else
                SerializeValue.SetBool( field, value );
                #endif
            }
            else
            {
                #if STACK_HISTROY
                field = SerializeValue.CreateBool( key, value, _file, _line );
                #else
                field = SerializeValue.CreateBool( key, value );
                #endif
                m_Fields.Add( field );
            }
        }
        

        #if STACK_HISTROY
        public void SetField( string key, int value, [CallerFilePath] string _file = "", [CallerLineNumber] int _line = -1 )
        #else
        public void SetField( string key, int value )
        #endif
        {
            SerializeValue field = GetField( key );
            if( field != null )
            {
                #if STACK_HISTROY
                SerializeValue.SetInt( field, value, _file, _line );
                #else
                SerializeValue.SetInt( field, value );
                #endif
            }
            else
            {
                #if STACK_HISTROY
                field = SerializeValue.CreateInt( key, value, _file, _line );
                #else
                field = SerializeValue.CreateInt( key, value );
                #endif
                m_Fields.Add( field );
            }
        }
        

        #if STACK_HISTROY
        public void SetField( string key, long value, [CallerFilePath] string _file = "", [CallerLineNumber] int _line = -1 )
        #else
        public void SetField( string key, long value )
        #endif
        {
            SerializeValue field = GetField( key );
            if( field != null )
            {
                #if STACK_HISTROY
                SerializeValue.SetLong( field, value, _file, _line );
                #else
                SerializeValue.SetLong( field, value );
                #endif
            }
            else
            {
                #if STACK_HISTROY
                field = SerializeValue.CreateLong( key, value, _file, _line );
                #else
                field = SerializeValue.CreateLong( key, value );
                #endif
                m_Fields.Add( field );
            }
        }
        

        #if STACK_HISTROY
        public void SetField( string key, float value, [CallerFilePath] string _file = "", [CallerLineNumber] int _line = -1 )
        #else
        public void SetField( string key, float value )
        #endif
        {
            SerializeValue field = GetField( key );
            if( field != null )
            {
                #if STACK_HISTROY
                SerializeValue.SetFloat( field, value, _file, _line );
                #else
                SerializeValue.SetFloat( field, value );
                #endif
            }
            else
            {
                #if STACK_HISTROY
                field = SerializeValue.CreateFloat( key, value, _file, _line );
                #else
                field = SerializeValue.CreateFloat( key, value );
                #endif
                m_Fields.Add( field );
            }
        }
        
    
        #if STACK_HISTROY
        public void SetField( string key, string value, [CallerFilePath] string _file = "", [CallerLineNumber] int _line = -1 )
        #else
        public void SetField( string key, string value )
        #endif
        {
            SerializeValue field = GetField( key );
            if( field != null )
            {
                #if STACK_HISTROY
                SerializeValue.SetString( field, value, _file, _line );
                #else
                SerializeValue.SetString( field, value );
                #endif
            }
            else
            {
                #if STACK_HISTROY
                field = SerializeValue.CreateString( key, value, _file, _line );
                #else
                field = SerializeValue.CreateString( key, value );
                #endif
                m_Fields.Add( field );
            }
        }
        

        #if STACK_HISTROY
        public void SetField( string key, Vector2 value, [CallerFilePath] string _file = "", [CallerLineNumber] int _line = -1 )
        #else
        public void SetField( string key, Vector2 value )
        #endif
        {
            SerializeValue field = GetField( key );
            if( field != null )
            {
                #if STACK_HISTROY
                SerializeValue.SetVector2( field, value, _file, _line );
                #else
                SerializeValue.SetVector2( field, value );
                #endif
            }
            else
            {
                #if STACK_HISTROY
                field = SerializeValue.CreateVector2( key, value, _file, _line );
                #else
                field = SerializeValue.CreateVector2( key, value );
                #endif
                m_Fields.Add( field );
            }
        }
        

        #if STACK_HISTROY
        public void SetField( string key, Vector3 value, [CallerFilePath] string _file = "", [CallerLineNumber] int _line = -1 )
        #else
        public void SetField( string key, Vector3 value )
        #endif
        {
            SerializeValue field = GetField( key );
            if( field != null )
            {
                #if STACK_HISTROY
                SerializeValue.SetVector3( field, value, _file, _line );
                #else
                SerializeValue.SetVector3( field, value );
                #endif
            }
            else
            {
                #if STACK_HISTROY
                field = SerializeValue.CreateVector3( key, value, _file, _line );
                #else
                field = SerializeValue.CreateVector3( key, value );
                #endif
                m_Fields.Add( field );
            }
        }
        

        #if STACK_HISTROY
        public void SetField( string key, Vector4 value, [CallerFilePath] string _file = "", [CallerLineNumber] int _line = -1 )
        #else
        public void SetField( string key, Vector4 value )
        #endif
        {
            SerializeValue field = GetField( key );
            if( field != null )
            {
                #if STACK_HISTROY
                SerializeValue.SetVector4( field, value, _file, _line );
                #else
                SerializeValue.SetVector4( field, value );
                #endif
            }
            else
            {
                #if STACK_HISTROY
                field = SerializeValue.CreateVector4( key, value, _file, _line );
                #else
                field = SerializeValue.CreateVector4( key, value );
                #endif
                m_Fields.Add( field );
            }
        }
        

        #if STACK_HISTROY
        public void SetField( string key, Color value, [CallerFilePath] string _file = "", [CallerLineNumber] int _line = -1 )
        #else
        public void SetField( string key, Color value )
        #endif
        {
            SerializeValue field = GetField( key );
            if( field != null )
            {
                #if STACK_HISTROY
                SerializeValue.SetColor( field, value, _file, _line );
                #else
                SerializeValue.SetColor( field, value );
                #endif
            }
            else
            {
                #if STACK_HISTROY
                field = SerializeValue.CreateColor( key, value, _file, _line );
                #else
                field = SerializeValue.CreateColor( key, value );
                #endif
                m_Fields.Add( field );
            }
        }
        

        #if STACK_HISTROY
        public void SetField( string key, GameObject value, [CallerFilePath] string _file = "", [CallerLineNumber] int _line = -1 )
        #else
        public void SetField( string key, GameObject value )
        #endif
        {
            SerializeValue field = GetField( key );
            if( field != null )
            {
                #if STACK_HISTROY
                SerializeValue.SetGameObject( field, value, _file, _line );
                #else
                SerializeValue.SetGameObject( field, value );
                #endif
            }
            else
            {
                #if STACK_HISTROY
                field = SerializeValue.CreateGameObject( key, value, _file, _line );
                #else
                field = SerializeValue.CreateGameObject( key, value );
                #endif
                m_Fields.Add( field );
            }
        }
        

        #if STACK_HISTROY
        public void SetField( string key, UnityEngine.UI.Text value, [CallerFilePath] string _file = "", [CallerLineNumber] int _line = -1 )
        #else
        public void SetField( string key, UnityEngine.UI.Text value )
        #endif
        {
            SerializeValue field = GetField( key );
            if( field != null )
            {
                #if STACK_HISTROY
                SerializeValue.SetLabel( field, value, _file, _line );
                #else
                SerializeValue.SetLabel( field, value );
                #endif
            }
            else
            {
                #if STACK_HISTROY
                field = SerializeValue.CreateLabel( key, value, _file, _line );
                #else
                field = SerializeValue.CreateLabel( key, value );
                #endif
                m_Fields.Add( field );
            }
        }
        

        #if STACK_HISTROY
        public void SetField( string key, UnityEngine.UI.Button value, [CallerFilePath] string _file = "", [CallerLineNumber] int _line = -1 )
        #else
        public void SetField( string key, UnityEngine.UI.Button value )
        #endif
        {
            SerializeValue field = GetField( key );
            if( field != null )
            {
                #if STACK_HISTROY
                SerializeValue.SetButton( field, value, _file, _line );
                #else
                SerializeValue.SetButton( field, value );
                #endif
            }
            else
            {
                #if STACK_HISTROY
                field = SerializeValue.CreateButton( key, value, _file, _line );
                #else
                field = SerializeValue.CreateButton( key, value );
                #endif
                m_Fields.Add( field );
            }
        }
        
   
        #if STACK_HISTROY
        public void SetField( string key, UnityEngine.UI.Toggle value, [CallerFilePath] string _file = "", [CallerLineNumber] int _line = -1 )
        #else
        public void SetField( string key, UnityEngine.UI.Toggle value )
        #endif
        {
            SerializeValue field = GetField( key );
            if( field != null )
            {
                #if STACK_HISTROY
                SerializeValue.SetToggle( field, value, _file, _line );
                #else
                SerializeValue.SetToggle( field, value );
                #endif
            }
            else
            {
                #if STACK_HISTROY
                field = SerializeValue.CreateToggle( key, value, _file, _line );
                #else
                field = SerializeValue.CreateToggle( key, value );
                #endif
                m_Fields.Add( field );
            }
        }
        

        #if STACK_HISTROY
        public void SetGlobal( string key, string fieldName, object value, [CallerFilePath] string _file = "", [CallerLineNumber] int _line = -1 )
        #else
        public void SetGlobal( string key, string fieldName, object value )
        #endif
        {
            SerializeValue field = GetField( key );
            if( field != null )
            {
                #if STACK_HISTROY
                SerializeValue.SetGlobal( field, value, _file, _line );
                #else
                SerializeValue.SetGlobal( field, value );
                #endif
            }
            else
            {
                #if STACK_HISTROY
                field = SerializeValue.CreateGlobal( key, fieldName, value, _file, _line );
                #else
                field = SerializeValue.CreateGlobal( key, fieldName, value );
                #endif
                m_Fields.Add( field );
            }
            
            #if STACK_HISTROY
            field.AddCallerInfo( _file, _line );
            #endif
        }
        

        #if STACK_HISTROY
        public void SetObject( string key, object value, [CallerFilePath] string _file = "", [CallerLineNumber] int _line = -1 )
        #else
        public void SetObject( string key, object value )
        #endif
        {
            SerializeValue field = GetField( key );
            if( field != null )
            {
                #if STACK_HISTROY
                SerializeValue.SetObject( field, value, _file, _line );
                #else
                SerializeValue.SetObject( field, value );
                #endif
            }
            else
            {
                #if STACK_HISTROY
                field = SerializeValue.CreateObject( key, value, _file, _line );
                #else
                field = SerializeValue.CreateObject( key, value );
                #endif
                m_Fields.Add( field );
            }
        }
        

        public void SetActive( int group, bool sw )
        {
            for( int i = 0; i < m_Fields.Count; ++i )
            {
                if( m_Fields[ i ].group == group )
                {
                    GameObject obj = m_Fields[ i ].v_GameObject;
                    if( obj != null )
                    {
                        if( obj.activeSelf != sw )
                        {
                            obj.SetActive( sw );
                        }
                    }
                }
            }
        }
        

        public GameObject SetActive( string key, bool sw )
        {
            SerializeValue field = GetField( key );
            if( field != null )
            {
                GameObject obj = field.v_GameObject;
                if( obj != null )
                {
                    if( obj.activeSelf != sw )
                    {
                        obj.SetActive( sw );
                    }
                    return obj;
                }
            }
            return null;
        }
        

        public GameObject SetActive( string key, bool sw, string label )
        {
            SerializeValue field = GetField( key );
            if( field != null )
            {
                GameObject obj = field.v_GameObject;
                if( obj != null )
                {
                    if( obj.activeSelf != sw )
                    {
                        obj.SetActive( sw );
                    }
                    if( field.type == SerializeValue.Type.UILabel )
                    {
                        UnityEngine.UI.Text uiLabel = obj.GetComponent<UnityEngine.UI.Text>();
                        if( uiLabel != null )
                        {
                            uiLabel.text = label;
                        }
                    }
                    return obj;
                }
            }
            return null;
        }
        

        public void SetUIOn( string key, bool value )
        {
            SerializeValue field = GetField( key );
            if( field != null )
            {
                field.v_UIOn = value;
            }
        }
        

        public UnityEngine.UI.Selectable SetInteractable( string key, bool value )
        {
            SerializeValue field = GetField( key );
            if( field != null )
            {
                UnityEngine.UI.Selectable selectable = field.v_UISelectable;
                if( selectable != null )
                {
                    if( selectable.interactable != value )
                    {
                        selectable.interactable = value;
                    }
                }
                return selectable;
            }
            return null;
        }
        

        public SerializeValue GetField( string key )
        {
            if ( string.IsNullOrEmpty( key ) || m_Fields == null )
            {
                return null;
            }
            for (int i = 0, count = m_Fields.Count; i < count; ++i)
            {
                var field = m_Fields[i];
                if (field?.key == key)
                {
                    return field;
                }
            }
            return null;
        }
        
 
        public bool TryGetField( string key, out SerializeValue field )
        {
            if( string.IsNullOrEmpty( key ) == false )
            {
                field = CollectionUtility.FindEquals<SerializeValue,string>( m_Fields, ( o ) => o.key, key );
                if( field != null )
                {
                    return true;
                }
            }
            field = null;
            return false;
        }
        

        public SerializeValue[] GetFields( )
        {
            return m_Fields.ToArray( );
        }
        

        public SerializeValue[] GetFields( int group )
        {
            List<SerializeValue> result = new List<SerializeValue>( );
            for( int i = 0; i < m_Fields.Count; ++i )
            {
                SerializeValue field = m_Fields[ i ];
                if( field != null )
                {
                    if( field.group == group )
                    {
                        result.Add( field );
                    }
                }
            }
            return result.ToArray();
        }
        

        public SerializeValue[] GetFields( string key )
        {
            List<SerializeValue> result = new List<SerializeValue>( );
            for( int i = 0; i < m_Fields.Count; ++i )
            {
                SerializeValue field = m_Fields[ i ];
                if( field != null )
                {
                    if( field.key == key )
                    {
                        result.Add( field );
                    }
                }
            }
            return result.ToArray();
        }
        
 
        public SerializeValue[] GetFields<T>( )
        {
            List<SerializeValue> result = new List<SerializeValue>( );
            for( int i = 0; i < m_Fields.Count; ++i )
            {
                object obj = m_Fields[ i ].v_Object;
                if( obj != null && obj is T )
                {
                    result.Add( m_Fields[ i ] );
                }
            }
            return result.ToArray( );
        }
        

        public void GetField( string key, ref bool result )                     { result = GetBool( key );                 }
        public void GetField( string key, ref bool result, bool defaultValue )  { result = GetBool( key, defaultValue );   }
        public bool GetBool( string key )                                       { return GetBool( key, false );            }
        public bool GetBool( string key, bool defaultValue )
        {
            SerializeValue field = GetField( key );
            if( field != null )
            {
                return field.v_Bool;
            }
            return defaultValue;
        }
        

        public void GetField( string key, ref int result )                      { result = GetInt( key );                  }
        public void GetField( string key, ref int result, int defaultValue )    { result = GetInt( key, defaultValue );    }
        public int GetInt( string key )                                         { return GetInt( key, 0 );                 }
        public int GetInt( string key, int defaultValue )
        {
            SerializeValue field = GetField( key );
            if( field != null )
            {
                return field.v_Int;
            }
            return defaultValue;
        }


        public void GetField( string key, ref long result ) { result = GetLong( key ); }
        public void GetField( string key, ref long result, long defaultValue ) { result = GetLong( key, defaultValue ); }
        public long GetLong( string key ) { return GetLong( key, 0 ); }
        public long GetLong( string key, long defaultValue )
        {
            SerializeValue field = GetField( key );
            if( field != null )
            {
                return field.v_Long;
            }
            return defaultValue;
        }
        

        public void GetField( string key, ref float result )                    { result = GetFloat( key, 0.0f );          }
        public void GetField( string key, ref float result, float defaultValue ){ result = GetFloat( key, defaultValue );  }
        public float GetFloat( string key )                                     { return GetFloat( key, 0.0f );            }
        public float GetFloat( string key, float defaultValue )
        {
            SerializeValue field = GetField( key );
            if( field != null )
            {
                return field.v_Float;
            }
            return defaultValue;
        }
        

        public void GetField( string key, ref string result )                       { result = GetString( key );               }
        public void GetField( string key, ref string result, string defaultValue )  { result = GetString( key, defaultValue ); }
        public string GetString( string key )                                       { return GetString( key, "");              }
        public string GetString( string key, string defaultValue )
        {
            SerializeValue field = GetField( key );
            if( field != null )
            {
                return field.v_String;
            }
            return defaultValue;
        }
        
 
        public void GetField( string key, ref Vector2 result )                      { result = GetVector2( key );              }
        public void GetField( string key, ref Vector2 result, Vector2 defaultValue ){ result = GetVector2( key, defaultValue );}
        public Vector2 GetVector2( string key )                                     { return GetVector2( key, Vector2.zero );  }
        public Vector2 GetVector2( string key, Vector2 defaultValue )
        {
            SerializeValue field = GetField( key );
            if( field != null )
            {
                return field.v_Vector2;
            }
            return defaultValue;
        }
        
  
        public void GetField( string key, ref Vector3 result )                      { result = GetVector3( key );              }
        public void GetField( string key, ref Vector3 result, Vector3 defaultValue ){ result = GetVector3( key, defaultValue );}
        public Vector3 GetVector3( string key )                                     { return GetVector3( key, Vector3.zero );  }
        public Vector3 GetVector3( string key, Vector3 defaultValue )
        {
            SerializeValue field = GetField( key );
            if( field != null )
            {
                return field.v_Vector3;
            }
            return defaultValue;
        }
        

        public void GetField( string key, ref Vector4 result )                      { result = GetVector4( key );              }
        public void GetField( string key, ref Vector4 result, Vector4 defaultValue ){ result = GetVector4( key, defaultValue );}
        public Vector4 GetVector4( string key )                                     { return GetVector4( key, Vector4.zero );  }
        public Vector4 GetVector4( string key, Vector4 defaultValue )
        {
            SerializeValue field = GetField( key );
            if( field != null )
            {
                return field.v_Vector4;
            }
            return defaultValue;
        }
        

        public void GetField( string key, ref Color result )                        { result = GetColor( key );              }
        public void GetField( string key, ref Color result, Color defaultValue )    { result = GetColor( key, defaultValue );}
        public Color GetColor( string key )                                         { return GetColor( key, Color.white );  }
        public Color GetColor( string key, Color defaultValue )
        {
            SerializeValue field = GetField( key );
            if( field != null )
            {
                return field.v_Color;
            }
            return defaultValue;
        }
        

        public void GetField( string key, ref GameObject result )                           { result = GetGameObject( key ); }
        public void GetField( string key, ref GameObject result, GameObject defaultValue )  { result = GetGameObject( key, defaultValue );}
        // [return: MaybeNull]
        public GameObject GetGameObject( string key )                                       { return GetGameObject( key, null );   }
        public GameObject GetGameObject( string key, GameObject defaultValue )
        {
            SerializeValue field = GetField( key );
            if( field != null )
            {
                return field.v_GameObject;
            }
            return defaultValue;
        }
        

        public void GetField( string key, ref UnityEngine.UI.Text result )                                  { result = GetUILabel( key ); }
        public void GetField( string key, ref UnityEngine.UI.Text result, UnityEngine.UI.Text defaultValue ){ result = GetUILabel( key, defaultValue );}
        public UnityEngine.UI.Text GetUILabel( string key )                                                 { return GetUILabel( key, null );  }
        public UnityEngine.UI.Text GetUILabel( string key, UnityEngine.UI.Text defaultValue )
        {
            SerializeValue field = GetField( key );
            if( field != null )
            {
                return field.v_UILabel;
            }
            return defaultValue;
        }
        
        public TextMeshProUGUI GetUITextMeshPro( string key )                                                 { return GetUITextMeshPro( key, null );  }
        public TextMeshProUGUI GetUITextMeshPro( string key, TextMeshProUGUI defaultValue )
        {
            SerializeValue field = GetField( key );
            if( field != null )
            {
                return field.v_UITextMeshPro;
            }
            return defaultValue;
        }

        public void GetField( string key, ref UnityEngine.UI.RawImage result )                                      { result = GetUIRawImage( key ); }
        public void GetField( string key, ref UnityEngine.UI.RawImage result, UnityEngine.UI.RawImage defaultValue ){ result = GetUIRawImage( key, defaultValue );}
        public UnityEngine.UI.RawImage GetUIRawImage( string key )                                                  { return GetUIRawImage( key, null );     }
        public UnityEngine.UI.RawImage GetUIRawImage( string key, UnityEngine.UI.RawImage defaultValue )
        {
            SerializeValue field = GetField( key );
            if( field != null )
            {
                return field.v_UIRawImage;
            }
            return defaultValue;
        }
        

        public void GetField( string key, ref UnityEngine.UI.Image result )                                    { result = GetUIImage( key ); }
        public void GetField( string key, ref UnityEngine.UI.Image result, UnityEngine.UI.Image defaultValue ){ result = GetUIImage( key, defaultValue );}
        public UnityEngine.UI.Image GetUIImage( string key )                                                  { return GetUIImage( key, null );     }
        public UnityEngine.UI.Image GetUIImage( string key, UnityEngine.UI.Image defaultValue )
        {
            SerializeValue field = GetField( key );
            if( field != null )
            {
                return field.v_UIImage;
            }
            return defaultValue;
        }
        

        public void GetField( string key, ref UnityEngine.UI.Button result )                                    { result = GetUIButton( key ); }
        public void GetField( string key, ref UnityEngine.UI.Button result, UnityEngine.UI.Button defaultValue ){ result = GetUIButton( key, defaultValue );}
        public UnityEngine.UI.Button GetUIButton( string key )                                                  { return GetUIButton( key, null );     }
        public UnityEngine.UI.Button GetUIButton( string key, UnityEngine.UI.Button defaultValue )
        {
            SerializeValue field = GetField( key );
            if( field != null )
            {
                return field.v_UIButton;
            }
            return defaultValue;
        }
        

        public void GetField( string key, ref UnityEngine.UI.Toggle result )                                    { result = GetUIToggle( key ); }
        public void GetField( string key, ref UnityEngine.UI.Toggle result, UnityEngine.UI.Toggle defaultValue ){ result = GetUIToggle( key, defaultValue );}
        public UnityEngine.UI.Toggle GetUIToggle( string key )                                                  { return GetUIToggle( key, null );     }
        public UnityEngine.UI.Toggle GetUIToggle( string key, UnityEngine.UI.Toggle defaultValue )
        {
            SerializeValue field = GetField( key );
            if( field != null )
            {
                return field.v_UIToggle;
            }
            return defaultValue;
        }
        

        public object GetGlobal( string key )                                                           { return GetGlobal( key, null );     }
        public object GetGlobal( string key, object defaultValue )
        {
            SerializeValue field = GetField( key );
            if( field != null )
            {
                return field.v_Global;
            }
            return defaultValue;
        }
        

        public object GetObject( string key )                                                           { return GetObject( key, null );     }
        public object GetObject( string key, object defaultValue )
        {
            SerializeValue field = GetField( key );
            if( field != null )
            {
                return field.v_Object;
            }
            return defaultValue;
        }
        public T GetObject<T>( string key )
        {
            return GetObject<T>( key, default(T) );
        }

        public T GetObject<T>( string key, T defaultValue )
        {
            SerializeValue field = GetField( key );
            if( field != null )
            {
                return (T)field.v_Object;
            }
            return defaultValue;
        }
        public T GetObject<T>( T defaultValue )
        {
            return GetObject<T>( typeof( T ).Name, defaultValue );
        }
        

        public T GetEnum<T>( string key )                                                               { return GetEnum<T>( key, default(T) );     }
        public T GetEnum<T>( string key, T defaultValue )
        {
            SerializeValue field = GetField( key );
            if( field != null )
            {
                return field.GetEnum<T>( );
            }
            return defaultValue;
        }


        public T GetDataList<T>( string key ) { return GetDataList<T>( key, default( T ) ); }
        public T GetDataList<T>( string key, T defaultValue )
        {
            DataSource.DataList data_list = GetObject<DataSource.DataList>(key);
            if( data_list != null )
            {
                return data_list.FindDataOfClass<T>( defaultValue );
            }
            return defaultValue;
        }
        
        // [return: MaybeNull]
        public T GetComponent<T>( string key ) where T : Component
        {
            return GetComponent<T>( key, null );
        }

        public T GetComponentOrError<T>( string key ) where T : Component
        {
            return GetComponent<T>( key, null ) ?? throw new Utility.KeyNotFoundException<T>(key);
        }

        public T GetComponent<T>( string key, T defaultValue ) where T : Component
        {
            GameObject gobj = GetGameObject( key );
            if( gobj != null )
            {
                T result = gobj.GetComponent<T>();
                if( result == null )
                {
                    result = gobj.GetComponentInParent<T>( );
                }
                return result;
            }
            return defaultValue;
        }
        
  
        public T GetContentParam<T>( string key ) where T : UIContentSource.Param { return GetContentParam<T>( key, default(T) ); }
        public T GetContentParam<T>( string key, T defaultValue ) where T : UIContentSource.Param
        {
            UIContentNode node = GetComponent<UIContentNode>( key );
            if( node != null )
            {
                return node.GetParam<T>();
            }
            return defaultValue;
        }
        

        public DataSource GetDataSource( string key )
        {
            return GetComponent<DataSource>( key );
        }
        public T GetDataSource<T>( string key ) { return GetDataSource<T>( key, default(T) ); }
        public T GetDataSource<T>( string key, T defaultValue )
        {
            DataSource source = GetComponent<DataSource>( key );
            if( source != null )
            {
                return source.FindDataOfClass<T>( defaultValue );
            }
            return defaultValue;
        }
        

        public bool GetUIOn( string key ){ return GetUIOn( key, false );     }
        public bool GetUIOn( string key, bool defaultValue )
        {
            SerializeValue field = GetField( key );
            if( field != null )
            {
                return field.v_UIOn;
            }
            return defaultValue;
        }
        

        public bool HasField( string key )
        {
            return CollectionUtility.FindEquals<SerializeValue,string>( m_Fields, ( o ) => o.key, key ) != null;
        }
        

        public bool IsActive( string key )
        {
            SerializeValue field = GetField( key );
            if( field != null )
            {
                GameObject obj = field.v_GameObject;
                if( obj != null )
                {
                    return obj.activeSelf;
                }
            }
            return false;
        }
        

        public SerializeValue[] GetFieldsFromGroupDefine( string name )
        {
            GroupDefine def = GetGroupDefine( name );
            if( def != null )
            {
                List<SerializeValue> result = new List<SerializeValue>( );
                for( int i = 0; i < m_Fields.Count; ++i )
                {
                    if( m_Fields[ i ].group == def.id )
                    {
                        result.Add( m_Fields[ i ] );
                    }
                }
                return result.ToArray( );
            }
            return new SerializeValue[0];
        }
        
  
        public void GetFieldsFromGroupDefine( string name, List<SerializeValue> result )
        {
            result.Clear();
            GroupDefine def = GetGroupDefine( name );
            if( def != null )
            {
                for( int i = 0; i < m_Fields.Count; ++i )
                {
                    if( m_Fields[ i ].group == def.id )
                    {
                        result.Add( m_Fields[ i ] );
                    }
                }
            }
        }


        public SerializeValue GetGroupField( int id, string key )
        {
            for( int i = 0; i < m_Fields.Count; ++i )
            {
                if( m_Fields[ i ].group == id && m_Fields[ i ].key == key )
                {
                    return m_Fields[ i ];
                }
            }
            return null;
        }
        public SerializeValue GetGroupField( string name, string key )
        {
            GroupDefine def = GetGroupDefine( name );
            if( def != null )
            {
                for( int i = 0; i < m_Fields.Count; ++i )
                {
                    if( m_Fields[ i ].group == def.id && m_Fields[ i ].key == key )
                    {
                        return m_Fields[ i ];
                    }
                }
            }
            return null;
        }
        

        public void GetGroupComponents<T>( int id, ref List<T> result ) where T : Component
        {
            if( result == null ) result = new List<T>( );
            for( int i = 0; i < m_Fields.Count; ++i )
            {
                SerializeValue value = m_Fields[ i ];
                if( value.group == id && value.isGameObject )
                {
                    T component = value.v_GameObject.GetComponent<T>( );
                    if( component != null )
                    {
                        result.Add( component );
                    }
                }
            }
        }
        public void GetGroupComponents<T>( string name, ref List<T> result ) where T : Component
        {
            GroupDefine def = GetGroupDefine( name );
            if( def != null )
            {
                GetGroupComponents<T>( def.id, ref result );
            }
        }
        
 
        public void SetGroupActive( int id, bool value )
        {
            GroupDefine def = GetGroupDefine( id );
            if( def != null )
            {
                var fields = ListPool<SerializeValue>.Rent();
                GetFieldsFromGroupDefine( def.name, fields );
                foreach (var field in fields)
                {
                    field.v_Active = value;
                }
                ListPool<SerializeValue>.Return(fields);
            }
        }
        public void SetGroupActive( string name, bool value )
        {
            var fields = ListPool<SerializeValue>.Rent();
            GetFieldsFromGroupDefine( name, fields );
            foreach (var field in fields)
            {
                field.v_Active = value;
            }
            ListPool<SerializeValue>.Return(fields);
        }
        

        public void SetGroupInteractable( int id, bool value )
        {
            GroupDefine def = GetGroupDefine( id );
            if( def != null )
            {
                SerializeValue[] values = GetFieldsFromGroupDefine( def.name );
                for( int i = 0; i < values.Length; ++i )
                {
                    values[ i ].v_UIInteractable = value;
                }
            }
        }
        public void SetGroupInteractable( string name, bool value )
        {
            SerializeValue[] values = GetFieldsFromGroupDefine( name );
            for( int i = 0; i < values.Length; ++i )
            {
                values[ i ].v_UIInteractable = value;
            }
        }

        public void SetGroupActive( int id, string key, bool value )
        {
            SerializeValue field = GetGroupField( id, key );
            if( field != null )
            {
                field.v_Active = value;
            }
        }
        public void SetGroupActive( string name, string key, bool value )
        {
            SerializeValue field = GetGroupField( name, key );
            if( field != null )
            {
                field.v_Active = value;
            }
        }
        
    
        public GroupDefine GetGroupDefine( int id )
        {
            return CollectionUtility.FindEquals<GroupDefine,int>( m_GroupDefine, ( prop ) => prop.id, id );
        }
        public GroupDefine GetGroupDefine( string name )
        {
            return CollectionUtility.FindEquals<GroupDefine,string>( m_GroupDefine, ( prop ) => prop.name, name );
        }

        public string GetGroupDefineName( int id )
        {
            GroupDefine def = GetGroupDefine( id );
            if( def != null )
            {
                return def.name;
            }
            return id.ToString( );
        }
        
  
        public bool HasGroupDefine( int id )
        {
            return CollectionUtility.FindEquals<GroupDefine,int>( m_GroupDefine, ( prop ) => prop.id, id ) != null;
        }
        
    
        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append(nameof(SerializeValueList));
            sb.Append('<');
            foreach (var field in m_Fields)
            {
                sb.Append('[');
                sb.Append(field.ToString());
                sb.Append(']');
                sb.Append(',');
            }
            sb.Append('>');
            return sb.ToString();
        }
    }
}
