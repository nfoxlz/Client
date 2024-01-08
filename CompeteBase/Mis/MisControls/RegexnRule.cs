// ===================================================================
// Compete Management Information System
// ===================================================================
// 版权所有 © Compete software studio 2018 保留所有权利。
// -------------------------------------------------------------------
// 版本    日期时间            作者     说明
// -------------------------------------------------------------------
// 1.0.0.0 2018/2/28 14:07:03 LeeZheng  新建。
// ===================================================================
using System.Globalization;
using System.Text.RegularExpressions;
using System.Windows.Controls;

namespace Compete.Mis.MisControls
{
    /// <summary>
    /// 正则表达式规则类。
    /// </summary>
    public sealed class RegexnRule : ValidationRule
    {
        private string? _regularExpression;

        /// <summary>
        /// 获取或设置正则表达式。
        /// </summary>
        public string? RegularExpression
        {
            get => _regularExpression;
            set
            {
                _regularExpression = value;
                regex = new Regex(_regularExpression!);
            }
        }

        /// <summary>
        /// 校验用的正则表达式。
        /// </summary>
        private Regex? regex;

        /// <summary>
        /// 获取或设置错误文本。
        /// </summary>
        public required string ErrorText { get; set; }

        /// <summary>
        /// 对值执行验证检查。
        /// </summary>
        /// <param name="value">要检查的来自绑定目标的值。</param>
        /// <param name="cultureInfo">要在此规则中使用的区域性。</param>
        /// <returns>验证结果。</returns>
        public override ValidationResult Validate(object value, CultureInfo cultureInfo) =>
            regex!.IsMatch((value ?? string.Empty).ToString()!) ? new ValidationResult(true, null) : new ValidationResult(false, ErrorText);
    }
}
