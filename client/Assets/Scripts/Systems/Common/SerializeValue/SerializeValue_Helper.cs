#if DEBUG_BUILD && UNITY_EDITOR
#define STACK_HISTROY
#endif
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Diagnostics;
using Debug = UnityEngine.Debug;

namespace EG
{
    public partial class SerializeValue
    {
        public static SerializeValue CreateEmpty( )
        {
            return new SerializeValue( );
        }
        
        #if STACK_HISTROY
        public static SerializeValue CreateCopy( SerializeValue v, [CallerFilePath] string _file = "", [CallerLineNumber] int _line = -1 )
        #else
        public static SerializeValue CreateCopy( SerializeValue v )
        #endif
        {
            SerializeValue result = new SerializeValue( v );
            #if STACK_HISTROY
            result.AddCallerInfo( v.GetCallerHistory( ) );
            result.AddCallerInfo( _file, _line );
            #endif
            return result;
        }
        
        #if STACK_HISTROY
        public static SerializeValue CreateType( Type type, string key, string serialize, [CallerFilePath] string _file = "", [CallerLineNumber] int _line = -1 )
        #else
        public static SerializeValue CreateType( Type type, string key, string serialize )
        #endif
        {
            SerializeValue result = null;
            if( serialize == null )
            {
                result = new SerializeValue( type, key );
            }
            else
            {
                result = new SerializeValue( type, key, serialize );
            }
            #if STACK_HISTROY
            result.AddCallerInfo( _file, _line );
            #endif
            return result;
        }
        
        #if STACK_HISTROY
        public static SerializeValue CreateBool( string key, bool value, [CallerFilePath] string _file = "", [CallerLineNumber] int _line = -1 )
        #else
        public static SerializeValue CreateBool( string key, bool value )
        #endif
        {
            SerializeValue result = new SerializeValue( key, value );
            #if STACK_HISTROY
            result.AddCallerInfo( _file, _line );
            #endif
            return result;
        }
        
        #if STACK_HISTROY
        public static SerializeValue CreateBool( bool value, [CallerFilePath] string _file = "", [CallerLineNumber] int _line = -1 )
        #else
        public static SerializeValue CreateBool( bool value )
        #endif
        {
            #if STACK_HISTROY
            return CreateBool( null, value, _file, _line );
            #else
            return CreateBool( null, value );
            #endif
        }
        
        #if STACK_HISTROY
        public static SerializeValue CreateInt( string key, int value, [CallerFilePath] string _file = "", [CallerLineNumber] int _line = -1 )
        #else
        public static SerializeValue CreateInt( string key, int value )
        #endif
        {
            SerializeValue result = new SerializeValue( key, value );
            #if STACK_HISTROY
            result.AddCallerInfo( _file, _line );
            #endif
            return result;
        }
        
        #if STACK_HISTROY
        public static SerializeValue CreateInt( int value, [CallerFilePath] string _file = "", [CallerLineNumber] int _line = -1 )
        #else
        public static SerializeValue CreateInt( int value )
        #endif
        {
            #if STACK_HISTROY
            return CreateInt( null, value, _file, _line );
            #else
            return CreateInt( null, value );
            #endif
        }
        
        #if STACK_HISTROY
        public static SerializeValue CreateLong( string key, long value, [CallerFilePath] string _file = "", [CallerLineNumber] int _line = -1 )
        #else
        public static SerializeValue CreateLong( string key, long value )
        #endif
        {
            SerializeValue result = new SerializeValue( key, value );
            #if STACK_HISTROY
            result.AddCallerInfo( _file, _line );
            #endif
            return result;
        }
        
        #if STACK_HISTROY
        public static SerializeValue CreateLong( long value, [CallerFilePath] string _file = "", [CallerLineNumber] int _line = -1 )
        #else
        public static SerializeValue CreateLong( long value )
        #endif
        {
            #if STACK_HISTROY
            return CreateLong( null, value, _file, _line );
            #else
            return CreateLong( null, value );
            #endif
        }
        
