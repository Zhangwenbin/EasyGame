using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

namespace EG
{
    public partial class UIParticleSystem : MaskableGraphic
    {

        public class ParticleAttribute : PropertyAttribute { }
        
        [System.Serializable]
        public struct ConeEmitter
        {
            public FloatRange Angle;
            public FloatRange Radius;
            public bool RandomDirection;
            
            public ConeEmitter( float angleMin, float angleMax )
            {
                Angle = new FloatRange( angleMin, angleMax );
                Radius = new FloatRange( 0, 0 );
                RandomDirection = false;
            }
            
            public void Calc( float emitterRotation, ref Vector3 pos, ref Vector3 vel )
            {
                vel = Quaternion.AngleAxis( Angle.Evaluate() + emitterRotation, Vector3.forward ) * Vector2.up;
                pos = vel.normalized * Radius.Evaluate();
                
                if( RandomDirection )
                {
                    vel = Quaternion.AngleAxis( Random.value * 360, Vector3.forward ) * Vector2.up;
                }
            }
        }
        
        [System.Serializable]
        public struct SphereEmitter
        {
            public FloatRange Radius;
            public bool Inverse;
            public bool RandomDirection;
            
            public SphereEmitter( float radiusMin, float radiusMax )
            {
                Radius = new FloatRange( radiusMin, radiusMax );
                Inverse = false;
                RandomDirection = false;
            }
            
            public void Calc( float emitterRotation, ref Vector3 pos, ref Vector3 vel )
            {
                if( RandomDirection )
                {
                    vel = Quaternion.AngleAxis( Random.value * 360, Vector3.forward ) * Vector2.up;
                    pos = Quaternion.AngleAxis( Random.value * 360, Vector3.forward ) * Vector2.up * Radius.Evaluate();
                }
                else
                {
                    vel = Quaternion.AngleAxis( Random.value * 360, Vector3.forward ) * Vector2.up;
                    pos = vel * Radius.Evaluate();
                }

                if( Inverse )
                {
                    vel = -vel;
                }
            }
        }
        
        [System.Serializable]
        public struct BoxEmitter
        {
            public float Width;
            public float Height;
            public bool RandomDirection;
            
            public BoxEmitter( float w, float h )
            {
                Width = w;
                Height = h;
                RandomDirection = false;
            }
            
            public void Calc( float emitterRotation, ref Vector3 pos, ref Vector3 vel )
            {
                if( RandomDirection )
                {
                    vel = Quaternion.AngleAxis( Random.value * 360, Vector3.forward ) * Vector2.up;
                }
                else
                {
                    vel = Quaternion.AngleAxis( emitterRotation, Vector3.forward ) * Vector2.up;
                }
                
                pos = new Vector3( ( Random.value - 0.5f ) * Width, ( Random.value - 0.5f ) * Height, 0f );
            }
        }
        
        [System.Serializable]
        public struct FloatRange
        {
            public float Min;
            public float Max;
            public float Evaluate()
            {
                return Mathf.Lerp( Min, Max, Random.value );
            }
            
            public FloatRange( float min, float max )
            {
                Min = min;
                Max = max;
            }
        }
        

        [System.Serializable]
        public struct Vector3Range
        {
            public Vector3 Min;
            public Vector3 Max;
            public Vector3 Evaluate()
            {
                Vector3 ret;
                ret.x = Mathf.Lerp( Min.x, Max.x, Random.value );
                ret.y = Mathf.Lerp( Min.y, Max.y, Random.value );
                ret.z = Mathf.Lerp( Min.z, Max.z, Random.value );
                return ret;
            }
            
            public Vector3Range( Vector3 min, Vector3 max )
            {
                Min = min;
                Max = max;
            }
        }
        

        [System.Serializable]
        public struct ColorRange
        {
            public Color Min;
            public Color Max;
            public Color Evaluate()
            {
                return Color.Lerp( Min, Max, Random.value );
            }
            
            public ColorRange( Color min, Color max )
            {
                Min = min;
                Max = max;
            }
        }
        
        // 
        [System.Serializable]
        public struct TextureSheetAnimation
        {
            public enum EMode
            {
                Grid,
            }
            
            public enum EAnimationRowType
            {
                WholeSheet,
                SingleRow
            }
            
            
            public EMode Mode;
            public Vector2Int Tiles;
            public AnimationCurve FrameOverTime;
            public EAnimationRowType Animation;
            public FloatRange StartFrame;
            public bool RandomRow;
            public int RowIdx;
            public int Cycles;
            
            // 
            float tw, th;
            int numTiles;
            
            // 
            public TextureSheetAnimation( int tx, int ty )
            {
                Mode = EMode.Grid;
                Tiles = new Vector2Int( tx, ty );
                FrameOverTime = new AnimationCurve( new Keyframe[] { new Keyframe( 0, 0 ), new Keyframe( 1, 1 ) } );
                Animation = EAnimationRowType.WholeSheet;
                StartFrame = new FloatRange( 0, 0 );
                RandomRow = false;
                RowIdx    = 0;
                Cycles = 1;
                
                tw = 1.0f;
                th = 1.0f;
                numTiles = 1;
            }
            
