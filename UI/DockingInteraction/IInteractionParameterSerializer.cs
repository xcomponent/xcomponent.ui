using System.Collections.Generic;

namespace XComponent.Common.UI.DockingInteraction
{
    public interface IInteractionParameterSerializer<TTypedParam>
    {
        TTypedParam Deserialize(Dictionary<string, object> parameter);
        Dictionary<string, object> Serialize(TTypedParam typedParameter);
    }
}