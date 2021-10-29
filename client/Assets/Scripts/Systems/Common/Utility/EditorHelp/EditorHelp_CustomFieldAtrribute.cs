using UnityEngine;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using System.Runtime.CompilerServices;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.ProjectWindowCallback;
#endif

namespace EG
{
    
#if UNITY_EDITOR
    public interface CustomFieldInterface
    {
        void OnCustomProperty( CustomFieldAttribute attr, UnityEditor.SerializedProperty prop, float width );
    }
    #endif
    
    public class CustomFieldButtonAttribute : System.Attribute
    {
#if UNITY_EDITOR
        public string text;
        public string method;
        public CustomFieldButtonAttribute( string _text, string _method )
        {
            text = _text;
            method = _method;
        }
        #else
        public CustomFieldButtonAttribute( string _text, string _method )
        {
        }
        #endif
    }
    

    public class CustomFieldEditorButtonAttribute : System.Attribute
    {
        public const int Button1 = 0;
        
        #if UNITY_EDITOR
        public delegate void EditorButtonAction( UnityEditor.SerializedProperty prop );
        public static EditorButtonAction[] EditorButton = new EditorButtonAction[1];
        public string text;
        public int num;
        public CustomFieldEditorButtonAttribute( string _text, int _num )
        {
            text = _text;
            num = _num;
        }
        #else
        public CustomFieldEditorButtonAttribute( string _text, int _num )
        {
        }
        #endif
    }
    
    public class CustomEnumDesc : System.Attribute
    {
        #if UNITY_EDITOR
        public string text;
        public string file;
        public int line;
        public CustomEnumDesc( string _text, [CallerFilePath] string _file = "", [CallerLineNumber] int _line = -1 )
        {
            text = _text;
            file = _file;
            line = _line;
        }
        #else
        public CustomEnumDesc( string _text )
        {
        }
        #endif
    }
    

    public class CustomFieldHelpAttribute : System.Attribute
    {
        #if UNITY_EDITOR
        public string text;
        public CustomFieldHelpAttribute( string _text )
        {
            text = _text;
        }
        #else
        public CustomFieldHelpAttribute( string _text )
        {
        }
        #endif
    }
    

    public class CustomFieldDispCondAttribute : System.Attribute
    {
        public enum EOperator { And, Or }
        #if UNITY_EDITOR
        public EOperator ope;
        public string prop;
        public string value;
        public bool sw;
        public CustomFieldDispCondAttribute( EOperator _ope, string _prop, string _value, bool _sw )
        {
            ope = _ope;
            prop = _prop;
            value = _value;
            sw = _sw;
        }
        public CustomFieldDispCondAttribute( string _prop, string _value, bool _sw )
        {
            ope = EOperator.And;
            prop = _prop;
            value = _value;
            sw = _sw;
        }
        #else
        public CustomFieldDispCondAttribute( EOperator _ope, string _prop, string _value, bool _sw )
        {
        }
        public CustomFieldDispCondAttribute( string _prop, string _value, bool _sw )
        {
        }
        #endif
    }
    
    public class CustomFieldRangeAttribute : System.Attribute
    {
        #if UNITY_EDITOR
        public float min;
        public float max;
        public CustomFieldRangeAttribute( float _min, float _max )
        {
            min = _min;
            max = _max;
        }
        #else
        public CustomFieldRangeAttribute( float _min, float _max )
        {
        }
        #endif
    }
    
    public class CustomFieldGroupAttribute : System.Attribute
    {
        #if UNITY_EDITOR
        public string groupName;
        public CustomFieldGroupAttribute( string _groupName )
        {
            groupName = _groupName;
        }
        #else
        public CustomFieldGroupAttribute( string _groupName )
        {
        }
        #endif
    }
    

    public class CustomFieldAttribute : System.Attribute
    {
        public enum Type
        {
            Bool                ,
            String              ,
            Int                 ,
            Float               ,
            MonoBehaviour       ,
            ScriptableObject    ,
            ScriptableObjects   ,
            GameObject          ,
            GameObjects         ,
            GameObjectId        ,
            Component           ,
            Components          ,
            Shader              ,
            Material            ,
            Animator            ,
            TextAsset           ,
            TextAssets          ,
            Texture             ,
            Vector2             ,
            Vector3             ,
            Vector2Int          ,
            Vector3Int          ,
            Color               ,
            Colors              ,
            Curve               ,
            UITweenController   ,
            UIText              ,
            UIBitmapText        ,
            UIRawImage          ,
            UIImage             ,
            UISprite            ,
            UISprites           ,
            Object              ,
            Objects             ,
            Custom              ,
            Enum                ,
            Enums               ,
            StringEnum          ,
            StringAndEnum       ,
            BitFlag             ,
            UIImageArray        ,
            EventRef            ,

            //SG: added for Global (Global-only enums should start from here to prevent clashing with JP enums)
            Vector4 = 1000      ,
            //End SG
        }
        
        #if UNITY_EDITOR
        
        public string                           text;
        public Type                             type;
        public System.Type                      systemType;
        public System.Type                      tblType;
        public CustomFieldRangeAttribute        range;
        public CustomFieldHelpAttribute         help;
        public CustomFieldButtonAttribute       button;
        public CustomFieldEditorButtonAttribute editor_button;
        
        public CustomFieldAttribute( string _text, Type _type )
        {
            text = _text;
            type = _type;
        }
        public CustomFieldAttribute( string _text, Type _type, System.Type _systemType )
        {
            text = _text;
            type = _type;
            systemType = _systemType;
        }
        public CustomFieldAttribute( string _text, Type _type, System.Type _systemType, System.Type _tblType )
        {
            text = _text;
            type = _type;
            systemType = _systemType;
            tblType = _tblType;
        }
        public CustomFieldAttribute( string _text, System.Type _systemType )
        {
            text = _text;
            type = Type.Object;
            systemType = _systemType;
        }
        
