using Compete.Mis.MisThreading;
using Microsoft.Extensions.Logging;
using System;
using System.Windows;

namespace Compete.Mis.MisControls
{
    /// <summary>
    /// 消息对话框。
    /// </summary>
    public static class MessageDialog
    {
        /// <summary>
        /// 风格，主要处理消息框按钮国际化问题。
        /// </summary>
        private static readonly Style messageBoxStyle = new();

        /// <summary>
        /// 静态构造方法。
        /// </summary>
        static MessageDialog()
        {
            // 添加按钮显示名。
            messageBoxStyle.Setters.Add(new Setter(Xceed.Wpf.Toolkit.MessageBox.OkButtonContentProperty, GlobalCommon.GetMessage("OKButton")));
            messageBoxStyle.Setters.Add(new Setter(Xceed.Wpf.Toolkit.MessageBox.CancelButtonContentProperty, GlobalCommon.GetMessage("CancelButton")));
            messageBoxStyle.Setters.Add(new Setter(Xceed.Wpf.Toolkit.MessageBox.YesButtonContentProperty, GlobalCommon.GetMessage("YesButton")));
            messageBoxStyle.Setters.Add(new Setter(Xceed.Wpf.Toolkit.MessageBox.NoButtonContentProperty, GlobalCommon.GetMessage("NoButton")));
        }

        /// <summary>
        /// 公共方法弹出对话框。
        /// </summary>
        /// <param name="messageBoxText">消息。</param>
        /// <param name="caption">标题。</param>
        /// <param name="button">按钮。</param>
        /// <param name="icon">图标。</param>
        /// <param name="defaultResult">默认按钮。</param>
        /// <param name="args">一个对象数组，其中包含零个或多个要设置格式的对象。</param>
        /// <returns>对话框结果（按了哪个按钮）。</returns>
        private static MessageBoxResult ShowMessageBox(string messageBoxText, string caption, MessageBoxButton button, MessageBoxImage icon, MessageBoxResult defaultResult = MessageBoxResult.OK, params object[] args)
        {
#if DEBUG
            return MessageBox.Show(GlobalCommon.GetMessage(messageBoxText, args), Application.Current.Resources.MergedDictionaries[0][caption].ToString(), button, icon, defaultResult);
#else
            return MessageBox.Show(GlobalCommon.GetMessage(messageBoxText, args), Application.Current.Resources.MergedDictionaries[0][caption].ToString(), button, icon, defaultResult);
            //if (BrowserInteropHelper.IsBrowserHosted)
            //    return MessageBox.Show(GlobalCommon.GetMessage(messageBoxText, args), Application.Current.Resources.MergedDictionaries[0][caption].ToString(), button, icon, defaultResult);
            //else
            //    return Xceed.Wpf.Toolkit.MessageBox.Show(GlobalCommon.GetMessage(messageBoxText, args), Application.Current.Resources.MergedDictionaries[0][caption].ToString(), button, icon, defaultResult, messageBoxStyle);
#endif
        }

        /// <summary>
        /// 显示成功消息。
        /// </summary>
        /// <param name="text">消息。</param>
        /// <param name="args">一个对象数组，其中包含零个或多个要设置格式的对象。</param>
        /// <returns>对话框结果（按了哪个按钮）。</returns>
        public static MessageBoxResult Success(string text, params object[] args) => ShowMessageBox(text, "MessageTitle.Success", MessageBoxButton.OK, MessageBoxImage.Asterisk, MessageBoxResult.OK, args);

        /// <summary>
        /// 显示信息性消息。
        /// </summary>
        /// <param name="text">消息。</param>
        /// <param name="args">一个对象数组，其中包含零个或多个要设置格式的对象。</param>
        /// <returns>对话框结果（按了哪个按钮）。</returns>
        public static MessageBoxResult Information(string text, params object[] args) => ShowMessageBox(text, "MessageTitle.Information", MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.OK, args);

