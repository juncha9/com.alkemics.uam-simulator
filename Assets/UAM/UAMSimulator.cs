using Sirenix.OdinInspector;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Alkemic.UAM
{


    public class UAMSimulator : LeadComponent
    {



        [SerializeField]
        private bool m_IsMain = false;
        public bool isMain => m_IsMain;

        [PresetComponent]
        [SerializeField]
        private GameObject _LT_VTOLPrefab;
        public GameObject LT_VTOLPrefab => _LT_VTOLPrefab;

        [PresetComponent]
        [SerializeField]
        private GameObject _ST_VTOLPrefab;
        public GameObject ST_VTOLPrefab => _ST_VTOLPrefab;

        [SerializeField]
        private Transform _VTOLParent;
        public Transform VTOLParent => _VTOLParent;

        [ShowOnly]
        private LocationControl m_LocationControl;
        public LocationControl locationControl => m_LocationControl;

        [ShowOnly]
        private RouteControl routeMaker;
        public RouteControl RouteMaker => routeMaker;


        [SerializeField]
        private int m_UAMCount = 50;
        public int UAMCount => m_UAMCount;

        [ShowOnly]
        private List<VTOL> m_EVTOLs = new List<VTOL>();
        public List<VTOL> EVTOLs => m_EVTOLs;

        [SerializeField]
        VertiPort station;

        protected override void OnPreAwake()
        {
            base.OnPreAwake();
            if (m_LocationControl == null)
            {
                m_LocationControl = GetComponentInChildren<LocationControl>();
            }
            if (routeMaker == null)
            {
                routeMaker = GetComponent<RouteControl>();
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