            // 
            public float GetStartFrame()
            {
                return StartFrame.Evaluate();
            }
            
            // 
            public void Prepare( ref Vector2 uvTL, ref Vector2 uvTR, ref Vector2 uvBL, ref Vector2 uvBR )
            {
                if( Tiles.x <= 0 )
                {
                    Tiles.x = 1;
                }
                tw = 1.0f / Tiles.x;
                
                if( Tiles.y <= 0 )
                {
                    Tiles.y = 1;
                }
                th = 1.0f / Tiles.y;
                
                uvTR.x = uvBR.x = tw;
                uvBL.y = uvBR.y = 1.0f - th;
                
                if( Animation == EAnimationRowType.WholeSheet )
                {
                    numTiles = Tiles.x * Tiles.y;
                }
                else
                {
                    numTiles = Tiles.x;
                }
                
                // RandomRow 
                if( RandomRow )
                {
                    RowIdx = 0;
                }
                else
                {
                    // 
                    RowIdx = Mathf.Clamp( RowIdx, 0, Tiles.y - 1 );
                }
            }
            
            // 
            public void OnUpdate( Particle particle, ref Vector2 uvOffset )
            {
                uvOffset = Vector2.zero;
                
                if( particle == null )
                    return;
                
                if( numTiles == 1 )
                    return;
                

                float frac = particle.Fraction * Cycles;
                if( frac > 1.0f )
                {
                    frac %= 1.0f;
                }
                
                float rate = FrameOverTime.Evaluate( frac );
                int   tileIdx = ( ( Mathf.FloorToInt( numTiles * rate ) + (int)particle.TexAnmStartFrame ) ) % numTiles;
                
                if( Tiles.x > 1 )
                {
                    uvOffset.x = ( tileIdx % Tiles.x ) * tw;
                }
                
                if( Tiles.y > 1 )
                {
                    int rowIdx = 0;
                    
                    if( Animation == EAnimationRowType.WholeSheet )
                    {
                        // 
                        rowIdx = tileIdx / Tiles.x;
                    }
                    else if( Animation == EAnimationRowType.SingleRow )
                    {
                        // 
                        rowIdx = ( RandomRow ) ? ( particle.randomSeed ) % Tiles.y : RowIdx;
                    }
                    
                    uvOffset.y = -rowIdx * th;
                }
                //DebugUtility.LogWarning( "frac : " + frac + ", rate :" + rate + ", tileIdx : " + tileIdx + ", ofs :" + uvOffset );
            }
        }
        
        // 
        [System.Serializable]
        public struct ParticleBurstPoint
        {
            public float Time;
            public int Count;
            
            public bool Check( float prev, float now )
            {
                return prev <= Time && Time < now;
            }
        }
        
        // 
        [System.Serializable]
        public struct ParticleBurst
        {
            public ParticleBurstPoint[] Points;
            public ParticleBurst( int n )
            {
                Points = new ParticleBurstPoint[ n ];
            }
        }
        
        // 
        [System.Serializable]
        public struct VelocityOverLifetime
        {
            public AnimationCurve X;
            public AnimationCurve Y;
            public float ScaleX;
            public float ScaleY;
            
            public VelocityOverLifetime( int n )
            {
                X = new AnimationCurve( new Keyframe[] { new Keyframe( 0, 0 ), new Keyframe( 1, 0 ) } );
                Y = new AnimationCurve( new Keyframe[] { new Keyframe( 0, 0 ), new Keyframe( 1, 0 ) } );
                ScaleX = 1;
                ScaleY = 1;
            }
            
            public float CalcX( float frac )
            {
                return X.Evaluate( frac ) * ScaleX;
            }
            
            public float CalcY( float frac )
            {
                return Y.Evaluate( frac ) * ScaleY;
            }
        }
        
        // 
        [System.Serializable]
        public struct LimitVelocityOverLifetime
        {
            public float Speed;         // 
            public float Dampen;        // 
            
            public LimitVelocityOverLifetime( int n )
            {
                Speed   = 1f;
                Dampen  = 0f;
            }
            
            public float Calc( float total, float magnitude )
            {
                if( magnitude < Mathf.Epsilon )
                    return 1.0f;
                
                if( Dampen == 0f )
                    return 1.0f;
                
                total *= ( 1.0f - Dampen );
                float result = magnitude * total;
                if( result < Speed )
                {
                    total = Speed / magnitude;
                }
                
                return total;
            }
        }
        
        // 
        [System.Serializable]
        public struct StretchBillboard
        {
            [ParticleAttribute]
            public FloatRange LengthScaleRange;
            
            public float SpeedScale;
            
            public StretchBillboard( float lengthScale, float speedScale )
            {
                SpeedScale = speedScale;
                LengthScaleRange = new FloatRange( lengthScale, lengthScale );
            }
            
            public float GetLength()
            {
                return LengthScaleRange.Evaluate();
            }
            
            // 
            public float CalcScale( float lengScl, float velocity )
            {
                return lengScl * ( 1.0f + velocity * SpeedScale );
            }
        }
    }
}
