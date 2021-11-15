using Sirenix.OdinInspector;
using System;
using System.Collections;

namespace UAM
{
    [Serializable]
    public class EVTOL_TakeOffTask : Task
    {
        [ReadOnly, ShowInInspector]
        private EVTOL target;
        public EVTOL Target => target;

        private Way takeOffWay = null;
        public Way TakeOffWay
        {
            set => takeOffWay = value;
            get => takeOffWay;
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
                if (Equals(Target.CurLocation, TakeOffWay.To) == true)
                {
                    break;
                }

                TickTask();
                yield return UAMStatic.TICK;
            }
            OverTask();
        }

    }


    [Serializable]
    public class EVTOL_MoveTask : Task
    {
        [ReadOnly, ShowInInspector]
        private EVTOL target;
        public EVTOL Target => target;

        private Way way = null;
        public Way Way
        {
            set => way = value;
            get => way;
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
            this.Way.MovingEVTOLs.Add(Target);
            while(true)
            {
                if(isInterrupted == true)
                {
                    break;
                }
                if(Equals(Target.CurLocation, Way.To) == true)
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
