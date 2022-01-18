using Alkemic.Scriptables;
using Shapes;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UIElements;

namespace Alkemic.UAM
{
    [ExecuteInEditMode]
    public class VertiPort : Location
    {
        [CacheComponent]
        private Location location;
        public Location Location => location;

        [CacheComponent]
        private RouteControl routeControl;
        public RouteControl RouteControl => routeControl;

        [CacheComponent]
        private TicketControl ticketControl;
        public TicketControl TicketControl => ticketControl;

        [CacheComponent]
        private HangarControl hangar;
        public HangarControl Hangar => hangar;

        [CacheComponent]
        private Line verticalLine;
        public Line VerticalLine => verticalLine;

        public bool IsTakeOff
        {
            get
            {
                return false;
            }
        }

        [HideInInspector]
        public IList<Route> Routes
        {
            get
            {
                if (routeControl == null) return null;
                return routeControl.Routes;
            }
        }

        [HideInInspector]
        public IList<Ticket> Tickets
        {
            get
            {
                if (ticketControl == null) return null;
                return ticketControl.Tickets;
            }
        } 

        public bool IsAble
        {
            get
            {
                return Routes != null && Routes.Count > 0;
            }
        }

        [OptionGroup]
        [SerializeField]
        private float takeOffDelay = 1f;
        public float TakeOffDelay => takeOffDelay;


        protected override void OnPreAwake()
        {
            base.OnPreAwake();

            this.CacheComponentInChildren(ref routeControl);
            if(ticketControl == null)
            {
                ticketControl = GetComponentInChildren<TicketControl>();
            }
            if(hangar == null)
            {
                hangar = GetComponentInChildren<HangarControl>();
            }
            //this.CacheComponentInChildren(ref ticketControl);
            //this.CacheComponentInChildren(ref hangar);

            this.CacheComponent(ref verticalLine);
        }

        protected override void Awake()
        {
            base.Awake();

            Debug.Assert(routeControl != null, $"[{name}:{GetType().Name}] {nameof(routeControl)} is null", gameObject);
            Debug.Assert(ticketControl != null, $"[{name}:{GetType().Name}] {nameof(ticketControl)} is null", gameObject);
            

        }

        protected override void Start()
        {
            base.Start();

            InitWithStaticData();

            StartAutoCoroutine(TakeOffVTOLRoutine());
        }

        private void DrawLine()
        {

        }
        private void InitWithStaticData()
        {
            var simulationEntry = UAMManager.Inst.CurSimulationEntry;

            if(simulationEntry != null)
            {
                this.takeOffDelay = simulationEntry.TakeOffDelay;
            }

            var preset = UAMManager.Inst.CurSimulationEntry?.VertiPortEntries[this.Key];
            if (preset != null)
            {
                if(Hangar != null)
                {
                    int vtolNo = 0;
                    foreach (var vtolTypeKey in preset.VTOLCounts.Keys)
                    {
                        var count = preset.VTOLCounts[vtolTypeKey];
                        for(int i = 0; i < count; i++)
                        {
                            var vtol = Hangar.CreateVTOL(vtolTypeKey);
                            vtol.Key = $"{this.Key}_{vtolNo.ToString("00")}";
                            if (vtol != null)
                            {
                                vtolNo++;
                            }
                        }
                    }
                }
                else
                {
                    Debug.LogError($"[{name}:{GetType().Name}] Hangar is not exist", gameObject);
                }
            }
        }

        protected override void SetupName()
        {
            base.SetupName();

            DataCache.Set(DataCache.Path.NAME, $"VP");
        }

        private IEnumerator TakeOffVTOLRoutine()
        {
            WaitForSeconds delay = new WaitForSeconds(0.1f);
            while (true)
            {
                yield return delay;
                /*
                if (this.IsTakeOff == true)
                {
                    continue;
                }
                */
                if(this.TicketControl.Tickets.Count > 0 && Hangar.VTOLs.Count > 0)
                {
                    var result = CheckTakeOffStatus();
                    if (result == true)
                    {
                        yield return new WaitForSeconds(takeOffDelay);
                    }
                }
            }
        }

        private bool CheckTakeOffStatus()  
        {
            var ticket = this.Tickets.FirstOrDefault();
            if (ticket == null)
            {
                Debug.LogError($"[{name}:{GetType().Name}] Ticket is not exist", gameObject);
                return false;
            }

            var selectVTOL = this.Hangar.VTOLs.FirstOrDefault();
            if(selectVTOL == null)
            {
                Debug.LogError($"[{name}:{GetType().Name}] VTOL is not exist");
                return false;
            }

            var route = this.Routes.Find(x => ticket.Destination == x.Destination);
            if(route == null)
            {
                Debug.LogError($"[{name}:{GetType().Name}] Route is not exist", gameObject);
                Destroy(ticket);
                return false;
            }

            selectVTOL.AssignRoute(route);
            Destroy(ticket);
            return true;
        }

        public void EnterVTOL(VTOL vtol)
        {
            this.Hangar.VTOLs.Add(vtol);
        }

        public void ExitVTOL(VTOL vtol)
        {
            this.Hangar.VTOLs.Remove(vtol);
        }


    }
}
;