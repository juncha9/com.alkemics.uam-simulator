using Sirenix.OdinInspector;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace Alkemic.UAM
{
    [Serializable]
    public class Task : BaseComponent
    {
        public enum TaskStatus
        {
            Waiting,
            Tasking,
            Completed,
            Inturrupted
        }

        [Serializable]
        public class TaskEvent : UnityEvent<Task> { }

        private const string GROUP_TASK = "Task";

        [CacheGroup]
        [Debug]
        private TaskControl parentTaskControl;
        public TaskControl ParentTaskControl => parentTaskControl;

        [PropertyGroup]
        [GUIColor("GetStatusColor")]
        [ShowInInspector]
        private TaskStatus status = TaskStatus.Waiting;
        public TaskStatus Status
        {
            protected set
            {
                status = value;
                if (status == TaskStatus.Tasking)
                {
                    this.enabled = true;
                }
                else
                {
                    this.enabled = false;
                }
            }
            get => status;
        }

#if UNITY_EDITOR
        private Color GetStatusColor()
        {
            switch (status)
            {
                case TaskStatus.Waiting:
                    return Color.gray;
                case TaskStatus.Tasking:
                    return Color.yellow;
                case TaskStatus.Completed:
                    return Color.green;
                case TaskStatus.Inturrupted:
                    return Color.red;
                default:
                    return Color.white;
            }
        }
#endif

        protected override void OnPreAwake()
        {
            base.OnPreAwake();
            this.CacheComponentInParent(ref parentTaskControl);
        }

        protected override void Awake()
        {
            base.Awake();
            Debug.Assert(parentTaskControl != null, $"[{DName}] {nameof(parentTaskControl)} is null", gameObject);

            this.enabled = false;
        }

        protected virtual void Update()
        {
            ParentTaskControl?.OnTaskUpdate.Invoke(this);
        }

        protected virtual void FixedUpdate()
        {
            ParentTaskControl?.OnTaskFixedUpdate.Invoke(this);
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
            Status = TaskStatus.Inturrupted;
        }

        public virtual void InitTask()
        {
            Status = TaskStatus.Tasking;
            ParentTaskControl?.OnTaskInited.Invoke(this);
        }

        public virtual void TickTask()
        {
            ParentTaskControl?.OnTaskTicked.Invoke(this);
        }

        public virtual void OverTask()
        {
            Status = TaskStatus.Completed;
            ParentTaskControl?.OnTaskOvered.Invoke(this);
        }

        public void Clear()
        {
            Status = TaskStatus.Waiting;
        }
    }


}
