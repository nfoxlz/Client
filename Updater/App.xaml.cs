using System;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Windows;
using System.Windows.Markup;

namespace Compete.Updater
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Languages", Path.ChangeExtension(ConfigurationManager.AppSettings["Language"] ?? CultureInfo.CurrentUICulture.IetfLanguageTag, ".xaml"));
            if (File.Exists(path))
                using (var stream = File.OpenRead(path))
                    Current.Resources.MergedDictionaries.Add(XamlReader.Load(stream) as ResourceDictionary);
        }
    }
}