        #if STACK_HISTROY
        public static SerializeValue CreateFloat( string key, float value, [CallerFilePath] string _file = "", [CallerLineNumber] int _line = -1 )
        #else
        public static SerializeValue CreateFloat( string key, float value )
        #endif
        {
            SerializeValue result = new SerializeValue( key, value );
            #if STACK_HISTROY
            result.AddCallerInfo( _file, _line );
            #endif
            return result;
        }
        
        #if STACK_HISTROY
        public static SerializeValue CreateFloat( float value, [CallerFilePath] string _file = "", [CallerLineNumber] int _line = -1 )
        #else
        public static SerializeValue CreateFloat( float value )
        #endif
        {
            #if STACK_HISTROY
            return CreateFloat( null, value, _file, _line );
            #else
            return CreateFloat( null, value );
            #endif
        }
        
        #if STACK_HISTROY
        public static SerializeValue CreateString( string key, string value, [CallerFilePath] string _file = "", [CallerLineNumber] int _line = -1 )
        #else
        public static SerializeValue CreateString( string key, string value )
        #endif
        {
            SerializeValue result = new SerializeValue( key, value );
            #if STACK_HISTROY
            result.AddCallerInfo( _file, _line );
            #endif
            return result;
        }
        
        #if STACK_HISTROY
        public static SerializeValue CreateString( string value, [CallerFilePath] string _file = "", [CallerLineNumber] int _line = -1 )
        #else
        public static SerializeValue CreateString( string value )
        #endif
        {
            #if STACK_HISTROY
            return CreateString( null, value, _file, _line );
            #else
            return CreateString( null, value );
            #endif
        }
        
        #if STACK_HISTROY
        public static SerializeValue CreateVector2( string key, Vector2 value, [CallerFilePath] string _file = "", [CallerLineNumber] int _line = -1 )
        #else
        public static SerializeValue CreateVector2( string key, Vector2 value )
        #endif
        {
            SerializeValue result = new SerializeValue( key, value );
            #if STACK_HISTROY
            result.AddCallerInfo( _file, _line );
            #endif
            return result;
        }
        
        #if STACK_HISTROY
        public static SerializeValue CreateVector2( Vector2 value, [CallerFilePath] string _file = "", [CallerLineNumber] int _line = -1 )
        #else
        public static SerializeValue CreateVector2( Vector2 value )
        #endif
        {
            #if STACK_HISTROY
            return CreateVector2( null, value, _file, _line );
            #else
            return CreateVector2( null, value );
            #endif
        }
        
        #if STACK_HISTROY
        public static SerializeValue CreateVector3( string key, Vector3 value, [CallerFilePath] string _file = "", [CallerLineNumber] int _line = -1 )
        #else
        public static SerializeValue CreateVector3( string key, Vector3 value )
        #endif
        {
            SerializeValue result = new SerializeValue( key, value );
            #if STACK_HISTROY
            result.AddCallerInfo( _file, _line );
            #endif
            return result;
        }
        
        #if STACK_HISTROY
        public static SerializeValue CreateVector3( Vector3 value, [CallerFilePath] string _file = "", [CallerLineNumber] int _line = -1 )
        #else
        public static SerializeValue CreateVector3( Vector3 value )
        #endif
        {
            #if STACK_HISTROY
            return CreateVector3( null, value, _file, _line );
            #else
            return CreateVector3( null, value );
            #endif
        }
        
        #if STACK_HISTROY
        public static SerializeValue CreateVector4( string key, Vector4 value, [CallerFilePath] string _file = "", [CallerLineNumber] int _line = -1 )
        #else
        public static SerializeValue CreateVector4( string key, Vector4 value )
        #endif
        {
            SerializeValue result = new SerializeValue( key, value );
            #if STACK_HISTROY
            result.AddCallerInfo( _file, _line );
            #endif
            return result;
        }
        
        #if STACK_HISTROY
        public static SerializeValue CreateVector4( Vector4 value, [CallerFilePath] string _file = "", [CallerLineNumber] int _line = -1 )
        #else
        public static SerializeValue CreateVector4( Vector4 value )
        #endif
        {
            #if STACK_HISTROY
            return CreateVector4( null, value, _file, _line );
            #else
            return CreateVector4( null, value );
            #endif
        }
        
