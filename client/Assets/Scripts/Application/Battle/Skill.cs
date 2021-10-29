using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skill
{
   public enum SkillState
   {
      None,
      Init,
      Start,
      Update,
      End
   }

   private const float DURATION = 3;
   private float duration;
   private SkillState _state;
   private ArmController owner;

   public bool IsEnd
   {
      get
      {
         return _state == SkillState.End || _state == SkillState.Init;
      }
   }
   public void Init(ArmController owner)
   {
      this.owner = owner;
      this.duration = DURATION;
      _state = SkillState.Init;
   }

   public void Start()
   {
      if (_state==SkillState.Init)
      {
         _state = SkillState.Start;
         Debug.Log("attack");
         PlayACT();
      }
   }

   void PlayACT()
   {
      BoltExt.TriggerEvt(owner.gameObject,"onUseSkill");
   }

   public void Update()
   {
      if (_state==SkillState.Start)
      {
         _state = SkillState.Update;
      }

      if ( _state == SkillState.Update)
      {
         this.duration -= Time.deltaTime;
         if (this.duration<=0)
         {
            End();
         }
      }
     
   }

   public void End()
   {
      if ( _state == SkillState.Update)
      {
         _state = SkillState.End;
         owner.OnSkillEnd();
      }
   }
   
   public void Reset( )
   {
      this.duration = DURATION;
      _state = SkillState.Init;
   }
}
