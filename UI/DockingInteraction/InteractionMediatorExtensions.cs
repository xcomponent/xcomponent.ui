using System.Collections.Generic;
using System.Linq;

namespace XComponent.Common.UI.DockingInteraction
{
    public static class InteractionMediatorExtensions
    {
        public static bool RunPeerToPeerInteraction<TParameter>(this IInteractionMediator mediator, IParticipantType target, string action, IEnumerable<TParameter> parameters, IInteractionParameterSerializer<TParameter> serializer = null)
            where TParameter : class, IInteractionParameter, new()
        {
            if (serializer == null)
            {
                serializer = new DefaultInteractionParameterSerializer<TParameter>();
            }
            IEnumerable<Dictionary<string, object>> rawParameters = parameters.Select(serializer.Serialize).ToList();

            IEnumerable<ParticipantIdentity> targetIdentities = mediator.GetParticipants(target);

            bool wasInteractionRun = false;
            foreach (ParticipantIdentity targetIdentity in targetIdentities)
            {
                var interaction = new Interaction(InteractionPattern.PeerToPeer, null, targetIdentity, rawParameters, action);
                bool wasCurrentInteractionRun = mediator.RunInteraction(interaction);
                if (wasCurrentInteractionRun)
                {
                    wasInteractionRun = true;
                }
            }

            return wasInteractionRun;
        }

        public static bool RunEnslaveInteraction<TParameter>(this IInteractionMediator mediator, ParticipantIdentity source, ParticipantIdentity target, string action, IEnumerable<TParameter> parameters, IInteractionParameterSerializer<TParameter> serializer = null)
            where TParameter : class, IInteractionParameter, new()
        {
            if (serializer == null)
            {
                serializer = new DefaultInteractionParameterSerializer<TParameter>();
            }
            IEnumerable<Dictionary<string, object>> rawParameters = parameters.Select(serializer.Serialize).ToList();

            var interaction = new Interaction(InteractionPattern.Enslave, source, target, rawParameters, action);
            return mediator.RunInteraction(interaction);
        }

        public static bool RunMasterSlaveInteraction(this IInteractionMediator mediator, ParticipantIdentity source, IParticipantType targetType, string action, IEnumerable<Dictionary<string, object>> rawParameters)
        {
            var targetIdentity = new ParticipantIdentity(targetType, null);
            var interaction = new Interaction(InteractionPattern.MasterSlave, source, targetIdentity, rawParameters, action);
            return mediator.RunInteraction(interaction);
        }

        public static bool RunSelectedParticipantInteraction<TParameter>(this IInteractionMediator mediator, string action, IEnumerable<TParameter> parameters, IInteractionParameterSerializer<TParameter> serializer = null)
            where TParameter : class, IInteractionParameter, new()        
        {
            if (serializer == null)
            {
                serializer = new DefaultInteractionParameterSerializer<TParameter>();
            }
            IEnumerable<Dictionary<string, object>> rawParameters = parameters.Select(serializer.Serialize).ToList();

            var interaction = new Interaction(InteractionPattern.SelectedParticipant, null, null, rawParameters, action);
            return mediator.RunInteraction(interaction);
        }

        public static bool RunBroadcastInteraction<TParameter>(this IInteractionMediator mediator, ParticipantIdentity source, string action, IEnumerable<TParameter> parameters, IInteractionParameterSerializer<TParameter> serializer = null)
            where TParameter : class, IInteractionParameter, new()        
        {
            if (serializer == null)
            {
                serializer = new DefaultInteractionParameterSerializer<TParameter>();
            }

            IEnumerable<Dictionary<string, object>> rawParameters = parameters.Select(serializer.Serialize).ToList();

            var interaction = new Interaction(InteractionPattern.Broadcast, source, null, rawParameters, action);
            return mediator.RunInteraction(interaction);
        }
    }
}
