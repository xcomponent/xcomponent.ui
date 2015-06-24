using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;

namespace XComponent.Common.UI.Command
{
    public interface IEventCommand<T> : ICommand
    {
        /// <summary>
        /// Get or set the event instance to be sent by the command.
        /// </summary>
        T EventInstance { get; set; }
    }
}
