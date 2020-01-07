namespace SpotifyWPF.Service.MessageBoxes
{    
    public enum MessageBoxButton
    {
        OK = 0,
        OkCancel = 1,
        YesNoCancel = 3,
        YesNo = 4
    }

    public enum MessageBoxResult
    {
        None = 0,
        Ok = 1,
        Cancel = 2,
        Yes = 6,
        No = 7
    }

    public enum MessageBoxIcon
    {
        None = 0,
        Error = 16,
        Hand = 16,
        Stop = 16,
        Question = 32,
        Exclamation = 48,
        Warning = 48,
        Information = 64,
        Asterisk = 64
    }

    public interface IMessageBoxService
    {
        MessageBoxResult ShowMessageBox(string message, string caption, MessageBoxButton buttons, MessageBoxIcon icon);
    }

    public class MessageBoxService : IMessageBoxService
    {
        public MessageBoxResult ShowMessageBox(string message, string caption, MessageBoxButton buttons, MessageBoxIcon icon)
        {
            return (MessageBoxResult)System.Windows.MessageBox.Show(message, caption,
                (System.Windows.MessageBoxButton)buttons, (System.Windows.MessageBoxImage)icon);
        }
    }
}
