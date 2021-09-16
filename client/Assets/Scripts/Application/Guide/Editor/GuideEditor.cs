
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

[CustomPropertyDrawer(typeof(GuideStepData))]
public class GuideStepDataDrawerUIE : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        // Using BeginProperty / EndProperty on the parent property means that
        // prefab override logic works on the entire property.
        EditorGUI.BeginProperty(position, label, property);

        // Draw label
        position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

        // Don't make child fields be indented
        var indent = EditorGUI.indentLevel;
        EditorGUI.indentLevel = 0;
        propertyIndex = 0;
        DrawProperty(position, property, "name", "名称");
        DrawProperty(position, property, "markFinished", "标记结束");
        DrawProperty(position, property, "recoverHandle", "回复操作");
        var sptype= DrawProperty(position, property, "type", "类型");
        var stepType = (GuideStepType)sptype.intValue;
        switch (stepType)
        {
            case GuideStepType.Collect:
                DrawProperty(position, property, "collectId", "收集id");
                DrawProperty(position, property, "onlyClickObj", "只是点击");
                
                break;
            case GuideStepType.DragIntoBag:
                DrawProperty(position, property, "coord", "格子坐标");
                DrawProperty(position, property, "spriteName", "背包图片");
                DrawProperty(position, property, "scale", "背包缩放");
                break;
            case GuideStepType.ClickUI:
                DrawProperty(position, property, "targetKey", "按钮名称");
                DrawProperty(position, property, "onlyClickObj", "只是点击");
                break;
            case GuideStepType.Delay:
                DrawProperty(position, property, "delayTime", "延迟时间");
                break;
            case GuideStepType.ShowDialog:
                break;
            case GuideStepType.CollectNoForce:
                break;
            case GuideStepType.WaitGamePosition:
                DrawProperty(position, property, "gamePosition", "游戏位置");
                break;
            case GuideStepType.WaitWindow:
                DrawProperty(position, property, "waitWindow", "等待界面");

                break;
            default:
                break;
        }
        DrawProperty(position, property, "useMaskEff", "使用遮罩");
        DrawProperty(position, property, "clickTxt", "点击文本");
        DrawProperty(position, property, "clickTxtPosition", "点击文本位置");

        DrawProperty(position, property, "handRotation", "手指旋转");
        DrawProperty(position, property, "moveTxt", "移动文本");
        DrawProperty(position, property, "headIcon", "头像");
        DrawProperty(position, property, "dialogTxt", "对话文本");

        DrawProperty(position, property, "dialogPosition", "对话位置");

        DrawProperty(position, property, "timeScale", "timeScale");

        DrawProperty(position, property, "isRectMask", "方形mask");
        DrawProperty(position, property, "maskCenter3D", "maskCenter3D");
        DrawProperty(position, property, "maskCenter", "mask中心");
        DrawProperty(position, property, "maskRadius", "mask大小");
        DrawProperty(position, property, "maskAlpha", "mask alpha");

        DrawProperty(position, property, "closeWindow", "关闭界面");

        DrawFunctionButton(position, property);
        // Set indent back to what it was
        EditorGUI.indentLevel = indent;

        EditorGUI.EndProperty();
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        var sptype = property.FindPropertyRelative("type");
        var stepType = (GuideStepType)sptype.intValue;
        int count = 0;
        switch (stepType)
        {
            case GuideStepType.Collect:
                count = 1;
                break;
            case GuideStepType.DragIntoBag:
                count = 3;
                break;
            case GuideStepType.ClickUI:
                count = 2;
                break;
            case GuideStepType.Delay:
                count = 1;
                break;
            case GuideStepType.ShowDialog:
                break;
            case GuideStepType.CollectNoForce:
                break;
            case GuideStepType.WaitGamePosition:
                count = 1;
                break;
            case GuideStepType.WaitWindow:
                count = 1;
                break;
            default:
                break;
        }
        count += 19;
        return count*propertyheight;
    }
    private float propertyheight = 20;
    private int propertyIndex = 0;
    private float tipsWidth = 100;
    private SerializedProperty DrawProperty(Rect rect, SerializedProperty property,string propertyName,string tips)
    {
        var x = rect.x;
        var y = rect.y+propertyIndex* propertyheight;
        var tipsRect = new Rect(x, y, tipsWidth, propertyheight);
        var tempRect = new Rect(x + tipsWidth, y, rect.width - tipsWidth, propertyheight);
        EditorGUI.LabelField(tipsRect, new GUIContent(tips));
        var sp = property.FindPropertyRelative(propertyName);
        EditorGUI.PropertyField(tempRect, sp, GUIContent.none);
        propertyIndex++;
        return sp;
    }

    private int updateIndex = 0;

    private void DrawFunctionButton(Rect rect, SerializedProperty property)
    {
        //var x = rect.x;
        //var y = rect.y + propertyIndex * propertyheight;
        //var tipsRect = new Rect(x, y, 50, propertyheight);
        //updateIndex=EditorGUI.IntField(tipsRect, updateIndex);
        //if (GUI.Button(tipsRect, new GUIContent("add")))
        //{

        //}
        
        //propertyIndex++;
    }
}