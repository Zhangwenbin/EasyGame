/**************************************************************************/
/*! @file   FastStringBuilder.cs
    @brief  高速文字列操作
***************************************************************************/
using UnityEngine;
using System.Collections.Generic;
using System.Text;
using System;
using System.Buffers;

namespace EG
{
    //=========================================================================
    //. 高速版 StringBuilder
    //=========================================================================
    public class FastStringBuilder
    {
        #pragma warning disable IDE0032
        #region フィールド

        private string                      m_StringGenerated   = "";           // 生成済み文字列
        private bool                        m_IsStringGenerated = false;        // 再生成が必要かどうか
        
        private char[]                      m_Buffer            = null;         // ワークバッファ
        private int                         m_BufferPos         = 0;            // ワークバッファ位置
        private int                         m_CharsCapacity     = 0;            // ワークバッファサイズ
        
        private List<char>                  m_Replacement       = null;         // Replace の一時文字列

        #endregion
        
        #region プロパティ

        public char[]                       RawBuffer => m_Buffer;              // ワークバッファ
        public int                          Length => m_BufferPos;              // ワークバッファ位置

        #endregion
        #pragma warning restore

        //=========================================================================
        //. 静的メソッド
        //=========================================================================
        #region 静的メソッド
        
        /// ***********************************************************************
        /// <summary>
        /// ビルダーの確保
        /// </summary>
        /// <param name="splitChar">分割文字</param>
        /// ***********************************************************************
        public static FastStringBuilder Alloc( )
        {
            FastStringBuilder builder = InstancePool<FastStringBuilder>.Shared.Rent() ?? new FastStringBuilder();
            return builder;
        }
        
        /// ***********************************************************************
        /// <summary>
        /// ビルダーの開放
        /// </summary>
        /// ***********************************************************************
        public static void Free( FastStringBuilder builder )
        {
            builder.Clear( );
            InstancePool<FastStringBuilder>.Shared.Return( builder );
        }
        
        /// ***********************************************************************
        /// <summary>
        /// Concat
        /// </summary>
        /// ***********************************************************************
        public static string Concat<T1, T2>(T1 a1, T2 a2)
        {
            var builder = Alloc();
            builder.AppendTyped(a1);
            builder.AppendTyped(a2);
            var s = builder.ToString();
            Free(builder);
            return s;
        }
        
        #endregion 静的メソッド
        
        //=========================================================================
        //. 初期化
        //=========================================================================
        #region 初期化
        
        /// ***********************************************************************
        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="initialCapacity">サイズ</param>
        /// ***********************************************************************
        public FastStringBuilder( int initialCapacity = 32 )
        {
            m_Buffer = ArrayPool<char>.Shared.Rent(NextPOT(initialCapacity));
            m_CharsCapacity = m_Buffer.Length;
        }
        
        /// ***********************************************************************
        /// <summary>
        /// クリア
        /// </summary>
        /// ***********************************************************************
        public FastStringBuilder Clear( )
        {
            m_BufferPos = 0;
            m_IsStringGenerated = false;
            return this;
        }
        
        #endregion 初期化
        
        //=========================================================================
        //. 確保
        //=========================================================================
        #region 確保
        
        /// ***********************************************************************
        /// <summary>
        /// バッファの確保
        /// </summary>
        /// ***********************************************************************
        private void ReallocateIFN( int nbCharsToAdd )
        {
            if( m_BufferPos + nbCharsToAdd > m_CharsCapacity )
            {
                var nextCapacity = Math.Max( m_CharsCapacity + nbCharsToAdd, NextPOT(m_CharsCapacity + 1) );
                char[] newChars = ArrayPool<char>.Shared.Rent(nextCapacity);
                m_CharsCapacity = newChars.Length;
                m_Buffer.CopyTo( newChars, 0 );
                ArrayPool<char>.Shared.Return(m_Buffer);
                m_Buffer = newChars;
            }
        }

        static int NextPOT(int n)
        {
            --n;
            while ((n & (n - 1)) != 0)
            {
                n &= n - 1;
            }
            return n << 1;
        }

