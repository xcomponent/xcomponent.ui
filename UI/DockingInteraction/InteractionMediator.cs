using System;
using System.Collections.Generic;
using System.Linq;

namespace XComponent.Common.UI.DockingInteraction
{
    public class InteractionMediator : IInteractionMediator, IDisposable
    {
        private IParticipantStore _participantStore;
        private readonly Func<string, string> _translator;

        public InteractionMediator(IParticipantStore participantStore, Func<string, string> translator)
        {
            _participantStore = participantStore;
            _translator = translator;
            _participantStore.ParticipantsAdded += ParticipantStoreOnParticipantsAdded;
            _participantStore.ParticipantsRemoved += ParticipantStoreOnParticipantsRemoved;
        }

        public string TranslateResource(string resourceKey)
        {
            return _translator(resourceKey);
        }

        public bool RunInteraction(Interaction interaction)
        {
            bool interactionSucceeded = false;
            if (interaction.InteractionPattern == InteractionPattern.SelectedParticipant)
            {
                var selectedPanel = _participantStore.GetSelectedParticipant();
                if (selectedPanel == null)
                {
                    return false;
                }
                return selectedPanel.HandleInteraction(interaction.Action, interaction.Parameters);
            }

            // Check if we have to create a new participant. This is only done for enslave and peer to peer interactions
            if (interaction.InteractionPattern == InteractionPattern.Enslave || interaction.InteractionPattern == InteractionPattern.PeerToPeer)
            {
                foreach (IInteractionParticipantCreator participantCreator in _participantStore.GetParticipantCreators())
                {
                    if (interaction.Target.ParticipantType == participantCreator.Identity.ParticipantType)
                    {
                        IEnumerable<IInteractionParticipant> newParticipants =
                            participantCreator.HandleInteraction(interaction.Action, interaction.Parameters).ToList();
                        interactionSucceeded = newParticipants.Any();

                        foreach (IInteractionParticipant newParticipant in newParticipants)
                        {
                            if (interaction.InteractionPattern == InteractionPattern.Enslave)
                            {
                                var newEnslavableParticipant = newParticipant as IEnslavableInteractionParticipant;
                                if (newEnslavableParticipant != null)
                                {
                                    newEnslavableParticipant.Master = interaction.Source;
                                    newEnslavableParticipant.HandleInteraction(interaction.Action, interaction.Parameters);
                                }
                            }
                            else if (interaction.InteractionPattern == InteractionPattern.PeerToPeer)
                            {
                                newParticipant.HandleInteraction(interaction.Action, interaction.Parameters);
                            }
                            _participantStore.AddParticipant(newParticipant);
                        }
                    }
                }
            }

            foreach (IInteractionParticipant participant in _participantStore.GetParticipants())
            {
                if (interaction.InteractionPattern == InteractionPattern.MasterSlave)
                {
                    var enslavableParticipant = participant as IEnslavableInteractionParticipant;
                    if (enslavableParticipant != null)
                    {
                        if (enslavableParticipant.Master != null
                            && enslavableParticipant.Master.UniqueName == interaction.Source.UniqueName
                            && enslavableParticipant.Identity.ParticipantType == interaction.Target.ParticipantType)
                        {
                            bool slaveHandled = enslavableParticipant.HandleInteraction(interaction.Action, interaction.Parameters);
                            if (!interactionSucceeded)
                            {
                                interactionSucceeded = slaveHandled;
                            }
                        }
                    }
                }
                else if (interaction.InteractionPattern == InteractionPattern.Enslave)
                {
                    var enslavableParticipant = participant as IEnslavableInteractionParticipant;
                    if (enslavableParticipant != null)
                    {
                        if (enslavableParticipant.Identity.UniqueName == interaction.Target.UniqueName)
                        {
                            enslavableParticipant.Master = interaction.Source;
                            interactionSucceeded = enslavableParticipant.HandleInteraction(interaction.Action, interaction.Parameters);
                        }
                    }
                }
                else if (interaction.InteractionPattern == InteractionPattern.PeerToPeer)
                {
                    if (participant.Identity.UniqueName == interaction.Target.UniqueName)
                    {
                        interactionSucceeded = participant.HandleInteraction(interaction.Action, interaction.Parameters);
                        // this is a peer to peer interaction, the target is unique
                        break;
                    }
                }
                else if (interaction.InteractionPattern == InteractionPattern.Broadcast)
                {
                    if (interaction.Source.UniqueName != participant.Identity.UniqueName)
                    {
                        participant.HandleInteraction(interaction.Action, interaction.Parameters);
                    }
                    // always return true for broadcast interactions
                    interactionSucceeded = true;
                }
            }

            return interactionSucceeded;
        }

