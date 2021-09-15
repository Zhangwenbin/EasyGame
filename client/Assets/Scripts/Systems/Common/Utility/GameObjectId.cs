
using UnityEngine;
using UnityEngine.Serialization;
using System.Collections.Generic;
using System.Linq;

namespace EG
{
    [AddComponentMenu("Scripts/System/Game/Common/GameObjectId")]
    public class GameObjectId : EG.AppMonoBehaviour
    {
        static Dictionary<string, List<GameObject>> m_GameObjects = new Dictionary<string, List<GameObject>>();
        
        #region 
        
        [SerializeField] string m_Id = "NewGameObjectID";
        
        #endregion
        
        #region 
        
        public string Id
        {
            set
            {
                if( m_Id != value )
                {
                    Release( );
                    m_Id = value;
                    Initialize( );
                }
            }
            get
            {
                return m_Id;
            }
        }
        
        #endregion
        
        //=========================================================================
        //. 初期化/破棄
        //=========================================================================
        #region 初期化/破棄
        
        /// ***********************************************************************
        /// <summary>
        /// 起動した瞬間に呼ばれる
        /// </summary>
        /// ***********************************************************************
        private void Awake( )
        {
            Release( );
            Initialize( );
        }
        
        /// ***********************************************************************
        /// <summary>
        /// 初期化
        /// </summary>
        /// ***********************************************************************
        public override void Initialize( )
        {
            if( string.IsNullOrEmpty( m_Id ) == false )
            {
                if( m_GameObjects.ContainsKey( m_Id ) == false )
                {
                    m_GameObjects[ m_Id ] = new List<GameObject>( );
                }
                m_GameObjects[ m_Id ].Insert( 0, gameObject );
            }
        }
        
        /// ***********************************************************************
        /// <summary>
        /// 起動した瞬間に呼ばれる
        /// </summary>
        /// ***********************************************************************
        public override void Release( )
        {
            List<string> removes = new List<string>( );
            
            foreach( var pair in m_GameObjects )
            {
                List<GameObject> list = pair.Value;
                if( list != null )
                {
                    list.Remove( gameObject );
                    for( int i = 0; i < list.Count; ++i )
                    {
                        if( list[ i ] == null )
                        {
                            list.RemoveAt( i-- );
                        }
                    }
                }
                if( list.Count == 0 )
                {
                    removes.Add( pair.Key );
                }
            }
            
            for( int i = 0; i < removes.Count; ++i )
            {
                m_GameObjects.Remove( removes[ i ] );
            }
        }
        
        #endregion
        
        //=========================================================================
        //. 設定/取得
        //=========================================================================
        #region 設定/取得
        
        /// ***********************************************************************
        /// <summary>
        /// 検索
        /// </summary>
        /// ***********************************************************************
        public static GameObject FindGameObject( string name )
        {
            try
            {
                List<GameObject> gameObjects = m_GameObjects[ name ];
                return gameObjects[ 0 ];
            }
            catch( System.Exception )
            {
                return null;
            }
        }

        public static T FindGameObject<T>( string name ) where T: Component
        {
            try
            {
                List<GameObject> gameObjects = m_GameObjects[ name ];
                return gameObjects[ 0 ].GetComponent<T>( );
            }
            catch( System.Exception )
            {
                return null;
            }
        }

        public static GameObject[] FindGameObjects( string name )
        {
            try
            {
                List<GameObject> gameObjects = m_GameObjects[ name ];
                return gameObjects.ToArray( );
            }
            catch( System.Exception )
            {
                return new GameObject[0];
            }
        }
        
        #endregion
        
        //=========================================================================
        //. イベント
        //=========================================================================
        #region イベント
        
        #endregion
        
        //=========================================================================
        //. エディタ用
        //=========================================================================
        #if UNITY_EDITOR
        
        public static string[] Keys
        {
            get
            {
                List<string> result = new List<string>( );
                m_GameObjects.Clear( );
                GameObjectId[] list = GameObject.FindObjectsOfType<GameObjectId>( );
                if( list.Length > 0 )
                {
                    for( int i = 0; i < list.Length; ++i )
                    {
                        list[ i ].Release( );
                        list[ i ].Initialize( );
                    }
                    result.AddRange( m_GameObjects.Keys.ToArray() );
                    CollectionUtility.StableSort<string>( result, ( p1, p2 ) => p1.CompareTo( p2 ) );
                }
                m_GameObjects.Clear( );
                return result.ToArray( );
            }
        }
        
        public static KeyValuePair<string,List<GameObject>>[] Pairs
        {
            get
            {
                List<KeyValuePair<string,List<GameObject>>> result = new List<KeyValuePair<string, List<GameObject>>>( );
                m_GameObjects.Clear( );
                GameObjectId[] list = GameObject.FindObjectsOfType<GameObjectId>( );
                if( list.Length > 0 )
                {
                    for( int i = 0; i < list.Length; ++i )
                    {
                        list[ i ].Release( );
                        list[ i ].Initialize( );
                    }
                    foreach( var pair in m_GameObjects )
                    {
                        result.Add( pair );
                    }
                    CollectionUtility.StableSort<KeyValuePair<string,List<GameObject>>>( result, ( p1, p2 ) => p1.Key.CompareTo( p2.Key ) );
                }
                m_GameObjects.Clear( );
                return result.ToArray( );
            }
        }
        
        /// ***********************************************************************
        /// <summary>
        /// エディタに必要な情報の整理
        /// </summary>
        /// ***********************************************************************
        private void OnValidate( )
        {
        }
        
        #endif
    }
    
    //=========================================================================
    //. エディタ
    //=========================================================================
    #if UNITY_EDITOR
    [UnityEditor.CustomEditor(typeof(GameObjectId))]
    public class EditorInspector_GameObjectId: UnityEditor.Editor
    {
        public override void OnInspectorGUI( )
        {
            //GameObjectId self = target as GameObjectId;
            
            serializedObject.Update( );
            UnityEditor.EditorGUILayout.PropertyField( serializedObject.FindProperty( "m_Id" ), new GUIContent( "ID" ) );
            serializedObject.ApplyModifiedProperties( );
            
            if( EditorHelp.GroupStart( "GameObjectId", "リスト" ) )
            {
                KeyValuePair<string,List<GameObject>>[] pairs = GameObjectId.Pairs;
                if( pairs != null )
                {
                    for( int i = 0; i < pairs.Length; ++i )
                    {
                        UnityEditor.EditorGUILayout.BeginHorizontal( );
                        
                        UnityEditor.EditorGUILayout.LabelField( pairs[i].Key );
                        
                        UnityEditor.EditorGUILayout.BeginVertical( );
                        for( int j = 0; j < pairs[ i ].Value.Count; ++j )
                        {
                            UnityEditor.EditorGUILayout.ObjectField( pairs[ i ].Value[ j ], typeof( GameObject ), true );
                        }
                        UnityEditor.EditorGUILayout.EndVertical( );
                        
                        UnityEditor.EditorGUILayout.EndHorizontal( );
                    }
                }
            }
            EditorHelp.GroupEnd( );
        }
    }
    #endif
}
