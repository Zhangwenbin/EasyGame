using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using MotionFramework;
using MotionFramework.Event;
using DG.Tweening;
using MotionFramework.Resource;

public class GuideManager : ModuleSingleton<GuideManager>, IModule
{
    public List<GuideBase> guides;
    public GuideBase currentGuide;
    
    private void Init()
    {
        guides = new List<GuideBase>();
        var handle = ResourceManager.Instance.LoadAssetSync<GuideConfig>("GuideConfig/guideConfig.asset");
        if (handle.AssetObject)
        {
            GuideConfig config = handle.AssetObject as GuideConfig;
            if (config!=null)
            {
                for (int i = 0; i < config.guides.Count; i++)
                {
                    var guide = config.guides[i];
#if UNITY_EDITOR
                        if (!guide.ignore)
#endif
                    {
                        GuideBase guideObj = new GuideBase();
                        guideObj.index = i+1;
                        guideObj.InitByConfig(guide);
                        guides.Add(guideObj);
                    }
                }
            }
            return;
        }
       

    }

    public bool IsInGuiding()
    {
        return currentGuide != null && !currentGuide.IsFinished();
    }

    public bool IsFinishGuide(string guideId)
    {
        return true;
    }

    public void FinishGuide(string guideId,bool realFinish=true)
    {
        if (realFinish)
        {
            if (currentGuide!=null&&currentGuide.id==guideId)
            {
                currentGuide = null;
            }
        }

    }

    public void CheckGuide(string startPos,bool startGuide=true)
    {
        if (IsInGuiding())
        {
            return;
        }
        for (int i = 0; i < guides.Count; i++)
        {
            var guide = guides[i];
            if (!IsFinishGuide(guide.id))
            {
                if (guide.startPos==startPos&&guide.condition())
                {
                    if (startGuide)
                    {
                        guide.OnStart();
                        currentGuide = guide;
                        return;
                    }
                }
            }
        }
    }


    public void UpdateGuide()
    {
        if (currentGuide!=null)
        {
            currentGuide.Update();
        }
    }


    public void OnCreate(object createParam)
    {
        Init();
    }

    public void OnUpdate()
    {
        UpdateGuide();
    }

    public void OnGUI()
    {
       
    }
}

public class GuideBase
{
    public string id;
    public int index;
    public string startPos;
    public Func<bool> condition;
    public List<GuideStep> steps;

    public int currentStep;
    private bool isFinished;

    public void InitByConfig(GuideData data)
    {
        this.id = data.id;
        this.startPos = data.startPos;
        switch (data.guideUnlockType)
        {
          
            
            default:
                break;
        }

        steps = new List<GuideStep>(data.steps.Count);
        for (int i = 0; i < data.steps.Count; i++)
        {
            var step = data.steps[i];
            switch (step.type)
            {
              
                default:
                    break;
            }
            steps[i].data = step;
            steps[i].index = i + 1;
        }

        currentStep = 0;
    }

    public GuideStep GetCurrentStep()
    {
        if (currentStep >= 0 && currentStep < steps.Count)
        {
            return steps[currentStep];
        }
        else
        {
            return null;
        }
    }

    public virtual void Init()
    {

    }

    public virtual void OnStart()
    {
        currentStep = 0;
        steps[0].OnStart();

        //SDKManager.Instance.ohayoo_game_guide(index * 100 + steps[0].index, $" 开始引导:{index * 100 + steps[0].index} {steps[0].data.name}");
    }

    public  void Update()
    {
        if (isFinished)
        {
            return;
        }
        if (currentStep>=0&&currentStep<steps.Count)
        {
            var step = steps[currentStep];
            if (step.IsFinish())
            {
                step.OnFinish();

               // SDKManager.Instance.ohayoo_game_guide(index * 100 + step.index, $" 结束引导:{index * 100 + step.index} {step.data.name}");
                
                currentStep++;
                if (currentStep >= 0 && currentStep < steps.Count)
                {
                    step = steps[currentStep];
                    step.OnStart();

                   // SDKManager.Instance.ohayoo_game_guide(index * 100 + step.index, $" 开始引导:{index * 100 + step.index} {step.data.name}");
                }
                else
                {
                    OnFinish();
                }             
            }
            else
            {
                step.Update();
            }


        }
        else
        {
            OnFinish();
        }
#if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.Space))
        {
            OnFinish();
        }
#endif
    }


