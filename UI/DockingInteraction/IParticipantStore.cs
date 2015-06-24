using System;
using System.Collections.Generic;

namespace XComponent.Common.UI.DockingInteraction
{
    public interface IParticipantStore : IDisposable
    {
        event EventHandler<ParticipantsChangedEventArgs> ParticipantsAdded;
        event EventHandler<ParticipantsChangedEventArgs> ParticipantsRemoved;

        IEnumerable<IInteractionParticipantCreator> GetParticipantCreators();
        IEnumerable<IInteractionParticipant> GetParticipants();
        IInteractionParticipant GetSelectedParticipant();
        void AddParticipant(IInteractionParticipant newParticipant);
    }
}