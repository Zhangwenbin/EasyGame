/**************************************************************************/
/*! @file   EditorHelp.cs
    @brief  エディタヘルプ
***************************************************************************/
using UnityEngine;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using System;
using System.Text.RegularExpressions;
using System.Diagnostics.CodeAnalysis;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.ProjectWindowCallback;
using UnityEditor.SceneManagement;
#endif

namespace EG
{
    #if UNITY_EDITOR
    //=========================================================================
    //. EditorUtility
    //=========================================================================
    public static partial class EditorHelp
    {
        public static void SetTitle( this EditorWindow window, string title )
        {
            window.titleContent.text = title;
        }
        
        //=========================================================================
        //. アセット作成
        //=========================================================================
        #region アセット作成
        
        /// ***********************************************************************
        /// <summary>
        /// 新しいスクリプタルオブジェクトアセットを作成する
        /// </summary>
        /// ***********************************************************************
        public static T CreateScriptableObject<T>( System.Action<ScriptableObject,string,string> callback ) where T : ScriptableObject
        {
            T asset = ScriptableObject.CreateInstance<T>( );
            
            // インスタンスID
            int instanceID = asset.GetInstanceID( );
            
            //マテリアルのアイコンを取得
            Texture2D icon = AssetPreview.GetMiniThumbnail( asset );
            
            // 生成クラス
            if( callback != null )
            {
                DoCreateScriptableObject endNameEditAction = ScriptableObject.CreateInstance<DoCreateScriptableObject>();
                endNameEditAction.Callback = callback;
                ProjectWindowUtil.StartNameEditingIfProjectWindowExists( instanceID, endNameEditAction, "NewScriptableObject.asset", icon, "" );
            }
            return asset;
        }
        
        /// ***********************************************************************
        /// <summary>
        /// 中身書き換え
        /// </summary>
        /// ***********************************************************************
        public class DoCreateScriptableObject : EndNameEditAction
        {
            public System.Action<ScriptableObject,string,string> Callback;
            public override void Action( int instanceId, string pathName, string resourceFile )
            {
                ScriptableObject sobj = (ScriptableObject)EditorUtility.InstanceIDToObject( instanceId );
                if( sobj != null )
                {
                    AssetDatabase.CreateAsset( sobj, pathName );
                    AssetDatabase.ImportAsset( pathName );
                    var asset = AssetDatabase.LoadAssetAtPath<ScriptableObject>( pathName );
                    if( Callback != null )
                    {
                        Callback( asset, pathName, resourceFile );
                        AssetDatabase.SaveAssets( );
                    }
                    ProjectWindowUtil.ShowCreatedAsset( asset );
                }
            }
        }
        
        #endregion
        
        //=========================================================================
        //. スクリプタルオブジェクト
        //=========================================================================
        #region スクリプタルオブジェクト
        
        /// ***********************************************************************
        /// <summary>
        /// スクリプタルオブジェクトの生成
        /// </summary>
        /// ***********************************************************************
        public static T CreateAssetOfType<T>() where T : ScriptableObject
        {
            T asset = ScriptableObject.CreateInstance<T>( );
            
            string path = AssetDatabase.GetAssetPath( Selection.activeObject );
            
            if( path.Length <= 0 )
            {
                path = "Assets";
            }
            else if( Path.GetExtension( path ).Length > 0 )
            {
                path = path.Replace( Path.GetFileName( AssetDatabase.GetAssetPath( Selection.activeObject ) ), "" );
            }
            
            string assetPath = AssetDatabase.GenerateUniqueAssetPath( path + "/New " + typeof( T ).ToString( ) + ".asset" );
            
            AssetDatabase.CreateAsset( asset, assetPath );
            AssetDatabase.SaveAssets( );
            
            EditorUtility.FocusProjectWindow( );
            Selection.activeObject = asset;
            
            return asset;
        }
        
        #endregion
        
        //=========================================================================
        //. IO
        //=========================================================================
        #region IO
        
        /// ***********************************************************************
        /// <summary>
        /// Editor用のTempPath取得
        /// </summary>
        /// ***********************************************************************
        public static string GetTempPath( bool temporary = false )
        {
            string path = "/TempEditor";

            if (temporary)
            {
                // バンドルID単位で管理したい場合
                path = Application.temporaryCachePath + path;
            }
            else
            {
                // リポジトリ単位で管理したい場合
                path = Application.dataPath + "/.." + path;
            }

            return path;
        }

