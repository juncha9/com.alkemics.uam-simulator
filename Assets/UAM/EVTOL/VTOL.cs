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
        public enum States
        {
            Stop,
            Move,
            TakeOff,
            Land,
        }

        [Serializable]
        public class LocationEvent : UnityEvent<Location> { }

        [PropertyGroup]
        [RuntimeOnly]
        private States state = States.Stop;
        public States State
        {
            private set
            {
                state = value;
                if (mover != null)
                {
                    switch (state)
                    {
                        case States.Stop:
                            looker.enabled = false;
                            shapeGameObject.SetActive(false);
                            break;
                        case States.Move:
                            looker.enabled = true;
                            shapeGameObject.SetActive(true);
                            break;
                        case States.TakeOff:
                        case States.Land:
                            looker.enabled = true;
                            shapeGameObject.SetActive(true);
                            break;
                        default:
                            break;
                    }
                }
            }
            get => state;
        }

        [CacheComponent]
        private Looker looker;

        [CacheComponent]
        private Mover mover;

        [PresetComponent]
        private GameObject shapeGameObject;

        private LocationEvent onLocationArrived = new LocationEvent();
        public LocationEvent OnLocationArrived => onLocationArrived;

        [CacheComponent]
        private TaskControl task;
        public TaskControl Task => task;

        [PropertyGroup]
        [RuntimeOnly]
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
        [RuntimeOnly]
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
        private 

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
                if (task == null) return false;
                return task.isTasking;
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
            this.CacheComponentInChildren(ref task);
        }

        protected override void Awake()
        {
            base.Awake();
            Debug.Assert(mover != null, $"[{name}:{GetType().Name}] {nameof(mover)} is null", gameObject);
            Debug.Assert(looker != null, $"[{name}:{GetType().Name}] {nameof(looker)} is null", gameObject);
            Debug.Assert(task != null, $"[{name}:{GetType().Name}] {nameof(task)} is null", gameObject);


            /*
            var dict = new Dictionary<Task.Type, Func<Task, IEnumerator>>();
            dict[Task.Type.MoveToLocation] = MoveToLocationRoutine;
            m_TaskControl.SetTaskLogic(dict);

            */

            OnLocationArrived.AddListener((location) =>
            {
                curLocation = location;
            });

            task.OnTaskInited.AddListener(OnTaskInit);
            task.OnTaskTicked.AddListener(OnTaskTick);
            task.OnTaskOvered.AddListener(OnTaskOver);
            task.OnTaskUpdate.AddListener(OnTaskUpdate);
            task.OnTaskFixedUpdate.AddListener(OnTaskFixedUpdate);
        }

        protected override void Start()
        {
            base.Start();

            task.StartTasks();

        }

        private void Update()
        {



        }

        public void AssignRoute(Route route)
        {
            if (route == null)
            {
                Debug.LogError($"[{name}:{GetType().Name}] Route is null", gameObject);
                return;
            }

            this.Task.AssignTask<TakeOffTask>();

            var items = route.ToMoveTaskItems();
            if(items == null)
            {
                Debug.LogError($"[{name}:{GetType().Name}] Failed to convert Route info to TaskItems", gameObject);
            }
            foreach(var item in items)
            {
                this.Task.AssignTask<MoveTask>((task) =>
                {
                    task.Way = item.way;
                    task.TargetLocation = item.targetLocation;
                });
            }

            this.Task.AssignTask<LandTask>();
        }

        private void OnTaskInit(Task task)
        {
            switch (task)
            {
                case TakeOffTask takeOffTask:
                    this.State = States.TakeOff;
                    mover.StartMove();
                    break;
                case LandTask landTask:
                    this.State = States.Land;
                    mover.StartMove();
                    break;
                case MoveTask moveTask:
                    looker.enabled = true;
                    this.Target = moveTask.TargetLocation;
                    mover.StartMove();
                    this.State = States.Move;
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
                case TakeOffTask verticalTask:
                    mover.Direction = Vector3.up;
                    mover.SpeedFactor = 20f * UAMStatic.knotPHour2Speed;
                    break;
                case LandTask landTask:
                    mover.Direction = Vector3.down;
                    mover.SpeedFactor = 20f * UAMStatic.knotPHour2Speed;
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
                                if (forwardVTOLs.Count > 0)
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
                case TakeOffTask takeOffTask:
                    this.looker.enabled = false;
                    this.mover.StopMove();
                    this.mover.ResetMove();
                    this.State = States.Stop;
                    break;
                case LandTask landTask:
                    this.looker.enabled = false;
                    this.mover.StopMove();
                    this.mover.ResetMove();
                    this.State = States.Stop;
                    break;
                case MoveTask moveTask:
                    this.looker.enabled = false;
                    this.mover.ResetMove();
                    this.Target = null;
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
                    CurLocation = location;
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
                if (location != null && CurLocation == location)
                {
                    CurLocation = null;
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
