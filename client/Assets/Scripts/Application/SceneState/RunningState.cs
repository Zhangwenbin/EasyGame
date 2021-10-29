using System.Collections;
using MotionFramework.Config;
using UnityEngine;

namespace EG.SceneState
{
    public class RunningState:IState
    {
        public const string name = "RunningState";
        public override string Name
        {
            get { return name; }
        }

        public override IEnumerator OnEnter(FsmMachine fsmMachine)
        {
            yield return base.OnEnter(fsmMachine);
        }

        public override void OnUpdate()
        {
            
        }

        public override void OnExit()
        {
            
        }

        public override bool CanGoto(IState next)
        {
            return true;
        }
    }
}