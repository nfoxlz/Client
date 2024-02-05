// ===================================================================
// Compete Management Information System
// ===================================================================
// 版权所有 © Compete software studio 2018 保留所有权利。
// -------------------------------------------------------------------
// 版本    日期时间            作者     说明
// -------------------------------------------------------------------
// 1.0.0.0 2018/3/8 10:21:00 LeeZheng 新建。
//==============================================================
using System;
using System.Collections.Generic;
using System.Data;
using System.Text.Json;

namespace Compete.Extensions
{
    /// <summary>
    /// 类型扩展类。
    /// </summary>
    public static class TypeExtensions
    {
        /// <summary>
        /// 转换为允许为空的类型。
        /// </summary>
        /// <param name="type">转换前的依据类型。</param>
        /// <returns>转换后的类型。</returns>
        public static Type ToNullable(this Type type)
        {
            if (type == typeof(long))
                return typeof(long?);
            else if (type == typeof(int))
                return typeof(int?);
            else if (type == typeof(short))
                return typeof(short?);
            else if (type == typeof(sbyte))
                return typeof(sbyte?);
            else if (type == typeof(ulong))
                return typeof(ulong?);
            else if (type == typeof(uint))
                return typeof(uint?);
            else if (type == typeof(ushort))
                return typeof(ushort?);
            else if (type == typeof(byte))
                return typeof(byte?);
            else if (type == typeof(bool))
                return typeof(bool?);
            else if (type == typeof(decimal))
                return typeof(decimal?);
            else if (type == typeof(double))
                return typeof(double?);
            else if (type == typeof(float))
                return typeof(float?);
            else if (type == typeof(Guid))
                return typeof(Guid?);
            else if (type == typeof(DateTimeOffset))
                return typeof(DateTimeOffset?);
            else if (type == typeof(DateTime))
                return typeof(DateTime?);
            else if (type == typeof(TimeSpan))
                return typeof(TimeSpan?);
            else
                return type;
        }

        /// <summary>
        /// 取得实现类名。
        /// </summary>
        /// <param name="type">基类或接口。</param>
        /// <returns>实现类名。</returns>
        public static string GetClassName(this Type type) => type.IsInterface && type.Name.Length > 1 && type.Name[0] == 'I' && char.IsUpper(type.Name[1]) ? type.Name[1..] : type.Name;

        /// <summary>
        /// 将类型转换为对应的“DbType”成员。
        /// </summary>
        /// <param name="type">转换前的依据类型。</param>
        /// <returns>转换后对应的“DbType”成员。</returns>
        public static DbType ToDbType(this Type type)
        {
            if (type == typeof(long))
                return DbType.Int64;
            else if (type == typeof(int))
                return DbType.Int32;
            else if (type == typeof(short))
                return DbType.Int16;
            else if (type == typeof(sbyte))
                return DbType.SByte;
            else if (type == typeof(ulong))
                return DbType.UInt64;
            else if (type == typeof(uint))
                return DbType.UInt32;
            else if (type == typeof(ushort))
                return DbType.UInt16;
            else if (type == typeof(byte))
                return DbType.Byte;
            else if (type == typeof(bool))
                return DbType.Boolean;
            else if (type == typeof(decimal))
                return DbType.Decimal;
            else if (type == typeof(double))
                return DbType.Double;
            else if (type == typeof(float))
                return DbType.Single;
            else if (type == typeof(Guid))
                return DbType.Guid;
            else if (type == typeof(DateTimeOffset))
                return DbType.DateTimeOffset;
            else if (type == typeof(DateTime))
                return DbType.DateTime;
            else if (type == typeof(TimeSpan))
                return DbType.Time;
            else if (type == typeof(byte[]))
                return DbType.Binary;
            else
                return DbType.String;
        }

        /// <summary>
        /// 取得对象对应的 <see cref="DbType"/> 类型。
        /// </summary>
        /// <typeparam name="T">必须为值类型。</typeparam>
        /// <param name="obj">依据对象。</param>
        /// <returns>对应的 <see cref="DbType"/> 类型。</returns>
        public static DbType GetDbType<T>(this T _) where T : struct => ToDbType(typeof(T));

        /// <summary>
        /// 取得对象对应的 <see cref="DbType"/> 类型。
        /// </summary>
        /// <param name="obj">依据对象。</param>
        /// <returns>对应的 <see cref="DbType"/> 类型。</returns>
        public static DbType GetDbType(this object obj) => obj.GetType().ToDbType();

