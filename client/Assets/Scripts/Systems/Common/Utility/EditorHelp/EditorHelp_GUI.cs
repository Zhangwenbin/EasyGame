
using UnityEngine;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.ProjectWindowCallback;
#endif

namespace EG
{
    #if UNITY_EDITOR
    
    public static partial class EditorHelp
    {
        public static class GUIStyles
        {
            static GUIStyle g_GroupTitleMC      = null;
            static GUIStyle g_GroupTitleML      = null;
            static GUIStyle g_ColorLabel        = null;
            static GUIStyle g_ButtonToggle      = null;
            static GUIStyle g_ButtonLabelML     = null;
            static GUIStyle g_IndexStyle        = null;
            
            public static void GetGroupTitle( ref GUIStyle style, TextAnchor anchor )
            {
                if( style == null )
                {
                    style = new GUIStyle( "OL SelectedRow");
                    style.alignment = anchor;
                    style.border = new RectOffset( 4, 4, 4, 4 );
                    style.padding = new RectOffset( 2, 2, 2, 2 );
                    style.fontStyle = FontStyle.Normal;
                    style.normal = new GUIStyleState( );
                    style.normal.textColor = Color.white;
                    style.normal.background = Texture2D.whiteTexture;
                    style.active = new GUIStyleState( );
                    style.active.textColor = Color.white;
                    style.active.background = Texture2D.whiteTexture;
                }
            }
            
            // 
            public static GUIStyle GetGroupTitleMiddleCenter( Color color )
            {
                GUI.backgroundColor = color;
                if( g_GroupTitleMC == null )
                {
                    GetGroupTitle( ref g_GroupTitleMC, TextAnchor.MiddleCenter );
                }
                return g_GroupTitleMC;
            }
            public static GUIStyle GetGroupTitleMiddleLeft( Color color )
            {
                GUI.backgroundColor = color;
                if( g_GroupTitleML == null )
                {
                    GetGroupTitle( ref g_GroupTitleML, TextAnchor.MiddleLeft );
                }
                return g_GroupTitleML;
            }
            public static GUIStyle GroupTitleCenter     { get { return GetGroupTitleMiddleCenter( new Color( 0, 0.6f, 0, 1 ) ); } }
            public static GUIStyle GroupTitleLeft       { get { return GetGroupTitleMiddleLeft( new Color( 0, 0.6f, 0, 1 ) );   } }
            public static GUIStyle GroupSubTitleCenter  { get { return GetGroupTitleMiddleCenter( new Color( 0.1f, 0.3f, 0.5f, 1.0f ) ); } }
            public static GUIStyle GroupSubTitleLeft    { get { return GetGroupTitleMiddleLeft( new Color( 0.1f, 0.3f, 0.5f, 1.0f ) );   } }
            
            // 
            public static GUIStyle GetColorLabel( )
            {
                if( g_ColorLabel == null )
                {
                    g_ColorLabel = new GUIStyle( GUI.skin.GetStyle( "label" ) );
                    g_ColorLabel.normal = new GUIStyleState( );
                    g_ColorLabel.normal.textColor = Color.white;
                    g_ColorLabel.normal.background = Texture2D.whiteTexture;
                }
                return g_ColorLabel;
            }
            
            // 
            public static GUIStyle ButtonToggle
            {
                get
                {
                    if( g_ButtonToggle == null )
                    {
                        g_ButtonToggle = new GUIStyle( GUI.skin.GetStyle( "button" ) );
                        g_ButtonToggle.alignment = TextAnchor.MiddleLeft;
                        g_ButtonToggle.normal = new GUIStyleState( );
                        g_ButtonToggle.normal.textColor = Color.white;
                        g_ButtonToggle.normal.background = Texture2D.whiteTexture;
                        g_ButtonToggle.active = new GUIStyleState( );
                        g_ButtonToggle.active.textColor = Color.white;
                        g_ButtonToggle.active.background = Texture2D.whiteTexture;
                    }
                    return g_ButtonToggle;
                }
            }
            
