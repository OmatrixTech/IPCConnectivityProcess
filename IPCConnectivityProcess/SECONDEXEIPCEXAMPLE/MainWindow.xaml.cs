using System.IO.Pipes;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using IPCCommonHelper;

namespace SECONDEXEIPCEXAMPLE
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        #region Fields declaration
        ClientServerCommonHelper ClientServerCommonHelper = ClientServerCommonHelper.Instance;
        StringBuilder sb;
        #endregion

        public MainWindow()
        {
            InitializeComponent();
            sb = new StringBuilder();
        }

        private void BtnClient_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (ClientServerCommonHelper != null) {
                    if (!String.IsNullOrEmpty(TxtBoxResult.Text))
                    {
                        ClientServerCommonHelper.TransmitDataToIPCServerExecutable(TxtBoxResult.Text);
                    }
                }
               
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}