    public virtual void OnFinish()
    {
        isFinished = true;
        GuideManager.Instance.FinishGuide(id);
        // UITools.CloseWindow<UIGuide>();
    }

    public bool IsFinished()
    {
        return isFinished;
    }
    
    public virtual void JumpGuide()
    {
        // var step = new GuideStep_ShowDialog();
        // step.index = steps.Count+1;
        // var stepData = new GuideStepData();
        // stepData.name = "结束对话";
        // stepData.type = GuideStepType.ShowDialog;
        // stepData.dialogTxt = "不打扰了,您继续赶海吧";
        // step.data = stepData;
        // currentStep = steps.Count;
        // steps.Add(step);
    }
}


public class GuideStep
{
    public int index;
    protected bool isFinished;

    public GuideStepData data;
    public virtual void Init()
    {

    }
    public virtual void OnStart()
    {

    }

    public virtual void Update()
    {

    }

    public virtual bool IsFinish()
    {
        return isFinished;
    }

    public virtual void OnFinish()
    {
        if (data.markFinished)
        {
            GuideManager.Instance.FinishGuide(GuideManager.Instance.currentGuide.id,false);
        }
        // var uiGuide = UITools.GetWindow<UIGuide>();
        // if (uiGuide!=null)
        // {
        //     uiGuide.OnFinishStep(this);
        // }
        Time.timeScale = 1;
    }
}



//
// public class GuideStep_ClickUI : GuideStep
// {
//     public GuideStep_ClickUI(string targetKey)
//     {
//         this.targetKey = targetKey;
//     }
//
//     public string targetKey;
//
//     private GameObject target;
//     private bool findTarget;
//     public override void Init()
//     {
//         base.Init();
//     }
//     public override void OnStart()
//     {
//         base.OnStart();
//     }
//
//     public override void Update()
//     {
//         base.Update();
//         if (target == null)
//         {
//             if (targetKey.Contains("|"))
//             {
//                 var keys = targetKey.Split('|');
//                 for (int i = 0; i < keys.Length; i++)
//                 {
//                     target = GameObject.Find(keys[i]);
//                     if (target!=null&&target.activeInHierarchy)
//                     {
//                         return;
//                     }
//                 }
//                
//                 return;
//             }
//             else
//             {
//                 target = GameObject.Find(targetKey);
//                 return;
//             }
//      
//         }
//         if (!findTarget)
//         {
//             var targetPos = target.transform.position;
//             var uiGuide = UITools.GetWindow<UIGuide>();
//             if (uiGuide.clickHand == null)
//             {
//                 return;
//             }
//             uiGuide.clickHand.transform.localPosition = uiGuide.clickHand.parent.worldToLocalMatrix.MultiplyPoint(targetPos);
//             uiGuide.clickHand.name = target.name;
//             uiGuide.handler.SetTarget(target, false);
//             uiGuide.tips.gameObject.SetActive(true);
//             uiGuide.clickHand.gameObject.SetActive(true);
//             uiGuide.clickEff.gameObject.SetActive(true);
//             uiGuide.OnClickHandAction = () => { isFinished = true; };
//             UpdateUIText();
//             if (data.onlyClickObj)
//             {
//                 uiGuide.OnClickBackAction = () => {
//                     isFinished = true;
//                 };
//             }
//
//             findTarget = true;
//         }
//        
//     }
//
//     public override void OnFinish()
//     {
//         base.OnFinish();
//     }
// }


public class GuideStep_Delay : GuideStep
{

    public GuideStep_Delay(float delay)
    {
        delayTime = delay;
    }
    public float delayTime;

    private float startTime;
    public override void Init()
    {
        base.Init();
    }
    public override void OnStart()
    {
        startTime = Time.time;
        Time.timeScale = 1;
    }

    public override void Update()
    {
        base.Update();
        if (Time.time-startTime>=delayTime)
        {
            isFinished = true;
        }
    }

    public override void OnFinish()
    {
        base.OnFinish();
    }
}



