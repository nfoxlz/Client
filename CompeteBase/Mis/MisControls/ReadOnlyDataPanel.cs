// ======================================================
// XXX项目
// ======================================================
// 版权所有 © Compete software studio 2019 保留所有权利。
// ------------------------------------------------------
// 版本    日期时间            作者     说明
// ------------------------------------------------------
// 1.0.0.0 2019/4/19 周五 9:24:15 LeeZheng 新建。
// ======================================================
using Compete.Extensions;
using System.Data;
using System.Windows;
using System.Windows.Controls;

namespace Compete.Mis.MisControls
{
    /// <summary>
    /// ReadOnlyDataPanel 类。
    /// </summary>
    public sealed class ReadOnlyDataPanel : DataPanelBase
    {
        protected override FrameworkElement CreateElement(DataColumn column, TextBlock titleControl)
        {
            var binding = GetBinding(column);

            FrameworkElement result = new TextBox() { IsReadOnly = true };
            if (column.DataType.IsNumeric())
                ((TextBox)result).TextAlignment = TextAlignment.Right;
            result.SetBinding(TextBox.TextProperty, binding);

            return result;
        }
    }
}
