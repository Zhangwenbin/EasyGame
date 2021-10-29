using System.Collections;
using MotionFramework.Config;
using UnityEngine;

namespace EG.GameState
{
    public class BattleState:IState
    {
        public const string name = "BattleState";
        public override string Name
        {
            get { return name; }
        }

        public override IEnumerator OnEnter(FsmMachine fsmMachine)
        {
            Events<string>.AddListener(EventsType.onSceneLoaded,OnSceneLoaded);
            yield return base.OnEnter(fsmMachine);
            IsStarted = false; 
            SceneManager.Instance.LoadScene("battle");
           
        }

        private void OnSceneLoaded(string obj)
        {
            IsStarted = true;
        }

        public override void OnUpdate()
        {
            if (IsStarted)
            {
                
            }
        }

        public override void OnExit()
        {
            Events<string>.RemoveListener(EventsType.onSceneLoaded,OnSceneLoaded);
        }
    }
}