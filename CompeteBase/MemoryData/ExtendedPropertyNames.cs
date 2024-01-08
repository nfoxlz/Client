// ===================================================================
// Compete Management Information System
// ===================================================================
// 版权所有 © Compete software studio 2018 保留所有权利。
// -------------------------------------------------------------------
// 版本    日期时间            作者     说明
// -------------------------------------------------------------------
// 1.0.0.0 2018/2/28 10:46:26 LeeZheng  新建。
// ===================================================================

namespace Compete.MemoryData
{
    /// <summary>
    /// ExtendedPropertyNames 类。
    /// </summary>
    public enum ExtendedPropertyNames
    {
        /// <summary>
        /// 必须，<see cref="bool"/> 型。
        /// </summary>
        IsRequired,

        /// <summary>
        /// 数据库类型，<see cref="System.Data.DbType"/> 型。
        /// </summary>
        DataType,

        /// <summary>
        /// 只读，<see cref="bool"/> 型。
        /// </summary>
        IsReadOnly,

        /// <summary>
        /// 可视，<see cref="bool"/> 型。
        /// </summary>
        IsVisible,

        /// <summary>
        /// 格式，<see cref="string"/> 型。
        /// </summary>
        Format,

        /// <summary>
        /// 展示格式，<see cref="string"/> 型。
        /// </summary>
        ShowFormat,

        /// <summary>
        /// 显示顺序，<see cref="int"/> 型。
        /// </summary>
        DisplayIndex,

        /// <summary>
        /// 默认系统值，<see cref="SystemVariables"/> 型。
        /// </summary>
        DefaultSystemValue,

        TargetNullValue,

        /// <summary>
        /// 最大值。
        /// </summary>
        Maximum,

        /// <summary>
        /// 最小值。
        /// </summary>
        Minimum,

        /// <summary>
        /// 长度。
        /// </summary>
        Length,

        /// <summary>
        /// 精度。
        /// </summary>
        Precision,

        /// <summary>
        /// 正则表达式，<see cref="string"/> 型。
        /// </summary>
        Regex,

        /// <summary>
        /// 正则表达式校验错误的提示文本，<see cref="string"/> 型。
        /// </summary>
        ErrorText,

        /// <summary>
        /// 控件，<see cref="Mis.MisControls.DataControlType"/> 型。
        /// </summary>
        Control,

        /// <summary>
        /// 控件参数，<see cref="string"/> 型。
        /// </summary>
        Parameters,
    }
}
