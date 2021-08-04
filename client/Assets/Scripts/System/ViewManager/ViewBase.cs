using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ViewBase 
{
    public Object openParam;
    public virtual ViewLayer layer { get; }
    public virtual void Load() { }
    public virtual void OnLoaded() { }
    public virtual void OnShow() { }

    public virtual void OnHide() { }

    public virtual void OnDestroy() { }
}

public enum ViewLayer
{
    window,
}