        /// ***********************************************************************
        /// <summary>
        /// ＧＵＩ
        /// </summary>
        /// ***********************************************************************
        public void GUIField( System.Type objType, UnityEditor.SerializedProperty prop, float width )
        {
            if( string.IsNullOrEmpty( text ) ) text = prop.name;
            CustomFieldHelpAttribute help_obj = help;
            if( type == Type.ScriptableObjects || type == Type.GameObjects || type == Type.Components || type == Type.TextAssets || type == Type.UISprites || type == Type.Colors || type == Type.Objects || type == Type.Enums ) help_obj = null;
            if( help_obj != null || button != null || editor_button != null )
            {
                UnityEditor.EditorGUILayout.BeginVertical( "box" );
            }
            switch( type )
            {
            case Type.Custom:
                {
                    CustomFieldInterface custom = prop.serializedObject.targetObject as CustomFieldInterface;
                    if( custom != null )
                    {
                        custom.OnCustomProperty( this, prop, width );
                    }
                }
                break;
            case Type.ScriptableObjects:
                {
                    GUIStyle idxStyle = EditorHelp.GUIStyles.IndexStyle;
                    GUIField_Array( objType, prop, width, ( arrayProp, sel, max ) =>
                        {
                            int change = -1;
                            EditorGUILayout.BeginHorizontal( );
                            {
                                if( systemType == null )
                                {
                                    arrayProp.objectReferenceValue = EditorHelp.ObjectField( arrayProp.serializedObject.targetObject, " ", arrayProp.objectReferenceValue, typeof( ScriptableObject ), true );
                                }
                                else
                                {
                                    arrayProp.objectReferenceValue = EditorHelp.ObjectField( arrayProp.serializedObject.targetObject, " ", arrayProp.objectReferenceValue, systemType, true );
                                }
                                string[] nums = new string[max];
                                for( int i = 0; i < max; ++i ) nums[i] = i.ToString( );
                                int nextSel = EditorGUILayout.Popup( sel, nums, idxStyle, GUILayout.Width( 80f ) );
                                if( nextSel != sel )
                                {
                                    change = nextSel;
                                }
                            }
                            EditorGUILayout.EndHorizontal( );
                            return change;
                        } );
                }
                break;
            case Type.GameObjects:
                {
                    GUIStyle idxStyle = EditorHelp.GUIStyles.IndexStyle;
                    GUIField_Array( objType, prop, width, ( arrayProp, sel, max ) =>
                        {
                            int change = -1;
                            EditorGUILayout.BeginHorizontal( );
                            {
                                arrayProp.objectReferenceValue = EditorHelp.ObjectField( arrayProp.serializedObject.targetObject, " ", arrayProp.objectReferenceValue, typeof( GameObject ), true ) as GameObject;
                                string[] nums = new string[max];
                                for( int i = 0; i < max; ++i ) nums[i] = i.ToString( );
                                int nextSel = EditorGUILayout.Popup( sel, nums, idxStyle, GUILayout.Width( 80f ) );
                                if( nextSel != sel )
                                {
                                    change = nextSel;
                                }
                            }
                            EditorGUILayout.EndHorizontal( );
                            return change;
                        } );
                }
                break;
            case Type.Components:
                {
                    GUIStyle idxStyle = EditorHelp.GUIStyles.IndexStyle;
                    GUIField_Array( objType, prop, width, ( arrayProp, sel, max ) =>
                        {
                            int change = -1;
                            EditorGUILayout.BeginHorizontal( );
                            {
                                arrayProp.objectReferenceValue = EditorHelp.ObjectField( arrayProp.serializedObject.targetObject, " ", arrayProp.objectReferenceValue, systemType, true );
                                string[] nums = new string[max];
                                for( int i = 0; i < max; ++i ) nums[i] = i.ToString( );
                                int nextSel = EditorGUILayout.Popup( sel, nums, idxStyle, GUILayout.Width( 80f ) );
                                if( nextSel != sel )
                                {
                                    change = nextSel;
                                }
                            }
                            EditorGUILayout.EndHorizontal( );
                            return change;
                        } );
                }
                break;
            case Type.TextAssets:
                {
                    GUIStyle idxStyle = EditorHelp.GUIStyles.IndexStyle;
                    GUIField_Array( objType, prop, width, ( arrayProp, sel, max ) =>
                        {
                            int change = -1;
                            EditorGUILayout.BeginHorizontal( );
                            {
                                arrayProp.objectReferenceValue = EditorHelp.ObjectField( arrayProp.serializedObject.targetObject, " ", arrayProp.objectReferenceValue, typeof( TextAsset ), true ) as TextAsset;
                                string[] nums = new string[max];
                                for( int i = 0; i < max; ++i ) nums[i] = i.ToString( );
                                int nextSel = EditorGUILayout.Popup( sel, nums, idxStyle, GUILayout.Width( 80f ) );
                                if( nextSel != sel )
                                {
                                    change = nextSel;
                                }
                            }
                            EditorGUILayout.EndHorizontal( );
                            return change;
                        } );
                }
                break;
            case Type.UISprites:
                {
                    GUIStyle idxStyle = EditorHelp.GUIStyles.IndexStyle;
                    GUIField_Array( objType, prop, width, ( arrayProp, sel, max ) =>
                        {
                            int change = -1;
                            EditorGUILayout.BeginHorizontal( );
                            {
                                arrayProp.objectReferenceValue = EditorHelp.ObjectField( arrayProp.serializedObject.targetObject, " ", arrayProp.objectReferenceValue, typeof( Sprite ), true ) as Sprite;
                                string[] nums = new string[max];
                                for( int i = 0; i < max; ++i ) nums[i] = i.ToString( );
                                int nextSel = EditorGUILayout.Popup( sel, nums, idxStyle, GUILayout.Width( 80f ) );
                                if( nextSel != sel )
                                {
                                    change = nextSel;
                                }
                            }
                            EditorGUILayout.EndHorizontal( );
                            return change;
                        } );
                }
                break;
            case Type.Colors:
                {
                    GUIStyle idxStyle = EditorHelp.GUIStyles.IndexStyle;
                    GUIField_Array( objType, prop, width, ( arrayProp, sel, max ) =>
                        {
                            int change = -1;
                            EditorGUILayout.BeginHorizontal( );
                            {
                                arrayProp.colorValue = EditorHelp.ColorField( arrayProp.serializedObject.targetObject, " ", arrayProp.colorValue );
                                string[] nums = new string[max];
                                for( int i = 0; i < max; ++i ) nums[i] = i.ToString( );
                                int nextSel = EditorGUILayout.Popup( sel, nums, idxStyle, GUILayout.Width( 80f ) );
                                if( nextSel != sel )
                                {
                                    change = nextSel;
                                }
                            }
                            EditorGUILayout.EndHorizontal( );
                            return change;
                        } );
                }
                break;
            case Type.Object:
                {
                    EditorGUI.indentLevel++;
                    bool isFoldout = UnityEditor.EditorPrefs.GetBool( prop.name );
                    if( isFoldout )
                    {
                        GUI.contentColor = Color.white;
                        GUI.backgroundColor = new Color( 0, 0.6f, 0 );
                    }
                    else
                    {
                        GUI.contentColor = new Color(0.6f,0.6f,0.6f);
                        GUI.backgroundColor = new Color( 0, 0.5f, 0 );
                    }
                    if( GUILayout.Button( (isFoldout ? "▼ ": "△ ") + text, EditorHelp.GUIStyles.ButtonToggle ) )
                    {
                        isFoldout = !isFoldout;
                        UnityEditor.EditorPrefs.SetBool( prop.name, isFoldout );
                    }
                    GUI.contentColor = Color.white;
                    GUI.backgroundColor = Color.white;
                    if( isFoldout )
                    {
                        UnityEditor.EditorGUILayout.BeginVertical( "box" );
                        {
                            CustomFieldAttribute.OnInspectorGUI( systemType, prop, width );
                        }
                        UnityEditor.EditorGUILayout.EndVertical( );
                    }
                    EditorGUI.indentLevel--;
                }
                break;
            case Type.Objects:
                {
                    GUIStyle idxStyle = EditorHelp.GUIStyles.IndexStyle;
                    GUIField_Array( objType, prop, width, ( arrayProp, sel, max ) =>
                        {
                            EditorGUI.indentLevel++;
                            int change = -1;
                            bool isFoldout = UnityEditor.EditorPrefs.GetBool(arrayProp.propertyPath );
                            string[] nums = new string[max];
                            for( int i = 0; i < max; ++i ) nums[i] = i.ToString( );
                            EditorGUILayout.BeginHorizontal( );
                            {
                                if( isFoldout )
                                {
                                    GUI.contentColor = Color.white;
                                    GUI.backgroundColor = new Color( 0, 0.6f, 0 );
                                }
                                else
                                {
                                    GUI.contentColor = new Color(0.6f,0.6f,0.6f);
                                    GUI.backgroundColor = new Color( 0, 0.5f, 0 );
                                }
                                string elementName = "Element:" + sel.ToString( );
                                SerializedProperty keyProp = arrayProp.FindPropertyRelative( "key" );
                                if( keyProp == null ) keyProp = arrayProp.FindPropertyRelative( "Key" );
                                if( keyProp != null )
                                {
                                    if( keyProp.propertyType == SerializedPropertyType.String )
                                        elementName = keyProp.stringValue;
                                    else if( keyProp.propertyType == SerializedPropertyType.Enum )
                                        elementName = keyProp.enumValueIndex >= 0 && keyProp.enumValueIndex < keyProp.enumDisplayNames.Length ? keyProp.enumDisplayNames[ keyProp.enumValueIndex ]: "Unknown";
                                }
                                if( GUILayout.Button( (isFoldout ? "▼ ": "△ ") + elementName, EditorHelp.GUIStyles.ButtonToggle ) )
                                {
                                    isFoldout = !isFoldout;
                                    UnityEditor.EditorPrefs.SetBool( arrayProp.propertyPath, isFoldout );
                                }
                                GUI.contentColor = Color.white;
                                GUI.backgroundColor = Color.white;
                                int nextSel = EditorGUILayout.Popup( sel, nums, idxStyle, GUILayout.Width( 80f ) );
                                if( nextSel != sel )
                                {
                                    change = nextSel;
                                }
                            }
                            EditorGUILayout.EndHorizontal( );
                            GUI.contentColor = Color.white;
                            GUI.backgroundColor = Color.white;
                            if( isFoldout )
                            {
                                UnityEditor.EditorGUILayout.BeginVertical( "box" );
                                {
                                    CustomFieldAttribute.OnInspectorGUI( systemType, arrayProp, width );
                                }
                                UnityEditor.EditorGUILayout.EndVertical( );
                            }
                            EditorGUI.indentLevel--;
                            return change;
                        } );
                }
                break;
            case Type.Enums:
                {
                    GUIStyle idxStyle = EditorHelp.GUIStyles.IndexStyle;
                    GUIField_Array( objType, prop, width, ( arrayProp, sel, max ) =>
                        {
                            int change = -1;
                            EditorGUILayout.BeginHorizontal( );
                            {
                                if( systemType != null )
                                {
                                    string[] list = System.Enum.GetNames( systemType );
                                    string[] dispNames = new string[ list.Length ];
                                    System.Array.Copy( list, dispNames, list.Length );
                                    if( tblType != null )
                                    {
                                        System.Reflection.FieldInfo tblFieldInfo = tblType.GetField( systemType.Name + "Names", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance );
                                        if( tblFieldInfo != null )
                                        {
                                            if( tblFieldInfo.FieldType == typeof( string[] ) )
                                            {
                                                dispNames = (string[])tblFieldInfo.GetValue( null );
                                            }
                                            else if( tblFieldInfo.FieldType == typeof( Utility.EnumArray.Element[] ) )
                                            {
                                                Utility.EnumArray.Element[] elements = (Utility.EnumArray.Element[])tblFieldInfo.GetValue( null );
                                                list = new string[ elements.Length ];
                                                dispNames = new string[ elements.Length ];
                                                for( int i = 0; i < elements.Length; ++i )
                                                {
                                                    list[ i ] = elements[ i ].index.ToString( );
                                                    dispNames[ i ] = elements[ i ].dispName;
                                                }
                                            }
                                        }
                                    }
                                    for( int n = 0; n < dispNames.Length; ++n )
                                    {
                                        dispNames[n] = dispNames[n].Replace( '_', '/' );
                                    }
                                    
                                    int intValue = arrayProp.intValue;
                                    string stringValue = System.Enum.ToObject( systemType, intValue ).ToString( );
                                    
                                    CustomEnumDesc[] descs = null;
                                    System.Reflection.FieldInfo fieldInfo = systemType.GetField( stringValue );
                                    if( fieldInfo != null )
                                    {
                                        descs = (CustomEnumDesc[])fieldInfo.GetCustomAttributes( typeof( CustomEnumDesc ), true );
                                    }
                                    if( descs != null && descs.Length > 0 ) UnityEditor.EditorGUILayout.BeginVertical( "box" );
                                    
                                    if( string.IsNullOrEmpty( stringValue ) ) stringValue = System.Enum.GetNames( systemType )[0].ToString();
                                    int selectIndex = ArrayUtility.FindIndex<string>( list, ( n ) => n == stringValue );
                                    if( selectIndex == -1 ){ stringValue = ""; selectIndex = 0; }
                                    int nextView = EditorHelp.Popup( arrayProp.serializedObject.targetObject, selectIndex, dispNames );
                                    if( nextView != selectIndex )
                                    {
                                        arrayProp.enumValueIndex = Utility.EnumStringToInt( systemType, list[ nextView ] );
                                    }
                                    
                                    if( descs != null && descs.Length > 0 )
                                    {
                                        UnityEditor.EditorGUILayout.HelpBox( descs[ 0 ].text, MessageType.Info );
                                        UnityEditor.EditorGUILayout.EndVertical( );
                                    }
                                }
                                else
                                {
                                    arrayProp.enumValueIndex = EditorHelp.Popup( arrayProp.serializedObject.targetObject, arrayProp.enumValueIndex, arrayProp.enumDisplayNames );
                                }
                                string[] nums = new string[max];
                                for( int i = 0; i < max; ++i ) nums[i] = i.ToString( );
                                int nextSel = EditorGUILayout.Popup( sel, nums, idxStyle, GUILayout.Width( 80f ) );
                                if( nextSel != sel )
                                {
                                    change = nextSel;
                                }
                            }
                            EditorGUILayout.EndHorizontal( );
                            return change;
                        } );
                }
                break;
            case Type.StringEnum:
                {
                    if( systemType != null )
                    {
                        string[] list = System.Enum.GetNames( systemType );
                        string[] dispNames = new string[ list.Length ];
                        System.Array.Copy( list, dispNames, list.Length );
                        if( tblType != null )
                        {
                            System.Reflection.FieldInfo tblFieldInfo = tblType.GetField( systemType.Name + "Names", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance );
                            if( tblFieldInfo != null )
                            {
                                if( tblFieldInfo.FieldType == typeof( string[] ) )
                                {
                                    dispNames = (string[])tblFieldInfo.GetValue( null );
                                }
                                else if( tblFieldInfo.FieldType == typeof( Utility.EnumArray.Element[] ) )
                                {
                                    Utility.EnumArray.Element[] elements = (Utility.EnumArray.Element[])tblFieldInfo.GetValue( null );
                                    list = new string[ elements.Length ];
                                    dispNames = new string[ elements.Length ];
                                    for( int i = 0; i < elements.Length; ++i )
                                    {
                                        list[ i ] = elements[ i ].index.ToString( );
                                        dispNames[ i ] = elements[ i ].dispName;
                                    }
                                }
                            }
                        }
                        for( int n = 0; n < dispNames.Length; ++n )
                        {
                            dispNames[n] = dispNames[n].Replace( '_', '/' );
                        }
                        
                        CustomEnumDesc[] descs = null;
                        System.Reflection.FieldInfo fieldInfo = systemType.GetField( prop.stringValue );
                        if( fieldInfo != null )
                        {
                            descs = (CustomEnumDesc[])fieldInfo.GetCustomAttributes( typeof( CustomEnumDesc ), true );
                        }
                        if( descs != null && descs.Length > 0 ) UnityEditor.EditorGUILayout.BeginVertical( "box" );
                        
                        string stringValue = (string)prop.stringValue;
                        if( string.IsNullOrEmpty( stringValue ) ) stringValue = System.Enum.GetNames( systemType )[0].ToString();
                        int selectIndex = ArrayUtility.FindIndex<string>( list, ( n ) => n == stringValue );
                        if( selectIndex == -1 ){ stringValue = ""; selectIndex = 0; }
                        int nextView = EditorHelp.Popup( prop.serializedObject.targetObject, text, selectIndex, dispNames );
                        if( nextView != selectIndex )
                        {
                            prop.stringValue = list[ nextView ];
                        }
                        
                        if( descs != null && descs.Length > 0 )
                        {
                            var desc = descs[ 0 ];
                            UnityEditor.EditorGUILayout.HelpBox( desc.text + "\nenum : " + stringValue, MessageType.Info );
                            EditorGUILayout.BeginHorizontal( );
                            GUILayout.FlexibleSpace();
                            if ( GUILayout.Button("def", GUILayout.Width( 40f ) ))
                            {
                                UnityEditorInternal.InternalEditorUtility.OpenFileAtLineExternal(desc.file, desc.line + 1);
                            }
                            if ( GUILayout.Button("case", GUILayout.Width( 50f ) ))
                            {
                                var found = EditorHelp.FindLineInTextFile(desc.file, " case +" + systemType.Name + "." + stringValue + ":");
                                UnityEditorInternal.InternalEditorUtility.OpenFileAtLineExternal(desc.file, found == -1 ? desc.line + 1 : found);
                            }
                            EditorGUILayout.EndHorizontal( );
                            UnityEditor.EditorGUILayout.EndVertical( );
                        }
                    }
                }
                break;
            case Type.StringAndEnum:
                {
                    if( systemType != null )
                    {
                        string[] list = System.Enum.GetNames( systemType );
                        string[] dispNames = new string[ list.Length ];
                        System.Array.Copy( list, dispNames, list.Length );
                        if( tblType != null )
                        {
                            System.Reflection.FieldInfo tblFieldInfo = tblType.GetField( systemType.Name + "Names", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance );
                            if( tblFieldInfo != null )
                            {
                                if( tblFieldInfo.FieldType == typeof( string[] ) )
                                {
                                    dispNames = (string[])tblFieldInfo.GetValue( null );
                                }
                                else if( tblFieldInfo.FieldType == typeof( Utility.EnumArray.Element[] ) )
                                {
                                    Utility.EnumArray.Element[] elements = (Utility.EnumArray.Element[])tblFieldInfo.GetValue( null );
                                    list = new string[ elements.Length ];
                                    dispNames = new string[ elements.Length ];
                                    for( int i = 0; i < elements.Length; ++i )
                                    {
                                        list[ i ] = elements[ i ].index.ToString( );
                                        dispNames[ i ] = elements[ i ].dispName;
                                    }
                                }
                            }
                        }
                        for( int n = 0; n < dispNames.Length; ++n )
                        {
                            dispNames[n] = dispNames[n].Replace( '_', '/' );
                        }
                        
                        CustomEnumDesc[] descs = null;
                        System.Reflection.FieldInfo fieldInfo = systemType.GetField( prop.stringValue );
                        if( fieldInfo != null )
                        {
                            descs = (CustomEnumDesc[])fieldInfo.GetCustomAttributes( typeof(CustomEnumDesc), true );
                        }
                        if( descs != null && descs.Length > 0 ) UnityEditor.EditorGUILayout.BeginVertical( "box" );
                        
                        UnityEditor.EditorGUILayout.BeginHorizontal( );
                        {
                            prop.stringValue = EditorGUILayout.TextField( text, prop.stringValue );
                            string stringValue = (string)prop.stringValue;
                            //if( string.IsNullOrEmpty( stringValue ) ) stringValue = System.Enum.GetNames( systemType )[0].ToString();
                            int selectIndex = ArrayUtility.FindIndex<string>( list, ( n ) => n == stringValue );
                            //if( selectIndex == -1 ){ stringValue = ""; selectIndex = 0; }
                            int nextView = EditorHelp.Popup( prop.serializedObject.targetObject, selectIndex, dispNames, GUILayout.Width( 20f ) );
                            if( nextView != selectIndex )
                            {
                                prop.stringValue = list[ nextView ];
                            }
                        }
                        
                        UnityEditor.EditorGUILayout.EndHorizontal( );
                        if( descs != null && descs.Length > 0 )
                        {
                            UnityEditor.EditorGUILayout.HelpBox( descs[0].text, MessageType.Info );
                            UnityEditor.EditorGUILayout.EndVertical( );
                        }
                    }
                }
                break;
            default:
                {
                    UnityEditor.EditorGUILayout.BeginHorizontal( );
                    {
                        EditorGUIUtility.labelWidth = width * 0.35f;
                        switch( type )
                        {
                        case Type.Bool:
                            prop.boolValue = EditorHelp.Toggle( prop.serializedObject.targetObject, text, prop.boolValue );
                            break;
                        case Type.String:
                            prop.stringValue = EditorHelp.TextField( prop.serializedObject.targetObject, text, prop.stringValue );
                            break;
                        case Type.Int:
                            if( range == null )
                            {
                                prop.intValue = EditorHelp.IntField( prop.serializedObject.targetObject, text, prop.intValue );
                            }
                            else
                            {
                                prop.intValue = EditorHelp.IntSlider( prop.serializedObject.targetObject, text, prop.intValue, (int)range.min, (int)range.max );
                            }
                            break;
                        case Type.Float:
                            if( range == null )
                            {
                                prop.floatValue = EditorHelp.FloatField( prop.serializedObject.targetObject, text, prop.floatValue );
                            }
                            else
                            {
                                prop.floatValue = EditorHelp.FloatSlider( prop.serializedObject.targetObject, text, prop.floatValue, range.min, range.max );
                            }
                            break;
                        case Type.ScriptableObject:
                            if( prop.isArray == false )
                            {
                                if( systemType == null )
                                {
                                    prop.objectReferenceValue = EditorHelp.ObjectField( prop.serializedObject.targetObject, text, prop.objectReferenceValue, typeof( ScriptableObject ), true );
                                }
                                else
                                {
                                    prop.objectReferenceValue = EditorHelp.ObjectField( prop.serializedObject.targetObject, text, prop.objectReferenceValue, systemType, true );
                                }
                            }
                            break;
                        case Type.GameObject:
                            if( prop.isArray == false )
                            {
                                prop.objectReferenceValue = EditorHelp.ObjectField( prop.serializedObject.targetObject, text, prop.objectReferenceValue, typeof( GameObject ), true ) as GameObject;
                            }
                            break;
                        case Type.GameObjectId:
                            {
                                string[] list = GameObjectId.Keys;
                                if( list != null )
                                {
                                    prop.stringValue = EditorHelp.TextField( prop.serializedObject.targetObject, text, prop.stringValue );
                                    int index = ArrayUtility.FindIndex<string>( list, ( p1 ) => p1 == prop.stringValue );
                                    int nextIndex = EditorHelp.Popup( prop.serializedObject.targetObject, index, list, GUILayout.Width( 15f ) ); 
                                    if( nextIndex != index )
                                    {
                                        prop.stringValue = list[ nextIndex ];
                                    }
                                }
                            }
                            break;
                        case Type.Component:
                            prop.objectReferenceValue = EditorHelp.ObjectField( prop.serializedObject.targetObject, text, prop.objectReferenceValue, systemType, true );
                            break;
                        case Type.Shader:
                            prop.objectReferenceValue = EditorHelp.ObjectField( prop.serializedObject.targetObject, text, prop.objectReferenceValue, typeof( Shader ), true ) as Shader;
                            break;
                        case Type.Material:
                            prop.objectReferenceValue = EditorHelp.ObjectField( prop.serializedObject.targetObject, text, prop.objectReferenceValue, typeof( Material ), true ) as Material;
                            break;
                        case Type.Animator:
                            prop.objectReferenceValue = EditorHelp.ObjectField( prop.serializedObject.targetObject, text, prop.objectReferenceValue, typeof( Animator ), true ) as Animator;
                            break;
                        case Type.TextAsset:
                            prop.objectReferenceValue = EditorHelp.ObjectField( prop.serializedObject.targetObject, text, prop.objectReferenceValue, typeof( TextAsset ), true ) as TextAsset;
                            break;
                        case Type.Texture:
                            prop.objectReferenceValue = EditorHelp.ObjectField( prop.serializedObject.targetObject, text, prop.objectReferenceValue, typeof( Texture ), true ) as Texture;
                            break;
                        case Type.Vector2:
                            prop.vector2Value = EditorHelp.Vector2Field( prop.serializedObject.targetObject, text, prop.vector2Value );
                            break;
                        case Type.Vector3:
                            prop.vector3Value = EditorHelp.Vector3Field( prop.serializedObject.targetObject, text, prop.vector3Value );
                            break;
                        case Type.Vector4:
                            prop.vector4Value = EditorHelp.Vector4Field(prop.serializedObject.targetObject, text, prop.vector4Value);
                            break;
                        case Type.Vector2Int:
                            prop.vector2IntValue = EditorHelp.Vector2IntField( prop.serializedObject.targetObject, text, prop.vector2IntValue );
                            break;
                        case Type.Vector3Int:
                            prop.vector3IntValue = EditorHelp.Vector3IntField( prop.serializedObject.targetObject, text, prop.vector3IntValue );
                            break;
                        case Type.Color:
                            prop.colorValue = EditorHelp.ColorField( prop.serializedObject.targetObject, text, prop.colorValue );
                            break;
                        case Type.Curve:
                            prop.animationCurveValue = EditorHelp.CurveField( prop.serializedObject.targetObject, text, prop.animationCurveValue );
                            break;
                        case Type.UITweenController:
                            prop.objectReferenceValue = EditorHelp.ObjectField( prop.serializedObject.targetObject, text, prop.objectReferenceValue, typeof( UITweenController ), true ) as UITweenController;
                            break;
                        case Type.MonoBehaviour:
                            GUIField_ObjectField<UnityEngine.MonoBehaviour>( text, objType, prop );
                            break;
                        case Type.UIText:
                            GUIField_ObjectField<UnityEngine.UI.Text>( text, objType, prop );
                            break;
                        //todo UIBitmapText
                        // case Type.UIBitmapText:
                        //     GUIField_ObjectField<UIBitmapText>( text, objType, prop );
                        //     break;
                        case Type.UIRawImage:
                            GUIField_ObjectField<UnityEngine.UI.RawImage>( text, objType, prop );
                            break;
                        case Type.UIImage:
                            GUIField_ObjectField<UnityEngine.UI.Image>( text, objType, prop );
                            break;
                        case Type.UISprite:
                            prop.objectReferenceValue = EditorHelp.ObjectField( prop.serializedObject.targetObject, text, prop.objectReferenceValue, typeof( Sprite ), true ) as Sprite;
                            break;
                        case Type.Enum:
                            {
                                if( systemType != null )
                                {
                                    string[] list = System.Enum.GetNames( systemType );
                                    string[] dispNames = new string[ list.Length ];
                                    System.Array.Copy( list, dispNames, list.Length );
                                    if( tblType != null )
                                    {
                                        System.Reflection.FieldInfo tblFieldInfo = tblType.GetField( systemType.Name + "Names", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance );
                                        if( tblFieldInfo != null )
                                        {
                                            if( tblFieldInfo.FieldType == typeof( string[] ) )
                                            {
                                                dispNames = (string[])tblFieldInfo.GetValue( null );
                                            }
                                            else if( tblFieldInfo.FieldType == typeof( Utility.EnumArray.Element[] ) )
                                            {
                                                Utility.EnumArray.Element[] elements = (Utility.EnumArray.Element[])tblFieldInfo.GetValue( null );
                                                list = new string[ elements.Length ];
                                                dispNames = new string[ elements.Length ];
                                                for( int i = 0; i < elements.Length; ++i )
                                                {
                                                    list[ i ] = elements[ i ].index.ToString( );
                                                    dispNames[ i ] = elements[ i ].dispName;
                                                }
                                            }
                                        }
                                    }
                                    for( int n = 0; n < dispNames.Length; ++n )
                                    {
                                        dispNames[n] = dispNames[n].Replace( '_', '/' );
                                    }
                                    
                                    int intValue = prop.intValue;
                                    string stringValue = System.Enum.ToObject( systemType, intValue ).ToString( );
                                    
                                    CustomEnumDesc[] descs = null;
                                    System.Reflection.FieldInfo fieldInfo = systemType.GetField( stringValue );
                                    if( fieldInfo != null )
                                    {
                                        descs = (CustomEnumDesc[])fieldInfo.GetCustomAttributes( typeof( CustomEnumDesc ), true );
                                    }
                                    if( descs != null && descs.Length > 0 ) UnityEditor.EditorGUILayout.BeginVertical( "box" );
                                    
                                    if( string.IsNullOrEmpty( stringValue ) ) stringValue = System.Enum.GetNames( systemType )[0].ToString();
                                    int selectIndex = ArrayUtility.FindIndex<string>( list, ( n ) => n == stringValue );
                                    if( selectIndex == -1 ){ stringValue = ""; selectIndex = 0; }
                                    int nextView = EditorHelp.Popup( prop.serializedObject.targetObject, text, selectIndex, dispNames );
                                    if( nextView != selectIndex )
                                    {
                                        prop.enumValueIndex = Utility.EnumStringToInt( systemType, list[ nextView ] );
                                    }
                                    
                                    if( descs != null && descs.Length > 0 )
                                    {
                                        UnityEditor.EditorGUILayout.HelpBox( descs[ 0 ].text, MessageType.Info );
                                        UnityEditor.EditorGUILayout.EndVertical( );
                                    }
                                }
                                else
                                {
                                    prop.enumValueIndex = EditorHelp.Popup( prop.serializedObject.targetObject, text, prop.enumValueIndex, prop.enumDisplayNames );
                                }
                            }
                            break;
                        case Type.BitFlag:
                            {
                                if( systemType != null )
                                {
                                    string[] list = System.Enum.GetNames( systemType );
                                    string[] dispNames = new string[ list.Length ];
                                    System.Array.Copy( list, dispNames, list.Length );
                                    if( tblType != null )
                                    {
                                        System.Reflection.FieldInfo tblFieldInfo = tblType.GetField( systemType.Name + "Names", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance );
                                        if( tblFieldInfo != null )
                                        {
                                            if( tblFieldInfo.FieldType == typeof( string[] ) )
                                            {
                                                dispNames = (string[])tblFieldInfo.GetValue( null );
                                            }
                                            else if( tblFieldInfo.FieldType == typeof( Utility.EnumArray.Element[] ) )
                                            {
                                                Utility.EnumArray.Element[] elements = (Utility.EnumArray.Element[])tblFieldInfo.GetValue( null );
                                                list = new string[ elements.Length ];
                                                dispNames = new string[ elements.Length ];
                                                for( int i = 0; i < elements.Length; ++i )
                                                {
                                                    list[ i ] = elements[ i ].index.ToString( );
                                                    dispNames[ i ] = elements[ i ].dispName;
                                                }
                                            }
                                        }
                                    }
                                    for( int n = 0; n < dispNames.Length; ++n )
                                    {
                                        dispNames[n] = dispNames[n].Replace( '_', '/' );
                                    }

                                    SerializedProperty child = null;
                                    if (prop.propertyType == SerializedPropertyType.Integer)
                                    {
                                        child = prop;
                                    }
                                    else
                                    {
                                        var p = prop.Copy( );
                                        var end = prop.GetEndProperty( true );
                                        if( p.Next( true ) )
                                        {
                                            while ( SerializedProperty.EqualContents( p, end ) == false )
                                            {
                                                child = p;
                                                break;
                                            }
                                        }
                                    }

                                    if (child != null)
                                    {
                                        var mask = child.intValue;
                                        int dispMask = 0;
                                        for( int i = 0; i < 32; ++i )
                                        {
                                            if( ( mask & (1<<i) ) != 0 )
                                            {
                                                int n = (int)System.Enum.Parse( systemType, list[i] );
                                                dispMask |= (1<<n);
                                            }
                                        }
                                        int nextMask = EditorHelp.MaskField( prop.serializedObject.targetObject, text, dispMask, dispNames );
                                        if( nextMask != dispMask )
                                        {
                                            mask = 0;
                                            for( int i = 0; i < 32; ++i )
                                            {
                                                if( ( nextMask & (1<<i) ) != 0 )
                                                {
                                                    string enumName = System.Enum.ToObject( systemType, i ).ToString( );
                                                    for( int j = 0; j < list.Length; ++j )
                                                    {
                                                        if( list[j] == enumName )
                                                        {
                                                            mask |= (1<<j);
                                                            break;
                                                        }
                                                    }
                                                }
                                            }
                                            child.intValue = mask;
                                        }
                                    }
                                }
                                else
                                {
                                    var child = prop.Copy( );
                                    var end = prop.GetEndProperty( true );
                                    if( child.Next( true ) )
                                    {
                                        while( SerializedProperty.EqualContents( child, end ) == false )
                                        {
                                            System.Reflection.FieldInfo fieldInfo = systemType.GetField( prop.name + "Names", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance );
                                            string[] flagNames = fieldInfo != null ? (string[])fieldInfo.GetValue( null ): null;
                                            if( flagNames != null )
                                            {
                                                int mask = child.intValue;
                                                child.intValue = EditorHelp.MaskField( prop.serializedObject.targetObject, text, mask, flagNames );
                                            }
                                            else
                                            {
                                                EditorHelp.LabelField( prop.serializedObject.targetObject, text, prop.name + "Names が存在しません" );
                                            }
                                            break;
                                        }
                                    }
                                }
                            }
                            break;
                        //todo UIImageArray
                        // case Type.UIImageArray:
                        //     GUIField_ObjectField<UIImageArray>(text, objType, prop);
                        //     break;
                        case Type.EventRef:
                            
                            break;
                        }
                    }
                    UnityEditor.EditorGUILayout.EndHorizontal( );
                }
                break;
            }
            if( help_obj != null )
            {
                UnityEditor.EditorGUILayout.HelpBox( help_obj.text, MessageType.Info );
            }
            if( button != null )
            {
                if( GUILayout.Button( button.text ) )
                {
                    System.Type fieldType = prop.serializedObject.targetObject.GetType( );
                    System.Reflection.MethodInfo methodInfo = fieldType.GetMethod( button.method );
                    if( methodInfo != null )
                    {
                        methodInfo.Invoke( prop.serializedObject.targetObject, new object[] { prop } );
                    }
                }
            }
            if( editor_button != null )
            {
                if( GUILayout.Button( editor_button.text ) )
                {
                    if( editor_button.num < CustomFieldEditorButtonAttribute.EditorButton.Length )
                    {
                        CustomFieldEditorButtonAttribute.EditorButtonAction action = CustomFieldEditorButtonAttribute.EditorButton[ editor_button.num ];
                        if( action != null )
                        {
                            action( prop );
                        }
                    }
                }
            }
            if( help_obj != null || button != null || editor_button != null )
            {
                UnityEditor.EditorGUILayout.EndVertical( );
            }
        }
        
