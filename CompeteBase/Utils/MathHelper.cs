// ======================================================
// XXX项目
// ======================================================
// 版权所有 © Compete software studio 2019 保留所有权利。
// ------------------------------------------------------
// 版本    日期时间            作者     说明
// ------------------------------------------------------
// 1.0.0.0 2019/4/18 周四 10:11:33 LeeZheng 新建。
// ======================================================
using System;

namespace Compete.Utils
{
    /// <summary>
    /// Math 类。
    /// </summary>
    public static class MathHelper
    {
        private static T? Compete<T>(T? val1, T? val2, Func<T, T, T> func) where T : struct
        {
            if (val1 == null)
                return val2;

            if (val2 == null)
                return val1;

            return func(val1.Value, val2.Value);
        }

        public static long? Max(long? val1, long? val2) => Compete(val1, val2, Math.Max);

        public static int? Max(int? val1, int? val2) => Compete(val1, val2, Math.Max);

        public static short? Max(short? val1, short? val2) => Compete(val1, val2, Math.Max);

        public static sbyte? Max(sbyte? val1, sbyte? val2) => Compete(val1, val2, Math.Max);

        public static decimal? Max(decimal? val1, decimal? val2) => Compete(val1, val2, Math.Max);

        public static double? Max(double? val1, double? val2) => Compete(val1, val2, Math.Max);

        public static float? Max(float? val1, float? val2) => Compete(val1, val2, Math.Max);

        public static ulong? Max(ulong? val1, ulong? val2) => Compete(val1, val2, Math.Max);

        public static uint? Max(uint? val1, uint? val2) => Compete(val1, val2, Math.Max);

        public static ushort? Max(ushort? val1, ushort? val2) => Compete(val1, val2, Math.Max);

        public static byte? Max(byte? val1, byte? val2) => Compete(val1, val2, Math.Max);

        public static long? Min(long? val1, long? val2) => Compete(val1, val2, Math.Min);

        public static int? Min(int? val1, int? val2) => Compete(val1, val2, Math.Min);

        public static short? Min(short? val1, short? val2) => Compete(val1, val2, Math.Min);

        public static sbyte? Min(sbyte? val1, sbyte? val2) => Compete(val1, val2, Math.Min);

        public static decimal? Min(decimal? val1, decimal? val2) => Compete(val1, val2, Math.Min);

        public static double? Min(double? val1, double? val2) => Compete(val1, val2, Math.Min);

        public static float? Min(float? val1, float? val2) => Compete(val1, val2, Math.Min);

        public static ulong? Min(ulong? val1, ulong? val2) => Compete(val1, val2, Math.Min);

        public static uint? Min(uint? val1, uint? val2) => Compete(val1, val2, Math.Min);

        public static ushort? Min(ushort? val1, ushort? val2) => Compete(val1, val2, Math.Min);

        public static byte? Min(byte? val1, byte? val2) => Compete(val1, val2, Math.Min);
    }
}