            // 
            public static GUIStyle IndexStyle
            {
                get
                {
                    if( g_IndexStyle == null )
                    {
                        g_IndexStyle = new GUIStyle( GUI.skin.GetStyle( "sv_label_1" ) );
                        g_IndexStyle.normal.textColor = Color.white;
                        g_IndexStyle.margin = new RectOffset( 0, 0, 3, 1 );
                        g_IndexStyle.padding = new RectOffset( 0, 0, 2, 2 );
                    }
                    return g_IndexStyle;
                }
            }
            
            // 
            public static GUIStyle LabelStyle
            {
                get
                {
                    if( g_ButtonLabelML == null )
                    {
                        g_ButtonLabelML = new GUIStyle( GUI.skin.GetStyle( "button" ) );
                        
                        g_ButtonLabelML.normal = new GUIStyleState( );
                        g_ButtonLabelML.normal.textColor = new Color( 192f/255f, 192f/255f, 192f/255f, 1.0f );
                        g_ButtonLabelML.normal.background = new Texture2D( 1, 1 );
                        g_ButtonLabelML.normal.background.SetPixel( 0, 0, new Color( 96f/255f, 96f/255f, 96f/255f, 1.0f ) );
                        g_ButtonLabelML.normal.background.Apply( );
                        
                        //g_ButtonLabel.hover = new GUIStyleState( );
                        //g_ButtonLabel.hover.textColor = new Color( 32f/255f, 32f/255f, 32f/255f, 1.0f );
                        //g_ButtonLabel.hover.background = new Texture2D( 1, 1 );
                        //g_ButtonLabel.hover.background.SetPixel( 0, 0, new Color( 153f/255f, 217f/255f, 234f/255f, 1.0f ) );
                        //g_ButtonLabel.hover.background.Apply( );
                        
                        g_ButtonLabelML.active = new GUIStyleState( );
                        g_ButtonLabelML.active.textColor = Color.red;
                        g_ButtonLabelML.active.background = new Texture2D( 1, 1 );
                        g_ButtonLabelML.active.background.SetPixel( 0, 0, new Color( 153f/255f, 217f/255f, 234f/255f, 1.0f ) );
                        g_ButtonLabelML.active.background.Apply( );
                        
                        g_ButtonLabelML.alignment = TextAnchor.MiddleLeft;
                        g_ButtonLabelML.margin.top = 0;
                        g_ButtonLabelML.margin.bottom = 0;
                    }
                    return g_ButtonLabelML;
                }
            }
        }
        
        #region 

        public static void SetClipCopy( string text )
        {
            GUIUtility.systemCopyBuffer = text;
        }
        public static string GetClipCopy( )
        {
            return GUIUtility.systemCopyBuffer;
        }
        
 
        public static void Undo( UnityEngine.Object obj )
        {
            if( obj == null ) return;
            UnityEditor.Undo.RecordObjects( new UnityEngine.Object[]{ obj }, string.Format( "{0}_{1}", obj.name, obj.GetInstanceID( ) ) );
        }
        
        #endregion 
        

        #region 
        
        // 
        static List<Color> m_ContentColorStack = new List<Color>();
        static List<Color> m_BGColorStack = new List<Color>();
        static List<Color> m_FGColorStack = new List<Color>();
        
 
        public static void PushContentColor( )
        {
            m_ContentColorStack.Add( GUI.contentColor );
        }
        public static void PushContentColor( Color color )
        {
            PushContentColor( );
            GUI.contentColor = color;
        }
        
        public static void PopContentColor( )
        {
            GUI.contentColor = m_ContentColorStack[ m_ContentColorStack.Count - 1 ];
            m_ContentColorStack.RemoveAt( m_ContentColorStack.Count - 1 );
        }
        

        public static void PushBackgroundColor( )
        {
            m_BGColorStack.Add( GUI.backgroundColor );
        }
        public static void PushBackgroundColor( Color color )
        {
            PushBackgroundColor( );
            GUI.backgroundColor = color;
        }
        

