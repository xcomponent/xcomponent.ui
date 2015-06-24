using System.Collections.Generic;

namespace XComponent.Common.UI.DockingInteraction
{
    public interface IInteractionParticipant
    {
        ParticipantIdentity Identity { get; }

        bool HandleInteraction(string action, IEnumerable<Dictionary<string, object>> parameters);

        IInteractionMediator Mediator { get; set; }
    }
}
