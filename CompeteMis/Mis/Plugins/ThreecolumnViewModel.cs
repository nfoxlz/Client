﻿using Compete.Extensions;
using Compete.MemoryData;
using System;
using System.Data;

namespace Compete.Mis.Plugins
{
    public sealed class ThreecolumnViewModel : ThreecolumnViewModel<ThreecolumnPluginSetting> { }

    public abstract class ThreecolumnViewModel<T> : SettingDataViewModel<T> where T : ThreecolumnPluginSetting, new()
    {
        protected override void ProcessData(DataSet data)
        {
            base.ProcessData(data);

            if (Setting?.CalculatedColumns is null)
                return;

            DataColumnCollection columns;
            foreach (var pair in Setting.CalculatedColumns)
                if (data.Tables.Contains(pair.Key))
                {
                    columns = data.Tables[pair.Key]!.Columns;
                    foreach (var setting in pair.Value)
                        if (!columns.Contains(setting.BalanceColumn!))
                        {
                            var column = DataCreator.CreateColumn(setting.BalanceColumn!);
                            if (!column.DataType.IsNumeric())
                                column.DataType = typeof(decimal);
                            if (column.DefaultValue is null || column.DefaultValue is DBNull)
                                column.DefaultValue = 0M;
                            columns.Add(column);
                        }
                }

            decimal balance;
            DataTable table;
            foreach (var pair in Setting.CalculatedColumns)
                if (data.Tables.Contains(pair.Key))
                {
                    foreach (var setting in pair.Value)
                    {
                        table = data.Tables[pair.Key]!;
                        if (table.Rows.Count == 0)
                            continue;

                        balance = Convert.ToDecimal(table.Rows[0][setting.BalanceColumn!]);
                        foreach (DataRow row in table.Rows)
                        {
                            balance += Convert.ToDecimal(row[setting.PlusColumn!]) - Convert.ToDecimal(row[setting.MinusColumn!]);
                            row[setting.BalanceColumn!] = balance;
                        }
                    }
                }
        }
    }
}
