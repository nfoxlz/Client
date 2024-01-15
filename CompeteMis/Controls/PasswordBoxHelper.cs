// ===================================================================
// Compete Management Information System
// ===================================================================
// 版权所有 © Compete software studio 2018 保留所有权利。
// -------------------------------------------------------------------
// 版本    日期时间            作者     说明
// -------------------------------------------------------------------
// 1.0.0.0 2018/2/12 10:18:23 LeeZheng  新建。
// ===================================================================
using System.Windows;
using Xceed.Wpf.Toolkit;

namespace Compete.Controls
{
    /// <summary>
    /// PasswordBox控件助手。
    /// </summary>
    /// <remarks>
    /// 为PasswordBox控件的Password增加绑定功能。
    /// </remarks>
    public static class PasswordBoxHelper
    {
        /// <summary>
        /// 标识 Password 附加属性。
        /// </summary>
        public static readonly DependencyProperty PasswordProperty =
            DependencyProperty.RegisterAttached("Password", typeof(string), typeof(PasswordBoxHelper), new FrameworkPropertyMetadata(string.Empty, OnPasswordPropertyChanged));

        /// <summary>
        /// 标识 Attach 附加属性。
        /// </summary>
        public static readonly DependencyProperty AttachProperty =
            DependencyProperty.RegisterAttached("Attach", typeof(bool), typeof(PasswordBoxHelper), new PropertyMetadata(false, (d, e) =>
            {
                if (d is not WatermarkPasswordBox passwordBox)
                    return;
                if ((bool)e.OldValue)
                    passwordBox.PasswordChanged -= PasswordChanged;
                if ((bool)e.NewValue)
                    passwordBox.PasswordChanged += PasswordChanged;
            }));

        /// <summary>
        /// 标识 IsUpdating 附加属性。
        /// </summary>
        private static readonly DependencyProperty IsUpdatingProperty =
           DependencyProperty.RegisterAttached("IsUpdating", typeof(bool), typeof(PasswordBoxHelper));

        /// <summary>
        /// 设置 Attach 附加属性的值。
        /// </summary>
        /// <param name="dependencyObject">设置值的 DependencyObject。</param>
        /// <param name="value">附加属性的值。</param>
        public static void SetAttach(DependencyObject dependencyObject, bool value) => dependencyObject.SetValue(AttachProperty, value);

        /// <summary>
        /// 取得 Attach 附加属性的值。
        /// </summary>
        /// <param name="dependencyObject">取得值的 DependencyObject。</param>
        /// <returns>附加属性的值。</returns>
        public static bool GetAttach(DependencyObject dependencyObject) => (bool)dependencyObject.GetValue(AttachProperty);

        /// <summary>
        /// 取得 Password 附加属性的值。
        /// </summary>
        /// <param name="dependencyObject">取得值的 DependencyObject。</param>
        /// <returns>附加属性的值。</returns>
        public static string GetPassword(DependencyObject dependencyObject) => (string)dependencyObject.GetValue(PasswordProperty);

        /// <summary>
        /// 设置 Password 附加属性的值。
        /// </summary>
        /// <param name="dependencyObject">设置值的 DependencyObject。</param>
        /// <param name="value">附加属性的值。</param>
        public static void SetPassword(DependencyObject dependencyObject, string value) => dependencyObject.SetValue(PasswordProperty, value);

        /// <summary>
        /// 取得 IsUpdating 附加属性的值。
        /// </summary>
        /// <param name="dependencyObject">取得值的 DependencyObject。</param>
        /// <returns>附加属性的值。</returns>
        private static bool GetIsUpdating(DependencyObject dependencyObject) => (bool)dependencyObject.GetValue(IsUpdatingProperty);

        /// <summary>
        /// 设置 IsUpdating 附加属性的值。
        /// </summary>
        /// <param name="dependencyObject">设置值的 DependencyObject。</param>
        /// <param name="value">附加属性的值。</param>
        private static void SetIsUpdating(DependencyObject dependencyObject, bool value) => dependencyObject.SetValue(IsUpdatingProperty, value);

        /// <summary>
        /// Password 依赖项属性更变的回调方法。
        /// </summary>
        /// <param name="d">属性已更改值的 DependencyObject 。</param>
        /// <param name="node">由所有事件跟踪问题到该属性的有效值的更改事件数据。</param>
        private static void OnPasswordPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            if (sender is not WatermarkPasswordBox passwordBox)
                return;
            passwordBox.PasswordChanged -= PasswordChanged;
            if (!GetIsUpdating(passwordBox))
                passwordBox.Password = (string)e.NewValue;
            passwordBox.PasswordChanged += PasswordChanged;
        }

        /// <summary>
        /// PasswordBox控件的PasswordChanged事件处理方法。
        /// </summary>
        /// <param name="sender">事件处理程序所附加到的对象。</param>
        /// <param name="e">事件数据。</param>
        private static void PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (sender is not WatermarkPasswordBox passwordBox)
                return;
            SetIsUpdating(passwordBox, true);
            SetPassword(passwordBox, passwordBox.Password);
            SetIsUpdating(passwordBox, false);
        }
    }
}
