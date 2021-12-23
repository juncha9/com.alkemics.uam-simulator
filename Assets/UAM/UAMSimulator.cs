using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.UI;

namespace Alkemic.UAM
{

    [RequireComponent(typeof(DataCache))]
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

        [InstanceGroup]
        [ShowOnly]
        private List<VTOL> _VTOLs = new List<VTOL>();
        public List<VTOL> VTOLs => _VTOLs;

        [OptionGroup]
        [SerializeField]
        private int maxTicketCount;
        
        

        protected override void OnValidate()
        {
            base.OnValidate();
            if (string.IsNullOrWhiteSpace(key) == true)
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

            Debug.Assert(locationControl != null, $"[{name}:{GetType().Name}] {nameof(locationControl)} is null", gameObject);
        }

        protected override void Start()
        {
            base.Start();

            StartAutoCoroutine(RandomTicketRoutine());
        }

        [Button]
        public void GenerateKey()
        {
            var guid = Guid.NewGuid();
            this.key = guid.ToString();
        }

        private IEnumerator RandomTicketRoutine()
        {
            WaitForSeconds delay = new WaitForSeconds(1f);
            var entryVertiPorts = this.locationControl.vertiPorts
                .Where(x => x.IsAble == true && x.TicketControl.Tickets.Count < 5)
                .ToList();

            while (true)
            {
                yield return delay;

                int vpNo = UnityEngine.Random.Range(0, entryVertiPorts.Count);
                var selectVP = entryVertiPorts[vpNo];

                int routeNo = UnityEngine.Random.Range(0, selectVP.Routes.Count);
                var selectRoute = selectVP.Routes[routeNo];
                var source = selectRoute.Source as VertiPort;
                var dest = selectRoute.Destination as VertiPort;

                if(source == null || dest == null)
                {
                    continue;
                }

                selectVP.TicketControl.OpenTicket(source, dest);
            }
        }

        /*
        [Button]1
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
                    Debug.LogError($"[{name}:{GetType().Name}] Error on create evtol", gameObject);
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

