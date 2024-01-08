// ===================================================================
// Compete Management Information System
// ===================================================================
// 版权所有 © Compete software studio 2018 保留所有权利。
// -------------------------------------------------------------------
// 版本    日期时间            作者     说明
// -------------------------------------------------------------------
// 1.0.0.0 2018/3/9 14:00:19 LeeZheng 新建。
//==============================================================
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Windows.Threading;

namespace Compete.Mis.MisThreading
{
    /// <summary>
    /// 线程助手基类。
    /// </summary>
    public abstract class ThreadingHelperBase
    {
        /// <summary>
        /// 转让控制权，以便让操作系统处理其它的事件。
        /// </summary>
        protected static void DoEvents()
        {
            var frame = new DispatcherFrame(true);
            Dispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.Background, (SendOrPostCallback)delegate (object? arg)
            {
                ((DispatcherFrame)arg!).Continue = false;
            }, frame);
            Dispatcher.PushFrame(frame);
        }

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
        /// <param name="busyContent">等待控件显示内容，如果为 null，显示默认内容。</param>
        public void Invoke(string? busyContent = null)
        {
            var currentCursor = ViewHelper.BeginProcess(busyContent); // 显示等待对话框。

            try
            {
                var thread = new Thread(new ThreadStart(ExecuteThreading));     // 定义新线程。

                isRunning = true;

                thread.Start();                                                 // 开始新线程。

                while (isRunning)
                    DoEvents();
            }
            catch (Exceptions.BusinessException exception)                      // 本系统定义的可忽略的异常。
            {
                MisControls.MessageDialog.Warning(exception.Message);
            }
            catch (Exception exception)                                         // 其它未处理的异常。
            {
                using (var factory = GlobalCommon.CreateLoggerFactory())
                    factory.CreateLogger<ThreadingHelperBase>().LogError(GlobalCommon.LogMessage, exception.ToString());
                //Controls.MessageDialog.Exception(exception);
                throw;
            }
            finally
            {
                ViewHelper.EndProcess(currentCursor);                 // 关闭等待对话框。
            }
        }
    }
}
