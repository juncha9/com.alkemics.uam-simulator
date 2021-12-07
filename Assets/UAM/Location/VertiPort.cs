using Linefy.Primitives;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Alkemic.UAM
{
    public enum VTOLType
    {
        LongTerm,
        ShortTerm
    }

    public class VertiPort : Location
    {

        [CacheGroup]
        [Debug]
        private RouteControl routeControl;
        public RouteControl RouteControl => routeControl;

        [PropertyGroup]
        [ShowOnly]
        private VTOL ClimbingVTOL = null;

        [PropertyGroup]
        [ShowOnly]
        private VTOL LandingVTOL = null;

        [OptionGroup]
        [ValueDropdown("@RouteControl.Routes")]
        [SerializeField]
        private Route selectedRoute;

        [PresetComponent]
        [SerializeField]
        private Transform landingAnchor;


        protected override void OnPreAwake()
        {
            base.OnPreAwake();

            this.CacheComponentInChildren(ref routeControl);
        }

        protected override void Awake()
        {
            base.Awake();

            Debug.Assert(routeControl != null, $"[{name}] {nameof(routeControl)} is null", gameObject);
        }

        [Button]
        public void MakeAndRoute(VTOLType type)
        {
            if(selectedRoute == null)
            {
                Debug.LogError($"[{name}] Route is not selected", gameObject);
                return;
            }

            var vtol = MakeVTOL(type);

            SetTask(vtol, selectedRoute);
        }
        
        public VTOL MakeVTOL(VTOLType type)
        {
            VTOL vtol = null;
            switch (type)
            {
                case VTOLType.LongTerm:
                    vtol = Instantiate(ParentSimulator.LT_VTOLPrefab, ParentSimulator.VTOLParent).GetComponent<VTOL>();
                    break;
                case VTOLType.ShortTerm:
                    vtol = Instantiate(ParentSimulator.ST_VTOLPrefab, ParentSimulator.VTOLParent).GetComponent<VTOL>();
                    break;
            }
            vtol.transform.position = landingAnchor.position;
            return vtol;
        }


        public void SetTask(VTOL vtol, Route route)
        {
            if(vtol == null)
            {
                Debug.LogError($"[{name}] VTOL is null", gameObject);
                return;
            }
            if(route == null)
            {
                Debug.LogError($"[{name}] Route is null", gameObject);
                return;
            }

            vtol.TaskControl.AddTask<VerticalMoveTask>((task) =>
            {
                task.MoveType = VerticalMove.TakeOff;
            });

            foreach(var way in route.Ways)
            {
                vtol.TaskControl.AddTask<MoveTask>((task) =>
                {
                    task.Way = way;
                });
            }

            vtol.TaskControl.AddTask<VerticalMoveTask>((task) =>
            {
                task.MoveType = VerticalMove.Land;
            });

            //evtol.TaskControl.AddTask(new TakeOffTask());



        }

    }


}
