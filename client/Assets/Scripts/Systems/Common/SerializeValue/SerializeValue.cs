#define USE_TEXTMESHPRO
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Diagnostics;
using TMPro;
using UnityEngine.UI;
using Debug = UnityEngine.Debug;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace EG
{

    [System.Serializable]
    public partial class SerializeValue 
    {
        public const int            GROUP_NONE  = 0;
        
        public static System.Type   TYPE_GLOBAL = typeof(GlobalVars);
        
        public enum Type
        {
            NONE                = 0,
            Bool                = 1,
            Int                 = 5,
            Long                = 6,
            Float               = 7,
            String              = 10,
            Vector2             = 20,
            Vector3             = 21,
            Vector4             = 22,
            Color               = 23,
            GameObject          = 50,
            UIRawImage          = 59,
            UIImage             = 60,
            UILabel             = 61,
            UIButton            = 62,
            UIToggle            = 63,
            Global              = 200,
            Object              = 300,
        };
        
        public enum PropertyType
        {
            Bool                = 1,
            Int                 = 5,
            Long                = 6,
            Float               = 7,
            String              = 10,
            Vector2             = 20,
            Vector3             = 21,
            Vector4             = 22,
            Color               = 23,
            GameObject          = 50,
            UIRawImage          = 59,
            UIImage             = 60,
            UILabel             = 61,
            UIButton            = 62,
            UIToggle            = 63,
            Global              = 200,
            Object              = 300,
            Active              = 1000,
            Enable              = 1001,
            UISelectable        = 1100,
            UIText              = 1101,
            UIInteractabel      = 1102,
            UIOn                = 1103,
        };
        

        [SerializeField] int                m_Group;
        [SerializeField] Type               m_Type;
        [SerializeField] string             m_Key;
        
        [SerializeField] GameObject         m_Obj;
        [SerializeField] string             m_Serialize;
        
        System.Object                       m_Work;
        
        static object                       s_BoxedTrue = true;
        static object                       s_BoxedFalse = false;
        
        #if UNITY_EDITOR
        public bool __foldout { set; get; }
        #endif
        

        public int                          group           { set { m_Group = value; } get { return m_Group; } }
        public Type                         type            { get { return m_Type;                          } }
        public string                       key             { set { m_Key = value; } get { return m_Key;    } }
        
        public bool                         isGameObject    { get { return m_Type == Type.GameObject; } }
        
        public override string ToString()
        {
            switch( type )
            {
            case Type.NONE:                 return string.Format( "key:{0} type:{1} value:{2}", key, type.ToString(), 0 );
            case Type.Bool:                 return string.Format( "key:{0} type:{1} value:{2}", key, type.ToString(), v_Bool );
            case Type.Int:                  return string.Format( "key:{0} type:{1} value:{2}", key, type.ToString(), v_Int );
            case Type.Long:                 return string.Format( "key:{0} type:{1} value:{2}", key, type.ToString(), v_Long );
            case Type.Float:                return string.Format( "key:{0} type:{1} value:{2}", key, type.ToString(), v_Float );
            case Type.String:               return string.Format( "key:{0} type:{1} value:{2}", key, type.ToString(), v_String );
            case Type.Vector2:              return string.Format( "key:{0} type:{1} value:{2}", key, type.ToString(), v_Vector2 );
            case Type.Vector3:              return string.Format( "key:{0} type:{1} value:{2}", key, type.ToString(), v_Vector3 );
            case Type.Vector4:              return string.Format( "key:{0} type:{1} value:{2}", key, type.ToString(), v_Vector4 );
            case Type.Color:                return string.Format( "key:{0} type:{1} value:{2}", key, type.ToString(), v_Color );
            case Type.GameObject:           return string.Format( "key:{0} type:{1} value:{2}", key, type.ToString(), v_GameObject != null ? v_GameObject.name: "null" );
            case Type.UILabel:              return string.Format( "key:{0} type:{1} value:{2}", key, type.ToString(), v_UILabel != null ? v_UILabel.name: "null" );
            case Type.UIButton:             return string.Format( "key:{0} type:{1} value:{2}", key, type.ToString(), v_UIButton != null ? v_UIButton.name: "null" );
            case Type.UIToggle:             return string.Format( "key:{0} type:{1} value:{2}", key, type.ToString(), v_UIToggle != null ? v_UIToggle.name: "null" );
            case Type.Global:               return string.Format( "key:{0} type:{1} value:{2}", key, type.ToString(), GetGlobal() );
            case Type.UIRawImage:           return string.Format( "key:{0} type:{1} value:{2}", key, type.ToString(), v_UIRawImage != null ? v_UIRawImage.name: "null" );
            case Type.UIImage:              return string.Format( "key:{0} type:{1} value:{2}", key, type.ToString(), v_UIImage != null ? v_UIImage.name: "null" );
            case Type.Object:               return string.Format( "key:{0} type:{1} value:{2}", key, type.ToString(), v_Object != null ? v_Object.ToString(): "null" );
            };
            return string.Format( "key:{0] type:UNKNOWN", key );
        }

        public string GetValueString()
        {
            switch( type )
            {
            case Type.NONE:                 return "0";
            case Type.Bool:                 return v_Bool.ToString();
            case Type.Int:                  return v_Int.ToString();
            case Type.Long:                 return v_Long.ToString();
            case Type.Float:                return v_Float.ToString();
            case Type.String:               return v_String;
            case Type.Vector2:              return v_Vector2.ToString();
            case Type.Vector3:              return v_Vector3.ToString();
            case Type.Vector4:              return v_Vector4.ToString();
            case Type.Color:                return v_Color.ToString();
            case Type.GameObject:           return v_GameObject != null ? v_GameObject.name: "null";
            case Type.UILabel:              return v_UILabel != null ? v_UILabel.name: "null";
            case Type.UIButton:             return v_UIButton != null ? v_UIButton.name: "null";
            case Type.UIToggle:             return v_UIToggle != null ? v_UIToggle.name: "null";
            case Type.Global:               return GetGlobal();
            case Type.UIRawImage:           return v_UIRawImage != null ? v_UIRawImage.name: "null";
            case Type.UIImage:              return v_UIImage != null ? v_UIImage.name: "null";
            case Type.Object:               return v_Object != null ? v_Object.ToString(): "null";
            };
            return string.Format( "UNKNOWN" );
        }
        
        
        #if UNITY_EDITOR
        
        public enum EditMode
        {
            LIST    ,   // 
            DETAIL  ,   // 
            SIMPLE  ,   // 
        }
        
        public void OnGUIInspect( Rect rect, EditMode editMode, UnityEngine.Object obj, SerializeValueList list )
        {
            const float     GROUP_WIDTH = 30;
            const float     KEY_WIDTH   = 0.4f;
            const float     FIELD_WIDTH = 0.6f;
            const float     DELETE_WIDTH= 30;
            const float     SPACE_WIDTH = 5;
            
            float width = rect.width - SPACE_WIDTH - 4;
            
            if( editMode != EditMode.SIMPLE )
            {
                width -= ( GROUP_WIDTH + DELETE_WIDTH );
            }
            
            Rect elementRect = rect;
            elementRect.x += 2;
            elementRect.y += 2;
            elementRect.height = EditorGUIUtility.singleLineHeight;
            
            GUI.Box( rect, "" );
            
            // 
            if( editMode != EditMode.SIMPLE )
            {
                elementRect.width = GROUP_WIDTH;
                int group = m_Group;
                int nextGroup = EditorGUI.IntField( elementRect, group );
                if( nextGroup != group )
                {
                    if( obj != null ) UnityEditor.Undo.RecordObjects( new UnityEngine.Object[]{ obj }, "SerializeValue" );
                    m_Group = nextGroup;
                    if( obj != null ) EditorUtility.SetDirty( obj );
                }
                elementRect.x += elementRect.width;
            }
            
            // 
            if( list != null )
            {
                elementRect.x += 2;
                elementRect.width = width * ( editMode == EditMode.DETAIL ? 1.0f: KEY_WIDTH ) - 2;
                string key = m_Key;
                string nextKey = EditorGUI.TextField( elementRect, key );
                if( nextKey != key )
                {
                    if( obj != null ) UnityEditor.Undo.RecordObjects( new UnityEngine.Object[]{ obj }, "SerializeValue" );
                    m_Key = nextKey;
                    if( obj != null ) EditorUtility.SetDirty( obj );
                }
                elementRect.x += elementRect.width;
            }
            else
            {
                elementRect.x += 2;
                elementRect.width = width * ( editMode == EditMode.DETAIL ? 1.0f: KEY_WIDTH ) - 2;
                Type nextType = (Type)UnityEditor.EditorGUI.EnumPopup( elementRect, m_Type );
                if( nextType != m_Type )
                {
                    if( obj != null ) UnityEditor.Undo.RecordObjects( new UnityEngine.Object[] { obj }, "SerializeValue" );
                    m_Type = nextType;
                    if( obj != null ) EditorUtility.SetDirty( obj );
                }
                elementRect.x += elementRect.width;
            }
            
            // 
            if( editMode != EditMode.DETAIL )
            {
                elementRect.x += 2;
                elementRect.width = width * FIELD_WIDTH - 2;
                OnGUIObjectFiled( elementRect, editMode, obj );
                elementRect.x += elementRect.width;
            }
            
            // 
            if( editMode != EditMode.SIMPLE )
            {
                elementRect.x += 2;
                elementRect.width = DELETE_WIDTH - 2;
                if( GUI.Button( elementRect, "×" ) )
                {
                    if( obj != null ) UnityEditor.Undo.RecordObjects( new UnityEngine.Object[]{ obj }, "SerializeValue" );
                    list.RemoveField( this );
                    if( obj != null ) EditorUtility.SetDirty( obj );
                }
            }
            
            // 
            if( editMode == EditMode.DETAIL )
            {
                width = rect.width - SPACE_WIDTH - 4;
                
                // 
                elementRect = rect;
                elementRect.x += 2;
                elementRect.y += EditorGUIUtility.singleLineHeight + 2;
                elementRect.width = width * 0.5f - 2;
                elementRect.height = EditorGUIUtility.singleLineHeight;
                EditorGUI.LabelField( elementRect, m_Type.ToString( ) );
                elementRect.x += elementRect.width;
                
                // 
                elementRect.x += 2;
                elementRect.width = width * 0.5f - 2;
                OnGUIObjectFiled( elementRect, editMode, obj );
                elementRect.x += elementRect.width;
            }
        }
        

        public void OnGUIInspect( UnityEngine.Object obj, SerializeValueList list, string label, float width )
        {
            EditorHelp.Title( label );
            Rect rect = GUILayoutUtility.GetLastRect( );
            GUILayout.Label( "" );
            Rect rectLast = GUILayoutUtility.GetLastRect( );
            Rect element = rect;
            element.x = rectLast.x;
            element.y += rect.height;
            element.width = rectLast.width;
            element.height = rectLast.y - rect.y;
            OnGUIInspect( element, EditMode.SIMPLE, obj, list );
        }
        

        public void OnGUIInspect( UnityEngine.Object obj, SerializeValueList list )
        {
            Rect rect = GUILayoutUtility.GetLastRect( );
            GUILayout.Label( "" );
            Rect rectLast = GUILayoutUtility.GetLastRect( );
            Rect element = rect;
            element.x = rectLast.x;
            element.y += rect.height;
            element.width = rectLast.width;
            element.height = rectLast.y - rect.y;
            OnGUIInspect( element, EditMode.SIMPLE, obj, list );
        }
        

        private void OnGUIObjectFiled( Rect rect, EditMode editMode, UnityEngine.Object obj )
        {
            GameObject gobj = v_GameObject;
            
            if( gobj != null )
            {
                rect.width -= 35;
            }
            
            switch( m_Type )
            {
            case SerializeValue.Type.Bool:
                {
                    bool value = v_Bool;
                    bool next = EditorGUI.Toggle( rect, value );
                    if( value != next )
                    {
                        if( obj != null ) UnityEditor.Undo.RecordObjects( new UnityEngine.Object[]{ obj }, "SerializeValue" );
                        v_Bool = next;
                        if( obj != null ) EditorUtility.SetDirty( obj );
                    }
                }
                break;
            case SerializeValue.Type.Int:
                {
                    int value = v_Int;
                    int next = EditorGUI.IntField( rect, value );
                    if( value != next )
                    {
                        if( obj != null ) UnityEditor.Undo.RecordObjects( new UnityEngine.Object[]{ obj }, "SerializeValue" );
                        v_Int = next;
                        if( obj != null ) EditorUtility.SetDirty( obj );
                    }
                }
                break;
            case SerializeValue.Type.Long:
                {
                    long value = v_Long;
                    long next = EditorGUI.LongField( rect, value );
                    if( value != next )
                    {
                        if( obj != null ) UnityEditor.Undo.RecordObjects( new UnityEngine.Object[]{ obj }, "SerializeValue" );
                        v_Long = next;
                        if( obj != null ) EditorUtility.SetDirty( obj );
                    }
                }
                break;
            case SerializeValue.Type.Float:
                {
                    float value = v_Float;
                    float next = EditorGUI.FloatField( rect, value );
                    if( value != next )
                    {
                        if( obj != null ) UnityEditor.Undo.RecordObjects( new UnityEngine.Object[]{ obj }, "SerializeValue" );
                        v_Float = next;
                        if( obj != null ) EditorUtility.SetDirty( obj );
                    }
                }
                break;
            case SerializeValue.Type.String:
                {
                    string value = v_String;
                    string next = EditorGUI.TextField( rect, value );
                    if( value != next )
                    {
                        if( obj != null ) UnityEditor.Undo.RecordObjects( new UnityEngine.Object[]{ obj }, "SerializeValue" );
                        v_String = next;
                        if( obj != null ) EditorUtility.SetDirty( obj );
                    }
                }
                break;
            case SerializeValue.Type.Vector2:
                {
                    Vector2 value = v_Vector2;
                    Vector2 next = EditorGUI.Vector2Field( rect, "", value );
                    if( value != next )
                    {
                        if( obj != null ) UnityEditor.Undo.RecordObjects( new UnityEngine.Object[]{ obj }, "SerializeValue" );
                        v_Vector2 = next;
                        if( obj != null ) EditorUtility.SetDirty( obj );
                    }
                }
                break;
            case SerializeValue.Type.Vector3:
                {
                    Vector3 value = v_Vector3;
                    Vector3 next = EditorGUI.Vector3Field( rect, "", value );
                    if( value != next )
                    {
                        if( obj != null ) UnityEditor.Undo.RecordObjects( new UnityEngine.Object[]{ obj }, "SerializeValue" );
                        v_Vector3 = next;
                        if( obj != null ) EditorUtility.SetDirty( obj );
                    }
                }
                break;
            case SerializeValue.Type.Vector4:
                {
                    Vector4 value = v_Vector4;
                    Vector4 next = EditorGUI.Vector4Field( rect, "", value );
                    if( value != next )
                    {
                        if( obj != null ) UnityEditor.Undo.RecordObjects( new UnityEngine.Object[]{ obj }, "SerializeValue" );
                        v_Vector4 = next;
                        if( obj != null ) EditorUtility.SetDirty( obj );
                    }
                }
                break;
            case SerializeValue.Type.Color:
                {
                    Color value = v_Color;
                    Color next = EditorGUI.ColorField(  rect, "", value );
                    if( value != next )
                    {
                        if( obj != null ) UnityEditor.Undo.RecordObjects( new UnityEngine.Object[]{ obj }, "SerializeValue" );
                        v_Color = next;
                        if( obj != null ) EditorUtility.SetDirty( obj );
                    }
                }
                break;
            case SerializeValue.Type.GameObject:
                {
                    GameObject value = v_GameObject;
                    GameObject next = EditorGUI.ObjectField( rect, value, typeof( GameObject ), true ) as GameObject;
                    if( value != next )
                    {
                        if( obj != null ) UnityEditor.Undo.RecordObjects( new UnityEngine.Object[]{ obj }, "SerializeValue" );
                        v_GameObject = next;
                        if( obj != null ) EditorUtility.SetDirty( obj );
                    }
                }
                break;
            case SerializeValue.Type.UILabel:
                {
                    #if USE_TEXTMESHPRO
                    if( gobj == null )
                    {
                        GameObject next = EditorGUI.ObjectField( rect, null, typeof( GameObject ), true ) as GameObject;
                        if( next != null )
                        {
                            if( obj != null ) UnityEditor.Undo.RecordObjects( new UnityEngine.Object[]{ obj }, "SerializeValue" );
                            if( next.gameObject.GetComponent<TMPro.TextMeshProUGUI>( ) != null || 
                                next.gameObject.GetComponent<UnityEngine.UI.Text>( ) != null )
                            {
                                v_GameObject = next.gameObject;
                            }
                            if( obj != null ) EditorUtility.SetDirty( obj );
                        }
                        break;
                    }
                    if( gobj != null )
                    {
                        TMPro.TextMeshProUGUI value = gobj.GetComponent<TMPro.TextMeshProUGUI>( );
                        if( value != null )
                        {
                            TMPro.TextMeshProUGUI next = EditorGUI.ObjectField( rect, value, typeof( TMPro.TextMeshProUGUI ), true ) as TMPro.TextMeshProUGUI;
                            if( value != next )
                            {
                                if( obj != null ) UnityEditor.Undo.RecordObjects( new UnityEngine.Object[]{ obj }, "SerializeValue" );
                                v_GameObject = next != null ? next.gameObject: null;
                                if( obj != null ) EditorUtility.SetDirty( obj );
                            }
                            break;
                        }
                    }
                    #endif
                    {
                        UnityEngine.UI.Text value = v_UILabel;
                        UnityEngine.UI.Text next = EditorGUI.ObjectField( rect, value, typeof( UnityEngine.UI.Text ), true ) as UnityEngine.UI.Text;
                        if( value != next )
                        {
                            if( obj != null ) UnityEditor.Undo.RecordObjects( new UnityEngine.Object[]{ obj }, "SerializeValue" );
                            v_UILabel = next;
                            if( obj != null ) EditorUtility.SetDirty( obj );
                        }
                    }
                }
                break;
            case SerializeValue.Type.UIRawImage:
                {
                    UnityEngine.UI.RawImage value = v_UIRawImage;
                    UnityEngine.UI.RawImage next = EditorGUI.ObjectField( rect, value, typeof( UnityEngine.UI.RawImage ), true ) as UnityEngine.UI.RawImage;
                    if( value != next )
                    {
                        if( obj != null ) UnityEditor.Undo.RecordObjects( new UnityEngine.Object[]{ obj }, "SerializeValue" );
                        v_UIRawImage = next;
                        if( obj != null ) EditorUtility.SetDirty( obj );
                    }
                }
                break;
            case SerializeValue.Type.UIImage:
                {
                    UnityEngine.UI.Image value = v_UIImage;
                    UnityEngine.UI.Image next = EditorGUI.ObjectField( rect, value, typeof( UnityEngine.UI.Image ), true ) as UnityEngine.UI.Image;
                    if( value != next )
                    {
                        if( obj != null ) UnityEditor.Undo.RecordObjects( new UnityEngine.Object[]{ obj }, "SerializeValue" );
                        v_UIImage = next;
                        if( obj != null ) EditorUtility.SetDirty( obj );
                    }
                }
                break;
            case SerializeValue.Type.UIButton:
                {
                    UnityEngine.UI.Button value = v_UIButton;
                    UnityEngine.UI.Button next = EditorGUI.ObjectField( rect, value, typeof( UnityEngine.UI.Button ), true ) as UnityEngine.UI.Button;
                    if( value != next )
                    {
                        if( obj != null ) UnityEditor.Undo.RecordObjects( new UnityEngine.Object[]{ obj }, "SerializeValue" );
                        v_UIButton = next;
                        if( obj != null ) EditorUtility.SetDirty( obj );
                    }
                }
                break;
            case SerializeValue.Type.UIToggle:
                {
                    UnityEngine.UI.Toggle value = v_UIToggle;
                    UnityEngine.UI.Toggle next = EditorGUI.ObjectField( rect, value, typeof( UnityEngine.UI.Toggle ), true ) as UnityEngine.UI.Toggle;
                    if( value != next )
                    {
                        if( obj != null ) UnityEditor.Undo.RecordObjects( new UnityEngine.Object[]{ obj }, "SerializeValue" );
                        v_UIToggle = next;
                        if( obj != null ) EditorUtility.SetDirty( obj );
                    }
                }
                break;
            case SerializeValue.Type.Object:
                {
                    if( Application.isPlaying == false )
                    {
                        EditorGUILayout.LabelField( "只在执行中有效", GUILayout.Width( 90f ) );
                    }
                    else 
                    {
                        object vobj = v_Object;
                        EditorGUILayout.LabelField( vobj == null ? "null": vobj.ToString( ) );
                    }
                }
                break;
            case SerializeValue.Type.Global:
                {
                    int index = -1;
                    string[] fieldStrings = GetGlobalTbl( GetGlobal( ), ref index );
                    int next = EditorGUI.Popup( rect, index, fieldStrings );
                    if( next != index )
                    {
                        if( obj != null ) UnityEditor.Undo.RecordObjects( new UnityEngine.Object[]{ obj }, "SerializeValue" );
                        SetGlobal( fieldStrings[ next ] );
                        if( obj != null ) EditorUtility.SetDirty( obj );
                    }
                }
                break;
            }
            
            rect.x += rect.width;
            
            // 
            if( gobj != null )
            {
                rect.x += 2;
                rect.width = 35 - 2;
                GUI.color = gobj.activeSelf ? Color.white: Color.gray;
                if( GUI.Button( rect, gobj.activeSelf ? "ON": "OFF" ) )
                {
                    gobj.SetActive( !gobj.activeSelf );
                }
                GUI.color = Color.white;
                rect.x += rect.width;
            }
        }
        
        
        public static string[] GetGlobalTbl( string value, ref int index )
        {
            List<string> fieldStrings = new List<string>();
            System.Reflection.FieldInfo[] fields = TYPE_GLOBAL.GetFields( System.Reflection.BindingFlags.Public|System.Reflection.BindingFlags.Static );
            for( int i = 0; i < fields.Length; ++i )
            {
                if( fields[i].IsLiteral == false )
                {
                    fieldStrings.Add( fields[ i ].Name );
                }
            }
            index = fieldStrings.FindIndex( ( prop ) => prop == value );
            return fieldStrings.ToArray( );
        }
        #endif
        
        private SerializeValue( )
        {
            m_Type          = Type.NONE;
            m_Key           = null;
            m_Obj           = null;
            m_Serialize     = null;
            m_Work          = null;
        }
   
        private SerializeValue( SerializeValue v )
        {
            m_Group         = v.m_Group;
            m_Type          = v.m_Type;
            m_Key           = v.m_Key;
            m_Obj           = v.m_Obj;
            m_Serialize     = v.m_Serialize;
            m_Work          = v.m_Work;
            #if UNITY_EDITOR
            __foldout       = v.__foldout;
            #endif
        }

        private SerializeValue( Type type, string key, string serialize )
        {
            m_Type          = type;
            m_Key           = key;
            m_Obj           = null;
            m_Serialize     = serialize;
            m_Work          = null;
        }

        private SerializeValue( Type type, string key )
        {
            m_Type          = type;
            m_Key           = key;
            m_Obj           = null;
            m_Serialize     = null;
            m_Work          = null;
            
            switch( type )
            {
            case Type.Bool:
                _SetBool( false );
                break;
            case Type.Int:
                _SetInt( 0 );
                break;
            case Type.Long:
                _SetLong( 0 );
                break;
            case Type.Float:
                _SetFloat( 0.0f );
                break;
            case Type.String:
                _SetString( "" );
                break;
            case Type.Vector2:
                _SetVector2( Vector2.zero );
                break;
            case Type.Vector3:
                _SetVector3( Vector3.zero );
                break;
            case Type.Vector4:
                _SetVector4( Vector4.zero );
                break;
            case Type.Color:
                _SetColor( Color.white );
                break;
            }
        }
        
        private SerializeValue( string key, bool value )
        {
            m_Type          = Type.Bool;
            m_Key           = key;
            m_Obj           = null;
            m_Serialize     = null;
            
            _SetBool( value );
        }
        private SerializeValue( bool value ) : this( null, value )
        {
        }
        
        private SerializeValue( string key, int value )
        {
            m_Type          = Type.Int;
            m_Key           = key;
            m_Obj           = null;
            m_Serialize     = null;
            
            _SetInt( value );
        }
        private SerializeValue( int value ): this( null, value )
        {
        }
        
        private SerializeValue( string key, long value )
        {
            m_Type          = Type.Long;
            m_Key           = key;
            m_Obj           = null;
            m_Serialize     = null;
            
            _SetLong( value );
        }
        private SerializeValue( long value ): this( null, value )
        {
        }
        
        private SerializeValue( string key, float value )
        {
            m_Type          = Type.Float;
            m_Key           = key;
            m_Obj           = null;
            m_Serialize     = null;
            
            _SetFloat( value );
        }
        private SerializeValue( float value ): this( null, value )
        {
        }
        
        private SerializeValue( string key, string value )
        {
            m_Type          = Type.String;
            m_Key           = key;
            m_Obj           = null;
            m_Serialize     = null;
            
            _SetString( value );
        }
        private SerializeValue( string value ): this( null, value )
        {
        }
        
        private SerializeValue( string key, Vector2 value )
        {
            m_Type          = Type.Vector2;
            m_Key           = key;
            m_Obj           = null;
            m_Serialize     = null;
            
            _SetVector2( value );
        }
        private SerializeValue( Vector2 value ): this( null, value )
        {
        }
        
        private SerializeValue( string key, Vector3 value )
        {
            m_Type          = Type.Vector3;
            m_Key           = key;
            m_Obj           = null;
            m_Serialize     = null;
            
            _SetVector3( value );
        }
        private SerializeValue( Vector3 value ): this( null, value )
        {
        }
        
        private SerializeValue( string key, Vector4 value )
        {
            m_Type          = Type.Vector4;
            m_Key           = key;
            m_Obj           = null;
            m_Serialize     = null;
            
            _SetVector4( value );
        }
        private SerializeValue( Vector4 value ): this( null, value )
        {
        }
        
        private SerializeValue( string key, Color value )
        {
            m_Type          = Type.Color;
            m_Key           = key;
            m_Obj           = null;
            m_Serialize     = null;
            
            _SetColor( value );
        }
        private SerializeValue( Color value ): this( null, value )
        {
        }
        
        private SerializeValue( string key, GameObject obj )
        {
            m_Type          = Type.GameObject;
            m_Key           = key;
            m_Obj           = obj;
            m_Serialize     = null;
            m_Work          = null;
        }
        private SerializeValue( GameObject value ): this( null, value )
        {
        }
        
        private SerializeValue( string key, UnityEngine.UI.Text obj )
        {
            m_Type          = Type.UILabel;
            m_Key           = key;
            m_Obj           = obj.gameObject;
            m_Serialize     = null;
            m_Work          = null;
        }
        private SerializeValue( UnityEngine.UI.Text value ): this( null, value )
        {
        }
        
        private SerializeValue( string key, UnityEngine.UI.RawImage obj )
        {
            m_Type          = Type.UIRawImage;
            m_Key           = key;
            m_Obj           = obj.gameObject;
            m_Serialize     = null;
            m_Work          = null;
        }
        private SerializeValue( UnityEngine.UI.RawImage value ): this( null, value )
        {
        }
        
        private SerializeValue( string key, UnityEngine.UI.Image obj )
        {
            m_Type          = Type.UIImage;
            m_Key           = key;
            m_Obj           = obj.gameObject;
            m_Serialize     = null;
            m_Work          = null;
        }
        private SerializeValue( UnityEngine.UI.Image value ): this( null, value )
        {
        }
        
        private SerializeValue( string key, UnityEngine.UI.Button obj )
        {
            m_Type          = Type.UIButton;
            m_Key           = key;
            m_Obj           = obj.gameObject;
            m_Serialize     = null;
            m_Work          = null;
        }
        private SerializeValue( UnityEngine.UI.Button value ): this( null, value )
        {
        }
        
        private SerializeValue( string key, UnityEngine.UI.Toggle obj )
        {
            m_Type          = Type.UIToggle;
            m_Key           = key;
            m_Obj           = obj.gameObject;
            m_Serialize     = null;
            m_Work          = null;
        }
        private SerializeValue( UnityEngine.UI.Toggle value ): this( null, value )
        {
        }
        
        private void _Reset()
        {
            switch( m_Type )
            {
            case Type.Bool:
            {
                bool tmp = true;
                if( bool.TryParse( m_Serialize, out tmp ) )
                {
                    m_Work  = tmp ? s_BoxedTrue : s_BoxedFalse;
                }
                break;
            }
            case Type.Int:
            {
                int tmp = 0;
                if( int.TryParse( m_Serialize, out tmp ) )
                {
                    m_Work  = tmp;
                }
                break;
            }
            case Type.Long:
            {
                long tmp = 0;
                if( long.TryParse( m_Serialize, out tmp ) )
                {
                    m_Work  = tmp;
                }
                break;
            }
            case Type.Float:
            {
                float tmp = 0;
                if( float.TryParse( m_Serialize, out tmp ) )
                {
                    m_Work  = tmp;
                }
                break;
            }
            case Type.String:
            {
                m_Work = m_Serialize;
                break;
            }
            case Type.Vector2:
            {
                m_Work = Vector2_Parse( m_Serialize );
                break;
            }
            case Type.Vector3:
            {
                m_Work = Vector3_Parse( m_Serialize );
                break;
            }
            case Type.Vector4:
            {
                m_Work = Vector4_Parse( m_Serialize );
                break;
            }
            case Type.Color:
            {
                m_Work = Color_Parse( m_Serialize );
                break;
            }
            case Type.Object:
            {
                break;
            }
            default:
                m_Work = null;
                break;
            }
        }
        

        public void Save()
        {
            if( m_Work == null )
            {
                _Reset( );
            }
            
            switch( m_Type )
            {
            case Type.Bool:
            {
                m_Serialize = (bool)m_Work ? "True" : "False";
                break;
            }
            case Type.Int:
            {
                m_Serialize = m_Work.ToString( );
                break;
            }
            case Type.Long:
            {
                m_Serialize = m_Work.ToString( );
                break;
            }
            case Type.Float:
            {
                m_Serialize = m_Work.ToString( );
                break;
            }
            case Type.String:
            {
                m_Serialize = m_Work.ToString( );
                break;
            }
            case Type.Vector2:
            {
                Vector2 value = (Vector2)m_Work;
                m_Serialize = "(" + value.x.ToString() + "," + value.y.ToString() + ")";
                break;
            }
            case Type.Vector3:
            {
                Vector3 value = (Vector3)m_Work;
                m_Serialize = "(" + value.x.ToString() + "," + value.y.ToString() + "," + value.z.ToString() + ")";
                break;
            }
            case Type.Vector4:
            {
                Vector4 value = (Vector4)m_Work;
                m_Serialize = "(" + value.x.ToString() + "," + value.y.ToString() + "," + value.z.ToString() + ")";
                break;
            }
            case Type.Color:
            {
                m_Serialize = m_Work.ToString();
                break;
            }
            case Type.Object:
            {
                break;
            }
            default:
                m_Work = null;
                break;
            }
        }
        

        private void _Write( SerializeValue src )
        {
            switch( src.type )
            {
            case Type.Bool:                         v_Bool              = src.v_Bool;               break;
            case Type.Int:                          v_Int               = src.v_Int;                break;
            case Type.Long:                         v_Long              = src.v_Long;               break;
            case Type.Float:                        v_Float             = src.v_Float;              break;
            case Type.String:                       v_String            = src.v_String;             break;
            case Type.Vector2:                      v_Vector2           = src.v_Vector2;            break;
            case Type.Vector3:                      v_Vector3           = src.v_Vector3;            break;
            case Type.Vector4:                      v_Vector4           = src.v_Vector4;            break;
            case Type.Color:                        v_Color             = src.v_Color;              break;
            case Type.GameObject:                   v_GameObject        = src.v_GameObject;         break;
            case Type.UILabel:                      v_UILabel           = src.v_UILabel;            break;
            case Type.UIRawImage:                   v_UIRawImage        = src.v_UIRawImage;         break;
            case Type.UIImage:                      v_UIImage           = src.v_UIImage;            break;
            case Type.UIButton:                     v_UIButton          = src.v_UIButton;           break;
            case Type.UIToggle:                     v_UIToggle          = src.v_UIToggle;           break;
            case Type.Object:                       v_Object            = src.v_Object;             break;
            case Type.Global:                       SetGlobal( src.GetGlobal() );                   break;
            }
        }
        
  
        private void _Write( PropertyType propType, SerializeValue src )
        {
            switch( propType )
            {
            case PropertyType.Bool:                 v_Bool              = src.v_Bool;               break;
            case PropertyType.Int:                  v_Int               = src.v_Int;                break;
            case PropertyType.Long:                 v_Long              = src.v_Long;               break;
            case PropertyType.Float:                v_Float             = src.v_Float;              break;
            case PropertyType.String:               v_String            = src.v_String;             break;
            case PropertyType.Vector2:              v_Vector2           = src.v_Vector2;            break;
            case PropertyType.Vector3:              v_Vector3           = src.v_Vector3;            break;
            case PropertyType.Vector4:              v_Vector4           = src.v_Vector4;            break;
            case PropertyType.Color:                v_Color             = src.v_Color;              break;
            case PropertyType.GameObject:           v_GameObject        = src.v_GameObject;         break;
            case PropertyType.UILabel:              v_UILabel           = src.v_UILabel;            break;
            case PropertyType.UIRawImage:           v_UIRawImage        = src.v_UIRawImage;         break;
            case PropertyType.UIImage:              v_UIImage           = src.v_UIImage;            break;
            case PropertyType.UIButton:             v_UIButton          = src.v_UIButton;           break;
            case PropertyType.UIToggle:             v_UIToggle          = src.v_UIToggle;           break;
            case PropertyType.Object:               v_Object            = src.v_Object;             break;
            case PropertyType.Global:               v_Global            = src.v_Global;             break;
            case PropertyType.Active:               v_Active            = src.v_Active;             break;
            case PropertyType.Enable:               v_Enable            = src.v_Enable;             break;
            case PropertyType.UISelectable:         v_UISelectable      = src.v_UISelectable;       break;
            case PropertyType.UIText:               v_UIText            = src.v_UIText;             break;
            case PropertyType.UIInteractabel:       v_UIInteractable    = src.v_UIInteractable;     break;
            case PropertyType.UIOn:                 v_UIOn              = src.v_UIOn;               break;
            }
        }
        

        public string GetPropertyName( PropertyType propType )
        {
            switch( propType )
            {
            case PropertyType.Bool:                 return v_String;
            case PropertyType.Int:                  return v_String;
            case PropertyType.Long:                 return v_String;
            case PropertyType.Float:                return v_String;
            case PropertyType.String:               return v_String;
            case PropertyType.Vector2:              return v_String;
            case PropertyType.Vector3:              return v_String;
            case PropertyType.Vector4:              return v_String;
            case PropertyType.Color:                return v_String;
            case PropertyType.GameObject:           return v_String;
            case PropertyType.UILabel:              return v_String;
            case PropertyType.UIRawImage:           return v_String;
            case PropertyType.UIImage:              return v_String;
            case PropertyType.UIButton:             return v_String;
            case PropertyType.UIToggle:             return v_String;
            case PropertyType.Object:               return v_String;
            case PropertyType.Global:               return v_String;
            case PropertyType.Active:               return v_String;
            case PropertyType.Enable:               return v_String;
            case PropertyType.UISelectable:         return v_String;
            case PropertyType.UIText:               return v_String;
            case PropertyType.UIInteractabel:       return v_String;
            case PropertyType.UIOn:                 return v_String;
            }
            return "";
        }
        
  
        public bool Equal( SerializeValue src )
        {
            if( type == Type.String || src != null && src.type == Type.String )
            {
                string t1 = v_Object.ToString( );
                string t2 = src != null ? src.v_Object.ToString( ): "";
                return t1.Equals( t2 );
            }
            else if( type == Type.Bool || src != null && src.type == Type.Bool )
            {
                bool t1 = v_Bool;
                bool t2 = src != null ? src.v_Bool: false;
                return t1.Equals( t2 );
            }
            else if( type == Type.Int || src != null && src.type == Type.Int )
            {
                int t1 = v_Int;
                int t2 = src != null ? src.v_Int: 0;
                return t1.Equals( t2 );
            }
            else if( type == Type.Float || src != null && src.type == Type.Float )
            {
                float t1 = v_Float;
                float t2 = src != null ? src.v_Float: 0;
                return t1.Equals( t2 );
            }
            else
            {
                object t1 = v_Object;
                object t2 = src != null ? src.v_Object: null;
                if( t1 == null || t2 == null )
                {
                    if( t1 == null && t2 == null ) return true;
                    return false;
                }
                return t1.Equals( t2 );
            }
        }
        

        public bool Equal( PropertyType propType, SerializeValue src )
        {
            switch( propType )
            {
            case PropertyType.Bool:                 return v_Bool == src.v_Bool;
            case PropertyType.Int:                  return v_Int == src.v_Int;
            case PropertyType.Long:                 return v_Long == src.v_Long;
            case PropertyType.Float:                return v_Float == src.v_Float;
            case PropertyType.String:               return v_String == src.v_String;
            case PropertyType.Vector2:              return v_Vector2 == src.v_Vector2;
            case PropertyType.Vector3:              return v_Vector3 == src.v_Vector3;
            case PropertyType.Vector4:              return v_Vector4 == src.v_Vector4;
            case PropertyType.Color:                return v_Color == src.v_Color;
            case PropertyType.GameObject:           return v_GameObject == src.v_GameObject;
            case PropertyType.UILabel:              return v_UILabel == src.v_UILabel;
            case PropertyType.UIRawImage:           return v_UIRawImage == src.v_UIRawImage;
            case PropertyType.UIImage:              return v_UIImage == src.v_UIImage;
            case PropertyType.UIButton:             return v_UIButton == src.v_UIButton;
            case PropertyType.UIToggle:             return v_UIToggle == src.v_UIToggle;
            case PropertyType.Object:               return v_Object == src.v_Object;
            case PropertyType.Global:               return v_Global == src.v_Global;
            case PropertyType.Active:               return v_Active == src.v_Active;
            case PropertyType.Enable:               return v_Enable == src.v_Enable;
            case PropertyType.UISelectable:         return v_UISelectable == src.v_UISelectable;
            case PropertyType.UIText:               return v_UIText == src.v_UIText;
            case PropertyType.UIInteractabel:       return v_UIInteractable == src.v_UIInteractable;
            case PropertyType.UIOn:                 return v_UIOn == src.v_UIOn;
            }
            return false;
        }
        
 
        public bool Greater( SerializeValue src )
        {
            switch( type )
            {
            case Type.Int:                          return v_Int > ( src != null ? src.v_Int: 0 );
            case Type.Long:                         return v_Long > ( src != null ? src.v_Long: 0 );
            case Type.Float:                        return v_Float > ( src != null ? src.v_Float: 0 );
            }
            return false;
        }
        

        public bool Greater( PropertyType propType, SerializeValue src )
        {
            switch( propType )
            {
            case PropertyType.Int:                  return v_Int > ( src != null ? src.v_Int: 0 );
            case PropertyType.Long:                 return v_Long > ( src != null ? src.v_Long: 0 );
            case PropertyType.Float:                return v_Float > ( src != null ? src.v_Float: 0 );
            }
            return false;
        }
        
   
        public bool EqualGreater( SerializeValue src )
        {
            switch( type )
            {
            case Type.Int:                          return v_Int >= ( src != null ? src.v_Int: 0 );
            case Type.Long:                         return v_Long >= ( src != null ? src.v_Long: 0 );
            case Type.Float:                        return v_Float >= ( src != null ? src.v_Float: 0 );
            }
            return false;
        }
        
        public bool EqualGreater( PropertyType propType, SerializeValue src )
        {
            switch( propType )
            {
            case PropertyType.Int:                  return v_Int >= ( src != null ? src.v_Int: 0 );
            case PropertyType.Long:                 return v_Long >= ( src != null ? src.v_Long: 0 );
            case PropertyType.Float:                return v_Float >= ( src != null ? src.v_Float: 0 );
            }
            return false;
        }
        
   
        public bool Less( SerializeValue src )
        {
            switch( type )
            {
            case Type.Int:                          return v_Int < ( src != null ? src.v_Int: 0 );
            case Type.Long:                         return v_Long < ( src != null ? src.v_Long: 0 );
            case Type.Float:                        return v_Float < ( src != null ? src.v_Float: 0 );
            }
            return false;
        }
        
   
        public bool Less( PropertyType propType, SerializeValue src )
        {
            switch( propType )
            {
            case PropertyType.Int:                  return v_Int < ( src != null ? src.v_Int: 0 );
            case PropertyType.Long:                 return v_Long < ( src != null ? src.v_Long: 0 );
            case PropertyType.Float:                return v_Float < ( src != null ? src.v_Float: 0 );
            }
            return false;
        }
        

        public bool EqualLess( SerializeValue src )
        {
            switch( type )
            {
            case Type.Int:                          return v_Int <= ( src != null ? src.v_Int: 0 );
            case Type.Long:                         return v_Long <= ( src != null ? src.v_Long: 0 );
            case Type.Float:                        return v_Float <= ( src != null ? src.v_Float: 0 );
            }
            return false;
        }
        

        public bool EqualLess( PropertyType propType, SerializeValue src )
        {
            switch( propType )
            {
            case PropertyType.Int:                  return v_Int <= ( src != null ? src.v_Int: 0 );
            case PropertyType.Long:                 return v_Long <= ( src != null ? src.v_Long: 0 );
            case PropertyType.Float:                return v_Float <= ( src != null ? src.v_Float: 0 );
            }
            return false;
        }
        
        private void _SetBool( bool value )
        {
            if( Application.isPlaying )
            {
                m_Work = value ? s_BoxedTrue : s_BoxedFalse;
            }
            else
            {
                m_Serialize = value.ToString();
            }
        }
        
        private bool _GetBool( )
        {
            if( Application.isPlaying )
            {
                if (m_Work != null) return (bool)m_Work;
            }
            bool result = false;
            bool.TryParse( m_Serialize, out result );
            return result;
        }
        

        public bool v_Bool
        {
            private set
            {
                switch( m_Type )
                {
                case Type.Bool:
                    _SetBool( value );
                    break;
                case Type.Int:
                    _SetInt( value ? 1: 0 );
                    break;
                case Type.Long:
                    _SetLong( value ? 1: 0 );
                    break;
                case Type.Float:
                    _SetFloat( value ? 1.0f: 0.0f );
                    break;
                case Type.String:
                    _SetString( value.ToString() );
                    break;
                case Type.GameObject:
                    v_GameObject.SetActiveSafe( value );
                    break;
                case Type.Global:
                    v_Global = value;
                    break;
                case Type.Object:
                    _SetBool( value );
                    break;
                }
            }
            get
            {
                switch( m_Type )
                {
                case Type.Bool:
                    return _GetBool();
                case Type.Int:
                    return GetInt() == 1;
                case Type.Long:
                    return GetLong() == 1;
                case Type.Float:
                    return GetFloat() == 1.0f;
                case Type.String:
                    {
                        bool result = false;
                        if( bool.TryParse( GetString(), out result ) )
                        {
                            return result;
                        }
                    }
                    break;
                case Type.GameObject:
                    {
                        GameObject gobj = v_GameObject;
                        if( gobj != null )
                        {
                            return gobj.activeSelf;
                        }
                    }
                    break;
                case Type.Global:
                    {
                        object obj = v_Global;
                        if( obj != null )
                        {
                            System.Type objType = obj.GetType( );
                            if( objType == typeof(bool) )       return (bool)obj;
                            else if( objType == typeof(int) )   return (int)obj == 1;
                            else if( objType == typeof(float) ) return (float)obj == 1.0f;
                            else if( objType == typeof(string) )return (string)obj == "1";
                        }
                    }
                    break;
                case Type.Object:
                    {
                        object obj = v_Object;
                        return obj != null;
                    }
                }
                return false;
            }
        }
        
        private void _SetInt( int value )
        {
            if( Application.isPlaying )
            {
                m_Work = value;
            }
            else
            {
                m_Serialize = value.ToString();
            }
        }
        
        private int GetInt( )
        {
            if( Application.isPlaying )
            {
                if( m_Work != null ) return (int)m_Work;
            }
            int result = 0;
            int.TryParse( m_Serialize, out result );
            return result;
        }
        

        public int v_Int
        {
            private set
            {
                switch( m_Type )
                {
                case Type.Bool:
                    _SetBool( value==1 ? true: false );
                    break;
                case Type.Int:
                    _SetInt( value );
                    break;
                case Type.Float:
                    _SetFloat( (float)value );
                    break;
                case Type.String:
                    _SetString( value.ToString() );
                    break;
                case Type.GameObject:
                    {
                        GameObject gobj = v_GameObject;
                        #if USE_TEXTMESHPRO
                        TMPro.TextMeshProUGUI tmproLabel = gobj.GetComponent<TMPro.TextMeshProUGUI>( );
                        if( tmproLabel != null )
                        {
                            tmproLabel.SetText( value.ToString( ) );
                            break;
                        }
                        #endif
                        UnityEngine.UI.Text uiLabel = gobj.GetComponent<Text>( );
                        if( uiLabel != null )
                        {
                            uiLabel.text = value.ToString( );
                            break;
                        }
                    }
                    break;
                case Type.UILabel:
                    {
                        #if USE_TEXTMESHPRO
                        GameObject gobj = v_GameObject;
                        TMPro.TextMeshProUGUI tmproLabel = gobj.GetComponent<TMPro.TextMeshProUGUI>( );
                        if( tmproLabel != null )
                        {
                            tmproLabel.SetText( value.ToString( ) );
                            break;
                        }
                        #endif
                        UnityEngine.UI.Text label = v_UILabel;
                        if( label != null )
                        {
                            label.text = value.ToString();
                        }
                    }
                    break;
                case Type.Global:
                    v_Global = value;
                    break;
                case Type.Object:
                    _SetInt( value );
                    break;
                }
            }
            get
            {
                switch( m_Type )
                {
                case Type.Bool:
                    return _GetBool() ? 1: 0;
                case Type.Int:
                    return GetInt();
                case Type.Float:
                    return (int)GetFloat();
                case Type.String:
                    {
                        int result = 0;
                        if( int.TryParse( GetString(), out result ) )
                        {
                            return result;
                        }
                    }
                    break;
                case Type.UILabel:
                    UnityEngine.UI.Text label = v_UILabel;
                    if( label != null )
                    {
                        int result = 0;
                        if( int.TryParse( label.text, out result ) )
                        {
                            return result;
                        }
                    }
                    break;
                case Type.Global:
                    {
                        object obj = v_Global;
                        if( obj != null )
                        {
                            System.Type objType = obj.GetType( );
                            if( objType == typeof(bool) )       return (bool)obj ? 1: 0;
                            else if( objType == typeof(int) )   return (int)obj;
                            else if( objType == typeof(float) ) return (int)(float)obj;
                            else if( objType == typeof(string) )
                            {
                                int result = 0;
                                if( int.TryParse( GetString(), out result ) )
                                {
                                    return result;
                                }
                            }
                        }
                    }
                    break;
                }
                return 0;
            }
        }
        

        private void _SetLong( long value )
        {
            if( Application.isPlaying )
            {
                m_Work = value;
            }
            else
            {
                m_Serialize = value.ToString();
            }
        }
        

        private long GetLong( )
        {
            if( Application.isPlaying )
            {
                if( m_Work != null ) return (long)m_Work;
            }
            long result = 0;
            long.TryParse( m_Serialize, out result );
            return result;
        }
        

        public long v_Long
        {
            private set
            {
                switch( m_Type )
                {
                case Type.Bool:
                    _SetBool( value==1 ? true: false );
                    break;
                case Type.Int:
                    _SetInt( (int)value );
                    break;
                case Type.Long:
                    _SetLong( value );
                    break;
                case Type.Float:
                    _SetFloat( (float)value );
                    break;
                case Type.String:
                    _SetString( value.ToString() );
                    break;
                case Type.GameObject:
                    {
                        GameObject gobj = v_GameObject;
                        #if USE_TEXTMESHPRO
                        TMPro.TextMeshProUGUI tmproLabel = gobj.GetComponent<TMPro.TextMeshProUGUI>( );
                        if( tmproLabel != null )
                        {
                            tmproLabel.SetText( value.ToString( ) );
                            break;
                        }
                        #endif
                        UnityEngine.UI.Text uiLabel = gobj.GetComponent<Text>( );
                        if( uiLabel != null )
                        {
                            uiLabel.text = value.ToString( );
                            break;
                        }
                    }
                    break;
                case Type.UILabel:
                    {
                        #if USE_TEXTMESHPRO
                        GameObject gobj = v_GameObject;
                        TMPro.TextMeshProUGUI tmproLabel = gobj.GetComponent<TMPro.TextMeshProUGUI>( );
                        if( tmproLabel != null )
                        {
                            tmproLabel.SetText( value.ToString( ) );
                            break;
                        }
                        #endif
                        UnityEngine.UI.Text label = v_UILabel;
                        if( label != null )
                        {
                            label.text = value.ToString();
                        }
                    }
                    break;
                case Type.Global:
                    v_Global = value;
                    break;
                case Type.Object:
                    _SetLong( value );
                    break;
                }
            }
            get
            {
                switch( m_Type )
                {
                case Type.Bool:
                    return _GetBool() ? 1: 0;
                case Type.Int:
                    return (long)GetInt();
                case Type.Long:
                    return GetLong();
                case Type.Float:
                    return (long)GetFloat();
                case Type.String:
                    {
                        long result = 0;
                        if( long.TryParse( GetString(), out result ) )
                        {
                            return result;
                        }
                    }
                    break;
                case Type.UILabel:
                    UnityEngine.UI.Text label = v_UILabel;
                    if( label != null )
                    {
                        long result = 0;
                        if( long.TryParse( label.text, out result ) )
                        {
                            return result;
                        }
                    }
                    break;
                case Type.Global:
                    {
                        object obj = v_Global;
                        if( obj != null )
                        {
                            System.Type objType = obj.GetType( );
                            if( objType == typeof(bool) )       return (bool)obj ? 1: 0;
                            else if( objType == typeof(int) )   return (long)obj;
                            else if( objType == typeof(long) )  return (long)obj;
                            else if( objType == typeof(float) ) return (long)(float)obj;
                            else if( objType == typeof(string) )
                            {
                                long result = 0;
                                if( long.TryParse( GetString(), out result ) )
                                {
                                    return result;
                                }
                            }
                        }
                    }
                    break;
                }
                return 0;
            }
        }
        

        private void _SetFloat( float value )
        {
            if( Application.isPlaying )
            {
                m_Work = value;
            }
            else
            {
                m_Serialize = value.ToString();
            }
        }
        

        private float GetFloat( )
        {
            if( Application.isPlaying )
            {
                if( m_Work != null ) return (float)m_Work;
            }
            float result = 0;
            float.TryParse( m_Serialize, out result );
            return result;
        }
        

        public float v_Float
        {
            private set
            {
                switch( m_Type )
                {
                case Type.Bool:
                    _SetBool( value==1.0f ? true: false );
                    break;
                case Type.Int:
                    _SetInt( (int)value );
                    break;
                case Type.Long:
                    _SetLong( (long)value );
                    break;
                case Type.Float:
                    _SetFloat( value );
                    break;
                case Type.String:
                    _SetString( value.ToString() );
                    break;
                case Type.GameObject:
                    {
                        GameObject gobj = v_GameObject;
                        #if USE_TEXTMESHPRO
                        TMPro.TextMeshProUGUI tmproLabel = gobj.GetComponent<TMPro.TextMeshProUGUI>( );
                        if( tmproLabel != null )
                        {
                            tmproLabel.SetText( value.ToString( ) );
                            break;
                        }
                        #endif
                        UnityEngine.UI.Text uiLabel = gobj.GetComponent<Text>( );
                        if( uiLabel != null )
                        {
                            uiLabel.text = value.ToString( );
                            break;
                        }
                    }
                    break;
                case Type.UILabel:
                    {
                        #if USE_TEXTMESHPRO
                        GameObject gobj = v_GameObject;
                        TMPro.TextMeshProUGUI tmproLabel = gobj.GetComponent<TMPro.TextMeshProUGUI>( );
                        if( tmproLabel != null )
                        {
                            tmproLabel.SetText( value.ToString( ) );
                            break;
                        }
                        #endif
                        UnityEngine.UI.Text label = v_UILabel;
                        if( label != null )
                        {
                            label.text = value.ToString();
                        }
                    }
                    break;
                case Type.Global:
                    v_Global = value;
                    break;
                case Type.Object:
                    _SetFloat( value );
                    break;
                }
            }
            get
            {
                switch( m_Type )
                {
                case Type.Bool:
                    return _GetBool() ? 1.0f: 0.0f;
                case Type.Int:
                    return (float)GetInt();
                case Type.Long:
                    return (float)GetLong();
                case Type.Float:
                    return GetFloat();
                case Type.String:
                    {
                        float result = 0.0f;
                        if( float.TryParse( GetString(), out result ) )
                        {
                            return result;
                        }
                    }
                    break;
                case Type.UILabel:
                    UnityEngine.UI.Text label = v_UILabel;
                    if( label != null )
                    {
                        float result = 0;
                        if( float.TryParse( label.text, out result ) )
                        {
                            return result;
                        }
                    }
                    break;
                case Type.Global:
                    object obj = v_Global;
                    if( obj != null )
                    {
                        System.Type objType = obj.GetType( );
                        if( objType == typeof(bool) )       return (bool)obj ? 1.0f: 0.0f;
                        else if( objType == typeof(int) )   return (float)(int)obj;
                        else if( objType == typeof(float) ) return (float)obj;
                        else if( objType == typeof(string) )
                        {
                            float result = 0;
                            if( float.TryParse( GetString(), out result ) )
                            {
                                return result;
                            }
                        }
                    }
                    break;
                }
                return 0.0f;
            }
        }
        

        private void _SetString( string value )
        {
            if( Application.isPlaying )
            {
                m_Work = value;
            }
            else
            {
                m_Serialize = value;
            }
        }
        

        private string GetString( )
        {
            if( Application.isPlaying )
            {
                if( m_Work != null ) return (string)m_Work;
            }
            return m_Serialize == null ? "": m_Serialize;
        }
        

        public string v_String
        {
            private set
            {
                switch( m_Type )
                {
                case Type.Bool:
                    {
                        bool result = false;
                        if( bool.TryParse( value, out result ) )
                        {
                            _SetBool( result );
                        }
                        else
                        {
                            _SetBool( false );
                        }
                    }
                    break;
                case Type.Int:
                    {
                        int result = 0;
                        if( int.TryParse( value, out result ) )
                        {
                            _SetInt( result );
                        }
                        else
                        {
                            _SetInt( 0 );
                        }
                    }
                    break;
                case Type.Long:
                    {
                        long result = 0;
                        if( long.TryParse( value, out result ) )
                        {
                            _SetLong( result );
                        }
                        else
                        {
                            _SetLong( 0 );
                        }
                    }
                    break;
                case Type.Float:
                    {
                        float result = 0.0f;
                        if( float.TryParse( value, out result ) )
                        {
                            _SetFloat( result );
                        }
                        else
                        {
                            _SetFloat( 0.0f );
                        }
                    }
                    break;
                case Type.String:
                    _SetString( value );
                    break;
                case Type.GameObject:
                    {
                        GameObject gobj = v_GameObject;
                        #if USE_TEXTMESHPRO
                        var uiTextMesh = gobj.GetComponent<TextMeshProUGUI>( );
                        if( uiTextMesh != null )
                        {
                            uiTextMesh.SetText( value );
                            break;
                        }
                        #endif
                        UnityEngine.UI.Text label = gobj.GetComponent<UnityEngine.UI.Text>( );
                        if( label != null )
                        {
                           
                            {
                                label.text = value;
                            }
                        }
                    }
                    break;
                case Type.UILabel:
                    {
                        #if USE_TEXTMESHPRO
                        GameObject gobj = v_GameObject;
                        var uiTextMesh = gobj.GetComponent<TextMeshProUGUI>( );
                        if( uiTextMesh != null )
                        {
                            uiTextMesh.SetText( value );
                            break;
                        }
                        #endif
                        UnityEngine.UI.Text label = v_UILabel;
                        if( label != null )
                        {
                          
                            {
                                label.text = value;
                            }
                        }
                    }
                    break;
                case Type.Global:
                    v_Global = value;
                    break;
                case Type.Object:
                    _SetString( value );
                    break;
                }
            }
            get
            {
                switch( m_Type )
                {
                case Type.Bool:
                    return _GetBool().ToString();
                case Type.Int:
                    return GetInt().ToString();
                case Type.Long:
                    return GetLong().ToString();
                case Type.Float:
                    return GetFloat().ToString();
                case Type.String:
                    return GetString();
                case Type.GameObject:
                    {
                        GameObject gobj = v_GameObject;
                        if( gobj != null )
                        {
                            #if USE_TEXTMESHPRO
                            var uiTextMesh = gobj.GetComponent<TextMeshProUGUI>( );
                            if( uiTextMesh != null )
                            {
                                return uiTextMesh.text;
                            }
                            #endif
                            UnityEngine.UI.Text uiLabel = gobj.GetComponent<UnityEngine.UI.Text>( );
                            if( uiLabel != null )
                            {
                                return uiLabel.text;
                            }
                            else
                            {
                                return gobj.ToString( );
                            }
                        }
                    }
                    break;
                case Type.UILabel:
                    {
                        #if USE_TEXTMESHPRO
                        GameObject gobj = v_GameObject;
                        if( gobj != null )
                        {
                            TextMeshProUGUI uiTextMesh = gobj.GetComponent<TextMeshProUGUI>( );
                            if( uiTextMesh != null )
                            {
                                return uiTextMesh.text;
                            }
                        }
                        #endif
                        UnityEngine.UI.Text label = v_UILabel;
                        if( label != null )
                        {
                            return label.text;
                        }
                    }
                    break;
                case Type.Global:
                    {
                        object obj = v_Global;
                        if( obj != null )
                        {
                            System.Type objType = obj.GetType( );
                            if( objType == typeof(bool) ) return (bool)obj ? "1": "0";
                            else if( objType == typeof(int) ) return ((int)obj).ToString();
                            else if( objType == typeof(float) ) return ((float)obj).ToString();
                            else if( objType == typeof(string) ) return (string)obj;
                        }
                    }
                    break;
                case Type.Object:
                    {
                        object obj = v_Object;
                        if( obj != null )
                        {
                            return obj.ToString( );
                        }
                    }
                    break;
                }
                return "";
            }
        }
        
        private static Vector2 Vector2_Parse( string value )
        {
            if( string.IsNullOrEmpty( value ) == false )
            {
                value = value.Substring( 1, value.Length - 2 );
                string[] v = value.Split( ',' );
                if( v.Length == 2 )
                {
                    return new Vector2( float.Parse( v[ 0 ] ), float.Parse( v[ 1 ] ) );
                }
            }
            return Vector2.zero;
        }
        
        private void _SetVector2( Vector2 value )
        {
            if( Application.isPlaying )
            {
                m_Work = value;
            }
            else
            {
                //m_Serialize = value.ToString();
                // 
                 m_Serialize = "(" + value.x.ToString() + "," + value.y.ToString() + ")";
            }
        }
        

        private Vector2 GetVector2( )
        {
            if( Application.isPlaying )
            {
                if( m_Work != null ) return (Vector2)m_Work;
            }
            return Vector2_Parse( m_Serialize );
        }
        

        public Vector2 v_Vector2
        {
            private set
            {
                switch( m_Type )
                {
                case Type.Vector2:
                    _SetVector2( value );
                    break;
                case Type.Global:
                    v_Global = value;
                    break;
                case Type.Object:
                    _SetVector2( value );
                    break;
                }
            }
            get
            {
                switch( m_Type )
                {
                case Type.Vector2:
                    return GetVector2();
                case Type.Global:
                    {
                        object obj = v_Global;
                        if( obj.GetType() == typeof(Vector2) )
                        {
                            return (Vector2)obj;
                        }
                    }
                    break;
                }
                return Vector2.zero;
            }
        }
        
        private static Vector3 Vector3_Parse( string value )
        {
            if( string.IsNullOrEmpty( value ) == false )
            {
                value = value.Substring( 1, value.Length - 2 );
                string[] v = value.Split( ',' );
                if( v.Length == 3 )
                {
                    return new Vector3( float.Parse( v[ 0 ] ), float.Parse( v[ 1 ] ), float.Parse( v[ 2 ] ) );
                }
            }
            return Vector3.zero;
        }
        

        private void _SetVector3( Vector3 value )
        {
            if( Application.isPlaying )
            {
                m_Work = value;
            }
            else
            {
                //m_Serialize = value.ToString();
                // 
                m_Serialize = "(" + value.x.ToString() + "," + value.y.ToString() + "," + value.z.ToString() + ")";
            }
        }
        

        private Vector3 GetVector3( )
        {
            if( Application.isPlaying )
            {
                if( m_Work != null ) return (Vector3)m_Work;
            }
            return Vector3_Parse( m_Serialize );
        }
        

        public Vector3 v_Vector3
        {
            private set
            {
                switch( m_Type )
                {
                case Type.Vector3:
                    _SetVector3( value );
                    break;
                case Type.Global:
                    v_Global = value;
                    break;
                case Type.Object:
                    _SetVector3( value );
                    break;
                }
            }
            get
            {
                switch( m_Type )
                {
                case Type.Vector3:
                    return GetVector3();
                case Type.Global:
                    {
                        object obj = v_Global;
                        if( obj.GetType() == typeof(Vector3) )
                        {
                            return (Vector3)obj;
                        }
                    }
                    break;
                }
                return Vector3.zero;
            }
        }
        
        private static Vector4 Vector4_Parse( string value )
        {
            if( string.IsNullOrEmpty( value ) == false )
            {
                value = value.Substring( 1, value.Length - 2 );
                string[] v = value.Split( ',' );
                if( v.Length == 4 )
                {
                    return new Vector4( float.Parse( v[ 0 ] ), float.Parse( v[ 1 ] ), float.Parse( v[ 2 ] ), float.Parse( v[ 3 ] ) );
                }
            }
            return Vector4.zero;
        }
        

        private void _SetVector4( Vector4 value )
        {
            if( Application.isPlaying )
            {
                m_Work = value;
            }
            else
            {
                //m_Serialize = value.ToString();
                // 
                m_Serialize = "(" + value.x.ToString() + "," + value.y.ToString() + "," + value.z.ToString() + "," + value.w.ToString() + ")";
            }
        }
        

        private Vector4 GetVector4( )
        {
            if( Application.isPlaying )
            {
                if( m_Work != null ) return (Vector4)m_Work;
            }
            return Vector4_Parse( m_Serialize );
        }
        

        public Vector4 v_Vector4
        {
            private set
            {
                switch( m_Type )
                {
                case Type.Vector4:
                    _SetVector4( value );
                    break;
                case Type.Global:
                    v_Global = value;
                    break;
                case Type.Object:
                    _SetVector4( value );
                    break;
                }
            }
            get
            {
                switch( m_Type )
                {
                case Type.Vector4:
                    return GetVector4();
                case Type.Global:
                    {
                        object obj = v_Global;
                        if( obj.GetType() == typeof(Vector4) )
                        {
                            return (Vector4)obj;
                        }
                    }
                    break;
                }
                return Vector4.zero;
            }
        }
        

        private static Color Color_Parse( string value )
        {
            if( string.IsNullOrEmpty( value ) == false )
            {
                string tmp = Utility.GetStringPickup( value, "RGBA(", ")" );
                string[] v = tmp.Split( ',' );
                if( v.Length == 4 )
                {
                    return new Color( float.Parse( v[ 0 ] ), float.Parse( v[ 1 ] ), float.Parse( v[ 2 ] ), float.Parse( v[ 3 ] ) );
                }
            }
            return Color.white;
        }
        
        private void _SetColor( Color value )
        {
            if( Application.isPlaying )
            {
                m_Work = value;
            }
            else
            {
                m_Serialize = value.ToString();
            }
        }
        
        private Color GetColor( )
        {
            if( Application.isPlaying )
            {
                if( m_Work != null ) return (Color)m_Work;
            }
            return Color_Parse( m_Serialize );
        }
        
        public Color v_Color
        {
            private set
            {
                switch( m_Type )
                {
                case Type.Color:
                    _SetColor( value );
                    break;
                case Type.UILabel:
                    {
                        #if USE_TEXTMESHPRO
                        GameObject gobj = v_GameObject;
                        TextMeshProUGUI uiTextMesh = gobj.GetComponent<TextMeshProUGUI>( );
                        if( uiTextMesh != null )
                        {
                            uiTextMesh.color = value;
                            break;
                        }
                        #endif
                        UnityEngine.UI.Text label = v_UILabel;
                        if( label != null )
                        {
                            {
                                label.color = value;
                            }
                        }
                    }
                    break;
                case Type.UIRawImage:
                    {
                        UnityEngine.UI.RawImage img = v_UIRawImage;
                        if( img != null )
                        {
                            img.color = value;
                        }
                    }
                    break;
                case Type.UIImage:
                    {
                        UnityEngine.UI.Image img = v_UIImage;
                        if( img != null )
                        {
                            img.color = value;
                        }
                    }
                    break;
                case Type.GameObject:
                    {
                        GameObject gobj = v_GameObject;
                        UnityEngine.UI.Graphic graphic = gobj.GetComponent<UnityEngine.UI.Graphic>( );
                        if( graphic != null )
                        {
                            graphic.color = value;
                        }
                    }
                    break;
                case Type.Global:
                    v_Global = value;
                    break;
                case Type.Object:
                    _SetColor( value );
                    break;
                }
            }
            get
            {
                switch( m_Type )
                {
                case Type.Color:
                    return GetColor();
                case Type.UILabel:
                    {
                        #if USE_TEXTMESHPRO
                        GameObject gobj = v_GameObject;
                        TextMeshProUGUI uiTextMesh = gobj.GetComponent<TextMeshProUGUI>( );
                        if( uiTextMesh != null )
                        {
                            return uiTextMesh.color;
                        }
                        #endif
                        UnityEngine.UI.Text label = v_UILabel;
                        if( label != null )
                        {
                            {
                                return label.color;
                            }
                        }
                    }
                    break;
                case Type.GameObject:
                    {
                        GameObject gobj = v_GameObject;
                        UnityEngine.UI.Graphic graphic = gobj.GetComponent<UnityEngine.UI.Graphic>( );
                        if( graphic != null )
                        {
                            return graphic.color;
                        }
                    }
                    break;
                case Type.Global:
                    {
                        object obj = v_Global;
                        if( obj.GetType() == typeof(Color) )
                        {
                            return (Color)obj;
                        }
                    }
                    break;
                }
                return Color.white;
            }
        }
        
        public GameObject v_GameObject
        {
            private set
            {
                switch( m_Type )
                {
                case Type.GameObject:
                case Type.UILabel:
                case Type.UIRawImage:
                case Type.UIImage:
                case Type.UIButton:
                case Type.UIToggle:
                    m_Obj = value;
                    break;
                case Type.Global:
                    v_Global = value;
                    break;
                case Type.Object:
                    m_Obj = value;
                    break;
                }
            }
            get
            {
                switch( m_Type )
                {
                case Type.GameObject:
                case Type.UILabel:
                case Type.UIRawImage:
                case Type.UIImage:
                case Type.UIButton:
                case Type.UIToggle:
                    return m_Obj;
                case Type.Global:
                    {
                        object obj = v_Global;
                        if( obj != null && obj.GetType() == typeof(GameObject) )
                        {
                            return (GameObject)obj;
                        }
                    }
                    break;
                case Type.Object:
                    return m_Obj;
                }
                return null;
            }
        }
        
        public UnityEngine.UI.Text v_UILabel
        {
            private set
            {
                switch( m_Type )
                {
                case Type.GameObject:
                case Type.UILabel:
                    m_Obj = value != null ? value.gameObject: null;
                    break;
                case Type.Global:
                    v_Global = value;
                    break;
                case Type.Object:
                    m_Obj = value != null ? value.gameObject: null;
                    break;
                }
            }
            get
            {
                switch( m_Type )
                {
                case Type.GameObject:
                case Type.UILabel:
                    return m_Obj != null ? m_Obj.GetComponent<UnityEngine.UI.Text>(): null;
                case Type.Global:
                    {
                        object obj = v_Global;
                        if( obj.GetType() == typeof(UnityEngine.UI.Text) )
                        {
                            return (UnityEngine.UI.Text)obj;
                        }
                    }
                    break;
                }
                return null;
            }
        }
        
        
        public TextMeshProUGUI v_UITextMeshPro
        {
            private set
            {
                switch( m_Type )
                {
                    case Type.GameObject:
                    case Type.UILabel:
                        m_Obj = value != null ? value.gameObject: null;
                        break;
                    case Type.Global:
                        v_Global = value;
                        break;
                    case Type.Object:
                        m_Obj = value != null ? value.gameObject: null;
                        break;
                }
            }
            get
            {
                switch( m_Type )
                {
                    case Type.GameObject:
                    case Type.UILabel:
                        return m_Obj != null ? m_Obj.GetComponent<TextMeshProUGUI>(): null;
                    case Type.Global:
                    {
                        object obj = v_Global;
                        if( obj.GetType() == typeof(TextMeshProUGUI) )
                        {
                            return (TextMeshProUGUI)obj;
                        }
                    }
                        break;
                }
                return null;
            }
        }
        public UnityEngine.UI.RawImage v_UIRawImage
        {
            private set
            {
                switch( m_Type )
                {
                case Type.GameObject:
                case Type.UIRawImage:
                    m_Obj = value != null ? value.gameObject: null;
                    break;
                case Type.Global:
                    v_Global = value;
                    break;
                case Type.Object:
                    m_Obj = value != null ? value.gameObject: null;
                    break;
                }
            }
            get
            {
                switch( m_Type )
                {
                case Type.GameObject:
                case Type.UIRawImage:
                    return m_Obj != null ? m_Obj.GetComponent<UnityEngine.UI.RawImage>(): null;
                case Type.Global:
                    {
                        object obj = v_Global;
                        if( obj.GetType() == typeof(UnityEngine.UI.RawImage) )
                        {
                            return (UnityEngine.UI.RawImage)obj;
                        }
                    }
                    break;
                }
                return null;
            }
        }
        
        public UnityEngine.UI.Image v_UIImage
        {
            private set
            {
                switch( m_Type )
                {
                case Type.GameObject:
                case Type.UIImage:
                    m_Obj = value != null ? value.gameObject: null;
                    break;
                case Type.Global:
                    v_Global = value;
                    break;
                case Type.Object:
                    m_Obj = value != null ? value.gameObject: null;
                    break;
                }
            }
            get
            {
                switch( m_Type )
                {
                case Type.GameObject:
                case Type.UIImage:
                    return m_Obj != null ? m_Obj.GetComponent<UnityEngine.UI.Image>(): null;
                case Type.Global:
                    {
                        object obj = v_Global;
                        if( obj.GetType() == typeof(UnityEngine.UI.Image) )
                        {
                            return (UnityEngine.UI.Image)obj;
                        }
                    }
                    break;
                }
                return null;
            }
        }
        

        public UnityEngine.UI.Button v_UIButton
        {
            private set
            {
                switch( m_Type )
                {
                case Type.GameObject:
                case Type.UIButton:
                    m_Obj = value != null ? value.gameObject: null;
                    break;
                case Type.Global:
                    v_Global = value;
                    break;
                case Type.Object:
                    m_Obj = value != null ? value.gameObject: null;
                    break;
                }
            }
            get
            {
                switch( m_Type )
                {
                case Type.GameObject:
                case Type.UIButton:
                    return m_Obj != null ? m_Obj.GetComponent<UnityEngine.UI.Button>(): null;
                case Type.Global:
                    {
                        object obj = v_Global;
                        if( obj.GetType() == typeof(UnityEngine.UI.Button) )
                        {
                            return (UnityEngine.UI.Button)obj;
                        }
                    }
                    break;
                }
                return null;
            }
        }
        
        public UnityEngine.UI.Toggle v_UIToggle
        {
            private set
            {
                switch( m_Type )
                {
                case Type.GameObject:
                case Type.UIToggle:
                    m_Obj = value != null ? value.gameObject: null;
                    break;
                case Type.Global:
                    v_Global = value;
                    break;
                case Type.Object:
                    m_Obj = value != null ? value.gameObject: null;
                    break;
                }
            }
            get
            {
                switch( m_Type )
                {
                case Type.GameObject:
                case Type.UIToggle:
                    return m_Obj != null ? m_Obj.GetComponent<UnityEngine.UI.Toggle>(): null;
                case Type.Global:
                    {
                        object obj = v_Global;
                        if( obj.GetType() == typeof(UnityEngine.UI.Toggle) )
                        {
                            return (UnityEngine.UI.Toggle)obj;
                        }
                    }
                    break;
                }
                return null;
            }
        }
        

        private void SetGlobal( string value )
        {
            m_Serialize = value;
        }
        

        private string GetGlobal( )
        {
            return m_Serialize == null ? "": m_Serialize;
        }
        

        public object v_Global
        {
            private set
            {
                switch( m_Type )
                {
                case Type.Global:
                    {
                        System.Reflection.FieldInfo filedInfo = TYPE_GLOBAL.GetField( GetGlobal() );
                        if( filedInfo != null )
                        {
                            if( filedInfo.FieldType.IsEnum )
                            {
                                if( value.GetType( ) == typeof( string ) )
                                {
                                    object result = System.Enum.Parse( filedInfo.FieldType, (string)value );
                                    if( result != null )
                                    {
                                        filedInfo.SetValue( null, result );
                                    }
                                }
                                else if( value.GetType( ) == typeof( int ) )
                                {
                                    object result = System.Enum.ToObject( filedInfo.FieldType, value );
                                    if( result != null )
                                    {
                                        filedInfo.SetValue( null, result );
                                    }
                                }
                            }
                            else
                            {
                                if( value.GetType( ) == filedInfo.FieldType )
                                {
                                    filedInfo.SetValue( null, value );
                                }
                            }
                        }
                    }
                    break;
                }
            }
            get
            {
                switch( m_Type )
                {
                case Type.Global:
                    {
                        System.Reflection.FieldInfo filedInfo = TYPE_GLOBAL.GetField( GetGlobal() );
                        if( filedInfo != null )
                        {
                            return filedInfo.GetValue( null );
                        }
                    }
                    break;
                }
                return null;
            }
        }
        

        public object v_Object
        {
            private set
            {
                if( m_Type == Type.Global )
                {
                    v_Global = value;
                    return;
                }
                if( value != null )
                {
                    System.Type valueType = value.GetType( );
                    if( valueType == Utility.TYPE_BOOL )            v_Bool          = (bool)value;
                    else if( valueType == Utility.TYPE_INT )        v_Int           = (int)value;
                    else if( valueType == Utility.TYPE_LONG )       v_Long          = (long)value;
                    else if( valueType == Utility.TYPE_FLOAT )      v_Float         = (float)value;
                    else if( valueType == Utility.TYPE_STRING )     v_String        = (string)value;
                    else if( valueType == Utility.TYPE_GAMEOBJECT ) v_GameObject    = (GameObject)value;
                    else if( valueType == Utility.TYPE_RAWIMAGE )   v_UIRawImage    = (UnityEngine.UI.RawImage)value;
                    else if( valueType == Utility.TYPE_IMAGE )      v_UIImage       = (UnityEngine.UI.Image)value;
                    else if( valueType == Utility.TYPE_TEXT )       v_UILabel       = (UnityEngine.UI.Text)value;
                    else if( valueType == Utility.TYPE_BUTTON )     v_UIButton      = (UnityEngine.UI.Button)value;
                    else if( valueType == Utility.TYPE_TOGGLE )     v_UIToggle      = (UnityEngine.UI.Toggle)value;
                    else
                    {
                        m_Type = Type.Object;
                        m_Work = value;
                    }
                }
                else
                {
                    m_Type = Type.Object;
                    m_Work = value;
                }
            }
            get
            {
                switch( m_Type )
                {
                case Type.Bool:
                    return _GetBool( );
                case Type.Int:
                    return GetInt( );
                case Type.Long:
                    return GetLong( );
                case Type.Float:
                    return GetFloat( );
                case Type.String:
                    return GetString( );
                case Type.GameObject:
                case Type.UIRawImage:
                case Type.UIImage:
                case Type.UILabel:
                case Type.UIButton:
                case Type.UIToggle:
                    return m_Obj;
                case Type.Global:
                    return v_Global;
                case Type.Object:
                    return m_Obj != null ? m_Obj: m_Work;
                }
                return null;
            }
        }
        
        public UnityEngine.MonoBehaviour v_MonoBehaviour
        {
            get
            {
                switch( m_Type )
                {
                case Type.GameObject:
                    {
                        UnityEngine.UI.Text text = v_UILabel;
                        if( text != null ) return text;
                        UnityEngine.UI.RawImage rawImage = v_UIRawImage;
                        if( rawImage != null ) return rawImage;
                        UnityEngine.UI.Image image = v_UIImage;
                        if( image != null ) return image;
                        UnityEngine.UI.Button button = v_UIButton;
                        if( button != null ) return button;
                        UnityEngine.UI.Toggle toggl = v_UIToggle;
                        if( toggl != null ) return toggl;
                    }
                    break;
                case Type.UILabel:
                    return v_UILabel;
                case Type.UIRawImage:
                    return v_UIRawImage;
                case Type.UIImage:
                    return v_UIImage;
                case Type.UIButton:
                    return v_UIButton;
                case Type.UIToggle:
                    return v_UIToggle;
                case Type.Global:
                    {
                        object obj = v_Global;
                        if( obj.GetType() == typeof(UnityEngine.MonoBehaviour) )
                        {
                            return (UnityEngine.MonoBehaviour)obj;
                        }
                    }
                    break;
                }
                return null;
            }
        }
        
  
        public Component v_Component
        {
            get
            {
                return v_MonoBehaviour;
            }
        }
        
 
        public Transform v_Tranform
        {
            get
            {
                switch( m_Type )
                {
                case Type.GameObject:
                case Type.UIRawImage:
                case Type.UIImage:
                case Type.UILabel:
                case Type.UIButton:
                case Type.UIToggle:
                    return m_Obj != null ? m_Obj.transform : null;
                case Type.Global:
                case Type.Object:
                    return null;
                }
                return null;
            }
        }
        
 
        public bool v_Active
        {
            set
            {
                GameObject gobj = v_GameObject;
                if( gobj != null )
                {
                    gobj.SetActive( value );
                }
            }
            get
            {
                GameObject gobj = v_GameObject;
                if( gobj != null )
                {
                    return gobj.activeSelf;
                }
                else
                {
                    return v_Bool;
                }
            }
        }
        
  
        public bool v_Enable
        {
            set
            {
                UnityEngine.MonoBehaviour behaviour = v_MonoBehaviour;
                if( behaviour != null )
                {
                    behaviour.enabled = value;
                }
            }
            get
            {
                UnityEngine.MonoBehaviour behaviour = v_MonoBehaviour;
                if( behaviour != null )
                {
                    return behaviour.enabled;
                }
                else
                {
                    return v_Bool;
                }
            }
        }
        

        public UnityEngine.UI.Selectable v_UISelectable
        {
            set
            {
                switch( m_Type )
                {
                case Type.GameObject:
                case Type.UIImage:
                case Type.UIButton:
                case Type.UIToggle:
                    m_Obj = value != null ? value.gameObject: null;
                    break;
                case Type.Global:
                    v_Global = value;
                    break;
                }
            }
            get
            {
                switch( m_Type )
                {
                case Type.GameObject:
                case Type.UIImage:
                case Type.UIButton:
                case Type.UIToggle:
                    return m_Obj != null ? m_Obj.GetComponent<UnityEngine.UI.Selectable>(): null;
                case Type.Global:
                    {
                        object obj = v_Global;
                        if( obj.GetType() == typeof(UnityEngine.UI.Selectable) )
                        {
                            return (UnityEngine.UI.Selectable)obj;
                        }
                    }
                    break;
                }
                return null;
            }
        }
        

        public string v_UIText
        {
            set
            {
                UnityEngine.UI.Text label = v_UILabel;
                if( label != null )
                {
                    label.text = value;
                }
            }
            get
            {
                UnityEngine.UI.Text label = v_UILabel;
                if( label != null )
                {
                    return label.text;
                }
                else
                {
                    return v_String;
                }
            }
        }
        
     
        public bool v_UIInteractable
        {
            set
            {
                UnityEngine.UI.Selectable selectbale = v_UISelectable;
                if( selectbale != null )
                {
                    selectbale.interactable = value;
                }
            }
            get
            {
                UnityEngine.UI.Selectable selectbale = v_UISelectable;
                if( selectbale != null )
                {
                    return selectbale.IsInteractable();
                }
                else
                {
                    return v_Bool;
                }
            }
        }
        
 
        public bool v_UIOn
        {
            set
            {
                UnityEngine.UI.Toggle tgl = v_UIToggle;
                if( tgl != null )
                {
                    tgl.isOn = value;
                }
            }
            get
            {
                UnityEngine.UI.Toggle tgl = v_UIToggle;
                if( tgl != null )
                {
                    return tgl.isOn;
                }
                return v_Bool;
            }
        }
        
        public T GetEnum<T>( )
        {
            switch( m_Type )
            {
            case Type.Int:
                {
                    object obj = System.Enum.ToObject( typeof( T ), GetInt( ) );
                    if( obj != null ) return (T)obj;
                }
                break;
            case Type.String:
                {
                    string text = GetString( );
                    if( string.IsNullOrEmpty( text ) == false )
                    {
                        try
                        {
                            object obj = System.Enum.Parse( typeof( T ), text );
                            if( obj != null ) return (T)obj;
                        }
                        catch( System.Exception e )
                        {
                            Debug.LogError( e );
                            return default(T);
                        }
                    }
                }
                break;
            case Type.Object:
                {
                    object obj = v_Object;
                    if( obj != null )
                    {
                        if( obj != null ) return (T)obj;
                    }
                }
                break;
            }
            return default(T);
        }
    }
}
