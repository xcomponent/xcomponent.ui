using System.Collections.Generic;

namespace XComponent.Common.UI.DockingInteraction
{
    public interface IInteractionParticipantCreator
    {
        ParticipantIdentity Identity { get; }

        IEnumerable<IInteractionParticipant> HandleInteraction(string action, IEnumerable<Dictionary<string, object>> parameters);
    }
}