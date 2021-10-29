using System.Collections;
using MotionFramework.Config;
using UnityEngine;

namespace EG.GameState
{
    public class HomeState:IState
    {
        public const string name = "HomeState";
        public override string Name
        {
            get { return name; }
        }

        public override IEnumerator OnEnter(FsmMachine fsmMachine)
        {
            Events<string>.AddListener(EventsType.onSceneLoaded,OnSceneLoaded);
            yield return base.OnEnter(fsmMachine);
            
            yield return WindowManager.Instance.LoadWindow<UIStartMenu>();
            SceneManager.Instance.LoadScene("home");
           
        }

        private void OnSceneLoaded(string obj)
        {
            WindowManager.Instance.OpenWindow<UIStartMenu>();
        }

        public override void OnUpdate()
        {
            
        }

        public override void OnExit()
        {
            Events<string>.RemoveListener(EventsType.onSceneLoaded,OnSceneLoaded);
        }
    }
}