        #if STACK_HISTROY
        public static SerializeValue CreateColor( string key, Color value, [CallerFilePath] string _file = "", [CallerLineNumber] int _line = -1 )
        #else
        public static SerializeValue CreateColor( string key, Color value )
        #endif
        {
            SerializeValue result = new SerializeValue( key, value );
            #if STACK_HISTROY
            result.AddCallerInfo( _file, _line );
            #endif
            return result;
        }
        
        #if STACK_HISTROY
        public static SerializeValue CreateColor( Color value, [CallerFilePath] string _file = "", [CallerLineNumber] int _line = -1 )
        #else
        public static SerializeValue CreateColor( Color value )
        #endif
        {
            #if STACK_HISTROY
            return CreateColor( null, value, _file, _line );
            #else
            return CreateColor( null, value );
            #endif
        }
        
        #if STACK_HISTROY
        public static SerializeValue CreateGameObject( string key, GameObject value, [CallerFilePath] string _file = "", [CallerLineNumber] int _line = -1 )
        #else
        public static SerializeValue CreateGameObject( string key, GameObject value )
        #endif
        {
            SerializeValue result = new SerializeValue( key, value );
            #if STACK_HISTROY
            result.AddCallerInfo( _file, _line );
            #endif
            return result;
        }
        
        #if STACK_HISTROY
        public static SerializeValue CreateGameObject( GameObject value, [CallerFilePath] string _file = "", [CallerLineNumber] int _line = -1 )
        #else
        public static SerializeValue CreateGameObject( GameObject value )
        #endif
        {
            #if STACK_HISTROY
            return CreateGameObject( null, value, _file, _line );
            #else
            return CreateGameObject( null, value );
            #endif
        }
        
        #if STACK_HISTROY
        public static SerializeValue CreateLabel( string key, UnityEngine.UI.Text value, [CallerFilePath] string _file = "", [CallerLineNumber] int _line = -1 )
        #else
        public static SerializeValue CreateLabel( string key, UnityEngine.UI.Text value )
        #endif
        {
            SerializeValue result = new SerializeValue( key, value );
            #if STACK_HISTROY
            result.AddCallerInfo( _file, _line );
            #endif
            return result;
        }
        
        #if STACK_HISTROY
        public static SerializeValue CreateLabel( UnityEngine.UI.Text value, [CallerFilePath] string _file = "", [CallerLineNumber] int _line = -1 )
        #else
        public static SerializeValue CreateLabel( UnityEngine.UI.Text value )
        #endif
        {
            #if STACK_HISTROY
            return CreateLabel( null, value, _file, _line );
            #else
            return CreateLabel( null, value );
            #endif
        }
        
        #if STACK_HISTROY
        public static SerializeValue CreateRawImage( string key, UnityEngine.UI.RawImage value, [CallerFilePath] string _file = "", [CallerLineNumber] int _line = -1 )
        #else
        public static SerializeValue CreateRawImage( string key, UnityEngine.UI.RawImage value )
        #endif
        {
            SerializeValue result = new SerializeValue( key, value );
            #if STACK_HISTROY
            result.AddCallerInfo( _file, _line );
            #endif
            return result;
        }
        
        #if STACK_HISTROY
        public static SerializeValue CreateRawImage( UnityEngine.UI.RawImage value, [CallerFilePath] string _file = "", [CallerLineNumber] int _line = -1 )
        #else
        public static SerializeValue CreateRawImage( UnityEngine.UI.RawImage value )
        #endif
        {
            #if STACK_HISTROY
            return CreateRawImage( null, value, _file, _line );
            #else
            return CreateRawImage( null, value );
            #endif
        }
        
        #if STACK_HISTROY
        public static SerializeValue CreateImage( string key, UnityEngine.UI.Image value, [CallerFilePath] string _file = "", [CallerLineNumber] int _line = -1 )
        #else
        public static SerializeValue CreateImage( string key, UnityEngine.UI.Image value )
        #endif
        {
            SerializeValue result = new SerializeValue( key, value );
            #if STACK_HISTROY
            result.AddCallerInfo( _file, _line );
            #endif
            return result;
        }
        
