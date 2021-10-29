using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

namespace EG
{

    [AddComponentMenu("Scripts/System/UI/Particle/UIParticleSystem")]
    public partial class UIParticleSystem : MaskableGraphic
    {
        
        readonly Vector3 POS_TOP_LEFT     = new Vector3( -1,  1, 0 );
        readonly Vector3 POS_TOP_RIGHT    = new Vector3(  1,  1, 0 );
        readonly Vector3 POS_BOTTOM_LEFT  = new Vector3( -1, -1, 0 );
        readonly Vector3 POS_BOTTOM_RIGHT = new Vector3(  1, -1, 0 );
        
        public enum ParticleUpdateMode
        {
            UnscaledTime,
            GameTime,
            FixedTime,
        }
        
        public enum EmitterTypes
        {
            Cone,
            Sphere,
            Box,
        }
        
        public enum ParticleRenderMode
        {
            Billboard,
            StretchBillboard
        }
        
        public enum SimulationSpace
        {
            Local,  // 
            World,  // 
        }
        
        
        
        public ParticleUpdateMode   updateMode          = ParticleUpdateMode.UnscaledTime;
        public EmitterTypes         emitterType         = EmitterTypes.Sphere;
        public ConeEmitter          coneEmitter         = new ConeEmitter( -20, 20 );
        public SphereEmitter        sphereEmitter       = new SphereEmitter( 0, 10 );
        public BoxEmitter           boxEmitter          = new BoxEmitter( 64, 64 );
        
        public ParticleRenderMode   RenderMode          = ParticleRenderMode.Billboard;
        public StretchBillboard     m_StretchBillboard  = new StretchBillboard(1, 0);
        
        public Texture SourceImage;
        
        // 
        public SimulationSpace      SimulateSpace       = SimulationSpace.Local;
        
        [ParticleAttribute]
        public float duration = 1;
        
        [ParticleAttribute]
        public float emissionRate = 10;
        
        [ParticleAttribute]
        public float gravityMultipler = 0;      
        
        [ParticleAttribute]
        public bool loop = true;
        
        [ParticleAttribute]
        public bool prewarm = false;
        
        [ParticleAttribute]
        public int maxParticles = 100;
        
        [ParticleAttribute]
        public float playbackSpeed = 1;
        
        [ParticleAttribute]
        public ColorRange startColor = new ColorRange(Color.white, Color.white);
        
        [ParticleAttribute]
        public float startDelay;
        
        [ParticleAttribute]
        public FloatRange startLifetime = new FloatRange(5, 5);

        [ParticleAttribute]
        public Vector3Range startRotation = new Vector3Range( Vector3.zero, Vector3.zero );
        
        [ParticleAttribute]
        public FloatRange startSize = new FloatRange(1, 1);
        
        [ParticleAttribute]
        public FloatRange startSpeed = new FloatRange(5, 5);
        
        [ParticleAttribute]
        public float emitterRotation;
        
        [ParticleAttribute]
        public Vector3Range angularVelocity = new Vector3Range( Vector3.zero, Vector3.zero );
        public bool angularVelocityEnable;

        [ParticleAttribute]
        public AnimationCurve rotationOverLifetime;
        public bool rotationOverLifetimeEnable;
        
        [ParticleAttribute]
        public Gradient colorOverLifetime;
        public bool colorOverLifetimeEnable;
        
        [ParticleAttribute]
        public AnimationCurve sizeOverLifetime;
        public bool sizeOverLifetimeEnable;
        
        [ParticleAttribute]
        public TextureSheetAnimation textureSheetAnimation = new TextureSheetAnimation(1, 1);
        public bool textureSheetAnimationEnable;
        
        [ParticleAttribute]
        public ParticleBurst burst = new ParticleBurst(0);
        public bool burstEnable;
        
        [ParticleAttribute]
        public VelocityOverLifetime velocityOverLifetime = new VelocityOverLifetime(0);
        public bool velocityOverLifetimeEnable;
        
        [ParticleAttribute]
        public LimitVelocityOverLifetime limitVelocityOverLifetime = new LimitVelocityOverLifetime(0);
        public bool limitVelocityOverLifetimeEnable;
        
