using Sirenix.OdinInspector;
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

        private Coroutine StartRoutine(in Task task)
        {
            var logic = task.logic;
            return StartCoroutine(logic.Invoke(task));
        }

        /*
        public void SetTaskLogic(Dictionary<Task.Type, Func<Task, IEnumerator>> dict)
        {
            this.logicDict = dict;
            foreach(var task in taskList)
            {
                if(dict.ContainsKey(task.type) == true)
                {
                    task.SetLogic(dict[task.type]);
                }
            }
        }
        */

        IEnumerator CheckTaskRoutine()
        {
            WaitForSeconds delay = new WaitForSeconds(0.1f);
            WaitForSeconds sleep = new WaitForSeconds(1.0f);
            while(true)
            {
                m_CurTask = taskList.Where(x => x.isCompleted == false).FirstOrValue(null);
                if(m_CurTask != null)
                {
                    var routine = StartRoutine(m_CurTask);
                    yield return routine;
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
