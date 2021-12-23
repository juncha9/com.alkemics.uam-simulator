using Alkemic.Collections;
using UnityEngine;

namespace Alkemic.UAM
{
    public class HangarControl : BaseComponent
    {
        [CacheComponent]
        private VertiPort vertiPort;

        [OptionGroup]
        [SerializeField]
        private int startVTOL_LTCount = 0;

        [OptionGroup]
        [SerializeField]
        private int startVTOL_STCount = 0;

        [InstanceGroup]
        [ShowOnly]
        private DestroyableList<VTOL> _VTOLs = new DestroyableList<VTOL>();
        public DestroyableList<VTOL> VTOLs => _VTOLs;

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

            InitVTOL();
        }

        private void InitVTOL()
        {
            for(int i =0; i< startVTOL_LTCount; i++)
            {
                MakeVTOL(VTOLType.LongTerm);
            }

            for(int i =0; i < startVTOL_STCount; i++)
            {
                MakeVTOL(VTOLType.ShortTerm);
            }
        }


        public VTOL MakeVTOL(VTOLType type)
        {
            Transform parent = vertiPort.ParentSimulator.VTOLParent;
            VTOL vtol = null;
            switch (type)
            {
                case VTOLType.LongTerm:
                    vtol = Instantiate(UAMManager.Inst.LT_VTOLPrefab, parent).GetComponent<VTOL>();
                    break;
                case VTOLType.ShortTerm:
                    vtol = Instantiate(UAMManager.Inst.ST_VTOLPrefab, parent).GetComponent<VTOL>();
                    break;
            }
            vtol.transform.position = transform.position;
            vtol.CurLocation = vertiPort;
            this.VTOLs.Add(vtol);
            return vtol;
        }
    }
}
