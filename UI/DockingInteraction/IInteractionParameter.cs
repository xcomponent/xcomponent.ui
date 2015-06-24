using System.Collections.Generic;

namespace XComponent.Common.UI.DockingInteraction
{
    public interface IInteractionParameter
    {
        void LoadState(Dictionary<string, object> rawParameter);
        Dictionary<string, object> DumpState();
    }
}