using Compete.Mis.MisThreading;
using Microsoft.Extensions.Logging;
using System;
using System.Configuration;
using System.IO;
using System.Net.Http;
using System.Reflection;
using System.Threading.Tasks;

namespace Compete.Mis.Frame.Services.WebApi
{
    internal sealed class UpdateService// : IUpdateService
    {
        //private static readonly string fileUrl = ConfigurationManager.AppSettings["FileBaseAddress"] ?? Constants.DefaultFileAddress;

        //private static readonly string localFilePath = AppDomain.CurrentDomain.BaseDirectory;

        //private static readonly string pluginPath = ConfigurationManager.AppSettings["PluginsPath"] ?? Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "../plugins");

        //private static readonly string settingPath = ConfigurationManager.AppSettings["SettingsPath"] ?? Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "../settings");

        //private static async void DownLoad(string name)
        //{
        //    using HttpClient client = new();
        //    // 发送GET请求下载文件
        //    HttpResponseMessage response = await client.GetAsync(fileUrl + name, HttpCompletionOption.ResponseHeadersRead);

        //    // 确保请求成功
        //    var responseMessage = response.EnsureSuccessStatusCode();
        //    if (!responseMessage.IsSuccessStatusCode)
        //        throw new WebApiServiceException(responseMessage);

        //    //var resultString = await response.Content.ReadAsStringAsync();
        //    using Stream stream = await response.Content.ReadAsStreamAsync();
        //    using FileStream fileStream = new(Path.Combine(localFilePath, name), FileMode.Create, FileAccess.Write, FileShare.None);
        //    await stream.CopyToAsync(fileStream);
        //}

        //private static async Task<string> DownLoadText(string url)
        //{
        //    using HttpClient client = new();
        //    // 发送GET请求下载文件
        //    HttpResponseMessage response = await client.GetAsync(fileUrl + url, HttpCompletionOption.ResponseHeadersRead);

        //    // 确保请求成功
        //    var responseMessage = response.EnsureSuccessStatusCode();
        //    if (!responseMessage.IsSuccessStatusCode)
        //        throw new WebApiServiceException(responseMessage);

        //    return await response.Content.ReadAsStringAsync();
        //}

        //public async void Update()
        //{
        //    var lines = (await DownLoadText("assemblies/AssemblyList.csv")).Split("\r\n");

        //    foreach (var line in lines)
        //    {
        //        var items = line.Split(',');
        //        switch (items.Length)
        //        {
        //            case 2:
        //                if (Assembly.LoadFrom(Path.Combine(localFilePath, items[0])).GetName().Version < new Version(items[1]))
        //                    DownLoad(items[0]);
        //                break;
        //            case 3:
        //                break;
        //            default:
        //                using (var factory = GlobalCommon.CreateLoggerFactory())
        //                    factory.CreateLogger<ThreadingHelperBase>().LogError(GlobalCommon.LogMessage, $"Read [{line}] info error.");
        //                break;
        //        }
        //    }
        //    //try
        //    //{
        //    //    var list = await DownLoad("");
        //    //}
        //    //catch (Exception)
        //    //{

        //    //    return false;
        //    //}
        //    //return true;
        //}
    }
}
