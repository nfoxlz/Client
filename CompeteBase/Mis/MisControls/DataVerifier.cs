// ===================================================================
// Compete Management Information System
// ===================================================================
// 版权所有 © Compete software studio 2018 保留所有权利。
// -------------------------------------------------------------------
// 版本    日期时间            作者     说明
// -------------------------------------------------------------------
// 1.0.0.0 2018/2/28 14:36:09 LeeZheng  新建。
// ===================================================================
using System;
using System.Data;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace Compete.Mis.MisControls
{
    /// <summary>
    /// 数据校验器类。
    /// </summary>
    public static class DataVerifier
    {
        /// <summary>
        /// 校验。不通过时，显示错误消息框。
        /// </summary>
        /// <param name="element">要校验的元素。</param>
        /// <returns>true为校验不通过，false为校验通过。</returns>
        public static bool Verify(this DependencyObject element)
        {
            var result = element.Verify(out string errorText);

            if (result)
                MessageDialog.Error(errorText);

            return result;
        }

        /// <summary>
        /// 校验。
        /// </summary>
        /// <param name="element">要校验的元素。</param>
        /// <param name="errorText">输出的错误文本。</param>
        /// <returns>true为校验不通过，false为校验通过。</returns>
        public static bool Verify(this DependencyObject element, out string errorText)
        {
            errorText = string.Empty;

            if (null == element)
                return false;

            var result = false;
            var errors = Validation.GetErrors(element);     // 取得元素自身校验器中的错误。
            if (errors.Count > 0)
            {
                var builder = new StringBuilder();
                foreach (ValidationError error in errors)
                {
                    builder.Append(error.ErrorContent.ToString());
                    builder.Append('\t');
                }


                errorText = builder.ToString();

                if (!result && element is UIElement uiElement)
                    uiElement.Focus();

                result = true;
            }


            if (element is ContentControl contentControl
                && contentControl.Content is DependencyObject content
                && content.Verify(out string errorPrompt))   // 校验 ContentControl 对象。
            {
                errorText += errorPrompt;
                result = true;
            }

            if (element is Panel panel)                                                                       // 校验 Panel 对象。
                foreach (UIElement child in panel.Children)
                    if (child.Verify(out errorPrompt))
                    {
                        errorText += errorPrompt;
                        return true;
                    }

            if (element is ItemsControl itemControl)                                                                // 校验 ItemsControl 对象。
                foreach (object item in itemControl.Items)
                {
                    if (item is not DependencyObject)
                        continue;

                    if (((DependencyObject)item).Verify(out errorPrompt))
                    {
                        errorText += errorPrompt;
                        result = true;
                    }
                }

            //if (element is Page && (element as Page).Content is DependencyObject && VerifyUI((element as Page).Content as DependencyObject))
            //    return true;

            return result;   // 校验通过。
        }

        /// <summary>
        /// 检查是否有错误。
        /// </summary>
        /// <param name="element">要检查的元素。</param>
        /// <returns>true为有错误，false为无错误。</returns>
        public static bool HasError(this DependencyObject element)
        {
            if (Validation.GetHasError(element))
                return true;

            if (element is ContentControl contentControl
                && contentControl.Content is DependencyObject content
                && content.HasError())    // 检查 ContentControl 对象。
                return true;

            if (element is Panel panel)                                                           // 检查 Panel 对象。
                foreach (UIElement child in panel.Children)
                    if (child.HasError())
                        return true;

            if (element is ItemsControl itemsControl)                                                    // 检查 ItemsControl 对象。
                foreach (object item in itemsControl.Items)
                {
                    if (item is not DependencyObject)
                        continue;

                    if (((DependencyObject)item).HasError())
                        return true;
                }

            return false;   // 无错误。
        }

        /// <summary>
        /// 校验数据集。不通过时，显示错误消息框。
        /// </summary>
        /// <param name="data">被校验的数据集。</param>
        /// <returns>true为校验不通过，false为校验通过。</returns>
        /// <remarks>
        /// 主要校验非空校验。
        /// </remarks>
        public static bool Verify(this DataSet data)
        {
            var result = data.Verify(out string errorText);

            if (result)
                MessageDialog.Error(errorText);

            return result;
        }

        /// <summary>
        /// 校验数据集。
        /// </summary>
        /// <param name="data">被校验的数据集。</param>
        /// <param name="errorText">输出的错误文本。</param>
        /// <returns>true为校验不通过，false为校验通过。</returns>
        /// <remarks>
        /// 主要校验非空校验。
        /// </remarks>
        public static bool Verify(this DataSet data, out string errorText)
        {
            errorText = string.Empty;

            if (null == data)
                return false;

            var result = false;
            var builder = new StringBuilder();
            foreach (DataTable table in data.Tables)
                if (table.Verify(out string error))
                {
                    builder.Append(error);
                    result = true;
                }

            if (result)
                errorText = builder.ToString();

            return result;
        }

        /// <summary>
        /// 是否为空判断。
        /// </summary>
        /// <param name="val">要判定的值。</param>
        /// <param name="column">值所在的数据列。</param>
        /// <returns>true为空，false不为空。</returns>
        public static bool IsNull(object val, DataColumn column)
        {
            return null == val || DBNull.Value == val
                || (column.ColumnName.ToLower().EndsWith("_id") && column.DataType == typeof(long) && (long)val == 0L)
                || (column.DataType == typeof(string) && string.IsNullOrWhiteSpace(val as string)
                || (column.DataType == typeof(Guid) && (Guid)val == Guid.Empty));
        }

        /// <summary>
        /// 校验数据表。不通过时，显示错误消息框。
        /// </summary>
        /// <param name="table">被校验的数据表。</param>
        /// <returns>true为校验不通过，false为校验通过。</returns>
        /// <remarks>
        /// 主要校验非空校验。
        /// </remarks>
        public static bool Verify(this DataTable table)
        {
            var result = table.Verify(out string errorText);

            if (result)
                MessageDialog.Error(errorText);

            return result;
        }

        /// <summary>
        /// 校验数据表。
        /// </summary>
        /// <param name="table">被校验的数据表。</param>
        /// <param name="errorText">输出的错误文本。</param>
        /// <returns>true为校验不通过，false为校验通过。</returns>
        /// <remarks>
        /// 主要校验非空校验。
        /// </remarks>
        public static bool Verify(this DataTable table, out string errorText)
        {
            errorText = string.Empty;
            var builder = new StringBuilder();

            if (null == table)
                return false;

            int index;
            var result = false;
            foreach (DataColumn column in table.Columns)
                if (Convert.ToBoolean(column.ExtendedProperties[MemoryData.ExtendedPropertyNames.IsVisible]) && Convert.ToBoolean(column.ExtendedProperties[MemoryData.ExtendedPropertyNames.IsRequired])) // 检查数据列是否设置为必须。
                {
                    index = 0;
                    foreach (DataRow row in table.Rows)
                    {
                        if (row.RowState == DataRowState.Deleted)
                            continue;
                        index++;
                        if (row.RowState == DataRowState.Unchanged)
                            continue;

                        if (IsNull(row[column], column))
                        {
                            builder.Append(index <= 1 ? GlobalCommon.GetMessage("Message.NonnullRuleError", column.Caption) : GlobalCommon.GetMessage("Message.DetailNonnullRuleError", index, column.Caption));
                            builder.Append('\t');
                            result = true;
                        }
                    }
                }

            errorText = builder.ToString();
            return result;
        }

        /// <summary>
        /// 校验数据记录视图。不通过时，显示错误消息框。
        /// </summary>
        /// <param name="rowView">被校验的数据记录视图。</param>
        /// <returns>true为校验不通过，false为校验通过。</returns>
        /// <remarks>
        /// 主要校验非空校验。
        /// </remarks>
        public static bool Verify(this DataRowView rowView)
        {
            var result = rowView.Verify(out string errorText);

            if (result)
                MessageDialog.Error(errorText);

            return result;
        }

        /// <summary>
        /// 校验数据记录视图。
        /// </summary>
        /// <param name="rowView">被校验的数据记录视图。</param>
        /// <param name="errorText">输出的错误文本。</param>
        /// <returns>true为校验不通过，false为校验通过。</returns>
        /// <remarks>
        /// 主要校验非空校验。
        /// </remarks>
        public static bool Verify(this DataRowView rowView, out string errorText)
        {
            errorText = string.Empty;

            if (null == rowView)
                return false;

            return rowView.Row.Verify(out errorText);
        }

        /// <summary>
        /// 校验数据记录。不通过时，显示错误消息框。
        /// </summary>
        /// <param name="row">被校验的数据记录。</param>
        /// <returns>true为校验不通过，false为校验通过。</returns>
        /// <remarks>
        /// 主要校验非空校验。
        /// </remarks>
        public static bool Verify(this DataRow row)
        {
            var result = row.Verify(out string errorText);

            if (result)
                MessageDialog.Error(errorText);

            return result;
        }

        /// <summary>
        /// 校验数据记录。
        /// </summary>
        /// <param name="row">被校验的数据记录。</param>
        /// <param name="errorText">输出的错误文本。</param>
        /// <returns>true为校验不通过，false为校验通过。</returns>
        /// <remarks>
        /// 主要校验非空校验。
        /// </remarks>
        public static bool Verify(this DataRow row, out string errorText)
        {
            errorText = string.Empty;

            if (null == row)
                return false;

            foreach (DataColumn column in row.Table.Columns)
                if (Convert.ToBoolean(column.ExtendedProperties[MemoryData.ExtendedPropertyNames.IsRequired]) && IsNull(row[column], column))
                {
                    errorText = GlobalCommon.GetMessage("NonnullRuleError", column.Caption);
                    return true;
                }

            return false;
        }
    }
}
