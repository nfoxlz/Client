﻿using Compete.Extensions;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using System.Windows.Documents;

namespace Compete.MemoryData
{
    public static class DataCreator
    {
        public static IEnumerable<DataColumnSetting>? GlobalDataColumnSettings { get; set; }

        //public static IDictionary<string, DataColumnSetting> AttachedDataColumnSettings { get; }
        //    = new Dictionary<string, DataColumnSetting>()
        //    {
        //        { "", new DataColumnSetting() { ColumnName = "BillTypeName" } },
        //    };

        public static IDictionary<ushort, string>? BillTypeNameSettings { get; set; }

        public static DataTable Create(IEnumerable<DataColumnSetting> settings)
        {
            var result = new DataTable();
            foreach (var setting in settings)
                result.Columns.Add(CreateColumn(setting));

            result.TableNewRow += Table_TableNewRow;

            return result;
        }

        private static void Table_TableNewRow(object sender, DataTableNewRowEventArgs e)
        {
            var columns = e.Row.Table.Columns;
            foreach (DataColumn column in columns)
                if (column.ExtendedProperties.Contains(ExtendedPropertyNames.DefaultSystemValue) && (SystemVariables)column.ExtendedProperties[ExtendedPropertyNames.DefaultSystemValue]! != SystemVariables.None)
                    e.Row[column] = Mis.GlobalCommon.CreateSystemVariable((SystemVariables)column.ExtendedProperties[ExtendedPropertyNames.DefaultSystemValue]!) ?? DBNull.Value;
        }

        private static void SetColumnProperties(DataColumn column, DataColumnSetting setting)
        {
            if (!column.AllowDBNull && setting.DefaultValue is null && SystemVariables.None == setting.DefaultSystemValue)
            {
                if (column.DataType == typeof(string))
                    column.DefaultValue = string.Empty;
                else if (column.DataType.IsNumeric())
                    column.DefaultValue = Convert.ChangeType(0, column.DataType);
                else if (column.DataType == typeof(DateTime))
                    column.DefaultValue = DateTime.Now;
                else if (column.DataType == typeof(Guid))
                    column.DefaultValue = default;
                else
                    column.DefaultValue = setting.DefaultValue is null ? DBNull.Value : Convert.ChangeType(setting.DefaultValue.ToString(), column.DataType);
            }
            else
                column.DefaultValue = setting.DefaultValue is null ? DBNull.Value : Convert.ChangeType(setting.DefaultValue.ToString(), column.DataType);
        }

        private static object GetMaxValue(Type type, object? maxValue)
        {
            if (maxValue is null)
            {
                if (type == typeof(decimal))
                    return decimal.MaxValue;
                else if (type == typeof(double))
                    return double.MaxValue;
                else if (type == typeof(float))
                    return float.MaxValue;
                else if (type == typeof(long))
                    return long.MaxValue;
                else if (type == typeof(int))
                    return int.MaxValue;
                else if (type == typeof(short))
                    return short.MaxValue;
                else if (type == typeof(sbyte))
                    return sbyte.MaxValue;
                else if (type == typeof(ulong))
                    return ulong.MaxValue;
                else if (type == typeof(uint))
                    return uint.MaxValue;
                else if (type == typeof(ushort))
                    return ushort.MaxValue;
                else if (type == typeof(byte))
                    return byte.MaxValue;
                else if (type == typeof(DateTime))
                    return DateTime.MaxValue;
                else if (type == typeof(DateTimeOffset))
                    return DateTimeOffset.MaxValue;
                else if (type == typeof(TimeSpan))
                    return TimeSpan.MaxValue;
                else
                    return 0;
            }
            else
                return maxValue;
        }

        private static object? GetMinValue(Type type, object? minValue)
        {
            if (minValue is null)
            {
                if (type == typeof(decimal))
                    return decimal.MinValue;
                else if (type == typeof(double))
                    return double.MinValue;
                else if (type == typeof(float))
                    return float.MinValue;
                else if (type == typeof(long))
                    return long.MinValue;
                else if (type == typeof(int))
                    return int.MinValue;
                else if (type == typeof(short))
                    return short.MinValue;
                else if (type == typeof(sbyte))
                    return sbyte.MinValue;
                else if (type == typeof(ulong))
                    return ulong.MinValue;
                else if (type == typeof(uint))
                    return uint.MinValue;
                else if (type == typeof(ushort))
                    return ushort.MinValue;
                else if (type == typeof(byte))
                    return byte.MinValue;
                else if (type == typeof(DateTime))
                    return DateTime.MinValue;
                else if (type == typeof(DateTimeOffset))
                    return DateTimeOffset.MinValue;
                else if (type == typeof(TimeSpan))
                    return TimeSpan.MinValue;
                else
                    return null;
            }
            else
                return minValue;
        }