        #if STACK_HISTROY
        public static SerializeValue CreateImage( UnityEngine.UI.Image value, [CallerFilePath] string _file = "", [CallerLineNumber] int _line = -1 )
        #else
        public static SerializeValue CreateImage( UnityEngine.UI.Image value )
        #endif
        {
            #if STACK_HISTROY
            return CreateImage( null, value, _file, _line );
            #else
            return CreateImage( null, value );
            #endif
        }
        
        #if STACK_HISTROY
        public static SerializeValue CreateButton( string key, UnityEngine.UI.Button value, [CallerFilePath] string _file = "", [CallerLineNumber] int _line = -1 )
        #else
        public static SerializeValue CreateButton( string key, UnityEngine.UI.Button value )
        #endif
        {
            SerializeValue result = new SerializeValue( key, value );
            #if STACK_HISTROY
            result.AddCallerInfo( _file, _line );
            #endif
            return result;
        }
        
        #if STACK_HISTROY
        public static SerializeValue CreateButton( UnityEngine.UI.Button value, [CallerFilePath] string _file = "", [CallerLineNumber] int _line = -1 )
        #else
        public static SerializeValue CreateButton( UnityEngine.UI.Button value )
        #endif
        {
            #if STACK_HISTROY
            return CreateButton( null, value, _file, _line );
            #else
            return CreateButton( null, value );
            #endif
        }
        
        #if STACK_HISTROY
        public static SerializeValue CreateToggle( string key, UnityEngine.UI.Toggle value, [CallerFilePath] string _file = "", [CallerLineNumber] int _line = -1 )
        #else
        public static SerializeValue CreateToggle( string key, UnityEngine.UI.Toggle value )
        #endif
        {
            SerializeValue result = new SerializeValue( key, value );
            #if STACK_HISTROY
            result.AddCallerInfo( _file, _line );
            #endif
            return result;
        }
        
        #if STACK_HISTROY
        public static SerializeValue CreateToggle( UnityEngine.UI.Toggle value, [CallerFilePath] string _file = "", [CallerLineNumber] int _line = -1 )
        #else
        public static SerializeValue CreateToggle( UnityEngine.UI.Toggle value )
        #endif
        {
            #if STACK_HISTROY
            return CreateToggle( null, value, _file, _line );
            #else
            return CreateToggle( null, value );
            #endif
        }
        
        #if STACK_HISTROY
        public static SerializeValue CreateGlobal( string key, string fieldName, object value, [CallerFilePath] string _file = "", [CallerLineNumber] int _line = -1 )
        #else
        public static SerializeValue CreateGlobal( string key, string fieldName, object value )
        #endif
        {
            SerializeValue result = new SerializeValue( SerializeValue.Type.Global, key, fieldName );
            result.v_Global = value;
            #if STACK_HISTROY
            result.AddCallerInfo( _file, _line );
            #endif
            return result;
        }
        
        #if STACK_HISTROY
        public static SerializeValue CreateGlobal( string fieldName, object value, [CallerFilePath] string _file = "", [CallerLineNumber] int _line = -1 )
        #else
        public static SerializeValue CreateGlobal( string fieldName, object value )
        #endif
        {
            #if STACK_HISTROY
            return CreateGlobal( null, fieldName, value, _file, _line );
            #else
            return CreateGlobal( null, fieldName, value );
            #endif
        }
        
        #if STACK_HISTROY
        public static SerializeValue CreateObject( string key, object value, [CallerFilePath] string _file = "", [CallerLineNumber] int _line = -1 )
        #else
        public static SerializeValue CreateObject( string key, object value )
        #endif
        {
            SerializeValue result = new SerializeValue( SerializeValue.Type.Object, key, null );
            result.v_Object = value;
            #if STACK_HISTROY
            result.AddCallerInfo( _file, _line );
            #endif
            return result;
        }
        
        #if STACK_HISTROY
        public static SerializeValue CreateObject( object value, [CallerFilePath] string _file = "", [CallerLineNumber] int _line = -1 )
        #else
        public static SerializeValue CreateObject( object value )
        #endif
        {
            #if STACK_HISTROY
            return CreateObject( null, value, _file, _line );
            #else
            return CreateObject( null, value );
            #endif
        }
        
