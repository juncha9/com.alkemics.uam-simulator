using UnityEngine;
using Alkemic.Collections;
using System.Collections;
using UnityEngine.UI;
using System;

#if UNITY_EDITOR
#endif

namespace Alkemic.UAM
{


    [RequireComponent(typeof(DataCache))]
    public class VertiPortIndicator : LeadComponent
    {
        [CacheComponent]
        private VertiPort vertiPort;

        public TicketControl TicketControl
        {
            get 
            {
                return vertiPort?.TicketControl; 
            }
        }

        public HangarControl HangarControl
        {
            get
            {
                return vertiPort?.Hangar;
            }
        }

        [InstanceGroup]
        [SerializeField]
        private DestroyableList<DataCache> ticketItems = new DestroyableList<DataCache>();
        public DestroyableList<DataCache> TicketItems => ticketItems;

        [InstanceGroup]
        [SerializeField]
        private DestroyableList<DataCache> _VTOLItems = new DestroyableList<DataCache>();
        public DestroyableList<DataCache> VTOLItems => _VTOLItems;

        protected override void OnPreAwake()
        {
            base.OnPreAwake();
            this.CacheComponentInParent(ref vertiPort);
        }

        protected override void Awake()
        {
            base.Awake();

            
        }

        protected override void Start()
        {
            base.Start();


            StartAutoCoroutine(SyncTicket());
            StartAutoCoroutine(SyncHangar());
        }

        IEnumerator SyncTicket()
        {
            WaitForSeconds delay = new WaitForSeconds(AppDefine.DEFAULT_DELAY_TIME);
            while(true)
            {
                yield return delay;
                if (TicketControl == null) continue;

                var tickets = TicketControl.Tickets;
                DataCache.Set("ticket_count", tickets.Count.ToString());

                for(int i = 0; i < TicketItems.Count; i++)
                {
                    var item = TicketItems[i];
                    if (tickets.IsValidIndex(i) == true)
                    {
                        var ticket = tickets[i];
                        item.Set("set_time", ticket.Time.ToString("HH:mm"));
                        item.Set("source", ticket.Source.Key);
                        item.Set("destination", ticket.Destination.Key);
                    }
                    else
                    {
                        item.Set("set_time", null);
                        item.Set("source", null);
                        item.Set("destination", null);
                    }
                }
                
            }
        }

        IEnumerator SyncHangar()
        {
            WaitForSeconds delay = new WaitForSeconds(AppDefine.DEFAULT_DELAY_TIME);
            while (true)
            {
                yield return delay;
                if (HangarControl == null) continue;
                var VTOLs = HangarControl.VTOLs;
                DataCache.Set("vtol_count", VTOLs.Count.ToString());

                for (int i = 0; i < VTOLItems.Count; i++)
                {
                    var item = VTOLItems[i];
                    if (VTOLs.IsValidIndex(i) == true)
                    {
                        var vtol = VTOLs[i];
                        item.Set("key", vtol.Key);
                        item.Set("state", vtol.State.ToString());
                        //item.Set("sourc", vtol.Source.Key);
                        //item.Set("destination", vtol.Destination.Key);
                    }
                    else
                    {
                        item.Set("key", null);
                        item.Set("state", null);
                    }
                }
                
            }
        }



    }


}
