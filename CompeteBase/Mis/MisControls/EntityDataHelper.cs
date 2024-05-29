// ======================================================
// XXX项目
// ======================================================
// 版权所有 © Compete software studio 2019 保留所有权利。
// ------------------------------------------------------
// 版本    日期时间            作者     说明
// ------------------------------------------------------
// 1.0.0.0 2019/8/1 周四 下午 3:24:28 LeeZheng 新建。
// ======================================================

using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Compete.Mis.MisControls
{
    /// <summary>
    /// EntityDataHelper 类。
    /// </summary>
    internal static class EntityDataHelper
    {
        private static readonly string[] columns = ["Bill_Id"];//"Id", 

        public static bool IsEntityColumn(string name)
        {
            foreach (var column in columns)
                if (column == name)
                    return false;
            return true;
        }

        public static string? GetTreeDisplay(string valuePath, string levelPath, string namePath, string? levelLength, DataTable entities, object? value)
        {
            var entityData = entities.AsEnumerable();

            var valueRow = (from row in entityData
                            where row[valuePath].Equals(value)
                            select row).FirstOrDefault();
            if (null == valueRow)
                return string.Empty;

            var lvlLength = (from length in levelLength?.Split('-', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries) ?? [valueRow[levelPath].ToString()!.Length.ToString()]
                             select Convert.ToInt32(length)).ToArray();


            var displayList = new List<string>();
            displayList.Insert(0, valueRow[namePath].ToString()!);
            var code = valueRow[levelPath].ToString();
            var sum = lvlLength.Sum();

            for (var i = lvlLength.Length - 1; 0 <= i; i--)
            {
                sum -= lvlLength[i];

                if (valueRow![levelPath].ToString()!.Length <= sum)
                    continue;

                valueRow = (from row in entityData
                            where row[levelPath].Equals(valueRow![levelPath].ToString()![..sum])
                            select row).FirstOrDefault();

                if (null == valueRow)
                    break;

                displayList.Insert(0, valueRow[namePath].ToString()!);
            }

            return string.Join('-', displayList) + $"({code})";
        }
    }
}
