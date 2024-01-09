using System.Windows.Controls;

namespace Compete.Mis.Frame.Views
{
    /// <summary>
    /// MainPage.xaml 的交互逻辑
    /// </summary>
    public partial class MainPage : Page
    {
        public MainPage()
        {
            InitializeComponent();

            GlobalCommon.MainBusyIndicator = DefaultBusyIndicator;
            Global.MainDocumentPane = MainDocumentPane;
        }
    }
}
