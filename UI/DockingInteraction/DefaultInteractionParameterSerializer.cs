using System.Collections.Generic;

namespace XComponent.Common.UI.DockingInteraction
{
    public class DefaultInteractionParameterSerializer<TTypedParam> : IInteractionParameterSerializer<TTypedParam>
        where TTypedParam : class, new()
    {
        public TTypedParam Deserialize(Dictionary<string, object> parameter)
        {
            var typedParameter = new TTypedParam();
            ((IInteractionParameter)typedParameter).LoadState(parameter);
            return typedParameter;
        }

        public Dictionary<string, object> Serialize(TTypedParam typedParameter)
        {
            if (typedParameter == null)
            {
                return null;
            }

            return ((IInteractionParameter)typedParameter).DumpState();
        }
    }
}