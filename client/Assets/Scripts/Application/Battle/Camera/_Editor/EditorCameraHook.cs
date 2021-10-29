using UnityEngine;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;

namespace EG
{
    [AddComponentMenu("")]
    [ExecuteInEditMode]
    public class EditorCameraHook : CameraHook
    {
        [System.NonSerialized]
        public bool BGMask;
        
        Texture2D mBGTex;
        
        void LateUpdate( )
        {
            // if( LWARS.LightSetting.GetInstance( ) )
            // {
            //     RefreshEnvironment( );
            // }
        }
        
        new protected void OnPreRender()
        {
            // if (LWARS.LightSetting.GetInstance())
            // {
            //     base.OnPreRender();
            // }

    //        if (BGMask)
    //        {
    //            Camera cam = GetComponent<Camera>();
    //            mBGTex = new Texture2D(1, 1);
    //            mBGTex.SetPixel(0, 0, cam.backgroundColor);
    //            mBGTex.Apply();
    //            Shader.SetGlobalTexture("_bgTex", mBGTex);
    //            Shader.EnableKeyword("BGMASK_ON");
    //        }
    //        else
    //        {
    //            Shader.DisableKeyword("BGMASK_ON");
    //        }
        }
        
        void FreeBGTex()
        {
            if( mBGTex != null )
            {
                DestroyImmediate( mBGTex );
                mBGTex = null;
            }
        }
        
        void OnPostRender()
        {
            FreeBGTex( );
        }
        
        protected override void OnDestroy()
        {
            FreeBGTex( );
            base.OnDestroy( );
        }
    }
    
    [CustomEditor(typeof(CameraHook), true)]
    public class EditorInspector_CameraHook : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            
    //        LWARS.CameraHook camera_hook = target as LWARS.CameraHook;
    //
    //
    //        GUILayout.Space(10);
    //
    //        EditorGUILayout.BeginHorizontal( "box" );
    //        {
    //            EditorGUILayout.LabelField( "ColorMod : ", GUILayout.Width( 120.0f ) );
    //            camera_hook.ColorMod = EditorGUILayout.ColorField(camera_hook.ColorMod );
    //        }
    //        EditorGUILayout.EndHorizontal();
            
        }
    }
}
#endif