        /// ***********************************************************************
        /// <summary>
        /// ＧＵＩ_オブジェクトフィールド
        /// </summary>
        /// ***********************************************************************
        private void GUIField_ObjectField<T>( string label, System.Type objType, UnityEditor.SerializedProperty prop ) where T : UnityEngine.MonoBehaviour
        {
            T obj = default(T);
            if( prop.objectReferenceValue != null )
            {
                if( objType == typeof( GameObject ) )       obj = ( prop.objectReferenceValue as GameObject ).GetComponent<T>( );
                else if( objType == typeof( T ) )           obj = ( prop.objectReferenceValue as T );
            }
            T nextObj = EditorHelp.ObjectField( prop.serializedObject.targetObject, label, obj, typeof( T ), true ) as T;
            if( nextObj != obj )
            {
                if( nextObj != null )
                {
                    if( objType == typeof( GameObject ) )   prop.objectReferenceValue = nextObj.gameObject;
                    else if( objType == typeof( T ) )       prop.objectReferenceValue = nextObj;
                }
                else
                {
                    prop.objectReferenceValue = null;
                }
            }
        }
        
        /// ***********************************************************************
        /// <summary>
        /// 配列
        /// </summary>
        /// ***********************************************************************
        private static string _edit_array_size = "";
        public void GUIField_Array( System.Type objType, UnityEditor.SerializedProperty prop, float width, System.Func<UnityEditor.SerializedProperty,int,int,int> callback )
        {
            if( prop.isArray )
            {
                EditorGUI.indentLevel++;
                bool isFoldout = UnityEditor.EditorPrefs.GetBool( prop.name );
                if( isFoldout )
                {
                    GUI.contentColor = Color.white;
                    GUI.backgroundColor = new Color32( 0, 140, 200, 255 );
                }
                else
                {
                    GUI.contentColor = new Color(0.6f,0.6f,0.6f);
                    GUI.backgroundColor = new Color32( 0, 120, 160, 255 );
                }
                if( GUILayout.Button( (isFoldout ? "▼ ": "△ ") + text, EditorHelp.GUIStyles.ButtonToggle ) )
                {
                    isFoldout = !isFoldout;
                    UnityEditor.EditorPrefs.SetBool( prop.name, isFoldout );
                }
                GUI.contentColor = Color.white;
                GUI.backgroundColor = Color.white;
                if( isFoldout )
                {
                    UnityEditor.EditorGUILayout.BeginVertical( "box" );
                    {
                        UnityEditor.EditorGUILayout.BeginHorizontal( );
                        {
                            UnityEditor.EditorGUILayout.LabelField( "数量:" + prop.arraySize );
                            _edit_array_size = UnityEditor.EditorGUILayout.TextField( _edit_array_size, GUILayout.Width( 80f ) );
                            int size = Utility.ParseInt( _edit_array_size, -1 );
                            GUI.enabled = size != -1;
                            if( GUILayout.Button( "更新", GUILayout.Width( 50f ) ) )
                            {
                                prop.arraySize = size;
                            }
                            GUI.enabled = true;
                        }
                        UnityEditor.EditorGUILayout.EndHorizontal( );
                        
                        int change_src = -1;
                        int change_dst = -1;
                        for( int i = 0; i < prop.arraySize; ++i )
                        {
                            SerializedProperty arrayProp = prop.GetArrayElementAtIndex( i );
                            int change = callback( arrayProp, i, prop.arraySize );
                            if( change != -1 )
                            {
                                change_src = i;
                                change_dst = change;
                            }
                        }
                        if( change_src != -1 && change_dst != -1 )
                        {
                            ++ prop.arraySize;
                            SerializedProperty arrayTmp = prop.GetArrayElementAtIndex( prop.arraySize-1 );
                            if( change_dst < change_src )
                            {
                                // 上に移動
                                for( int j = change_dst, n = change_src; j < change_src; ++j )
                                {
                                    SerializedProperty arrayProp1 = prop.GetArrayElementAtIndex( n );
                                    SerializedProperty arrayProp2 = prop.GetArrayElementAtIndex( j );
                                    arrayProp2.CopyTo( arrayTmp, systemType );
                                    arrayProp1.CopyTo( arrayProp2, systemType );
                                    arrayTmp.CopyTo( arrayProp1, systemType );
                                }
                            }
                            else
                            {
                                // 下に移動
                                for( int j = change_dst, n = change_src; j > change_src; --j )
                                {
                                    SerializedProperty arrayProp1 = prop.GetArrayElementAtIndex( n );
                                    SerializedProperty arrayProp2 = prop.GetArrayElementAtIndex( j );
                                    arrayProp2.CopyTo( arrayTmp, systemType );
                                    arrayProp1.CopyTo( arrayProp2, systemType );
                                    arrayTmp.CopyTo( arrayProp1, systemType );
                                }
                            }
                            -- prop.arraySize;
                        }
                        if( help != null )
                        {
                            UnityEditor.EditorGUILayout.HelpBox( help.text, MessageType.Info );
                        }
                    }
                    UnityEditor.EditorGUILayout.EndVertical( );
                }
                EditorGUI.indentLevel--;
            }
        }
        
