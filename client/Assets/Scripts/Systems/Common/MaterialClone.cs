using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace EG
{
    [AddComponentMenu("Scripts/System/Common/MaterialClone")]
    public class MaterialClone : AppMonoBehaviour
    {
        public struct Param
        {
            public Graphic      graphic;
            public Renderer     renderer;
            public Material     matreg;
            public Material     mat;
            
            public Param( Graphic _graphic )
            {
                graphic = _graphic;
                renderer = null;
                matreg = graphic.material;
                mat = new Material( graphic.material );
                graphic.material = mat;
            }
            
            public Param( Renderer _renderer )
            {
                graphic = null;
                renderer = _renderer;
                matreg = _renderer.sharedMaterial;
                mat = new Material( _renderer.sharedMaterial );
                _renderer.material = mat;
            }
        }
        
        private List<Param>     m_Materials         = new List<Param>( );
        
        
        public List<Param>      Materials           { get { return m_Materials; } }
        public Material         Material            { get { return m_Materials.Count > 0 ? m_Materials[0].mat: null; } }
        
        
        
        public override void Release( )
        {
            if( m_Materials.Count == 0 ) return;
            
            // 
            for( int i = 0; i < m_Materials.Count; ++i )
            {
                if( m_Materials[i].graphic != null )
                {
                    m_Materials[i].graphic.material = m_Materials[i].matreg;
                }
                else if( m_Materials[i].renderer != null )
                {
                    m_Materials[i].renderer.material = m_Materials[i].matreg;
                }
                Destroy( m_Materials[i].mat );
            }
            
            m_Materials.Clear( );
        }
        
        public void Refresh( bool checkAll = false )
        {
            m_Materials.Clear( );
            
            if( checkAll )
            {
                // 
                Graphic[] graphics = GetComponentsInChildren<Graphic>( true );
                if( graphics != null && graphics.Length > 0 )
                {
                    for( int i = 0; i < graphics.Length; ++i )
                    {
                        m_Materials.Add( new Param( graphics[i] ) );
                    }
                }
                
                // 
                // Renderer Particle Renderer SkinnedMeshRenderer, MeshRenderer 
                SkinnedMeshRenderer[] sknRenderers = GetComponentsInChildren<SkinnedMeshRenderer>( true );
                if( sknRenderers != null && sknRenderers.Length > 0 )
                {
                    for( int i = 0; i < sknRenderers.Length; ++i )
                    {
                        m_Materials.Add( new Param( sknRenderers[ i] ) );
                    }
                }
                
                MeshRenderer[] meshRenderers = GetComponentsInChildren<MeshRenderer>( true );
                if( meshRenderers != null && meshRenderers.Length > 0 )
                {
                    for( int i = 0; i < meshRenderers.Length; ++i )
                    {
                        m_Materials.Add( new Param( meshRenderers[ i] ) );
                    }
                }
            }
            else
            {
                // Graphic 
                Graphic graphic = gameObject.GetComponent<UnityEngine.UI.Graphic>( );
                if( graphic != null )
                {
                    m_Materials.Add( new Param( graphic ) );
                }
                
                // 
                // Renderer ParticleのRenderer SkinnedMeshRenderer, MeshRenderer
                SkinnedMeshRenderer sknRenderer = GetComponentInChildren<SkinnedMeshRenderer>();
                if( sknRenderer != null )
                {
                    m_Materials.Add( new Param( sknRenderer ) );
                }
                
                MeshRenderer meshRenderer = GetComponentInChildren<MeshRenderer>();
                if( meshRenderer != null )
                {
                    m_Materials.Add( new Param( meshRenderer ) );
                }
            }
        }
        
    }
}
