using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

namespace UAM
{
    

    public class EVTOL : Behavior
    {
        [Serializable]
        public class LocationArriveEvent : UnityEvent<Location> { }

        private LocationArriveEvent m_OnLocationArrived = new LocationArriveEvent();
        public LocationArriveEvent onLocationArrived => m_OnLocationArrived;

        private TaskControl m_TaskControl;

        [SerializeField]
        private float m_Speed = 10f;
        
        [ReadOnly, ShowInInspector]
        private Location m_CurLocation = null;
        public Location curLocation => m_CurLocation;


        [ReadOnly, ShowInInspector]
        private Location m_PreLocation = null;
        public Location preLocation => m_PreLocation;


        [SerializeField]
        private Location m_TargetLocation;
        public Location targetLocation => m_TargetLocation;

        [ReadOnly, ShowInInspector]
        private float m_Distance;

        protected override void OnPreAwake()
        {
            base.OnPreAwake();
            if(m_TaskControl == null)
            {
                m_TaskControl = GetComponent<TaskControl>();
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
        /*
        private IEnumerator MoveToLocationRoutine(Task task)
        {
            


        }
        */

        protected override void Start()
        {
            base.Start();
        }

        private void Update()
        {
            if(m_TargetLocation != null)
            {
                m_Distance = Vector3.Distance(m_TargetLocation.transform.position, this.transform.position);
                if(m_Distance > 1f)
                {
                    transform.LookAt(m_TargetLocation.transform.position);
                    transform.Translate(Vector3.forward * Time.deltaTime * m_Speed);
                }
           
            }
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
