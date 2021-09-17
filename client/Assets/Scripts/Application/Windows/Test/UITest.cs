/**************************************************************************/
/*@brief  简要描述   
  @author zwb
***************************************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
#if UNITY_EDITOR
using UnityEditor;
#endif
namespace EG
{
    //=========================================================================
    //简要注释
    //=========================================================================
    public class UITest : CanvasWindow
    {
        
        #region 编辑器
        #if UNITY_EDITOR

        #endif
        #endregion

        public override string WindowResource => "UINewVersion.prefab";

        public override int WindowLayer => (int)EWindowLayer.Panel;

        public override bool FullScreen => false;

        private Button btnCancel, btnConfirm;
        public override void OnCreate()
        {
            btnCancel = GetUIComponent<Button>("cancel");
            btnConfirm = GetUIComponent<Button>("confirm");

            AddButtonListener("cancel", OnCancel);
            AddButtonListener("confirm", OnConfirm);
        }

        private void OnConfirm()
        {
           Debug.Log("OnConfirm");
        }

        private void OnCancel()
        {
            Debug.Log("OnCancel");
        }

        public override void OnRefresh()
        {
           
        }

        public override void OnUpdate()
        {
            
        }

        public override void OnDestroy()
        {
            
        }
    }
}