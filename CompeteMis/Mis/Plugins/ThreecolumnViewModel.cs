namespace Compete.Mis.Plugins
{
    internal sealed class ThreecolumnViewModel : ThreecolumnViewModel<ThreecolumnPluginSetting> { }

    internal abstract class ThreecolumnViewModel<T> : SettingDataViewModel<T> where T : ThreecolumnPluginSetting
    {
    }
}
