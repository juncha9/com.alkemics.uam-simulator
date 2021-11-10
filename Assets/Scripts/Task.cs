using Sirenix.OdinInspector;
using Sirenix.Serialization;
using System;
using System.Collections;
using UnityEngine;

namespace UAM
{
    
    [IncludeMyAttributes()]
    [ShowIf("@useDebug == true")]
    [VerticalGroup("Debug")]
    public class DebugOnly : Attribute
    {

    }

    [Serializable]
    public class EVTOL_MoveTask : Task
    {
        [ReadOnly, ShowInInspector]
        private EVTOL m_Target;
        public EVTOL target => m_Target;

        private Location m_DestLocation = null;
        public Location destLocation
        {
            set => m_DestLocation = value;
            get => m_DestLocation;
        }

        protected override void OnPreAwake()
        {
            base.OnPreAwake();
            if (m_Target == null)
            {
                m_Target = GetComponentInParent<EVTOL>();
            }
        }

        public override IEnumerator TaskRoutine()
        {
            isTasking = true;
            yield return target.MoveToLocationRoutine(destLocation);
            isTasking = false;
        }
    }


    [Serializable]
    public class Task : Behavior
    {
        private const string GROUP_TASK = "Task";


        [DebugOnly]
        private TaskControl m_ParentTaskControl;
        public TaskControl parentTaskControl => m_ParentTaskControl;

        [HorizontalGroup(GROUP_TASK)]
        [ReadOnly, ShowInInspector]
        private bool m_IsCompleted = false;
        public bool isCompleted
        {
            private set => m_IsCompleted = value;
            get => m_IsCompleted;
        }

        [HorizontalGroup(GROUP_TASK)]
        [ReadOnly, ShowInInspector]
        private bool m_IsTasking = false;
        public bool isTasking
        {
            protected set => m_IsTasking = value;
            get => m_IsTasking;
        }
        

        protected override void OnPreAwake()
        {
            base.OnPreAwake();
            if(m_ParentTaskControl == null)
            {
                m_ParentTaskControl = GetComponentInParent<TaskControl>();
            }
        }

        protected override void Awake()
        {
            base.Awake();
            Debug.Assert(m_ParentTaskControl != null, $"[{name}] {nameof(m_ParentTaskControl)} is null", gameObject);
        }

        public Coroutine StartTask()
        {
            return StartCoroutine(TaskRoutine());
        }
        public virtual IEnumerator TaskRoutine()
        {
            isTasking = true;
            yield return null;
            isTasking = false;
        }

        public void Complete()
        {
            this.isCompleted = true;
        }

    }


}
