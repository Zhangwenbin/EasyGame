using UnityEngine;

namespace EG
{

    [AddComponentMenu("Scripts/Application/Common/Effect/EffectBase")]
    public class EffectBase : AppMonoBehaviour
    {
        public enum EPlayMode
        {
            None            ,
            OneShot         ,
            Loop            ,
        }
        #if UNITY_EDITOR
        public static string[] EPlayModeNames = new string[]
        {
            "None",
            "OneShot",
            "Loop",
        };
        #endif
        
        public enum EFlag
        {
            UseAliveTime    ,
            Mirror          ,
        }
        #if UNITY_EDITOR
        public static string[] EFlagNames = new string[]
        {
            "UseAliveTime",
            "Mirror",
        };
        #endif
        
        public enum GenerateOption
        {
            None            = 0,
            Attach          = (1<<0),
            AttachOffset    = (1<<1),
            AttachRotation  = (1<<2),
            OneShot         = (1<<3),
            Mirror          = (1<<4),
        }
        

        
        [CustomFieldAttribute("PlayMode",CustomFieldAttribute.Type.Enum,typeof(EPlayMode),typeof(EffectBase))]
        public EPlayMode        PlayMode        = EPlayMode.None;
        
        [CustomFieldAttribute("EFlag",CustomFieldAttribute.Type.BitFlag,typeof(EFlag),typeof(EffectBase))]
        public BitFlag          Flag            = new BitFlag( );
        
        [CustomFieldAttribute("m_AliveTime",CustomFieldAttribute.Type.Float)]
        public float            m_AliveTime     = 0;
        
        private Vector3         m_LocalScale    = Vector3.one;
        
        
        private void Awake( )
        {
            Initialize( );
        }

        public override void Initialize( )
        {
            if( isInitialized ) return;
            
            base.Initialize( );
            
            // ----------------------------------------
            
            m_LocalScale = transform.localScale;
        }
        

        public override void Release( )
        {
            if( isInitialized == false ) return;
            
            // ----------------------------------------
            
            base.Release( );
        }
        
        
        private void LateUpdate( ) 
        {
            if( gameObject.layer != GameUtility.LayerEffect )
            {
                GameUtility.SetLayer( this, GameUtility.LayerEffect, true );
            }
            
            if( Flag.HasValue( (int)EFlag.UseAliveTime ) )
            {
                m_AliveTime -= Time.deltaTime;
                if( m_AliveTime <= 0 )
                {
                    gameObject.SafeDestroy( );
                    return;
                }
            }
            
            if( PlayMode == EPlayMode.OneShot )
            {
                ParticleSystem[] particles = gameObject.GetComponentsInChildren<ParticleSystem>( );
                if( particles != null )
                {
                    for( int i = particles.Length - 1; i >= 0; --i )
                    {
                        ParticleSystem ps = particles[ i ];
                        if( ps.IsAlive( ) )
                        {
                            return;
                        }
                    }
                }
                
                UIParticleSystem[] uiparticles = gameObject.GetComponentsInChildren<UIParticleSystem>( );
                if( uiparticles != null )
                {
                    for( int i = uiparticles.Length - 1; i >= 0; --i )
                    {
                        if( uiparticles[ i ].IsAlive )
                        {
                            return;
                        }
                    }
                }
                
                gameObject.SafeDestroy( );
                
                return;
            }
            
            if( Flag.HasValue( (int)EFlag.Mirror ) )
            {
                Vector3 newScale = m_LocalScale;
                newScale.z *= -1;
                transform.localScale = newScale;
            }
        }
        
        
        public void StopDestroy( )
        {
            GameUtility.StopEmitters( gameObject );
            PlayMode = EPlayMode.OneShot;
        }
        
        public void SetPlayMode( EPlayMode playmode )
        {
            PlayMode = playmode;
        }
        

        public void SetMirror( bool value )
        {
            Flag.SetValue( (int)EFlag.Mirror, value );
        }
        

        public void SetAliveTime( float time )
        {
            Flag.SetValue( (int)EFlag.UseAliveTime, true );
            m_AliveTime = time;
        }
        
        
        private void OnEnable( )
        {
        }
        

        private void OnDisable( )
        {
        }
        
        
        public static EffectBase GenerateEffect( GameObject attach, Vector3 basePosition, Quaternion baseRotation, GameObject effectTemplate, float aliveTime, GenerateOption option )
        {
            if( effectTemplate == null ) return null;
            
            Vector3 spawnPos;
            Quaternion spawnRot;
            CalcPosition( attach, basePosition, baseRotation, effectTemplate, option, out spawnPos, out spawnRot );
            
            GameObject newGobj = Instantiate<GameObject>( effectTemplate, spawnPos, spawnRot );
            
            EffectBase generateObject = newGobj.RequireComponent<EffectBase>( );
            
            if( ( option & GenerateOption.OneShot ) != 0 )
            {
                generateObject.SetPlayMode( EffectBase.EPlayMode.OneShot );
            }
            
            if( ( option & GenerateOption.Mirror ) != 0 )
            {
                generateObject.SetMirror( true );
            }
            
            if( attach != null && ( option & GenerateOption.Attach ) != 0 )
            {
                generateObject.transform.SetParent( attach.transform );
            }
            
            if( aliveTime > 0 )
            {
                generateObject.SetAliveTime( aliveTime );
            }
            
            return generateObject;
        }
        

        public static void CalcPosition( GameObject attach, Vector3 basePosition, Quaternion baseRotation, Vector3 localPosition, Quaternion localRotation, GenerateOption option, out Vector3 spawnPos, out Quaternion spawnRot )
        {
            spawnPos = basePosition + localPosition;
            spawnRot = baseRotation * localRotation;
            
            if( attach != null )
            {
                Transform transform = attach.transform;

                if( ( option & GenerateOption.AttachOffset ) != 0 )
                {
                    spawnPos = transform.TransformPoint( spawnPos );
                }
                else
                {
                    spawnPos = transform.TransformPoint( Vector3.zero ) + spawnPos;
                }
                
                if( ( option & GenerateOption.AttachRotation ) != 0 )
                {
                    spawnRot = transform.rotation * spawnRot;
                }
            }
        }
        
        public static void CalcPosition( GameObject attach, Vector3 basePosition, Quaternion baseRotation, GameObject prefab, GenerateOption option, out Vector3 spawnPos, out Quaternion spawnRot )
        {
            CalcPosition( attach, basePosition, baseRotation, prefab.transform.localPosition, prefab.transform.localRotation, option, out spawnPos, out spawnRot );
        }
        

    }
    
    #if UNITY_EDITOR
    
    [UnityEditor.CustomEditor(typeof(EffectBase), true)]
    public class EditorInspector_EffectBase : UnityEditor.Editor
    {

        public override void OnInspectorGUI( )
        {
            CustomFieldAttribute.OnInspectorGUI( typeof( EffectBase ), serializedObject );
        }
    }
    
    #endif
}
