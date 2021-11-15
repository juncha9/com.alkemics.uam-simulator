using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UAM
{
    public class UAMSimulator : Behavior
    {
        [SerializeField]
        private bool m_IsMain = false;
        public bool isMain => m_IsMain;

        [SerializeField]
        private GameObject _EVTOLPrefab;
        public GameObject EVTOLPrefab => _EVTOLPrefab;

        [SerializeField]
        private Transform _EVTOLParent;
        public Transform EVTOLParent => _EVTOLParent;

        [ReadOnly, ShowInInspector]
        private LocationControl m_LocationControl;
        public LocationControl locationControl => m_LocationControl;

        [ReadOnly, ShowInInspector]
        private RouteMaker routeMaker;
        public RouteMaker RouteMaker => routeMaker;


        [SerializeField]
        private int m_UAMCount = 50;
        public int UAMCount => m_UAMCount;

        [ReadOnly, ShowInInspector]
        private List<EVTOL> m_EVTOLs = new List<EVTOL>();
        public List<EVTOL> EVTOLs => m_EVTOLs;

        [SerializeField]
        Station station;

        protected override void OnPreAwake()
        {
            base.OnPreAwake();
            if(m_LocationControl == null)
            {
                m_LocationControl = GetComponentInChildren<LocationControl>();
            }
            if(routeMaker == null)
            {
                routeMaker = GetComponent<RouteMaker>();
            }

        }

        protected override void Awake()
        {
            base.Awake();
            Debug.Assert(station != null, $"[{name}] {nameof(station)} is null", gameObject);

        }

        protected override void Start()
        {
            base.Start();

        }

        [Button]
        public void CreateEVTOL()
        {
            var evtol = Instantiate(EVTOLPrefab, EVTOLParent).GetComponent<EVTOL>();
            evtol.transform.position = station.transform.position;
            this.EVTOLs.Add(evtol);
            evtol.TaskControl.AddTask<EVTOL_TakeOffTask>((task) =>
            {
                task.Height = 1000f;
            });

            foreach(var way in RouteMaker.Ways)
            {
                evtol.TaskControl.AddTask<EVTOL_MoveTask>((task) =>
                {
                    task.Way = way;
                });
            }


        }

        /*
        [Button]
        private void StartSim()
        {

            for (int i =0; i< UAMCount; i++)
            {
                var evtol = Instantiate(m_EVTOLPrefab, m_EVTOLParent)?.GetComponent<EVTOL>();
                
                

                this.EVTOLs.Add(evtol);

                var locations = locationControl.locations;
                WayPoint randomLocation = locations[Random.Range(0, locations.Count)];
                if(randomLocation != null)
                {
                    evtol.transform.position = randomLocation.transform.position;
                }
                else
                {
                    Debug.LogError($"[{name}] Error on create evtol", gameObject);
                }
                
            }
        }


        private IEnumerator CheckEVTOLRoutine()
        {
            WaitForSeconds delay = new WaitForSeconds(0.1f);
            while(true)
            {
                foreach(var evtol in EVTOLs)
                {
                    if (evtol.isTasking == true) continue;
                    if (evtol.curLocation == null) continue;
                    var locations = evtol.curLocation.ableWays;
                    var targetLoc = locations[Random.Range(0, locations.Count)];
                    if(targetLoc != null)
                    {
                        evtol.taskControl.AddTask<EVTOL_MoveTask>(
                            (task) =>
                            {
                                task.destLocation = targetLoc;
                            });
                    }
                }
                yield return delay;
            }
        }
        */



    }


}

