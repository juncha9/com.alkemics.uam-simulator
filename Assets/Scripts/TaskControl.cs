﻿using Sirenix.OdinInspector;
using Sirenix.Serialization;
using Sirenix.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace UAM
{
   
    public class TaskControl : Behavior
    {

        [SerializeField]
        private GameObject m_Target;
        public GameObject target => m_Target;

        [ReadOnly, ShowInInspector]
        private Task m_CurTask = null;

        [ReadOnly, ShowInInspector]
        private Coroutine m_CurRoutine = null;

        [ReadOnly, ShowInInspector]
        private List<Task> m_TaskList = new List<Task>();
        public List<Task> taskList => m_TaskList;

        //[ReadOnly, ShowInInspector]
        //private Dictionary<Task.Type, Func<Task, IEnumerator>> logicDict = null;


        protected override void Awake()
        {
            base.Awake();

            var tasks = GetComponentsInChildren<Task>();
            this.taskList.AddRange(tasks);
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
            this.taskList.Add(_task);
        }

        IEnumerator CheckTaskRoutine()
        {
            WaitForSeconds delay = new WaitForSeconds(0.1f);
            WaitForSeconds sleep = new WaitForSeconds(1.0f);
            while(true)
            {
                m_CurTask = taskList.Where(x => x.isCompleted == false).FirstOrValue(null);
                if(m_CurTask != null)
                {
                    m_CurRoutine = m_CurTask.StartTask();
                    yield return m_CurRoutine;
                    m_CurTask.Complete();
                }
                else
                {
                    yield return sleep;
                }
                yield return delay;
            }
        }


    }


}
