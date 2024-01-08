using Microsoft.Extensions.Logging;
using System.IO;
using System.Text.Json;
using System.Windows;
using System.Windows.Markup;
using Xceed.Wpf.AvalonDock.Layout;

namespace Compete.Mis.Plugins
{
    internal sealed class SettingPlugin : PluginBase
    {
        private static void AddDocument(object ui, string title)
        {
            var document = new LayoutDocument() { Content = ui, Title = title };
            GlobalCommon.MainDocumentPane!.Children.Add(document);
            GlobalCommon.MainDocumentPane.SelectedContentIndex = GlobalCommon.MainDocumentPane.Children.Count - 1;
        }

        protected override void Run(PluginCommandParameter? parameter)
        {
            using var factory = GlobalCommon.CreateLoggerFactory();
            var logger = factory.CreateLogger<SettingPlugin>();

            if (parameter == null)
            {
                logger.LogWarning("参数为空。");
                return;
            }

            var basePath = Path.Combine(GlobalCommon.PluginPath, parameter.Path);
            if (!Path.Exists(basePath))
            {
                logger.LogWarning("插件路径【{}】没有找到。", basePath);
                return;
            }

            var uiPath = Path.Combine(basePath, "ui.xaml");
            if (!File.Exists(uiPath))
            {
                logger.LogWarning("插件UI文件【{}】没有找到。", uiPath);
                return;
            }

            var path = Path.Combine(basePath, "setting.json");
            PluginSetting setting = File.Exists(path) ? JsonSerializer.Deserialize<PluginSetting>(File.ReadAllText(path))! : new PluginSetting();

            var ui = XamlReader.Parse(File.ReadAllText(uiPath));

            if (ui is FrameworkElement uiElement)
            {
                //uiElement.DataContext is PluginViewModel viewModel;

                var viewModel = uiElement.DataContext is PluginViewModel model ? model : uiElement.DataContext == null ? (SettingDataViewModel)(uiElement.DataContext = new SettingDataViewModel()) : null;
                if (viewModel != null)
                {
                    if (viewModel is SettingDataViewModel settingDataViewModel)
                    {
                        settingDataViewModel.PluginParameter ??= parameter;
                        settingDataViewModel.Setting ??= setting;
                    }

                    viewModel.Authorition &= parameter.Authorition;
                    uiElement.DataContext ??= viewModel;
                }
                //if (viewModel != null && viewModel.Data == null)
                //    viewModel.Data = GlobalCommon.DataProvider!.Query(basePath, setting.LoadName, setting.QueryParameters);
            }

            if (ui is FrameworkElement element && element?.DataContext is DataViewModel dataViewModel)
            {
                dataViewModel.View = ui;
                dataViewModel.Initialize();
            }

            switch (setting.UIType)
            {
                case PluginUIType.Window:
                case PluginUIType.Dialog:
                    Window window = (ui as Window) ?? new Window() { Title = parameter.Title, Content = ui, };
                    if (setting.UIType == PluginUIType.Window)
                        window.Show();
                    else
                        window.ShowDialog();
                    break;
                case PluginUIType.Document:
                    AddDocument(ui, parameter.Title);
                    break;
                default:
                    if (ui is Window windowUI)
                        windowUI.Show();
                    else
                        AddDocument(ui, parameter.Title);
                    break;
            }
        }
    }
}
