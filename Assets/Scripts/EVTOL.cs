using Alkemic;
using Alkemic.Movement;
using Linefy;
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
    public class EVTOL : LeadComponent
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
                            looker.enabled = false;
                            break;
                        case State.Move:
                            looker.enabled = true;
                            break;
                        case State.TakeOff:
                            looker.enabled = true;
                            break;
                        default:
                            break;
                    }
                }
            }
            get => m_State;
        }

        [BoxGroup("Component")]
        private Looker looker;

        [BoxGroup("Component")]
        private Mover mover;


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
                        looker.Target = targetLocation.transform;
                    }
                    else
                    {
                        looker.Target = null;
                    }
                }
            }
            get => targetLocation;
        }

        [ShowInInspector]
        public bool IsTasking
        {
            get
            {
                if (taskControl == null) return false;
                return taskControl.isTasking;
            }
        }

        public float TargetKnotPHour
        {
            set
            {
                if (mover != null)
                {
                    mover.TargetSpeed = value * UAMStatic.knotPHour2Speed;
                }
            }
            get
            {
                return mover.TargetSpeed * UAMStatic.speed2KnotPHour;
            }
        }

        [ShowInInspector]
        private Lines directionLines = null; 

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

            directionLines = new Lines(1);

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

        private void OnDrawGizmos()
        {

            
        }

        private void Update()
        {

            Vector3 from = transform.position;
            Vector3 to = transform.position + (transform.forward * 500f);
            directionLines[0] = new Line(from, to, Color.red, Color.red, 10f, 5f);
            directionLines.Draw();
            
            if(this.state == State.Move)
            {
                if(mover != null)
                {
                    mover.Direction = transform.forward;
                }
            }

        }


        private void OnTaskInit(Task task)
        {
            switch (task)
            {
                case EVTOL_TakeOffTask takeOffTask:
                    mover.StartMove(transform.up, 40f * UAMStatic.knotPHour2Speed);
                    this.state = State.TakeOff;
                    break;

                case EVTOL_MoveTask moveTask:
                    looker.enabled = true;
                    this.TargetLocation = moveTask.Way.To;
                    mover.StartMove(transform.forward, 80f * UAMStatic.knotPHour2Speed);
                    this.state = State.Move;
                    break;
            }

        }

        private void OnTaskTick(Task task)
        {
            switch (task)
            {
                case EVTOL_MoveTask moveTask:
                    //Debug.Log("UpdateDirection");
                    //this.mover.Direction = transform.forward;
                    break;
            }
        }




        private void OnTaskOver(Task task)
        {

            switch (task)
            {
                case EVTOL_TakeOffTask takeOffTask:
                    this.looker.enabled = false;
                    this.mover.StopMove(useReset: true);
                    this.state = State.Idle;
                    break;
                case EVTOL_MoveTask moveTask:
                    this.looker.enabled = false;
                    this.mover.StopMove(useReset: true);
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
