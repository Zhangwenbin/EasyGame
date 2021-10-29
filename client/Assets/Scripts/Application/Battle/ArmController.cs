using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArmController : MonoBehaviour
{
    
    //ref
    //property

    public float attack;

    private Monster monster;
    
    private WjjController parent;

    private Skill curSkill;
    // Start is called before the first frame update
    public void Init(WjjController parent)
    {
        this.parent = parent;
        transform.parent = parent.transform;
        transform.localPosition=Vector3.zero;
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.layer==LayerMask.NameToLayer("monster"))
        {
            monster = other.GetComponent<Monster>();
            Attack();
        }
       
    }

    public void Attack()
    {
        if (curSkill==null)
        {
            curSkill = new Skill();
            curSkill.Init(this);
            curSkill.Start();
        }
        else
        {
            if (curSkill.IsEnd)
            {
                curSkill.Reset();
                curSkill.Start();
            }
            else
            {
                //打断
            }
        }
        
    }

    private Monster FindMonster()
    {
        return monster;
    }

    public void OnAttack()
    {
        var monster = FindMonster();
        if (monster!=null&&monster.IsAlive)
        {
            BoltExt.TriggerEvt(gameObject,"onHitMonster");
            monster.OnDamage(attack);
        }
    }

    public void OnSkillEnd()
    {
        
       
    }
    
    public void TriggerAnimationEvent(AnimationEvent animationEvent)
    {
        Debug.Log(animationEvent.stringParameter);
        if (animationEvent.stringParameter=="onhit")
        {
            OnAttack();
        }else if (animationEvent.stringParameter=="onend")
        {
          
        }
    }

   

    private void Update()
    {
        if (curSkill!=null)
        {
            curSkill.Update();
        }
    }
    
    
}
