// ===================================================================
// Compete Management Information System
// ===================================================================
// 版权所有 © Compete software studio 2018 保留所有权利。
// -------------------------------------------------------------------
// 版本    日期时间            作者     说明
// -------------------------------------------------------------------
// 1.0.0.0 2018/6/1 21:58:09 LeeZheng 新建。
//==============================================================
using System.Windows;
using System.Windows.Input;

namespace Compete.Mis
{
    /// <summary>
    /// ViewHelper 类。
    /// </summary>
    public static class ViewHelper
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
            // 显示等待控件。
            if (GlobalCommon.MainBusyIndicator is not null)
            {
                GlobalCommon.MainBusyIndicator.BusyContent = string.IsNullOrWhiteSpace(busyContent) ? waitMessage : GlobalCommon.GetMessage("WaitMessage", GlobalCommon.GetMessage(busyContent));
                GlobalCommon.MainBusyIndicator.IsBusy = true;
            }

            if (element is null)
                return null;

            // 将鼠标光标图橡改为等待。
            Cursor currentCursor = element.Cursor;
            element.Cursor = Cursors.Wait;
            return currentCursor;
        }

        /// <summary>
        /// 结束等待控件。
        /// </summary>
        /// <param name="element">要还原鼠标光标图橡的元素。</param>
        /// <param name="currentCursor">元素的原始鼠标光标图橡。</param>
        public static void EndProcess(this FrameworkElement element, Cursor? currentCursor = null)
        {
            if (GlobalCommon.MainBusyIndicator is not null)
                GlobalCommon.MainBusyIndicator.IsBusy = false;    // 结束等待控件。
            if (element is not null)
                element.Cursor = currentCursor;         // 还原鼠标光标图橡。
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

        /// <summary>
        /// 取得所在窗口。
        /// </summary>
        /// <param name="frameworkElement">要检测的控件。</param>
        /// <returns>所在窗口，未找到返回空。</returns>
        public static Window? GetWindow(this FrameworkElement frameworkElement)
        {
            if (frameworkElement is Window)
                return frameworkElement as Window;

            var element = frameworkElement;
            while (element is not null)
            {
                if (element.Parent is Window)
                    return element.Parent as Window;

                element = element.Parent as FrameworkElement;
            }

            return null;
        }
    }
}