        public class DrawNormal
        {
            CustomFieldAttribute attr;
            System.Type propType;
            UnityEditor.SerializedProperty it;
            
            public DrawNormal( CustomFieldAttribute _attr, System.Type _propType, UnityEditor.SerializedProperty _it )
            {
                attr = _attr;
                propType = _propType;
                it = _it;
            }
            
            public void GUIField( float width )
            {
                attr.GUIField( propType, it, width );
            }
        }
        

        public class DrawGroup
        {
            public static GUIStyle                  style   = null;
            
            public class Element
            {
                public UnityEditor.SerializedProperty   it;
                public CustomFieldAttribute             attr;
                public System.Type                      type;
                
                public Element( UnityEditor.SerializedProperty _it, CustomFieldAttribute _attr, System.Type _type )
                {
                    it = _it;
                    attr = _attr;
                    type = _type;
                }
            }
            
            public System.Type      propType;
            public string           name;
            public string           key;
            public List<Element>    elements = new List<Element>();
            
            public DrawGroup( System.Type _propType, string _name )
            {
                propType = _propType;
                name = _name;
                key = propType.ToString() + "@" + name;
            }
            
            public void Add( UnityEditor.SerializedProperty it, CustomFieldAttribute attr, System.Type type )
            {
                elements.Add( new Element( it, attr, type ) );
            }
            
