namespace Compete.Mis.Plugins
{
    internal sealed class SettingPlugin : CustomSettingPlugin<DataPluginSetting>
    {
        protected override SettingDataViewModel CreateViewModel() => new();
    }
}
