using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UAM
{
    public class RouteMaker : Behavior
    {

        [SerializeField]
        private List<Way> ways = new List<Way>();
        public List<Way> Ways => ways;

    }


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

        [SerializeField]
        private int m_UAMCount = 50;
        public int UAMCount => m_UAMCount;

        [ReadOnly, ShowInInspector]
        private List<EVTOL> m_EVTOLs = new List<EVTOL>();
        public List<EVTOL> EVTOLs => m_EVTOLs;

        protected override void OnPreAwake()
        {
            base.OnPreAwake();
            if(m_LocationControl == null)
            {
                m_LocationControl = GetComponentInChildren<LocationControl>();
            }
        }

        protected override void Start()
        {
            base.Start();

        }

        [Button]
        public void CreateEVTOL()
        {
            var evtol = Instantiate(EVTOLPrefab, EVTOLParent).GetComponent<EVTOL>();
            this.EVTOLs.Add(evtol);
            evtol.TaskControl.AddTask<EVTOL_MoveTask>((task) =>
            {
                task

            });

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

