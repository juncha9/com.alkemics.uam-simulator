using Sirenix.OdinInspector;
using System;

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

        public void Init(VertiPort source, VertiPort destination)
        {
            this.source = source;
            this.destination = destination;
        }
    }


}

