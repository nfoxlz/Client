namespace Compete.Mis.Plugins
{
    internal sealed class ThreecolumnPlugin : CustomSettingPlugin<ThreecolumnPluginSetting>
    {
        protected override ThreecolumnViewModel CreateViewModel() => new();
    }
}
