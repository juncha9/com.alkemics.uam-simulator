using Linefy.Primitives;
using Sirenix.OdinInspector;
using System.Collections;
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
        private VTOL TakingVTOL = null;

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

        protected override void Start()
        {
            base.Start();

            StartAutoCoroutine(MakeVTOLRoutine());
        }


        private IEnumerator MakeVTOLRoutine()
        {
            float sleepTime = 2.5f;
            while(true)
            {
                if (routeControl.Routes == null || routeControl.Routes.Count <= 0)
                {
                    yield break;
                }

                MakeAndSetRoute();

                yield return new WaitForSeconds(sleepTime);
            }
        }

        [Button]
        public void MakeAndSetRoute()
        {
            var route = this.routeControl.Routes[Random.Range(0, routeControl.Routes.Count)];
            
            var vtol = MakeVTOL(VTOLType.LongTerm);

            SetTask(vtol, route);   
        }
        

        public VTOL MakeVTOL(VTOLType type)
        {
            VTOL vtol = null;
            switch (type)
            {
                case VTOLType.LongTerm:
                    vtol = Instantiate(UAMManager.Inst.LT_VTOLPrefab, ParentSimulator.VTOLParent).GetComponent<VTOL>();
                    break;
                case VTOLType.ShortTerm:
                    vtol = Instantiate(UAMManager.Inst.ST_VTOLPrefab, ParentSimulator.VTOLParent).GetComponent<VTOL>();
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

            Location preLocation = null;
            Location nextLocation = null;
            int i = 0;
            foreach (var way in route.Ways)
            {
                if(i == 0)
                {
                    if(Equals(way.LocationA.Key, this.Key) == true)
                    {
                        preLocation = way.LocationA;
                        nextLocation = way.LocationB;
                    }
                    else if(Equals(way.LocationB.Key, this.Key) == true)
                    {
                        preLocation = way.LocationB;
                        nextLocation = way.LocationA;
                    }
                    else
                    {
                        throw new System.Exception("Location not match");
                    }
                }
                else
                {
                    if(Equals(way.LocationA, nextLocation) == true)
                    {
                        preLocation = way.LocationA;
                        nextLocation = way.LocationB;
                    }
                    else if(Equals(way.LocationB, nextLocation) == true)
                    {
                        preLocation = way.LocationB;
                        nextLocation = way.LocationA;
                    }
                    else
                    {
                        throw new System.Exception("Location not match");
                    }
                }

                vtol.TaskControl.AddTask<MoveTask>((task) =>
                {
                    task.Way = way;
                    task.TargetLocation = nextLocation;
                });
                i++;
            }

            vtol.TaskControl.AddTask<VerticalMoveTask>((task) =>
            {
                task.MoveType = VerticalMove.Land;
            });

            //evtol.TaskControl.AddTask(new TakeOffTask());



        }

    }


}