        public static void PopBackgroundColor()
        {
            GUI.backgroundColor = m_BGColorStack[m_BGColorStack.Count - 1];
            m_BGColorStack.RemoveAt(m_BGColorStack.Count - 1);
        }
        

        public static void PushForegroundColor()
        {
            m_FGColorStack.Add( GUI.color );
        }
        

        public static void PopForegroundColor()
        {
            GUI.color = m_FGColorStack[m_FGColorStack.Count - 1];
            m_FGColorStack.RemoveAt(m_FGColorStack.Count - 1);
        }
        
        #endregion 
        

        #region 
        

        public static void ClearKeyControll( )
        {
            GUI.FocusControl( "" );
            GUIUtility.hotControl = 0;
            GUIUtility.keyboardControl = 0;
        }
        
        /// ***********************************************************************
        /// ***********************************************************************
        public static bool ChangeKeyControllValue( string controlName, ref int value, int min, int max )
        {
            if( GUI.GetNameOfFocusedControl( ) == controlName )
            {
                Event ev = Event.current;
                if( ev != null && ev.isKey )
                {
                    switch( ev.type )
                    {
                    case EventType.KeyDown:
                        if( ev.keyCode == KeyCode.UpArrow )
                        {
                            -- value;
                            if( value < min ) value = max - 1;
                            return true;
                        }
                        else if( ev.keyCode == KeyCode.DownArrow )
                        {
                            ++ value;
                            if( value >= max ) value = min;
                            return true;
                        }
                        break;
                    }
                }
            }
            return false;
        }
        
        #endregion 
        

        #region 
        

        public static void Title( string title )
        {
            GUI.contentColor = new Color( 0.9f, 0.9f, 0.9f );
            GUILayout.Label( title, EditorHelp.GUIStyles.GroupTitleLeft );
            GUI.contentColor = Color.white;
            GUI.backgroundColor = Color.white;
            GUI.color = Color.white;
        }
        public static void TitleCenter( string title )
        {
            GUI.contentColor =  new Color( 0.9f, 0.9f, 0.9f );
            GUILayout.Label( title, EditorHelp.GUIStyles.GroupTitleCenter );
            GUI.contentColor = Color.white;
            GUI.backgroundColor = Color.white;
            GUI.color = Color.white;
        }
        

        public static bool SubTitle( ref bool foldout, string title )
        {
            GUI.contentColor = foldout ? new Color( 0.9f, 0.9f, 0.9f ): new Color( 0.7f, 0.7f, 0.7f );
            if( GUILayout.Button( (foldout ? "▼ ": "△ ") + title, EditorHelp.GUIStyles.GroupSubTitleLeft ) ) foldout = !foldout;
            GUI.contentColor = Color.white;
            GUI.backgroundColor = Color.white;
            GUI.color = Color.white;
            return foldout;
        }
        public static bool SubTitle( string key, string title )
        {
            bool foldout = EditorPrefs.GetBool( key );
            bool bakFoldout = foldout;
            SubTitle( ref foldout, title );
            if( foldout != bakFoldout ) EditorPrefs.SetBool( key, foldout );
            return foldout;
        }
        public static void SubTitle( string title )
        {
            GUI.contentColor = new Color( 0.9f, 0.9f, 0.9f );
            GUILayout.Label( title, EditorHelp.GUIStyles.GroupSubTitleLeft );
            GUI.contentColor = Color.white;
            GUI.backgroundColor = Color.white;
            GUI.color = Color.white;
        }
        public static void SubTitleCenter( string title )
        {
            GUI.contentColor = new Color( 0.9f, 0.9f, 0.9f );
            GUILayout.Label( title, EditorHelp.GUIStyles.GroupSubTitleCenter );
            GUI.contentColor = Color.white;
            GUI.backgroundColor = Color.white;
            GUI.color = Color.white;
        }
        

