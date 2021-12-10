using Alkemic.Scriptables;
using Alkemic.Editors;
using System;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

namespace Alkemic.UAM
{

    [Serializable]
    public class RouteScriptable : Scriptable
    {
        [NonSerialized]
        private bool isEditing = false;

        [SerializeField]
        public List<RouteData> RouteDatas = new List<RouteData>();

        [ValueDropdown("@ParentLocationControl?.GetWayList()")]
        public List<Way> editWays = new List<Way>();

        private UAMSimulator

        private void GetWayList(LocationControl locationControl)
        {

        }

        [Button]
        public void EditRoute()
        {
            if(isEditing == true)
            {
                isEditing = false;
            }
            else
            {
                isEditing = true;
                if(EditingRoute == null)
                {
                    EditingRoute = new Route();
                }
            }


        }
    }


}

