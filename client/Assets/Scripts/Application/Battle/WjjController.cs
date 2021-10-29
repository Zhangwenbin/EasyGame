using System;
using System.Collections;
using System.Collections.Generic;
using EG;
using UnityEngine;
using UnityEngine.AI;
using Object = System.Object;

public class WjjController : MonoBehaviour
{
    private bool isReady;
    
    //property
    [Range(1,100)]
    public float hp;
    [Range(1,10)]
    public float moveSpeed;
    [Range(0.2f,1)]
    public float rotateSpeed;
    
    //move
    private Vector3 moveDir;


    private BodyController body;
    private ArmController arm;
    private void Awake()
    {
        Init();
    }
    
    // Start is called before the first frame update
    void Init()
    {        
        AssetManager.Instance.GetAsset("Excavator_01.prefab",OnLoadBody);
        AssetManager.Instance.GetAsset("Arm01.prefab",OnLoadArm);
    }

    void OnLoadBody(string key, Object obj)
    {
        GameObject g = (GameObject) obj;
        body = g.GetComponent<BodyController>();
        body.Init(this);
    }
    
    void OnLoadArm(string key, Object obj)
    {
        GameObject g = (GameObject) obj;
        arm = g.GetComponent<ArmController>();
        arm.Init(this);
        isReady = true;
    }
    
   
}