        #endregion 確保
        
        //=========================================================================
        //. 設定/取得
        //=========================================================================
        #region 設定/取得

        /// ***********************************************************************
        /// <summary>
        /// 文字列を設定する( メモリを割り当てない )
        /// </summary>
        /// <param name="str">文字列</param>
        /// ***********************************************************************
        public void Set( string str )
        {
            // 将来の追加を管理するためにm_charsリストを埋めますが、最終的なstringGeneratedを直接設定します
            Clear( );
            Append( str );
            m_StringGenerated = str;
            m_IsStringGenerated = true;
        }

        /// ***********************************************************************
        /// <summary>
        /// 文字列の追加( メモリ割り当てなし )
        /// </summary>
        /// <param name="value">文字列</param>
        /// ***********************************************************************
        public FastStringBuilder AppendTop( string value )
        {
            if( string.IsNullOrEmpty( value ) ) return this;
            ReallocateIFN( value.Length );
            int n = value.Length;
            for( int i = m_BufferPos-1; i >= 0; --i )
            {
                m_Buffer[ n + i ] = m_Buffer[ i ];
            }
            for( int i = 0; i < n; i++ )
            {
                m_Buffer[ i ] = value[ i ];
            }
            m_BufferPos += n;
            m_IsStringGenerated = false;
            return this;
        }
        
        /// ***********************************************************************
        /// <summary>
        /// 文字の追加( メモリ割り当てなし )
        /// </summary>
        /// <param name="value">文字</param>
        /// ***********************************************************************
        public FastStringBuilder AppendTop( char value )
        {
            ReallocateIFN( 1 );
            for( int i = m_BufferPos-1; i >= 0; --i )
            {
                m_Buffer[ 1 + i ] = m_Buffer[ i ];
            }
            m_Buffer[ 0 ] = value;
            ++ m_BufferPos;
            m_IsStringGenerated = false;
            return this;
        }
        
        /// ***********************************************************************
        /// <summary>
        /// 文字列の追加( メモリ割り当てなし )
        /// </summary>
        /// <param name="value">文字列</param>
        /// ***********************************************************************
        public FastStringBuilder Append( string value )
        {
            ReallocateIFN( value.Length );
            int n = value.Length;
            for( int i = 0; i < n; i++ )
                m_Buffer[ m_BufferPos + i ] = value[ i ];
            m_BufferPos += n;
            m_IsStringGenerated = false;
            return this;
        }
        
        /// ***********************************************************************
        /// <summary>
        /// 文字の追加( メモリ割り当てなし )
        /// </summary>
        /// <param name="value">文字</param>
        /// ***********************************************************************
        public FastStringBuilder Append( char value )
        {
            ReallocateIFN( 1 );
            m_Buffer[ m_BufferPos ] = value;
            ++ m_BufferPos;
            m_IsStringGenerated = false;
            return this;
        }
        
        /// ***********************************************************************
        /// <summary>
        /// 文字列の追加( メモリ割り当てなし )
        /// </summary>
        /// <param name="value">文字列</param>
        /// ***********************************************************************
        public FastStringBuilder AppendTyped<T>(T value)
        {
            if (!Appenders.Initialized) return this;

            Appender<T>.Default.Append(this, value);
            return this;
        }

        #region generics appender

        class Appender<T>
        {
            internal static Appender<T> Default;
            internal virtual void Append(FastStringBuilder sb, T value)
            {
                throw new NotImplementedException($"Formatter<{typeof(T)}>::Append");
            }
        }

        static class Appenders
        {
            internal static bool Initialized = Initialize();

            static bool Initialize()
            {
                Appender<int>.Default = new I32();
                Appender<float>.Default = new F32();
                Appender<char>.Default = new C();
                Appender<string>.Default = new S();
                return true;
            }

