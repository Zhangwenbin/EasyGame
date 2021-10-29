using System;
using UnityEngine;
using System.Collections.Generic;
using System.Reflection;

internal static class EventInternal
{
    public static Dictionary<EventsType, Delegate> eventTable = new Dictionary<EventsType, Delegate>();

    public static void OnListenerAdding(EventsType eventType, Delegate listenerBeingAdded)
    {
        if (!eventTable.ContainsKey(eventType))
        {
            eventTable.Add(eventType, null);
        }

        Delegate d = eventTable[eventType];
        if (d != null && d.GetType() != listenerBeingAdded.GetType())
        {
            Debug.LogError("监听失败 请检查要加入的监听函数");
        }
    }

    public static void OnListenerRemoving(EventsType eventType, Delegate listenerBeingRemoved)
    {
        if (eventTable.ContainsKey(eventType))
        {
            Delegate d = eventTable[eventType];
            if (d == null)
            {
                Debug.Break();
            }
            else if (d.GetType() != listenerBeingRemoved.GetType())
            {
                Debug.Break();
            }
        }
        else
        {
            // DebugUtility.Assert(false);
        }
    }

    public static bool HasListener(EventsType eventType)
    {
        return eventTable.ContainsKey(eventType);
    }

    public static void OnListenerRemoved(EventsType eventType)
    {
        if (eventTable[eventType] == null)
        {
            eventTable.Remove(eventType);
        }
    }

    public static void OnBroadcasting(EventsType eventType)
    {
        //
    }
}

//no parameter
public static class Events
{
    #region 常量与字段

    private static readonly Dictionary<EventsType, Delegate> eventTable = EventInternal.eventTable;

    #endregion

    #region 方法

    public static void AddListener(EventsType eventType, Action handler)
    {
        EventInternal.OnListenerAdding(eventType, handler);
        Delegate d = eventTable[eventType];
        if (d != null && d.GetType() != handler.GetType())
        {
            return;
        }
        else
        {
           eventTable[eventType] = (Action)eventTable[eventType] + handler;
        }
       
    }

    public static void RemoveListener(EventsType eventType, Action handler)
    {
        if (!EventInternal.HasListener(eventType)) return;

        //EventInternal.OnListenerRemoving(eventType, handler);
        Delegate d = eventTable[eventType];

        if (d != null && d.GetType() == handler.GetType())
        {
            eventTable[eventType] = (Action)eventTable[eventType] - handler;
            EventInternal.OnListenerRemoved(eventType);
        }
        else
        {
            Debug.LogError("消息的监听函数和要移除的函数不一致");   
        }
    }

    public static void RemoveAllListener()
    {
        List<EventsType> eventTypes = new List<EventsType>();
        List<Action> handlers = new List<Action>();
        var iter = eventTable.GetEnumerator();
        while (iter.MoveNext())
        {
            var et = iter.Current;
            EventInternal.OnListenerRemoving(et.Key, et.Value);
            eventTypes.Add(et.Key);
            if (et.Value is Action)
            {
                handlers.Add((Action)et.Value);
            }
        }

        for (int i = 0; i < handlers.Count; i++)
        {
            var handle = handlers[i];
            handle = (Action)handle - handle;
        }

        for (int i = 0; i < eventTypes.Count; i++)
        {
            EventInternal.OnListenerRemoved(eventTypes[i]);
        }

        eventTable.Clear();
    }
    

    //dispatch
    public static void Broadcast(EventsType eventType)
    {

        EventInternal.OnBroadcasting(eventType);
        Delegate d;
        if (eventTable.TryGetValue(eventType, out d))
        {
            var callback = d as Action;
            if (callback != null)
            {
                callback();
            }
            else
            {
                //  Debug.Break();
                //Logger.Message(string.Format("Event:{0} is no listener!!!", eventType.ToString()));
            }
            
        }
    }
    #endregion
}

//One parameter
public static class Events<T>
{
    #region 常量与字段

    private static readonly Dictionary<EventsType, Delegate> eventTable = EventInternal.eventTable;

    #endregion

    #region 方法

    public static void AddListener(EventsType eventType, Action<T> handler)
    {
        EventInternal.OnListenerAdding(eventType, handler);
        eventTable[eventType] = (Action<T>)eventTable[eventType] + handler;
       
    }

    public static void RemoveListener(EventsType eventType, Action<T> handler)
    {
        if (!EventInternal.HasListener(eventType)) return;
        EventInternal.OnListenerRemoving(eventType, handler);
        eventTable[eventType] = (Action<T>)eventTable[eventType] - handler;
        EventInternal.OnListenerRemoved(eventType);
       
    }

    //dispatch
    public static void Broadcast(EventsType eventType, T arg1)
    {
        EventInternal.OnBroadcasting(eventType);
        Delegate d;
        if (eventTable.TryGetValue(eventType, out d))
        {
            var callback = d as Action<T>;
            if (callback != null)
            {
                callback(arg1);
            }
            else
            {
                //  Debug.Break();
                //Logger.Error(string.Format("Event:{0} is no listener!!!", eventType.ToString()));
            }

        }
    }

    #endregion
}

public enum EventsType
{
    //场景相关
    sceneLoadingPercent=0,
    onSceneLoaded,
    
    //资源相关
    assetStatusChange=100,
    assetProgressChange=101,
}
