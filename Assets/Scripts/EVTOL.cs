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
                            mover.direction = null;
                            looker.enabled = false;
                            mover.enabled = false;
                            break;
                        case State.Move:
                            mover.direction = null;
                            looker.enabled = true;
                            mover.enabled = true;
                            break;
                        case State.TakeOff:
                            mover.direction = Vector3.up;
                            looker.enabled = true;
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
        private EVTOL_Mover mover;

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
                mover = GetComponent<EVTOL_Mover>();
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
        }

        protected override void Start()
        {
            base.Start();

            taskControl.StartTasks();

        }

        private void OnTaskInit(Task task)
        {
            switch (task)
            {
                case EVTOL_TakeOffTask takeOffTask:
                    this.mover.targetKnotPHour = 10f;
                    this.state = State.TakeOff;
                    break;

                case EVTOL_MoveTask moveTask:
                    this.TargetLocation = moveTask.Way.To;
                    this.mover.targetKnotPHour = 10f;
                    this.state = State.Move;
                    break;
            }

        }

        private void OnTaskTick(Task task)
        {
            switch (task)
            {
                case EVTOL_MoveTask moveTask:
                    this.mover.direction = transform.forward;
                    break;
            }
        }

        private void OnTaskOver(Task task)
        {

            switch (task)
            {
                case EVTOL_TakeOffTask takeOffTask:
                    this.looker.enabled = false;
                    this.mover.enabled = false;
                    this.mover.targetKnotPHour = 0f;
                    this.state = State.Idle;
                    break;
                case EVTOL_MoveTask moveTask:
                    this.looker.enabled = false;
                    this.mover.enabled = false;
                    this.mover.targetKnotPHour = 0f;
                    this.TargetLocation = null;
                    this.state = State.Idle;
                    break;
            }

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
