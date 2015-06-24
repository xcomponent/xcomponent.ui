using System.Windows;

namespace XComponent.Common.UI.MessageBox
{
    public interface IMessageService
    {
        MessageBoxResult ShowMessage(string message, string title, MessageBoxButton messageBoxButton = MessageBoxButton.OK, MessageBoxImage image = MessageBoxImage.None);
    }
}
