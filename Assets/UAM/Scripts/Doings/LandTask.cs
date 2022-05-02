using System.Collections;
using UnityEngine;
using Alkemic.Doings;

namespace Alkemic.UAM
{
    public class LandTask : Doing
    {
        [PropertyGroup]
        [ShowOnly]
        private VTOL target;
        public VTOL Target => target;

        private VertiPort vertiPort;

        protected override void OnPreAwake()
        {
            base.OnPreAwake();
            this.CacheComponentInParent(ref target);
        }

        public override void InitTask()
        {
            base.InitTask();
            this.vertiPort = target.CurLocation as VertiPort;
            if(vertiPort == null)
            {
                Debug.LogError($"[{name}:{GetType().Name}] Land point is not VertiPort", gameObject);
            }
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
                if (target.transform.position.y <= UAMDefine.DefaultLandHeight)
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
