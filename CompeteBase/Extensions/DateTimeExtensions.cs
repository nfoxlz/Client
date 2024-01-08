// ======================================================
// Compete Management Information System
// ======================================================
// 版权所有 © Compete software studio 2019 保留所有权利。
// ------------------------------------------------------
// 版本    日期时间               作者     说明
// ------------------------------------------------------
// 1.0.0.0 2019/4/1 周一 14:23:51 LeeZheng 新建。
// ======================================================
using System;

namespace Compete.Extensions
{
    /// <summary>
    /// DateTimeExtensions 类。
    /// </summary>
    public static class DateTimeExtensions
    {
        public static int GetQuarter(this DateTime dateTime) => dateTime.Month / 4;

        public static DateTime AddQuarter(this DateTime dateTime, int quarters) => dateTime.AddMonths(quarters * 4);

        public static int GetTenDay(this DateTime dateTime) => dateTime.Day == 31 ? 2 : dateTime.Day / 10;

        private static readonly string[] tenDayNames = { "上旬", "中旬", "下旬" };

        public static string GetTenDayName(this DateTime dateTime) => tenDayNames[dateTime.GetTenDay()];
    }
}
