// ===================================================================
// Compete Management Information System
// ===================================================================
// 版权所有 © Compete software studio 2018 保留所有权利。
// -------------------------------------------------------------------
// 版本    日期时间            作者     说明
// -------------------------------------------------------------------
// 1.0.0.0 2018/2/9 15:19:43 LeeZheng  新建。
// ===================================================================
using System.Windows;

namespace Compete.Mis.MisControls
{
    /// <summary>
    /// 属性改变中路由事件参数类。
    /// </summary>
    public class PropertyChangingRoutedEventArgs(RoutedEvent routedEvent, object source, object oldValue, object newValue)
        : CancelRoutedEventArgs(routedEvent, source)
    {

        /// <summary>
        /// 获取更改之前的属性的值。
        /// </summary>
        public object OldValue { get; } = oldValue;

        /// <summary>
        /// 获取更改后获取属性的值。
        /// </summary>
        public object NewValue { get; } = newValue;
    }
}
