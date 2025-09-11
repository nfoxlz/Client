using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.IO.MemoryMappedFiles;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Markup;

namespace Compete.Mis
{
    internal static class Global
    {
        public static Frame.Services.WebApi.WebApiHelper ServiceHelper { get; }
            = new Frame.Services.WebApi.WebApiHelper(ConfigurationManager.AppSettings["WebApiBaseAddress"] ?? Constants.DefaultBaseAddress, Constants.SignPassword) { UseDataSignature = Convert.ToBoolean(ConfigurationManager.AppSettings["UseDataSignature"] ?? "True") };

        private static readonly string settingPath = ConfigurationManager.AppSettings["SettingsPath"] ?? Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "../settings");

        //private static readonly string updatePath = ConfigurationManager.AppSettings["UpdatePath"] ?? "/CompeteMIS";

        private static readonly Frame.Services.IAccountService service = DispatchProxy.Create<Frame.Services.IAccountService, Frame.Services.WebApi.WpfWebApiServiceProxy>();

        public static Utils.SFTPSetting SFTPSetting { get; private set; }

        static Global()
        {
            var sftpSettingPath = Path.Combine(settingPath, "sftp.json");
            SFTPSetting = File.Exists(sftpSettingPath) ? JsonSerializer.Deserialize<Utils.SFTPSetting>(File.ReadAllText(sftpSettingPath)) ?? Utils.SFTPSetting.Default : Utils.SFTPSetting.Default;
            //SFTPSetting = File.Exists(sftpSettingPath) ? JsonSerializer.Deserialize<Utils.SFTPSetting>(File.ReadAllText(sftpSettingPath)) ?? new Utils.SFTPSetting() : new Utils.SFTPSetting();
        }

        public static void Initialize()
        {
            // 预热
            Task.Run(() => Scripts.ScriptBuilder.GetType(Scripts.ScriptTemplates.CalculatorTemplate, string.Empty, "Compete.Scripts.Calculator", "CSharp"));

            //Frame.Services.GlobalServices.UpdateService.Update();

            //LoadLanguage(ConfigurationManager.AppSettings["Language"] ?? "zh-CN");  // 设置语言。
            LoadLanguage(ConfigurationManager.AppSettings["Language"] ?? CultureInfo.CurrentUICulture.IetfLanguageTag);

#if !DEBUG && !DEBUG_JAVA
            CheckUpdate();
#endif

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
#if !JAVA_LANGUAGE
            Utils.Cryptography.PublicKey = service.GetPublicKey();
#endif
            Plugins.PluginHelper.DefaultCommand = new Plugins.SettingPlugin();

            Application.Current.MainWindow.Visibility = Visibility.Visible;
        }

        //public static void Initialized() => Application.Current.MainWindow.Language = XmlLanguage.GetLanguage(ConfigurationManager.AppSettings["Language"] ?? CultureInfo.CurrentUICulture.IetfLanguageTag);

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

        private static void LoadLanguage(string language)
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
            }
            Application.Current.MainWindow.Language = XmlLanguage.GetLanguage(language);
        }

        private const string SharedMemoryFileName = "UpdateSharedMemory";

        private static void CheckUpdate()
        {
            var result = Frame.Services.GlobalServices.UpdateService.Chcek(SFTPSetting);
            if (result.Data != null && result.Data.Count() > 0)
            {
                if (MisControls.MessageDialog.Question("Message.FoundNewVersion", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    var bytes = Encoding.UTF8.GetBytes(string.Join('|', result.Data));

                    // 启动更新程序
                    //using (var memoryMappedFile = MemoryMappedFile.CreateOrOpen(SharedMemoryFileName, bytes.Length + 4))
                    var memoryMappedFile = MemoryMappedFile.CreateOrOpen(SharedMemoryFileName, bytes.Length + 4);
                    using (var accessor = memoryMappedFile.CreateViewAccessor())
                    {
                        accessor.Write(0, bytes.Length);
                        accessor.WriteArray(4, bytes, 0, bytes.Length);
                    }

                    Process.Start(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Updater.exe"), $"{SharedMemoryFileName}");

                    //var startInfo = new System.Diagnostics.ProcessStartInfo
                    //{
                    //    FileName = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Updater.exe"),
                    //    Arguments = $"{SharedMemoryFileName} {updatePath}",
                    //    UseShellExecute = true,
                    //    Verb = "runas"
                    //};
                    //var updateWindow = new UpdateWindow(fileList.Data);
                    //updateWindow.ShowDialog();

                    Application.Current.Shutdown();
                    Environment.Exit(0);
                }
            }
        }
    }
}
