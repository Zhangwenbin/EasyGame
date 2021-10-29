/**************************************************************************/
/*@brief  简要描述   
  @author zwb
***************************************************************************/

using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
namespace EG
{
    //=========================================================================
    //简要注释
    //=========================================================================
    public class IState
    {
        public bool IsStarted;
        public virtual string Name { get;}

        public object arg;
        private FsmMachine _fsmMachine;
        
        public virtual IEnumerator OnEnter(FsmMachine fsmMachine)
        {
            _fsmMachine = fsmMachine;
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

        public virtual bool CanGoto(IState next)
        {
            return true;
        }
        
        public void GotoState(string next,object args=null)
        {
            if (_fsmMachine!=null)
            {
                _fsmMachine.GoTo(next,args);
            }
        }
    }
    public class FsmMachine
    {
        private Dictionary<string, IState> _states;
        private IState pretState { get;  set; }
        private IState requestState { get;  set; }
        public IState currentState { get; private set; }

        private MonoBehaviour owner;

        public void Initialize(string startState,MonoBehaviour owner,string nameSpace)
        {
            this.owner = owner;
            _states = new Dictionary<string, IState>();
            InitStates(nameSpace);
            var state = GetState(startState);
            if (state!=null)
            {
                StartCoroutine(state.OnEnter(this));
                currentState = state;
            }
        }

        private void InitStates(string nameSpace)
        {
            foreach( System.Reflection.Assembly assembly in System.AppDomain.CurrentDomain.GetAssemblies() )
            {
                if( assembly.FullName.StartsWith( "Mono.Cecil" ) ) continue;
                if( assembly.FullName.StartsWith( "UnityScript" ) ) continue;
                if( assembly.FullName.StartsWith( "Boo.Lan" ) ) continue;
                if( assembly.FullName.StartsWith( "System" ) ) continue;
                if( assembly.FullName.StartsWith( "I18N" ) ) continue;
                if( assembly.FullName.StartsWith( "UnityEngine" ) ) continue;
                if( assembly.FullName.StartsWith( "UnityEditor" ) ) continue;
                if( assembly.FullName.StartsWith( "mscorlib" ) ) continue;
                // Debug.Log(assembly.GetName());
                System.Type[] types = assembly.GetTypes();
                
                foreach( System.Type type in types )
                {
                    if( type.IsClass && !type.IsAbstract && type.IsSubclassOf( typeof(IState) )&&nameSpace.Equals(type.Namespace) )
                    {
                        var state = (IState)Activator.CreateInstance(type);
                        var name = state.Name;
                        Debug.Log(name);
                        if (!_states.ContainsKey(name))
                        {
                            _states.Add(name,state);
                        }
                    }
                }
            }
        }
        
        private Coroutine StartCoroutine(IEnumerator routine)
        {
            return owner.StartCoroutine(routine);
        }
        
        public bool HasState(string name)
        {
            return _states.ContainsKey(name);
        }

        public IState GetState(string name)
        {
            if (HasState(name))
            {
                return _states[name];
            }
            else
            {
                return null;
            }
        }
        
        public void GoTo(string name,object arg=null)
        {
            var next = GetState(name);
            if (CanGo(currentState,next))
            {
                Debug.Log("goto "+name);
                requestState = next;
                requestState.arg = arg;
            }
            else
            {
                Debug.LogError("can not goto "+name);
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
                StartCoroutine(currentState.OnEnter(this));
            }

            if (currentState!=null)
            {
                currentState.OnUpdate();
            }
                
        }

        private bool CanGo(IState cur, IState next)
        {
            return cur.CanGoto(next);
        }
    }
}