        private static DataColumn CreateColumn(DataColumnSetting setting)
        {
            var column = new DataColumn(setting.ColumnName, setting.DataType ?? setting.DbDataType.ToType())
            {
                Caption = setting.Caption,
                AllowDBNull = setting.AllowDBNull,
                ReadOnly = setting.ReadOnly,
                Unique = setting.IsUnique,
                AutoIncrement = setting.IsAutoIncrement,
                AutoIncrementSeed = setting.AutoIncrementSeed,
                AutoIncrementStep = setting.AutoIncrementStep,
                Expression = setting.Expression,
            };

            SetColumnProperties(column, setting);

            if (column.DataType == typeof(string))
                column.MaxLength = setting.MaxLength;
            //else
            //    column.ExtendedProperties[ExtendedPropertyNames.Length] = setting.MaxLength;

            column.ExtendedProperties[ExtendedPropertyNames.DataType] = setting.DbDataType;
            column.ExtendedProperties[ExtendedPropertyNames.IsRequired] = setting.IsRequired;
            column.ExtendedProperties[ExtendedPropertyNames.IsReadOnly] = setting.IsReadOnly;
            column.ExtendedProperties[ExtendedPropertyNames.IsVisible] = setting.IsVisible;
            column.ExtendedProperties[ExtendedPropertyNames.Format] = setting.Format;
            column.ExtendedProperties[ExtendedPropertyNames.ShowFormat] = setting.ShowFormat;
            column.ExtendedProperties[ExtendedPropertyNames.DisplayIndex] = setting.DisplayIndex;
            column.ExtendedProperties[ExtendedPropertyNames.DefaultSystemValue] = setting.DefaultSystemValue;
            column.ExtendedProperties[ExtendedPropertyNames.TargetNullValue] = setting.TargetNullValue;
            column.ExtendedProperties[ExtendedPropertyNames.Maximum] = GetMaxValue(column.DataType, setting.MaxValue);
            column.ExtendedProperties[ExtendedPropertyNames.Minimum] = GetMinValue(column.DataType, setting.MinValue);
            //column.ExtendedProperties[ExtendedPropertyNames.Length] = setting.Length;
            column.ExtendedProperties[ExtendedPropertyNames.Length] = setting.MaxLength;
            column.ExtendedProperties[ExtendedPropertyNames.Precision] = setting.Precision;
            column.ExtendedProperties[ExtendedPropertyNames.Regex] = setting.Regex;
            column.ExtendedProperties[ExtendedPropertyNames.ErrorText] = setting.ErrorText;
            column.ExtendedProperties[ExtendedPropertyNames.Control] = setting.Control;
            column.ExtendedProperties[ExtendedPropertyNames.Parameters] = setting.Parameters;
            return column;
        }

        public static DataColumn CreateColumn(string columnName)
        {
            var columnSetting = (from setting in GlobalDataColumnSettings
                                 where setting.ColumnName.Equals(columnName, StringComparison.OrdinalIgnoreCase)
                                 select setting).FirstOrDefault();
            return columnSetting is null ? new DataColumn(columnName) : CreateColumn(columnSetting);
        }


        private const string billTypeColumnName = "Bill_Type";

        private const string billTypeNameColumnName = "BillTypeName";

        public static DataTable Create(Mis.Models.SimpleData table)
        {
            string column;
            DataColumn dataColumn;
            var result = new DataTable();
            var billTypeIndex = -1;
            bool hasDefaultSystemValue = false;

            result.TableName = table.TableName;

            var count = table.Columns!.Length;
            for (int index = 0; index < count; index++)
            {
                column = table.Columns[index];
                var columnSetting = (from setting in GlobalDataColumnSettings
                                     where setting.ColumnName.Equals(column, StringComparison.OrdinalIgnoreCase)
                                     select setting).FirstOrDefault();
                if (columnSetting is null)
                    result.Columns.Add(column);
                else
                {
                    dataColumn = CreateColumn(columnSetting);
                    result.Columns.Add(dataColumn);
                    if ((SystemVariables)dataColumn.ExtendedProperties[ExtendedPropertyNames.DefaultSystemValue]! != SystemVariables.None)
                        hasDefaultSystemValue = true;
                }

                if (column.Equals(billTypeColumnName, StringComparison.OrdinalIgnoreCase))
                {
                    result.Columns.Add(new DataColumn(billTypeNameColumnName) { Caption = "单据类型" });
                    billTypeIndex = index + 1;
                }
            }

            DataRow newRow;
            int columnIndex;
            if (table.Rows is not null)
            {
                if (billTypeIndex >= 0)
                    count++;
                foreach (var row in table.Rows)
                {
                    newRow = result.NewRow();
                    //newRow.ItemArray = row;
                    columnIndex = 0;
                    for (int i = 0; i < count; i++)
                    {
                        if (i == billTypeIndex)
                        {
                            if (BillTypeNameSettings!.TryGetValue((ushort)newRow[billTypeColumnName], out var name))
                                newRow[billTypeNameColumnName] = name;
                            continue;
                        }
                        var newValue = row[columnIndex] is JsonElement element ? element.GetValue(result.Columns[i].DataType) : row[columnIndex];
                        newRow[i] = newValue is null ? result.Columns[i].AllowDBNull ? DBNull.Value : result.Columns[i].DefaultValue ?? result.Columns[i].DataType.GetDefault() : newValue;
                        //newRow[i] = Convert.ChangeType((row[columnIndex] is JsonElement element ? element.GetValue(result.Columns[i].DataType) : row[columnIndex]) ?? result.Columns[i].DefaultValue ?? result.Columns[i].DataType.GetDefault(), result.Columns[i].DataType);
                        columnIndex++;
                    }
                    result.Rows.Add(newRow);
                }
            }

            if (hasDefaultSystemValue)
                result.TableNewRow += Table_TableNewRow;

            return result;
        }

