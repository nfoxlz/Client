using System.Windows.Controls;

namespace Compete.Mis.Frame.Views
{
    /// <summary>
    /// LoginPage.xaml 的交互逻辑
    /// </summary>
    public partial class LoginPage : Page
    {
        public LoginPage()
        {
            InitializeComponent();

            GlobalCommon.MainBusyIndicator = DefaultBusyIndicator;
        }
    }
}
