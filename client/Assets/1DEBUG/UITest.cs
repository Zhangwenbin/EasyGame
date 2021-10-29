using System.Collections;
using System.Collections.Generic;
using EG;
using UnityEngine;

public class UITest : MonoBehaviour
{
    public UIImageArray ImageArray;

    public int imageIndex;

    public UIAnimationSlider _UIAnimationSlider;
    // Start is called before the first frame update
    void Start()
    {
        ImageArray.ImageIndex = imageIndex;
     
        // 动画slider,从1300先到2000,到达终点后,再从起点2000,到3000中间
        _UIAnimationSlider.SetRange(1000, 2000);
        _UIAnimationSlider.SetValue(1300);
        _UIAnimationSlider.SetActionReset((prop) =>
        {
            prop.SetRange(2000,4000);
            prop.Refresh();
        });
        _UIAnimationSlider.SetActionUpdate((prop) =>
        {
            Debug.Log(prop.Value);
        });
        _UIAnimationSlider.Play(1300, 3000, 1f);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
