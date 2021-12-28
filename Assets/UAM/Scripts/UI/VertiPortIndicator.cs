using UnityEngine;
using Alkemic.Collections;
using System.Collections;

#if UNITY_EDITOR
#endif

namespace Alkemic.UAM
{
    public class VertiPortIndicator : BaseComponent
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
        }

        IEnumerator SyncTicket()
        {
            WaitForSeconds delay = new WaitForSeconds(0.5f);
            while(true)
            {
                var tickets = TicketControl.Tickets;
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
                        item.Set("dest", null);
                    }
                }
                yield return delay;
            }
        }

        IEnumerable SyncHangar()
        {
            WaitForSeconds delay = new WaitForSeconds(0.5f);
            while (true)
            {
                yield return delay;
                var VTOLs = HangarControl.VTOLs;
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
                        //item.Set("source", null);
                        //item.Set("dest", null);
                    }
                }
                
            }
        }



    }


}