        #if STACK_HISTROY
        public static void Reset( SerializeValue obj, [CallerFilePath] string _file = "", [CallerLineNumber] int _line = -1 )
        #else
        public static void Reset( SerializeValue obj )
        #endif
        {
            obj._Reset( );
            #if STACK_HISTROY
            obj.AddCallerInfo( _file, _line );
            #endif
        }
        
       
        #if STACK_HISTROY
        public static void SetBool( SerializeValue obj, bool value, [CallerFilePath] string _file = "", [CallerLineNumber] int _line = -1 )
        #else
        public static void SetBool( SerializeValue obj, bool value )
        #endif
        {
            obj.v_Bool = value;
            #if STACK_HISTROY
            obj.AddCallerInfo( _file, _line );
            #endif
        }
        
        #if STACK_HISTROY
        public static void SetInt( SerializeValue obj, int value, [CallerFilePath] string _file = "", [CallerLineNumber] int _line = -1 )
        #else
        public static void SetInt( SerializeValue obj, int value )
        #endif
        {
            obj.v_Int = value;
            #if STACK_HISTROY
            obj.AddCallerInfo( _file, _line );
            #endif
        }
        
        #if STACK_HISTROY
        public static void SetLong( SerializeValue obj, long value, [CallerFilePath] string _file = "", [CallerLineNumber] int _line = -1 )
        #else
        public static void SetLong( SerializeValue obj, long value )
        #endif
        {
            obj.v_Long = value;
            #if STACK_HISTROY
            obj.AddCallerInfo( _file, _line );
            #endif
        }
        
        #if STACK_HISTROY
        public static void SetFloat( SerializeValue obj, float value, [CallerFilePath] string _file = "", [CallerLineNumber] int _line = -1 )
        #else
        public static void SetFloat( SerializeValue obj, float value )
        #endif
        {
            obj.v_Float = value;
            #if STACK_HISTROY
            obj.AddCallerInfo( _file, _line );
            #endif
        }
        
        #if STACK_HISTROY
        public static void SetString( SerializeValue obj, string value, [CallerFilePath] string _file = "", [CallerLineNumber] int _line = -1 )
        #else
        public static void SetString( SerializeValue obj, string value )
        #endif
        {
            obj.v_String = value;
            #if STACK_HISTROY
            obj.AddCallerInfo( _file, _line );
            #endif
        }
        
        #if STACK_HISTROY
        public static void SetVector2( SerializeValue obj, Vector2 value, [CallerFilePath] string _file = "", [CallerLineNumber] int _line = -1 )
        #else
        public static void SetVector2( SerializeValue obj, Vector2 value )
        #endif
        {
            obj.v_Vector2 = value;
            #if STACK_HISTROY
            obj.AddCallerInfo( _file, _line );
            #endif
        }
        
        #if STACK_HISTROY
        public static void SetVector3( SerializeValue obj, Vector3 value, [CallerFilePath] string _file = "", [CallerLineNumber] int _line = -1 )
        #else
        public static void SetVector3( SerializeValue obj, Vector3 value )
        #endif
        {
            obj.v_Vector3 = value;
            #if STACK_HISTROY
            obj.AddCallerInfo( _file, _line );
            #endif
        }
        
        #if STACK_HISTROY
        public static void SetVector4( SerializeValue obj, Vector4 value, [CallerFilePath] string _file = "", [CallerLineNumber] int _line = -1 )
        #else
        public static void SetVector4( SerializeValue obj, Vector4 value )
        #endif
        {
            obj.v_Vector4 = value;
            #if STACK_HISTROY
            obj.AddCallerInfo( _file, _line );
            #endif
        }
        
        #if STACK_HISTROY
        public static void SetColor( SerializeValue obj, Color value, [CallerFilePath] string _file = "", [CallerLineNumber] int _line = -1 )
        #else
        public static void SetColor( SerializeValue obj, Color value )
        #endif
        {
            obj.v_Color = value;
            #if STACK_HISTROY
            obj.AddCallerInfo( _file, _line );
            #endif
        }
        
