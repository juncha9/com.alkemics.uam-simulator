using Alkemic.Movement;
using Shapes;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Rendering.Universal;
using Alkemic.Doings;

namespace Alkemic.UAM
{
    [RequireComponent(typeof(DataCache))]
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


        [OptionGroup]
        [SerializeField]
        private string key;
        public string Key
        {
            set
            {
                key = value;
                DataCache?.Set(DataCache.Path.KEY, key);
            }
            get => key;
        }


        [ReadOnly]
        private string _VTOLTypeKey;
        public string VTOLTypeKey
        {
            set => _VTOLTypeKey = value;
            get => _VTOLTypeKey;
        }

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
                            mover.enabled = false;
                            looker.enabled = false;
                            SetActiveShape(false);
                            break;
                        case States.Move:
                            looker.enabled = true;
                            mover.enabled = true;
                            SetActiveShape(true);
                            break;
                        case States.TakeOff:
                            looker.enabled = true;
                            mover.enabled = true;
                            SetActiveShape(true);
                            break;
                        case States.Land:
                            looker.enabled = true;
                            SetActiveShape(true);
                            break;
                        default:
                            break;
                    }
                }
            }
            get => state;
        }

        [PresetComponent]
        [SerializeField]
        private Line leftLine;
        [PresetComponent]
        [SerializeField]
        private Line rightLine;


        [CacheComponent]
        private Looker looker;

        [CacheComponent]
        private Mover mover;

        private LocationEvent onLocationArrived = new LocationEvent();
        public LocationEvent OnLocationArrived => onLocationArrived;

        [CacheComponent]
        private DoingHandler task;
        public DoingHandler Task => task;

        [SerializeField]
        private List<GameObject> shapeObjects = new List<GameObject>();
        public List<GameObject> ShapeObjects => shapeObjects;

        [PropertyGroup]
        [RuntimeOnly]
        private Location curLocation = null;
        public Location CurLocation
        {
            set
            {
                if (curLocation == value) return;
                curLocation = value;
                if(curLocation != null)
                {
                    PreLocation = curLocation;
                }
                if (IsDebug == true) { Debug.Log($"[{name}:{GetType().Name}] Location changed, [{preLocation?.Key}] to [{curLocation?.Key}]", gameObject); }
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
                this.vertiPort = preLocation as VertiPort;
            }
            get => preLocation;
        }

        [PropertyGroup]
        [ShowOnly]
        private VertiPort vertiPort;
        public VertiPort VertiPort
        {
            get => vertiPort;
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
                    if (target != null)
                    {
                        switch (target)
                        {
                            case Location location:
                                looker.Target = location.AirAnchor.transform;
                                break;
                            case MonoBehaviour mono:
                                looker.Target = mono.transform;
                                break;
                            default:
                                looker.Target = null;
                                break;
                        }
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

        public float CurKPH
        {
            get
            {
                return mover.CurSpeedFactor * UAMDefine.speed2KnotPHour;
            }
        }

        public float TargetKPH
        {
            set
            {
                if (mover != null)
                {
                    mover.SpeedFactor = value * UAMDefine.knotPHour2Speed;
                }
            }
            get
            {
                return mover.SpeedFactor * UAMDefine.speed2KnotPHour;
            }
        }

        [Range(UAMDefine.MinVTOLSpeed, UAMDefine.MaxVTOLSpeed)]
        [OptionGroup]
        [SerializeField]
        private float maxKPH = UAMDefine.speed2KnotPHour;
        public float MaxKPH => maxKPH;

        private Line line;


        protected override void OnValidate()
        {
            base.OnValidate();

            
        }

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

            this.State = state;
        }

        protected override void Start()
        {
            base.Start();

            this.name = $"{GetType().Name} [{Key}]";

            InitWithStaticData();

            task.StartTasks();

            StartAutoCoroutine(IndicateRoutine());
        }

        private void Update()
        {
            if(target != null)
            {
                switch (target)
                {
                    case Location location:
                        if(location.AirAnchor == null)
                        {
                            break;
                        }
                        leftLine.Start = transform.right * -300f;
                        leftLine.End = transform.InverseTransformPoint(location.AirAnchor.position);
                        rightLine.Start = transform.right * 300f;
                        rightLine.End = transform.InverseTransformPoint(location.AirAnchor.position);
                        break;

                    case MonoBehaviour mono:
                        leftLine.Start = transform.right * -300f;
                        leftLine.End = transform.InverseTransformPoint(mono.transform.position);
                        rightLine.Start = transform.right * 300f;
                        rightLine.End = transform.InverseTransformPoint(mono.transform.position);
                        break;

                    default:
                        leftLine.Start = Vector3.zero;
                        leftLine.End = Vector3.zero;
                        rightLine.Start = Vector3.zero;
                        rightLine.End = Vector3.zero;
                        break;
                }

            }
        }

        private IEnumerator IndicateRoutine()
        {
            WaitForSeconds delay = new WaitForSeconds(AppDefine.DEFAULT_DELAY_TIME);

            while(true)
            {
                yield return delay;
                this.DataCache.Set(DataCache.Path.KEY, Key);
                this.DataCache.Set("state", this.State.ToString());
                this.DataCache.Set("kph", $"{CurKPH.ToString("0")}/{MaxKPH.ToString("0")}");
               
            }


        
        }


        private void InitWithStaticData()
        {
            var preset = UAMManager.Inst.CurSimulationEntry?.EVTOLEntries[this.VTOLTypeKey];
            if (preset != null)
            {
                this.maxKPH = preset.MaxSpeed;
            }
        }

        private void SetActiveShape(bool value)
        {
            foreach(var shape in ShapeObjects)
            {
                shape.SetActive(value);
            }
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

        private void OnTaskInit(Doing task)
        {
            switch (task)
            {
                case TakeOffTask takeOffTask:
                    this.State = States.TakeOff;
                    mover.StartMove();
                    vertiPort.ExitVTOL(this);
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

        private void OnTaskUpdate(Doing task)
        {
            
        }

        private void OnTaskFixedUpdate(Doing task)
        {
            switch (task)
            {
                case TakeOffTask verticalTask:
                    mover.Direction = Vector3.up;
                    mover.SpeedFactor = 40f * UAMDefine.knotPHour2Speed;
                    break;
                case LandTask landTask:
                    mover.Direction = Vector3.down;
                    mover.SpeedFactor = 40f * UAMDefine.knotPHour2Speed;
                    break;

                case MoveTask moveTask:
                    {
                        float speed = this.MaxKPH;
                        var way = moveTask.Way;
                        if (way != null)
                        {
                            int _index = way.MovingVTOLs.IndexOf(this);
                            if (_index >= 0)
                            {
                                var forwardVTOLs = way.MovingVTOLs.Take(_index).ToList();
                                if (forwardVTOLs.Count > 0)
                                {
                                    float forwardSpeed = forwardVTOLs.Min(x => x.mover.CurSpeedFactor) * UAMDefine.speed2KnotPHour;
                                    if (speed < forwardSpeed)
                                    {
                                        Debug.Log($"MinSpeed is {forwardSpeed}");
                                        speed = forwardSpeed;
                                    }
                                }
                            }
                        }

                        mover.Direction = transform.forward;
                        mover.SpeedFactor = speed * UAMDefine.knotPHour2Speed;
                    }

                    break;
            }
        }

        private void OnTaskTick(Doing task)
        {

        }

        private void OnTaskOver(Doing task)
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
                    vertiPort.EnterVTOL(this);
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