        /// ***********************************************************************
        /// <summary>
        /// 保存
        /// </summary>
        /// ***********************************************************************
        public static void ConfigSave<T>( string file, T value, bool temporary = false )
        {
            // 保存
            if( value != null )
            {
                // 保存
                string text = JsonUtility.ToJson( value, true );
                string path = GetTempPath(temporary) + "/" + file + ".txt";
                CreateDirectory( path );
                System.IO.File.WriteAllText( path, text, System.Text.Encoding.UTF8 );
            }
        }
        
        /// ***********************************************************************
        /// <summary>
        /// 読み込み
        /// </summary>
        /// ***********************************************************************
        public static T ConfigLoad<T>( string file, T defaultValue, bool temporary = false )
        {
            T result = defaultValue;
            string path = GetTempPath(temporary) + "/" + file + ".txt";
            if( System.IO.File.Exists( path ) )
            {
                string text = System.IO.File.ReadAllText( path, System.Text.Encoding.UTF8 );
                if( string.IsNullOrEmpty( text ) == false )
                {
                    result = JsonUtility.FromJson<T>( text );
                }
            }
            return result;
        }
        
        /// ***********************************************************************
        /// <summary>
        /// フォルダ生成
        /// </summary>
        /// ***********************************************************************
        public static void CreateDirectory( string path )
        {
            FileUtility.CreateDirectory( path );
        }
        
        

        public static string SaveFilePanel( string title, string folder, string defaultName, string extension )
        {
            folder = folder.Replace( "\\", "/" );
            folder = Utility.GetStringCutFront( folder, "Assets/", false );
            return EditorUtility.SaveFilePanel( title, folder, defaultName, extension );
        }
        

        public static string OpenFilePanel( string title, string folder, string extension )
        {
            folder = folder.Replace( "\\", "/" );
            folder = Utility.GetStringCutFront( folder, "Assets/", false );
            return EditorUtility.OpenFilePanel( title, folder, extension );
        }
        
        #endregion
        
        //=========================================================================
        //. プログレスバー
        //=========================================================================
        #region プログレスバー
        
        public static class AsyncProgressBar
        {
            private static MethodInfo m_Display = null;
            private static MethodInfo m_Clear   = null;
            
            static AsyncProgressBar( )
            {
                var type = typeof( Editor ).Assembly
                    .GetTypes()
                    .Where( c => c.Name == "AsyncProgressBar" )
                    .FirstOrDefault()
                ;
                
                m_Display = type.GetMethod( "Display" );
                m_Clear   = type.GetMethod( "Clear"   );
            }
            
            public static void Display( string progressInfo, float progress )
            {
                var parameters = new object[] { progressInfo, progress };
                m_Display.Invoke( null, parameters );
            }
            
            public static void Clear( )
            {
                m_Clear.Invoke( null, null );
            }
        }
        
        public static class ProgressBar
        {
            public static void Display( string progressInfo, float progress )
            {
                UnityEditor.EditorUtility.DisplayProgressBar( "", progressInfo, progress );
            }
            
            public static void Clear( )
            {
                UnityEditor.EditorUtility.ClearProgressBar( );
            }
        }
         
        #endregion
        
        //=========================================================================
        //. ヘルパー
        //=========================================================================
        #region ヘルパー
        
        /// ***********************************************************************
        /// <summary>
        /// SetDirty オーバーライド
        /// </summary>
        /// ***********************************************************************
        public static void SetDirty( UnityEngine.Object obj )
        {
            UnityEditor.EditorUtility.SetDirty( obj );
        }
        
        /// ***********************************************************************
        /// <summary>
        /// シリアライズプロパティのコピー( self -> prop へコピー )
        /// </summary>
        /// ***********************************************************************
        public static void GenericCopyTo( this SerializedProperty self, SerializedProperty prop, System.Type propType )
        {
            List<SerializedProperty> propList = new List<SerializedProperty>( ); 
            
            SerializedProperty child = self.Copy( );
            SerializedProperty end = self.GetEndProperty( true );
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
                    
                    propList.Add( child.Copy( ) );
                    
                    if( child.Next( false ) == false ) break;
                }
            }
            
