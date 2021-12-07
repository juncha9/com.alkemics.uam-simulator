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
        private Location from;
        public Location From
        {
            set => from = value;
            get => from;
        }

        [PropertyGroup]
        [SerializeField]
        private Location to;
        public Location To
        {
            set => to = value;
            get => to;
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
            get => $"{from?.Key}_{to?.Key}";
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
                this.name = $"{GetType().Name} [{from?.Key}]-[{to?.Key}]";
            }
        }

        public void Setup(Location from, Location to, bool isOneWay = false)
        {
            this.from = from;
            this.to = to;
            this.isOneWay = isOneWay;
            ReloadName();
        }

        private void OnDrawGizmos()
        {
            Color color;
            GUIStyle style;
            if (movingVTOLs.Count > 0)
            {
                style = new GUIStyle();
                style.normal.textColor = Color.white;
                style.fontSize = 10;
                style.fontStyle = FontStyle.Bold;
                style.alignment = TextAnchor.MiddleLeft;
                for (int i = 0; i < movingVTOLs.Count; i++)
                {
                    var vtol = movingVTOLs[i];
                    Handles.Label(vtol.transform.position + (vtol.transform.right * 50f), $"[{i + 1}]", style);
                }
            }

            if (from == null || to == null)
            {
                wayLines.Dispose();
                return;
            }
            else
            {
                if (Selection.activeGameObject == this.gameObject)
                {
                    color = ColorDefine.RED_ORANGE;
                }
                else
                {
                    if (isOneWay == false)
                    {
                        color = UAMStatic.LineColor;
                    }
                    else
                    {
                        color = UAMStatic.AltLineColor;
                    }
                }
            }

            style = new GUIStyle();
            style.normal.textColor = Color.white;
            style.fontSize = 10;
            style.fontStyle = FontStyle.Bold;
            style.alignment = TextAnchor.MiddleLeft;
            Vector3 pos = (from.transform.position + to.transform.position) / 2f;
            Handles.Label(pos + (Vector3.up * 500f), $"[{this.from.Key}]\n[{this.to.Key}]", style);

            if (isOneWay == false)
            {
                wayLines[0] = new Line(
                    new Vector3(0, 0, 0),
                    To.transform.position - From.transform.position,
                    color,
                    color,
                    UAMStatic.LineWidth,
                    UAMStatic.LineWidth);
            }
            else
            {
                wayLines[0] = new Line(
                    new Vector3(0, 0, 0),
                    To.transform.position - From.transform.position,
                    color,
                    color,
                    UAMStatic.LineWidth + 2f,
                    UAMStatic.LineWidth - 2f);
            }

            wayLines.DrawNow(transform.localToWorldMatrix);
        }
    
        private void Update()
        {
            if (from == null || to == null)
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
                    From.transform.position,
                    To.transform.position,
                    lineColor,
                    lineColor,
                    UAMStatic.LineWidth,
                    UAMStatic.LineWidth);
            }
            else
            {
                wayLines[0] = new Line(
                    From.transform.position,
                    To.transform.position,
                    lineColor,
                    lineColor,
                    UAMStatic.LineWidth + 2f,
                    UAMStatic.LineWidth - 2f);
            }

            wayLines.Draw();
        }

        private void FixedUpdate()
        {
                if (To != null && movingVTOLs.Count > 0)
                {
                    movingVTOLs.Sort((a, b) =>
                    {
                        if (a == null) return 1;
                        else if (b == null) return -1;

                        if (Vector3.Distance(a.transform.position, To.transform.position) < Vector3.Distance(b.transform.position, To.transform.position))
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
