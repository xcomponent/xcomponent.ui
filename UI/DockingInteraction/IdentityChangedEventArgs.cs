using System;

namespace XComponent.Common.UI.DockingInteraction
{
    public class IdentityChangedEventArgs : EventArgs
    {
        public IdentityChangedEventArgs(string oldName, string newName)
        {
            OldName = oldName;
            NewName = newName;
        }

        public string OldName { get; private set; }
        public string NewName { get; private set; }
    }
}
