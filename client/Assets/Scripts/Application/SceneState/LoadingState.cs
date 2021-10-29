using System.Collections;
using MotionFramework.Config;
using UnityEngine;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace EG.SceneState
{
    public class LoadingState:IState
    {
        public const string name = "LoadingState";
        public override string Name
        {
            get { return name; }
        }

        private AsyncOperationHandle handle;
        public override IEnumerator OnEnter(FsmMachine fsmMachine)
        {
            yield return base.OnEnter(fsmMachine);
            var sceneName = arg.ToString();
             handle = AssetManager.Instance.GetScene(sceneName);
             IsStarted = true;
             yield return null;
        }
       
        public override void OnUpdate()
        {
            if (!handle.IsDone)
            {
                Events<float>.Broadcast(EventsType.sceneLoadingPercent,handle.PercentComplete);
                return;
            }
            GotoState(RunningState.name);
        }

        public override void OnExit()
        {
            Events<float>.Broadcast(EventsType.sceneLoadingPercent,1);
            WindowManager.Instance.CloseWindow<UILoading>();
        }
    }
}