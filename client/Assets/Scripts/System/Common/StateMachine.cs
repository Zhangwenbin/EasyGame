using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace EG
{
    public class StateMachine 
    {
        public const int INVALID_ID = -1;

        // 
        public delegate bool RequestMethod(StateMachine state, State next);
        // 
        public delegate void StateMethod(StateMachine state);

        // 
        public struct StateParam
        {
            public int pri;
            public string name;
            public StateMethod begin;
            public StateMethod main;
            public StateMethod late_main;
            public StateMethod end;
        };

        // state
        public class State
        {
            int m_Id;
            int m_Hash;
            StateParam m_StateParam;

            public StateParam param
            {
                set
                {
                    m_StateParam = value;
                }
                get
                {
                    return m_StateParam;
                }
            }

            public int id
            {
                set
                {
                    m_Id = value;
                }
                get
                {
                    return m_Id;
                }
            }

            public string name
            {
                set
                {
                    m_StateParam.name = value;
                    m_Hash = name.GetHashCode();
                }
                get
                {
                    return m_StateParam.name;
                }
            }

            public int priority
            {
                get
                {
                    return m_StateParam.pri;
                }
            }

            public virtual object work
            {
                set { }
                get { return null; }
            }

            public State()
            {
            }

            public State(int id, StateParam param)
            {
                m_Id = id;
                m_Hash = string.IsNullOrEmpty(param.name) ? 0 : param.name.GetHashCode();
                m_StateParam = param;
            }

            public virtual void SetOwner(object owner)
            {
            }

            public void Change(StateParam param)
            {
                m_Hash = param.name.GetHashCode();
                m_StateParam = param;
            }

            public void Change(StateMethod main)
            {
                m_StateParam.main = main;
            }

            public bool Equals(int hash)
            {
                if (m_Hash == hash)
                {
                    return true;
                }
                return false;
            }

            public bool Equals(string name)
            {
                if (this.name == name)
                {
                    return true;
                }
                return false;
            }

            public bool Equals(State state)
            {
                if (id == state.id)
                {
                    return true;
                }
                return false;
            }

            public virtual bool CanChange(State state)
            {
                return true;
            }

            public virtual void Begin(StateMachine state) { if (m_StateParam.begin != null) { m_StateParam.begin(state); } }
            public virtual void Main(StateMachine state) { if (m_StateParam.main != null) { m_StateParam.main(state); } }
            public virtual void LateMain(StateMachine state) { if (m_StateParam.late_main != null) { m_StateParam.late_main(state); } }
            public virtual void End(StateMachine state) { if (m_StateParam.end != null) { m_StateParam.end(state); } }
        };


        // 泛型
        public class State<T> : State
        {
            protected string m_TypeName;
            protected T m_Self;
            protected List<IEnumerator> m_Tasks = new List<IEnumerator>();
            public T self { get { return m_Self; } }
            public State() : base(0, new StateParam()) { }
            public virtual void Command(string cmd) { }
            public override void SetOwner(object owner) { m_Self = (T)owner; }
            public override void Main(StateMachine state) { UpdateTask(); }
            public override void LateMain(StateMachine state) { }
            public string TypeName { get { if (m_TypeName == null) m_TypeName = typeof(T).Name; return m_TypeName; } }
            public bool UpdateTask()
            {
                while (m_Tasks.Count > 0)
                {
                    IEnumerator task = m_Tasks[0];
                    if (task == null)
                    {
                        m_Tasks.RemoveAt(0);
                        continue;
                    }
                    if (task.MoveNext() == false)
                    {
                        m_Tasks.RemoveAt(0);
                        continue;
                    }
                    return true;
                }
                return false;
            }
            public void AddTask(IEnumerator task)
            {
                m_Tasks.Add(task);
            }
            public bool HasTask() { return m_Tasks.Count > 0; }
        };

        // 排序
        public class PriorityState : State
        {
            public PriorityState(int id, StateParam param) : base(id, param)
            {
            }
            public override bool CanChange(State state)
            {
                if (priority <= state.priority)
                {
                    return true;
                }
                return false;
            }
        };



        #region 字段

        List<State> m_MethodsList = new List<State>();

        object m_Owner = null;

        IPhase m_Phase = null;
        int m_Proc;

        float m_Time;
        float m_TimeSystem;
        int m_FrameCount;

        State m_StateRequest;
        State m_StateCurrent;
        State m_StatePrev;

        object m_RequestValue;

        bool m_PriorityUsed;

        RequestMethod m_RequestCallback;
        StateMethod m_ChangeCallback;

        #endregion


        #region 属性

        public int Proc
        {
            set { m_Proc = value; }
            get { return m_Proc; }
        }

        public float Time
        {
            set { m_Time = value; }
            get { return m_Time; }
        }

        public float TimeSystem
        {
            set { m_TimeSystem = value; }
            get { return m_TimeSystem; }
        }

        public int FrameCount
        {
            set { m_FrameCount = value; }
            get { return m_FrameCount; }
        }

        public IPhase phase { get { return m_Phase; } }

        #endregion


        public void AttachPhase(IPhase value) { m_Phase = value; }

        public void PriorityEnable() { m_PriorityUsed = true; }

        public State GetCurrentState() { return m_StateCurrent; }   // 当前状态
        public int GetCurrentStateId() { return m_StateCurrent != null ? m_StateCurrent.id : INVALID_ID; }   // 当前状态id
        public string GetCurrentStateName() { return m_StateCurrent != null ? m_StateCurrent.name : ""; }   // 当前状态名字
        public int GetPrevStateId() { return m_StatePrev != null ? m_StatePrev.id : INVALID_ID; }   // 上一个状态id
        public string GetPrevStateName() { return m_StatePrev != null ? m_StatePrev.name : ""; }   // 上一个状态名字
        public bool IsRequestState() { return m_StateRequest != null ? true : false; }   // 是否在请求状态
        public int GetRequestState() { return m_StateRequest != null ? m_StateRequest.id : INVALID_ID; }   // 获得请求状态id
        public string GetRequestStateName() { return m_StateRequest != null ? m_StateRequest.name : ""; }   // 获得请求状态名字
        public bool EqualState(string name) { return GetCurrentStateName() == name; }   // 状态相同
        public bool EqualState(int id) { return GetCurrentStateId() == id; }   // 状态相同
        public bool EqualStateAndReq(string name) { return GetRequestStateName() == name || GetCurrentStateName() == name; }   // 正在或者正在请求
        public bool EqualStateAndReq(int id) { return GetRequestState() == id || GetCurrentStateId() == id; }   // 正在或者正在请求

        public void ResetTime() { m_Time = m_TimeSystem = 0.0f; }
        public float GetTime() { return m_Time; }
        public float GetTimeSystem() { return m_TimeSystem; }

        public void ResetFrameCount() { m_FrameCount = 0; }
        public int GetFrameCount() { return m_FrameCount; }
        public void SubFrameCount(int count) { m_FrameCount -= count; }

        // 
        public void SetRequestCallback(RequestMethod callback) { m_RequestCallback = callback; }
        // 
        public void SetChangeCallback(StateMethod callback) { m_ChangeCallback = callback; }

        // 
        protected virtual bool CallRequest(State state) { if (m_RequestCallback != null) { return m_RequestCallback(this, state); } return true; }
        protected virtual void CallChange() { if (m_ChangeCallback != null) { m_ChangeCallback(this); } }
        protected virtual void CallBegin() { if (m_StateCurrent != null) { m_StateCurrent.Begin(this); } }
        protected virtual void CallMain() { if (m_StateCurrent != null) { m_StateCurrent.Main(this); } }
        protected virtual void CallLateMain() { if (m_StateCurrent != null) { m_StateCurrent.LateMain(this); } }
        protected virtual void CallEnd() { if (m_StateCurrent != null) { m_StateCurrent.End(this); } }

        /// ***********************************************************************
        /// <summary>
        /// 初始化
        /// </summary>
        /// ***********************************************************************
        public void Initialize(object owner, IPhase phase)
        {
            m_MethodsList.Clear();

            m_Owner = owner;

            m_Phase = phase;

            m_Proc = 0;

            m_Time = 0.0f;
            m_TimeSystem = 0.0f;
            m_FrameCount = 0;

            m_StateRequest = null;
            m_StateCurrent = null;
            m_StatePrev = null;

            m_RequestValue = null;

            m_RequestCallback = null;
            m_ChangeCallback = null;
        }
        public void Initialize(object owner)
        {
            Initialize(owner, null);
        }
        public void Initialize()
        {
            Initialize(null, null);
        }

        /// ***********************************************************************
        /// <summary>
        /// Release
        /// </summary>
        /// ***********************************************************************
        public void Release()
        {
            m_MethodsList.Clear();
        }

        /// ***********************************************************************
        /// <summary>
        /// Reset
        /// </summary>
        /// ***********************************************************************
        public void Reset()
        {
            // 
            CallEnd();

            // 
            m_Proc = 0;
            m_Time = 0.0f;
            m_TimeSystem = 0.0f;
            m_FrameCount = 0;

            // 
            m_StateRequest = null;
            m_StateCurrent = null;
            m_StatePrev = null;
            m_RequestValue = null;
        }

        /// ***********************************************************************
        /// <summary>
        /// Reset
        /// </summary>
        /// ***********************************************************************
        public void Reset(State state)
        {
            // 
            CallEnd();

            // 
            m_Proc = 0;
            m_Time = 0.0f;
            m_TimeSystem = 0.0f;
            m_FrameCount = 0;

            // 
            m_StateRequest = null;
            m_StateCurrent = state;
            m_StatePrev = null;
            m_RequestValue = null;

            // 
            CallChange();

            // 
            CallBegin();
        }

        /// ***********************************************************************
        /// <summary>
        /// Reset
        /// </summary>
        /// ***********************************************************************
        public void Reset(string name)
        {
            Reset(GetState(name));
        }


        /// ***********************************************************************
        /// <summary>
        /// 更新
        /// </summary>
        /// ***********************************************************************
        public virtual void Update(bool updateState)
        {
            while (m_StateRequest != null)
            {
                // 
                CallEnd();

                // 
                m_StatePrev = m_StateCurrent;
                m_StateCurrent = m_StateRequest;
                m_StateRequest = null;

                m_StateCurrent.work = m_RequestValue;
                m_RequestValue = null;

                m_Proc = 0;
                m_Time = 0.0f;
                m_TimeSystem = 0.0f;
                m_FrameCount = 0;

                // 
                CallChange();

                // 
                CallBegin();
            }

            // 
            if (updateState)
            {
                CallMain();

                // 
                //m_Time += TimerManager.DeltaTime;
                //m_TimeSystem += TimerManager.DeltaTimeSystem;
                ++m_FrameCount;

                // 
                if (m_Phase != null)
                {
                    m_Phase.Update();
                }
            }
        }

        /// ***********************************************************************
        /// <summary>
        /// 更新
        /// </summary>
        /// ***********************************************************************
        public virtual void LateUpdate(bool updateState)
        {
            if (updateState)
            {
                CallLateMain();
            }
        }

        /// ***********************************************************************
        /// <summary>
        /// 更新
        /// </summary>
        /// ***********************************************************************
        public virtual void Update()
        {
            Update(true);
        }

        /// ***********************************************************************
        /// <summary>
        /// LateUpdate
        /// </summary>
        /// ***********************************************************************
        public virtual void LateUpdate()
        {
            LateUpdate(true);
        }


        /// ***********************************************************************
        /// <summary>
        /// CreateState
        /// </summary>
        /// <param name="id"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        /// ***********************************************************************
        protected virtual State CreateState(int id, StateParam param)
        {
            if (m_PriorityUsed)
            {
                return new PriorityState(id, param);
            }
            return new State(id, param);
        }

        /// ***********************************************************************
        /// <summary>
        /// GetState
        /// </summary>
        /// ***********************************************************************
        public virtual State GetState(string name)
        {
            int hash = name.GetHashCode();

            foreach (State methods in m_MethodsList)
            {
                if (methods.Equals(hash))
                {
                    if (methods.Equals(name))    // 
                    {
                        return methods;
                    }
                }
            }

            return null;
        }

        /// ***********************************************************************
        /// <summary>
        /// 
        /// </summary>
        /// ***********************************************************************
        public bool HasState(string name)
        {
            int hash = name.GetHashCode();

            foreach (State methods in m_MethodsList)
            {
                if (methods.Equals(hash))
                {
                    if (methods.Equals(name))    // 
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        /// ***********************************************************************
        /// ***********************************************************************
        public int AddState(string name, StateMethod begin, StateMethod main, StateMethod end, StateMethod late_main)
        {
            StateParam param;

            param.pri = 0;
            param.name = name;
            param.begin = begin;
            param.main = main;
            param.late_main = late_main;
            param.end = end;

            return AddState(param);
        }
        public int AddState(string name, StateMethod begin, StateMethod main, StateMethod end)
        {
            return AddState(name, begin, main, end, null);
        }

        /// ***********************************************************************

        /// ***********************************************************************
        public int AddState(StateParam param)
        {
            // 
            if (GetState(param.name) == null)
            {
                int id = m_MethodsList.Count;
                State state = CreateState(id, param);

                // 
                m_MethodsList.Add(state);

                return id;
            }
            return INVALID_ID;
        }

        /// ***********************************************************************
        /// ***********************************************************************
        public int AddState<T>(State<T> state)
        {
            string name = state.GetType().Name;

            // 
            if (GetState(name) == null)
            {
                state.SetOwner(m_Owner);
                state.id = m_MethodsList.Count;
                state.name = name;

                // 
                m_MethodsList.Add(state);

                return state.id;
            }

            return INVALID_ID;
        }

        /// ***********************************************************************
        /// ***********************************************************************
        public int ChangeState(string name, StateMethod begin, StateMethod main, StateMethod end, StateMethod late_main)
        {
            State state = GetState(name);

            if (state != null)
            {
                StateParam param;

                param.pri = 0;
                param.name = name;
                param.begin = begin;
                param.main = main;
                param.late_main = late_main;
                param.end = end;

                state.Change(param);

                return state.id;
            }

            return INVALID_ID;
        }
        public int ChangeState(string name, StateMethod begin, StateMethod main, StateMethod end)
        {
            return ChangeState(name, begin, main, end, null);
        }

        /// ***********************************************************************
        /// ***********************************************************************
        public int ChangeState(string name, StateMethod main)
        {
            State state = GetState(name);

            if (state != null)
            {
                state.Change(main);

                return state.id;
            }

            return INVALID_ID;
        }

        /// ***********************************************************************
        /// ***********************************************************************
        public void RemoveState(State state)
        {
            if (state != null)
            {
                m_MethodsList.Remove(state);
            }
        }

        /// ***********************************************************************
        /// ***********************************************************************
        public void RemoveState(string name)
        {
            RemoveState(GetState(name));
        }

        /// ***********************************************************************
        /// ***********************************************************************
        public virtual bool RequestState(State next, object value)
        {
            if (next != null && CallRequest(next))
            {
                bool change = true;

                if (next.priority == -1)
                {
                    // 
                    m_StateRequest = next;
                    m_RequestValue = value;
                    return true;
                }

                if (m_StateCurrent != null && m_StateCurrent.CanChange(next) == false)
                {
                    change = false;
                }

                if (change)
                {
                    if (m_StateRequest == null || m_StateRequest.CanChange(next))
                    {
                        m_StateRequest = next;
                        m_RequestValue = value;
                        return true;
                    }
                }
            }

            return false;
        }

        /// ***********************************************************************
        /// ***********************************************************************
        public virtual bool RequestState(string name, object value)
        {
            State state = GetState(name);
            if (state == null)
            {
                return false;
            }
            return RequestState(state, value);
        }
        public virtual bool RequestState(string name)
        {
            State state = GetState(name);
            if (state == null)
            {
                return false;
            }
            return RequestState(state, null);
        }
    }
}