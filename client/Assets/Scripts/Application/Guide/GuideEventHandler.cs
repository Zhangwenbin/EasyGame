using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using System;

public class GuideEventHandler :MonoBehaviour, IPointerClickHandler, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public Action onClick;
    public void OnPointerClick(PointerEventData eventData)
    {
        PassEvent(eventData, ExecuteEvents.pointerClickHandler);
    }

    public GameObject eventTarget;

    public bool canDrag;

    public bool includeChild;

    public void SetTarget(GameObject eventTarget, bool canDrag = false, bool includeChild=true)
    {
        this.eventTarget = eventTarget;
        this.includeChild = includeChild;
        this.canDrag = canDrag;
     
    }
    //把事件透下去
    public void PassEvent<T>(PointerEventData data, ExecuteEvents.EventFunction<T> function) where T : IEventSystemHandler
    {
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(data, results);

        for (int i = 0; i < results.Count; i++)
        {
            if (includeChild)
            {
                if (IsTargetOrTargetChild(results[i].gameObject.transform) )
                {
                    ExecuteEvents.Execute(eventTarget, data, function);//响应点击位置上的UI
                    if (onClick != null)
                    {
                        onClick();
                    }
                    return;
                }
            }
            else
            {
                if (results[i].gameObject == eventTarget)
                {
                    ExecuteEvents.Execute(eventTarget, data, function);//响应点击位置上的UI
                    if (onClick != null)
                    {
                        onClick();
                    }
                    return;
                }
            }
         
        }
    }

    //是目标对象或者目标对象的子对象
    public bool IsTargetOrTargetChild(Transform go)
    {
        while (go!=null)
        {
            if (go.gameObject==eventTarget)
            {
                return true;
            }
            else
            {
                go = go.parent;
            }
        }
        return false;
    }

    private bool beginDrag = false;
    private GameObject dragGameObject;
    private Vector3 startPos;
    public void OnBeginDrag(PointerEventData data)
    {
        if (!canDrag)
        {
            return;
        }
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(data, results);

        for (int i = 0; i < results.Count; i++)
        {
            if (results[i].gameObject != gameObject && results[i].gameObject.name == gameObject.name && results[i].gameObject.activeInHierarchy)
            {
                ExecuteEvents.Execute(results[i].gameObject, data, ExecuteEvents.beginDragHandler);//响应点击位置上的UI
                beginDrag = true;
                dragGameObject = results[i].gameObject;
                startPos = transform.position;
                return;
            }
        }
    }

    void IDragHandler.OnDrag(PointerEventData data)
    {
        if (!canDrag)
        {
            return;
        }
        if (beginDrag&&dragGameObject!=null)
        {
            ExecuteEvents.Execute(dragGameObject, data, ExecuteEvents.dragHandler);//响应点击位置上的UI
            RectTransformUtility.ScreenPointToWorldPointInRectangle(transform as RectTransform, data.position, data.enterEventCamera, out Vector3 pos);
            pos.z = 0;
            transform.position = pos;
            return;
        }


    }

    void IEndDragHandler.OnEndDrag(PointerEventData data)
    {
        if (!canDrag)
        {
            return;
        }
        if (beginDrag && dragGameObject != null)
        {
            ExecuteEvents.Execute(dragGameObject, data, ExecuteEvents.endDragHandler);//响应点击位置上的UI
        }
        transform.position = startPos;
        beginDrag = false;
        dragGameObject = null;
    }
}
