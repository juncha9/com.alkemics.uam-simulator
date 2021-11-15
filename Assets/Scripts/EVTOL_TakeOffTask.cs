using Sirenix.OdinInspector;
using System;
using System.Collections;
using UnityEngine;

namespace UAM
{
    [Serializable]
    public class EVTOL_TakeOffTask : Task
    {
        [ReadOnly, ShowInInspector]
        private EVTOL target;
        public EVTOL Target => target;

        [SerializeField]
        private float height = 1000;
        public float Height
        {
            set => height = value;
            get => height;
        }

        protected override void OnPreAwake()
        {
            base.OnPreAwake();
            if (target == null)
            {
                target = GetComponentInParent<EVTOL>();
            }
        }

        public override IEnumerator TaskRoutine()
        {
            InitTask();
            while (true)
            {
                if (isInterrupted == true)
                {
                    break;
                }
                if (target.transform.position.y > height)
                {
                    break;
                }

                TickTask();
                yield return UAMStatic.TICK;
            }
            OverTask();
        }

    }




}
