using Alkemic.Collections;
using Sirenix.OdinInspector;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Alkemic.UAM
{

    public class RouteControl : BaseComponent
    {
        [CacheGroup]
        [Debug]
        private VertiPort parentVertiPort;

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

            this.CacheComponentInParent(ref parentVertiPort);
        }

        protected override void Start()
        {
            base.Start();
            SyncRoutes();
        }

        private void SyncRoutes()
        {
            string vertiPortKey = parentVertiPort?.Key;
            var routeDatas = UAM.RouteContainer.RouteDatas.Where(x => x.VertiPortKey == vertiPortKey);
            foreach(var routeData in routeDatas)
            {
                CreateRoute(routeData);
            }
        }

        public void CreateRoute(RouteData data)
        {
            var newRoute = gameObject.AddComponent<Route>();
            newRoute.Init(data);
            this.routes.Add(newRoute);
        }
       
    }


}