        public event EventHandler<EventArgs> ParticipantsChanged;

        public IEnumerable<ParticipantIdentity> GetParticipants(IEnumerable<IParticipantType> peerTypes)
        {
            var participants = new List<ParticipantIdentity>();

            foreach (IParticipantType peerType in peerTypes)
            {
                participants.AddRange(
                    from interactionParticipantCreator in _participantStore.GetParticipantCreators()
                    where peerType == interactionParticipantCreator.Identity.ParticipantType
                    select interactionParticipantCreator.Identity);

                participants.AddRange(
                    from interactionParticipant in _participantStore.GetParticipants()
                    where peerType == interactionParticipant.Identity.ParticipantType
                    select interactionParticipant.Identity);
            }

            return participants.Distinct(ParticipantIdentity.UniqueNameParticipantTypeComparer).ToList();
        }

        public IEnumerable<ParticipantIdentity> GetParticipants(IParticipantType peerTypes)
        {
            return GetParticipants(Enumerable.Repeat(peerTypes, 1));
        }

        private void ParticipantStoreOnParticipantsRemoved(object sender, ParticipantsChangedEventArgs eventArgs)
        {
            foreach (IInteractionParticipant removedParticipant in eventArgs.Participants)
            {
                removedParticipant.Identity.Changed -= OnParticipantIdentityChanged;
            }

            NotifyParticipantsChanged();

        }
        
        private void ParticipantStoreOnParticipantsAdded(object sender, ParticipantsChangedEventArgs eventArgs)
        {
            foreach (IInteractionParticipant addedParticipant in eventArgs.Participants)
            {
                addedParticipant.Mediator = this;
                addedParticipant.Identity.Changed += OnParticipantIdentityChanged;

                var enslavableParticipant = addedParticipant as IEnslavableInteractionParticipant;
                IEnumerable<IInteractionParticipant> participants = _participantStore.GetParticipants();
                if (enslavableParticipant == null)
                {
                    // Master has just been added -> update slave identities
                    foreach (IEnslavableInteractionParticipant enslavedParticipant in participants.OfType<IEnslavableInteractionParticipant>()
                                .Where(
                                    p =>
                                        p.Master != null &&
                                        p.Master.ParticipantType ==
                                        addedParticipant.Identity.ParticipantType &&
                                        p.Master.UniqueName == addedParticipant.Identity.UniqueName))
                    {
                        enslavedParticipant.Master = addedParticipant.Identity;
                    }
                }
                else
                {
                    if (enslavableParticipant.Master != null)
                    {
                        // Slave has just been added -> update it's Master identity
                        var master = participants.FirstOrDefault(
                            p =>
                                enslavableParticipant.Master.ParticipantType == p.Identity.ParticipantType &&
                                enslavableParticipant.Master.UniqueName == p.Identity.UniqueName);

                        if (master != null)
                        {
                            enslavableParticipant.Master = master.Identity;
                        }
                    }
                }
            }

            NotifyParticipantsChanged();
        }

        private void OnParticipantIdentityChanged(object sender, IdentityChangedEventArgs eventArgs)
        {
            NotifyParticipantsChanged();
        }

        private void NotifyParticipantsChanged()
        {
            EventHandler<EventArgs> participantsChanged = ParticipantsChanged;
            if (participantsChanged != null)
            {
                participantsChanged(this, EventArgs.Empty);
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    if (_participantStore != null)
                    {
                        _participantStore.ParticipantsAdded -= ParticipantStoreOnParticipantsAdded;
                        _participantStore.ParticipantsRemoved -= ParticipantStoreOnParticipantsRemoved;
                        _participantStore = null;
                    }
                }

                _disposed = true;
            }
        }

        ~InteractionMediator()
        {
            Dispose(false);
        }

        private bool _disposed;
    }
}