        #if STACK_HISTROY
        public static void SetGameObject( SerializeValue obj, GameObject value, [CallerFilePath] string _file = "", [CallerLineNumber] int _line = -1 )
        #else
        public static void SetGameObject( SerializeValue obj, GameObject value )
        #endif
        {
            obj.v_GameObject = value;
            #if STACK_HISTROY
            obj.AddCallerInfo( _file, _line );
            #endif
        }
        
        #if STACK_HISTROY
        public static void SetLabel( SerializeValue obj, UnityEngine.UI.Text value, [CallerFilePath] string _file = "", [CallerLineNumber] int _line = -1 )
        #else
        public static void SetLabel( SerializeValue obj, UnityEngine.UI.Text value )
        #endif
        {
            obj.v_UILabel = value;
            #if STACK_HISTROY
            obj.AddCallerInfo( _file, _line );
            #endif
        }
        
        #if STACK_HISTROY
        public static void SetRawImage( SerializeValue obj, UnityEngine.UI.RawImage value, [CallerFilePath] string _file = "", [CallerLineNumber] int _line = -1 )
        #else
        public static void SetRawImage( SerializeValue obj, UnityEngine.UI.RawImage value )
        #endif
        {
            obj.v_UIRawImage = value;
            #if STACK_HISTROY
            obj.AddCallerInfo( _file, _line );
            #endif
        }
        
        #if STACK_HISTROY
        public static void SetImage( SerializeValue obj, UnityEngine.UI.Image value, [CallerFilePath] string _file = "", [CallerLineNumber] int _line = -1 )
        #else
        public static void SetImage( SerializeValue obj, UnityEngine.UI.Image value )
        #endif
        {
            obj.v_UIImage = value;
            #if STACK_HISTROY
            obj.AddCallerInfo( _file, _line );
            #endif
        }
        
     
        #if STACK_HISTROY
        public static void SetButton( SerializeValue obj, UnityEngine.UI.Button value, [CallerFilePath] string _file = "", [CallerLineNumber] int _line = -1 )
        #else
        public static void SetButton( SerializeValue obj, UnityEngine.UI.Button value )
        #endif
        {
            obj.v_UIButton = value;
            #if STACK_HISTROY
            obj.AddCallerInfo( _file, _line );
            #endif
        }
        
        
        #if STACK_HISTROY
        public static void SetToggle( SerializeValue obj, UnityEngine.UI.Toggle value, [CallerFilePath] string _file = "", [CallerLineNumber] int _line = -1 )
        #else
        public static void SetToggle( SerializeValue obj, UnityEngine.UI.Toggle value )
        #endif
        {
            obj.v_UIToggle = value;
            #if STACK_HISTROY
            obj.AddCallerInfo( _file, _line );
            #endif
        }
        
  
        #if STACK_HISTROY
        public static void SetGlobal( SerializeValue obj, object value, [CallerFilePath] string _file = "", [CallerLineNumber] int _line = -1 )
        #else
        public static void SetGlobal( SerializeValue obj, object value )
        #endif
        {
            obj.v_Global = value;
            #if STACK_HISTROY
            obj.AddCallerInfo( _file, _line );
            #endif
        }
        
      
        #if STACK_HISTROY
        public static void SetObject( SerializeValue obj, object value, [CallerFilePath] string _file = "", [CallerLineNumber] int _line = -1 )
        #else
        public static void SetObject( SerializeValue obj, object value )
        #endif
        {
            obj.v_Object = value;
            #if STACK_HISTROY
            obj.AddCallerInfo( _file, _line );
            #endif
        }
        
