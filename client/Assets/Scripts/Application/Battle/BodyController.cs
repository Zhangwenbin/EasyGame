using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BodyController : MonoBehaviour
{
       //ref
       private Animator animator;
       private WjjController parent;

       public void Init(WjjController parent)
       {
              this.parent = parent;
              animator = GetComponent<Animator>();
              transform.parent = parent.transform;
              transform.localPosition = Vector3.zero;
       }

 
    
}
