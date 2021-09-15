using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EG
{
    [System.Serializable]
    public class BitFlag
    {
        [SerializeField] protected int m_Value;
        
        public int flag
        {
            get { return m_Value; }
            set { m_Value = value; }
        }

        public BitFlag()
        {
            m_Value = 0;
        }

        public void Clear()
        {
            m_Value = 0;
        }

        public void CopyTo(BitFlag dst)
        {
            if (dst==null)
            {
                return;
            }

            dst.m_Value = m_Value;
        }

        public void SetValue(object flag, bool sw=true)
        {
            if (sw)
            {
                m_Value |= (1 << (int) flag);
            }
            else
            {
                m_Value &= ~(1 << (int) flag);
            }
        }
        public void SetValue( int flag, bool sw=true )
        {
            if( sw ) m_Value |= (1<<flag);
            else     m_Value &= ~(1<<flag);
        }
        public void ResetValue( object flag )
        {
            m_Value &= ~(1<<(int)flag);
        }

        public void ResetValue(int flag)
        {
            m_Value &= ~(1 << flag);
        }
        public bool HasValue( object flag )
        {
            return ( m_Value & (1<<(int)flag) ) != 0;
        }
        public bool HasValue( int flag )
        {
            return ( m_Value & (1<<flag) ) != 0;
        }
        public bool NotValue( object flag )
        {
            return !HasValue( flag );
        }
        public bool NotValue( int flag )
        {
            return !HasValue( flag );
        }
    }
    
    [System.Serializable]
    public class BitFlag<T> : BitFlag where T : struct, Enum
    {
        public BitFlag( )
        {
            m_Value = 0;
        }
        

        public void CopyTo( BitFlag<T> dst )
        {
            if( dst == null ) return;
            dst.m_Value = m_Value;
        }
        
        
        public void SetValue( T flag, bool sw )
        {
            int ivalue = CSharpHelpers.EnumSupport.ToInt32<T>( flag );
            if( sw ) m_Value |= (1<<ivalue);
            else     m_Value &= ~(1<<ivalue);
        }
        
        public void ResetValue( T flag )
        {
            int ivalue = CSharpHelpers.EnumSupport.ToInt32<T>( flag );
            m_Value &= ~(1<<ivalue);
        }
        
        public bool HasValue( T flag )
        {
            int ivalue = CSharpHelpers.EnumSupport.ToInt32<T>( flag );
            return ( m_Value & (1<<ivalue) ) != 0;
        }
        

        public bool NotValue( T flag )
        {
            return !HasValue( flag );
        }
        

        public override string ToString( )
        {
            string result = "";
            Array array = System.Enum.GetValues( typeof( T ) );
            for( int i = 0; i < array.Length; ++i )
            {
                int n = (int)array.GetValue( i );
                if( ( m_Value & (1 << n) ) != 0 )
                {
                    if( string.IsNullOrEmpty( result ) == false ) result += "|";
                    result += array.GetValue( i ).ToString( );
                }
            }
            return result;
        }
        
    }
}