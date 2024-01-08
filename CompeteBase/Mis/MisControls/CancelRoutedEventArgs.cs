// ===================================================================
// Compete Management Information System
// ===================================================================
// 版权所有 © Compete software studio 2018 保留所有权利。
// -------------------------------------------------------------------
// 版本    日期时间            作者     说明
// -------------------------------------------------------------------
// 1.0.0.0 2018/2/9 15:17:00 LeeZheng  新建。
// ===================================================================
using System.Windows;

namespace Compete.Mis.MisControls
{
    /// <summary>
    /// 取消路由事件参数类。
    /// </summary>
    public class CancelRoutedEventArgs : RoutedEventArgs
    {
        /// <summary>
        /// 获取或设置一个值，该值指示是否应取消事件。
        /// </summary>
        public bool Cancel { get; set; }

        /// <summary>
        /// 初始化 <see cref="CancelRoutedEventArgs"/> 类的新实例。
        /// </summary>
        /// <param name="routedEvent">此实例的路由的事件标识符</param>
        /// <param name="source">当处理该事件将报告一个备用源。 这将预先填充 Source 属性。</param>
        public CancelRoutedEventArgs(RoutedEvent routedEvent, object source)
            : base(routedEvent, source) => Cancel = false;
    }
}
