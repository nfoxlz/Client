using System.Windows;
using System.Windows.Navigation;

namespace Compete.Mis.Frame.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : NavigationWindow
    {
        public MainWindow()
        {
            Global.Initialize();
            InitializeComponent();
        }

        private void NavigationWindow_Navigating(object sender, NavigatingCancelEventArgs e)
        {
            if (Content is MainPage page && e.Content is LoginPage)
            {
                if (page.DataContext is ViewModels.MainViewModel mainViewModel && mainViewModel.CanGoBack)
                {
                    mainViewModel.CanGoBack = false;
                    WindowStyle = WindowStyle.None;
                    ResizeMode = ResizeMode.NoResize;
                    Width = 320D;
                    Height = 180D;
                    WindowState = WindowState.Normal;
                }
                else
                {
                    e.Cancel = true;
                    return;
                }
            }

            if (e.Content is FrameworkElement element && element.DataContext is ViewModels.PageViewModel viewModel)
                viewModel.Refresh();
        }
    }
}
