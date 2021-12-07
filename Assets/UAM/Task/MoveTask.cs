﻿using Sirenix.OdinInspector;
using System;
using System.Collections;
using UnityEngine;


namespace Alkemic.UAM
{


    [Serializable]
    public class MoveTask : Task
    {
        private VTOL target;
        public VTOL Target => target;

        /*
         * 비행기 2대
         * 단거리 20분 이내
         * 
         * 중장거리 전역 이동
         *
         * 
         * 1m => 
         * 서쪽 600m
         * 동쪽 300m 
         * 
         */

        [InlineEditor]
        [SerializeField]
        private Way way = null;
        public Way Way
        {
            set => way = value;
            get => way;
        }

        protected override void OnPreAwake()
        {
            base.OnPreAwake();
            this.CacheComponentInParent(ref target);
        }

        public override IEnumerator TaskRoutine()
        {
            InitTask();
            this.Way.MovingVTOLs.Add(Target);
            while (true)
            {
                if (this.Status == TaskStatus.Inturrupted) { break; }
                if (Equals(Target.CurLocation, this.Way.To) == true)
                {
                    break;
                }

                TickTask();
                yield return UAMStatic.TICK;
            }
            this.Way.MovingVTOLs.Remove(Target);
            OverTask();
        }

        public override void TickTask()
        {
            base.TickTask();
            if (ParentTaskControl != null && ParentTaskControl.IsDebug == true)
            {
                Vector3 lineStart = target.transform.position;
                Vector3 lineEnd = this.Way.To.transform.position;
                Debug.DrawLine(lineStart, lineEnd, Color.green, UAMStatic.TICK_TIME);
            }


        }
    }




}
