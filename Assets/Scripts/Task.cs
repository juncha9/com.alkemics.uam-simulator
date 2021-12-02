using Alkemic;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace UAM
{
    [Serializable]
    public class Task : BaseComponent
    {
        [Serializable]
        public class TaskEvent : UnityEvent<Task> { }
        
        private const string GROUP_TASK = "Task";

        [DebugOnly]
        private TaskControl m_ParentTaskControl;
        public TaskControl parentTaskControl => m_ParentTaskControl;

        private bool m_IsTasking = false;
        [HorizontalGroup(GROUP_TASK)]
        [GUIColor("isTasking ? Color.green : Color.red")]
        [HideLabel]
        [ReadOnly, ShowInInspector]
        public bool isTasking
        {
            protected set => m_IsTasking = value;
            get => m_IsTasking;
        }

        private bool m_IsCompleted = false;
        [HorizontalGroup(GROUP_TASK)]
        [GUIColor("isCompleted ? Color.green : Color.yellow")]
        [ReadOnly, ShowInInspector]
        public bool isCompleted
        {
            protected set => m_IsCompleted = value;
            get => m_IsCompleted;
        }

        private bool m_IsInterrupted = false;
        [HorizontalGroup(GROUP_TASK)]
        [GUIColor("isInterrupted ? Color.red : Color.white")]
        [HideLabel]
        [ReadOnly, ShowInInspector]
        public bool isInterrupted
        {
            private set => m_IsInterrupted = value;
            get => m_IsInterrupted;
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
            InitTask();
            yield return null;
            TickTask();
            OverTask();
        }

        public void Inturrupt()
        {
            isInterrupted = true;
        }

        public virtual void InitTask()
        {
            isTasking = true;
            parentTaskControl?.onTaskInit.Invoke(this);
        }

        public virtual void TickTask()
        {
            parentTaskControl?.onTaskTick.Invoke(this);
        }

        public virtual void OverTask()
        {
            isTasking = false;
            isCompleted = true;
            parentTaskControl?.onTaskOver.Invoke(this);
        }

        public void Clear()
        {
            isInterrupted = false;
            isCompleted = false;
        }
    }


}
