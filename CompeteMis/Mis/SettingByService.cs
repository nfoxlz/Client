using System;
using System.Collections.Generic;
using Compete.Utils;

namespace Compete.Mis
{
    internal sealed class SettingByService : ISetting
    {
        private readonly IDictionary<string, string> globalSettings;// = new Dictionary<string, string>();

        public SettingByService() => globalSettings = Frame.Services.GlobalServices.FrameService.GetSettings();

        public object? GetSetting(SettingNames name, Type type) => globalSettings.TryGetValue(Enum.GetName(typeof(SettingNames), name)!, out string? value) ? Convert.ChangeType(value, type) : null;

        public T? GetSetting<T>(SettingNames name) => (T?)GetSetting(name, typeof(T));
    }
}