            for( int i = 0; i < propList.Count; ++i )
            {
                SerializedProperty value1 = propList[ i ];
                SerializedProperty value2 = prop.FindPropertyRelative( value1.name );
                value1.CopyTo( value2, propType );
            }
        }
        
        /// ***********************************************************************
        /// <summary>
        /// シリアライズプロパティのコピー( self -> prop へコピー )
        /// </summary>
        /// ***********************************************************************
        public static void CopyTo( this SerializedProperty self, SerializedProperty prop, System.Type propType )
        {
            switch( prop.propertyType )
            {
            case SerializedPropertyType.AnimationCurve:
                prop.animationCurveValue = self.animationCurveValue;
                break;
            case SerializedPropertyType.ArraySize:
                prop.arraySize = self.arraySize;
                break;
            case SerializedPropertyType.Boolean:
                prop.boolValue = self.boolValue;
                break;
            case SerializedPropertyType.Bounds:
                prop.boundsValue = self.boundsValue;
                break;
            case SerializedPropertyType.BoundsInt:
                prop.boundsIntValue = self.boundsIntValue;
                break;
            case SerializedPropertyType.Character:
                prop.intValue = self.intValue;
                break;
            case SerializedPropertyType.Color:
                prop.colorValue = self.colorValue;
                break;
            case SerializedPropertyType.Enum:
                prop.enumValueIndex = self.enumValueIndex;
                break;
            case SerializedPropertyType.ExposedReference:
                prop.exposedReferenceValue = self.exposedReferenceValue;
                break;
            case SerializedPropertyType.FixedBufferSize:
                // 読み込み専用
                break;
            case SerializedPropertyType.Float:
                prop.floatValue = self.floatValue;
                break;
            case SerializedPropertyType.Generic:
                if( prop != null )
                {
                    self.GenericCopyTo( prop, propType );
                }
                break;
            case SerializedPropertyType.Gradient:
                {
                    BindingFlags instanceAnyPrivacyBindingFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;
                    PropertyInfo propertyInfo = typeof( SerializedProperty ).GetProperty( "gradientValue", instanceAnyPrivacyBindingFlags, null, typeof( Gradient ), new System.Type[0], null );
                    if( propertyInfo == null ) return;
                    propertyInfo.SetValue( prop, propertyInfo.GetValue( self, null ), null );
                }
                break;
            case SerializedPropertyType.Integer:
                prop.intValue = self.intValue;
                break;
            case SerializedPropertyType.LayerMask:
                prop.intValue = self.intValue;
                break;
            case SerializedPropertyType.ObjectReference:
                prop.objectReferenceValue = self.objectReferenceValue;
                break;
            case SerializedPropertyType.Quaternion:
                prop.quaternionValue = self.quaternionValue;
                break;
            case SerializedPropertyType.Rect:
                prop.rectValue = self.rectValue;
                break;
            case SerializedPropertyType.RectInt:
                prop.rectIntValue = self.rectIntValue;
                break;
            case SerializedPropertyType.String:
                prop.stringValue = self.stringValue;
                break;
            case SerializedPropertyType.Vector2:
                prop.vector2Value = self.vector2Value;
                break;
            case SerializedPropertyType.Vector2Int:
                prop.vector2IntValue = self.vector2IntValue;
                break;
            case SerializedPropertyType.Vector3:
                prop.vector3Value = self.vector3Value;
                break;
            case SerializedPropertyType.Vector3Int:
                prop.vector3IntValue = self.vector3IntValue;
                break;
            case SerializedPropertyType.Vector4:
                prop.vector4Value = self.vector4Value;
                break;
            }
        }
        
        #endregion
        
        //=========================================================================
        //. ビューポートに表示する警告
        //=========================================================================
        #region ビューポートに表示する警告
         
        public class ViewportWarnings
        {
            static List<string> mLines = new List<string>();
            
            public static string[] GetWarnings( )
            {
                mLines.Clear( );
                
                Transform[] transforms = Component.FindObjectsOfType<Transform>( );
                for( int i = transforms.Length - 1; i >= 0; --i )
                {
                    if( transforms[ i ].parent == null )
                    {
                        transforms[ i ].BroadcastMessage( "DisplayWarnings", SendMessageOptions.DontRequireReceiver );
                    }
                }
                
                return mLines.ToArray( );
            }
            
            public static void Add( GameObject src, string text )
            {
                mLines.Add( "[" + src.name + "] " + text );
            }
        }
        
        #endregion


        public static List<string> GetHierarchyPath(Transform t, List<string> hierarchy = null)
        {
            if (hierarchy != null) hierarchy.Clear();
            hierarchy = hierarchy ?? new List<string>();
            while (t != null)
            {
                hierarchy.Insert(0, t.name);
                t = t.parent;
            }
            if (hierarchy.Count > 0 && hierarchy[0] == "Canvas (Environment)")
            {
                // prefabStage root
                hierarchy.RemoveAt(0);
            }
            return hierarchy;
        }

        static Dictionary<string, int> hashCache;

        public static int ComputeKeysHash(List<string> keys)
        {
            if (keys == null) throw new ArgumentNullException(nameof(keys));

            var sb = Utility.GetStringBuilder();
            foreach (var key in keys)
            {
                sb.Append(key);
                sb.Append(';');
            }
            return sb.ComputeFarmHash32();
        }

        public static bool DumpStringKeysClassFile(string path, string @namespace, string className, List<string> keys, Func<List<string>> getComments = null, Func<List<string>> getHeader = null)
        {
            if (keys == null) throw new ArgumentNullException(nameof(keys));

            var hash = ComputeKeysHash(keys);

            var sb = Utility.GetStringBuilder();
            if (File.Exists(path))
            {
                var beforeHash = 0;
                if (hashCache?.TryGetValue(path, out beforeHash) ?? false)
                {
                    if (hash == beforeHash)
                    {
                        return false;
                    }
                }

                hashCache = hashCache ?? new Dictionary<string, int>();

                // hash:xxxxxxxxxxxx
                var beforeHashString = FileUtility.ReadSingleLine(path, 1).SplitBy(':')[1].Trim();
                if (int.TryParse(beforeHashString, out beforeHash) && hash == beforeHash)
                if (hash == beforeHash)
                {
                    hashCache[path] = beforeHash;
                    return false;
                }
                if (uint.TryParse(beforeHashString, System.Globalization.NumberStyles.HexNumber, null, out var beforeHashU) && (uint)hash == beforeHashU)
                {
                    hashCache[path] = (int)beforeHashU;
                    return false;
                }
            }

            var header = getHeader?.Invoke();
            var comments = getComments?.Invoke();

            sb.Length = 0;
            sb.Append("// AUTOGENERATED\n");
            sb.Append("// hash:").Append(hash.ToString()).Append('\n');
            if (header != null)
            {
                foreach (var h in header)
                {
                    sb.Append("// ").Append(h).Append('\n');
                }
            }
            sb.Append("#pragma warning disable IDE1006\n");
            sb.Append("namespace ").Append(@namespace).Append('\n');
            sb.Append("{\n");
            sb.Append("    public static class ").Append(ToSafeSymbolName(className)).Append('\n');
            sb.Append("    {\n");
            for (var i = 0; i < keys.Count; ++i)
            {
                var key = keys[i];
                if (i > 0) sb.Append('\n');

                if (comments != null && i < comments.Count)
                {
                    var comment = comments[i];
                    if (!string.IsNullOrEmpty(comment))
                    {
                        sb.Append("        // ").Append(comment).Append('\n');
                    }
                }

                sb.Append("        public const string ").Append(ToSafeSymbolName(key));
                sb.Append(" = \"").Append(key).Append("\";\n");
            }
            sb.Append("    }\n");
            sb.Append("}\n");

            FileUtility.MakeDirectory(path);
            File.WriteAllText(path, sb.ToString(), System.Text.Encoding.UTF8);

            hashCache = hashCache ?? new Dictionary<string, int>();
            hashCache[path] = hash;

            return true;
        }

        public static string ToSafeSymbolName(string name)
        {
            if (string.IsNullOrEmpty(name)) return "_";

            name = Regex.Replace(name, "[^a-zA-Z0-9]", "_");
            if (Regex.IsMatch(name, "^[0-9]")) name = "_" + name;
            return EscapeKeyword(name);
        }

        // https://msdn.microsoft.com/en-us/library/x53a06bb.aspx?f=255&MSPPError=-2147217396
        public static string EscapeKeyword(string token)
        {
            return Regex.Replace(token, "^(abstract|as|base|bool|break|byte|case|catch|char|checked|class|const|continue|decimal|default|delegate|do|double|else|enum|event|explicit|extern|false|finally|fixed| float |for|foreach|goto|if|implicit|in|int|interface|internal|is|lock|long|namespace|new|null|object|operator|out|override|params|private|protected|public|readonly|ref|return|sbyte|sealed|short|sizeof|stackalloc|static|string|struct|switch|this|throw|true|try|typeof|uint|ulong|unchecked|unsafe|ushort|using|virtual|void|volatile|while)$", "@$1");
        }

        public static long GetLocalID(GameObject go)
        {
            if (go == null)
            {
                return -1;
            }

            string guid;
            long localId;

            if (EditorUtility.IsPersistent(go) && go.transform.parent == null)
            {
                foreach (var o in AssetDatabase.LoadAllAssetsAtPath(AssetDatabase.GetAssetPath(go)))
                {
                    if (AssetDatabase.TryGetGUIDAndLocalFileIdentifier(o, out guid, out localId))
                    {
                        return localId;
                    }
                }
                return -1;
            }

            // prefab?
            string path = null;
            GameObject root = null;
            var stage = UnityEditor.Experimental.SceneManagement.PrefabStageUtility.GetPrefabStage(go);

            if (stage != null)
            {
                // prefabStage(prefabダブルクリックで開くやつ)上のインスタンス
                path = stage.prefabAssetPath;
                root = stage.prefabContentsRoot;
            }
            else if (PrefabUtility.IsPartOfPrefabAsset(go))
            {
                // AssetDatabase.LoadAssetAtPath()等で取得されたprefabアセット
                path = AssetDatabase.GetAssetPath(go.GetInstanceID());
                root = AssetDatabase.LoadAssetAtPath<GameObject>(path);
            }
            else if (PrefabUtility.IsPartOfPrefabInstance(go))
            {
                // sceneに置かれたprefabのインスタンス
                path = PrefabUtility.GetPrefabAssetPathOfNearestInstanceRoot(go);
                root = PrefabUtility.GetNearestPrefabInstanceRoot(go);
            }
            else if (EditorSceneManager.IsPreviewSceneObject(go.transform.root.gameObject))
            {
                // PrefabUtility.LoadPrefabContents()で開いたインスタンス
                path = go.scene.path;
                root = go.transform.root.gameObject;
            }

            var hierarchy = new List<string>();
            var t = go.transform;
            while (t != null && t != root?.transform)
            {
                hierarchy.Insert(0, t.name);
                t = t.parent;
            }

            if (!string.IsNullOrEmpty(path) && root != null)
            {
                var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
                if (hierarchy.Count == 0)
                {
                    if (AssetDatabase.TryGetGUIDAndLocalFileIdentifier(prefab, out guid, out localId))
                    {
                        return localId;
                    }
                    return -1;
                }

                // TryGetGUIDAndLocalFileIdentifierは常にrootのLocalIDを返すので,
                // 深いオブジェクトのLocalIDが欲しい時は自力でもぐってリフレクションで取得する
                // Unsupported.GetLocalIdentifierInFile()はint32を返すので下位32bitしか取得できない.
                t = prefab.transform;
                foreach (var h in hierarchy)
                {
                    t = t.Find(h);
                    if (t == null) break;
                }
                if (t != null)
                {
                    var inspectorModeInfo = typeof(SerializedObject).GetProperty("inspectorMode", BindingFlags.NonPublic | BindingFlags.Instance);
                    if (inspectorModeInfo != null)
                    {
                        // so.inspectorMode = InspectorMode.Debug;  // をReflectionで
                        var so = new SerializedObject(t.gameObject);
                        inspectorModeInfo.SetValue(so, InspectorMode.Debug, null);
                        // cspell:ignore Identfier
                        var prop = so.FindProperty("m_LocalIdentfierInFile");
                        return prop.longValue;
                    }
                }
            }

            return -1;
        }

        public static string GetMonoScriptPath(MonoBehaviour o)
        {
            if (o == null)
            {
                return "";
            }

            var serialized = new SerializedObject(o);
            var mono = serialized.FindProperty("m_Script").objectReferenceValue;
            if (mono == null)
            {
                return "";
            }

            return AssetDatabase.GetAssetPath(mono);
        }

        public static void OpenMonoScriptInEditor(MonoBehaviour o)
        {
            if (o == null)
            {
                return;
            }

            var path = GetMonoScriptPath(o);
            if (string.IsNullOrEmpty(path))
            {
                return;
            }

            EditorGUIUtility.PingObject(AssetDatabase.LoadMainAssetAtPath(path));

            var line = FindLineInTextFile(path, " class +" + (o?.GetType().Name ?? ""));
            UnityEditorInternal.InternalEditorUtility.OpenFileAtLineExternal(path, line);
        }

        public static void OpenEventDefinitionInEditor(UnityEngine.Events.UnityEventBase ev)
        {
            var (path, line) = GetEventDefinitionLine(ev);
            if (string.IsNullOrEmpty(path))
            {
                return;
            }

            EditorGUIUtility.PingObject(AssetDatabase.LoadMainAssetAtPath(path));
            UnityEditorInternal.InternalEditorUtility.OpenFileAtLineExternal(path, line);
        }

        public static (string, int) GetEventDefinitionLine(UnityEngine.Events.UnityEventBase ev)
        {
            if (ev.GetPersistentEventCount() == 0)
            {
                return ("", -1);
            }

            var target = ev.GetPersistentTarget(0) as MonoBehaviour;
            var methodName = ev.GetPersistentMethodName(0);
            if (target == null)
            {
                return ("", -1);
            }

            var path = GetMonoScriptPath(target);
            var classStart = FindLineInTextFile(path, " class +" + (target?.GetType().Name ?? ""));
            var line = FindLineInTextFile(path, " " + methodName + "\\s*\\(", classStart);

            return (path, line);
        }

        public static int FindLineInTextFile(string path, string regexPattern, int start = 1)
        {
            if (string.IsNullOrEmpty(path) || !File.Exists(path))
            {
                return -1;
            }

            Regex regex;
            try
            {
                regex = new Regex(regexPattern);
            }
            catch (ArgumentException)
            {
                return -1;
            }

            using (var s = new StreamReader(path))
            {
                var line = 0;
                while (!s.EndOfStream)
                {
                    ++line;
                    var code = s.ReadLine();
                    if (line < start)
                    {
                        continue;
                    }
                    if (regex.IsMatch(code))
                    {
                        return line;
                    }
                }
            }

            return -1;
        }

        /// 
        public static void ValidateNullableSerializeFields(MonoBehaviour behaviour)
        {
            var type = behaviour.GetType();
            var sb = FastStringBuilder.Alloc();
            foreach (var field in ReflectionUtility.GetFields(type))
            {
                if (field.FieldType.IsValueType)
                    continue;

                var serializable = ReflectionUtility.GetCustomAttribute<SerializeField>(field);
                if (serializable == null && !field.IsPublic)
                    continue;

                var value = field.GetValue(behaviour);
                if (value == null || (value is UnityEngine.Object uo && !uo))
                {
                    var go = behaviour.gameObject;
                    var hierarchy = go ? AnimationUtility.CalculateTransformPath(go.transform, go.transform.root) : "";
                    var msg = $"null非許容のフィールド[{field.Name}]の値がnullの状態です <{type.Name}> ({hierarchy})";
                    Debug.LogError(msg, behaviour);
                    sb.Append(msg);
                    sb.Append('\n');
                }
            }

            if (sb.Length > 0)
            {
                EditorUtility.DisplayDialog("null非許容", sb.ToString(), "Ok", "");
            }
            FastStringBuilder.Free(sb);
        }

        /// <summary>string一覧</summary>
        public static string DrawPopupStringField(string label, string value, string[] options, bool keepValueOption = true)
        {
            var index = Array.IndexOf(options, value);
            if (index == -1 && keepValueOption)
            {
                var temp = new string[options.Length + 1];
                index = 0;
                temp[0] = value;
                for (var i = 0; i < options.Length; ++i)
                {
                    temp[i + 1] = options[i];
                }
                options = temp;
            }

            var nextIndex = EditorGUILayout.Popup(label, index, options, Array.Empty<GUILayoutOption>());
            if (index == nextIndex)
                return value;

            if ((uint)nextIndex >= (uint)options.Length)
                return "";

            return options[nextIndex];
        }
    }
    
    #endif
}
