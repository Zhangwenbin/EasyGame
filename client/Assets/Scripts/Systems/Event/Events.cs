using System;
using UnityEngine;
using System.Collections.Generic;
using System.Reflection;

internal static class EventInternal
{
    public static Dictionary<object, Delegate> eventTable = new Dictionary<object, Delegate>();

    public static void OnListenerAdding(object eventType, Delegate listenerBeingAdded)
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

    public static void OnListenerRemoving(object eventType, Delegate listenerBeingRemoved)
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

    public static bool HasListener(object eventType)
    {
        return eventTable.ContainsKey(eventType);
    }

    public static void OnListenerRemoved(object eventType)
    {
        if (eventTable[eventType] == null)
        {
            eventTable.Remove(eventType);
        }
    }

    public static void OnBroadcasting(object eventType)
    {
        //
    }
}

//no parameter
public static class Events
{
    #region 常量与字段

    private static readonly Dictionary<object, Delegate> eventTable = EventInternal.eventTable;
    private static readonly Dictionary<object, Dictionary<string, object>> _listenerDic = new Dictionary<object, Dictionary<string, object>>();

    #endregion

    #region 方法

    public static void AddListener(object eventType, Action handler, Type type=null)
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
        if (type==null)
        {
            return;
        }
        Dictionary<string, object> types;
        if (!_listenerDic.ContainsKey(eventType))
        {
            types = new Dictionary<string, object>();
            types.Add(type.Name, eventType);
            _listenerDic.Add(eventType, types);
        }
        else
        {
            _listenerDic.TryGetValue(eventType, out types);
            if (types.ContainsKey(type.Name))
            {
                //addListener already!!!
            }
            else
            {
                types.Add(type.Name, eventType);
            }
        }
    }

    public static void RemoveListener(object eventType, Action handler, Type type=null)
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
        if (type == null)
        {
            return;
        }
        Dictionary<string, object> types;
        if (!_listenerDic.ContainsKey(eventType))
        {
            //no listener!!!
        }
        else
        {
            _listenerDic.TryGetValue(eventType, out types);
            if (types.ContainsKey(type.Name))
            {
                types.Remove(type.Name);
                if (types.Count == 0)
                {
                    _listenerDic.Remove(eventType);
                }
            }
            else
            {
                //no listener!!!
            }
        }
    }

    public static void RemoveAllListener()
    {
        List<object> eventTypes = new List<object>();
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
        _listenerDic.Clear();
    }
    

    //dispatch
    public static void Broadcast(object eventType)
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

    private static readonly Dictionary<object, Delegate> eventTable = EventInternal.eventTable;
    private static readonly Dictionary<object, Dictionary<string, object>> _listenerDic = new Dictionary<object, Dictionary<string, object>>();

    #endregion

    #region 方法

    public static void AddListener(object eventType, Action<T> handler, Type type)
    {
        EventInternal.OnListenerAdding(eventType, handler);
        eventTable[eventType] = (Action<T>)eventTable[eventType] + handler;

        Dictionary<string, object> types;
        if (!_listenerDic.ContainsKey(eventType))
        {
            types = new Dictionary<string, object>();
            types.Add(type.Name, eventType);
            _listenerDic.Add(eventType, types);
        }
        else
        {
            _listenerDic.TryGetValue(eventType, out types);
            if (types.ContainsKey(type.Name))
            {
                //addListener already!!!
            }
            else
            {
                types.Add(type.Name, eventType);
            }
        }
    }

    public static void RemoveListener(object eventType, Action<T> handler, Type type)
    {
        if (!EventInternal.HasListener(eventType)) return;
        EventInternal.OnListenerRemoving(eventType, handler);
        eventTable[eventType] = (Action<T>)eventTable[eventType] - handler;
        EventInternal.OnListenerRemoved(eventType);

        Dictionary<string, object> types;
        if (!_listenerDic.ContainsKey(eventType))
        {
            //no listener!!!
        }
        else
        {
            _listenerDic.TryGetValue(eventType, out types);
            if (types.ContainsKey(type.Name))
            {
                types.Remove(type.Name);
                if (types.Count == 0)
                {
                    _listenerDic.Remove(eventType);
                }
            }
            else
            {
                //no listener!!!
            }
        }
    }

    //dispatch
    public static void Broadcast(object eventType, T arg1)
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
