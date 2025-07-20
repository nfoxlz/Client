using Microsoft.CodeAnalysis.CSharp.Syntax;
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

        private static readonly string settingPath = ConfigurationManager.AppSettings["SettingsPath"] ?? Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "../settings");

        public static void Initialize()
        {
            // 预热
            Task.Run(() => Scripts.ScriptBuilder.GetType(Scripts.ScriptTemplates.CalculatorTemplate, string.Empty, "Compete.Scripts.Calculator", "CSharp"));

            //Frame.Services.GlobalServices.UpdateService.Update();

            //LoadLanguage(ConfigurationManager.AppSettings["Language"] ?? "zh-CN");  // 设置语言。
            LoadLanguage(ConfigurationManager.AppSettings["Language"] ?? CultureInfo.CurrentUICulture.IetfLanguageTag);

            Common.ObjectHelper.AddAssembly(typeof(Global));

            //Application.Current.MainWindow.Language = XmlLanguage.GetLanguage(CultureInfo.CurrentUICulture.IetfLanguageTag);

            var path = Path.Combine(settingPath, "columns.json");
            if (File.Exists(path))
                MemoryData.DataCreator.GlobalDataColumnSettings = JsonSerializer.Deserialize<IEnumerable<MemoryData.DataColumnSetting>>(File.ReadAllText(path));
            path = Path.Combine(settingPath, "billType.json");
            if (File.Exists(path))
                MemoryData.DataCreator.BillTypeNameSettings = JsonSerializer.Deserialize<IDictionary<ushort, string>>(File.ReadAllText(path));
            path = Path.Combine(settingPath, "errors.json");
            if (File.Exists(path))
                GlobalCommon.ErrorDictionary = JsonSerializer.Deserialize<IDictionary<int, string>>(File.ReadAllText(path));
            path = Path.Combine(settingPath, "treeEntity.json");
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
            GlobalCommon.GlobalConfiguration = new SettingByService();

            if (!GlobalCommon.TreeEntitySettingDictionary!.ContainsKey(Constants.EntityAccount))
                GlobalCommon.TreeEntitySettingDictionary.Add(Constants.EntityAccount,
                    new MisControls.TreeEntitySetting
                    {
                        DisplayName = "科目",
                        LevelLength = GlobalCommon.GlobalConfiguration!.GetSetting<string>(Utils.SettingNames.AccountStructure),
                        LevelPath = "Account_Code",
                    });
        }

        public static readonly Compete.Runtime.Caching.Cache<string, object?> GlobalCache
            = new("Global", key =>
            key switch
            {
                "IsFinanceClosed" => Frame.Services.GlobalServices.FrameService.IsFinanceClosed(),
                _ => null,
            });

        public static void LoadLanguage(string language)
        {
            var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Languages", Path.ChangeExtension(language, ".xaml"));
            if (File.Exists(path))
            {
                using (var stream = File.OpenRead(path))
                    Application.Current.Resources.MergedDictionaries.Add(XamlReader.Load(stream) as ResourceDictionary);
                //{
                //    var resourceDictionary = XamlReader.Load(stream) as ResourceDictionary;
                //    //Application.Current.Resources.MergedDictionaries.Remove(resourceDictionary);
                //    Application.Current.Resources.MergedDictionaries.Add(resourceDictionary);
                //}

                Application.Current.Dispatcher.Thread.CurrentUICulture
                    = Application.Current.Dispatcher.Thread.CurrentCulture
                    = CultureInfo.GetCultureInfo(language);

                Application.Current.MainWindow.Language = XmlLanguage.GetLanguage(language);
            }
        }
    }
}
