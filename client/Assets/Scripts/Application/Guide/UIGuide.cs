using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using MotionFramework.Audio;
using MotionFramework.Window;

// [Window((int)EWindowLayer.Guide, false)]
sealed class UIGuide : CanvasWindow
{
    public RectTransform clickHand, clickEff, moveEff, movehand;
    public GameObject tips;
    public UnityEngine.UI.Image moveSeafood;
    public RawImage backobj;

    public GameObject dialogObj;
    public UISprite headSprite;
    public Text dialogTxt;

    public Text clickTxt;
    public Text moveTxt;

    public Transform hand;
    public Transform back;

    public RawImage rectBack;

    public GuideEventHandler handler;

    public bool useMaskEff;

    public Action OnClickHandAction;
    public Action OnClickBackAction;
    private float _curalpha;
    private float _curRadius;
    private bool updateMask;
    private float alphadesspeed;
    private float circledesspeed;
    private float minRadius;

    public Canvas canvas;

    private int clickBackgroundCount;
    private const int MAXCOUNT=20;

    public override void OnCreate()
    {
        clickHand = GetUIElement("clickhand").transform as RectTransform;
        hand = GetUIElement("Hand");
        clickEff = GetUIElement("clickEffect") as RectTransform;
        moveEff = GetUIElement("moveEffect") as RectTransform;
        movehand = GetUIElement("movehand") as RectTransform;
        moveSeafood = GetUIComponent<Image>("moveSeafood");
        tips = GetUIElement("tips").gameObject;
        backobj = GetUIComponent<RawImage>("Background");
        dialogObj = GetUIElement("dialogs").gameObject;
        headSprite = GetUIComponent<UISprite>("head");
        dialogTxt = GetUIComponent<Text>("dialogTxt");
        clickTxt = GetUIComponent<Text>("clickText");
        moveTxt = GetUIComponent<Text>("moveText");
        canvas = clickHand.GetComponentInParent<Canvas>();


        backobj.color = new Color(1, 1, 1, 0);
        back = AddButtonListener("Background", BackBtnClicked).transform;

        rectBack = GetUIComponent<RawImage>("rectMat");

        minRadius = clickHand.rect.width;
        alphadesspeed = 5;
        circledesspeed = 10;
        handler = GetUIComponent<GuideEventHandler>("clickhand");
        handler.onClick = OnClickHand;
        HideAll();
    }
    public override void OnDestroy()
    {
    }
    public override void OnRefresh()
    {
        var index = GuideManager.Instance.currentGuide.currentStep;
        if (GuideManager.Instance.currentGuide.steps[index].data.recoverHandle)
        {
            backobj.gameObject.SetActive(false);
        }
        else
        {
            backobj.gameObject.SetActive(true);
        }
    }
    public override void OnUpdate()
    {
        if (updateMask)
        {
            UpdateGuideMask();
        }
    }

    private void OnClickHand()
    {
        if (OnClickHandAction != null)
        {
            OnClickHandAction();
        }
    }

    private void BackBtnClicked()
    {
        clickBackgroundCount++;
        if (clickBackgroundCount>MAXCOUNT)
        {
            if (GuideManager.Instance.currentGuide!=null)
            {
                GuideManager.Instance.currentGuide.JumpGuide();
            }

            clickBackgroundCount = 0;
            return;
        }
        if (OnClickBackAction != null)
        {
            OnClickBackAction();
        }

        var index = GuideManager.Instance.currentGuide.currentStep;
        useMaskEff=GuideManager.Instance.currentGuide.steps[index].data.useMaskEff;
        if (!useMaskEff)
        {
            return;
        }
        SetBackMaskType(EBackMaskType.Round);
        //backobj.material.EnableKeyword("_ROUNDMODE_ROUND");
        //backobj.material.DisableKeyword("_ROUNDMODE_ELLIPSE");
        backobj.material.SetVector("_Center", new Vector4(clickHand.transform.localPosition.x, clickHand.transform.localPosition.y, 0, 0));

        _curalpha = 200;

        _curRadius = 500;

        backobj.material.SetFloat("_Radius", _curRadius);

        backobj.color = new Color(1, 1, 1, _curalpha / 255);
        updateMask = true;
    }


