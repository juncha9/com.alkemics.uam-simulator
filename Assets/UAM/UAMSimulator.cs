using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Alkemic.UAM
{


    public class UAMSimulator : LeadComponent
    {
        public static IList<UAMSimulator> GetSimulators()
        {
            var list = GameObject.FindObjectsOfType<UAMSimulator>().ToList();
            list.Insert(0, null);
            return list;
        }

        [PropertyGroup]
        [SerializeField]
        private string key;
        public string Key => key;

        [PresetComponent]
        [SerializeField]
        private Transform _VTOLParent;
        public Transform VTOLParent => _VTOLParent;

        [ShowOnly]
        private LocationControl locationControl;
        public LocationControl LocationControl => locationControl;


        [SerializeField]
        private int _UAMCount = 50;
        public int UAMCount => _UAMCount;

        [ShowOnly]
        private List<VTOL> m_EVTOLs = new List<VTOL>();
        public List<VTOL> EVTOLs => m_EVTOLs;

        protected override void OnValidate()
        {
            base.OnValidate();
            if(string.IsNullOrWhiteSpace(key) == true)
            {
                GenerateKey();
            }

        }


        protected override void OnPreAwake()
        {
            base.OnPreAwake();
            this.CacheComponentInChildren(ref locationControl);

        }

        protected override void Awake()
        {
            base.Awake();

            Debug.Assert(locationControl != null, $"[{name}] {nameof(locationControl)} is null", gameObject);
        }


        [Button]
        public void GenerateKey()
        {
            var guid = Guid.NewGuid();
            this.key = guid.ToString();
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

