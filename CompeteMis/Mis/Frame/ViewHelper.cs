using System.Windows;
using System.Windows.Input;

namespace Compete.Mis.Frame
{
    internal static class ViewHelper
    {
        /// <summary>
        /// 等待控件消息。
        /// </summary>
        private static readonly string waitMessage = GlobalCommon.GetMessage("WaitMessage");

        /// <summary>
        /// 开始显示等待控件。
        /// </summary>
        /// <param name="element">要取得鼠标光标图橡的元素。</param>
        /// <param name="busyContent">等待控件显示内容，如果为 null，显示默认内容。</param>
        /// <returns>元素的原始鼠标光标图橡。</returns>
        public static Cursor? BeginProcess(this FrameworkElement element, string? busyContent = null)
        {
            //Application.Current.Dispatcher.Invoke(() => Application.Current.MainWindow.IsEnabled = false);

            // 显示等待控件。
            GlobalCommon.MainBusyIndicator?.Dispatcher.Invoke(() =>
                {
                    GlobalCommon.MainBusyIndicator.BusyContent = string.IsNullOrWhiteSpace(busyContent) ? waitMessage : GlobalCommon.GetMessage("WaitMessage", GlobalCommon.GetMessage(busyContent));
                    GlobalCommon.MainBusyIndicator.IsBusy = true;
                });

            if (element is null)
                return null;

            // 将鼠标光标图橡改为等待。
            return element.Dispatcher.Invoke(() =>
            {
                Cursor currentCursor = element.Cursor;
                element.Cursor = Cursors.Wait;
                return currentCursor;
            });
        }

        /// <summary>
        /// 结束等待控件。
        /// </summary>
        /// <param name="element">要还原鼠标光标图橡的元素。</param>
        /// <param name="currentCursor">元素的原始鼠标光标图橡。</param>
        public static void EndProcess(this FrameworkElement element, Cursor? currentCursor = null)
        {
            element?.Dispatcher.Invoke(() => element.Cursor = currentCursor);    // 还原鼠标光标图橡。

            GlobalCommon.MainBusyIndicator?.Dispatcher.Invoke(() => GlobalCommon.MainBusyIndicator.IsBusy = false);  // 结束等待控件。

            //Application.Current.Dispatcher.Invoke(() => Application.Current.MainWindow.IsEnabled = true);
        }

        /// <summary>
        /// 开始显示等待控件。
        /// </summary>
        /// <param name="busyContent">等待控件显示内容，如果为 null，显示默认内容。</param>
        /// <returns>等待控件的原始鼠标光标图橡。</returns>
        public static Cursor? BeginProcess(string? busyContent = null) => GlobalCommon.MainBusyIndicator?.BeginProcess(busyContent);

        /// <summary>
        /// 结束等待控件。
        /// </summary>
        /// <param name="currentCursor">等待控件的原始鼠标光标图橡。</param>
        public static void EndProcess(Cursor? currentCursor = null) => GlobalCommon.MainBusyIndicator?.EndProcess(currentCursor);
    }
}
