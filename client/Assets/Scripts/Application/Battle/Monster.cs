using System;
using System.Collections;
using System.Collections.Generic;
using Bolt;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UIElements;

public class Monster : MonoBehaviour
{
    //property
    [Range(1,100)]
    public float hp;
    [HideInInspector]
    public bool IsAlive;

    private void Awake()
    {
        IsAlive = true;
        BoltExt.RegisterEvt(gameObject,OnBoltEvent);
    }

    public void OnDamage (float damageValue)
    {
        if (!IsAlive)
        {
            return;
        }
        hp -= damageValue;
        Debug.Log("attackdamage:"+damageValue+" currenthp"+hp);
        PlayOnDamageEffect(damageValue);
        if (hp<=0)
        {
            hp = 0;
            IsAlive = false;
            PlayOnKilledEffect();
            return;
        }
    }
    
    public void PlayOnDamageEffect (float damageValue)
    {
         BoltExt.TriggerEvt(gameObject,"onGetHit",hp*0.01f);
    }
    public void PlayOnKilledEffect ()
    {
        BoltExt.TriggerEvt(gameObject,"onKilled",1);
    }
    
    public void TriggerAnimationEvent(AnimationEvent animationEvent)
    {
        Debug.Log(animationEvent.stringParameter);
        if (animationEvent.stringParameter=="onend")
        {
            if (!IsAlive)
            {
                GameObject.Destroy(gameObject);
            }
        }
    }

    
    private void OnBoltEvent(CustomEventArgs obj)
    {
        if (obj.name=="died")
        {
            GameObject.Destroy(gameObject);
        }
    }
}
