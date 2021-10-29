using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace EG
{
    public static class MathUtility
    {
        #region 境界指定
        
        /// ***********************************************************************
        /// <param name="n">数值</param>
        /// <param name="align">2的幂</param>
        /// ***********************************************************************
        public static int RoundUp( int n, int align )
        {
            return (int)( ( n + (align-1) ) & ~(align-1) );
        }
        
        /// ***********************************************************************
        /// <summary>
        /// 切り上げ
        /// </summary>
        /// <param name="n">丸め対象の浮動小数点数</param>
        /// ***********************************************************************
        public static float RoundUp( float n )
        {
            //  10.0f -> 10.0f
            //  10.2f -> 11.0f
            //  10.7f -> 11.0f
            // -10.0f -> -10.0f
            // -10.2f -> -10.0f
            // -10.7f -> -10.0f
            return Mathf.Ceil( n );
        }
        public static int RoundUpToInt( float n )
        {
            //  10.0f -> 10.0f
            //  10.2f -> 11.0f
            //  10.7f -> 11.0f
            // -10.0f -> -10.0f
            // -10.2f -> -10.0f
            // -10.7f -> -10.0f
            return Mathf.CeilToInt( n );
        }
        
        /// ***********************************************************************
        /// <summary>
        /// 切り上げ
        /// </summary>
        /// <param name="n">丸め対象の倍精度浮動小数点数</param>
        /// ***********************************************************************
        public static double RoundUp( double n )
        {
            return System.Math.Ceiling( n );
        }
        
        /// ***********************************************************************
        /// <summary>
        /// 切り上げ
        /// </summary>
        /// <param name="n">丸め対象の浮動小数点数</param>
        /// <param name="digits">戻り値の有効桁数の精度</param>
        /// <remarks>
        /// 有効小数桁数が小数第 2 位になるように切り上げ
        /// float result = RoundUp( 12.328, 2 );
        /// result = 12.33
        /// </remarks>
        /// ***********************************************************************
        public static double RoundUp( double n, int digits )
        {
            double dCoef = System.Math.Pow( 10, digits );
            return n > 0 ? System.Math.Ceiling( n * dCoef ) / dCoef: System.Math.Floor( n * dCoef ) / dCoef;
        }
        
        /// ***********************************************************************
        /// <summary>
        /// 切り捨て
        /// </summary>
        /// <param name="n">数値</param>
        /// <param name="align">境界(２のべき乗)</param>
        /// ***********************************************************************
        public static int RoundDown( int n, int align )
        {
            return (int)( n & ~(align-1) );
        }
        
        /// ***********************************************************************
        /// <summary>
        /// 切り捨て
        /// </summary>
        /// <param name="n">丸め対象の浮動小数点数</param>
        /// ***********************************************************************
        public static float RoundDown( float n )
        {
            //  10.0f -> 10.0f
            //  10.2f -> 10.0f
            //  10.7f -> 10.0f
            // -10.0f -> -11.0f
            // -10.2f -> -11.0f
            // -10.7f -> -11.0f
            return Mathf.Floor( n );
        }
        public static int RoundDownToInt( float n )
        {
            //  10.0f -> 10.0f
            //  10.2f -> 10.0f
            //  10.7f -> 10.0f
            // -10.0f -> -11.0f
            // -10.2f -> -11.0f
            // -10.7f -> -11.0f
            return Mathf.FloorToInt( n );
        }
        
        /// ***********************************************************************
        /// <summary>
        /// 切り捨て
        /// </summary>
        /// <param name="n">丸め対象の倍精度浮動小数点数</param>
        /// ***********************************************************************
        public static double RoundDown( double n )
        {
            return System.Math.Floor( n );
        }
        
        /// ***********************************************************************
        /// <summary>
        /// 切り捨て
        /// </summary>
        /// <param name="n">丸め対象の浮動小数点数</param>
        /// <param name="digits">戻り値の有効桁数の精度</param>
        /// <remarks>
        /// 有効小数桁数が小数第 2 位になるように切り捨てる
        /// float result = RoundDown( 12.328, 2 );
        /// result = 12.32
        /// </remarks>
        /// ***********************************************************************
        public static double RoundDown( double n, int digits )
        {
            double dCoef = System.Math.Pow( 10, digits );
            return n > 0 ? System.Math.Floor( n * dCoef ) / dCoef: System.Math.Ceiling( n * dCoef ) / dCoef;
        }
        
        #endregion 境界指定
        
        //=========================================================================
        //. 汎用
        //=========================================================================
        #region 汎用
        
        /// ***********************************************************************
        /// <summary>
        /// 丸め
        /// </summary>
        /// <param name="value">値</param>
        /// <param name="min">最小値</param>
        /// <param name="max">最大値</param>
        /// ***********************************************************************
        public static int Clamp( int value, int min, int max )
        {
            return value < min ? min: value > max ? max: value;
        }
        
        /// ***********************************************************************
        /// <summary>
        /// 丸め
        /// </summary>
        /// <param name="value">値</param>
        /// <param name="min">最小値</param>
        /// <param name="max">最大値</param>
        /// ***********************************************************************
        public static float Clamp( float value, float min, float max )
        {
            return value < min ? min: value > max ? max: value;
        }
        
        /// ***********************************************************************
        /// <summary>
        /// 繰り返し
        /// </summary>
        /// <param name="value">値</param>
        /// <param name="min">最小値</param>
        /// <param name="max">最大値</param>
        /// ***********************************************************************
        public static int Repeat( int value, int min, int max )
        {
            int offset = min;
            
            min     = 0;
            max     -= offset;
            value   -= offset;
            
            if( value < min )
            {
                value %= max;
                if( value < 0 ) value += max;
            }
            else if( value >= max )
            {
                value %= max;
            }
            
            return value + offset;
        }
        
        /// ***********************************************************************
        /// <summary>
        /// 繰り返し
        /// </summary>
        /// <param name="value">値</param>
        /// <param name="min">最小値</param>
        /// <param name="max">最大値</param>
        /// ***********************************************************************
        public static float Repeat( float value, float min, float max )
        {
            float offset = min;
            
            min     = 0;
            max     -= offset;
            value   -= offset;
            
            if( value < min )
            {
                value %= max;
                if( value < 0 ) value += max;
            }
            else if( value >= max )
            {
                value %= max;
            }
            
            return value + offset;
        }
        
        #endregion
        
        //=========================================================================
        //. 角度
        //=========================================================================
        #region 角度
        
        /// ***********************************************************************
        /// <summary>
        /// 回転角の差を縮める
        /// </summary>
        /// <param name="rot">現在の回転角</param>
        /// <param name="target">目標の回転角</param>
        /// <param name="rate">2点間の差の rate 倍の補間量で補間する(0.0f < rate < 1.0f)</param>
        /// <param name="min">最小補間量(少なくともこの補間量で補間を行う)</param>
        /// <param name="max">最大補間量(これ以上の補間量にはならない)</param>
        /// <returns>補完後の角度</returns>
        /// ***********************************************************************
        public static float InterpEuler( float rot, float target, float rate, float min, float max )
        {
            if( rot < 0.0f )            rot += 360.0f;
            else if( rot >= 360.0f )    rot -= 360.0f;
            if( target < 0.0f )         target += 360.0f;
            else if( target >= 360.0f ) target -= 360.0f;
            
            // 差角
            float f0 = target - rot;
            if( f0 > 180.0f )           f0 -= 360.0f;
            else if( f0 < -180.0f )     f0 += 360.0f;
            
            // 変化量
            f0 *= rate;
            float abs = Mathf.Abs( f0 );
            if( abs < min )
            {
                if( f0 < 0 )
                {
                    f0 = -min;
                }
                else
                {
                    f0 = min;
                }
            }
            else if( abs > max )
            {
                if( f0 < 0 )
                {
                    f0 = -max;
                }
                else
                {
                    f0 = max;
                }
            }
            
            // 加算
            f0 = rot + f0;
            //if( f0 >= 180.0f )     f0 -= 360.0f;
            //else if( f0 < 180.0f ) f0 += 360.0f;
            
            // 0 - 360 におさめて返す
            if( f0 >= 360.0f )      f0 -= 360.0f;
            else if( f0 < 0.0f )    f0 += 360.0f;
            
            return f0;
        }
        
        /// ***********************************************************************
        /// <summary>
        /// 角度幅を計算する(オイラー角)
        /// </summary>
        /// ***********************************************************************
        public static float CalcRotRangeEuler( float a, float b )
        {
            float n = Mathf.Abs( a - b );
            if( n > 180 )
            {
                n = 360 - n;
            }
            return n;
        }
        
        #endregion
        
        //=========================================================================
        //. 距離
        //=========================================================================
        #region 距離
        
        /// ***********************************************************************
        /// <summary>
        /// 2点間のXZ軸での距離を計算する
        /// </summary>
        /// ***********************************************************************
        public static float CalcDistance( Vector3 a, Vector3 b )
        {
            return ( a - b ).magnitude;
        }
        
        /// ***********************************************************************
        /// <summary>
        /// 2点間のXZ軸での距離を計算する
        /// </summary>
        /// ***********************************************************************
        public static float CalcDistanceXZ( Vector3 a, Vector3 b )
        {
            a = a - b;
            a.y = 0;
            return a.magnitude;
        }
        
        /// ***********************************************************************
        /// <summary>
        /// XZ軸での距離を計算する
        /// </summary>
        /// ***********************************************************************
        public static float CalcDistanceXZ( Vector3 a )
        {
            a.y = 0;
            return a.magnitude;
        }
        
        /// ***********************************************************************
        /// <summary>
        /// 2点間の距離を計算する
        /// </summary>
        /// ***********************************************************************
        public static int CalcDistance( int x1, int y1, int x2, int y2 )
        {
            return Math.Abs( x1 - x2 ) + Math.Abs( y1 - y2 );
        }
        
        /// ***********************************************************************
        /// <summary>
        /// 2点間の距離を計算する
        /// </summary>
        /// ***********************************************************************
        public static int CalcDistance( Vector2Int p1, Vector2Int p2 )
        {
            return Math.Abs( p1.x - p2.x ) + Math.Abs( p1.y - p2.y );
        }
        
        #endregion
        
        //=========================================================================
        //. 方向
        //=========================================================================
        #region 方向
        
        /// ***********************************************************************
        /// <summary>
        /// p1からp2への向きを取得
        /// </summary>
        /// ***********************************************************************
        public static Vector2Int CalcDirection( int p1X, int p1Y, int p2X, int p2Y )
        {
            int dx = p2X - p1X;
            int dy = p2Y - p1Y;
            int ax = Math.Abs( dx );
            int ay = Math.Abs( dy );
            if( ax > ay )
            {
                if( dx < 0 ) return Vector2Int.left;
                if( dx > 0 ) return Vector2Int.right;
            }
            if( ax < ay )
            {
                if( dy < 0 ) return Vector2Int.down;
                if( dy > 0 ) return Vector2Int.up;
            }
            if( dx > 0 ) return Vector2Int.right;
            if( dx < 0 ) return Vector2Int.left;
            if( dy > 0 ) return Vector2Int.up;
            if( dy < 0 ) return Vector2Int.down;
            return Vector2Int.up;
        }
        
        /// ***********************************************************************
        /// <summary>
        /// p1からp2への向きを取得
        /// </summary>
        /// ***********************************************************************
        public static Vector2Int CalcDirection( Vector2Int p1, Vector2Int p2 )
        {
            return CalcDirection( p1.x, p1.y, p2.x, p2.y );
        }
        
        /// ***********************************************************************
        /// <summary>
        /// 向きを計算する
        /// </summary>
        /// ***********************************************************************
        public static Vector2Int CalcDirection( float x, float y )
        {
            float r = (Mathf.Rad2Deg * Mathf.Atan2(y, x) + 360.0f) % 360.0f;
            
            if( 325 <= r || r < 45 )
            {
                return Vector2Int.right;
            }
            else if( 45 <= r && r < 135 )
            {
                return Vector2Int.up;
            }
            else if( 135 <= r && r < 225 )
            {
                return Vector2Int.left;
            }
            else
            {
                return Vector2Int.down;
            }
        }
        
        #endregion
        
        //=========================================================================
        //. 固定少数(簡易的な)
        //=========================================================================
        #region 固定少数(簡易的な)

        /// ***********************************************************************
        /// <summary>
        /// floatから固定少数への変換
        /// </summary>
        /// ***********************************************************************
        public static int GetFixInt( int value )
        {
            return value * 1000;
        }
        
        /// ***********************************************************************
        /// <summary>
        /// floatから固定少数への変換
        /// </summary>
        /// ***********************************************************************
        public static int GetFixInt( float value )
        {
            return Mathf.FloorToInt( value * 1000 );
        }
        
        /// ***********************************************************************
        /// <summary>
        /// Vector3から固定称す数Vector3への変換
        /// </summary>
        /// ***********************************************************************
        public static Vector3Int GetFixVector3( Vector3 value )
        {
            return new Vector3Int( GetFixInt( value.x ), GetFixInt( value.y ), GetFixInt( value.z ) );
        }
        
        /// ***********************************************************************
        /// <summary>
        /// 固定少数からからfloatへの変換
        /// </summary>
        /// ***********************************************************************
        public static float GetFloatFromFix( int value )
        {
            return (float)value * 0.001f;
        }
        
        /// ***********************************************************************
        /// <summary>
        /// 固定少数Vector3からVector3への変換
        /// </summary>
        /// ***********************************************************************
        public static Vector3 GetVector3FromFix( Vector3Int value )
        {
            return new Vector3( GetFloatFromFix( value.x ), GetFloatFromFix( value.y ), GetFloatFromFix( value.z ) );
        }
        
        /// ***********************************************************************
        /// <summary>
        /// 固定少数同士の乗算
        /// </summary>
        /// ***********************************************************************
        public static int MulFix( int a, int b )
        {
            return a * b / 1000;
        }
        
        /// ***********************************************************************
        /// <summary>
        /// 固定少数同士の除算
        /// </summary>
        /// ***********************************************************************
        public static int DivFix( int a, int b )
        {
            return a * 1000 / b;
        }
        
        /// ***********************************************************************
        /// <summary>
        /// 固定少数の平方根
        /// </summary>
        /// ***********************************************************************
        public static int SqrtFix( int value )
        {
            return GetFixInt( Mathf.Sqrt( GetFloatFromFix( value ) ) );
        }
        
        /// ***********************************************************************
        /// <summary>
        /// 固定少数のアークタンジェント
        /// </summary>
        /// ***********************************************************************
        public static int ATanFix( int value )
        {
            return GetFixInt( Mathf.Atan( GetFloatFromFix( value ) ) );
        }
        
        /// ***********************************************************************
        /// <summary>
        /// 固定少数のSIN
        /// </summary>
        /// ***********************************************************************
        public static int SinFix( int value )
        {
            return GetFixInt( Mathf.Sin( GetFloatFromFix( value ) ) );
        }
        
        /// ***********************************************************************
        /// <summary>
        /// 固定少数のCOS
        /// </summary>
        /// ***********************************************************************
        public static int CosFix( int value )
        {
            return GetFixInt( Mathf.Cos( GetFloatFromFix( value ) ) );
        }
        
        /// ***********************************************************************
        /// <summary>
        /// 固定少数の乗数
        /// </summary>
        /// ***********************************************************************
        public static int PowFix( int value, int t )
        {
            return GetFixInt( Mathf.Pow( GetFloatFromFix( value ), GetFloatFromFix( t ) ) );
        }
        
        #endregion
        
        //=========================================================================
        //. 線分演算関係
        //=========================================================================
        #region 線分演算関係
        
        /// ***********************************************************************
        /// <summary>
        /// 点1から点2への直線経路の取得
        /// </summary>
        /// <param name="p1">P1</param>
        /// <param name="p2">P2</param>
        /// ***********************************************************************
        //public static List<Vector2Int> CalcFixLineRoot( Vector2Int p1, Vector2Int p2, bool inStart = true /*始点を含めるかどうか*/ )
        //{
        //    List<Vector2Int> result = new List<Vector2Int>( );
            
        //    if( p1.x == p2.x && p1.y == p2.y )
        //    {
        //        result.Add( p1 );
        //        return result;
        //    }
            
        //    // X軸に平行な線
        //    if( p1.y == p2.y )
        //    {
        //        if( p1.x < p2.x )
        //        {
        //            for( int x = p1.x; x <= p2.x; ++x )
        //            {
        //                result.Add( new Vector2Int( x, p1.y ) );
        //            }
        //        }
        //        else
        //        {
        //            for( int x = p1.x; x >= p2.x; --x )
        //            {
        //                result.Add( new Vector2Int( x, p1.y ) );
        //            }
        //        }
        //    }
        //    // Y軸に平行な線
        //    else if( p1.x == p2.x )
        //    {
        //        if( p1.y < p2.y )
        //        {
        //            for( int y = p1.y; y <= p2.y; ++y )
        //            {
        //                result.Add( new Vector2Int( p1.x, y ) );
        //            }
        //        }
        //        else
        //        {
        //            for( int y = p1.y; y >= p2.y; --y )
        //            {
        //                result.Add( new Vector2Int( p1.x, y ) );
        //            }
        //        }
        //    }
        //    // 斜めの線
        //    else
        //    {
        //        // 係数
        //        int min = GetFixInt( 0.01f );       // ここは変更しないように
        //        int max = GetFixInt( 0.75f );       // ここ上げるととラインが太くなる
                
        //        // 直線の方程式を解く( y = a * x + b );
        //        int x1 = GetFixInt( p1.x );
        //        int y1 = GetFixInt( p1.y );
        //        int x2 = GetFixInt( p2.x );
        //        int y2 = GetFixInt( p2.y );
        //        int a = DivFix( y2 - y1, x2 - x1 );
        //        int b = DivFix( MulFix( x2, y1 ) - MulFix( x1, y2 ), x2 - x1 );
                
        //        // X 軸方向の計算
        //        if( p1.x < p2.x )
        //        {
        //            for( int x = p1.x; x <= p2.x; ++x )
        //            {
        //                int fix_y = MulFix( a, GetFixInt( x ) ) + b;
        //                int i1 = MathUtility.RoundDownToInt( GetFloatFromFix( fix_y + min ) );
        //                int i2 = MathUtility.RoundDownToInt( GetFloatFromFix( fix_y + max ) );
                    
        //                Vector2Int pos = new Vector2Int( x, i1 );
        //                if( result.FindIndex( ( prop ) => prop.x == pos.x & prop.y == pos.y ) == -1 )
        //                {
        //                    result.Add( pos );
        //                    if( pos.x == p2.x && pos.y == p2.y ) break;
        //                }
                    
        //                pos = new Vector2Int( x, i2 );
        //                if( result.FindIndex( ( prop ) => prop.x == pos.x & prop.y == pos.y ) == -1 )
        //                {
        //                    result.Add( pos );
        //                    if( pos.x == p2.x && pos.y == p2.y ) break;
        //                }
        //            }
        //        }
        //        else
        //        {
        //            for( int x = p1.x; x >= p2.x; --x )
        //            {
        //                int fix_y = MulFix( a, GetFixInt( x ) ) + b;
        //                int i1 = MathUtility.RoundDownToInt( GetFloatFromFix( fix_y + min ) );
        //                int i2 = MathUtility.RoundDownToInt( GetFloatFromFix( fix_y + max ) );
                    
        //                Vector2Int pos = new Vector2Int( x, i1 );
        //                if( result.FindIndex( ( prop ) => prop.x == pos.x & prop.y == pos.y ) == -1 )
        //                {
        //                    result.Add( pos );
        //                    if( pos.x == p2.x && pos.y == p2.y ) break;
        //                }
                    
        //                pos = new Vector2Int( x, i2 );
        //                if( result.FindIndex( ( prop ) => prop.x == pos.x & prop.y == pos.y ) == -1 )
        //                {
        //                    result.Add( pos );
        //                    if( pos.x == p2.x && pos.y == p2.y ) break;
        //                }
        //            }
        //        }
                
        //        // Y 軸方向の計算
        //        if( p1.y < p2.y )
        //        {
        //            for( int y = p1.y; y <= p2.y; ++y )
        //            {
        //                int fix_x = DivFix( GetFixInt( y ) - b, a );
        //                int i1 = MathUtility.RoundDownToInt( GetFloatFromFix( fix_x + min ) );
        //                int i2 = MathUtility.RoundDownToInt( GetFloatFromFix( fix_x + max ) );
                    
        //                Vector2Int pos = new Vector2Int( i1, y );
        //                if( result.FindIndex( ( prop ) => prop.x == pos.x & prop.y == pos.y ) == -1 )
        //                {
        //                    result.Add( pos );
        //                    if( pos.x == p2.x && pos.y == p2.y ) break;
        //                }
                    
        //                pos = new Vector2Int( i2, y );
        //                if( result.FindIndex( ( prop ) => prop.x == pos.x & prop.y == pos.y ) == -1 )
        //                {
        //                    result.Add( pos );
        //                    if( pos.x == p2.x && pos.y == p2.y ) break;
        //                }
        //            }
        //        }
        //        else
        //        {
        //            for( int y = p1.y; y >= p2.y; --y )
        //            {
        //                int fix_x = DivFix( GetFixInt( y ) - b, a );
        //                int i1 = MathUtility.RoundDownToInt( GetFloatFromFix( fix_x + min ) );
        //                int i2 = MathUtility.RoundDownToInt( GetFloatFromFix( fix_x + max ) );
                    
        //                Vector2Int pos = new Vector2Int( i1, y );
        //                if( result.FindIndex( ( prop ) => prop.x == pos.x & prop.y == pos.y ) == -1 )
        //                {
        //                    result.Add( pos );
        //                    if( pos.x == p2.x && pos.y == p2.y ) break;
        //                }
                    
        //                pos = new Vector2Int( i2, y );
        //                if( result.FindIndex( ( prop ) => prop.x == pos.x & prop.y == pos.y ) == -1 )
        //                {
        //                    result.Add( pos );
        //                    if( pos.x == p2.x && pos.y == p2.y ) break;
        //                }
        //            }
        //        }
        //    }
            
        //    if( inStart == false )
        //    {
        //        int index = result.FindIndex( ( prop ) => prop.x == p1.x && prop.y == p1.y );
        //        if( index != -1 ) result.RemoveAt( index );
        //    }
            
        //    return result;
        //}
        //public static List<Vector2Int> CalcFixLineRoot( Vector3 p1, Vector3 p2, bool inStart = true /*始点を含めるかどうか*/ )
        //{
        //    Vector2Int start = new Vector2Int( MathUtility.RoundDownToInt( p1.x ), MathUtility.RoundDownToInt( p1.z ) );
        //    Vector2Int end   = new Vector2Int( MathUtility.RoundDownToInt( p2.x ), MathUtility.RoundDownToInt( p2.z ) );
        //    return CalcFixLineRoot( start, end, inStart );
        //}
        
        /// ***********************************************************************
        /// <summary>
        /// 点1から指定方向へ指定距離進む場合の直線経路を取得
        /// </summary>
        /// <param name="p1">始点</param>
        /// <param name="dir">正規化された方向</param>
        /// <param name="maxDist">距離</param>
        /// ***********************************************************************
        public static List<Vector2Int> CalcFixLineRoot( Vector2 p1, Vector2 p2, bool inStart = true /*始点を含めるかどうか*/ )
        {
            Vector2Int  start   = new Vector2Int( Mathf.FloorToInt( p1.x ), Mathf.FloorToInt( p1.y ) );
            Vector2Int  end     = new Vector2Int( Mathf.FloorToInt( p2.x ), Mathf.FloorToInt( p2.y ) );
            Vector2     dir     = p2 - p1;
            Vector2Int  ipos    = start;
            Vector2     pos     = p1;
            List<Vector2Int> result = new List<Vector2Int>( );
            
            int maxDist = Math.Abs( start.x - end.x ) +  Math.Abs( start.y - end.y );
            int dist = 0;
            dir = dir.normalized * 0.2f;
            
            result.Add( ipos );
            
            do
            {
                pos += dir;
                
                ipos.x = Mathf.FloorToInt( pos.x );
                ipos.y = Mathf.FloorToInt( pos.y );
                
                int index = result.FindIndex( ( prop ) => prop == ipos );
                if( index == -1 )
                {
                    result.Add( new Vector2Int( ipos.x, ipos.y ) );
                }
                
                dist = Math.Abs( start.x - ipos.x ) +  Math.Abs( start.y - ipos.y );
            }
            while( ipos != end && dist <= maxDist );
            
            if( inStart == false )
            {
                int index = result.FindIndex( ( prop ) => prop == start );
                if( index != -1 ) result.RemoveAt( index );
            }
            
            return result;
        }
        public static List<Vector2Int> CalcFixLineRoot( Vector3 p1, Vector3 p2, bool inStart = true )
        {
            return CalcFixLineRoot( new Vector2( p1.x, p1.z ), new Vector2( p2.x, p2.z ), inStart );
        }
        
        /// ***********************************************************************
        /// <summary>
        /// 点Pから直線へ垂直に落とした点を取得
        /// </summary>
        /// <param name="intersection">交点の格納先</param>
        /// <param name="p">点P</param>
        /// <param name="v1">線分の開始点</param>
        /// <param name="v2">線分の終了点</param>
        /// <returns>交差していたら true を返します</returns>
        /// ***********************************************************************
        public static bool CalcLineVertical( out Vector3 intersection, out float t, ref Vector3 p, ref Vector3 v1, ref Vector3 v2 )
        {
            Vector3 sub = v2 - v1;
        
            // 垂線の足が２頂点間にあるかは t を調べます
            float it = ( Vector3.Dot( p, sub ) - Vector3.Dot( v1, sub ) ) / Vector3.Dot ( sub, sub );
            
            t = it;
            
            // 点から直線へ垂直に落とした点は直線内には存在しない
            if( it > 1.0f )
            {
                intersection = v2;
            }
            else if( it < 0.0f )
            {
                intersection = v1;
            }
            else
            {
                // t を元に交点を取得
                intersection.x = v1.x + sub.x * it;
                intersection.y = v1.y + sub.y * it;
                intersection.z = v1.z + sub.z * it;
                
                return true;
            }
            
            // 交点は線分内に存在しない
            return false;
        }
        
        /// ***********************************************************************
        /// <summary>
        /// 点Pから直線へ垂直に落とした点を取得
        /// </summary>
        /// <param name="intersection">交点の格納先</param>
        /// <param name="p">点P</param>
        /// <param name="v1">線分の開始点</param>
        /// <param name="v2">線分の終了点</param>
        /// <returns>交差していたら true を返します</returns>
        /// ***********************************************************************
        public static bool IsLineVertical( ref Vector3 p, ref Vector3 v1, ref Vector3 v2 )
        {
            Vector3 sub = v2 - v1;
            
            // 垂線の足が２頂点間にあるかは t を調べます
            float t = ( Vector3.Dot( p, sub ) - Vector3.Dot( v1, sub ) ) / Vector3.Dot ( sub, sub );
            
            // 点から直線へ垂直に落とした点は直線内には存在しない
            if( t >= 0.0f && t <= 1.0f )
            {
                return true;
            }
            
            // 交点は線分内に存在しない
            return false;
        }
        
        /// ***********************************************************************
        /// <summary>
        /// 線分と球の接触判定
        /// </summary>
        /// <param name="vct">線分の正規化されたベクトル</param>
        /// <param name="norm">線分の法線</param>
        /// <param name="intersection">接触点の格納先</param>
        /// <param name="start">線分の開始点</param>
        /// <param name="end">線分の終了点</param>
        /// <param name="p">球の中心点</param>
        /// <param name="r">球の半径</param>
        /// <returns>接触していたら true を返します</returns>
        /// ***********************************************************************
        public static bool IsLineHit( ref Vector3 start, ref Vector3 end, ref Vector3 p, float r )
        {
            Vector3 vct         = ( end - start ).normalized;
            Vector3 R           = start - p;
            Vector3 C           = R - Vector3.Dot( R, vct ) * vct;
            float   Omega       = C.sqrMagnitude - ( r * r );
            
            // 現在の位置でめり込んでいるかチェック
            if( Omega < -0.01f )
            {
                // 点Pから直線へ垂直に落とした点を取得
                if( IsLineVertical( ref p, ref start, ref end ) )
                {
                    return true;
                }
                // 端点とのチェック
                else if( IsSphereHit( ref p, r, ref start, 0.0f ) )
                {
                    return true;
                }
                else if( IsSphereHit( ref p, r, ref end, 0.0f ) )
                {
                    return true;
                }
                
                return false;
            }
            
            return false;
        }
        
        #endregion 線分演算関係
        
        //=========================================================================
        //. 球演算関係
        //=========================================================================
        #region 球演算関係
        
        /// ***********************************************************************
        /// <summary>
        /// 球同士の接触判定
        /// </summary>
        /// <param name="p1">球１の中心点</param>
        /// <param name="r1">球１の半径</param>
        /// <param name="p2">球２の中心点</param>
        /// <param name="r2">球２の半径</param>
        /// <returns>接触していたら true を返します</returns>
        /// ***********************************************************************
        public static bool IsSphereHit( ref Vector3 p1, float r1, ref Vector3 p2, float r2 )
        {
            float r     = r1 + r2;
            float rr    = r * r;
            
            Vector3 sub = p1 - p2;
            if( sub.sqrMagnitude < rr - 0.01f )
            {
                return true;
            }
            
            return false;
        }
        
        #endregion 球演算関係
        
        //=========================================================================
        //. レイ演算関係
        //=========================================================================
        #region レイ演算関係
        
        /// ***********************************************************************
        /// <summary>
        /// レイと面の接触判定
        /// </summary>
        /// <returns>接触していたら true を返します</returns>
        /// ***********************************************************************
        public static bool IsPlaneHit( ref Ray ray, ref Vector3 p, ref Plane plane )
        {
            float t = Vector3.Dot( ray.direction, plane.normal );
            if( t == 0 ) return false; // 直線と面は平行
            
            // t <= 0.0f 平面は直線の後ろにある
            t = -( Vector3.Dot( ray.origin, plane.normal ) / t );
            
            // 交点
            p = ray.origin + t * ray.direction;
            
            return true;
        }
        
        #endregion レイ演算関係
    }
}
