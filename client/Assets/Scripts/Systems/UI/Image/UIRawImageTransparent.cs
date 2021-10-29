using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

namespace EG
{
    [AddComponentMenu("Scripts/System/UI/Raw Image (透明)")]
    public class UIRawImageTransparent : RawImage
    {
        public string Preview;
        
    #if UNITY_EDITOR
       protected override void Awake()
       {
           if( UnityEditor.BuildPipeline.isBuildingPlayer )
           {
               Preview = null;
               return;
           }
            
           base.Awake( );
       }
    #endif
        
    #if UNITY_EDITOR
       public override Texture mainTexture
       {
           get
           {
               if( !Application.isPlaying && !string.IsNullOrEmpty( Preview ) )
               {
                   return EditorHelp.Load<Texture>( Preview + ".png" );
               }
               return base.mainTexture;
           }
       }
    #endif
        
       protected override void OnPopulateMesh( VertexHelper vh )
       {
           //return;
           if( texture != null
    #if UNITY_EDITOR
                || (!Application.isPlaying && !string.IsNullOrEmpty( Preview ))
    #endif
           )
           {
               base.OnPopulateMesh( vh );
           }
           else
           {
               vh.Clear( );
           }
       }
    }
}