        public static bool GroupStart( ref bool foldout, string title, float width=0 )
        {
            UnityEditor.EditorGUILayout.BeginVertical( "box" );
            GUI.contentColor = foldout ? new Color( 0.9f, 0.9f, 0.9f ): new Color( 0.7f, 0.7f, 0.7f );
            GUILayoutOption option = width > 0 ? GUILayout.Width( width ): null;
            if( option == null )
            {
                if( GUILayout.Button( (foldout ? "▼ ": "△ ") + title, EditorHelp.GUIStyles.GroupTitleLeft ) ) foldout = !foldout;
            }
            else
            {
                if( GUILayout.Button( (foldout ? "▼ ": "△ ") + title, EditorHelp.GUIStyles.GroupTitleLeft, option ) ) foldout = !foldout;
            }
            GUI.contentColor = Color.white;
            GUI.backgroundColor = Color.white;
            GUI.color = Color.white;
            return foldout;
        }
        public static bool GroupStart( string key, string title, float width=0 )
        {
            bool foldout = EditorPrefs.GetBool( key );
            bool bakFoldout = foldout;
            GroupStart( ref foldout, title, width );
            if( foldout != bakFoldout ) EditorPrefs.SetBool( key, foldout );
            return foldout;
        }
        public static void GroupStart( string title, float width=0 )
        {
            UnityEditor.EditorGUILayout.BeginVertical( "box" );
            GUI.contentColor = new Color( 0.9f, 0.9f, 0.9f );
            GUILayoutOption option = width > 0 ? GUILayout.Width( width ): null;
            if( option == null ) GUILayout.Label( title, EditorHelp.GUIStyles.GroupTitleLeft );
            else                 GUILayout.Label( title, EditorHelp.GUIStyles.GroupTitleLeft, option );
            GUI.contentColor = Color.white;
            GUI.backgroundColor = Color.white;
            GUI.color = Color.white;
        }
        

        public static void GroupEnd( )
        {
            UnityEditor.EditorGUILayout.EndVertical( );
        }
        
        /// ***********************************************************************
        public static void HeadStart( string title, Color color )
        {
            GUI.contentColor = color;
            string text = "<b><size=11>" + title + "</size></b>";
            GUILayout.Label( text, "dragtab", GUILayout.MinWidth( 20f ) );
            //if( !GUILayout.Toggle( true, text, "dragtab", GUILayout.MinWidth( 20f ) ) ) foldout = !foldout;
            GUI.contentColor = Color.white;
            GUI.backgroundColor = Color.white;
            GUILayout.BeginHorizontal();
            UnityEditor.EditorGUILayout.BeginHorizontal("CN box", GUILayout.MinHeight(10f));
            GUILayout.BeginVertical();
            GUILayout.Space(2f);
        }
        public static void HeadStart( string title )
        {
            HeadStart( title, new Color( 0.35f, 0.75f, 1.0f ) );
        }
        
        public static bool HeadStart( ref bool foldout, string title )
        {
            Color backColor = Color.white;
            if( foldout )
            {
                GUI.contentColor = new Color32( 48, 48, 48, 255 );
                backColor = new Color32( 200, 170, 0, 255 );
            }
            else
            {
                GUI.contentColor = new Color32( 64, 64, 64, 255 );
                backColor = new Color32( 150, 115, 0, 255 );
            }
            //string          text = "<b><size=11>" + title + "</size></b>";
            string          text = title;
            if( foldout )   text = "\u25BC " + text;
            else            text = "\u25BA " + text;
            if( !GUILayout.Toggle( true, text, EditorHelp.GUIStyles.GetGroupTitleMiddleLeft( backColor ), GUILayout.MinWidth( 20f ) ) ) foldout = !foldout;
            //if( !GUILayout.Toggle( true, text, "dragtab", GUILayout.MinWidth( 20f ) ) ) foldout = !foldout;
            GUI.color = Color.white;
            GUI.contentColor = Color.white;
            GUI.backgroundColor = Color.white;
            GUILayout.BeginHorizontal();
            UnityEditor.EditorGUILayout.BeginHorizontal("CN box",GUILayout.MinHeight(15f));
            GUILayout.BeginVertical();
            GUILayout.Space(2f);
            return foldout;
        }
        public static bool HeadStart( string foldoutKey, string title )
        {
            bool isOldFoldout = EditorPrefs.GetBool( foldoutKey );
            bool isFoldout = isOldFoldout;
            HeadStart( ref isFoldout, title );
            if( isFoldout != isOldFoldout )
            {
                EditorPrefs.SetBool( foldoutKey, isFoldout );
            }
            return isFoldout;
        }
        