            class I32 : Appender<int> { internal override void Append(FastStringBuilder sb, int v) => sb.Append(v); }
            class F32 : Appender<float> { internal override void Append(FastStringBuilder sb, float v) => sb.Append(v); }
            class C : Appender<char> { internal override void Append(FastStringBuilder sb, char v) => sb.Append(v); }
            class S : Appender<string> { internal override void Append(FastStringBuilder sb, string v) => sb.Append(v); }
        }
        
        #endregion

        /// ***********************************************************************
        /// <summary>
        /// INTの追加( メモリ割り当てなし )
        /// </summary>
        /// <param name="value">int値</param>
        /// ***********************************************************************
        public FastStringBuilder Append( int value )
        {
            m_IsStringGenerated = false;
            
            // 任意の整数を処理するのに十分なバッファが割り当てられていることを確認
            ReallocateIFN( 16 );
            
            // 負の値か？
            if( value < 0 )
            {
                value = -value;
                m_Buffer[ m_BufferPos++ ] = '-';
            }
            
            // 数字を逆順にコピー
            int nbChars = 0;
            do
            {
                m_Buffer[ m_BufferPos++ ] = (char)('0' + value%10);
                value /= 10;
                nbChars++;
            }
            while( value != 0 );
            
            // 結果を逆にする
            for( int i = nbChars/2-1; i >= 0; i-- )
            {
                char c = m_Buffer[ m_BufferPos-i-1 ];
                m_Buffer[ m_BufferPos-i-1 ] = m_Buffer[ m_BufferPos-nbChars+i ];
                m_Buffer[ m_BufferPos-nbChars+i ] = c;
            }
            
            return this;
        }

        /// ***********************************************************************
        /// <summary>
        /// INTの追加( メモリ割り当てなし )
        /// </summary>
        /// <param name="value">int値</param>
        /// ***********************************************************************
        public FastStringBuilder Append( int value, int width, char pad )
        {
            m_IsStringGenerated = false;
            
            // 任意の整数を処理するのに十分なバッファが割り当てられていることを確認
            ReallocateIFN( 16 + width );

            var sign = false;
            // 負の値か？
            if( value < 0 )
            {
                value = -value;
                sign = true;
            }
            
            // 数字を逆順にコピー
            int nbChars = 0;
            do
            {
                m_Buffer[ m_BufferPos++ ] = (char)('0' + value % 10);
                value /= 10;
                nbChars++;
            }
            while( value != 0 );
            
            if(sign)
            {
                m_Buffer[ m_BufferPos++ ] = '-';
                nbChars++;
            }

            // 足りない桁を詰める
            while (nbChars < width)
            {
                m_Buffer[ m_BufferPos++ ] = pad;
                nbChars++;
            }

            // 結果を逆にする
            for( int i = nbChars/2-1; i >= 0; i-- )
            {
                char c = m_Buffer[ m_BufferPos-i-1 ];
                m_Buffer[ m_BufferPos-i-1 ] = m_Buffer[ m_BufferPos-nbChars+i ];
                m_Buffer[ m_BufferPos-nbChars+i ] = c;
            }
            
            return this;
        }

        /// ***********************************************************************
        /// <summary>
        /// uintの追加( メモリ割り当てなし )
        /// </summary>
        /// <param name="value">uint値</param>
        /// ***********************************************************************
        public FastStringBuilder Append( uint value )
        {
            return Append((long)value);
        }
        
        /// ***********************************************************************
        /// <summary>
        /// longの追加( メモリ割り当てなし )
        /// </summary>
        /// <param name="value">long値</param>
        /// ***********************************************************************
        public FastStringBuilder Append( long value )
        {
            m_IsStringGenerated = false;
            
            // 任意の整数を処理するのに十分なバッファが割り当てられていることを確認
            ReallocateIFN( 32 );
            
            // 負の値か？
            if( value < 0 )
            {
                value = -value;
                m_Buffer[ m_BufferPos++ ] = '-';
            }
            
            // 数字を逆順にコピー
            int nbChars = 0;
            do
            {
                m_Buffer[ m_BufferPos++ ] = (char)('0' + value%10);
                value /= 10;
                nbChars++;
            }
            while( value != 0 );
            
            // 結果を逆にする
            for( int i = nbChars/2-1; i >= 0; i-- )
            {
                char c = m_Buffer[ m_BufferPos-i-1 ];
                m_Buffer[ m_BufferPos-i-1 ] = m_Buffer[ m_BufferPos-nbChars+i ];
                m_Buffer[ m_BufferPos-nbChars+i ] = c;
            }
            
            return this;
        }
        
