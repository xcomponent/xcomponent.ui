using System.Collections.Generic;
using System.Linq;

namespace XComponent.Common.UI.DockingInteraction
{
    public class Interaction
    {
        public Interaction(InteractionPattern interactionPattern, ParticipantIdentity source, ParticipantIdentity target, IEnumerable<Dictionary<string, object>> parameters) 
            : this(interactionPattern, source, target, parameters, null)
        {
        }

        public Interaction(InteractionPattern interactionPattern, ParticipantIdentity source, ParticipantIdentity target, IEnumerable<Dictionary<string, object>> parameters, string action)
        {
            InteractionPattern = interactionPattern;
            Source = source;
            Target = target;
            Parameters = parameters.ToArray();
            Action = action;
        }

        public InteractionPattern InteractionPattern { get; private set; }

        public ParticipantIdentity Source { get; private set; }
        
        public ParticipantIdentity Target { get; private set; }

        public string Action { get; set; }

        public IEnumerable<Dictionary<string, object>> Parameters { get; private set; }

    }
}