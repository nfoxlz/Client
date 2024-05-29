using Compete.Utils;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Markup;

namespace Compete.Mis
{
    internal static class Global
    {
        public static Frame.Services.WebApi.WebApiHelper ServiceHelper { get; }
            = new Frame.Services.WebApi.WebApiHelper(ConfigurationManager.AppSettings["WebApiBaseAddress"] ?? Constants.DefaultBaseAddress, Constants.SignPassword);

        private static readonly string settingsPath = ConfigurationManager.AppSettings["SettingsPath"] ?? Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "../settings");

        public static void Initialize()
        {
            // 预热
            Task.Run(() =>
            {
                Scripts.ScriptBuilder.GetType(Scripts.ScriptTemplates.CalculatorTemplate, string.Empty, "Compete.Scripts.Calculator", "CSharp");
            });

            Common.ObjectHelper.AddAssembly(typeof(Global));

            Application.Current.MainWindow.Language = XmlLanguage.GetLanguage(CultureInfo.CurrentUICulture.IetfLanguageTag);

            var path = Path.Combine(settingsPath, "columns.json");
            if (File.Exists(path))
                MemoryData.DataCreator.GlobalDataColumnSettings = JsonSerializer.Deserialize<IEnumerable<MemoryData.DataColumnSetting>>(File.ReadAllText(path));
            path = Path.Combine(settingsPath, "billType.json");
            if (File.Exists(path))
                MemoryData.DataCreator.BillTypeNameSettings = JsonSerializer.Deserialize<IDictionary<ushort, string>>(File.ReadAllText(path));
            path = Path.Combine(settingsPath, "errors.json");
            if (File.Exists(path))
                GlobalCommon.ErrorDictionary = JsonSerializer.Deserialize<IDictionary<int, string>>(File.ReadAllText(path));
            path = Path.Combine(settingsPath, "treeEntity.json");
            GlobalCommon.TreeEntitySettingDictionary = File.Exists(path) ? JsonSerializer.Deserialize<IDictionary<string, MisControls.TreeEntitySetting>>(File.ReadAllText(path)) : new Dictionary<string, MisControls.TreeEntitySetting>();
            GlobalCommon.ServerDateTimeProvider = new Provider.DateTimeProvider();
            GlobalCommon.DataProvider = new Provider.DataProvider();
            GlobalCommon.EntityDataProvider = new Provider.EntityDataProvider();

            Plugins.PluginHelper.DefaultCommand = new Plugins.SettingPlugin();

        }

        private static readonly Enums.IEumnDataProvider provider = new Provider.EumnDataProvider();

        public static void LoginedInitialize()
        {
            Enums.EnumHelper.Initialize(provider);
            GlobalCommon.GlobalConfiguration = new ConfigurationByService();

            if (!GlobalCommon.TreeEntitySettingDictionary!.ContainsKey(Constants.EntityAccount))
                GlobalCommon.TreeEntitySettingDictionary.Add(Constants.EntityAccount,
                    new MisControls.TreeEntitySetting
                    {
                        DisplayName = "科目",
                        LevelLength = GlobalCommon.GlobalConfiguration!.GetConfig<string>(ConfigurationNames.AccountStructure),
                    });
        }
    }
}
