namespace Compete.Mis.Plugins
{
    public abstract class CustomSettingDataViewModel<T> : DataViewModel where T : class
    {
        public PluginCommandParameter? PluginParameter { get; set; }

        public T? Setting { get; set; }
    }
}
