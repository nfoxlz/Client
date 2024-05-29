using Compete.Extensions;
using System.IO;
using System.Text.Json;
using System.Windows;

namespace Compete.Mis.Plugins
{
    internal abstract class CustomSettingPlugin<T> : UIPlugin where T : UIPluginSetting, new()
    {
        protected abstract CustomSettingDataViewModel<T> CreateViewModel();

        protected override void SetupUI(object ui, string basePath, PluginCommandParameter parameter)
        {
            base.SetupUI(ui, basePath, parameter);

            var path = Path.Combine(basePath, "setting.json");
            T setting = File.Exists(path) ? JsonSerializer.Deserialize<T>(File.ReadAllText(path))! : new T();

            if (ui is FrameworkElement uiElement)
            {
                //uiElement.DataContext is PluginViewModel viewModel;

                var viewModel = uiElement.DataContext is PluginViewModel model ? model : null == uiElement.DataContext ? (CustomSettingDataViewModel<T>)(uiElement.DataContext = CreateViewModel()) : null;
                if (viewModel != null)
                {
                    viewModel.PluginParameter ??= parameter;
                    viewModel.Authorition &= parameter.Authorition;
                    viewModel.TrySetPropertyValue("SettingFileName", path);
                    uiElement.DataContext ??= viewModel;
                }
                //if (null != viewModel && null == viewModel.Data)
                //    viewModel.Data = GlobalCommon.DataProvider!.Query(basePath, setting.LoadName, setting.QueryParameters);
            }

            if (ui is FrameworkElement element && element?.DataContext is DataViewModel dataViewModel)
            {
                dataViewModel.View = ui;
                dataViewModel.Initialize();
            }

            Show(ui, setting.UIType, parameter.Title);
        }
    }
}
