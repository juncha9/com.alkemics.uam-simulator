using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

namespace Alkemic.UAM
{

    public class TaskControl : BaseComponent
    {
        public class TaskEvent : UnityEvent<Task> { }

        [PresetComponent]
        [SerializeField]
        private GameObject target;
        public GameObject Target => target;

        [RuntimeOnly]
        private Task curTask = null;

        [RuntimeOnly]
        private Coroutine curRoutine = null;

        [ShowInInspector]
        public bool isTasking
        {
            get
            {
                if (curTask == null) return false;
                else return true;
            }
        }

        [InstanceGroup]
        [RuntimeOnly]
        private List<Task> taskList = new List<Task>();
        public List<Task> TaskList => taskList;

        private TaskEvent onTaskInited = new TaskEvent();
        public TaskEvent OnTaskInited => onTaskInited;

        private TaskEvent onTaskTicked = new TaskEvent();
        public TaskEvent OnTaskTicked => onTaskTicked;

        private TaskEvent onTaskUpdate = new TaskEvent();
        public TaskEvent OnTaskUpdate => onTaskUpdate;

        private TaskEvent onTaskFixedUpdate = new TaskEvent();
        public TaskEvent OnTaskFixedUpdate => onTaskFixedUpdate;

        private TaskEvent onTaskOvered = new TaskEvent();
        public TaskEvent OnTaskOvered => onTaskOvered;

        protected override void Awake()
        {
            base.Awake();

            var tasks = GetComponentsInChildren<Task>();
            this.TaskList.AddRange(tasks);
        }

        [Button]
        public void Interrupt()
        {
            if (curTask == null) return;
            if (IsDebug == true) { Debug.Log($"[{name}] Task[{curTask.GetType()}] has Inturrupted", gameObject); }
            curTask.Inturrupt();
        }


        [Button]
        public void StartTasks()
        {
            StartAutoCoroutine(CheckTaskRoutine());
        }

        public void AddTask<T>(Action<T> initCallback = null) where T : Task
        {
            T _task = gameObject.AddComponent<T>();
            initCallback?.Invoke(_task);
            this.TaskList.Add(_task);
        }

        IEnumerator CheckTaskRoutine()
        {
            WaitForSeconds delay = new WaitForSeconds(0.02f);
            while (true)
            {
                curTask = TaskList.Where(x => x.Status == Task.TaskStatus.Waiting).FirstOrValue(null);
                if (curTask != null)
                {
                    curRoutine = curTask.StartTask();
                    if (IsDebug == true) { Debug.Log($"[{name}] Task{curTask.GetType()} started", gameObject); }
                    yield return curRoutine;
                }
                else
                {
                    yield return delay;
                }
                yield return delay;
            }
        }


    }


}
