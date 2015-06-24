using System;
using System.Collections.Generic;

namespace XComponent.Common.UI.DockingInteraction
{
    public class ParticipantsChangedEventArgs : EventArgs
    {
        public IEnumerable<IInteractionParticipant> Participants { get; private set; }

        public ParticipantsChangedEventArgs(IEnumerable<IInteractionParticipant> participants)
        {
            Participants = participants;
        }
    }
}