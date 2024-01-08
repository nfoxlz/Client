using System;

namespace Compete.Threading
{
    internal sealed class SyncThreading : SyncThreadingBase
    {
        public SyncThreading(Action action) => ThreadingAction = action;

        /// <summary>
        /// 获取或设置不具有返回值的方法。
        /// </summary>
        public Action ThreadingAction { get; set; }

        /// <summary>
        /// 执行。
        /// </summary>
        protected override void Execute() => ThreadingAction();

        public static void Invoke(Action action) => new SyncThreading(action).Invoke();
    }

    internal sealed class SyncThreading<T> : SyncThreadingBase
    {
        public SyncThreading(Func<T> threadingFunc) => ThreadingFunc = threadingFunc;


        /// <summary>
        /// 获取或设置执行具有 <see cref="T"/> 类型返回值的方法。
        /// </summary>
        public Func<T> ThreadingFunc { get; set; }

        /// <summary>
        /// 获取返回值。
        /// </summary>
        public T? Result { get; set; } = default;

        /// <summary>
        /// 执行。
        /// </summary>
        protected override void Execute() => Result = ThreadingFunc();

        public static T? Invoke(Func<T> func, T? defaultValue = default)
        {
            var syncThreading = new SyncThreading<T>(func)
            {
                Result = defaultValue,
            };
            syncThreading.Invoke();

            return syncThreading.Result;
        }
    }
}
