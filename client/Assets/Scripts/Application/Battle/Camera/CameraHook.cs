using UnityEngine;
using System.Collections;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace EG
{

    [AddComponentMenu("Scripts/Application/Camera/CameraHook")]
    [DisallowMultipleComponent]
    public partial class CameraHook : AppMonoBehaviour
    {
        public delegate void PreCullEvent( Camera camera );
        
        
        private double      m_GameTime      = 0.0;
        private Vector4     m_GameTimeProp  = Vector4.zero;


        // public Color                    ColorFade
        // {
        //     set 
        //     {
        //         LightSetting.SetFadeColor( value );
        //     }
        //     get
        //     {
        //         return LightSetting.GetFadeColor( );
        //     }
        // }
        //
        // public float                    CrystalFade
        // {
        //     set 
        //     {
        //         LightSetting.SetCrystalFade( value );
        //     }
        //     get
        //     {
        //         return LightSetting.GetCrystalFade( );
        //     }
        // }
        //
        // public Color                    ColorMod
        // {
        //     set 
        //     {
        //         LightSetting.SetModColor( value );
        //     }
        //     get
        //     {
        //         return LightSetting.GetModColor( );
        //     }
        // }
        
        
        public static PreCullEvent      PreCullEventListeners;
        
        
        private void Start( )
        {
        }
        

        private void LateUpdate( )
        {
            
        }
        

        public void RefreshEnvironment( )
        {
            #if UNITY_EDITOR
            if( Application.isPlaying == false )
            {
                //float editorTime = (float)EditorApplication.timeSinceStartup;
                //Shader.SetGlobalVector( "_GameTime", new Vector4( editorTime/20, editorTime, editorTime*2, editorTime*3 ) );
                Shader.SetGlobalTexture( "_DepthShadowMapTexBg", Texture2D.blackTexture );
                Shader.SetGlobalTexture( "_DepthShadowMapDepthTexBg", Texture2D.blackTexture );
                Shader.SetGlobalTexture( "_DepthShadowMapTex", Texture2D.blackTexture );
                Shader.SetGlobalTexture( "_DepthShadowMapDepthTex", Texture2D.blackTexture );
                Shader.SetGlobalTexture( "_ShadowMapTex1", Texture2D.blackTexture );
                Shader.SetGlobalTexture( "_ShadowMapTex2", Texture2D.blackTexture );
            }
            else
            {
                m_GameTime += TimerManager.UnscaledDeltaTime;
                m_GameTimeProp.x = (float)m_GameTime / 20;
                m_GameTimeProp.y = (float)m_GameTime;
                m_GameTimeProp.z = (float)m_GameTime * 2;
                m_GameTimeProp.w = (float)m_GameTime * 3;
                Shader.SetGlobalVector( "_GameTime", m_GameTimeProp );
            }
            #else
            m_GameTime += TimerManager.UnscaledDeltaTime;
            m_GameTimeProp.x = (float)m_GameTime / 20;
            m_GameTimeProp.y = (float)m_GameTime;
            m_GameTimeProp.z = (float)m_GameTime * 2;
            m_GameTimeProp.w = (float)m_GameTime * 3;
            Shader.SetGlobalVector( "_GameTime", m_GameTimeProp );
            #endif
            //
            // Shader.SetGlobalVector( "_LightDir", LightSetting.GetLightDir( ) );
            //
            // Shader.SetGlobalColor( "_AmbientLightColor", LightSetting.GetAmbientLightColor( ) );
            //
            // Shader.SetGlobalColor( "_DirectLightColor", LightSetting.GetDirectLightColor( ) );
            //
            // Shader.SetGlobalFloat( "_RimScale", LightSetting.GetInDirectLightExp( ) );
            // Shader.SetGlobalColor( "_RimColor", LightSetting.GetInDirectLightColor( ) );
            //
            // Shader.SetGlobalTexture( "_NoiseTex", LightSetting.GetNoiseTexture( ) );
            // Shader.SetGlobalVector( "_NoiseLightParamBg", LightSetting.GetNoiseLightParamBg( ) );
            // //Shader.SetGlobalVector( "_NoiseLightParam", LightSetting.GetNoiseLightParam( ) );
            // //Shader.SetGlobalVector( "_NoiseShadowParam", LightSetting.GetNoiseShadowParam( ) );
            //
            // if( LightSetting.CheckFogUse( ) )
            // {
            //     Shader.SetGlobalVector( "_FogParam", LightSetting.GetFogParam( ) );
            //     Shader.SetGlobalColor( "_FogColor", LightSetting.GetFogColor( ) );
            // }
            // else
            // {
            //     Shader.SetGlobalVector( "_FogParam", Vector4.zero );
            // }
            //
            //
            // Shader.SetGlobalVector( "_LightMapParam", LightSetting.GetLightMapParam( ) );
            //
            // Shader.SetGlobalColor( "_LightMapColor", LightSetting.GetLightMapColor( ) );
            //
            //
            // Shader.SetGlobalVector( "_ShadowExp", LightSetting.GetShadowExp( ) );
            // Shader.SetGlobalColor( "_ShadowFadeColor", LightSetting.GetShadowFadeColor( ) );
            //
            //
            // Shader.SetGlobalColor( "_ColorMod", ColorMod );
            //
            // Shader.SetGlobalColor( "_ColorFade", ColorFade );
            //
            // Shader.SetGlobalFloat( "_CrystalFade", CrystalFade );
        }
        

        protected void OnPreCull( )
        {
//            #if !PRODUCTION_BUILD
//            DebugProfiler.BeginSample( "OnPreCull", new Color( 192f/255f, 64f/255f, 64f/255f ) );
//            #endif
            
            

            if( Application.isPlaying )
            {
               // GetComponent<Camera>( ).cullingMask &= ~(1 << GameUtility.LayerHidden);
            }
            

            if( PreCullEventListeners != null )
            {
                PreCullEventListeners.Invoke( GetComponent<Camera>( ) );
            }
            
//            #if !PRODUCTION_BUILD
//            DebugProfiler.EndSample( );
//            #endif
        }
        

        protected void OnPreRender( )
        {
            RefreshEnvironment( );
        }
        


        public static CameraHook Inject( )
        {
            Camera mainCamera = Camera.main;
            if( mainCamera != null )
            {
                CameraHook hook = mainCamera.gameObject.RequireComponent<CameraHook>( );
                return hook;
            }
            return null;
        }
        

        public static void AddPreCullEventListener( PreCullEvent e )
        {
            if( e == null )
            {
                return;
            }
            
            PreCullEventListeners += e;
        }
        

        public static void RemovePreCullEventListener( PreCullEvent e )
        {
            if( e == null )
            {
                return;
            }
            
            PreCullEventListeners -= e;
        }
        
        
        #if DEBUG_BUILD
        public override Rect OnDebugInspecor( Rect rect, DebugMenu.WindowBase window )
        {
            window.GuiDouble( ref rect, "GameTime", m_GameTime );
            window.GuiVector4( ref rect, "GameTimeProp", m_GameTimeProp );
            return rect;
        }
        #endif

    }
}
