using System;
using System.Collections;
using UnityEngine;

namespace Alkemic.UAM
{
    public enum VerticalMove
    {
        TakeOff,
        Land
    }

    [Serializable]
    public class VerticalMoveTask : Task
    {
        [OptionGroup]
        [SerializeField]
        private VerticalMove moveType;
        public VerticalMove MoveType
        {
            set => moveType = value;
            get => moveType;
        }

        [CacheGroup]
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
                    if (target.transform.position.y >= UAMStatic.DefaultAirHeight)
                    {
                        DebugEx.DrawBounds(new Bounds(target.transform.position, new Vector3(50f, 50f, 50f)), Color.red, 1f);
                        break;
                    }
                    break;
                }
                if (moveType == VerticalMove.TakeOff && target.transform.position.y >= UAMStatic.DefaultAirHeight)
                {
                    DebugEx.DrawBounds(new Bounds(target.transform.position, new Vector3(50f, 50f, 50f)), Color.green, 1f);
                    break;
                }
                else if (moveType == VerticalMove.Land && target.transform.position.y <= UAMStatic.DefaultLandHeight)
                {
                    DebugEx.DrawBounds(new Bounds(target.transform.position, new Vector3(50f, 50f, 50f)), Color.green, 1f);
                    break;
                }
                TickTask();
                yield return UAMStatic.TICK;
            }
            OverTask();
        }

    }




}
