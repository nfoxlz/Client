using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Navigation;

namespace Compete.Mis.Frame.ViewModels
{
    internal sealed partial class LoginViewModel : PageViewModel
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

#if DEBUG || DEBUG_JAVA
        public LoginViewModel()
        {
            // 调试时自动输入的租户、用户与口令。
            Tenant = "Test";    // Defualt
            User = "SuperMan";
            Password = "PASSWORD";
        }
#endif

        [RelayCommand(CanExecute = nameof(CanOk))]
        private void Ok()
        {
            var user = service.Authenticate(Tenant!, User!, Password!);
            if (user != null)
            {
                GC.Collect();
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

                if (mainWindow.CanGoForward)
                    mainWindow.GoForward();
                else
                    mainWindow.Navigate(new Views.MainPage());
            }
            else if (authenticationCount <= 0)
            {
                // 失败，且已达到验证最大次数。
                MisControls.MessageDialog.Error("LoginPage.UserOrPasswordErrorMessageExit");
                //var window = (sender as FrameworkElement).GetWindow();
                //if (window != null && !BrowserInteropHelper.IsBrowserHosted)
                //    window.Close();
                //Application.Current.MainWindow.Close();
                Application.Current.Shutdown();
            }
            else    // 失败，但未达到验证最大次数。
            {
                MisControls.MessageDialog.Warning("LoginPage.UserOrPasswordErrorMessage", authenticationCount);
                authenticationCount--;
            }
        }

        private bool CanOk() => !string.IsNullOrWhiteSpace(Tenant) && !string.IsNullOrWhiteSpace(User) && !string.IsNullOrWhiteSpace(Password);

        [RelayCommand]
        private static void Cancel() => Application.Current.Shutdown();

        public override void Refresh() => authenticationCount = 2;
    }
}