        #if STACK_HISTROY
        public static void Write( SerializeValue obj, SerializeValue src, [CallerFilePath] string _file = "", [CallerLineNumber] int _line = -1 )
        #else
        public static void Write( SerializeValue obj, SerializeValue src )
        #endif
        {
            obj._Write( src );
            #if STACK_HISTROY
            obj.AddCallerInfo( src.GetCallerHistory( ) );
            obj.AddCallerInfo( _file, _line );
            #endif
        }
        
       
        #if STACK_HISTROY
        public static void Write( SerializeValue obj, PropertyType propType, SerializeValue src, [CallerFilePath] string _file = "", [CallerLineNumber] int _line = -1 )
        #else
        public static void Write( SerializeValue obj, PropertyType propType, SerializeValue src )
        #endif
        {
            obj._Write( propType, src );
            #if STACK_HISTROY
            obj.AddCallerInfo( src.GetCallerHistory( ) );
            obj.AddCallerInfo( _file, _line );
            #endif
        }
        
       
        public static SerializeValue GetStaticProperty( string key )
        {
            string[] namefields = key.Split( '+' );
            if( namefields.Length < 2 ) return null;
            
            System.Type classType = System.Type.GetType( namefields[0] );
            
            string[] properties = namefields[1].Split( '.' );

            object instance = null;
            for( int i = 0; i < properties.Length-1; ++i )
            {
                System.Reflection.MethodInfo methodInfo = classType.GetMethod( 
                                                                properties[i], 
                                                                System.Reflection.BindingFlags.Instance|System.Reflection.BindingFlags.Public|System.Reflection.BindingFlags.NonPublic, 
                                                                null, 
                                                                System.Reflection.CallingConventions.Any, 
                                                                new System.Type[0], 
                                                                null );
                if( methodInfo != null )
                {
                    instance = methodInfo.Invoke( instance, null );
                }
                else 
                {
                    System.Reflection.PropertyInfo propertyInfo = classType.GetProperty( properties[i], System.Reflection.BindingFlags.Instance|System.Reflection.BindingFlags.Public|System.Reflection.BindingFlags.NonPublic );
                    if( propertyInfo != null )
                    {
                        instance = propertyInfo.GetValue( instance );
                    }
                }
                if( instance == null ) break;
                classType = instance.GetType( );
            }
            
            return GetProperty( classType, instance, properties[ properties.Length-1 ] );
        }
        
      
        public static SerializeValue GetProperty( System.Type type, object instance, string key )
        {
            SerializeValue target = null;
            
            
            try
            {
                System.Reflection.MethodInfo methodInfo = type.GetMethod( 
                                                                key, 
                                                                System.Reflection.BindingFlags.Instance|System.Reflection.BindingFlags.Public|System.Reflection.BindingFlags.NonPublic, 
                                                                null, 
                                                                System.Reflection.CallingConventions.Any, 
                                                                new System.Type[0], 
                                                                null );
                if( methodInfo != null )
                {
                    object res = methodInfo.Invoke( instance, null );
                    if( res != null )
                    {
                        target = MakeValue( res.GetType( ), res );
                    }
                }
                
                System.Reflection.PropertyInfo propertyInfo = type.GetProperty( key, System.Reflection.BindingFlags.Instance|System.Reflection.BindingFlags.Public|System.Reflection.BindingFlags.NonPublic );
                if( propertyInfo != null )
                {
                    target = MakeValue( propertyInfo.PropertyType, propertyInfo.GetValue( instance ) );
                }
            }
            catch( System.Exception e )
            {
                Debug.LogError( e.ToString( ) );
            }
            
            return target;
        }
        
        public static SerializeValue MakeValue( System.Type valueType, object value )
        {
            SerializeValue target = null;
            
            if( valueType != null )
            {
                if( valueType == Utility.TYPE_BOOL )
                {
                    target = SerializeValue.CreateBool( (bool)value );
                }
                else if( valueType == Utility.TYPE_INT )
                {
                    target = SerializeValue.CreateInt( (int)value );
                }
                else if( valueType == Utility.TYPE_FLOAT )
                {
                    target = SerializeValue.CreateFloat( (float)value );
                }
                else if( valueType == Utility.TYPE_STRING )
                {
                    target = SerializeValue.CreateString( (string)value );
                }
                else 
                {
                    target = SerializeValue.CreateObject( value );
                }
            }
            
            return target;
        }
        
