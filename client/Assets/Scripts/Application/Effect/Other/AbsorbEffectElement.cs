using UnityEngine;
using System.Collections;


namespace EG
{

    [AddComponentMenu("Scripts/Application/Common/Effect/Other/AbsorbEffectElement")]
    public class AbsorbEffectElement : AppMonoBehaviour
    {
        public enum Phase
        {
            None            ,
            MoveToMidPoint  ,
            MoveToGoal      ,
            Exit            ,
        }
        
        public delegate void Callback( AbsorbEffectElement element, object value );
        
        
        
        public float                    Speed           = 5.0f;             // 
        public float                    Damping         = 12;               // 
        public float                    Delay           = 0.8f;             // 
        public float                    Accel           = 20;               // 
        
        // ------------------------------------
        
        private AbsorbEffectElement     m_Resource      = null;
        private Phase                   m_Phase         = Phase.None;
        
        private Transform               m_Transform     = null;
        private Transform               m_TargetObject  = null;
        private Vector3                 m_TargetOffset  = Vector3.zero;
        
        private float                   m_Speed         = 0;
        private float                   m_Damping       = 0;
        private float                   m_Delay         = 0;
        private float                   m_Accel         = 0;
        
        private EffectParam             m_HitEffect     = null;
        private GameUtility.AttachParam m_HitAttachParam= default(GameUtility.AttachParam);
        
        private Callback                m_Callback      = null;
        private object                  m_Value         = null;
        

        public Vector3              TargetPos       { get { return ( m_TargetObject != null) ? m_TargetObject.position + m_TargetOffset : Vector3.zero; } }
        


        public override void Initialize( )
        {
            base.Initialize( );
            
            // ----------------------------------------
            
            m_Transform = GetComponent<Transform>( );
            
            m_Delay     = Delay * Random.Range( 0.9f, 1.1f );
            m_Speed     = Speed * Random.Range( 0.9f, 1.1f );
            m_Damping   = Damping * Random.Range( 0.9f, 1.1f );
            m_Accel     = Accel * Random.Range( 0.9f, 1.1f );
            
            ParticleSystem[] array = gameObject.GetComponentsInChildren<ParticleSystem>( true );
            for(int i = 0; i < array.Length; ++i)
            {
                ParticleSystem.EmissionModule emission = array[i].emission;
                emission.enabled = true;
            }
            
            // 
            if( m_Resource != null )
            {
                ParticleSystem[] resources = m_Resource.GetComponentsInChildren<ParticleSystem>( true );
                for( int i = 0; i < resources.Length && i < array.Length; ++i )
                {
                    ParticleSystem.MainModule main = array[ i ].main;
                    main.cullingMode = ParticleSystemCullingMode.AlwaysSimulate;
                    main.loop = resources[i].main.loop;
                }
            }
            else
            {
                for( int i = 0; i < array.Length; ++i )
                {
                    ParticleSystem.MainModule main = array[ i ].main;
                    main.cullingMode = ParticleSystemCullingMode.AlwaysSimulate;
                }
            }
        }
        

        public override void Release( )
        {
            // ----------------------------------------
            
            base.Release( );
        }
        
        
        private void Update( )
        {
            switch( m_Phase )
            {
                case Phase.MoveToMidPoint:
                {
                    m_Speed = Mathf.Max( m_Speed - m_Damping * Time.deltaTime, 0.0f );
                    m_Delay -= Time.deltaTime;
                    if( m_Delay <= 0.0f )
                    {
                        m_Phase = Phase.MoveToGoal;
                    }
                }
                break;
                case Phase.MoveToGoal:
                {
                    m_Speed += m_Accel * Time.deltaTime;
                    
                    Vector3 v = TargetPos - transform.position;
                    float delta = m_Speed * Time.deltaTime;
                    if( Vector3.Dot( v, v ) <= delta * delta )
                    {
                        GameUtility.StopEmitters( gameObject );
                        m_Speed = 0;
                        m_Phase = Phase.Exit;
                    }
                    
                    m_Transform.rotation = Quaternion.LookRotation( v );
                }
                break;
                case Phase.Exit:
                {
                    ParticleSystem[] ps = GetComponentsInChildren<ParticleSystem>( );
                    for(int i = 0; i < ps.Length; ++i )
                    {
                        if( ps[i].particleCount > 0 )
                        { 
                            return;
                        }
                    }
                    End( );
                }
                break;
            }
            
            m_Transform.position += m_Transform.forward * m_Speed * Time.deltaTime;
        }
        

        public void SetResource( AbsorbEffectElement value )
        {
            m_Resource  = value;
        }

        public void SetHitEffect( EffectParam hit, GameUtility.AttachParam attachParam )
        {
            m_HitEffect = hit;
            m_HitAttachParam = attachParam;
        }
        
   
        public void SetCallback( Callback callback, object value )
        {
            m_Callback  = callback;
            m_Value     = value;
        }
        

        public void Play( Vector3 start, Vector3 offset, Quaternion rot, Transform goal )
        {
            m_Transform.position    = start;
            m_Transform.rotation    = rot;
            m_TargetObject          = goal;
            m_TargetOffset          = offset;
            
            m_Phase = Phase.MoveToMidPoint;
            
            TrailRenderer trail = gameObject.GetComponent<TrailRenderer>( );
            if( trail != null )
            {
                trail.Clear( );
            }
            
            gameObject.SetActive( true );
        }
        

        public void End( )
        {
            if( m_HitEffect != null )
            {
                EffectData data = m_HitEffect.CreateEffect( ref m_HitAttachParam );
                if( data != null )
                {
                    data.Play( );
                }
            }
            
            if( m_Callback != null )
            {
                m_Callback( this, m_Value );
                m_Callback = null;
            }
            
            m_Phase = Phase.None;
            
            gameObject.SetActive( false );
        }
        
   
        public bool IsEnd( )
        {
            return m_Phase == Phase.None || m_Phase == Phase.Exit;
        }
        
        
    }
    
    #if UNITY_EDITOR
    
    [UnityEditor.CustomEditor(typeof(AbsorbEffectElement), true)]
    public class EditorInspactor_AbsorbEffectElement : UnityEditor.Editor
    {

        public override void OnInspectorGUI( )
        {
            CustomFieldAttribute.OnInspectorGUI( typeof( AbsorbEffectElement ), serializedObject );
        }
    }
    
    #endif
}
