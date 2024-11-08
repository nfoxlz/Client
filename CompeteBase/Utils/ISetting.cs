using System;

namespace Compete.Utils
{
    public interface ISetting
    {
        object? GetSetting(SettingNames name, Type type);

        T? GetSetting<T>(SettingNames name);
    }
}