        public static void SetStaticProperty( string key, object value )
        {
            string[] namefields = key.Split( '+' );
            if( namefields.Length < 2 ) return;
            
            System.Type classType = System.Type.GetType( namefields[0] );
            
            string[] properties = namefields[1].Split( '.' );
            
            object instance = null;
            for( int i = 0; i < properties.Length-1; ++i )
            {
                System.Reflection.MethodInfo methodInfo = classType.GetMethod( 
                                                                properties[i], 
                                                                System.Reflection.BindingFlags.Instance|System.Reflection.BindingFlags.Public|System.Reflection.BindingFlags.NonPublic, 
                                                                null, 
                                                                System.Reflection.CallingConventions.Any, 
                                                                new System.Type[0], 
                                                                null );
                if( methodInfo != null )
                {
                    instance = methodInfo.Invoke( instance, null );
                }
                else 
                {
                    System.Reflection.PropertyInfo propertyInfo = classType.GetProperty( properties[i], System.Reflection.BindingFlags.Instance|System.Reflection.BindingFlags.Public|System.Reflection.BindingFlags.NonPublic );
                    if( propertyInfo != null )
                    {
                        instance = propertyInfo.GetValue( instance );
                    }
                }
                if( instance == null ) break;
                classType = instance.GetType( );
            }
            
            SetProperty( classType, instance, properties[ properties.Length-1 ], value );
        }
        

        public static void SetProperty( System.Type type, object instance, string key, object value )
        {
            try
            {
                if( value != null )
                {
                    System.Type[] types = new System.Type[] { value.GetType() };
                    
                    System.Reflection.MethodInfo methodInfo = type.GetMethod( 
                                                                key, 
                                                                System.Reflection.BindingFlags.Instance|System.Reflection.BindingFlags.Public|System.Reflection.BindingFlags.NonPublic, 
                                                                null, 
                                                                System.Reflection.CallingConventions.Any, 
                                                                types, 
                                                                null );
                    if( methodInfo != null )
                    {
                        methodInfo.Invoke( instance, new object[] { value } );
                    }
                    
                    System.Reflection.PropertyInfo propertyInfo = type.GetProperty( key, System.Reflection.BindingFlags.Instance|System.Reflection.BindingFlags.Public|System.Reflection.BindingFlags.NonPublic );
                    if( propertyInfo != null )
                    {
                        propertyInfo.SetValue( instance, value );
                    }
                }
            }
            catch( System.Exception e )
            {
                Debug.LogError( e.ToString( ) );
            }
        }
        
        
        #if STACK_HISTROY
        
        public class CallerInfo
        {
            public string file;
            public int    line;
            
            public CallerInfo( string _file, int _line )
            {
                file = _file;
                line = _line;
            }
        }
        
        private List<CallerInfo> m_CallerHistory = new List<CallerInfo>( );
        
        public void AddCallerInfo( List<CallerInfo> history )
        {
            #if STACK_HISTROY
            if( history != null && history.Count > 0 )
            {
                m_CallerHistory.AddRange( history );
            }
            #endif
        }
        
        public List<CallerInfo> GetCallerHistory(  )
        {
            return m_CallerHistory;
        }
        
        #endif
        
        #if !STACK_HISTROY
        [Conditional("__DUMMY_SIMBOL__")]
        #endif
        public void AddCallerInfo( [CallerFilePath] string _file = "", [CallerLineNumber] int _line = -1 )
        {
            #if STACK_HISTROY
            if( string.IsNullOrEmpty( _file ) == false )
            {
                m_CallerHistory.Add( new CallerInfo( _file, _line ) );
            }
            #endif
        }
        
        #if !STACK_HISTROY
        [Conditional("__DUMMY_SIMBOL__")]
        #endif
        public void GetCallerHistoryString( ref List<(string _file,int _line)> result )
        {
            #if STACK_HISTROY
            if( result == null )
            {
                result = new List<(string _file,int _line)>( );
            }
            else
            {
                result.Clear( );
            }
            for( int i = 0; i < m_CallerHistory.Count; ++i )
            {
                var info = m_CallerHistory[i];
                result.Add( ( info.file, info.line ) );
            }
            #endif
        }
        
    }
    
    #if UNITY_EDITOR
    namespace SerializeValueEditor
    {
        public class InfoData
        {
            public string               name;
            public SerializeValueList   list;
            
            public InfoData( string _name, SerializeValueList _list )
            {
                name    = _name;
                list    = _list;
            }
        }
        
        public interface IInfomation
        {
            string          GetDataName( );
            List<InfoData>  GetDataList( );
        }
    }
    #endif
}