        public static void HeadEnd( )
        {
            GUILayout.Space(3f);
            GUILayout.EndVertical( );
            UnityEditor.EditorGUILayout.EndHorizontal();
            GUILayout.Space(3f);
            GUILayout.EndHorizontal();
            GUILayout.Space(3f);
        }
        
        #endregion 
        

        #region 
        

        public static bool Toggle( UnityEngine.Object obj, string label, bool value, params GUILayoutOption[] options )
        {
            bool nextValue = EditorGUILayout.Toggle( label, value, options );
            if( nextValue != value )
            {
                Undo( obj );
            }
            return nextValue;
        }
        

        public static bool ToggleLeft( UnityEngine.Object obj, string label, bool value, params GUILayoutOption[] options )
        {
            bool nextValue = EditorGUILayout.ToggleLeft( label, value, options );
            if( nextValue != value )
            {
                Undo( obj );
            }
            return nextValue;
        }
        

        public static int IntField( UnityEngine.Object obj, string label, int value, params GUILayoutOption[] options )
        {
            int nextValue = EditorGUILayout.IntField( label, value, options );
            if( nextValue != value )
            {
                Undo( obj );
            }
            return nextValue;
        }
        

        public static float FloatField( UnityEngine.Object obj, string label, float value, params GUILayoutOption[] options )
        {
            float nextValue = EditorGUILayout.FloatField( label, value, options );
            if( nextValue != value )
            {
                Undo( obj );
            }
            return nextValue;
        }
        

        public static string TextField( UnityEngine.Object obj, string label, string value, params GUILayoutOption[] options )
        {
            string nextValue = EditorGUILayout.TextField( label, value, options );
            if( nextValue != value )
            {
                Undo( obj );
            }
            return nextValue;
        }
        

        public static Vector2 Vector2Field( UnityEngine.Object obj, string label, Vector2 value, params GUILayoutOption[] options )
        {
            Vector2 nextValue = EditorGUILayout.Vector2Field( label, value, options ); 
            if( nextValue != value )
            {
                Undo( obj );
            }
            return nextValue;
        }
        

        public static Vector3 Vector3Field( UnityEngine.Object obj, string label, Vector3 value, params GUILayoutOption[] options )
        {
            Vector3 nextValue = EditorGUILayout.Vector3Field( label, value, options ); 
            if( nextValue != value )
            {
                Undo( obj );
            }
            return nextValue;
        }

        //SG: added for Global
        public static Vector4 Vector4Field( UnityEngine.Object obj, string label, Vector4 value, params GUILayoutOption[] options )
        {
            Vector4 nextValue = EditorGUILayout.Vector4Field( label, value, options ); 
            if( nextValue != value )
            {
                Undo( obj );
            }
            return nextValue;
        }
        //End SG
        
        public static Vector2Int Vector2IntField( UnityEngine.Object obj, string label, Vector2Int value, params GUILayoutOption[] options )
        {
            Vector2Int nextValue = EditorGUILayout.Vector2IntField( label, value, options ); 
            if( nextValue != value )
            {
                Undo( obj );
            }
            return nextValue;
        }
        

        public static Vector3Int Vector3IntField( UnityEngine.Object obj, string label, Vector3Int value, params GUILayoutOption[] options )
        {
            Vector3Int nextValue = EditorGUILayout.Vector3IntField( label, value, options ); 
            if( nextValue != value )
            {
                Undo( obj );
            }
            return nextValue;
        }
        

        public static Color ColorField( UnityEngine.Object obj, string label, Color value, params GUILayoutOption[] options )
        {
            Color nextValue = EditorGUILayout.ColorField( label, value, options ); 
            if( nextValue != value )
            {
                Undo( obj );
            }
            return nextValue;
        }
        

