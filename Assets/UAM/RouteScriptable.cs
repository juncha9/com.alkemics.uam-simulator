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

#if UNITY_EDITOR
        [HeaderGroup]
        //[ValueDropdown("@Alkemic.UAM.UAMSimulator.GetSimulators()")]
        [ShowInInspector]
        public UAMSimulator GUI_Simulator = null;

        private void OnEnable()
        {
            GUI_Simulator = GameObject.FindObjectOfType<UAMSimulator>();
        }

        private void OnDisable()
        {
            GUI_Simulator = null;
        }

        public RouteData EditingRoute
        {
            get
            {
                return RouteDatas.Find(x => x.IsEdit == true);
            }
        }
#endif


        [SerializeField]
        public List<RouteData> RouteDatas = new List<RouteData>();




    }


}

