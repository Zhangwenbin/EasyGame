using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;

namespace EG
{
    [AddComponentMenu("Scripts/System/Common/MonoEvent")]
    public class MonoEvent : UnityEngine.MonoBehaviour
    {
        public delegate void Action( MonoEvent obj );
        
        protected SerializeValueList                m_ValueList     = new SerializeValueList( );
        protected List<Action>                      m_Actions       = null;

        public SerializeValueList                   ValueList       { get { return m_ValueList; } }
        
        
        public static MonoEvent operator +( MonoEvent body, Action action )
        {
            if( body.m_Actions == null ) body.m_Actions = new List<Action>( );
            if( body.m_Actions.FindIndex( ( prop ) => prop == action ) == -1 )
            {
                body.m_Actions.Add( action );
            }
            return body;
        }
        
        public static MonoEvent operator -( MonoEvent body, Action action )
        {
            if( body.m_Actions != null )
            {
                body.m_Actions.Remove( action );
            }
            return body;
        }
        
        void Update( )
        {
            if( m_Actions != null && m_Actions.Count > 0 )
            {
                Action[] actions = m_Actions.ToArray( );
                for( int i = 0; i < actions.Length; ++i )
                {
                    actions[ i ]( this );
                }
            }
            else
            {
                this.RemoveComponent( );
            }
        }
    }
}
