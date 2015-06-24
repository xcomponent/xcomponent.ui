using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;

namespace XComponent.Common.UI.DockingInteraction
{
    public class ParticipantStore : IParticipantStore
    {
        public event EventHandler<ParticipantsChangedEventArgs> ParticipantsAdded;
        public event EventHandler<ParticipantsChangedEventArgs> ParticipantsRemoved;

        public ObservableCollection<object> ParticipantSource { get; private set; }
        public ObservableCollection<IInteractionParticipantCreator> ParticipantCreators { get; private set; }
        public object SelectedParticipant { get; set; }

        public ParticipantStore(ObservableCollection<object> participantSource, ObservableCollection<IInteractionParticipantCreator> participantCreators)
        {
            ParticipantSource = participantSource;
            ParticipantSource.CollectionChanged += PanelViewModelsOnCollectionChanged;

            ParticipantCreators = participantCreators;
        }

        private void PanelViewModelsOnCollectionChanged(object sender, NotifyCollectionChangedEventArgs eventArgs)
        {
            if (eventArgs.Action == NotifyCollectionChangedAction.Add)
            {
                NotifyParticipantsAdded(eventArgs.NewItems.OfType<IInteractionParticipant>());
            }
            else if (eventArgs.Action == NotifyCollectionChangedAction.Remove)
            {
                NotifyParticipantsRemoved(eventArgs.OldItems.OfType<IInteractionParticipant>());
            }
        }

        private void NotifyParticipantsAdded(IEnumerable<IInteractionParticipant> addedParticipants)
        {
            EventHandler<ParticipantsChangedEventArgs> participantsAddedHandler = ParticipantsAdded;
            if (participantsAddedHandler != null)
            {
                var participantsAddedEventArgs = new ParticipantsChangedEventArgs(addedParticipants);
                participantsAddedHandler(this, participantsAddedEventArgs);
            }
        }

        private void NotifyParticipantsRemoved(IEnumerable<IInteractionParticipant> removedParticipants)
        {
            EventHandler<ParticipantsChangedEventArgs> participantsRemovedHandler = ParticipantsRemoved;
            if (participantsRemovedHandler != null)
            {
                var participantsRemovedEventArgs = new ParticipantsChangedEventArgs(removedParticipants);
                participantsRemovedHandler(this, participantsRemovedEventArgs);
            }
        }

        public IEnumerable<IInteractionParticipantCreator> GetParticipantCreators()
        {
            return ParticipantCreators;
        }

        public IEnumerable<IInteractionParticipant> GetParticipants()
        {
            return ParticipantSource.OfType<IInteractionParticipant>();
        }

        public IInteractionParticipant GetSelectedParticipant()
        {
            return SelectedParticipant as IInteractionParticipant;
        }

        public void AddParticipant(IInteractionParticipant newParticipant)
        {
            ParticipantSource.Add(newParticipant);
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
                    ParticipantSource.CollectionChanged -= PanelViewModelsOnCollectionChanged;
                }

                _disposed = true;
            }
        }

        ~ParticipantStore()
        {
            Dispose(false);
        }

        private bool _disposed;
    }
}