using System;
using System.Collections.Generic;

namespace XComponent.Common.UI.DockingInteraction
{
    public interface IInteractionMediator
    {
        bool RunInteraction(Interaction interaction);

        event EventHandler<EventArgs> ParticipantsChanged;

        IEnumerable<ParticipantIdentity> GetParticipants(IEnumerable<IParticipantType> peerTypes);

        IEnumerable<ParticipantIdentity> GetParticipants(IParticipantType peerTypes);

        string TranslateResource(string resourceKey);
    }
}