        /// <summary>
        /// 将DbType成员类型转换为对应的类型。
        /// </summary>
        /// <param name="type"><see cref="DbType"/> 成员类型。</param>
        /// <param name="isRequired">是否是必须的，true时不可为空，false时可以为空。</param>
        /// <returns>转换后对应的类型。</returns>
        public static Type ToType(this DbType type, bool isRequired = true)
        {
            if (isRequired)
                return type switch
                {
                    DbType.Int64 => typeof(long),
                    DbType.Int32 => typeof(int),
                    DbType.Int16 => typeof(short),
                    DbType.SByte => typeof(sbyte),
                    DbType.UInt64 => typeof(ulong),
                    DbType.UInt32 => typeof(uint),
                    DbType.UInt16 => typeof(ushort),
                    DbType.Byte => typeof(byte),
                    DbType.Boolean => typeof(bool),
                    DbType.Decimal or DbType.Currency or DbType.VarNumeric => typeof(decimal),
                    DbType.Double => typeof(double),
                    DbType.Single => typeof(float),
                    DbType.Guid => typeof(Guid),
                    DbType.DateTime or DbType.DateTime2 or DbType.Date => typeof(DateTime),
                    DbType.DateTimeOffset => typeof(DateTimeOffset),
                    DbType.Time => typeof(TimeSpan),
                    DbType.Binary => typeof(byte[]),
                    _ => typeof(string),
                };
            else
                return type switch
                {
                    DbType.Int64 => typeof(long?),
                    DbType.Int32 => typeof(int?),
                    DbType.Int16 => typeof(short?),
                    DbType.SByte => typeof(sbyte?),
                    DbType.UInt64 => typeof(ulong?),
                    DbType.UInt32 => typeof(uint?),
                    DbType.UInt16 => typeof(ushort?),
                    DbType.Byte => typeof(byte?),
                    DbType.Boolean => typeof(bool?),
                    DbType.Decimal or DbType.Currency or DbType.VarNumeric => typeof(decimal?),
                    DbType.Double => typeof(double?),
                    DbType.Single => typeof(float?),
                    DbType.Guid => typeof(Guid?),
                    DbType.DateTime or DbType.DateTime2 or DbType.Date => typeof(DateTime?),
                    DbType.DateTimeOffset => typeof(DateTimeOffset?),
                    DbType.Time => typeof(TimeSpan?),
                    DbType.Binary => typeof(byte[]),
                    _ => typeof(string),
                };
        }

        /// <summary>
        /// 是否是数值类型。
        /// </summary>
        /// <param name="type">检测的类型。</param>
        /// <returns>true时为数值类型，false时为非数值类型。</returns>
        public static bool IsNumeric(this Type type) => type == typeof(decimal) || type == typeof(double) || type == typeof(float)
                || type == typeof(long) || type == typeof(int) || type == typeof(short) || type == typeof(sbyte)
                || type == typeof(ulong) || type == typeof(uint) || type == typeof(ushort) || type == typeof(byte);

        /// <summary>
        /// 检测DbType成员是否是数值类型。
        /// </summary>
        /// <param name="type">检测的DbType成员类型。</param>
        /// <returns>true时为数值类型，false时为非数值类型。</returns>
        public static bool IsNumeric(this DbType type) => type == DbType.Currency || type == DbType.Decimal || type == DbType.Double || type == DbType.Single
                || type == DbType.Int64 || type == DbType.Int32 || type == DbType.Int16 || type == DbType.SByte
                || type == DbType.UInt64 || type == DbType.UInt32 || type == DbType.UInt16 || type == DbType.Byte;

        public static bool IsString(this DbType type) =>
            type == DbType.String || type == DbType.AnsiString || type == DbType.StringFixedLength || type == DbType.AnsiStringFixedLength;

        /// <summary>
        /// 将字符串类型名转换为对应的类型。
        /// </summary>
        /// <param name="val">字符串类型名。</param>
        /// <returns>对应的类型。</returns>
        public static Type ToType(this string val)
        {
            var type = val.ToUpper();

            if (type == "GUID")
                return typeof(Guid);
            else if (type == "INT64" || type == "LONG")
                return typeof(long);
            else if (type == "INT32" || type == "INT")
                return typeof(int);
            else if (type == "INT16" || type == "SHORT")
                return typeof(short);
            else if (type == "UINT64" || type == "ULONG")
                return typeof(ulong);
            else if (type == "UINT32" || type == "UINT")
                return typeof(uint);
            else if (type == "UINT16" || type == "USHORT")
                return typeof(ushort);
            else if (type == "BYTE")
                return typeof(byte);
            else if (type == "SBYTE")
                return typeof(sbyte);
            else if (type == "BOOLEAN" || type == "BOOL")
                return typeof(bool);
            else if (type == "DATETIME")
                return typeof(DateTime);
            else if (type == "DATETIMEOFFSET")
                return typeof(DateTimeOffset);
            else if (type == "TIMESPAN")
                return typeof(TimeSpan);
            else if (type == "DECIMAL")
                return typeof(decimal);
            else if (type == "DOUBLE")
                return typeof(double);
            else if (type == "SINGLE" || type == "FLOAT")
                return typeof(float);
            else if (type == "BIN" || type == "BYTE[]")
                return typeof(byte[]);
            else
                return typeof(string);
        }

