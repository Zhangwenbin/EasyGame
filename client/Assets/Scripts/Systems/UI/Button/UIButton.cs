using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace EG
{
    [AddComponentMenu("Scripts/System/UI/Button/UIButton")]
    public class UIButton : UnityEngine.UI.Button
    {
        public enum EFlag
        {
            Dummy                   ,
            InteractableGrayScale   ,
            CloneMaterial           ,
            InteractableSyncRaycast ,
        }
        
        #if UNITY_EDITOR
        public static string[]  EFlagNames = new string[]
        {
            "",
            "Interactable关闭时以灰度显示",
            "CloneMaterial",
            "InteractableSyncRaycast",
        };
        #endif
        

        [CustomFieldAttribute("Flag",CustomFieldAttribute.Type.BitFlag,typeof(EFlag),typeof(UIButton))]
        [UnityEngine.Serialization.FormerlySerializedAs("m_Flag")]
        public BitFlag          m_Flag          = new BitFlag( );
        
        // ---------------------------------------------
        
        private bool            m_Active        = true;
        private Graphic[]       m_Graphics      = null;
        private Material        m_Material      = null;
        
        
        private bool            isInteractableGrayScale     { get { return m_Flag.HasValue( (int)EFlag.InteractableGrayScale ); } }
        private bool            isInteractableSyncRaycast   { get { return m_Flag.HasValue( (int)EFlag.InteractableSyncRaycast ); } }
        
        protected override void Awake()
        {
            base.Awake( );
            
            // ----------------------------------------
            
            if( Application.isPlaying )
            {
                if( m_Flag.HasValue( EFlag.CloneMaterial ) )
                {
                    if( image != null )
                    {
                        Material material = image.material;
                        if( material != null )
                        {
                            m_Material = new Material( material );
                            image.material = m_Material;
                        }
                    }
                }
            }
        }
        
        protected override void Start( )
        {
            base.Start( );
        }
        
        void LateUpdate( )
        {
            if( isInteractableGrayScale )
            {
                if( m_Active != interactable )
                {
                    if( image != null )
                    {
                        Material material = image.material;
                        if( interactable )
                        {
                            if( material != null )
                            {
                                material.DisableKeyword( "USE_GRAYSCALE" );
                            }
                        }
                        else
                        {
                            if( material != null )
                            {
                                material.EnableKeyword( "USE_GRAYSCALE" );
                            }
                        }
                    }
                    m_Active = interactable;
                }
            }
            else
            {
                if( m_Active == false )
                {
                    if( image != null )
                    {
                        Material material = image.material;
                        if( material != null )
                        {
                            material.DisableKeyword( "USE_GRAYSCALE" );
                        }
                    }
                    m_Active = true;
                }
            }
            
            // 
            if( isInteractableSyncRaycast )
            {
                if( m_Graphics == null ) m_Graphics = gameObject.GetComponents<Graphic>( );
                if( m_Graphics != null && m_Graphics.Length > 0 )
                {
                    for( int i = 0; i < m_Graphics.Length; ++i )
                    {
                        m_Graphics[i].raycastTarget = interactable;
                    }
                }
            }
        }
        
        protected override void OnDestroy( )
        {
            if( m_Material != null )
            {
                Destroy( m_Material );
                m_Material = null;
            }
            
            base.OnDestroy( );
        }
        
    }
}
