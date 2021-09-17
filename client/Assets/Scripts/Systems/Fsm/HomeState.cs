using System.Collections;
using MotionFramework.Config;
using UnityEngine;

namespace EG
{
    public class HomeState:IGameState
    {
        public const string name = "home";
        public override string Name
        {
            get { return name; }
        }

        public override IEnumerator OnEnter()
        {
           var handle= AssetManager.Instance.GetScene("home");
           while (!handle.IsDone)
           {
               Events<float>.Broadcast(EventsType.sceneLoadingPercent,handle.PercentComplete);
               yield return null;
           }
           Events<float>.Broadcast(EventsType.sceneLoadingPercent,1);
           IsStarted = true;
        }
       
        public override void OnUpdate()
        {
            
        }

        public override void OnExit()
        {
            
        }
    }
}