using UnityEngine;
using System.Collections;

namespace EG
{
    public enum ECurveType
    {
        None                ,
        Linear              ,
        CLeap               ,
        Spring              ,
        InSin               ,
        OutSin              ,
        InOutSin            ,
    }
    
    public static class CurveUtility
    {
        public static float Sample( ECurveType type, float start, float end, float value, AnimationCurve curve = null )
        {
            switch( type )
            {
            case ECurveType.Linear:     return Linear( start, end, value );
            case ECurveType.CLeap:      return CLerp( start, end, value );
            case ECurveType.Spring:     return Spring( start, end, value );
            case ECurveType.InSin:      return InSin( start, end, value );
            case ECurveType.OutSin:     return OutSin( start, end, value );
            case ECurveType.InOutSin:   return InOutSin( start, end, value );
            default:
                if( curve != null ) return start + ( end - start ) * curve.Evaluate( value );
                break;
            }
            return 0.0f;
        }
        
        public static float Sample( ECurveType type, float start, float end, float value )
        {
            switch( type )
            {
            case ECurveType.Linear:     return Linear( start, end, value );
            case ECurveType.CLeap:      return CLerp( start, end, value );
            case ECurveType.Spring:     return Spring( start, end, value );
            case ECurveType.InSin:      return InSin( start, end, value );
            case ECurveType.OutSin:     return OutSin( start, end, value );
            case ECurveType.InOutSin:   return InOutSin( start, end, value );
            }
            return 0.0f;
        }
        
        public static float Linear( float start, float end, float value )
        {
            return Mathf.Lerp( start, end, value );
        }
        
        public static float CLerp( float start, float end, float value )
        {
            float min = 0.0f;
            float max = 360.0f;
            float half = Mathf.Abs((max - min) / 2.0f);
            float retval = 0.0f;
            float diff = 0.0f;
            if ((end - start) < -half){
                diff = ((max - start) + end) * value;
                retval = start + diff;
            }else if ((end - start) > half){
                diff = -((max - end) + start) * value;
                retval = start + diff;
            }else retval = start + (end - start) * value;
            return retval;
        }
        
        public static float Spring( float start, float end, float value )
        {
            value = Mathf.Clamp01(value);
            value = (Mathf.Sin(value * Mathf.PI * (0.2f + 2.5f * value * value * value)) * Mathf.Pow(1f - value, 2.2f) + value) * (1f + (1.2f * (1f - value)));
            return start + (end - start) * value;
        }
        
    
        public static float InSin( float start, float end, float value )
        {
            end -= start;
            return -end * Mathf.Cos( value * ( Mathf.PI * 0.5f ) ) + end + start;
        }
        
        public static float OutSin( float start, float end, float value )
        {
            end -= start;
            return end * Mathf.Sin( value * ( Mathf.PI * 0.5f ) ) + start;
        }
        
        public static float InOutSin( float start, float end, float value )
        {
            end -= start;
            return -end * 0.5f * ( Mathf.Cos( Mathf.PI * value ) - 1 ) + start;
        }
        
    }
}