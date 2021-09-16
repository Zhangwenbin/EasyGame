using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[CreateAssetMenu(fileName = "Data", menuName = "CreateSO/GuideConfig", order = 1)]
public class GuideConfig : ScriptableObject
{
    public List<GuideData> guides;

}

[Serializable]
public class GuideData
{

    public string name;
    public bool ignore;
    public string id;
    public string startPos;
    public GuideUnlockType guideUnlockType;
    public int levelId;
    [Header("步骤")]
    public List<GuideStepData> steps;
}

[Serializable]
public class GuideStepData
{
    public string name;
    public bool markFinished;
    public bool recoverHandle;
    [Tooltip("步骤的类型")]
    public GuideStepType type;
    public bool useMaskEff;
    public string clickTxt;
    public Vector3 clickTxtPosition;
    public float handRotation;
    public string moveTxt;
    public string headIcon;
    public string dialogTxt;
    public Vector2 dialogPosition;

    //背包专用
    public Vector2Int coord;
    public string spriteName;
    public float scale;

    //点击UI 专用
    public string targetKey;

    //延迟专用
    public float delayTime;

    public Vector2Int gamePosition;

    public int collectId;

    public bool onlyClickObj;

    public float timeScale;

    public bool isRectMask;
    public bool maskCenter3D;
    public string maskCenter;
    public Vector2 maskRadius;
    public float maskAlpha;

    public string waitWindow;

    public string openWindow;
    public string closeWindow;
}

public enum GuideStepType
{
    [InspectorName("海货收集")]
    Collect,
    [InspectorName("拖进背包")]
    DragIntoBag,
    [InspectorName("点击UI")]
    ClickUI,
    [InspectorName("延迟时间")]
    Delay,
    [InspectorName("显示对话")]
    ShowDialog,
    [InspectorName("非强制收集")]
    CollectNoForce,
    [InspectorName("等待到达地图位置")]
    WaitGamePosition,
    [InspectorName("等待打开界面")]
    WaitWindow,
    [InspectorName("等待qte击中")]
    WaitQTEHit,
    [InspectorName("抽坑预览")]
    PuzzleMove,
    [InspectorName("等待进入抽坑")]
    WaitForEnterPuzzle,
    [InspectorName("关闭UI")]
    CloseWindow,
}

public enum GuideUnlockType
{
    None,
    CurrentLevel,
    Unlock,
    FinishLevel
}