        [System.NonSerialized]
        public bool IsPlaying = false;
        
        [System.NonSerialized]
        public bool emit = true;
        
        List<Particle>  m_Particles         = new List<Particle>();
        List<Particle>  m_PoolParticles     = new List<Particle>();
        
        float           m_PrevTime;
        float           m_Time;
        
        float           m_SpawnCount;
        
        bool            m_SettingLoop       = false;         
        
        public int      ParticleCount   { get { return m_Particles.Count; }                                  }
        public float    PlaybackTime    { get { return m_Time; } set { m_Time = value; }                      }
        public bool     IsAlive
        {
            get
            {
                return ParticleCount > 0 || m_Time < duration || loop;
            }
        }
        
        public override Texture mainTexture
        {
            get
            {
                if( SourceImage != null )
                    return SourceImage;
                
                if( material != null )
                    return material.mainTexture;
                
                return base.mainTexture;
            }
        }
        
        
        protected override void Awake()
        {
            base.Awake();
            
            // ----------------------------------------
            
            m_SettingLoop = loop;
        }
        
        protected override void Start()
        {
            base.Start();
            
            // ----------------------------------------
            
            ResetParticleSystem();
        }
        
        
        public override bool Raycast(Vector2 sp, Camera eventCamera)
        {
            return false;
        }
        
        void Update()
        {
            #if UNITY_EDITOR
            //
            if( UnityEditor.EditorApplication.isPlaying == false )
            {
                return;
            }
            #endif
            
            if( IsPlaying == false )
                return;
            
            switch( updateMode )
            {
                case ParticleUpdateMode.FixedTime:
                    AdvanceTime(Time.fixedDeltaTime);
                    break;
                case ParticleUpdateMode.GameTime:
                    AdvanceTime(Time.deltaTime);
                    break;
                case ParticleUpdateMode.UnscaledTime:
                    AdvanceTime(Time.unscaledDeltaTime);
                    break;
            }
        }
        

        public void AdvanceTime(float dt)
        {
            dt *= playbackSpeed;
            
            m_PrevTime = m_Time;
            m_Time += dt;
            
            if (m_Time >= duration)
            {
                if (loop)
                {
                    m_Time = 0;
                    m_PrevTime = 0;
                }
                else
                {
                    m_Time = duration;
                }
            }
            
            // 
            for (int i = m_Particles.Count - 1; i >= 0; --i)
            {
                Particle p = m_Particles[i];
                
                if( p.Update( dt ) )
                {
                    m_Particles.RemoveAt(i);
                    m_PoolParticles.Add(p);
                }
            }
            
            if( emit == false )
            {
                return;
            }
            
            UpdateEmitter( dt );
        }
        

        void UpdateEmitter( float dt )
        {
            if( burstEnable )
            {
                for (int i = 0; i < burst.Points.Length; ++i)
                {
                    if( burst.Points[i].Check( m_PrevTime, m_Time ) )
                    {
                        m_SpawnCount += burst.Points[i].Count;
                    }
                }
            }
            
            if( 0.0f <= m_Time && m_Time < duration )
            {
                m_SpawnCount += emissionRate * dt;
            }
            
            if( m_SpawnCount >= 1.0f )
            {
                int spawn = Mathf.FloorToInt( m_SpawnCount );
                
                for( int i = 0; i < spawn && m_Particles.Count < maxParticles; ++i )
                {
                    Particle p;
                    
                    if( m_PoolParticles.Count > 0 )
                    {
                        p = m_PoolParticles[0];
                        m_PoolParticles.RemoveAt(0);
                    }
                    else
                    {
                        p = new Particle( this );
                    }
                    
                    Vector3 pos = Vector3.zero;
                    Vector3 vel = Vector3.up;
                    
                    CalcStartPosAndVelByEmitter( ref pos, ref vel );
                    p.Setup( ref pos, ref vel );
                    
                    if( RenderMode == ParticleRenderMode.StretchBillboard )
                    {
                        p.SetStretchLength( m_StretchBillboard.GetLength() );
                    }
                    
                    if( textureSheetAnimationEnable )
                    {
                        p.SetTexAnmStartFrame( textureSheetAnimation.GetStartFrame() );
                    }
                    
                    m_Particles.Add( p );
                }
                
                m_SpawnCount -= spawn;
            }
            
            // 
            SetVerticesDirty();
            
            #if UNITY_EDITOR
            if( Application.isPlaying == false)
            {
                // 
                UpdateGeometry();
                UpdateMaterial();
                
                // 
                Canvas c = GetComponentInParent<Canvas>();
                if( c != null )
                {
                    c.enabled = !c.enabled;
                    c.enabled = !c.enabled;
                }
            }
            #endif
        }
        
    
        void CalcStartPosAndVelByEmitter( ref Vector3 pos, ref Vector3 vel )
        {
            switch( emitterType )
            {
                case EmitterTypes.Box:
                    boxEmitter.Calc( emitterRotation, ref pos, ref vel );
                    break;
                    
                case EmitterTypes.Cone:
                    coneEmitter.Calc( emitterRotation, ref pos, ref vel );
                    break;
                    
                case EmitterTypes.Sphere:
                    sphereEmitter.Calc( emitterRotation, ref pos, ref vel );
                    break;
            }
        }
        

