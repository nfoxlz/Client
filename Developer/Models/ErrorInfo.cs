// ======================================================
// Compete Management Information System
// ======================================================
// 版权所有 © Compete software studio 2018 保留所有权利。
// ------------------------------------------------------
// 版本    日期时间               作者     说明
// ------------------------------------------------------
// 1.0.0.0 2018/9/2 周日 11:36:44 LeeZheng 新建。
// ======================================================

namespace Compete.Mis.Developer.Models
{
    /// <summary>
    /// ErrorInfo 类。
    /// </summary>
    internal sealed class ErrorInfo
    {
        public ErrorLevel Level { get; set; }

        public required string Location { get; set; }

        public required string Message { get; set; }
    }
}
