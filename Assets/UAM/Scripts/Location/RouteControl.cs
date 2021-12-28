using Alkemic.Collections;
using System.Linq;
using UnityEngine;

namespace Alkemic.UAM
{

    public class RouteControl : BaseComponent
    {
        [CacheGroup]
        [Debug]
        private VertiPort vertiPort;

        [InstanceGroup]
        [ShowOnly]
        private DestroyableList<Route> routes = new DestroyableList<Route>();
        public DestroyableList<Route> Routes => routes;

        protected override void OnValidate()
        {
            base.OnValidate();
        }

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

            SyncRoutes();
        }

        private void SyncRoutes()
        {
            var routeDatas = UAM.Route.RouteDatas.Where(x => x.Source == vertiPort.Key);
            foreach (var routeData in routeDatas)
            {
                CreateRoute(routeData);
            }
        }

        public void CreateRoute(RouteData data)
        {
            var newRoute = gameObject.AddComponent<Route>();
            newRoute.Setup(data);
            this.routes.Add(newRoute);
        }

    }


}