    private void UpdateGuideMask()
    {
        if (_curalpha == 0 && _curRadius == minRadius)
        {
            updateMask = false;
            return;
        }
        if (_curalpha > 0 && _curRadius == minRadius)
        {
            var targetvalue = _curalpha - alphadesspeed;
            if (targetvalue < 0)
            {
                targetvalue = 0;
            }
            backobj.color = new Color(1, 1, 1, targetvalue / 255);

            _curalpha = targetvalue;
        }

        if (_curRadius > minRadius)
        {
            var targetvalue = _curRadius - circledesspeed;
            if (targetvalue < minRadius)
            {
                targetvalue = minRadius;
            }
            backobj.material.SetFloat("_Radius", targetvalue);

            _curRadius = targetvalue;
        }

    }

    public void SetCustomMask(Vector2 center, float alpha, Vector2 radius, bool isRect)
    {
        if (isRect)
        {
            SetBackMaskType(EBackMaskType.Ellipse);
            //backobj.material.EnableKeyword("_ROUNDMODE_ELLIPSE");
            //backobj.material.DisableKeyword("_ROUNDMODE_ROUND");
            backobj.color = new Color(1, 1, 1, alpha);
            backobj.material.SetVector("_Center", new Vector4(center.x, center.y, 0, 0));

            backobj.material.SetFloat("_Width", radius.x);
            backobj.material.SetFloat("_Height", radius.y);
        }
        else
        {
            SetBackMaskType(EBackMaskType.Round);
            //backobj.material.EnableKeyword("_ROUNDMODE_ROUND");
            //backobj.material.DisableKeyword("_ROUNDMODE_ELLIPSE");
            backobj.color = new Color(1, 1, 1, alpha);
            backobj.material.SetVector("_Center", new Vector4(center.x, center.y, 0, 0));

            backobj.material.SetFloat("_Radius", radius.x);
        }
    }

    private enum EBackMaskType
    {
        Round, Ellipse/*, Rect,*/
    }
    private void SetBackMaskType(EBackMaskType type)
    {
        if (type == EBackMaskType.Round)
        {
            backobj.material.EnableKeyword("_ROUNDMODE_ROUND");
            backobj.material.DisableKeyword("_ROUNDMODE_ELLIPSE");
            //backobj.material.DisableKeyword("_ROUNDMODE_RECT");
        }
        else if (type == EBackMaskType.Ellipse)
        {
            backobj.material.DisableKeyword("_ROUNDMODE_ROUND");
            backobj.material.EnableKeyword("_ROUNDMODE_ELLIPSE");
            //backobj.material.DisableKeyword("_ROUNDMODE_RECT");
        }
        //else if (type == EBackMaskType.Rect)
        //{
        //    backobj.material.DisableKeyword("_ROUNDMODE_ROUND");
        //    backobj.material.DisableKeyword("_ROUNDMODE_ELLIPSE");
        //    backobj.material.EnableKeyword("_ROUNDMODE_RECT");
        //}
    }



    public void OnFinishStep(GuideStep step)
    {
        OnClickHandAction = null;
        OnClickBackAction = null;
        HideAll();
        clickBackgroundCount = 0;
    }

    public void HideAll()
    {
        tips.SetActive(false);
        moveEff.gameObject.SetActive(false);
        clickEff.gameObject.SetActive(false);
        clickHand.gameObject.SetActive(false);
        dialogObj.SetActive(false);
        SetCustomMask(Vector2.zero,0,Vector2.zero,false);
    }

    public void HideBack()
    {
        back.gameObject.SetActive(false);
    }
    public void ShowBack()
    {
        back.gameObject.SetActive(true);
    }

}