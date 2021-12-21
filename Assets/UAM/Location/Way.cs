using Alkemic.Collections;
using Linefy;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Alkemic.UAM
{
    public class Way : BaseComponent
    {
        [Debug]
        private UAMSimulator parentSimulator;
        public UAMSimulator ParentSimulator => parentSimulator;

        [Debug]
        private LocationControl parentLocationControl;
        public LocationControl ParentLocationControl => parentLocationControl;

        [Debug]
        private Location parentLocation;
        public Location ParentLocation => parentLocation;

        [PropertyGroup]
        [SerializeField]
        private Location locationA;
        public Location LocationA
        {
            set => locationA = value;
            get => locationA;
        }

        [PropertyGroup]
        [SerializeField]
        private Location locationB;
        public Location LocationB
        {
            set => locationB = value;
            get => locationB;
        }

        [PropertyGroup]
        [SerializeField]
        private bool isOneWay = false;
        public bool IsOneWay
        {
            set => isOneWay = value;
            get => isOneWay;
        }


        [PropertyGroup]
        [ShowOnly]
        public string Key
        {
            get => $"{locationA?.Key}_{locationB?.Key}";
        }

        [InstanceGroup]
        [RuntimeOnly]
        private DestroyableList<VTOL> movingVTOLs = new DestroyableList<VTOL>();
        public DestroyableList<VTOL> MovingVTOLs => movingVTOLs;

        private Lines wayLines = new Lines(1);



        private Lines selectionLines = new Lines(1);

        protected override void OnPreAwake()
        {
            base.OnPreAwake();
            this.CacheComponentInParent(ref parentSimulator);
            this.CacheComponentInParent(ref parentLocation);
            this.CacheComponentInParent(ref parentLocationControl);
        }

        protected override void OnValidate()
        {
            base.OnValidate();

            if (transform.position.y != UAMStatic.DefaultAirHeight)
            {
                transform.position = new Vector3(transform.position.x, UAMStatic.DefaultAirHeight, transform.position.z);
            }

            ReloadName();
        }

        public void ReloadName()
        {
            if (Helper.IsPrefabMode == false)
            {
                this.name = $"{GetType().Name} [{locationA?.Key}]-[{locationB?.Key}]";
            }
        }

        public void Setup(Location from, Location to, bool isOneWay = false)
        {
            this.locationA = from;
            this.locationB = to;
            this.isOneWay = isOneWay;
            ReloadName();
        }



        private void OnDrawGizmos()
        {
            GUI_DrawText();
            GUI_DrawLine();
            /*
            style = new GUIStyle();
            style.normal.textColor = Color.white;
            style.fontSize = 10;
            style.fontStyle = FontStyle.Bold;
            style.alignment = TextAnchor.MiddleLeft;
            Vector3 pos = (from.transform.position + to.transform.position) / 2f;
            Handles.Label(pos + (Vector3.up * 500f), $"[{this.from.Key}]\n[{this.to.Key}]", style);
            */ 

        }

#if UNITY_EDITOR
        private Lines GUI_wayLines = new Lines(1);

        private void GUI_DrawText()
        {
            GUIStyle style;
            Color color;

            if (movingVTOLs.Count > 0)
            {
                style = new GUIStyle();
                style.normal.textColor = Color.white;
                style.fontSize = 12;
                //style.fontStyle = FontStyle.Bold;
                style.alignment = TextAnchor.MiddleLeft;
                for (int i = 0; i < movingVTOLs.Count; i++)
                {
                    var vtol = movingVTOLs[i];
                    Handles.Label(vtol.transform.position + (vtol.transform.right * 50f), $"[{i + 1}]", style);
                }
            }

            
            if (UAM.Route?.EditingRoute != null &&
                UAM.Route?.EditingRoute.GUI_Ways.Contains(this) == true)
            {
                color = ColorDefine.RADICAL_RED;

                int index = UAM.Route.EditingRoute.GUI_Ways.IndexOf(this) + 1;

                style = new GUIStyle(GUI.skin.box);
                style.normal.textColor = ColorDefine.RADICAL_RED;
                style.fontSize = 12;
                style.fontStyle = FontStyle.Bold;
                style.alignment = TextAnchor.MiddleCenter;
                Handles.Label((locationA.transform.position + locationB.transform.position) / 2 + (Vector3.up * 1000f), $"{index}", style);
            }
        }

        private void GUI_DrawLine()
        {
            if (locationA == null || locationB == null)
            {
                GUI_wayLines.Dispose();
                return;
            }

            Color color;
            if (Selection.activeGameObject == this.gameObject)
            {
                color = ColorDefine.RED_ORANGE;
            }
            else if (UAM.Route?.EditingRoute != null &&
                UAM.Route?.EditingRoute.GUI_Ways.Contains(this) == true)
            {
                color = ColorDefine.RADICAL_RED;
            }
            else if (isOneWay == false)
            {
                color = UAMStatic.LineColor;
            }
            else
            {
                color = UAMStatic.AltLineColor;
            }
            if (isOneWay == false)
            {
                GUI_wayLines[0] = new Line(
                    LocationA.transform.position,
                    LocationB.transform.position,
                    color,
                    color,
                    UAMStatic.LineWidth,
                    UAMStatic.LineWidth);
            }
            else
            {
                /*
                wayLines[0] = new Line(
                    new Vector3(0, 0, 0),
                    LocationB.transform.position - LocationA.transform.position,
                    color,
                    color,
                    UAMStatic.LineWidth + 2f,
                    UAMStatic.LineWidth - 2f);
                */
                GUI_wayLines[0] = new Line(
                    LocationA.transform.position,
                    LocationB.transform.position,
                    color,
                    color,
                    UAMStatic.LineWidth + 2f,
                    UAMStatic.LineWidth - 2f);
            }

            GUI_wayLines.DrawNow(Gizmos.matrix);
        }
#endif

        private void Update()
        {
            if (locationA == null || locationB == null)
            {
                wayLines.Dispose();
            }
            Color lineColor;
            if (isOneWay == false)
            {
                lineColor = UAMStatic.LineColor;
            }
            else
            {
                lineColor = UAMStatic.AltLineColor;
            }
            if (isOneWay == false)
            {
                wayLines[0] = new Line(
                    LocationA.transform.position,
                    LocationB.transform.position,
                    lineColor,
                    lineColor,
                    UAMStatic.LineWidth,
                    UAMStatic.LineWidth);
            }
            else
            {
                wayLines[0] = new Line(
                    LocationA.transform.position,
                    LocationB.transform.position,
                    lineColor,
                    lineColor,
                    UAMStatic.LineWidth + 2f,
                    UAMStatic.LineWidth - 2f);
            }

            wayLines.Draw();
        }

        private void FixedUpdate()
        {
            if (LocationB != null && movingVTOLs.Count > 0)
            {
                movingVTOLs.Sort((a, b) =>
                {
                    if (a == null) return 1;
                    else if (b == null) return -1;

                    if (Vector3.Distance(a.transform.position, LocationB.transform.position) < Vector3.Distance(b.transform.position, LocationB.transform.position))
                    {
                        return -1;
                    }
                    else
                    {
                        return 1;
                    }

                });
            }
        }

    }
}
