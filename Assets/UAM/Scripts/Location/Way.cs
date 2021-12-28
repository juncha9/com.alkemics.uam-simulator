using Alkemic.Collections;
//using Linefy;
using UnityEngine;
using Shapes;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Alkemic.UAM
{
    [ExecuteInEditMode]
    public class Way : BaseComponent
    {
        [CacheComponent]
        private Line visibleLine;
        public Line VisibleLine => visibleLine;

        [CacheComponent]
        private UAMSimulator simulator;
        public UAMSimulator Simulator => simulator;

        [CacheComponent]
        private LocationControl locationControl;
        public LocationControl ParentLocationControl => locationControl;

        [CacheComponent]
        private Location location;
        public Location Location => location;

        public Location LocationA
        {
            get => location;
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
            get => $"{LocationA?.Key}_{LocationB?.Key}";
        }

        [InstanceGroup]
        [RuntimeOnly]
        private DestroyableList<VTOL> movingVTOLs = new DestroyableList<VTOL>();
        public DestroyableList<VTOL> MovingVTOLs => movingVTOLs;

        //private Lines wayLine = new Lines(1);

        protected override void OnPreAwake()
        {
            base.OnPreAwake();
            this.CacheComponent(ref visibleLine);

            this.CacheComponentInParent(ref simulator);
            this.CacheComponentInParent(ref location);
            this.CacheComponentInParent(ref locationControl);
        }

        protected override void OnValidate()
        {
            base.OnValidate();

            if (transform.position.y != UAMStatic.DefaultAirHeight)
            {
                transform.position = new Vector3(transform.position.x, UAMStatic.DefaultAirHeight, transform.position.z);
            }

            UpdateNameOnValid();
            SetupLine();
        }

        protected override void Awake()
        {
            base.Awake();

            
        }

        protected override void Start()
        {
            base.Start();


            SetupLine();
        }

        public void UpdateNameOnValid()
        {
            if (EditorHelper.IsPrefabMode == false)
            {
                this.name = $"{GetType().Name} [{LocationA?.Key}]-[{LocationB?.Key}]";
            }
        }

        public void Setup(Location to, bool isOneWay = false)
        {
            this.locationB = to;
            this.isOneWay = isOneWay;
        }

        private void OnDrawGizmos()
        {
            //GUI_DrawText();
            //GUI_DrawLine();
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

        private void Update()
        {
            UpdateLine();
        }
       
        private void SetupLine()
        {
            if (visibleLine == null) return;
            if (LocationA?.AirAnchor == null || LocationB?.AirAnchor == null)
            {
                this.visibleLine.Start = new Vector3(0f, 0f, 0f);
                this.visibleLine.End = new Vector3(0f, 0f, 0f);
                return;
            }

            this.visibleLine.Start = transform.InverseTransformPoint(LocationA.AirAnchor.transform.position);
            this.visibleLine.End = transform.InverseTransformPoint(LocationB.AirAnchor.transform.position);
        }


        private void UpdateLine()
        {
            if (visibleLine == null) return;

            Color color;

#if UNITY_EDITOR
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
#else
            if (isOneWay == false)
            {
                color = UAMStatic.LineColor;
            }
            else
            {
                color = UAMStatic.AltLineColor;
            }
#endif


            this.visibleLine.Color = color;
        }


        private void FixedUpdate()
        {
            SortMovingVTOLs();
        }

        private void SortMovingVTOLs()
        {
            if (LocationA == null || LocationB == null) return;
            if (movingVTOLs.Count <= 0) return;

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


#if UNITY_EDITOR

        /*
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
        */

#endif


    }
}