        public static AnimationCurve CurveField( UnityEngine.Object obj, string label, AnimationCurve value, params GUILayoutOption[] options )
        {
            AnimationCurve nextValue = EditorGUILayout.CurveField( label, value, options ); 
            if( nextValue != value )
            {
                Undo( obj );
            }
            return nextValue;
        }
        
 
        public static Object ObjectField( UnityEngine.Object obj, string label, Object value, System.Type objType, bool allowSceneObjects, params GUILayoutOption[] options )
        {
            Object nextValue = EditorGUILayout.ObjectField( label, value, objType, allowSceneObjects, options );
            if( nextValue != value )
            {
                Undo( obj );
            }
            return nextValue;
        }
        

        public static int Popup( UnityEngine.Object obj, string label, int value, string[] displayNames, params GUILayoutOption[] options )
        {
            int nextValue = EditorGUILayout.Popup( label, value, displayNames, options ); 
            if( nextValue != value )
            {
                Undo( obj );
            }
            return nextValue;
        }
        public static int Popup( UnityEngine.Object obj, int value, string[] displayNames, params GUILayoutOption[] options )
        {
            int nextValue = EditorGUILayout.Popup( value, displayNames, options ); 
            if( nextValue != value )
            {
                Undo( obj );
            }
            return nextValue;
        }
        

        public static System.Enum EnumPopup( UnityEngine.Object obj, string label, System.Enum value, params GUILayoutOption[] options )
        {
            System.Enum nextValue = EditorGUILayout.EnumPopup( label, value, options ); 
            if( nextValue != value )
            {
                Undo( obj );
            }
            return nextValue;
        }
        

        public static int MaskField( UnityEngine.Object obj, string label, int value, string[] displayNames, params GUILayoutOption[] options )
        {
            int nextValue = EditorGUILayout.MaskField( label, value, displayNames, options ); 
            if( nextValue != value )
            {
                Undo( obj );
            }
            return nextValue;
        }
        

        public static int IntSlider( UnityEngine.Object obj, string label, int value, int min, int max, params GUILayoutOption[] options )
        {
            int nextValue = EditorGUILayout.IntSlider( label, value, min, max, options );
            if( nextValue != value )
            {
                Undo( obj );
            }
            return nextValue;
        }
        

        public static float FloatSlider( UnityEngine.Object obj, string label, float value, float min, float max, params GUILayoutOption[] options )
        {
            float nextValue = EditorGUILayout.Slider( label, value, min, max, options );
            if( nextValue != value )
            {
                Undo( obj );
            }
            return nextValue;
        }
        

        public static void LabelField( UnityEngine.Object obj, string label, string value, params GUILayoutOption[] options )
        {
            EditorGUILayout.LabelField( label, value, options );
        }
        
        // todo  SerializeValueBehaviour还没有
        // public static string TargetField( GameObject root, string target, float width = 0 )
        // {
        //     GUI.changed = false;
        //     SerializeValueBehaviour valueBehaviour = root.GetComponent<SerializeValueBehaviour>( );
        //     if( valueBehaviour != null )
        //     {
        //         GUI.enabled = true;
        //         if( width == 0 ) width = EditorGUIUtility.currentViewWidth; 
        //         SerializeValueList valueList = valueBehaviour.list;
        //         SerializeValue[] gobjs = valueList.GetFields<GameObject>( );
        //         List<string> names = new List<string>( );
        //         for( int i = 0; i < gobjs.Length; ++i ) names.Add( gobjs[ i ].key );
        //         int value = names.FindIndex( ( prop ) => prop == target );
        //         GUI.contentColor = value == -1 ? Color.red: Color.green;
        //         int nextValue = UnityEditor.EditorGUILayout.Popup( "", value, names.ToArray( ), GUILayout.Width( width - 30f ) );
        //         if( value != nextValue )
        //         {
        //             GUI.changed = true;
        //             target = names[ nextValue ];
        //         }
        //         GameObject gobj = valueList.GetGameObject( target );
        //         GUIStyle style = new GUIStyle( "toolbarButton" );
        //         style.alignment = TextAnchor.MiddleCenter;
        //         GUI.enabled = gobj != null ? true: false;
        //         if( GUILayout.Button( "sel", style, GUILayout.Width( 30f ) ) )
        //         {
        //             UnityEditor.Selection.activeGameObject = gobj;
        //         }
        //         GUI.enabled = true;
        //         GUI.contentColor = Color.white;
        //     }
        //     return target;
        // }
        
