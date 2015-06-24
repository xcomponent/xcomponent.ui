using System.Collections.Generic;

namespace XComponent.Common.UI.DockingInteraction
{
    public interface IInteractionParticipantWithContextMenu : IInteractionParticipant
    {
        IEnumerable<IParticipantType> ContextMenuPeerTypes { get; }
        IEnumerable<IParticipantType> ContextMenuSlaveTypes { get; }
    }
}