            public void GUIField( float width )
            {
                if( style == null )
                {
                    style = new GUIStyle( GUI.skin.GetStyle( "button" ) );
                    style.alignment = TextAnchor.MiddleLeft;
                    style.normal = new GUIStyleState( );
                    style.normal.textColor = Color.white;
                    style.normal.background = Texture2D.whiteTexture;
                    style.active = new GUIStyleState( );
                    style.active.textColor = Color.white;
                    style.active.background = Texture2D.whiteTexture;
                }
                
                UnityEditor.EditorGUILayout.BeginVertical( "box" );
                {
                    bool isFoldout = EditorPrefs.GetBool( key );
                    if( isFoldout )
                    {
                        GUI.contentColor = Color.white;
                        GUI.backgroundColor = new Color32( 200, 170, 0, 255 );
                    }
                    else
                    {
                        GUI.contentColor = new Color(0.3f,0.3f,0.3f);
                        GUI.backgroundColor = new Color32( 150, 115, 0, 255 );
                    }
                    if( GUILayout.Button( ( isFoldout ? "▼ ": "△ " ) + name, style ) )
                    {
                        isFoldout = !isFoldout;
                        EditorPrefs.SetBool( key, isFoldout );
                    }
                    GUI.contentColor = Color.white;
                    GUI.backgroundColor = Color.white;
                    if( isFoldout )
                    {
                        for( int i = 0; i < elements.Count; ++i )
                        {
                            elements[ i ].attr.GUIField( elements[ i ].type, elements[ i ].it, width );
                        }
                    }
                }
                UnityEditor.EditorGUILayout.EndVertical( );
            }
        }
        
