using System.Windows;

namespace Compete.Mis.Developer
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            Global.Initialize();
        }
    }
}
