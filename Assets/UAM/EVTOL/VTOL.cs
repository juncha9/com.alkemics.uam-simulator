using Alkemic.Movement;
using Sirenix.OdinInspector;
using System;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

namespace Alkemic.UAM
{
    [RequireComponent(typeof(Looker))]
    [RequireComponent(typeof(Mover))]
    public class VTOL : LeadComponent
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
                if (mover != null)
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

        [CacheGroup]
        [Debug]
        private TaskControl taskControl;
        public TaskControl TaskControl => taskControl;

        [PropertyGroup]
        [ShowOnly]
        private Location curLocation = null;
        public Location CurLocation
        {
            set
            {
                if (curLocation == value) return;
                preLocation = curLocation;
                curLocation = value;
                if (IsDebug == true) { Debug.Log($"[{DName}] Location changed, [{preLocation?.Key}] to [{curLocation?.Key}]", gameObject); }
            }
            get => curLocation;
        }

        [PropertyGroup]
        [ShowOnly]
        private Location preLocation = null;
        public Location PreLocation
        {
            set
            {
                preLocation = value;
            }
            get => preLocation;
        }

        [PropertyGroup]
        [RuntimeOnly]
        private object target;
        public object Target
        {
            set
            {
                target = value;
                if (looker != null)
                {
                    if (target != null && target is MonoBehaviour)
                    {
                        looker.Target = (target as MonoBehaviour).transform;
                    }
                    else
                    {
                        looker.Target = null;
                    }
                }
            }
            get => target;
        }

        [PropertyGroup]
        [ShowOnly]
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
                    mover.SpeedFactor = value * UAMStatic.knotPHour2Speed;
                }
            }
            get
            {
                return mover.SpeedFactor * UAMStatic.speed2KnotPHour;
            }
        }

        [Range(UAMStatic.MinVTOLSpeed, UAMStatic.MaxVTOLSpeed)]
        [OptionGroup]
        [SerializeField]
        private float maxSpeed = UAMStatic.speed2KnotPHour;
        public float MaxSpeed => maxSpeed;

        protected override void OnPreAwake()
        {
            base.OnPreAwake();
            this.CacheComponent(ref mover);
            this.CacheComponent(ref looker);
            this.CacheComponentInChildren(ref taskControl);
        }

        protected override void Awake()
        {
            base.Awake();
            Debug.Assert(mover != null, $"[{name}] {nameof(mover)} is null", gameObject);
            Debug.Assert(looker != null, $"[{name}] {nameof(looker)} is null", gameObject);
            Debug.Assert(taskControl != null, $"[{name}] {nameof(taskControl)} is null", gameObject);


            /*
            var dict = new Dictionary<Task.Type, Func<Task, IEnumerator>>();
            dict[Task.Type.MoveToLocation] = MoveToLocationRoutine;
            m_TaskControl.SetTaskLogic(dict);

            */

            LocationArrived.AddListener((location) =>
            {
                curLocation = location;
            });

            taskControl.OnTaskInited.AddListener(OnTaskInit);
            taskControl.OnTaskTicked.AddListener(OnTaskTick);
            taskControl.OnTaskOvered.AddListener(OnTaskOver);
            taskControl.OnTaskUpdate.AddListener(OnTaskUpdate);
            taskControl.OnTaskFixedUpdate.AddListener(OnTaskFixedUpdate);
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



        }


        private void OnTaskInit(Task task)
        {
            switch (task)
            {
                case VerticalMoveTask takeOffTask:
                    this.state = State.TakeOff;
                    mover.StartMove();
                    break;

                case MoveTask moveTask:
                    looker.enabled = true;
                    this.Target = moveTask.Way.To;
                    mover.StartMove();
                    this.state = State.Move;
                    break;
            }

        }

        private void OnTaskUpdate(Task task)
        {

        }

        private void OnTaskFixedUpdate(Task task)
        {
            switch (task)
            {
                case VerticalMoveTask verticalTask:
                    if (verticalTask.MoveType == VerticalMove.TakeOff)
                    {
                        mover.Direction = Vector3.up;
                        mover.SpeedFactor = 40f * UAMStatic.knotPHour2Speed;
                    }
                    else if (verticalTask.MoveType == VerticalMove.Land)
                    {
                        mover.Direction = Vector3.down;
                        mover.SpeedFactor = 40f * UAMStatic.knotPHour2Speed;
                    }
                    break;

                case MoveTask moveTask:
                    {
                        float speed = this.MaxSpeed;
                        var way = moveTask.Way;
                        if (way != null)
                        {
                            int _index = way.MovingVTOLs.IndexOf(this);
                            if (_index >= 0)
                            {
                                var forwardVTOLs = way.MovingVTOLs.Take(_index).ToList();
                                if(forwardVTOLs.Count > 0)
                                {
                                    float forwardSpeed = forwardVTOLs.Min(x => x.mover.CurSpeedFactor) * UAMStatic.speed2KnotPHour;
                                    if (speed < forwardSpeed)
                                    {
                                        Debug.Log($"MinSpeed is {forwardSpeed}");
                                        speed = forwardSpeed;
                                    }
                                }
                            }
                        }

                        mover.Direction = transform.forward;
                        mover.SpeedFactor = speed * UAMStatic.knotPHour2Speed;
                    }

                    break;
            }
        }

        private void OnTaskTick(Task task)
        {

        }

        private void OnTaskOver(Task task)
        {

            switch (task)
            {
                case VerticalMoveTask takeOffTask:
                    this.looker.enabled = false;
                    this.mover.StopMove();
                    this.mover.ResetMove();
                    this.state = State.Idle;
                    break;
                case MoveTask moveTask:
                    this.looker.enabled = false;
                    this.mover.StopMove();
                    this.mover.ResetMove();
                    this.Target = null;
                    this.state = State.Idle;
                    break;
            }

        }



        private void OnTriggerEnter(Collider other)
        {
            if (other.tag == "Hit")
            {
                var location = other.GetComponentInParent<Location>();
                if (location != null)
                {
                    curLocation = location;
                }

                if (IsDebug == true)
                {
                    var pos = (transform.position + other.transform.position) / 2f;
                    DebugEx.DrawBounds(new Bounds(pos, new Vector3(50f, 50f, 50f)), ColorDefine.GREEN_APPLE, 2f);
                }

            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.tag == "Hit")
            {
                var location = other.GetComponentInParent<Location>();
                if (location != null && curLocation == location)
                {
                    curLocation = null;
                }

                if (IsDebug == true)
                {
                    var pos = (transform.position + other.transform.position) / 2f;
                    DebugEx.DrawBounds(new Bounds(pos, new Vector3(50f, 50f, 50f)), ColorDefine.RED_ORANGE, 2f);
                }
            }
        }


    }


}