        /// <summary>
        /// 显示提示性消息。
        /// </summary>
        /// <param name="text">消息。</param>
        /// <param name="args">一个对象数组，其中包含零个或多个要设置格式的对象。</param>
        /// <returns>对话框结果（按了哪个按钮）。</returns>
        public static MessageBoxResult Prompt(string text, params object[] args) => ShowMessageBox(text, "MessageTitle.Prompt", MessageBoxButton.OK, MessageBoxImage.Exclamation, MessageBoxResult.OK, args);

        /// <summary>
        /// 显示非关键性问题（警告）消息。
        /// </summary>
        /// <param name="text">消息。</param>
        /// <param name="args">一个对象数组，其中包含零个或多个要设置格式的对象。</param>
        /// <returns>对话框结果（按了哪个按钮）。</returns>
        public static MessageBoxResult Warning(string text, params object[] args) => Warning(text, MessageBoxButton.OK, args);

        public static MessageBoxResult Warning(string text, MessageBoxButton button, params object[] args) => Warning(text, button, button switch
        {
            MessageBoxButton.YesNo or MessageBoxButton.YesNoCancel => MessageBoxResult.Yes,
            _ => MessageBoxResult.OK,
        }, args);

        public static MessageBoxResult Warning(string text, MessageBoxButton button, MessageBoxResult defaultResult, params object[] args) => ShowMessageBox(text, "MessageTitle.Warning", button, MessageBoxImage.Warning, defaultResult, args);

        /// <summary>
        /// 显示可恢复的错误（错误）消息
        /// </summary>
        /// <param name="text">消息</param>
        /// <param name="args">一个对象数组，其中包含零个或多个要设置格式的对象。</param>
        /// <returns>对话框结果（按了哪个按钮）</returns>
        public static MessageBoxResult Error(string text, params object[] args) => ShowMessageBox(text, "MessageTitle.Error", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK, args);

        /// <summary>
        /// 显示错误或应用程序崩溃消息。
        /// </summary>
        /// <param name="text">消息。</param>
        /// <param name="args">一个对象数组，其中包含零个或多个要设置格式的对象。</param>
        /// <returns>对话框结果（按了哪个按钮）。</returns>
        public static MessageBoxResult Critical(string text, params object[] args) => ShowMessageBox(text, "MessageTitle.Critical", MessageBoxButton.OK, MessageBoxImage.Stop, MessageBoxResult.OK, args);

        /// <summary>
        /// 询问。
        /// </summary>
        /// <param name="text">消息。</param>
        /// <param name="button">显示的按钮。</param>
        /// <param name="args">一个对象数组，其中包含零个或多个要设置格式的对象。</param>
        /// <returns>对话框结果（按了哪个按钮）。</returns>
        public static MessageBoxResult Question(string text, MessageBoxButton button, params object[] args) =>
            Question(text, button, button switch
            {
                MessageBoxButton.YesNo or MessageBoxButton.YesNoCancel => MessageBoxResult.Yes,
                _ => MessageBoxResult.OK,
            }, args);

        /// <summary>
        /// 询问。
        /// </summary>
        /// <param name="text">消息。</param>
        /// <param name="button">显示的按钮。</param>
        /// <param name="defaultResult">默认结果。</param>
        /// <param name="args">一个对象数组，其中包含零个或多个要设置格式的对象。</param>
        /// <returns>对话框结果（按了哪个按钮）。</returns>
        public static MessageBoxResult Question(string text, MessageBoxButton button, MessageBoxResult defaultResult, params object[] args) => ShowMessageBox(text, "MessageTitle.Question", button, MessageBoxImage.Question, defaultResult, args);

        /// <summary>
        /// 异常。
        /// </summary>
        /// <param name="ex">异常变量。</param>
        /// <param name="args">一个对象数组，其中包含零个或多个要设置格式的对象。</param>
        /// <returns>对话框结果（按了哪个按钮）。</returns>
        public static MessageBoxResult Exception(Exception exception, params object[] args)
        {
            //Logging.LogHelper.Logger.LogException(exception);
            using (var factory = GlobalCommon.CreateLoggerFactory())
                factory.CreateLogger<ThreadingHelperBase>().LogError(GlobalCommon.LogMessage, exception.ToString());


            return ShowMessageBox(exception.Message, "MessageTitle.Exception", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK, args);
        }
    }
}
