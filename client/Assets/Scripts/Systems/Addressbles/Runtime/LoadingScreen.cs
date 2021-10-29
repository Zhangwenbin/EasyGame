using System;
using EG;
using UnityEngine;
using UnityEngine.UI;

public class LoadingScreen:MonoBehaviour
{
    public static LoadingScreen Instance;
    public GameObject downlaodTipPanel;
    public Button confirmDownLoadBtn;
    public Button cancelDownLoadBtn;
    public Text progressTipTxt;
    public Slider progress;
    
    private void Awake()
    {
        Instance = this;
    }

    void SetTipsStatic(AssetStatus status)
    {
        
        switch (status)
        {
            case AssetStatus.Init:
                progressTipTxt.text = "资源准备中";
                break;
            case AssetStatus.CheckUpdate:
                progressTipTxt.text = "检查资源更新";
                break;
            case AssetStatus.Update:
                progressTipTxt.text = "资源下载中";
                break;
            case AssetStatus.Ready:
                progressTipTxt.text = "资源准备完毕";
                break;
            case AssetStatus.ConfirmUpdate:
                downlaodTipPanel.SetActive(true);
                break;
            case AssetStatus.Preload:
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(status), status, null);
        }
    }

    void SetProgress(float v)
    {
        progress.value = v;
    }

    private void OnEnable()
    {
        Events<AssetStatus>.AddListener(EventsType.assetStatusChange,SetTipsStatic);
        Events<float>.AddListener(EventsType.assetProgressChange,SetProgress);

    }

    private void OnDisable()
    {
        Events<AssetStatus>.RemoveListener(EventsType.assetStatusChange,SetTipsStatic);
        Events<float>.RemoveListener(EventsType.assetProgressChange,SetProgress);
    }
}