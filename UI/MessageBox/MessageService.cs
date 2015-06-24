using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace XComponent.Common.UI.MessageBox
{
    public class MessageService : IMessageService
    {
        public static IMessageService Current { get; internal set; }

        static MessageService()
        {
            Current = new MessageService();
        }

        private MessageService()
        {            
        }

        public MessageBoxResult ShowMessage(string message, string title, MessageBoxButton messageBoxButton = MessageBoxButton.OK, MessageBoxImage image = MessageBoxImage.None)
        {
            return XCMessageBox.ShowMessage(message, title, messageBoxButton, image);
        }
    }
}
