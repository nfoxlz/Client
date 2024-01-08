// ===================================================================
// Compete Management Information System
// ===================================================================
// 版权所有 © Compete software studio 2018 保留所有权利。
// -------------------------------------------------------------------
// 版本    日期时间            作者     说明
// -------------------------------------------------------------------
// 1.0.0.0 2018/3/15 8:40:24 LeeZheng 新建。
//==============================================================
using System;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace Compete.Mis.MisControls
{
    /// <summary>
    /// 多选框控件类。
    /// </summary>
    public sealed class MultichoiceBox : ChoiceBox
    {
        /// <summary>
        /// 创建单个项目。
        /// </summary>
        /// <returns>单个项目。</returns>
        protected override ToggleButton CreateItem() => new CheckBox();

        /// <summary>
        /// 项目的 IsChecked 属性改变时，调用该方法。
        /// </summary>
        /// <param name="value">项目的值。</param>
        /// <param name="isChecked">IsChecked 属性的值。</param>
        protected override void OnIsCheckedChanged(sbyte value, bool isChecked)
        {
            if (isChecked)
                Value |= Convert.ToInt64(Math.Pow(2D, value));
            else
                Value &= ~Convert.ToInt64(Math.Pow(2D, value));
        }

        /// <summary>
        /// 显示项目。
        /// </summary>
        /// <param name="item">项目控件。</param>
        /// <param name="itemValue">项目值。</param>
        protected override void ShowItem(ToggleButton item, sbyte itemValue)
        {
            var mask = Convert.ToInt64(Math.Pow(2D, itemValue));
            item.IsChecked = (Value & mask) == mask;
        }
    }
}
