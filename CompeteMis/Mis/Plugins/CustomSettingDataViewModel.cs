using CommunityToolkit.Mvvm.ComponentModel;
using System.IO;
using System.Text.Json;

namespace Compete.Mis.Plugins
{
    public abstract partial class CustomSettingDataViewModel<T> : DataViewModel where T : class, new()
    {
        [ObservableProperty]
        private T? _setting;

        [ObservableProperty]
        private string? _settingFileName;

        partial void OnSettingFileNameChanged(string? value) => Setting = !string.IsNullOrWhiteSpace(value) && File.Exists(value) ? JsonSerializer.Deserialize<T>(File.ReadAllText(value)) : new();
    }
}
