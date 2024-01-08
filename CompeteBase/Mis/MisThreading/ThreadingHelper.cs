// ===================================================================
// Compete Management Information System
// ===================================================================
// 版权所有 © Compete software studio 2018 保留所有权利。
// -------------------------------------------------------------------
// 版本    日期时间            作者     说明
// -------------------------------------------------------------------
// 1.0.0.0 2018/3/9 9:03:13 LeeZheng 新建。
//==============================================================
using System;

namespace Compete.Mis.MisThreading
{
    /// <summary>
    /// 线程助手类。执行不具有返回值的方法。
    /// </summary>
    public sealed class ThreadingHelper : ThreadingHelperBase
    {
        /// <summary>
        /// 获取或设置不具有返回值的方法。
        /// </summary>
        public required Action Action { get; set; }

        /// <summary>
        /// 执行。
        /// </summary>
        protected override void Execute() => Action();

        public static T? Invoke<T>(Func<T> func, string? busyContent = null, T? defaultValue = default)
        {
            var helper = new ThreadingHelper<T>()
            {
                Result = defaultValue,
                Func = func
            };
            helper.Invoke(busyContent);

            return helper.Result;
        }

        public static void Invoke(Action action, string? busyContent = null) => new ThreadingHelper() { Action = action }.Invoke(busyContent);
    }

    /// <summary>
    /// 线程助手类。执行具有 <see cref="T"/> 类型返回值的方法。
    /// </summary>
    /// <typeparam name="T">方法的返回值类型。</typeparam>
    public sealed class ThreadingHelper<T> : ThreadingHelperBase
    {
        /// <summary>
        /// 获取或设置执行具有 <see cref="T"/> 类型返回值的方法。
        /// </summary>
        public required Func<T> Func { get; set; }

        /// <summary>
        /// 获取返回值。
        /// </summary>
        public T? Result { get; set; }

        /// <summary>
        /// 执行。
        /// </summary>
        protected override void Execute() => Result = Func();
    }
}
