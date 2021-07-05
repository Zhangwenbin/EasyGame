using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace EG
{
    public class IPhase
    {
        public virtual void Update() { }
    }

    public class Phase<T> : IPhase
    {
        #region 字段

        T m_Phase;            // 阶段
        int m_Count;            // 计数
        float m_Time;             // 经过时间
        //SerializeValueList m_ValueList;        // 

        #endregion

        #region 属性

        //public SerializeValueList ValueList
        //{
        //    get
        //    {
        //        if (m_ValueList == null) m_ValueList = new SerializeValueList();
        //        return m_ValueList;
        //    }
        //}

        public override string ToString()
        {
            return string.Format("[Phase: phase={0}, count={1}, time={2}]", GetPhase(), GetCount(), GetTime());
        }

        #endregion

        /// ***********************************************************************
        /// <summary>
        /// 初始化
        /// </summary>
        /// ***********************************************************************
        public void Initialize()
        {
            m_Phase = default(T);
            m_Count = 0;
            m_Time = 0;
        }

        /// ***********************************************************************
        /// <summary>
        /// 释放
        /// </summary>
        /// ***********************************************************************
        public void Release()
        {
        }

        /// ***********************************************************************
        /// <summary>
        /// 重置
        /// </summary>
        /// ***********************************************************************
        public void Reset()
        {
        }

  
        /// ***********************************************************************
        /// <summary>
        /// 更新
        /// </summary>
        /// ***********************************************************************
        public override void Update()
        {
            //m_Time += sysTimerManager.deltaTime;
        }

        /// ***********************************************************************
        /// <summary>
        /// 更新时间
        /// </summary>
        /// ***********************************************************************
        public void UpdateTime(float time)
        {
            m_Time += time;
        }
        //public void UpdateTime()
        //{
        //    UpdateTime(TimerManager.DeltaTime);
        //}

  
        /// ***********************************************************************
        /// <summary>
        /// 设定阶段
        /// </summary>
        /// ***********************************************************************
        public void SetPhase(T phase)
        {
            m_Phase = phase;
            m_Count = 0;
            m_Time = 0.0f;
        }

        /// ***********************************************************************
        /// <summary>
        /// 获取阶段
        /// </summary>
        /// ***********************************************************************
        public T GetPhase()
        {
            return m_Phase;
        }

        /// ***********************************************************************
        /// <summary>
        /// 跳转计数
        /// </summary>
        /// ***********************************************************************
        public void JumpCount(int value)
        {
            m_Count = value;
        }
        public void IncCount()
        {
            ++m_Count;
        }

        /// ***********************************************************************
        /// <summary>
        /// 获取计数
        /// </summary>
        /// ***********************************************************************
        public int GetCount()
        {
            return m_Count;
        }

        /// ***********************************************************************
        /// <summary>
        /// 设定经过时间
        /// </summary>
        /// ***********************************************************************
        public void SetTime(float value)
        {
            m_Time = value;
        }

        /// ***********************************************************************
        /// <summary>
        /// 获取时间
        /// </summary>
        /// ***********************************************************************
        public float GetTime()
        {
            return m_Time;
        }
    }
}