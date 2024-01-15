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

                var viewModel = uiElement.DataContext is PluginViewModel model ? model : uiElement.DataContext == null ? (CustomSettingDataViewModel<T>)(uiElement.DataContext = CreateViewModel()) : null;
                if (viewModel != null)
                {
                    viewModel.PluginParameter ??= parameter;
                    viewModel.PluginSetting ??= setting;
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

            Show(ui, setting.UIType, parameter.Title);
        }
    }
}
