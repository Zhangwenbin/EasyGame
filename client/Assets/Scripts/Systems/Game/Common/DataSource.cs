using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics.CodeAnalysis;

namespace EG
{
    public class DataSource : UnityEngine.MonoBehaviour
    {
        public struct DataPair
        {
            public System.Type  Type;
            public object       Data;

            public DataPair( System.Type type, object data )
            {
                Type = type;
                Data = data;
            }
        }

        public class DataList
        {
            private List<DataPair>  m_Data = new List<DataPair>( );
            
            public DataList Clone( )
            {
                return new DataList
                {
                    m_Data = m_Data.ToList()
                };
            }
            
            public void Clear( )
            {
                m_Data.Clear();
            }
            
            public List<DataPair> GetData( )
            {
                return m_Data;
            }
            
            public void Append( DataList list )
            {
                for( int i = 0; i < list.m_Data.Count; ++i )
                {
                    Add( list.m_Data[i].Type, list.m_Data[i].Data );
                }
            }
            
            public void Add( System.Type type , object data )
            {
                for( int i = 0; i < m_Data.Count; ++i )
                {
                    if( m_Data[ i ].Type == type )
                    {
                        m_Data[ i ] = new DataPair( type, data );
                        return;
                    }
                }

                m_Data.Add( new DataPair( type, data ) );
            }

   
            public void Add<T>(T data)
            {
                Add(typeof(T), data);
            }

    
            public T FindDataOfClass<T>( ref bool success )
            {
                success = false;
                for (int i = 0; i < m_Data.Count; ++i)
                {
                    if( m_Data[ i ].Type == typeof( T ) )
                    {
                        success = true;
                        return (T)m_Data[ i ].Data;
                    }
                }
                return default(T);
            }
            public T FindDataOfClass<T>(  T defaultValue )
            {
                for( int i = 0; i < m_Data.Count; ++i )
                {
                    if( m_Data[ i ].Type == typeof( T ) )
                    {
                        return (T)m_Data[ i ].Data;
                    }
                }
                return defaultValue;
            }
            public object FindDataOfClass( string className )
            {
                for( int i = 0; i < m_Data.Count; ++i )
                {
                    if( m_Data[ i ].Type.FullName.IndexOf( className ) != -1 )
                    {
                        return m_Data[ i ].Data;
                    }
                }
                return null;
            }
            public object FindDataOfType( string typeName )
            {
                for( int i = 0; i < m_Data.Count; ++i )
                {
                    if( m_Data[ i ].Type.Name == typeName )
                    {
                        return m_Data[ i ].Data;
                    }
                }
                return null;
            }
            internal T FindDataOfBaseClass<T>() where T : class
            {
                for (var i = 0; i < m_Data.Count; ++i)
                {
                    var data = m_Data[i].Data as T;
                    if (data != null)
                    {
                        return data;
                    }
                }
                return null;
            }
            
            public object FindDataOfClass( object type, ref bool success )
            {
                success = false;
                for( int i = 0; i < m_Data.Count; ++i )
                {
                    if( m_Data[ i ].Type.Equals( type ) )
                    {
                        success = true;
                        return m_Data[ i ].Data;
                    }
                }
                return null;
            }
            public object FindDataOfClass( object type, object defaultValue )
            {
                for( int i = 0; i < m_Data.Count; ++i )
                {
                    if( m_Data[ i ].Type.Equals( type ) )
                    {
                        return m_Data[ i ].Data;
                    }
                }
                return defaultValue;
            }
        }
        

        private DataList            m_DataList  = new DataList( );

        
        public void Clear( )
        {
            m_DataList.Clear();
        }


        public DataList GetDataList( )
        {
            return m_DataList;
        }

 
        public void Append(DataList list)
        {
            if (list == null) return;
            m_DataList.Append(list);
        }


        public void Add( System.Type type , object data )
        {
            if( type == typeof(DataList) )
            {
                m_DataList.Append( (DataList)data );
            }
            else
            {
                m_DataList.Add( type, data );
            }
        }


        public void Add<T>(T data)
        {
            Add(typeof(T), data);
        }

        
        public T FindDataOfClass<T>(  T defaultValue )
        {
            var result = defaultValue;
            bool isSuccess = false;
            
            result = m_DataList.FindDataOfClass<T>( ref isSuccess );
            if( isSuccess == false )
            {
                // 
                Transform parent = transform.parent;
                if( parent != null )
                {
                    DataSource source = parent.GetComponentInParent<DataSource>( );
                    if( source != null )
                    {
                        result = source.FindDataOfClass<T>( defaultValue );
                    }
                }
            }

            return result;
        }


        public object FindDataOfClass( object type, object defaultValue )
        {
            object result = defaultValue;
            bool isSuccess = false;