        void _CalcSimulate( float simulateTime )
        {
            if( simulateTime <= 0 )
                return;
            
            float elapse = 0.3f;
            while( simulateTime > 0 )
            {
                float tmp = Mathf.Min( elapse, simulateTime );
                AdvanceTime( tmp );
                simulateTime -= tmp;
            }
        }
        

        public void ResetParticleSystem()
        {
#if UNITY_EDITOR
            if( Application.isPlaying )
            #endif
            {
                loop = m_SettingLoop;
            }
            IsPlaying = true;
            emit = true;
            m_Time = -startDelay;
            m_Particles.Clear();
            
            if( loop && prewarm )
            {
                _CalcSimulate( startLifetime.Max * 1.5f );
            }
        }
        
        public void ResumeEmitters()
        {
            UIParticleSystem[] particles = GetComponentsInChildren<UIParticleSystem>();
            for( int i = 0; i < particles.Length; ++i )
            {
                particles[ i ].IsPlaying = true;
            }
        }
        
        public void PauseEmitters()
        {
            UIParticleSystem[] particles = GetComponentsInChildren<UIParticleSystem>();
            for( int i = 0; i < particles.Length; ++i )
            {
                particles[ i ].IsPlaying = false;
            }
        }
        
        public void ResetEmitters()
        {
            UIParticleSystem[] particles = GetComponentsInChildren<UIParticleSystem>();
            for( int i = 0; i < particles.Length; ++i )
            {
                particles[ i ].ResetParticleSystem();
                particles[ i ].IsPlaying = false;
                particles[ i ].UpdateGeometry();
            }
        }
        
