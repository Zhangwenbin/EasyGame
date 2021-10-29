/**************************************************************************/
/*@brief  简要描述   
  @author zwb
***************************************************************************/
using System.Collections;
using System.Collections.Generic;
using EG.GameState;
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
    public class UIStartMenu : CanvasWindow
    {
        public override string WindowResource => "UIStartMenu.prefab";

        public override int WindowLayer => (int)EWindowLayer.Panel;

        public override bool FullScreen => true;

        private Button startButton;
        public override void OnCreate()
        {
            startButton =AddButtonListener("startButton",OnClickStartButton);
        }

        private void OnClickStartButton()
        {
            Debug.Log("start game");
            Game.Goto(BattleState.name);
            Close<UIStartMenu>();
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