        public static DataSet Create(IList<Mis.Models.SimpleData> tables)
        {
            var result = new DataSet();
            foreach (var table in tables)
                result.Tables.Add(Create(table));
            return result;
        }

        //public static DataSet Create(IList<Mis.Models.SimpleData> tables)
        //{
        //    var result = new DataSet();
        //    foreach (var table in tables)
        //        result.Tables.Add(Create(table));
        //    return result;
        //}

        public static void AddColumns(DataTable table, IEnumerable<DataColumnSetting> settings)
        {
            foreach(var setting in settings)
                table.Columns.Add(CreateColumn(setting));
        }

        public static void AddColumns(DataTable table, IEnumerable<string> settings)
        {
            foreach (var setting in settings)
                table.Columns.Add(CreateColumn(setting));
        }

        public static void AddColumns(DataSet data, IDictionary<string, IEnumerable<string>> settings)
        {
            foreach (DataTable table in data.Tables)
                if (settings.TryGetValue(table.TableName, out IEnumerable<string>? setting))
                    AddColumns(table, setting);
        }

        public static void AddColumns(DataSet data, IDictionary<string, IEnumerable<DataColumnSetting>> settings)
        {
            foreach(DataTable table in  data.Tables)
                if (settings.TryGetValue(table.TableName, out IEnumerable<DataColumnSetting>? setting))
                    AddColumns(table, setting);
        }

        private static void SetColumn(DataColumn column, DataColumnSetting setting)
        {
            if (!string.IsNullOrWhiteSpace(setting.Caption))
                column.Caption = setting.Caption;
            if (column.DataType == typeof(string) && setting.MaxLength >= 0)
                column.MaxLength = setting.MaxLength;
            if (!setting.AllowDBNull)
                column.AllowDBNull = false;
            if (setting.ReadOnly)
                column.ReadOnly = true;
            if (setting.DefaultValue is not null)
                column.DefaultValue = setting.DefaultValue;
            if (setting.IsUnique)
                column.Unique = true;
            if (setting.IsAutoIncrement)
                column.AutoIncrement = true;
            if (setting.AutoIncrementSeed != 1)
                column.AutoIncrementSeed = setting.AutoIncrementSeed;
            if (setting.AutoIncrementStep != 1)
                column.AutoIncrementStep = setting.AutoIncrementStep;
            if (!string.IsNullOrWhiteSpace(setting.Expression))
                column.Expression = setting.Expression;

            SetColumnProperties(column, setting);

            if (setting.IsRequired)
                column.ExtendedProperties[ExtendedPropertyNames.IsRequired] = true;
            if (setting.IsReadOnly)
                column.ExtendedProperties[ExtendedPropertyNames.IsReadOnly] = true;
            if (!setting.IsVisible)
                column.ExtendedProperties[ExtendedPropertyNames.IsVisible] = false;
            if (!string.IsNullOrWhiteSpace(setting.Format))
                column.ExtendedProperties[ExtendedPropertyNames.Format] = setting.Format;
            if (!string.IsNullOrWhiteSpace(setting.ShowFormat))
                column.ExtendedProperties[ExtendedPropertyNames.ShowFormat] = setting.ShowFormat;
            if (setting.DisplayIndex >= 0)
                column.ExtendedProperties[ExtendedPropertyNames.DisplayIndex] = setting.DisplayIndex;
            if (setting.DefaultSystemValue != SystemVariables.None)
                column.ExtendedProperties[ExtendedPropertyNames.DefaultSystemValue] = setting.DefaultSystemValue;
            if (setting.TargetNullValue is not null)
                column.ExtendedProperties[ExtendedPropertyNames.TargetNullValue] = setting.TargetNullValue;
            if (setting.MaxValue is not null)
                column.ExtendedProperties[ExtendedPropertyNames.Maximum] = GetMaxValue(column.DataType, setting.MaxValue);
            if (setting.MinValue is not null)
                column.ExtendedProperties[ExtendedPropertyNames.Minimum] = GetMinValue(column.DataType, setting.MinValue);
            //if (setting.Length > 0)
            //    column.ExtendedProperties[ExtendedPropertyNames.Length] = setting.Length;
            if (setting.MaxLength > 0)
                column.ExtendedProperties[ExtendedPropertyNames.Length] = setting.MaxLength;
            if (setting.Precision > 0)
                column.ExtendedProperties[ExtendedPropertyNames.Precision] = setting.Precision;
            if (!string.IsNullOrWhiteSpace(setting.Regex))
                column.ExtendedProperties[ExtendedPropertyNames.Regex] = setting.Regex;
            if (!string.IsNullOrWhiteSpace(setting.ErrorText))
                column.ExtendedProperties[ExtendedPropertyNames.ErrorText] = setting.ErrorText;
            if (setting.Control != Mis.MisControls.DataControlType.Default)
                column.ExtendedProperties[ExtendedPropertyNames.Control] = setting.Control;
            if (!string.IsNullOrWhiteSpace(setting.Parameters))
                column.ExtendedProperties[ExtendedPropertyNames.Parameters] = setting.Parameters;
        }