        //=========================================================================
        //. GUI
        //=========================================================================
        

        public static void OnInspectorGUI( Rect position, System.Type propType, UnityEditor.SerializedObject serializedObject, float width )
        {
            serializedObject.Update( );
            UnityEditor.SerializedProperty it = serializedObject.GetIterator( );
            
            List<DrawNormal> draws = new List<DrawNormal>( ); 
            Dictionary<string,DrawGroup> groups = new Dictionary<string,DrawGroup>( );
            
            while( it.NextVisible( true ) )
            {
                System.Reflection.FieldInfo field = propType.GetField( it.name, System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance );
                if( field == null )
                {
                    continue;
                }
                
                object[] attrs = field.GetCustomAttributes( false );
                if( attrs.Length > 0 )
                {
                    CustomFieldButtonAttribute button = null;
                    CustomFieldEditorButtonAttribute editor_button = null;
                    CustomFieldHelpAttribute help = null;
                    CustomFieldRangeAttribute range = null;
                    List<CustomFieldDispCondAttribute> conds = new List<CustomFieldDispCondAttribute>( );
                    string groupName = null;
                    // 
                    for( int i = 0; i < attrs.Length; ++i )
                    {
                        if( attrs[ i ] is CustomFieldRangeAttribute )
                        {
                            range = attrs[ i ] as CustomFieldRangeAttribute;
                        }
                        if( attrs[ i ] is CustomFieldHelpAttribute )
                        {
                            help = attrs[ i ] as CustomFieldHelpAttribute;
                        }
                        if( attrs[ i ] is CustomFieldButtonAttribute )
                        {
                            button = attrs[ i ] as CustomFieldButtonAttribute;
                        }
                        if( attrs[ i ] is CustomFieldEditorButtonAttribute )
                        {
                            editor_button = attrs[ i ] as CustomFieldEditorButtonAttribute;
                        }
                        if( attrs[ i ] is CustomFieldDispCondAttribute )
                        {
                            conds.Add( attrs[ i ] as CustomFieldDispCondAttribute );
                        }
                        if( attrs[ i ] is CustomFieldGroupAttribute )
                        {
                            groupName = ( attrs[ i ] as CustomFieldGroupAttribute ).groupName;
                        }
                    }
                    // 
                    bool isDisp = conds.Count == 0 ? true: false;
                    for( int i = 0; i < conds.Count; ++i )
                    {
                        CustomFieldDispCondAttribute cond = conds[ i ] as CustomFieldDispCondAttribute;
                        System.Reflection.FieldInfo condField = propType.GetField( cond.prop, System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance );
                        if( condField != null )
                        {
                            object condValue = condField.GetValue( serializedObject.targetObject );
                            if( condValue != null )
                            {
                                bool isCond = condValue.ToString( ) == cond.value;
                                {
                                    if( i == 0 )
                                    {
                                        isDisp = isCond ? cond.sw: !cond.sw;
                                    }
                                    else if( isCond )
                                    {
                                        if( cond.ope == CustomFieldDispCondAttribute.EOperator.And )     isDisp &= cond.sw;
                                        else if( cond.ope == CustomFieldDispCondAttribute.EOperator.Or ) isDisp |= cond.sw;
                                    }
                                }
                            }
                        }
                    }
                    if( isDisp == false ) continue;
                    // 
                    for( int i = 0; i < attrs.Length; ++i )
                    {
                        if( attrs[ i ] is CustomFieldAttribute )
                        {
                            CustomFieldAttribute attr = attrs[ i ] as CustomFieldAttribute;
                            attr.range = range;
                            attr.help = help;
                            attr.button = button;
                            attr.editor_button = editor_button;
                            
                            if( string.IsNullOrEmpty( groupName ) )
                            {
                                draws.Add( new DrawNormal( attr, field.FieldType, serializedObject.FindProperty( it.propertyPath ) ) );
                            }
                            else
                            {
                                DrawGroup list = null;
                                if( groups.TryGetValue( groupName, out list ) == false )
                                {
                                    list = new DrawGroup( propType, groupName );
                                    groups.Add( groupName, list );
                                }
                                list.Add( serializedObject.FindProperty( it.propertyPath ), attr, field.FieldType );
                            }
                        }
                    }
                }
                else
                {
                    UnityEditor.EditorGUILayout.PropertyField( it, true );
                }
            };
            
            // 
            foreach( var pair in groups )
            {
                DrawGroup draw = pair.Value;
                if( draw != null )
                {
                    draw.GUIField( width );
                }
            }
            
            foreach( var draw in draws )
            {
                draw.GUIField( width );
            }
            
            serializedObject.ApplyModifiedProperties( );
        }
        public static void OnInspectorGUI( Rect position, System.Type propType, UnityEditor.SerializedObject serializedObject )
        {
            OnInspectorGUI( position, propType, serializedObject, UnityEditor.EditorGUIUtility.currentViewWidth );
        }
        public static void OnInspectorGUI( System.Type propType, UnityEditor.SerializedObject serializedObject )
        {
            OnInspectorGUI( new Rect( 0, 0, 0, 0 ), propType, serializedObject, UnityEditor.EditorGUIUtility.currentViewWidth );
        }
        

