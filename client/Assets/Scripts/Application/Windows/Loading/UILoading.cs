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
    public class UILoading : CanvasWindow
    {
        public override string WindowResource => "UILoading.prefab";

        public override int WindowLayer => (int)EWindowLayer.Loading;

        public override bool FullScreen => true;

        private Slider _slider;
        public override void OnCreate()
        {
            _slider = GetUIComponent<Slider>("Slider");

            Events<float>.AddListener(EventsType.sceneLoadingPercent,OnUpateTime,typeof(UILoading));
        }

        private void OnUpateTime(float time)
        {
            _slider.value = time;
        }

        public override void OnRefresh()
        {
           
        }

        public override void OnUpdate()
        {
            
        }

        public override void OnDestroy()
        {
            Events<float>.RemoveListener(EventsType.sceneLoadingPercent,OnUpateTime,typeof(UILoading));
        }
    }
}