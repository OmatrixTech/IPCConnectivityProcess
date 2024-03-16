using IPCCommonHelper;
using System.IO;
using System.IO.Pipes;
using System.Text;
using System.Windows;

namespace IPCServerExample
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        #region Fields declaration
        ClientServerCommonHelper ClientServerCommonHelper = ClientServerCommonHelper.Instance;
        #endregion

        StringBuilder sb; 
        public MainWindow()
        {
            InitializeComponent();
            sb = new StringBuilder();
            ClientServerCommonHelper.StartServer();
            ClientServerCommonHelper.TriggerReceivedMessage += ClientServerCommonHelper_TriggerReceivedMessage;
        }

        private void ClientServerCommonHelper_TriggerReceivedMessage(object? sender, MessageReceived e)
        {
            if (e != null)
            {
                Application.Current.Dispatcher.BeginInvoke(new Action(() =>
                {
                    TxtBlckResult.Text =sb.Append("Message Received from :" +e.ReceivedData).ToString();
                }));
            }
        }
    }
}