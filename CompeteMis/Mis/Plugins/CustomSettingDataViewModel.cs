using Compete.Extensions;

namespace Compete.Mis.Plugins
{
    public abstract class CustomSettingDataViewModel<T> : DataViewModel where T : class, new()
    {
        protected override void NewSetting(object? setting)
        {
            base.NewSetting(setting);

            //Setting = setting as T;
            if (setting != null)
            {
                Setting = new();
                setting.DynamicMapTo(Setting);
            }
        }

        public T? Setting { get; set; }
    }
}
