using Alkemic.Collections;
using UnityEngine;

namespace Alkemic.UAM
{
    public class HangarControl : BaseComponent
    {
        [CacheComponent]
        private VertiPort vertiPort;
        public VertiPort VertiPort => vertiPort;

        [InstanceGroup]
        [ShowOnly]
        private DestroyableList<VTOL> _VTOLs = new DestroyableList<VTOL>();
        public DestroyableList<VTOL> VTOLs => _VTOLs;

        [Debug]
        public UAMSimulator Simulator => vertiPort?.Simulator;

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
        }

        public VTOL CreateVTOL(string key)
        {
            var vtol = Simulator.CreateVTOL(key, VertiPort.GroundAnchor.position);
            if (vtol == null)
            {
                Debug.LogError($"[{name}:{GetType().Name}] Cannot creat VTOL[{key}]", gameObject);
                return null;
            }
            vtol.transform.position = transform.position;
            vtol.CurLocation = vertiPort;
            this.VTOLs.Add(vtol);
            return vtol;
        }

    }
}
