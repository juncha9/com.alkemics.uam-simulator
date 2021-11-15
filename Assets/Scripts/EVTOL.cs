using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

namespace UAM
{


    [RequireComponent(typeof(Looker))]
    [RequireComponent(typeof(Mover))]
    public class EVTOL : Behavior
    {
        public enum State
        {
            Idle,
            Move,
            TakeOff,
        }
      
        [Serializable]
        public class LocationEvent : UnityEvent<Location> { }

        private State m_State = State.Idle;
        [ShowInInspector]
        public State state
        {
            private set
            {
                m_State = value;
                if(mover != null)
                {
                    switch (m_State)
                    {
                        case State.Idle:
                            mover.direction = Vector3.zero;
                            mover.enabled = false;
                            break;
                        case State.Move:
                            mover.direction = transform.forward;
                            mover.enabled = true;
                            break;
                        case State.TakeOff:
                            mover.direction = transform.up;
                            mover.enabled = true;
                            break;
                        default:
                            break;
                    }
                }
            }
            get => m_State;
        }

        #region [ Component ]

        [BoxGroup("Component")]
        private Looker looker;

        [BoxGroup("Component")]
        private Mover mover;

        #endregion


        [SerializeField]
        private TextMeshProUGUI m_NameTextMesh;

        [SerializeField]
        private TextMeshProUGUI m_SpeedTextMesh;

        private LocationEvent locationArrived = new LocationEvent();
        public LocationEvent LocationArrived => locationArrived;

        [ReadOnly, ShowInInspector]
        private TaskControl taskControl;
        public TaskControl TaskControl => taskControl;

        [ReadOnly, ShowInInspector]
        private Location curLocation = null;
        public Location CurLocation
        {
            set
            {
                curLocation = value;
                if(curLocation != null)
                {
                    preLocation = curLocation;
                }
            }
            get => curLocation;
        }

        [ReadOnly, ShowInInspector]
        private Location preLocation = null;
        public Location PreLocation
        {
            set
            {
                preLocation = value;
            }
            get => preLocation;
        }

        [SerializeField]
        private Location targetLocation;
        public Location TargetLocation
        {
            set
            {
                targetLocation = value;
                if(looker != null)
                {
                    if(targetLocation != null)
                    {
                        looker.target = targetLocation.transform;
                    }
                    else
                    {
                        looker.target = null;
                    }
                }
            }
            get => targetLocation;
        }

        [ReadOnly, ShowInInspector]
        private float distance;

        [ShowInInspector]
        public bool IsTasking
        {
            get
            {
                if (taskControl == null) return false;
                return taskControl.isTasking;
            }
        }

        protected override void OnPreAwake()
        {
            base.OnPreAwake();
            if (mover == null)
            {
                mover = GetComponent<Mover>();
            }
            if(looker == null)
            {
                looker = GetComponent<Looker>();
            }
            if (taskControl == null)
            {
                taskControl = GetComponentInChildren<TaskControl>();
            }
        }

        protected override void Awake()
        {
            base.Awake();
            /*
            var dict = new Dictionary<Task.Type, Func<Task, IEnumerator>>();
            dict[Task.Type.MoveToLocation] = MoveToLocationRoutine;
            m_TaskControl.SetTaskLogic(dict);

            */
            LocationArrived.AddListener((location) =>
            {
                curLocation = location;
            });

            taskControl.onTaskInit.AddListener(OnTaskInit);

            taskControl.onTaskTick.AddListener(OnTaskTick);

            taskControl.onTaskOver.AddListener(OnTaskOver);

            taskControl.StartTasks();

        }

        private void OnTaskInit(Task task)
        {
            switch (task)
            {
                case EVTOL_MoveTask moveTask:
                    this.TargetLocation = moveTask.Way.To;
                    this.state = State.Move;
                    break;
            }

        }

        private void OnTaskTick(Task task)
        {
            switch (task)
            {
                case EVTOL_MoveTask moveTask:       
                    break;
            }
        }

        private void OnTaskOver(Task task)
        {

            switch (task)
            {
                case EVTOL_MoveTask moveTask:
                    this.TargetLocation = null;
                    this.state = State.Idle;
                    break;
            }

        }

        private void Update()
        {

            if(targetLocation != null)
            {
                distance = Vector3.Distance(targetLocation.transform.position, this.transform.position);
                if(distance > 1f)
                {
                    transform.LookAt(targetLocation.transform.position);
                    
                }
            }

            /*
            if(m_NameTextMesh != null)
            {
                m_NameTextMesh.text = this.name;
            }

            if(m_SpeedTextMesh != null)
            {
                m_SpeedTextMesh.text = m_CurSpeed.ToString("00.0 m/s");
            }
            */

        }

        private void OnTriggerEnter(Collider other)
        {
            if(other.tag == "Hit")
            {
                var location = other.GetComponentInParent<WayPoint>();
                if(location != null)
                {
                    curLocation = location;
                }
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if(other.tag == "Hit")
            {
                var location = other.GetComponentInParent<WayPoint>();
                if(location != null && curLocation == location)
                {
                    curLocation = null;
                }
            }
        }


    }


}
