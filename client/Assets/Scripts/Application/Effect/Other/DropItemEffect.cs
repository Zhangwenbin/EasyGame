using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace EG
{

    public class DropItemEffect : UnityEngine.MonoBehaviour
    {
        const string TREASURE_GAMEOBJECT_NAME = "UI_DROPITEM";
        
        private enum State
        {
            NONE        ,
            OPEN        ,       // 
            POPUP       ,       // 
            MOVE        ,       // 
            END         ,       // 
            DELETE      ,       // 
            DESTROY     ,       // 
        }
        
        
        State           m_State         = State.NONE;
        
        RectTransform   m_TargetRect    = null;
        //DropData        m_DropItem      = null;
        
        public float    Acceleration    = 1;
        public float    Delay           = 1.0f;
        float           m_Speed         = 0.0f;
        Animator        m_EndAnimator   = null;
        

        float           m_OpenDelay     = 0;
        
        public float    PopSpeed        = 1.0f;
        float           m_PopSpeed      = 0.0f;
        float           m_ScaleSpeed    = 0.0f;
        

        float           m_DeleteDelay   = 1.0f;
        
        
        
        public RectTransform    TargetRect  { get { return m_TargetRect;            } }
        
        public bool             isDestroy   { get { return m_State == State.DESTROY; } }
        

        
        private void Start( )
        {
            Hide( );
        }
        

        public void Setup(  )
        {

            GameObject go = GameObjectId.FindGameObject( TREASURE_GAMEOBJECT_NAME );
            if( go == null )
            {
                Debug.LogError( TREASURE_GAMEOBJECT_NAME + "。" );
            }
            else
            {
                m_TargetRect = go.transform as RectTransform;
            }
            
            // var dataSrc = DataSource.Bind( gameObject, new UIIconData( drop ) );
            // if( dataSrc != null )
            // {
            //     dataSrc.Add<DropData>(drop);
            // }
            //
            // ParamDisplay.UpdateAll( gameObject );
            
            transform.localScale = new Vector3( 0.3f, 0.3f, 1.0f );
            transform.position = new Vector3( transform.position.x, transform.position.y + 25.0f, transform.position.z );
            
            m_EndAnimator = go.GetComponent<Animator>();
//            m_EndAnimator.SetBool( "open", false );
        }
        

        private void Update( )
        {
            switch( m_State )
            {
            case State.OPEN:
                State_Open();
                break;
            case State.POPUP:
                State_Popup();
                break;
            case State.MOVE:
                State_Move();
                break;
            case State.END:
                State_End();
                break;
            case State.DELETE:
                State_Delete();
                break;
            }
        }
        
        
        private void Hide( )
        {
            foreach( Transform child in gameObject.transform )
            {
                child.gameObject.SetActive( false );
            }
        }
        

        private void Show( )
        {
            foreach( Transform child in gameObject.transform )
            {
                child.gameObject.SetActive( true );
            }
        }
        

        public void Open( float delay )
        {
            m_OpenDelay = delay;
            m_State = State.OPEN;
        }
        

        
        private void State_Open()
        {
            m_OpenDelay -= TimerManager.DeltaTime;
            if( m_OpenDelay > 0 ) return;
            
            Show( );
            
            m_State = State.POPUP;
        }
        

        private void State_Popup()
        {
            Delay -= TimerManager.DeltaTime;
            
            m_PopSpeed += PopSpeed * Time.deltaTime;
            if( 1 > transform.localScale.x + m_PopSpeed )
            {
                Vector3 localScale = transform.localScale;
                transform.localScale = new Vector3( localScale.x + m_PopSpeed, localScale.y + m_PopSpeed, localScale.z );
                
                float s = m_PopSpeed * 100;
                if( s > 25.0f )
                {
                    s = 25.0f;
                }
                Vector3 localPos = transform.localPosition;
                transform.localPosition = new Vector3( localPos.x, localPos.y + s, localPos.z );
            }
            else
            {
                transform.localScale = new Vector3( 1, 1, 1 );
                
                if( Delay < 0.0f )
                {
                    m_State = State.MOVE;
                }
            }
        }
        

        private void State_Move()
        {
            m_Speed += Acceleration * Time.deltaTime;
            
            Vector3 targetPos = m_TargetRect.position;
            //targetPos.x -= m_TargetRect.sizeDelta.x * 0.8f;
            targetPos.y += m_TargetRect.sizeDelta.y * 0.5f;
            
            Vector3 targetDiff = targetPos - transform.position;
            Vector3 velocity = targetDiff.normalized * m_Speed;
            
            if( velocity.sqrMagnitude < targetDiff.sqrMagnitude )
            {
                transform.position += velocity;
                
                m_ScaleSpeed += 1.0f * Time.deltaTime;
                if( transform.localScale.x - m_ScaleSpeed > 0.5f )
                {
                    transform.localScale = new Vector3( transform.localScale.x - m_ScaleSpeed, transform.localScale.y - m_ScaleSpeed, 1 );
                }
            }
            else
            {
                transform.position = targetPos;
                
                Hide( );
                
                m_State = State.END;
            }
        }
        

        private void State_End()
        {
            if( m_EndAnimator != null )
            {
                if( !m_EndAnimator.GetBool( "open" ) )
                {
                    m_EndAnimator.SetBool( "open", true );
                }
                else
                {
                    m_EndAnimator.Play( "open", 0, 0.0f );
                }
            }
            
            
            m_DeleteDelay = 0.1f;
            
            m_State = State.DELETE;
        }
        

        private void State_Delete()
        {
            m_DeleteDelay -= Time.deltaTime;
            if( m_DeleteDelay < 0 )
            {
                gameObject.SafeDestroy( );
                
                // 
                m_State = State.DESTROY;
            }
        }
        
    }
}
