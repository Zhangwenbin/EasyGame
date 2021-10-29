using UnityEngine;
using UnityEditor;
using System;
using System.Collections;
using System.Collections.Generic;

namespace EG
{

    public class EditorWindow_PreviewController : EditorWindow
    {

        System.Action       m_DrawAnmCtrlAct    = null;
        
        public void Setup( System.Action act )
        {
            this.titleContent.text = "预览控制";
            m_DrawAnmCtrlAct = act;
        }
        
        void OnGUI()
        {
            if( m_DrawAnmCtrlAct != null )
            {
                m_DrawAnmCtrlAct.Invoke();
            }
        }
        
    }
}
