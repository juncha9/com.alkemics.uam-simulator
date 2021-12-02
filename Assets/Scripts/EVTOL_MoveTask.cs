﻿using Sirenix.OdinInspector;
using System;
using System.Collections;
using UnityEngine;

namespace UAM
{


    [Serializable]
    public class EVTOL_MoveTask : Task
    {
        [ReadOnly, ShowInInspector]
        private EVTOL target;
        public EVTOL Target => target;

        /*
         * 비행기 2대
         * 단거리 20분 이내
         * 
         * 
         * 
         * 
         * 
         * 
         * 
         * 
         * 중장거리 전역 이동
         *
         * 
         * 1m => 
         * 서쪽 600m
         * 동쪽 300m 
         * 
         */

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
                if (Equals(Target.CurLocation, way.To) == true)
                {
                    break;
                }

                TickTask();
                yield return UAMStatic.TICK;
            }
            this.Way.MovingEVTOLs.Remove(Target);
            OverTask();
        }
    }




}