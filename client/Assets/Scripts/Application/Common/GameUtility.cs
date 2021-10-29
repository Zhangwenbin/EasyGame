using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace EG
{

    public static class GameUtility
    {
        public const float SmallNumber = 0.0001f;

        
        static bool     m_LayerIndexInitialized;
        
        static int      m_LayerDefault;
        static int      m_LayerUI;

        static int      m_Layer3DUI;
        static int      m_LayerEffect;

        
        public static int LayerDefault              { get { if (!m_LayerIndexInitialized) InitializeLayerIndices(); return m_LayerDefault;          } }
        public static int LayerUI                   { get { if (!m_LayerIndexInitialized) InitializeLayerIndices(); return m_LayerUI;               } }
  
        public static int Layer3DUI                 { get { if (!m_LayerIndexInitialized) InitializeLayerIndices(); return m_Layer3DUI;             } }

        public static int LayerEffect               { get { if (!m_LayerIndexInitialized) InitializeLayerIndices(); return m_LayerEffect;           } }

 

        public static int LayerMaskDefault          { get { return 1 << LayerDefault;           } }
        public static int LayerMaskUI               { get { return 1 << LayerUI;                } }

        public static int LayerMask3DUI             { get { return 1 << Layer3DUI;              } }
        public static int LayerMaskEffect           { get { return 1 << LayerEffect;            } }


        static void InitializeLayerIndices()
        {
            m_LayerDefault          = GetLayerIndex( "Default" );
            m_LayerUI               = GetLayerIndex( "UI" );
            m_Layer3DUI             = GetLayerIndex( "3DUI" );
            m_LayerEffect           = GetLayerIndex( "EFFECT" );

            m_LayerIndexInitialized = true;
        }
        

        static int GetLayerIndex( string name )
        {
            int ret = LayerMask.NameToLayer( name );
            if( ret < 0 )
            {
                Debug.LogError( "Layer '" + name + "' not found." );
                ret = 0;
            }
            return ret;
        }
        

        public static void SetLayer( GameObject go, int layer, bool changeChildren )
        {
            if( go == null ) return;
            if( changeChildren )
            {
                Transform[] tr = go.GetComponentsInChildren<Transform>( true );
                for( int i = 0; i < tr.Length; ++i )
                {
                    tr[ i ].gameObject.layer = layer;
                }
            }
            else
            {
                go.gameObject.layer = layer;
            }
        }

        public static void SetLayer( Component go, int layer, bool changeChildren )
        {
            if( go == null ) return;
            SetLayer( go.gameObject, layer, changeChildren );
        }
        
        public enum EAttachOption
        {
            None             = 0,
            EnablePosOffset  = ( 1<<0 ),
            EnableRotOffset  = ( 1<<1 ),
        }
        
        [System.Serializable]
        public struct AttachParam
        {
            public GameUtility.EAttachOption    Option;                // 
            public GameObject                   AttachTarget;          // 
            public Vector3                      PosOffset;             // 
            public Quaternion                   RotOffset;             // 
            
            public void SetPosOffset( Vector3 ofs )
            {
                Option |= GameUtility.EAttachOption.EnablePosOffset;
                PosOffset = ofs;
            }
            
            public void SetRotOffset( Quaternion ofs )
            {
                Option |= GameUtility.EAttachOption.EnableRotOffset;
                RotOffset = ofs;
            }
        }
        
        public static void CalcAttachPosition( ref AttachParam param, Vector3 localPosition, Quaternion localRotation, out Vector3 spawnPos, out Quaternion spawnRot )
        {
            spawnPos = param.PosOffset + localPosition;
            spawnRot = param.RotOffset * localRotation;
            
            if( param.AttachTarget != null )
            {
                Transform transform = param.AttachTarget.transform;
                
                if( ( param.Option & EAttachOption.EnablePosOffset ) != 0 )
                {
                    spawnPos = transform.TransformPoint( spawnPos );
                }
                else
                {
                    spawnPos = transform.TransformPoint( Vector3.zero ) + spawnPos;
                }
                
                if( ( param.Option & EAttachOption.EnableRotOffset ) != 0 )
                {
                    spawnRot = transform.rotation * spawnRot;
                }
            }
        }
        
        public static void CalcAttachPosition( ref AttachParam param, GameObject prefab, out Vector3 spawnPos, out Quaternion spawnRot )
        {
            CalcAttachPosition( ref param, prefab.transform.localPosition, prefab.transform.localRotation, out spawnPos, out spawnRot );
        }
        

        public static void CalcAttachPosition( GameObject attach, Vector3 posOffset, Quaternion rotOffset, GameObject prefab, EAttachOption option, out Vector3 spawnPos, out Quaternion spawnRot )
        {
            AttachParam param = new AttachParam();
            param.AttachTarget = attach;
            param.PosOffset = posOffset;
            param.RotOffset = rotOffset;
            param.Option = option;
            
            CalcAttachPosition( ref param, prefab.transform.localPosition, prefab.transform.localRotation, out spawnPos, out spawnRot );
        }
        
        
        public static void StopEmitters( GameObject obj )
        {
            ParticleSystem[] psList = obj.GetComponentsInChildren<ParticleSystem>( true );
            for( int i = 0; i < psList.Length; ++i )
            {
                ParticleSystem.EmissionModule module = psList[i].emission;
                ParticleSystem.MainModule main = psList[ i ].main;
                module.enabled = false;
                main.loop = false;
            }
        }
        
        
        public static Vector3 RaycastGround( Vector3 position )
        {
            int layerMask = 0x7FFFFFFF & ~( LayerMaskEffect );
            return Utility.RaycastGround( position, layerMask );
        }

 
        public static Vector3 WorldToUI( Camera cam, RectTransform canvas, RectTransform trs, Vector3 worldPos )
        {
            float w = ( canvas.rect.width - trs.rect.width ) / 2;
            float h = ( canvas.rect.height - trs.rect.height ) / 2;
            
            Vector3 anchoredPosition = cam.WorldToScreenPoint( worldPos );
            
            anchoredPosition.x = ( anchoredPosition.x / Screen.width * canvas.rect.width ) - w;
            anchoredPosition.y = ( anchoredPosition.y / Screen.height * canvas.rect.height ) - h;
            
            return anchoredPosition;
        }
        
 
        public static void Swap<T>( ref T a, ref T b )
        {
            T tmp = a;
            a = b;
            b = tmp;
        }

    }
}
