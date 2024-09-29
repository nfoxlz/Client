using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Runtime.Caching;
using System.Runtime.InteropServices;
using System.Windows;
using Xceed.Wpf.AvalonDock.Layout;
using Xceed.Wpf.Toolkit;

namespace Compete.Mis
{
    public static class GlobalCommon
    {
        public const long ApplicationNo = 0L;

        public const long ClientSide = 0L;

        public static Models.Entity? CurrentTenant { get; set; }

        public static Models.Entity? CurrentUser { get; set; }

        public static MisControls.IEntityDataProvider? EntityDataProvider { get; set; }

        public static IDictionary<string, MisControls.TreeEntitySetting>? TreeEntitySettingDictionary { get; set; }

        public static IDictionary<int, string>? ErrorDictionary { get; set; }

        /// <summary>
        /// 获取或设置繁忙提示条。
        /// </summary>
        public static BusyIndicator? MainBusyIndicator { get; set; }

        /// <summary>
        /// 获取或设置主文档面板。
        /// </summary>
        public static LayoutDocumentPane? MainDocumentPane { get; set; }
        
        /// <summary>
        /// 取得消息。
        /// </summary>
        /// <param name="displayName">显示名。</param>
        /// <param name="arg">格式化参数。</param>
        /// <returns>屏幕消息。</returns>
        public static string GetMessage(string displayName, params object[] arg)
            => Application.Current.Resources.MergedDictionaries.Count > 0 ? string.Format((Application.Current.Resources.MergedDictionaries[0][displayName] ?? displayName).ToString()!, arg) : displayName;

        public static string GetMessageOrDefault(string displayName, string defaultMessage, params object[] arg)
            => Application.Current.Resources.MergedDictionaries.Count > 0 ? string.Format((Application.Current.Resources.MergedDictionaries[0][displayName] ?? defaultMessage).ToString()!, arg) : defaultMessage;

        /// <summary>
        /// 获取枚举字典。
        /// </summary>
        /// <remarks>
        /// 在 <see cref="Controls.DataPanel"/> 控件生成 <see cref="Controls.SinglechoiceBox"/> 与 <see cref="Controls.MultichoiceBox"/> 等控件时使用。
        /// </remarks>
        public static IDictionary<string, IDictionary<sbyte, string>> EnumDictionary
        {
            get
            {
                return (IDictionary<string, IDictionary<sbyte, string>>)MemoryCache.Default.Get(nameof(EnumDictionary));
            }
        }

        private const string defaultLoggerPath = "logs/client-{Date}.log";

        public const string LogMessage = "{Message}";

        public static ILoggerFactory CreateLoggerFactory(string path = defaultLoggerPath, LogLevel level = LogLevel.Information) =>
            LoggerFactory.Create(builder =>
                builder.AddFilter(string.Empty, level)
                        //.AddSimpleConsole(options =>
                        //{
                        //    options.IncludeScopes = true;
                        //    options.SingleLine = true;
                        //    //options.TimestampFormat = "g";
                        //    options.TimestampFormat = "yyyy/M/d ddd H:m:s.FFFFFF ";
                        //})
                        .AddFile(path, level));

        public static ILoggerFactory CreateLoggerFactory(LogLevel level) => CreateLoggerFactory(defaultLoggerPath, level);

        public static string PluginPath { get; } = ConfigurationManager.AppSettings["PluginPath"] ?? Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "../plugins");

        public static MemoryData.IServerDateTimeProvider? ServerDateTimeProvider { get; set; }

        public static Plugins.IDataProvider? DataProvider { get; set; }

        public static Utils.IConfiguration? GlobalConfiguration { get; set; }

        [DllImport("CompeteLib.dll", EntryPoint = "releaseString", CallingConvention = CallingConvention.Cdecl)]
        private static extern void ReleaseString(IntPtr str);

        public static string ConvertString(IntPtr str)
        {
            var result = Marshal.PtrToStringAnsi(str)!;
            ReleaseString(str);
            return result;
        }

        public static object? CreateSystemVariable(MemoryData.SystemVariables value)
        {
            DateTime now;

            switch (value)
            {
                case MemoryData.SystemVariables.NewGuid:
                    return Guid.NewGuid();
                case MemoryData.SystemVariables.CurrentDateTime:
                    return DateTime.Now;
                case MemoryData.SystemVariables.CurrentDate:
                    return DateTime.Now.Date;
                case MemoryData.SystemVariables.CurrentTime:
                    return DateTime.Now.TimeOfDay;
                case MemoryData.SystemVariables.CurrentYearMonth:
                    now = DateTime.Now;
                    return new DateTime(now.Year, now.Month, 1);
                case MemoryData.SystemVariables.CurrentYear:
                    return new DateTime(DateTime.Now.Year, 1, 1);
                case MemoryData.SystemVariables.CurrentMonth:
                    return DateTime.Now.Month;
                case MemoryData.SystemVariables.PreviousYearMonth:
                    now = DateTime.Now;
                    return new DateTime(now.AddMonths(-1).Year, now.AddMonths(-1).Month, 1);
                case MemoryData.SystemVariables.PreviousYear:
                    return new DateTime(DateTime.Now.AddYears(-1).Year, 1, 1);
                case MemoryData.SystemVariables.PreviousMonth:
                    return DateTime.Now.AddMonths(-1).Month;
                case MemoryData.SystemVariables.ServerDateTime:
                    return ServerDateTimeProvider!.GetServerDateTime();
                case MemoryData.SystemVariables.ServerDate:
                    return ServerDateTimeProvider!.GetServerDate();
                case MemoryData.SystemVariables.ServerTime:
                    return ServerDateTimeProvider!.GetServerTime();
                case MemoryData.SystemVariables.AccountingDate:
                    return ServerDateTimeProvider!.GetAccountingDate();
                case MemoryData.SystemVariables.CurrentApplication:
                    return ApplicationNo;
                case MemoryData.SystemVariables.CurrentClientSide:
                    return ClientSide;
                case MemoryData.SystemVariables.CurrentTenantId:
                    return CurrentTenant!.Id;
                case MemoryData.SystemVariables.CurrentTenantCode:
                    return CurrentTenant!.Code;
                case MemoryData.SystemVariables.CurrentTenantName:
                    return CurrentTenant!.Name;
                case MemoryData.SystemVariables.CurrentUserId:
                    return CurrentUser!.Id;
                case MemoryData.SystemVariables.CurrentUserCode:
                    return CurrentUser!.Code;
                case MemoryData.SystemVariables.CurrentUserName:
                    return CurrentUser!.Name;
                default:
                    return null;
            }
        }
    }
}
