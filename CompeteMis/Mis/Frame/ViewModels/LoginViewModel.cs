using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Navigation;

namespace Compete.Mis.Frame.ViewModels
{
    internal sealed partial class LoginViewModel : ObservableObject
    {
        private static readonly Services.IAccountService service = DispatchProxy.Create<Services.IAccountService, Services.WebApi.WpfWebApiServiceProxy>();

        /// <summary>
        /// 已校验次数。
        /// </summary>
        private byte authenticationCount;

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(OkCommand))]
        private string? _tenant;

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(OkCommand))]
        private string? _user;

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(OkCommand))]
        private string? _password;

        public LoginViewModel()
        {
#if DEBUG
            // 调试时自动输入的租户、用户与口令。
            Tenant = "Test";    // Defualt
            User = "SuperMan";
            Password = "PASSWORD";
#endif
        }


        [RelayCommand(CanExecute = nameof(CanOk))]
        private void Ok()
        {
            var user = service.Authenticate(Tenant!, User!, Password!);
            if (user != null)
            {
                Tenant = string.Empty;
                User = string.Empty;
                Password = string.Empty;

                GlobalCommon.CurrentTenant = user.Tenant;
                GlobalCommon.CurrentUser = user;

                Task.Run(Global.LoginedInitialize);

                var mainWindow = (NavigationWindow)Application.Current.MainWindow;
                mainWindow.WindowStyle = WindowStyle.SingleBorderWindow;
                mainWindow.ResizeMode = ResizeMode.CanResize;
                mainWindow.WindowState = WindowState.Maximized;
                mainWindow.Navigate(new Views.MainPage());
            }
            else if (authenticationCount > 1)
            {
                // 失败，且已达到验证最大次数。
                MisControls.MessageDialog.Error("LoginPage.UserOrPasswordErrorMessageExit");
                //var window = (sender as FrameworkElement).GetWindow();
                //if (window != null && !BrowserInteropHelper.IsBrowserHosted)
                //    window.Close();
                Application.Current.MainWindow.Close();
            }
            else
            {
                // 失败，但未达到验证最大次数。
                authenticationCount++;
                //Controls.MessageDialog.Warning("LoginPage.UserOrPasswordErrorMessage");
            }
        }

        private bool CanOk() => !string.IsNullOrWhiteSpace(Tenant) && !string.IsNullOrWhiteSpace(User) && !string.IsNullOrWhiteSpace(Password);

        [RelayCommand]
        private static void Cancel() => Application.Current.Shutdown();
    }
}
