namespace XComponent.Common.UI.DockingInteraction
{
    public interface IEnslavableInteractionParticipant : IInteractionParticipant
    {
        ParticipantIdentity Master { get; set; }
        string SlaveTitle { get; set; }
    }
}