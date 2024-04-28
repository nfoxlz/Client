// ======================================================
// Compete Management Information System
// ======================================================
// 版权所有 © Compete software studio 2018 保留所有权利。
// ------------------------------------------------------
// 版本    日期时间               作者     说明
// ------------------------------------------------------
// 1.0.0.0 2018/8/18 周六 20:38:13 LeeZheng 新建。
// ======================================================

namespace Compete.Mis.Developer.Models
{
    /// <summary>
    /// 数据库连接设置类。
    /// </summary>
    internal class DatabaseConnectionSetting
    {
        //public string? AssemblyName { get; set; }

        //public string? ConnectionFactory { get; set; }

        public string? ProviderName { get; set; } = "Npgsql";

        public string? ConnectionString { get; set; } = "Host=localhost;Database=business;Username=postgres;Password=postgres;";

    }
}
