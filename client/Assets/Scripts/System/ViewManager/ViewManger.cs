using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ViewManger : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public ViewBase ShowView<T>() where T:ViewBase
    {
        var view= System.Activator.CreateInstance<T>();
        return view;
    }

    public void HideView()
    {

    }
}
