// ===================================================================
// Compete Management Information System
// ===================================================================
// 版权所有 © Compete software studio 2018 保留所有权利。
// -------------------------------------------------------------------
// 版本    日期时间            作者     说明
// -------------------------------------------------------------------
// 1.0.0.0 2018/2/28 14:02:24 LeeZheng  新建。
// ===================================================================
using System;
using System.Globalization;
using System.Windows.Controls;

namespace Compete.Mis.MisControls
{
    /// <summary>
    /// 非空规则类。
    /// </summary>
    public sealed class NonnullRule : ValidationRule
    {
        /// <summary>
        /// 获取或设置显示名。
        /// </summary>
        public required string DisplayName { get; set; }

        /// <summary>
        /// 对值执行验证检查。
        /// </summary>
        /// <param name="value">要检查的来自绑定目标的值。</param>
        /// <param name="cultureInfo">要在此规则中使用的区域性。</param>
        /// <returns>验证结果。</returns>
        public override ValidationResult Validate(object value, CultureInfo cultureInfo) =>
            value == null || value == DBNull.Value || (value is string && string.IsNullOrWhiteSpace(value as string)) || (value is Guid guid && guid == Guid.Empty)
                ? new ValidationResult(false, GlobalCommon.GetMessage("NonnullRuleError", DisplayName))
                : new ValidationResult(true, null);// || string.IsNullOrWhiteSpace(value.ToString())
    }
}
