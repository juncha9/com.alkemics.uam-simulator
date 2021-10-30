using Sirenix.OdinInspector;
using UnityEngine;

namespace UAM
{
    public class UAMSimulator : Behavior
    {
        [SerializeField]
        private bool m_IsMain = false;
        public bool isMain => m_IsMain;

        [SerializeField]
        private Transform m_UAMParent;
        public Transform uamParent => m_UAMParent;

        [SerializeField]
        private Color m_LineColor = Color.yellow;
        public Color lineColor => m_LineColor;

        [Range(2f, 10f)]
        [SerializeField]
        private float m_LineWidth = 2f;
        public float lineWidth => m_LineWidth;

        [ReadOnly, ShowInInspector]
        private LocationControl m_LocationControl;
        public LocationControl locationControl => m_LocationControl;

        protected override void OnPreAwake()
        {
            base.OnPreAwake();
            if(m_LocationControl == null)
            {
                m_LocationControl = GetComponentInChildren<LocationControl>();
            }

        }

    }


}
