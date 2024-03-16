using IPCCommonHelper;
using System.Windows;

namespace IPCClientExample
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
       
        #region Fields declaration
        ClientServerCommonHelper ClientServerCommonHelper = ClientServerCommonHelper.Instance;
        #endregion
        public MainWindow()
        {
            InitializeComponent();
        }

        private void BtnClient_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(TxtBoxResult.Text))
            {
                ClientServerCommonHelper.TransmitDataToIPCServerExecutable(TxtBoxResult.Text);
            }
        }
    }
}