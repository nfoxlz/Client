using System.Threading;

namespace Compete.Threading
{
    internal abstract class SyncThreadingBase
    {
        /// <summary>
        /// 标识是否在运行。
        /// </summary>
        private bool isRunning;

        /// <summary>
        /// 执行线程。
        /// </summary>
        private void ExecuteThreading()
        {
            Execute();
            isRunning = false;
        }

        /// <summary>
        /// 执行。
        /// </summary>
        protected abstract void Execute();

        /// <summary>
        /// 调用新的线程，执行方法。
        /// </summary>
        public void Invoke()
        {
            var thread = new Thread(ExecuteThreading);     // 定义新线程。

            isRunning = true;

            thread.Start();                                                 // 开始新线程。

            while (isRunning)
                IApplicationManager.DefaultManager.DoEvents();
        }
    }
}