            // 
            result = m_DataList.FindDataOfClass( type, ref isSuccess );
            if( isSuccess == false )
            {
                // 
                Transform parent = transform.parent;
                if( parent != null )
                {
                    DataSource source = parent.GetComponentInParent<DataSource>( );
                    if( source != null )
                    {
                        result = source.FindDataOfClass( type, defaultValue );
                    }
                }
            }

            return result;
        }

 
        public object FindDataOfClass( string className )
        {
            object result = null;

            // 
            result = m_DataList.FindDataOfClass( className );
            if( result == null )
            {
                // 
                Transform parent = transform.parent;
                if( parent != null )
                {
                    DataSource source = parent.GetComponentInParent<DataSource>( );
                    if( source != null )
                    {
                        result = source.FindDataOfClass( className );
                    }
                }
            }

            return result;
        }

  
        public object FindDataOfType( string typeName )
        {
            object result = null;

            // 
            result = m_DataList.FindDataOfType( typeName );
            if( result == null )
            {
                // 
                Transform parent = transform.parent;
                if( parent != null )
                {
                    DataSource source = parent.GetComponentInParent<DataSource>( );
                    if( source != null )
                    {
                        result = source.FindDataOfType( typeName );
                    }
                }
            }

            return result;
        }


        public static T FindDataOfClass<T>(GameObject root,  T defaultValue)
        {

            var buffer = ListPool<DataSource>.Rent();
            root.GetComponentsInParent(true, buffer);
            var target = buffer.Count > 0 ? buffer[0] : null;
            ListPool<DataSource>.Return(buffer);
            if (target == null)
            {
                return defaultValue;
            }
            return target.FindDataOfClass<T>( defaultValue );
        }


        public static T FindDataOfBaseClass<T>(GameObject root) where T : class
        {
            var sources = ListPool<DataSource>.Rent();
            root.GetComponentsInParent(true, sources);
            T data = null;
            foreach (var source in sources)
            {
                data = source.m_DataList.FindDataOfBaseClass<T>();
                if (data != null)
                {
                    break;
                }
            }
            ListPool<DataSource>.Return(sources);
            return data;
        }

 
        public static DataSource Create( GameObject obj )
        {
            DataSource ds = obj.GetComponent<DataSource>( );

            if( ds == null )
            {
                ds = obj.AddComponent<DataSource>( );
                ds.hideFlags = HideFlags.DontSave | HideFlags.NotEditable;
            }

            return ds;
        }


        public static DataSource Bind<T>( GameObject obj, T data )
        {
            return Bind( obj, typeof( T ), data );
        }
        public static DataSource Bind<T>( Component comp, T data )
        {
            if( comp != null )
            {
                return Bind( comp.gameObject, typeof( T ), data );
            }
            return null;
        }


        public static DataSource Bind( GameObject obj, System.Type type, object data )
        {
            if( obj == null )
            {
                return null;
            }

            DataSource ds = null;

            // 
            if( type == typeof( DataSource ) )
            {
                DataSource src = data as DataSource;

                // 
                ds = obj.RequireComponent<DataSource>( );

                // 
                ds.Append( src.GetDataList( ) );
            }
            // 
            else if( type== typeof(DataList) )
            {
                // 
                ds = obj.RequireComponent<DataSource>( );

                // 
                ds.Append( data as DataList );
            }
            else
            {
                // 
                ds = Create( obj );

                // 
                ds.Add( type, data );
            }

            return ds;
        }

     
        public static void Release( GameObject obj )
        {
            DataSource ds = obj.GetComponent<DataSource>( );
            if( ds != null )
            {
                ds.Clear( );
            }
        }
        public static void Release( Component comp )
        {
            if( comp != null )
            {
                DataSource ds = comp.gameObject.GetComponent<DataSource>( );
                if( ds != null )
                {
                    ds.Clear( );
                }
            }
        }
        
        #if UNITY_EDITOR

        [UnityEditor.CustomEditor(typeof(DataSource))]
        class EditorInspector_DataSource: UnityEditor.Editor
        {
            public override void OnInspectorGUI ()
            {
                DataSource      ds      = target as DataSource;
                DataList        list    = ds.GetDataList( );
                List<DataPair>  data    = list.GetData( );
                GUIStyle        style   = new GUIStyle( GUI.skin.label );

                style.wordWrap = true;

                for (int i = 0; i < data.Count; ++i)
                {
                    UnityEditor.EditorGUILayout.LabelField( "Class:" + data[i].Type, style );
                    UnityEditor.EditorGUILayout.LabelField( "Data:" + data[i].Data, style );
                    UnityEditor.EditorGUILayout.Space( );
                }
            }
        }

        #endif
    }
}
