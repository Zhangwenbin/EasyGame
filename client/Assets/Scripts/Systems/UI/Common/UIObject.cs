using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

#if UNITY_EDITOR
using UnityEditor;
#endif


namespace EG
{
    [AddComponentMenu("Scripts/System/UI/Common/UIObject")]
    public class UIObject : AppMonoBehaviour
    {

        protected RectTransform m_RectTransform                 = null;
        protected Vector2       m_DefaultAnchoredPosition       = Vector2.zero;
        
        
        public RectTransform    RectTransform                   { get { return m_RectTransform;             } }
        public Vector2          DefaultAnchoredPosition         { get { return m_DefaultAnchoredPosition;   } }
        
        
        private void Awake( )
        {
            m_RectTransform = gameObject.GetComponent<RectTransform>( );
            Refresh( );
        }
        

        public void Refresh( )
        {
            if( m_RectTransform != null )
            {
                m_DefaultAnchoredPosition = m_RectTransform.anchoredPosition;
            }
        }
        
        
    }
}
