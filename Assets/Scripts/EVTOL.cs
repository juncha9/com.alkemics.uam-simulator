using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace UAM
{
    

    public class EVTOL : Behavior
    {
        [Serializable]
        public class LocationArriveEvent : UnityEvent<Location> { }

        [SerializeField]
        private TextMeshProUGUI m_NameTextMesh;

        [SerializeField]
        private TextMeshProUGUI m_SpeedTextMesh;

        private LocationArriveEvent m_OnLocationArrived = new LocationArriveEvent();
        public LocationArriveEvent onLocationArrived => m_OnLocationArrived;

        [ReadOnly, ShowInInspector]
        private TaskControl m_TaskControl;
        public TaskControl taskControl => m_TaskControl;

        [SerializeField]
        private float m_Speed = 1000f;
        public float speed
        {
            set => m_Speed = value;
            get => m_Speed;
        }

        [ReadOnly, ShowInInspector]
        private float m_TargetSpeed;

        [ReadOnly, ShowInInspector]
        private float m_CurSpeed;

        [ReadOnly, ShowInInspector]
        private Location m_CurLocation = null;
        public Location curLocation => m_CurLocation;

        [ReadOnly, ShowInInspector]
        private Location m_PreLocation = null;
        public Location preLocation => m_PreLocation;

        [SerializeField]
        private Location m_TargetLocation;
        public Location targetLocation
        {
            private set => m_TargetLocation = value;
            get => m_TargetLocation;
        }

        [ReadOnly, ShowInInspector]
        private float m_Distance;

        [ReadOnly, ShowInInspector]
        private bool m_IsTasking = false;
        public bool isTasking
        {
            private set => m_IsTasking = value;
            get => m_IsTasking;
        }

        [ReadOnly, ShowInInspector]
        private bool m_IsInterrupt = false;
        public bool isInterrupt
        {
            private set => m_IsInterrupt = value;
            get => m_IsInterrupt;
        }

        protected override void OnPreAwake()
        {
            base.OnPreAwake();
            if(m_TaskControl == null)
            {
                m_TaskControl = GetComponentInChildren<TaskControl>();
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
            onLocationArrived.AddListener((location) =>
            {
                m_CurLocation = location;
            });

            m_TaskControl.StartTasks();

        }

        public IEnumerator MoveToLocationRoutine(Location location)
        {
            WaitForSeconds delay = new WaitForSeconds(0.1f);
            
            targetLocation = location;
            isTasking = true;
            while (true)
            {
                if (isInterrupt == true)
                {
                    isInterrupt = false;
                    targetLocation = null;
                    isTasking = false;
                    yield break;
                }

                if (Equals(curLocation, location) == true)
                {
                    targetLocation = null;
                    isTasking = false;
                    yield break;
                }

                yield return delay;
            }
        }

        protected override void Start()
        {
            base.Start();

            m_TargetSpeed = m_Speed;
        }

        private void Update()
        {
            m_CurSpeed = Mathf.Lerp(m_CurSpeed, m_TargetSpeed, Time.deltaTime * 10f);

            if(m_TargetLocation != null)
            {
                m_Distance = Vector3.Distance(m_TargetLocation.transform.position, this.transform.position);
                if(m_Distance > 1f)
                {
                    transform.LookAt(m_TargetLocation.transform.position);
                    transform.Translate(Vector3.forward * Time.deltaTime * m_CurSpeed);
                }
            }

            if(m_NameTextMesh != null)
            {
                m_NameTextMesh.text = this.name;
            }

            if(m_SpeedTextMesh != null)
            {
                m_SpeedTextMesh.text = m_CurSpeed.ToString("00.0 m/s");
            }


        }

        public void SetInterrupt()
        {
            if (isTasking == false) return;
            m_IsInterrupt = true;
        }

        private void OnTriggerEnter(Collider other)
        {
            if(other.tag == "Hit")
            {
                var location = other.GetComponentInParent<Location>();
                if(location != null)
                {
                    m_CurLocation = location;
                }
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if(other.tag == "Hit")
            {
                var location = other.GetComponentInParent<Location>();
                if(location != null && m_CurLocation == location)
                {
                    m_CurLocation = null;
                }
            }
        }


    }


}