        public static void SetColumns(DataTable table, IEnumerable<DataColumnSetting> settings)
        {
            foreach (DataColumn column in table.Columns)
            {
                var columnSetting = (from setting in settings
                                     where setting.ColumnName == column.ColumnName
                                     select setting).FirstOrDefault();
                if (columnSetting is not null)
                    SetColumn(column, columnSetting);
            }
        }

        public static void SetColumns(DataSet data, IDictionary<string, IEnumerable<DataColumnSetting>> settings)
        {
            foreach(DataTable table in data.Tables)
                if (settings.TryGetValue(table.TableName, out IEnumerable<DataColumnSetting>? setting))
                    SetColumns(table, setting);
        }

        public static Mis.Models.SimpleData ConvertSimpleDataTable(DataTable table)
        {
            var columnCount = table.Columns.Count;
            var rowCount = table.Rows.Count;

            var result = new Mis.Models.SimpleData
            {
                TableName = table.TableName,
                Columns = new string[columnCount],
                Rows = new object[rowCount][],
            };

            for (int i = 0; i < columnCount; i++)
                result.Columns[i] = table.Columns[i].ColumnName;

            for (int i = 0; i < rowCount; i++)
            {
                result.Rows[i] = new object[columnCount];
                for (int j = 0; j < columnCount; j++)
#if JAVA_LANGUAGE
                    if (table.Columns[j].DataType == typeof(DateTime))    // Java
                        result.Rows[i][j] = Utils.JavaHelper.ConvertDateTime((DateTime)table.Rows[i][j]);
                    else if (table.Columns[j].DataType == typeof(DateTimeOffset))
                        result.Rows[i][j] = Utils.JavaHelper.ConvertDateTime((DateTimeOffset)table.Rows[i][j]);
                    else
#endif
                        result.Rows[i][j] = table.Rows[i][j];
            }

            return result;
        }

        //public static IDictionary<string, Mis.Models.SimpleDataTable> ConvertSimpleDataSet(DataSet data)
        //{
        //    var result = new Dictionary<string, Mis.Models.SimpleDataTable>();
        //    foreach (DataTable table in data.Tables)
        //        result.Add(table.TableName, ConvertSimpleDataTable(table));
        //    return result;
        //}
        public static IList<Mis.Models.SimpleData> ConvertSimpleDataSet(DataSet data)
        {
            var result = new List<Mis.Models.SimpleData>();
            foreach (DataTable table in data.Tables)
                result.Add(ConvertSimpleDataTable(table));
            return result;
        }

        public static DataTable Create(string tableName, IEnumerable<string> columns)
        {
            var result = new DataTable(tableName);

            foreach (var column in columns)
            {
                var columnSetting = (from setting in GlobalDataColumnSettings
                                     where setting.ColumnName == column
                                     select setting).FirstOrDefault();

                result.Columns.Add(CreateColumn(columnSetting ?? new DataColumnSetting { ColumnName = column, Caption = column, DataType = typeof(string), DbDataType = DbType.String }));
            }

            result.TableNewRow += Table_TableNewRow;

            return result;
        }

        public static DataSet Create(IDictionary<string, IEnumerable<string>> settings)
        {
            var result = new DataSet();

            foreach (var setting in settings)
                result.Tables.Add(Create(setting.Key, setting.Value));

            return result;
        }
    }
}
