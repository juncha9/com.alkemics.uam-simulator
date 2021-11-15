using Linefy;
using Sirenix.OdinInspector;
using UnityEngine;

#if UNITY_EDITOR
#endif

namespace UAM
{
    public class Way : Behavior
    {

        private UAMSimulator parentSimulator;
        public UAMSimulator ParentSimulator => parentSimulator;

        private LocationControl parentLocationControl;
        public LocationControl ParentLocationControl => parentLocationControl;

        private Location location;
        public Location Location => location;
        
        [HorizontalGroup("Location")]
        [LabelWidth(20f)]
        [SerializeField]
        private Location from;
        public Location From
        {
            set => from = value;
            get => from;
        }

        [HorizontalGroup("Location")]
        [LabelWidth(20f)]
        [SerializeField]
        private Location to;
        public Location To
        {
            set => to = value;
            get => to;
        }

        [HorizontalGroup("Location")]
        [HideLabel]
        [SerializeField]
        private bool isOneWay = false;
        public bool IsOneWay
        {
            set => isOneWay = value;
            get => isOneWay;
        }

        [ShowInInspector, ReadOnly]
        private DestroyableList<EVTOL> movingEVTOLs = new DestroyableList<EVTOL>();
        public DestroyableList<EVTOL> MovingEVTOLs => movingEVTOLs;

        private Lines drawinglines = new Lines(1);
        private Lines drawingLinesOnGizmo = new Lines(1);


        protected override void OnPreAwake()
        {
            base.OnPreAwake();
            if(location == null)
            {
                location = GetComponent<Location>();
            }
            if(parentSimulator == null)
            {
                parentSimulator = GetComponentInParent<UAMSimulator>();
            }
            if(ParentLocationControl == null)
            {
                parentLocationControl = GetComponentInParent<LocationControl>();
            }
        }

        public void Setup(Location from, Location to, bool isOneWay = false)
        {
            this.from = from;
            this.to = to;
            this.isOneWay = isOneWay;
        }

        

        private void OnDrawGizmos()
        {
            if (from == null || to == null)
            {
                drawingLinesOnGizmo.Dispose();
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
            drawingLinesOnGizmo[0] = new Line(
                new Vector3(0, 0, 0),
                To.transform.position - From.transform.position,
                lineColor,
                lineColor,
                UAMStatic.lineWidth,
                UAMStatic.lineWidth
                );
            drawingLinesOnGizmo.DrawNow(transform.localToWorldMatrix);
        }

        private void Update()
        {
            if (from == null || to == null)
            {
                drawinglines.Dispose();
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
            drawinglines[0] = new Line(
                From.transform.position,
                To.transform.position,
                lineColor,
                lineColor,
                UAMStatic.lineWidth,
                UAMStatic.lineWidth
                );
            drawinglines.Draw();
        }

    }


}
