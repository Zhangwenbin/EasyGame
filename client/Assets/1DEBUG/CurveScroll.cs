using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EG;
public class CurveScroll : MonoBehaviour
{
    
    public class ChapterData
    {
        public string name;
        public string title;
    }

    public int chapterCount;
    private  SerializeValueList ValueList; 
    
    [Range(0, 200)]
    [SerializeField]
    float OffsetAdd_X = 40;

    [Range(-200, 200)]
    [SerializeField]
    float OffsetAdd_Center = 40;

    [Range(0, 200)]
    [SerializeField]
    float OffsetMulti = 120;

    [Range(0, 2.0f)]
    [SerializeField]
    float Curve = 0.9f;
    private void Start()
    {
        SerializeValueBehaviour serializeValueBehaviour = GetComponent<SerializeValueBehaviour>();
        ValueList = serializeValueBehaviour.list;
        MakeList();
    }

    void MakeList()
    {
        var source = new Content.ItemSource();
        List<ChapterData> chapters = new List<ChapterData>();
        for (int i = 0; i < chapterCount; i++)
        {
            chapters.Add(new ChapterData(){name = "name"+i,title = "tile"+i});
        }
       
        foreach (var chapter in chapters)
        {
            if (chapter != null)
            {
                source.Add(this, chapter);
            }
        }

        var layout = ValueList.GetComponent<UIContentLayout>("content");
        layout.Initialize(source, Vector2.zero);
    }
    
    public void MoveCurvedPosition(RectTransform child)
    {
        RectTransform parent = child.parent.parent as RectTransform;
        RectTransform content = child.parent as RectTransform;

        Rect rect = parent.rect;

        float anchored_pos_y = child.anchoredPosition.y * -1 - content.anchoredPosition.y - (child.rect.height / 2) - OffsetAdd_Center;

        float proportion = anchored_pos_y / rect.height;
        double sin = Math.Sin(proportion * 180.0f * (Math.PI / 180.0f)) / Curve;

        Vector2 pos = child.anchoredPosition;

        pos.x = (float)(OffsetAdd_X + OffsetMulti * sin);

        child.anchoredPosition = pos;
    }

    class Content
        {
            public class ItemAccessor : UIContentListTemplate.ItemAccessorBase
            {
                const string KEY_GO_NODE = "node";
                
                UICanvasElementCallback m_CanvasCallback;
                private ChapterData m_Chapter;

                private CurveScroll m_View;
                public override bool IsValid
                {
                    get { return m_Chapter != null; }
                }
                
                public void Initialize(CurveScroll view, ChapterData chapter_data)
                {
                    m_View = view;
                    m_Chapter = chapter_data;
                }
                
                protected override void OnBind()
                {
                    if (m_Chapter == null)
                        return;
        
                    SerializeValueBehaviour value = Node.GetComponent<SerializeValueBehaviour>();
        
                    m_CanvasCallback = Node.GetComponent<UICanvasElementCallback>();
                    m_CanvasCallback.OnLayoutComplete = OnLayoutComplete;
        
                    DataSource source = DataSource.Bind(Node.gameObject, m_Chapter);
                    
                    if (m_Chapter != null)
                    {
                        DataSource.Bind(Node, m_Chapter);
                    }

                    value.list.GetUITextMeshPro("title").text = m_Chapter.title;
                }
                
                protected override void OnClear()
                {
                    if (m_Chapter != null)
                    {
                        m_Chapter = null;
                    }
                }
                
                //public override void ForceUpdate()
                //{
                //    if (Node != null)
                //    {
                //        SerializeValueBehaviour value_behaviour = Node.GetComponent<SerializeValueBehaviour>();
                //
                //        ParamDisplay.UpdateAll(Node.gameObject);
                //    }
                //}
        
                public override void LateUpdate()
                {
                    base.LateUpdate();
                    //---------------------------
                    m_CanvasCallback.SetDirty();
                }
        
                void OnLayoutComplete()
                {
                    var rect = Node.transform as RectTransform;
                    m_View.MoveCurvedPosition(rect);
                }
        
            }
            
            public class ItemParameter : UIContentListTemplate.ItemParameterBase
            {
                public ItemParameter(CurveScroll view, ChapterData chapter_data)
                {
                    CreateAccessor<ItemAccessor>();
                    GetAccessor<ItemAccessor>().Initialize(view, chapter_data);
                }
            }
            
            public class ItemSource : UIContentListTemplate.ItemSourceBase
            {
                //public void Add(List<ChapterData> chapters)
                //{
                //    int max = chapters.Count;
                //    for (int i = 0; i < max; ++i)
                //    {
                //        ItemParameter param = new ItemParameter(chapters[i]);
                //        if (param.IsValid())
                //        {
                //            Add(param);
                //        }
                //    }
                //}
        
                public void Add(CurveScroll view, ChapterData chapter)
                {
                    if (chapter == null)
                        return;
        
                    ItemParameter itemParam = new ItemParameter(view, chapter);
                    if (itemParam.IsValid())
                    {
                        Add(itemParam);
                    }
                }
        
            }
        }
        

}
