using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Markup;
using Xceed.Wpf.AvalonDock.Layout;

namespace Compete.Mis
{
    internal static class Global
    {
        /// <summary>
        /// 获取或设置主文档面板。
        /// </summary>
        public static LayoutDocumentPane? MainDocumentPane { get; set; }

        public static Frame.Services.WebApi.WebApiHelper ServiceHelper { get; }
            = new Frame.Services.WebApi.WebApiHelper(ConfigurationManager.AppSettings["WebApiBaseAddress"] ?? Constants.DefaultBaseAddress, Constants.SignPassword);

        public static void Initialize()
        {
            // 预热
            Task.Run(() =>
            {
                Scripts.ScriptBuilder.GetType(Scripts.ScriptTemplates.CalculatorTemplate, string.Empty, "Compete.Scripts.Calculator", "CSharp");
            });

            Application.Current.MainWindow.Language = XmlLanguage.GetLanguage(CultureInfo.CurrentUICulture.IetfLanguageTag);

            var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "../settings/columns.json");
            if (File.Exists(path))
                MemoryData.DataCreator.GlobalDataColumnSettings = JsonSerializer.Deserialize<IEnumerable<MemoryData.DataColumnSetting>>(File.ReadAllText(path));
            path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "../settings/billType.json");
            if (File.Exists(path))
                MemoryData.DataCreator.BillTypeNameSettings = JsonSerializer.Deserialize<IDictionary<ushort, string>>(File.ReadAllText(path));
            path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "../settings/errors.json");
            if (File.Exists(path))
                GlobalCommon.ErrorDictionary = JsonSerializer.Deserialize<IDictionary<int, string>>(File.ReadAllText(path));
            GlobalCommon.ServerDateTimeProvider = new Provider.DateTimeProvider();
            GlobalCommon.DataProvider = new Provider.DataProvider();
            GlobalCommon.EntityDataProvider = new Provider.EntityDataProvider();

            Plugins.PluginHelper.DefaultCommand = new Plugins.SettingPlugin();
            //Enums.EnumHelper.Initialize(new Provider.EumnDataProvider());
        }

        public static void LoginedInitialize() => Enums.EnumHelper.Initialize(new Provider.EumnDataProvider());
    }
}
