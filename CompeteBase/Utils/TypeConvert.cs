// ===================================================================
// Compete Management Information System
// ===================================================================
// 版权所有 © Compete software studio 2018 保留所有权利。
// -------------------------------------------------------------------
// 版本    日期时间            作者     说明
// -------------------------------------------------------------------
// 1.0.0.0 2018/3/2 13:12:58 LeeZheng  新建。
// ===================================================================
using System;

namespace Compete.Utils
{
    /// <summary>
    /// 类型转换类。
    /// </summary>
    public static class TypeConvert
    {
        /// <summary>
        /// 返回一个指定类型的对象，该对象的值等效于指定的对象。支持 <see cref="Nullable{T}"/> 结构。
        /// </summary>
        /// <param name="value">一个实现 <see cref="IConvertible"/> 接口的对象。</param>
        /// <param name="conversionType">要返回的对象的类型。</param>
        /// <returns>指定类型的等效对象。</returns>
        public static object? ChangeType(object value, Type conversionType) => value is null ? null : Convert.ChangeType(value, conversionType);

        /// <summary>
        /// 返回一个指定类型的对象，该对象的值等效于指定的对象。支持 <see cref="Nullable{T}"/> 结构。
        /// </summary>
        /// <param name="value">一个实现 <see cref="IConvertible"/> 接口的类型。</param>
        /// <param name="value">一个实现 <see cref="IConvertible"/> 接口的对象。</param>
        /// <returns>指定类型的等效对象。</returns>
        public static T? ChangeNullableType<T>(object? value)
            where T : struct
            => value is null ? null : Convert.ChangeType(value, typeof(T)) as T?;

        /// <summary>
        /// 返回一个指定类型的对象，该对象的值等效于指定的对象。
        /// </summary>
        /// <param name="value">一个实现 <see cref="IConvertible"/> 接口的类型。</param>
        /// <param name="value">一个实现 <see cref="IConvertible"/> 接口的对象。</param>
        /// <returns>指定类型的等效对象。</returns>
        public static T? ChangeType<T>(object value) => value is null ? default : (T)Convert.ChangeType(value, typeof(T));
    }
}