        /// ***********************************************************************
        /// <summary>
        /// FLOATの追加( メモリ割り当てなし )
        /// </summary>
        /// <param name="valueF">float値</param>
        /// ***********************************************************************
        public FastStringBuilder Append( float valueF )
        {
            return Append( valueF, 7 );
        }

        public FastStringBuilder Append( float valueF, int digitMax )
        {
            double value = valueF;
            m_IsStringGenerated = false;
            
            // 浮動小数点数を処理するのに十分なバッファが割り当てられていることを確認
            ReallocateIFN( 32 );
            
            // 0 の場合
            if( value == 0 )
            {
                m_Buffer[ m_BufferPos++ ] = '0';
                return this;
            }
            
            // 負の値か？
            if( value < 0 )
            {
                value = -value;
                m_Buffer[ m_BufferPos++ ] = '-';
            }
            
            // 7桁をlongとして取得します
            int nbDecimals = 0;
            while( value < 1000000 )
            {
                value *= 10;
                nbDecimals++;
                if( nbDecimals >= digitMax ) break;
            }
            long valueLong = (long)System.Math.Round( value );
            
            // 逆順に解析
            int nbChars = 0;
            bool isLeadingZero = true;
            while( valueLong != 0 || nbDecimals >= 0 )
            {
                // 0または10進数以外の場合、先頭の0の削除を停止
                if( valueLong%10 != 0 || nbDecimals <= 0 )
                    isLeadingZero = false;
                
                // 最後の数字を書く（先行ゼロがない場合）
                if( !isLeadingZero )
                    m_Buffer[ m_BufferPos + (nbChars++) ] = (char)('0' + valueLong%10);
                
                // 小数点を追加
                if( --nbDecimals == 0 && !isLeadingZero )
                    m_Buffer[ m_BufferPos + (nbChars++) ] = '.';
                
                valueLong /= 10;
            }
            m_BufferPos += nbChars;
            
            // 結果を逆にする
            for( int i=nbChars/2-1; i>=0; i-- )
            {
                char c = m_Buffer[ m_BufferPos-i-1 ];
                m_Buffer[ m_BufferPos-i-1 ] = m_Buffer[ m_BufferPos-nbChars+i ];
                m_Buffer[ m_BufferPos-nbChars+i ] = c;
            }
            
            return this;
        }
        
        /// ***********************************************************************
        /// <summary>
        /// 置き換え
        /// </summary>
        /// <param name="oldStr">置き換え元</param>
        /// <param name="newStr">置き換え先</param>
        /// ***********************************************************************
        public FastStringBuilder Replace( string oldStr, string newStr )
        {
            if( m_BufferPos == 0 )
                return this;
            
            if( m_Replacement == null )
                m_Replacement = new List<char>();
            
            // 新しい文字列を作成します
            for( int i = 0; i < m_BufferPos; i++ )
            {
                bool isToReplace = false;
                
                // 最初の文字が見つかった場合、置き換える文字列の残りを確認
                if( m_Buffer[ i ] == oldStr[ 0 ] )
                {
                    int k=1;
                    while( k < oldStr.Length && m_Buffer[ i+k ] == oldStr[ k ] )
                        k++;
                    isToReplace = (k >= oldStr.Length);
                }
                
                // 交換する
                if( isToReplace )
                {
                    i += oldStr.Length-1;
                    if( newStr != null )
                        for( int k = 0; k < newStr.Length; k++ )
                            m_Replacement.Add( newStr[ k ] );
                }
                // 置換なし、古い文字をコピー
                else
                {
                    m_Replacement.Add( m_Buffer[ i ] );
                }
            }
            
            // 新しい文字列をm_charsにコピーして戻します
            ReallocateIFN( m_Replacement.Count - m_BufferPos );
            for( int k = 0; k < m_Replacement.Count; k++ )
                m_Buffer[ k ] = m_Replacement[ k ];
            
            m_BufferPos = m_Replacement.Count;
            m_Replacement.Clear();
            m_IsStringGenerated = false;
            return this;
        }
        
