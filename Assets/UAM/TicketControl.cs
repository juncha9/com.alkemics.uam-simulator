using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;

namespace Alkemic.UAM
{

    public class Ticket : BaseComponent
    {
        [HorizontalGroup("Time")]
        [ShowOnly]
        private DateTime time;
        public DateTime Time => time;

        [HorizontalGroup("Location")]
        [ShowOnly]
        private VertiPort source;
        public VertiPort Source
        {
            private set => source = value;
            get => source;
        }
        [HorizontalGroup("Location")]
        [ShowOnly]
        private VertiPort destination;
        public VertiPort Destination
        {
            private set => destination = value;
            get => destination;
        }

        private Ticket()
        {

        }

        public void Init(VertiPort source, VertiPort destination)
        {
            this.source = source;
            this.destination = destination;
        }


    }


    public class TicketControl : BaseComponent
    {
        private VertiPort vertiPort;

        private List<Ticket> tickets = new List<Ticket>();
        public List<Ticket> Tickets => tickets;

        protected override void OnPreAwake()
        {
            base.OnPreAwake();

            this.CacheComponentInParent(ref vertiPort);
        }

        public void OpenTicket(VertiPort source, VertiPort destination)
        {
            Ticket newTicket = new Ticket(source, destination);







        }
    }


}

