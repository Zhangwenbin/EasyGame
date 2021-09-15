using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace EG
{
    public static class Utility
    {
        // 
        public static System.Type TYPE_BOOL         = typeof(bool);
        public static System.Type TYPE_BYTE         = typeof(byte);
        public static System.Type TYPE_CHAR         = typeof(char);
        public static System.Type TYPE_INT          = typeof(int);
        public static System.Type TYPE_LONG         = typeof(long);
        public static System.Type TYPE_FLOAT        = typeof(float);
        public static System.Type TYPE_DOUBLE       = typeof(double);
        public static System.Type TYPE_STRING       = typeof(string);
        public static System.Type TYPE_STRINGS      = typeof(string[]);
        public static System.Type TYPE_VECTOR2      = typeof(Vector2);
        public static System.Type TYPE_VECTOR3      = typeof(Vector3);
        public static System.Type TYPE_VECTOR4      = typeof(Vector4);
        public static System.Type TYPE_COLOR        = typeof(Color);
        public static System.Type TYPE_ENUM         = typeof(System.Enum);
        public static System.Type TYPE_GAMEOBJECT   = typeof(GameObject);
        public static System.Type TYPE_TEXTASSET    = typeof(TextAsset);
        public static System.Type TYPE_TEXTURE      = typeof(Texture);
        public static System.Type TYPE_ANIMCLIP     = typeof(AnimationClip);
        public static System.Type TYPE_RAWIMAGE     = typeof(UnityEngine.UI.RawImage);
        public static System.Type TYPE_IMAGE        = typeof(UnityEngine.UI.Image);
        public static System.Type TYPE_TEXT         = typeof(UnityEngine.UI.Text);
        public static System.Type TYPE_BUTTON       = typeof(UnityEngine.UI.Button);
        public static System.Type TYPE_TOGGLE       = typeof(UnityEngine.UI.Toggle);
        public static System.Type TYPE_SCRIPTABLE   = typeof(ScriptableObject);
        //todo no SerializeValueList
        // public static System.Type TYPE_VALUELIST    = typeof(SerializeValueList);
        // public static System.Type TYPE_VALUE        = typeof(SerializeValue);
        public static System.Type TYPE_FONT         = typeof(Font);
        public static System.Type TYPE_MATERIAL     = typeof(Material);
        
        // 
        public enum EPlatform
        {
            EDITOR      = 0,
            STANDALONE  ,
            IOS         ,
            ANDROID     ,
        }
        

        #region 
        
        public static void SetActiveSafe( this GameObject self, bool value )
        {
            if( self != null && self.activeSelf != value )
            {
                self.SetActive( value );
            }
        }
        
        public static void SetActiveSafe( this Component self, bool value )
        {
            if( self != null )
            {
                GameObject gobj = self.gameObject;
                if( gobj != null && gobj.activeSelf != value )
                {
                    gobj.SetActive( value );
                }
            }
        }
        
        public static void SetPositionToZero(  this GameObject self, bool value )
        {
            if (self == null)
                return;

            var rect = self.transform as RectTransform;
            if (rect == null)
            {
                if (self.activeSelf != value)
                {
                    self.SetActive(value);
                }
                return;
            }

            if ( value )
            {
                if (!self.activeSelf)
                    self.SetActive(true);

                var zero = Vector2.zero;
                if (rect.anchoredPosition != zero)
                {
                    rect.anchoredPosition = zero;
                }
            }
            else
            {
                if (!self.activeSelf)
                {
                    // 
                    return;
                }

                var far = new Vector2(-65000, -65000);
                if (rect.anchoredPosition != far)
                {
                    rect.anchoredPosition = far;
                }
            }
        }


        //todo SerializeValueList
        // public static void SetPositionToZero( this SerializeValueList self, string key, bool value )
        // {
        //     if (self == null)
        //         return;
        //
        //     self.GetGameObject(key).SetPositionToZero(value);
        // }


        public static void SetParent( this GameObject self, GameObject parent, bool worldPositionStays )
        {
            if( self != null && parent != null )
            {
                self.transform.SetParent( parent.transform, worldPositionStays );
            }
        }
        public static void SetParent( this GameObject self, Transform parent, bool worldPositionStays )
        {
            if( self != null && parent != null )
            {
                self.transform.SetParent( parent, worldPositionStays );
            }
        }
        public static void SetParent( this GameObject self, Component parent, bool worldPositionStays )
        {
            if( self != null && parent != null )
            {
                self.transform.SetParent( parent.transform, worldPositionStays );
            }
        }
        
        public static void SetParent( this Component self, GameObject parent, bool worldPositionStays )
        {
            if( self != null && parent != null )
            {
                self.transform.SetParent( parent.transform, worldPositionStays );
            }
        }
        public static void SetParent( this Component self, Transform parent, bool worldPositionStays )
        {
            if( self != null && parent != null )
            {
                self.transform.SetParent( parent, worldPositionStays );
            }
        }
        public static void SetParent( this Component self, Component parent, bool worldPositionStays )
        {
            if( self != null && parent != null )
            {
                self.transform.SetParent( parent.transform, worldPositionStays );
            }
        }
        
        public static GameObject CreateChild( this GameObject self, string name )
        {
            GameObject gobj = new GameObject( name );
            gobj.transform.SetParent( self.transform, false );
            return gobj;
        }
        
        public static void SafeDestroy( this GameObject self, bool toCache = false )
        {
            if( self != null )
            {
                var monoBehaviour = self.GetComponent<AppMonoBehaviour>( );
                if( monoBehaviour != null )
                {
                    monoBehaviour.Release( );
                }
                GameObject.Destroy(self);
            }
        }
        

        public static void SafeDestroyImmidiate( this GameObject self )
        {
            if( self != null )
            {
                var monoBehaviour = self.GetComponent<AppMonoBehaviour>( );
                if( monoBehaviour != null )
                {
                    monoBehaviour.Release( );
                }
                GameObject.DestroyImmediate( self );
            }
        }
        
        public static void SafeDestroy( this Component self )
        {
            if( self != null )
            {
                GameObject gobj = self.gameObject;
                if( gobj != null )
                {
                    gobj.SafeDestroy( );
                }
            }
        }
        
        public static void SafeDestroy( this MonoBehaviour self )
        {
            if( self != null )
            {
                GameObject gobj = self.gameObject;
                if( gobj != null )
                {
                    gobj.SafeDestroy( );
                }
            }
        }
        
        public static void RemoveComponent<T>( this GameObject self ) where T: Component
        {
            if( self != null )
            {
                Component[] components = self.GetComponents<T>();
                if( components != null )
                {
                    for( int i = 0; i < components.Length; ++i )
                    {
                        Component.DestroyImmediate( components[ i ] );
                    }
                }
            }
        }
         

        public static void RemoveComponent( this Component self )
        {
            if( self != null )
            {
                Component.DestroyImmediate( self );
            }
        }
        

        public static string GetPath( this GameObject go, GameObject root = null )
        {
            System.Text.StringBuilder sb = GetStringBuilder( );
            
            if( root == null )
            {
                for( Transform tr = go.transform; tr != null; tr = tr.parent )
                {
                    sb.Insert( 0, tr.gameObject.name );
                    sb.Insert( 0, '/' );
                }
            }
            else
            {
                for( Transform tr = go.transform; tr != null && tr.gameObject != root; tr = tr.parent )
                {
                    sb.Insert( 0, tr.gameObject.name );
                    sb.Insert( 0, '/' );
                }
            }
            
            return sb.ToString( );
        }
        
        //todo no DataSource
        // public static DataSource GetDataSource( this GameObject go )
        // {
        //     if( go != null )
        //     {
        //         DataSource source = go.GetComponent<DataSource>( );
        //         if( source == null )
        //         {
        //             source = go.GetComponentInParent<DataSource>( );
        //         }
        //         return source;
        //     }
        //     return null;
        // }
        
        //
        // public static T GetDataSource<T>( this GameObject go )
        // {
        //     if( go != null )
        //     {
        //         DataSource source = go.GetComponent<DataSource>( );
        //         if( source == null )
        //         {
        //             source = go.GetComponentInParent<DataSource>( );
        //         }
        //         if( source != null )
        //         {
        //             return source.FindDataOfClass<T>( default(T) );
        //         }
        //     }
        //     return default(T);
        // }
        

        static public GameObject FindChild( this GameObject parent, string targetName, bool includeInactive )
        {
            Transform transform = parent.transform;
            if( includeInactive )
            {
                for( int i = 0, cnt = transform.childCount; i < cnt; ++i )
                {
                    Transform child = transform.GetChild( i );
                    if( child.name == targetName )
                    {
                        return child.gameObject;
                    }
                }
            }
            else
            {
                for( int i = 0, cnt = transform.childCount; i < cnt; ++i )
                {
                    Transform child = transform.GetChild( i );
                    if( child.gameObject.activeInHierarchy && child.name == targetName )
                    {
                        return child.gameObject;
                    }
                }
            }
            return null;
        }
        static public GameObject FindChild( this GameObject parent, string targetName )
        {
            return FindChild( parent, targetName, false );
        }
        // 
        static public GameObject FindChild( this Component parent, string targetName, bool includeInactive )
        {
            Transform transform = parent.transform;
            if( includeInactive )
            {
                for( int i = 0, cnt = transform.childCount; i < cnt; ++i )
                {
                    Transform child = transform.GetChild( i );
                    if( child.name == targetName )
                    {
                        return child.gameObject;
                    }
                }
            }
            else
            {
                for( int i = 0, cnt = transform.childCount; i < cnt; ++i )
                {
                    Transform child = transform.GetChild( i );
                    if( child.gameObject.activeInHierarchy && child.name == targetName )
                    {
                        return child.gameObject;
                    }
                }
            }
            return null;
        }
        static public GameObject FindChild( this Component parent, string targetName )
        {
            return FindChild( parent, targetName, false );
        }
        
   
        static public GameObject FindChildAll( this GameObject parent, string targetName, bool includeInactive )
        {
            Transform[] children = parent.GetComponentsInChildren<Transform>( includeInactive );
            foreach( Transform child in children )
            {
                if( child.name == targetName && parent != child.gameObject )
                {
                    return child.gameObject;
                }
            }
            return null;
        }
        static public GameObject FindChildAll( this GameObject parent, string targetName )
        {
            return FindChildAll( parent, targetName, false );
        }
        // 
        static public GameObject FindChildAll( this Component parent, string targetName, bool includeInactive )
        {
            Transform[] children = parent.GetComponentsInChildren<Transform>( includeInactive );
            foreach( Transform child in children )
            {
                if( child.name == targetName && parent != child )
                {
                    return child.gameObject;
                }
            }
            return null;
        }
        static public GameObject FindChildAll( this Component parent, string targetName )
        {
            return FindChildAll( parent, targetName, false );
        }
        

        static public T FindChildComponent<T>( this GameObject parent, string targetName, bool includeInactive ) where T : Component
        {
            GameObject obj = parent.FindChild( targetName, includeInactive );
            if( obj != null )
            {
                return obj.GetComponent( typeof( T ) ) as T;
            }
            return null;
        }
        static public T FindChildComponent<T>( this GameObject parent, string targetName ) where T : Component
        {
            return FindChildComponent<T>( parent, targetName, false );
        }
        // 
        static public T FindChildComponent<T>( this Component parent, string targetName, bool includeInactive ) where T : Component
        {
            GameObject obj = parent.FindChild( targetName, includeInactive );
            if( obj != null )
            {
                return obj.GetComponent( typeof( T ) ) as T;
            }
            return null;
        }
        static public T FindChildComponent<T>( this Component parent, string targetName ) where T : Component
        {
            return FindChildComponent<T>( parent, targetName, false );
        }
        

        static public T FindChildComponentAll<T>( this GameObject parent, string targetName, bool includeInactive ) where T : Component
        {
            T[] children = parent.GetComponentsInChildren<T>( includeInactive );
            foreach( T child in children )
            {
                if( child.name == targetName && parent != child.gameObject )
                {
                    return child;
                }
            }
            return null;
        }
        static public T FindChildComponentAll<T>( this GameObject parent, string targetName ) where T : Component
        {
            return FindChildComponentAll<T>( parent, targetName, false );
        }
        // 
        static public T FindChildComponentAll<T>( this Component parent, string targetName, bool includeInactive ) where T : Component
        {
            GameObject obj = parent.FindChildAll( targetName, includeInactive );
            if( obj != null )
            {
                return obj.GetComponent( typeof( T ) ) as T;
            }
            return null;
        }
        static public T FindChildComponentAll<T>( this Component parent, string targetName ) where T : Component
        {
            return FindChildComponentAll<T>( parent, targetName, false );
        }
        

        public static GameObject[] GetChilds( this GameObject self, bool includeInactive )
        {
            Transform transform = self.transform;
            int max = transform.childCount;
             
            if( max > 0 )
            {
                List<GameObject> result = new List<GameObject>();
                
                if( includeInactive )
                {
                    for( int i = 0; i < max; ++i )
                    {
                        GameObject gobj = transform.GetChild( i ).gameObject;
                        result.Add( gobj );
                    }
                }
                else
                {
                    for( int i = 0; i < max; ++i )
                    {
                        GameObject gobj = transform.GetChild( i ).gameObject;
                        if( gobj.activeInHierarchy )
                        {
                            result.Add( gobj );
                        }
                    }
                }
                
                return result.ToArray();
            }
            
            return null;
        }
        public static GameObject[] GetChilds( this GameObject self )
        {
            return GetChilds( self, false );
        }
        
 
        public static GameObject[] GetChildsAll( this GameObject self, bool includeInactive )
        {
            List<GameObject> result = new List<GameObject>( );
            GameObject[] childs = self.GetChilds( includeInactive );
            if( childs != null )
            {
                result.AddRange( childs );
                for( int i = 0; i < childs.Length; ++i )
                {
                    GameObject[] tmps = childs[ i ].GetChildsAll( includeInactive );
                    if( tmps != null )
                    {
                        result.AddRange( tmps );
                    }
                }
            }
            return result.ToArray( );
        }
        
  
        public static T GetComponent<T>( this GameObject[] parents ) where T : Component
        {
            for( int i = 0; i < parents.Length; ++i )
            {
                T result = parents[ i ].GetComponent<T>( );
                if( result != null )
                {
                    return result;
                }
            }
            return null;
        }
        

        public static T RequireComponent<T>( this GameObject self ) where T : Component
        {
            T result = self.GetComponent<T>( );
            if( result == null )
            {
                result = self.AddComponent<T>( );
            }
            return result;
        }
        

        public static T GetComponentSafe<T>( this GameObject self ) where T : Component
        {
            if (!self) return null;
            #if UNITY_EDITOR
            var list = ListPool<T>.Rent();
            self.GetComponents(list);
            var comp = list.Count > 0 ? list[0] : null;
            list.Free();
            #else
            var comp = self.GetComponent<T>();
            #endif
            return comp;
        }


        public static T GetComponentSafe<T>( this Component self ) where T : Component
        {
            if (!self) return null;
            #if UNITY_EDITOR
            var list = ListPool<T>.Rent();
            self.GetComponents(list);
            var comp = list.Count > 0 ? list[0] : null;
            list.Free();
            #else
            var comp = self.GetComponent<T>();
            #endif
            return comp;
        }
        
        //todo MonoEvent
        // public static MonoEvent AddMonoEvent( this GameObject go, MonoEvent.Action action )
        // {
        //     MonoEvent monoEv = go.RequireComponent<MonoEvent>( );
        //     if( monoEv != null )
        //     {
        //         monoEv += action;
        //     }
        //     return monoEv;
        // }
        
        // public static void RemoveMonoEvent( this GameObject go, MonoEvent.Action action )
        // {
        //     MonoEvent monoEv = go.GetComponent<MonoEvent>( );
        //     if( monoEv != null )
        //     {
        //         monoEv -= action;
        //     }
        // }
        
        #endregion
        

        #region 

        public static Vector2 GetPivotAnchoredPosition( this RectTransform transform, Vector2 pos )
        {
            Vector2 pivot = transform.pivot;
            Vector2 size = transform.sizeDelta;
            pos.x += ( 0.5f - pivot.x ) * size.x;
            pos.y += ( 0.5f - pivot.y ) * size.y;
            return pos;
        }
        
        public static Vector2 GetPivotAnchoredPosition( this RectTransform transform )
        {
            Vector2 pivot = transform.pivot;
            Vector2 size = transform.sizeDelta;
            Vector2 pos = transform.anchoredPosition;
            pos.x += ( 0.5f - pivot.x ) * size.x;
            pos.y += ( 0.5f - pivot.y ) * size.y;
            return pos;
        }
        
        #endregion
        

        #region 
        
        public static Vector3 RaycastGround( Vector3 position, int layerMask = 0x7FFFFFFF )
        {
            RaycastHit hit;
            if( Physics.Raycast( new Vector3( position.x, 1000, position.z ), -Vector3.up, out hit, 1000.0f, layerMask ) )
            {
                return hit.point;
            }
            position.y = 0;
            return position;
        }
        public static Vector3 RaycastGround( Vector2 position )
        {
            return RaycastGround( new Vector3( position.x, 0, position.y ) );
        }
        public static float RaycastGround( float x, float z )
        {
            return RaycastGround( new Vector3( x, 0, z ) ).y;
        }
        
        public static float CalcRaycastHeight( float x, float y )
        {
            float maxY = 100;
            
            float x0 = Mathf.Floor( x - 0.5f ) + 0.5f;
            float y0 = Mathf.Floor( y - 0.5f ) + 0.5f;
            float x1 = Mathf.Ceil( x - 0.5f ) + 0.5f;
            float y1 = Mathf.Ceil( y - 0.5f ) + 0.5f;
            float h00 = 0;
            float h10 = 0;
            float h01 = 0;
            float h11 = 0;
            
            RaycastHit hit;
            
            if( Physics.Raycast( new Vector3( x0, maxY, y0 ), -Vector3.up, out hit ) )
            {
                h00 = hit.point.y;
            }
            if( Physics.Raycast( new Vector3( x1, maxY, y0 ), -Vector3.up, out hit ) )
            {
                h10 = hit.point.y;
            }
            if( Physics.Raycast( new Vector3( x0, maxY, y1 ), -Vector3.up, out hit ) )
            {
                h01 = hit.point.y;
            }
            if( Physics.Raycast( new Vector3( x1, maxY, y1 ), -Vector3.up, out hit ) )
            {
                h11 = hit.point.y;
            }
            
            float u = (x - x0);
            float v = (y - y0);
            float h = Mathf.Lerp( Mathf.Lerp( h00, h10, u ), Mathf.Lerp( h01, h11, u ), v ); 
            
            return h;
        }
        
        #endregion
        
        #region 
       　
        // ENUM
        public class EnumArray
        {
            public class Element
            {
                public object   index;
                public string   dispName;
                public object   value1;
                public object   value2;
                
                public Element( object _index, string _dispName, object _value1, object _value2 )
                {
                    index = _index;
                    dispName = _dispName;
                    value1 = _value1;
                    value2 = _value2;
                }

                public Element( object _index, string _dispName ) : this( _index, _dispName, (int)0, (int)0 )
                {
                }
            }
            
            public class Set
            {
                public int      index;
                public string   name;
                public string   dispName;
            }
            
            public Set[]        sets;
            public Element[]    elements;
            
            public string[]     names
            {
                get
                {
                    string[] result = null;
                    if( sets != null )
                    {
                        result = new string[ sets.Length ];
                        for( int i = 0; i < sets.Length; ++i )
                        {
                            result[ i ] = sets[ i ].name;
                        }
                    }
                    else
                    {
                        result = new string[ elements.Length ];
                        for( int i = 0; i < elements.Length; ++i )
                        {
                            result[ i ] = elements[ i ].index.ToString( );
                        }
                    }
                    return result;
                }
            }
            
            public string[]     dispNames
            {
                get
                {
                    string[] result = null;
                    if( sets != null )
                    {
                        result = new string[ sets.Length ];
                        for( int i = 0; i < sets.Length; ++i )
                        {
                            result[ i ] = sets[ i ].dispName;
                        }
                    }
                    else
                    {
                        result = new string[ elements.Length ];
                        for( int i = 0; i < elements.Length; ++i )
                        {
                            result[ i ] = elements[ i ].dispName;
                        }
                    }
                    return result;
                }
            }
            
            public EnumArray( string[] _names, string[] _dispNames )
            {
                sets = new Set[ _names.Length ];
                for( int i = 0; i < _names.Length; ++i )
                {
                    Set set = new Set( );
                    set.index   = i;
                    set.name    = _names[i];
                    set.dispName= _dispNames[i];
                    sets[ i ] = set;
                }
            }
            public EnumArray( Element[] _elements )
            {
                elements = _elements;
            }
            
            public void Sort( )
            {
                ArrayUtility.StableSort<Set>( sets, ( Set a, Set b ) => a.dispName.CompareTo( b.dispName ) );
            }
            
            public int FindIndex( string enumValue )
            {
                if( sets != null )
                {
                    return ArrayUtility.FindIndex<Set>( sets, ( prop ) => prop.name == enumValue );
                }
                else if( elements != null )
                {
                    return ArrayUtility.FindIndex<Element>( elements, ( prop ) => (string)prop.index == enumValue );
                }
                return -1;
            }
            public int FindIndex( int intValue )
            {
                if( sets != null )
                {
                    return ArrayUtility.FindIndex<Set>( sets, ( prop ) => prop.name == intValue.ToString() );
                }
                else if( elements != null )
                {
                    return ArrayUtility.FindIndex<Element>( elements, ( prop ) => (int)prop.index == intValue );
                }
                return -1;
            }
            
            public string GetDispName( int intValue )
            {
                if( sets != null )
                {
                    int index = FindIndex( intValue );
                    if( index != -1 ) return sets[ index ].dispName;
                }
                else if( elements != null )
                {
                    int index = FindIndex( intValue );
                    if( index != -1 ) return elements[ index ].dispName;
                }
                return "";
            }

            public T GetEnumFromDispIndex<T>( int intValue ) where T : System.Enum
            {
                return (T)elements[ intValue ].index;
            }
            
            public Element GetElementFromDispIndex( int intValue )
            {
                return elements[ intValue ];
            }
            public Element GetElement( int intValue )
            {
                int index = FindIndex( intValue );
                if( index != -1 ) return elements[ index ];
                return null;
            }
            public Element[] GetElements( int mask )
            {
                List<Element> result = new List<Element>( );
                for( int i = 0; i < elements.Length; ++i )
                {
                    if( ( (int)elements[ i ].index & mask ) != 0 )
                    {
                        result.Add( elements[ i ] );
                    }
                }
                return result.ToArray();
            }
            public Element[] GetElements( int[] intValues )
            {
                List<Element> result = new List<Element>( );
                for( int i = 0; i < intValues.Length; ++i )
                {
                    int index = FindIndex( intValues[i] );
                    if( index != -1 ) result.Add( elements[ index ] );
                }
                return result.ToArray();
            }
            
            public object Parse( System.Type enumType, int index )
            {
                return System.Enum.Parse( enumType, sets[ index ].name );
            }
        }
        

        public static T EnumParse<T>( string enumTypeName )
        {
            try
            {
                T result = (T)System.Enum.Parse( typeof( T ), enumTypeName );
                return result;
            }
            catch( System.Exception e )
            {
                Debug.LogError( e.ToString() );
            }
            return default(T);
        }
        

        public static T EnumParse<T>( int enumTypeValue )
        {
            try
            {
                T result = (T)System.Enum.ToObject( typeof( T ), enumTypeValue );
                return result;
            }
            catch( System.Exception e )
            {
                Debug.LogError( e.ToString() );
            }
            return default(T);
        }
        
        public static string EnumToString( System.Type enumType, object enumObject )
        {
            return enumObject.ToString( );
        }
        

        public static int EnumToInt( System.Type enumType, object enumObject )
        {
            try
            {
                int result = System.Convert.ToInt32( System.Enum.Format( enumType, enumObject, "D" ) );
                return result;
            }
            catch( System.Exception e )
            {
                Debug.LogError( e.ToString() );
            }
            return 0;
        }
        
        public static int EnumStringToInt( System.Type enumType, string enumTypeName )
        {
            try
            {
                object obj = System.Enum.Parse( enumType, enumTypeName );
                return System.Convert.ToInt32( obj );
            }
            catch( System.Exception e )
            {
                Debug.LogError( e.ToString() );
            }
            return 0;
        }
        
        #endregion
        
        #region common
        

        public static System.Type GetType( string typeName )
        {
            System.Type defaultType = typeof( MonoBehaviour );
            return defaultType.Assembly.GetType( typeName );
        }
        

        public static Color Color( int r, int g, int b )
        {
            float t = 1.0f / 255.0f;
            return new Color( r*t, g*t, b*t ); 
        }
        public static Color Color( int r, int g, int b, int a )
        {
            float t = 1.0f / 255.0f;
            return new Color( r*t, g*t, b*t, a*t ); 
        }
        public static Color Color( int[] tbl )
        {
            if( tbl == null || tbl.Length < 2 ) return UnityEngine.Color.white;
            float r = (float)tbl[0] / 255.0f;
            float g = (float)tbl[1] / 255.0f;
            float b = (float)tbl[2] / 255.0f;
            float a = tbl.Length > 3 ? (float)tbl[3] / 255.0f: 1.0f;
            return new Color( r, g, b, a ); 
        }
        public static Color Color( int[] tbl, Color defaultColor )
        {
            if( tbl == null || tbl.Length < 2 ) return defaultColor;
            float r = (float)tbl[0] / 255.0f;
            float g = (float)tbl[1] / 255.0f;
            float b = (float)tbl[2] / 255.0f;
            float a = tbl.Length > 3 ? (float)tbl[3] / 255.0f: 1.0f;
            return new Color( r, g, b, a ); 
        }
        
        #endregion
        
        #region 文字列
        
        // string.Format 
        // www.atmarkit.co.jp/ait/articles/0401/30/news069.html
        
        static System.Text.StringBuilder g_StringBuilderLocal   = new System.Text.StringBuilder(128);
        static System.Text.StringBuilder g_StringBuilder        = new System.Text.StringBuilder(128);
        
        public static System.Text.StringBuilder GetStringBuilder()
        {
            g_StringBuilder.Length = 0;
            return g_StringBuilder;
        }
        

        public static string StringConcat( string str1, params string[] strs )
        {
            g_StringBuilderLocal.Length = 0;
            g_StringBuilderLocal.Append( str1 );
            for( int i = 0; i < strs.Length; ++i ) g_StringBuilderLocal.Append( strs[i] );
            return g_StringBuilderLocal.ToString( );
        }
        public static string StringConcat( string str1, string str2 )
        {
            g_StringBuilderLocal.Length = 0;
            g_StringBuilderLocal.Append( str1 );
            g_StringBuilderLocal.Append( str2 );
            return g_StringBuilderLocal.ToString( );
        }
        
        public static string GetStringCutFront( string str, string rm, bool inrm )
        {
            int s = str.IndexOf( rm );
            
            if( s != -1 )
            {
                if( inrm == false )
                {
                    return str.Substring( s, str.Length - s );
                }
                else
                {
                    return str.Substring( s + rm.Length, str.Length - s - rm.Length );
                }
            }
            
            return str;
        }
        public static string GetStringCutFront( string str, string rm )
        {
            return GetStringCutFront( str, rm, false );
        }
        

        public static string GetStringCut( string str, string rm, bool inrm )
        {
            int s = str.IndexOf( rm );
            
            if( s != -1 )
            {
                if( inrm == false )
                {
                    return str.Remove( s + rm.Length );
                }
                else
                {
                    return str.Remove( s );
                }
            }
            
            return str;
        }
        public static string GetStringCut( string str, string rm )
        {
            return GetStringCut( str, rm, true );
        }
        

        public static string GetStringRemove( string str, string rm )
        {
            int s = str.IndexOf( rm );
            
            if( s != -1 )
            {
                return str.Remove( s, rm.Length );
            }
            
            return str;
        }
        
        public static string GetStringPickup( string str, char st, char ed, int startIndex=0 )
        {
            int s = str.IndexOf( st, startIndex );
            int e = str.IndexOf( ed, startIndex );
            
            if( s != -1 && e != -1 )
            {
                return str.Substring( s+1, e-s-1 );
            }
            
            return "";
        }
        public static string GetStringPickup( string str, string st, string ed, int startIndex=0 )
        {
            int s = str.IndexOf( st, startIndex );
            int e = str.IndexOf( ed, startIndex );
            
            if( s != -1 && e != -1 )
            {
                return str.Substring( s+st.Length, e-s-st.Length );
            }
            
            return "";
        }
        

        public static bool ParseBool( string s )
        {
            bool result = false;
            if( string.IsNullOrEmpty( s ) == false )
            {
                int intResult = 0;
                if( int.TryParse( s, out intResult ) )
                {
                    if( intResult == 0 ) return false;
                    else                 return true;
                }
                else if( bool.TryParse( s, out result ) )
                {
                    return result;
                }
            }
            return result;
        }
        
        public static int ParseInt( string s, int defaultValue )
        {
            int result = defaultValue;
            if( string.IsNullOrEmpty( s ) == false )
            {
                if( int.TryParse( s, out result ) == false )
                {
                    result = defaultValue;
                }
            }
            return result;
        }
        public static int ParseInt( string s )
        {
            int result = 0;
            if( string.IsNullOrEmpty( s ) == false )
            {
                if( int.TryParse( s, out result ) == false )
                {
                    result = 0;
                }
            }
            return result;
        }
        

        public static float ParseFloat( string s )
        {
            float result = 0;
            if( string.IsNullOrEmpty( s ) == false )
            {
                if( float.TryParse( s, out result ) == false )
                {
                    result = 0;
                }
            }
            return result;
        }
        

        public static T ParseEnum<T>( string s, T defaultValue )
        {
            T result = defaultValue;
            if( string.IsNullOrEmpty( s ) == false )
            {
                try
                {
                    result = (T)System.Enum.Parse( typeof( T ), s );
                }
                catch( System.Exception e )
                {
                    Debug.LogError( e.ToString( ) );
                }
            }
            return result;
        }
        

        public static byte[] ToByteArray( string srcStr, System.Text.Encoding enc )
        {
            if( string.IsNullOrEmpty( srcStr ) ) return null;
            return enc.GetBytes( srcStr );
        }
        

        public static string ToString( byte[] data, System.Text.Encoding enc )
        {
            if( data == null ) return "";
            return enc.GetString( data );
        }
        
        #endregion
        

        #region 
        

        public static string CalcMd5( string srcStr, System.Text.Encoding enc )
        {
            System.Security.Cryptography.MD5 md5 = System.Security.Cryptography.MD5.Create();
            
            // md5
            byte[] srcBytes = enc.GetBytes(srcStr);
            byte[] destBytes = md5.ComputeHash(srcBytes);
            
            // 
            System.Text.StringBuilder destStrBuilder;
            destStrBuilder = new System.Text.StringBuilder();
            foreach (byte curByte in destBytes) {
                destStrBuilder.Append(curByte.ToString("x2"));
            }
            
            // 
            return destStrBuilder.ToString();
        }
        
  
        public static string CalcMd5( byte[] srcBytes )
        {
            System.Security.Cryptography.MD5 md5 = System.Security.Cryptography.MD5.Create();
            
            // 
            byte[] destBytes = md5.ComputeHash(srcBytes);
            
            // 
            System.Text.StringBuilder destStrBuilder;
            destStrBuilder = new System.Text.StringBuilder();
            foreach (byte curByte in destBytes) {
                destStrBuilder.Append(curByte.ToString("x2"));
            }
            
            // 
            return destStrBuilder.ToString();
        }
        

        public static string CalcMd5( System.IO.FileInfo fileInfo )
        {
            System.IO.FileStream stream = fileInfo.OpenRead();
            byte[] srcBytes = new byte[ stream.Length ];
            stream.Read( srcBytes, 0, (int)stream.Length );
            stream.Close();
                    
            System.Security.Cryptography.MD5 md5 = System.Security.Cryptography.MD5.Create();
            
            // 
            byte[] destBytes = md5.ComputeHash(srcBytes);
            
            // 
            System.Text.StringBuilder destStrBuilder;
            destStrBuilder = new System.Text.StringBuilder();
            foreach (byte curByte in destBytes) {
                destStrBuilder.Append(curByte.ToString("x2"));
            }
            
            // 
            return destStrBuilder.ToString();
        }
        
        public static string CalcSHA256( string srcStr, System.Text.Encoding enc )
        {
            System.Security.Cryptography.SHA256 sha256 = System.Security.Cryptography.SHA256.Create();
            
            // 
            byte[] srcBytes = enc.GetBytes(srcStr);
            byte[] destBytes = sha256.ComputeHash(srcBytes);
            
            // 
            System.Text.StringBuilder destStrBuilder;
            destStrBuilder = new System.Text.StringBuilder();
            foreach (byte curByte in destBytes)
            {
                destStrBuilder.Append(curByte.ToString("x2"));
            }
            
            // 
            return destStrBuilder.ToString();
        }

        #endregion
        

        #region 
        

        public static Texture2D ToTexture2D( this Texture self )
        {
            var sw = self.width;
            var sh = self.height;
            var format = TextureFormat.RGBA32;
            var result = new Texture2D( sw, sh, format, false );
            var currentRT = RenderTexture.active;
            var rt = new RenderTexture( sw, sh, 32 );
            Graphics.Blit( self, rt );
            RenderTexture.active = rt;
            var source = new Rect( 0, 0, rt.width, rt.height );
            result.ReadPixels( source, 0, 0 );
            result.Apply();
            RenderTexture.active = currentRT;
            return result;
        }
        
        #endregion 
        
        #region 例外

        public class KeyNotFoundException<T> : System.Exception
        {
            public KeyNotFoundException( string key ) :
                base(typeof(T).ToString() + " '" + key + "' doesn't exist.")
            { 

            }
        }
        
        public static void Throw(Exception except)
        {
            throw except;
        }


        public static void Throw(string msg)
        {
            throw new Exception(msg);
        }
        
        public static T AssertNotNull<T>( T target, string msg)
        {
            if (target == null)
            {
                throw new NullReferenceException(msg);
            }
            return target;
        }

        #endregion
    }
}
