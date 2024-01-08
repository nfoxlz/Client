// ===================================================================
// Compete Management Information System
// ===================================================================
// 版权所有 © Compete software studio 2018 保留所有权利。
// -------------------------------------------------------------------
// 版本    日期时间            作者     说明
// -------------------------------------------------------------------
// 1.0.0.0 2018/2/28 16:51:39 LeeZheng  新建。
// ===================================================================
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;

namespace Compete.Extensions
{
    /// <summary>
    /// 数据扩展类。
    /// </summary>
    public static class DataExtensions
    {
        /// <summary>
        /// 将 <see cref="DataRow"/> 类型对象转换为 <see cref="IDictionary<string, object>"/> 类型的对象。
        /// </summary>
        /// <param name="row"><see cref="DataRow"/> 类型的源对象。</param>
        /// <returns>转换后的 <see cref="IDictionary<string, object>"/> 类型对象。</returns>
        public static IDictionary<string, object> ToDictionary(this DataRow row, string[]? columns = null)
        {
            var result = new Dictionary<string, object>();

            foreach (DataColumn column in row.Table.Columns)
                if (row[column] != DBNull.Value && (columns == null || (from columnName in columns
                                                                        where column.ColumnName == columnName
                                                                        select columnName).Any()))
                    result.Add(column.ColumnName, row[column]);

            return result;
        }

        public static IDictionary<string, object?> ToNullValueDictionary(this DataRow row)
        {
            var result = new Dictionary<string, object?>();

            foreach (DataColumn column in row.Table.Columns)
                result.Add(column.ColumnName, row[column] == DBNull.Value ? null : row[column]);

            return result;
        }

        /// <summary>
        /// 将 <see cref="DataRow"/> 类型对象的数据同名复制到目标对象。
        /// </summary>
        /// <param name="souceRow"><see cref="DataRow"/> 类型源对象。</param>
        /// <param name="targetRow">目标对象。</param>
        public static void CopyTo(this DataRow souceRow, DataRow targetRow, IEnumerable<string>? omittedColumns = null)
        {
            var columns = targetRow.Table.Columns;
            foreach (DataColumn column in souceRow.Table.Columns)
            {
                if (omittedColumns != null && (from columnName in omittedColumns
                                               where columnName == column.ColumnName
                                               select columnName).Any())
                    continue;
                if (columns.Contains(column.ColumnName) && !Mis.MisControls.DataVerifier.IsNull(souceRow[column], column))//souceRow[column.ColumnName] != DBNull.Value
                    targetRow[column.ColumnName] = Convert.ChangeType(souceRow[column], columns[column.ColumnName]!.DataType);
            }

            //var columns = souceRow.Table.Columns;
            ////targetRow.BeginEdit();
            //foreach (DataColumn column in targetRow.Table.Columns)
            //{
            //    if (omittedColumns != null && (from columnName in omittedColumns
            //                                   where columnName == column.ColumnName
            //                                   select columnName).Any())
            //        continue;

            //    if (columns.Contains(column.ColumnName) && !Mis.MisControls.DataVerifier.IsNull(souceRow[column], column))//souceRow[column.ColumnName] != DBNull.Value
            //        targetRow[column] = Convert.ChangeType(souceRow[column.ColumnName], column.DataType);
            //}
            ////targetRow.EndEdit();
        }

        public static object[] ToArray(this DataRow row, IEnumerable<string>? columns = null)
        {
            var result = new List<object>();

            var dataColumns = row.Table.Columns;
            if (columns == null)
                foreach (DataColumn column in dataColumns)
                    result.Add(row[column]);
            else
                foreach (var column in columns)
                {
                    Debug.Assert(dataColumns.Contains(column), $"未找到指定的列【{column}】。");
                    result.Add(row[column]);
                }

            return [.. result];
        }
    }
}