        //public static string GetClaimValueType(this object obj)
        //{
        //    if (obj is string)
        //        return ClaimValueTypes.String;
        //    else if (obj is long)
        //        return ClaimValueTypes.Integer64;
        //    else if (obj is int)
        //        return ClaimValueTypes.Integer32;
        //    else if (obj is short)
        //        return ClaimValueTypes.Integer;
        //    else if (obj is ulong)
        //        return ClaimValueTypes.UInteger64;
        //    else if (obj is uint)
        //        return ClaimValueTypes.UInteger32;
        //    else if (obj is bool)
        //        return ClaimValueTypes.Boolean;
        //    else if (obj is double || obj is float || obj is decimal)
        //        return ClaimValueTypes.Double;
        //    else if (obj is DateTime)
        //        return ClaimValueTypes.DateTime;
        //    else if (obj is TimeSpan)
        //        return ClaimValueTypes.Time;
        //    else if (obj is Guid || obj is byte[] || obj is byte)
        //        return ClaimValueTypes.HexBinary;
        //    else
        //        return ClaimValueTypes.String;
        //}

        public static bool IsGenericInterface(this Type type, Type interfaceType)
        {
            foreach(var item in type.GetInterfaces())
                if (item.IsGenericType && item.GetGenericTypeDefinition() == interfaceType)
                    return true;

            return false;
        }

        public static bool IsGenericInterface<T>(this Type type) => type.IsGenericInterface(typeof(T));

        public static bool IsGenericInterface(this object obj, Type interfaceType) => obj.GetType().IsGenericInterface(interfaceType);


        public static bool IsGenericInterface<T>(this object obj) => obj.GetType().IsGenericInterface<T>();

        public static object? GetDefault(this Type type)
        {
            if (type == typeof(string))
                return default(string);
            else if (type == typeof(long))
                return default(long);
            else if (type == typeof(int))
                return default(int);
            else if (type == typeof(short))
                return default(short);
            else if (type == typeof(sbyte))
                return default(sbyte);
            else if (type == typeof(ulong))
                return default(ulong);
            else if (type == typeof(uint))
                return default(uint);
            else if (type == typeof(ushort))
                return default(ushort);
            else if (type == typeof(byte))
                return default(byte);
            else if (type == typeof(decimal))
                return default(decimal);
            else if (type == typeof(double))
                return default(double);
            else if (type == typeof(float))
                return default(float);
            else if (type == typeof(DateTime))
                return default(DateTime);
            else if (type == typeof(DateTimeOffset))
                return default(DateTimeOffset);
            else if (type == typeof(TimeSpan))
                return default(TimeSpan);
            else if (type == typeof(Guid))
                return default(Guid);
            else
                return default;
        }

        public static object? GetValue(this JsonElement element, Type type)
        {
            if (type == typeof(string))
                if (element.ValueKind == JsonValueKind.Number)
                    return element.GetDecimal().ToString();
                else
                    return element.GetString();
            else if (type == typeof(long))
                return element.GetInt64();
            else if (type == typeof(int))
                return element.GetInt32();
            else if (type == typeof(short))
                return element.GetInt16();
            else if (type == typeof(sbyte))
                return element.GetSByte();
            else if (type == typeof(decimal))
                return element.GetDecimal();
            else if (type == typeof(double))
                return element.GetDouble();
            else if (type == typeof(float))
                return element.GetSingle();
            else if (type == typeof(bool))
                return element.GetBoolean();
            else if (type == typeof(DateTime))
                if (element.ValueKind == JsonValueKind.Array)
                {
                    var list = new List<int>();
                    foreach (var item in element.EnumerateArray())
                        list.Add(item.GetInt32());
                    return new DateTime(list[0], list[1], list[2], list[3], list[4], list[5], list[6] / 1000000);
                }
#if JAVA_LANGUAGE
                else if (element.ValueKind == JsonValueKind.Number) // Java
                    return Utils.JavaHelper.ToDateTime(element.GetInt64());
#endif
                else
                    return element.GetDateTime();
            else if (type == typeof(DateTimeOffset))
                return element.GetDateTimeOffset();
            else if (type == typeof(ulong))
                return element.GetUInt64();
            else if (type == typeof(uint))
                return element.GetUInt32();
            else if (type == typeof(ushort))
                return element.GetUInt16();
            else if (type == typeof(byte))
                return element.GetByte();
            else
                return null;
        }
    }
}
