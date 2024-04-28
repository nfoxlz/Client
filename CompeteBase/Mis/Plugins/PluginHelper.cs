using System.IO;
using System.Windows.Input;
using System.Text.Json;

namespace Compete.Mis.Plugins
{
    public static class PluginHelper
    {
        //private static readonly Common.ObjectHelper objectHelper = new ();

        public static ICommand? DefaultCommand { get; set; }

        public static ICommand? CreateCommand(PluginCommandParameter parameter)
        {
            var path = Path.Combine(GlobalCommon.PluginPath, parameter.Path, "class.json");
            if (Path.Exists(path))
                return Common.ObjectHelper.Default.CreateInstance(JsonSerializer.Deserialize<Common.TypeSetting>(File.ReadAllText(path))) as ICommand;

            return DefaultCommand;
        }
    }
}
