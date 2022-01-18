using System;
using System.Collections;
using UnityEngine;

namespace Alkemic.UAM
{

    public class TakeOffTask : Task
    {
        [PropertyGroup]
        [ShowOnly]
        private VTOL target;
        public VTOL Target => target;

        protected override void OnPreAwake()
        {
            base.OnPreAwake();
            this.CacheComponentInParent(ref target);
        }

        public override IEnumerator TaskRoutine()
        {
            InitTask();
            while (true)
            {
                if (this.Status == TaskStatus.Inturrupted)
                {
                    DebugEx.DrawBounds(new Bounds(target.transform.position, new Vector3(50f, 50f, 50f)), Color.red, 1f);
                    break;
                }
                if (target.transform.position.y >= UAMDefine.DefaultAirHeight)
                {
                    DebugEx.DrawBounds(new Bounds(target.transform.position, new Vector3(50f, 50f, 50f)), Color.green, 1f);
                    break;
                }
                TickTask();
                yield return UAMDefine.TICK;
            }
            OverTask();
        }

    }




}