        //
        // public static GameObject GetTargetField( GameObject root, string target )
        // {
        //     SerializeValueBehaviour valueBehaviour = root.GetComponent<SerializeValueBehaviour>( );
        //     if( valueBehaviour != null )
        //     {
        //         return valueBehaviour.list.GetGameObject( target );
        //     }
        //     return null;
        // }
        // // 
        // public static T GetTargetField<T>( GameObject root, string target ) where T : Component
        // {
        //     GameObject gobj = GetTargetField( root, target );
        //     if( gobj != null )
        //     {
        //         return gobj.GetComponent<T>( );
        //     }
        //     return null;
        // }
        
        #endregion 
        

        #region common
        

        public static void Separator( float height = 4 )
        {
            GUILayout.Box( "", new GUIStyle( "sv_iconselector_back" ), GUILayout.Height( height ), GUILayout.ExpandWidth( true ) );
        }
        

        public static float VerticalSplitter( float width, float min, float max, bool flip = false )
        {
            GUILayout.Box( "", GUIStyle.none, GUILayout.Width( 10 ), GUILayout.ExpandHeight( true ) );
            Rect rc = GUILayoutUtility.GetLastRect( );
            
            EditorGUIUtility.AddCursorRect( rc, MouseCursor.ResizeHorizontal );
            
            int controlID = GUIUtility.GetControlID( FocusType.Passive );
            
            switch( Event.current.type )
            {
            case EventType.MouseDown:
                if( rc.Contains( Event.current.mousePosition ) )
                {
                    GUIUtility.hotControl = controlID;
                    Event.current.Use( );
                }
                break;
            case EventType.MouseUp:
            case EventType.Ignore:
                if( GUIUtility.hotControl == controlID )
                {
                    GUIUtility.hotControl = 0;
                    Event.current.Use( );
                }
                break;
            case EventType.MouseDrag:
                if( GUIUtility.hotControl == controlID )
                {
                    if( flip )
                    {
                        width = Mathf.Clamp( width - Event.current.delta.x, min, max );
                    }
                    else
                    {
                        width = Mathf.Clamp( width + Event.current.delta.x, min, max );
                    }
                    Event.current.Use( );
                }
                break;
            }
            
            return width;
        }
         
    
        public static float HorizontalSplitter( float height, float min, float max, bool flip = false, float handleSize = 4.0f )
        {
            GUILayout.Box( "", GUIStyle.none, GUILayout.Height( handleSize ), GUILayout.ExpandWidth( true ) );
            Rect rc = GUILayoutUtility.GetLastRect( );
            
            EditorGUIUtility.AddCursorRect( rc, MouseCursor.ResizeVertical );
            
            int controlID = GUIUtility.GetControlID( FocusType.Passive );
            
            switch( Event.current.type )
            {
                case EventType.MouseDown:
                    if( rc.Contains( Event.current.mousePosition ) )
                    {
                        GUIUtility.hotControl = controlID;
                        Event.current.Use( );
                    }
                    break;
                case EventType.MouseUp:
                case EventType.Ignore:
                    if( GUIUtility.hotControl == controlID )
                    {
                        GUIUtility.hotControl = 0;
                        Event.current.Use( );
                    }
                    break;
                case EventType.MouseDrag:
                    if( GUIUtility.hotControl == controlID )
                    {
                        if( flip )
                        {
                            height = Mathf.Clamp( height - Event.current.delta.y, min, max );
                        }
                        else
                        {
                            height = Mathf.Clamp( height + Event.current.delta.y, min, max );
                        }
                        Event.current.Use( );
                    }
                    break;
            }
            
            return height;
        }
        
        #endregion common
    }
    
    #endif
}