        public void StopEmitters()
        {
            UIParticleSystem[] particles = GetComponentsInChildren<UIParticleSystem>();
            for( int i = 0; i < particles.Length; ++i )
            {
                particles[ i ].emit = false;
                particles[ i ].loop = false;
            }
        }
        

        
        protected override void OnPopulateMesh(VertexHelper vh)
        {
            UIVertex v = new UIVertex();
            Vector2 uvTopLeft = new Vector2( 0, 1 );
            Vector2 uvTopRight = new Vector2( 1, 1 );
            Vector2 uvBottomLeft = new Vector2( 0, 0 );
            Vector2 uvBottomRight = new Vector2( 1, 0 );
            Vector2 uvOffset = new Vector2(0, 0);
            Color32 color;
            Quaternion quat;
            int n = 0;
            
            vh.Clear();
            
            if (textureSheetAnimationEnable)
            {
                textureSheetAnimation.Prepare( ref uvTopLeft, ref uvTopRight, ref uvBottomLeft, ref uvBottomRight );
            }
            
            if( RenderMode == ParticleRenderMode.Billboard )
            {
                for (int i = m_Particles.Count - 1; i >= 0; --i)
                {
                    Particle p = m_Particles[i];
                    
                    quat = Quaternion.Euler( p.rotation );
                    float extent = p.size;
                    float fraction = p.Fraction;
                    color = p.color;
                    
                    if (textureSheetAnimationEnable)
                    {
                        textureSheetAnimation.OnUpdate( p, ref uvOffset );
                    }
                    
                    if (sizeOverLifetimeEnable)
                    {
                        extent *= sizeOverLifetime.Evaluate(fraction);
                    }
                    
                    if (colorOverLifetimeEnable)
                    {
                        color *= colorOverLifetime.Evaluate(fraction);
                    }
                    
                    Vector3 pos = p.GetPos( SimulateSpace );
                    v.color = color;
                    
                    v.position = pos + quat * POS_TOP_LEFT * extent;
                    v.uv0 = uvOffset + uvTopLeft;
                    vh.AddVert(v);
                    
                    v.position = pos + quat * POS_TOP_RIGHT * extent;
                    v.uv0 = uvOffset + uvTopRight;
                    vh.AddVert(v);
                    
                    v.position = pos + quat * POS_BOTTOM_RIGHT * extent;
                    v.uv0 = uvOffset + uvBottomRight;
                    vh.AddVert(v);
                    
                    v.position = pos + quat * POS_BOTTOM_LEFT * extent;
                    v.uv0 = uvOffset + uvBottomLeft;
                    vh.AddVert(v);
                    
                    vh.AddTriangle(n + 0, n + 1, n + 2);
                    vh.AddTriangle(n + 2, n + 3, n + 0);
                    n += 4;
                }
            }
            else if( RenderMode == ParticleRenderMode.StretchBillboard )
            {
                for (int i = m_Particles.Count - 1; i >= 0; --i)
                {
                    Particle p = m_Particles[i];
                    
                    float extent = p.size;
                    float fraction = 1.0f - p.lifetime / p.startLifetime;
                    color = p.color;
                    
                    if (sizeOverLifetimeEnable)
                    {
                        extent *= sizeOverLifetime.Evaluate(fraction);
                    }
                    
                    Vector2 up;
                    Vector2 right;
                    
                    if (p.visualVelocity.sqrMagnitude > 0.0f)
                    {
                        float scale = m_StretchBillboard.CalcScale( p.StretchLengScl, p.velocity.magnitude );
                        
                        up = p.visualVelocity.normalized;
                        right = new Vector2(up.y * extent, -up.x * extent);
                        up *= extent * 2 * scale;
                    }
                    else
                    {
                        up = Vector2.up;
                        right = Vector2.right;
                    }
                    
                    if (textureSheetAnimationEnable)
                    {
                        textureSheetAnimation.OnUpdate( p, ref uvOffset );
                    }
                    
                    if (colorOverLifetimeEnable)
                    {
                        color *= colorOverLifetime.Evaluate(fraction);
                    }
                    
                    Vector3 pos = p.GetPos( SimulateSpace );
                    v.color = color;
                    
                    v.position.x = pos.x - right.x;
                    v.position.y = pos.y - right.y;
                    v.uv0 = uvOffset + uvBottomLeft;
                    vh.AddVert(v);
                    
                    v.position.x = pos.x + right.x;
                    v.position.y = pos.y + right.y;
                    v.uv0 = uvOffset + uvTopLeft;
                    vh.AddVert(v);
                    
                    v.position.x = pos.x + right.x - up.x;
                    v.position.y = pos.y + right.y - up.y;
                    v.uv0 = uvOffset + uvTopRight;
                    vh.AddVert(v);
                    
                    v.position.x = pos.x - right.x - up.x;
                    v.position.y = pos.y - right.y - up.y;
                    v.uv0 = uvOffset + uvBottomRight;
                    vh.AddVert(v);
                    
                    vh.AddTriangle(n + 0, n + 1, n + 2);
                    vh.AddTriangle(n + 2, n + 3, n + 0);
                    n += 4;
                }
            }
        }

        #if UNITY_EDITOR
        
        public void Simulate( float dt, bool isRestart )
        {
            if( isRestart )
            {
                ResetParticleSystem();
            }
            
            AdvanceTime( dt );
            
            if( isRestart )
            {
                AdvanceTime( dt );
            }
        }
        
        #endif
    }
}