        public static void OnInspectorGUI( System.Type propType, UnityEditor.SerializedProperty serializedProperty, float width )
        {
            List<DrawNormal> draws = new List<DrawNormal>( ); 
            Dictionary<string,DrawGroup> groups = new Dictionary<string,DrawGroup>( );
            
            var child = serializedProperty.Copy( );
            var end = serializedProperty.GetEndProperty( true );
            if( child.Next( true ) )
            {
                while( SerializedProperty.EqualContents( child, end ) == false )
                {
                    System.Reflection.FieldInfo field = propType.GetField( child.name, System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance );
                    if( field == null )
                    {
                        if( child.Next( false ) == false ) break;
                        continue;
                    }
                    
                    object[] attrs = field.GetCustomAttributes( false );
                    if( attrs.Length > 0 )
                    {
                        CustomFieldButtonAttribute button = null;
                        CustomFieldEditorButtonAttribute editor_button = null;
                        CustomFieldHelpAttribute help = null;
                        CustomFieldRangeAttribute range = null;
                        List<CustomFieldDispCondAttribute> conds = new List<CustomFieldDispCondAttribute>( );
                        string groupName = null;
                        // グループを持っているか調べる
                        for( int i = 0; i < attrs.Length; ++i )
                        {
                            if( attrs[ i ] is CustomFieldRangeAttribute )
                            {
                                range = attrs[ i ] as CustomFieldRangeAttribute;
                            }
                            if( attrs[ i ] is CustomFieldHelpAttribute )
                            {
                                help = attrs[ i ] as CustomFieldHelpAttribute;
                            }
                            if( attrs[ i ] is CustomFieldButtonAttribute )
                            {
                                button = attrs[ i ] as CustomFieldButtonAttribute;
                            }
                            if( attrs[ i ] is CustomFieldEditorButtonAttribute )
                            {
                                editor_button = attrs[ i ] as CustomFieldEditorButtonAttribute;
                            }
                            if( attrs[ i ] is CustomFieldDispCondAttribute )
                            {
                                conds.Add( attrs[ i ] as CustomFieldDispCondAttribute );
                            }
                            if( attrs[ i ] is CustomFieldGroupAttribute )
                            {
                                groupName = ( attrs[ i ] as CustomFieldGroupAttribute ).groupName;
                            }
                        }
                        // 
                        bool isDisp = conds.Count == 0 ? true: false;
                        for( int i = 0; i < conds.Count; ++i )
                        {
                            CustomFieldDispCondAttribute cond = attrs[ i ] as CustomFieldDispCondAttribute;
                            System.Reflection.FieldInfo condField = propType.GetField( cond.prop, System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance );
                            if( condField != null )
                            {
                                object condValue = condField.GetValue( serializedProperty.serializedObject.targetObject );
                                if( condValue != null )
                                {
                                    if( condValue.ToString( ) == cond.value )
                                    {
                                        if( i == 0 )
                                            isDisp = cond.sw;
                                        else
                                        {
                                            if( cond.ope == CustomFieldDispCondAttribute.EOperator.And )     isDisp &= cond.sw;
                                            else if( cond.ope == CustomFieldDispCondAttribute.EOperator.Or ) isDisp |= cond.sw;
                                        }
                                    }
                                }
                            }
                        }
                        if( isDisp == false ) continue;
                        // 
                        for( int i = 0; i < attrs.Length; ++i )
                        {
                            if( attrs[ i ] is CustomFieldAttribute )
                            {
                                CustomFieldAttribute attr = attrs[ i ] as CustomFieldAttribute;
                                attr.range = range;
                                attr.help = help;
                                attr.button = button;
                                attr.editor_button = editor_button;
                                
                                if( string.IsNullOrEmpty( groupName ) )
                                {
                                    draws.Add( new DrawNormal( attr, field.FieldType, child.Copy() ) );
                                }
                                else
                                {
                                    DrawGroup list = null;
                                    if( groups.TryGetValue( groupName, out list ) == false )
                                    {
                                        list = new DrawGroup( propType, groupName );
                                        groups.Add( groupName, list );
                                    }
                                    list.Add( child.Copy(), attr, field.FieldType );
                                }
                            }
                        }
                    }
                    else
                    {
                        UnityEditor.EditorGUILayout.PropertyField( child, true );
                    }
                    
                    if( child.Next( false ) == false ) break;
                }
            }
            
            // 
            foreach( var pair in groups )
            {
                DrawGroup draw = pair.Value;
                if( draw != null )
                {
                    draw.GUIField( width );
                }
            }
            
            foreach( var draw in draws )
            {
                draw.GUIField( width );
            }
        }
        public static void OnInspectorGUI( System.Type propType, UnityEditor.SerializedProperty serializedProperty )
        {
            OnInspectorGUI( propType, serializedProperty, UnityEditor.EditorGUIUtility.currentViewWidth );
        }
        
        #else
        
        public CustomFieldAttribute( string _text, Type _type )
        {
        }
        public CustomFieldAttribute( string _text, Type _type, System.Type _systemType )
        {
        }
        public CustomFieldAttribute( string _text, Type _type, System.Type _systemType, System.Type _tblType )
        {
        }
        public CustomFieldAttribute( string _text, System.Type _systemType )
        {
        }
        
        #endif
    }
    
}