        /// ***********************************************************************
        /// <summary>
        /// 分割
        /// </summary>
        /// <param name="splitChar">分割文字</param>
        /// <param name="output">格納先</param>
        /// ***********************************************************************
        public int Split( char splitChar, ref string[] output )
        {
            if( output == null ) output = new string[ StringUtility.SPLIT_COL_NUM ];
            int count_of_delimiter = output.Length - 1;
            int count = 0;
            int start = 0;
            int i = 0;
            int max = m_BufferPos;
            
            // 区切り文字の位置を探す
            for( i = 0; i < max; i++ )
            {
                if( m_Buffer[i] == splitChar )
                {
                    output[ count ] = new string( m_Buffer, start, i - start );
                    if( count == count_of_delimiter )
                    {
                        return count + 1;
                    }
                    
                    start = i + 1;
                    ++ count;
                }
            }
            
            output[ count ] = new string( m_Buffer, start, i - start );
            
            return count + 1;
        }
        
        #endregion 設定/取得
        
        //=========================================================================
        //. 確認
        //=========================================================================
        #region 確認
        
        /// ***********************************************************************
        /// <summary>
        /// 空かどうか調べる
        /// </summary>
        /// ***********************************************************************
        public bool IsEmpty( )
        {
            return (m_IsStringGenerated ? (m_StringGenerated == null) : (m_BufferPos == 0));
        }
        
        /// ***********************************************************************
        /// <summary>
        /// 検索
        /// </summary>
        /// ***********************************************************************
        public  bool Contains( string str )
        {
            if( string.IsNullOrEmpty( str ) ) return false;
            
            for( int i = 0; i < m_BufferPos; i++ )
            {
                if( m_Buffer[i] == str[0] )
                {
                    if( str.Length > 1 )
                    {
                        if( i + str.Length > m_BufferPos )
                        {
                            return false;
                        }
                        
                        bool isMiss = false;
                        for( int j = 1; j < str.Length; ++j )
                        {
                            if( str[j] != m_Buffer[i+j] )
                            {
                                isMiss = true;
                                break;
                            }
                        }
                        if( isMiss ) continue;
                    }
                    
                    return true;
                }
            }
            
            return false;
        }
        
        #endregion 確認
        
        //=========================================================================
        //. その他
        //=========================================================================
        #region その他
        /// ***********************************************************************
        /// <summary>
        /// 文字列を返す
        /// </summary>
        /// ***********************************************************************
        public override string ToString( )
        {
            // 必要に応じて不変文字列を再生成
            if( !m_IsStringGenerated )
            {
                m_StringGenerated = WeakStringPool.Shared.GetOrAdd( m_Buffer, 0, m_BufferPos );
                m_IsStringGenerated = true;
            }
            return m_StringGenerated;
        }

        /// ***********************************************************************
        /// <summary>
        /// char[]
        /// </summary>
        /// ***********************************************************************
        public void WriteTo(char[] array)
        {
            if (array == null) throw new ArgumentNullException(nameof(array));
            if (array.Length < m_BufferPos) throw new ArgumentException(nameof(array));
            Buffer.BlockCopy(m_Buffer, 0, array, 0, m_BufferPos * 2);
        }

        /// ***********************************************************************
        /// <summary>
        /// ArraySegment<char>
        /// </summary>
        /// ***********************************************************************
        public ArraySegment<char> ToArraySegment()
        {
            return m_Buffer.Segment(0, m_BufferPos);
        }

        #endregion 
    }
}
