using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace EG
{
    public partial class UIContentController : AppMonoBehaviour
    {
        
        public float paddingLeft                        { get { return m_PaddingLeft; }             set { m_PaddingLeft = value; } }
        public float paddingRight                       { get { return m_PaddingRight; }            set { m_PaddingRight = value; } }
        public float paddingTop                         { get { return m_PaddingTop; }              set { m_PaddingTop = value; } }
        public float paddingBottom                      { get { return m_PaddingBottom; }           set { m_PaddingBottom = value; } }
        public Vector2 cellSize                         { get { return m_CellSize; }                set { m_CellSize = value; } }
        public Vector2 spacing                          { get { return m_Spacing; }                 set { m_Spacing = value; } }
        public Constraint constraint                    { get { return m_Constraint; }              set { m_Constraint = value; } }            
        public int constraintCount                      { get { return m_ConstraintCount; }         set { m_ConstraintCount = value; } }
        public int constraintCountSub                   { get { return m_ConstraintCountSub; }      set { m_ConstraintCountSub = value; } }
        public bool periodicZeroBoundary                { get { return m_PeriodicZeroBoundary; }    set { m_PeriodicZeroBoundary = value; } }
        public bool keepsNodeIndexOrder                 { get { return m_KeepsNodeIndexOrder; }     set { m_KeepsNodeIndexOrder = value; } }
        public bool notMoveRefresh                      { get { return m_NotMoveRefresh; }          set { m_NotMoveRefresh = value; } }
    }
}
