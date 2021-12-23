using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

namespace Alkemic.UAM
{

    public class VertiPort : Location
    {

        [CacheComponent]
        private RouteControl routeControl;
        public RouteControl RouteControl => routeControl;

        [CacheComponent]
        private TicketControl ticketControl;
        public TicketControl TicketControl => ticketControl;

        [CacheComponent]
        private HangarControl hangar;
        public HangarControl Hangar => hangar;

        [PresetComponent]
        [SerializeField]
        private Transform landingAnchor;

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


        protected override void OnPreAwake()
        {
            base.OnPreAwake();

            this.CacheComponentInChildren(ref routeControl);
            this.CacheComponentInChildren(ref ticketControl);
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

            StartAutoCoroutine(MakeVTOLRoutine());
        }

        private IEnumerator MakeVTOLRoutine()
        {
            WaitForSeconds delay = new WaitForSeconds(1.0f);
            while (true)
            {
                yield return delay;
                if (IsTakeOff == true)
                {
                    continue;
                }
                if(Tickets.Count <= 0)
                {
                    continue;
                }
                if(Hangar.VTOLs.Count <= 0)
                {
                    continue;
                }
            }
        }

        private bool CheckTicket()  
        {
            var ticket = this.Tickets.FirstOrDefault();
            if (ticket == null)
            {
                Debug.LogError($"[{name}:{GetType().Name}] Ticket is not exist", gameObject);
                return false;
            }

            var selectVTOL = this.hangar.VTOLs.FirstOrDefault();
            if(selectVTOL == null)
            {
                Debug.LogError($"VTOL is not exist");
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
