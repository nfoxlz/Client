using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;
using System.IO;
using System.Windows;
using System.Windows.Markup;
using Xceed.Wpf.AvalonDock.Layout;

namespace Compete.Mis.Plugins
{
    internal abstract class UIPlugin : PluginBase
    {
        protected static void AddDocument(object ui, string title)
        {
            var document = new LayoutDocument() { Content = ui, Title = title };
            GlobalCommon.MainDocumentPane!.Children.Add(document);
            GlobalCommon.MainDocumentPane.SelectedContentIndex = GlobalCommon.MainDocumentPane.Children.Count - 1;
        }

        protected static void Show(object ui, PluginUIType uIType, string title)
        {
            switch (uIType)
            {
                case PluginUIType.Window:
                case PluginUIType.Dialog:
                    Window window = (ui as Window) ?? new Window()
                    {
                        Title = title,
                        Content = ui,
                        WindowStartupLocation = WindowStartupLocation.CenterScreen,
                    };
                    if (uIType == PluginUIType.Window)
                        window.Show();
                    else
                        window.ShowDialog();
                    break;
                case PluginUIType.Document:
                    AddDocument(ui, title);
                    break;
                default:
                    if (ui is Window windowUI)
                        windowUI.Show();
                    else
                        AddDocument(ui, title);
                    break;
            }
        }

        protected virtual void SetupUI(object ui, string basePath, PluginCommandParameter parameter) { }

        protected override void Run(PluginCommandParameter? parameter)
        {
            using var factory = GlobalCommon.CreateLoggerFactory();
            var logger = factory.CreateLogger<UIPlugin>();

            if (null == parameter)
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

            //var ui = XamlReader.Parse(File.ReadAllText(uiPath));
            SetupUI(XamlReader.Parse(File.ReadAllText(uiPath)), basePath, parameter);
        }
    }
}
