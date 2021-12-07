using Alkemic.Collections;
using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;

namespace Alkemic.UAM
{

    public class RouteControl : BaseComponent
    {


        [InstanceGroup]
        [ShowOnly]
        [SerializeField]
        private DestroyableList<Route> routes = new DestroyableList<Route>();
        public DestroyableList<Route> Routes => routes;

        protected override void OnValidate()
        {
            base.OnValidate();

            this.SyncComponentsWithChildren(routes);
        }

        [Button]
        public void AddRoute()
        {
            this.gameObject.AddComponent<Route>();
        }
       
    }


}

