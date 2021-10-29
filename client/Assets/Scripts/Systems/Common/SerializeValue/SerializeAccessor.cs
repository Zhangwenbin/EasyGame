#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace EG
{
    public class SerializeAccessor<T>
    {

        private SerializeValue  m_Value;
        private T               m_Default;

        public override string ToString( )
        {
            if( m_Value != null )
            {
                object obj = m_Value.v_Object;
                if( obj != null )
                {
                    return obj.ToString( );
                }
            }
            return base.ToString( );
        }
        
        public SerializeAccessor( )
        {
            m_Default = default(T);
        }
        
        public SerializeAccessor( T value )
        {
            m_Default = value;
        }
        
        public SerializeAccessor( SerializeValue value )
        {
            m_Value = value;
        }
        
        
        public void Reset( )
        {
            if( m_Value != null )
            {
                if( m_Default != null )
                {
                    SerializeValue.SetObject( m_Value, m_Default );
                }
            }
        }
        
        public void Attach( SerializeValue value )
        {
            m_Value = value;
            //Reset( );
        }
        
        public void Set( T value )
        {
            SerializeValue.SetObject( m_Value, value );
        }
        
        public T Get( )
        {
            if( m_Value != null )
            {
                object obj = m_Value.v_Object;
                if( obj != null )
                {
                    return (T)obj;
                }
            }
            return m_Default;
        }
    }
}
