using Sirenix.OdinInspector;
using Sirenix.Serialization;
using System;
using System.Collections;
using UnityEngine;

namespace UAM
{
    public class EVTOL_MoveTask : Task
    {

        [ReadOnly, ShowInInspector]
        private EVTOL m_Target;

        [SerializeField]
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
            yield return m_Target.MoveToLocationRoutine(destLocation);
            isTasking = false;
        }
    }


    [Serializable]
    public class Task : Behavior
    {
        [ReadOnly, ShowInInspector]
        private TaskControl m_ParentTaskControl;
        public TaskControl parentTaskControl => m_ParentTaskControl;

        [ReadOnly, ShowInInspector]
        private bool m_IsCompleted = false;
        public bool isCompleted
        {
            private set => m_IsCompleted = value;
            get => m_IsCompleted;
        }

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
