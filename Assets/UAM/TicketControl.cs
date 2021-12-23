﻿using System.Collections.Generic;

namespace Alkemic.UAM
{
    public class TicketControl : BaseComponent
    {
        [CacheComponent]
        private VertiPort vertiPort;

        [InstanceGroup]
        [ShowOnly]
        private List<Ticket> tickets = new List<Ticket>();
        public List<Ticket> Tickets => tickets;

        protected override void OnPreAwake()
        {
            base.OnPreAwake();

            this.CacheComponentInParent(ref vertiPort);
        }

        protected override void Awake()
        {
            base.Awake();
            
        }

        public void OpenTicket(VertiPort source, VertiPort destination)
        {
            var ticket = gameObject.AddComponent<Ticket>();
            ticket.Init(source, destination);

            this.Tickets.Add(ticket);
        }
    }


}

