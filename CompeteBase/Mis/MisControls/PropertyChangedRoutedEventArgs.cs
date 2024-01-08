// ===================================================================
// Compete Management Information System
// ===================================================================
// 版权所有 © Compete software studio 2018 保留所有权利。
// -------------------------------------------------------------------
// 版本    日期时间            作者     说明
// -------------------------------------------------------------------
// 1.0.0.0 2018/2/9 10:15:15 LeeZheng  新建。
// ===================================================================
using System.Windows;

namespace Compete.Mis.MisControls
{
    /// <summary>
    /// 属性改变后路由事件参数类。
    /// </summary>
    public class PropertyChangedRoutedEventArgs : RoutedEventArgs
    {
        /// <summary>
        /// 初始化 <see cref="PropertyChangedRoutedEventArgs"/> 类的新实例。
        /// </summary>
        /// <param name="routedEvent">路由事件。</param>
        /// <param name="source">发生事件的源。</param>
        /// <param name="oldValue">更改之前的属性的值。</param>
        /// <param name="newValue">更改后获取属性的值。</param>
        public PropertyChangedRoutedEventArgs(RoutedEvent routedEvent, object source, object oldValue, object newValue)
            : base(routedEvent, source)
        {
            OldValue = oldValue;
            NewValue = newValue;
        }

        /// <summary>
        /// 获取更改之前的属性的值。
        /// </summary>
        public object OldValue { get; }

        /// <summary>
        /// 获取更改后获取属性的值。
        /// </summary>
        public object NewValue { get; }
    }
}
