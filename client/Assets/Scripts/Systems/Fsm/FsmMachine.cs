/**************************************************************************/
/*@brief  简要描述   
  @author zwb
***************************************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
namespace EG
{
    //=========================================================================
    //简要注释
    //=========================================================================
    public class FsmMachine
    {
        private Dictionary<string, IGameState> _states;
        private IGameState pretState { get;  set; }
        private IGameState requestState { get;  set; }
        public IGameState currentState { get; private set; }

        private MonoBehaviour owner;

        public void Initialize(string startState,MonoBehaviour owner)
        {
            this.owner = owner;
            _states = new Dictionary<string, IGameState>();
            var state = AddState(startState);
            StartCoroutine(state.OnEnter());
            currentState = state;
        }
        
        private Coroutine StartCoroutine(IEnumerator routine)
        {
            return owner.StartCoroutine(routine);
        }

        public void AddState(IGameState state)
        {
            var name = state.Name;
            if (!_states.ContainsKey(name))
            {
                _states.Add(name,state);
            }
        }
        
        public IGameState AddState(string name)
        {
            if (!_states.ContainsKey(name))
            {
                IGameState state;
                switch (name)
                {
                    case InitState.name:
                        state = new InitState();
                        break;
                    case HomeState.name:
                        state = new HomeState();
                        break;
                    default:
                        state = new IGameState();
                        break;
                }
                _states.Add(name,state);
            }
            return _states[name];
        }

        public bool HasState(string name)
        {
            return _states.ContainsKey(name);
        }

        public IGameState GetState(string name)
        {
            if (HasState(name))
            {
                return _states[name];
            }
            else
            {
                return AddState(name);
            }
        }
        
        public void GoTo(string name)
        {
            var next = GetState(name);
            if (CanGo(currentState,next))
            {
                Debug.Log("goto "+name);
                requestState = next;
            }
            else
            {
                Debug.LogWarning("can not goto "+name);
            }
            
        }

        public void Update()
        {
            while( requestState != null )
            {
                currentState.OnExit();
                
                pretState     = currentState;
                currentState  = requestState;
                requestState  = null;
                StartCoroutine(currentState.OnEnter());
            }

            if (currentState!=null)
            {
                currentState.OnUpdate();
            }
                
        }

        private bool CanGo(IGameState cur, IGameState next)
        {
            return cur.Name != next.Name;
        }
        //=========================================================================
        //Editor 编辑器
        //=========================================================================
        #region 编辑器
        #if UNITY_EDITOR

        #endif
        #endregion
    }

    public class IGameState
    {
        public bool IsStarted;
        public virtual string Name { get;}

        public virtual IEnumerator OnEnter()
        {
            IsStarted = true;
            yield return null;
        }

        public virtual void OnUpdate()
        {
            if (!IsStarted)
            {
                return;
            }
        }

        public virtual void OnExit()
        {
            IsStarted = false;
        }
    }
  
}