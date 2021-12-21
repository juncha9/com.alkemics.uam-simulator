using Alkemic.Collections;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

namespace Alkemic.UAM
{

    public class HangarControl : BaseComponent
    {

        [CacheComponent]
        private VertiPort vertiPort;

        [OptionGroup]
        [SerializeField]
        private int startVTOL_LTCount = 0;

        [OptionGroup]
        [SerializeField]
        private int startVTOL_STCount = 0;

        [InstanceGroup]
        [ShowOnly]
        private DestroyableList<VTOL> _VTOLs = new DestroyableList<VTOL>();
        public DestroyableList<VTOL> VTOLs => _VTOLs;

        protected override void OnPreAwake()
        {
            base.OnPreAwake();

            this.CacheComponentInParent(ref vertiPort);
        }

        protected override void Awake()
        {
            base.Awake();

            Debug.Assert(vertiPort != null, $"[{name}:{GetType().Name}] {nameof(vertiPort)} is null", gameObject);
        }

        protected override void Start()
        {
            base.Start();

            InitVTOL();
        }

        private void InitVTOL()
        {
            for(int i =0; i< startVTOL_LTCount; i++)
            {
                MakeVTOL(VTOLType.LongTerm);
            }

            for(int i =0; i < startVTOL_STCount; i++)
            {
                MakeVTOL(VTOLType.ShortTerm);
            }
        }


        public VTOL MakeVTOL(VTOLType type)
        {
            Transform parent = vertiPort.ParentSimulator.VTOLParent;
            VTOL vtol = null;
            switch (type)
            {
                case VTOLType.LongTerm:
                    vtol = Instantiate(UAMManager.Inst.LT_VTOLPrefab, parent).GetComponent<VTOL>();
                    break;
                case VTOLType.ShortTerm:
                    vtol = Instantiate(UAMManager.Inst.ST_VTOLPrefab, parent).GetComponent<VTOL>();
                    break;
            }
            vtol.transform.position = transform.position;
            vtol.CurLocation = vertiPort;
            this.VTOLs.Add(vtol);
            return vtol;
        }


    }

    public class VertiPort : Location
    {

        [CacheComponent]
        private RouteControl route;
        public RouteControl Route => route;

        [CacheComponent]
        private TicketControl ticket;
        public TicketControl Ticket => ticket;

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
                if (route == null) return null;
                return route.Routes;
            }
        }

        public IList<Ticket> Tickets
        {
            get
            {
                if (ticket == null) return null;
                return ticket.Tickets;
            }
        } 

        protected override void OnPreAwake()
        {
            base.OnPreAwake();

            this.CacheComponentInChildren(ref route);
            this.CacheComponentInChildren(ref ticket);
        }

        protected override void Awake()
        {
            base.Awake();

            Debug.Assert(route != null, $"[{name}:{GetType().Name}] {nameof(route)} is null", gameObject);
            Debug.Assert(ticket != null, $"[{name}:{GetType().Name}] {nameof(ticket)} is null", gameObject);
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

        private void CheckTicket()  
        {
            var ticket = this.Tickets.FirstOrDefault();
            if (ticket == null)
            {
                Debug.LogError($"[{name}:{GetType().Name}] Ticket is not exist", gameObject);
                return;
            }

            var selectVTOL = this.hangar.VTOLs.FirstOrDefault();
            if(selectVTOL == null)
            {
                Debug.LogError($"VTOL is not exist");
                return;
            }

            var route = this.Routes.Find(x => ticket.Destination == x.Destination);
            if(route == null)
            {
                Debug.LogError($"[{name}:{GetType().Name}] Route is not exist", gameObject);
                Destroy(ticket);
                return;
            }

            if(ticket != null && selectVTOL != null)
            {
                selectVTOL.AssignRoute(route);
            }
        }

        public void Enter(VTOL vtol)
        {
            this.Hangar.VTOLs.Add(vtol);

        }

        [Button]
        public void MakeAndSetRoute()
        {
            var route = this.route.Routes[Random.Range(0, this.route.Routes.Count)];

            var vtol = MakeVTOL(VTOLType.LongTerm);

            SetTask(vtol, route);
        }


    }


}
