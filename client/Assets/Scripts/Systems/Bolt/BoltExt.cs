using System;
using System.Collections;
using System.Collections.Generic;
using Bolt;
using Ludiq;
using UnityEngine;

public static class BoltExt
{
    public static void RegisterEvt(GameObject target,Action<CustomEventArgs> BoltEvent)
    {
        EventBus.Register<CustomEventArgs>(new EventHook("Custom",target),BoltEvent);
    }
    
    public static void TriggerEvt(GameObject target, string name, params object[] args)
    {
        CustomEvent.Trigger(target,name,args);
    }

    public static void SetFlowMachineGraph(this GameObject gameObject,FlowMacro macro)
    {
        var flow = gameObject.GetComponent<FlowMachine>();
        if (flow==null)
        {
            flow = gameObject.AddComponent<FlowMachine>();
        }
        flow.nest.macro = macro;
    }
    
}
