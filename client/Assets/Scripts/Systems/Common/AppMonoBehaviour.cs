using UnityEngine;

namespace EG
{
    public class AppMonoBehaviour : MonoBehaviour
    {

        protected bool                  m_Initialize            = false;


        public bool isInitialized       { get { return m_Initialize;            } }
        
        public bool IsInitialized( )    { return m_Initialize;                  }

        
        protected virtual void OnDestroy( )
        {
            Release();
        }
        
        public virtual void Initialize( )
        {
            m_Initialize = true;

        }

        public virtual void Release( )
        {
            m_Initialize = false;
        }
        
        public virtual void Reset( )
        {
        }
        
        
        
        public virtual void OnCacheEnable( )
        {
        }
        

        public virtual void OnCacheDisable( )
        {
        }
        
        
        #if DEBUG_BUILD
        public virtual Rect OnDebugInspecor( Rect rect, DebugMenu.WindowBase window )
        {
            return rect;
        }
        #endif

    }
}