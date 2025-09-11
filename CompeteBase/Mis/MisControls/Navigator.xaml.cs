// ===================================================================
// Compete Management Information System
// ===================================================================
// 版权所有 © Compete software studio 2018 保留所有权利。
// -------------------------------------------------------------------
// 版本    日期时间            作者     说明
// -------------------------------------------------------------------
// 1.0.0.0 2018/2/5 9:36:41 LeeZheng  新建。
// ===================================================================
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Compete.Mis.MisControls
{
    /// <summary>
    /// Navigator.xaml 的交互逻辑
    /// </summary>
    public partial class Navigator : UserControl
    {
        /// <summary>
        /// 本控件的ViewModel。
        /// </summary>
        private readonly NavigatorViewModel viewModel;

        /// <summary>
        /// 初始化 <see cref="Navigator"/> 类的新实例。
        /// </summary>
        public Navigator()
        {
            InitializeComponent();

            viewModel = (NavigatorViewModel)MainStackPanel.DataContext;

            // PageSize 更变中处理。
            viewModel.PageSizeChangingFunc = (oldValue, newValue) =>
            {
                var args = new PropertyChangingRoutedEventArgs(PageSizeChangingRoutedEvent, this, oldValue, newValue);
                RaiseEvent(args);
                return args.Cancel;
            };

            // PageSize 更变后处理。
            viewModel.PageSizeChangedAction = (oldValue, newValue) =>
            {
                PageSize = newValue;

                var args = new PropertyChangedRoutedEventArgs(PageSizeChangedRoutedEvent, this, oldValue, newValue);
                RaiseEvent(args);
            };

            // CurrentPageNo 更变中处理。
            viewModel.CurrentPageNoChangingFunc = (oldValue, newValue) =>
            {
                var args = new PropertyChangingRoutedEventArgs(CurrentPageNoChangingRoutedEvent, this, oldValue, newValue);
                RaiseEvent(args);
                return args.Cancel;
            };

            // CurrentPageNo 更变后处理。
            viewModel.CurrentPageNoChangedAction = (oldValue, newValue) =>
            {
                CurrentPageNo = newValue;

                var args = new PropertyChangedRoutedEventArgs(CurrentPageNoChangedRoutedEvent, this, oldValue, newValue);
                RaiseEvent(args);
            };
        }

        #region 依赖属性

        /// <summary>
        /// 获取或设置一个值，该值指示记录总数。
        /// </summary>
        public ulong RecordCount
        {
            get { return (ulong)GetValue(RecordCountProperty); }
            set { SetValue(RecordCountProperty, value); }
        }

        /// <summary>
        /// 标识 RecordCount 的依赖属性。
        /// </summary>
        public static readonly DependencyProperty RecordCountProperty =
            DependencyProperty.Register(nameof(RecordCount), typeof(ulong), typeof(Navigator), new PropertyMetadata(0UL, (d, e) =>
            {
                var navigator = d as Navigator;
                navigator!.viewModel.RecordCount = navigator.RecordCount;
            }));

        /// <summary>
        /// 获取或设置一个值，该值指示每页的记录数。
        /// </summary>
        public ushort PageSize
        {
            get { return (ushort)GetValue(PageSizeProperty); }
            set { SetValue(PageSizeProperty, value); }
        }

        /// <summary>
        /// 标识 PageSize 的依赖属性。
        /// </summary>
        public static readonly DependencyProperty PageSizeProperty =
            DependencyProperty.Register(nameof(PageSize), typeof(ushort), typeof(Navigator), new PropertyMetadata(GlobalConstants.DefaultNavigatorPageSize,
                (d, e) =>
                {
                    var navigator = d as Navigator;
                    navigator!.PageCollectionComboBox.SelectedItem = navigator.PageSize;
                    navigator.viewModel.PageSize = navigator.PageSize;
                }),
                (value) => (ushort)value > 0);

        /// <summary>
        /// 获取或设置一组值，该组值用于每页显示最大记录的备选值，选择后做为PageSize的值。
        /// </summary>
        public IEnumerable<ushort> PageCollection
        {
            get { return (IEnumerable<ushort>)GetValue(PageCollectionProperty); }
            set { SetValue(PageCollectionProperty, value); }
        }

        /// <summary>
        /// 标识 PageCollection 的依赖属性。
        /// </summary>
        public static readonly DependencyProperty PageCollectionProperty =
            DependencyProperty.Register(nameof(PageCollection), typeof(IEnumerable<ushort>), typeof(Navigator), new PropertyMetadata(GlobalConstants.DefaultNavigatorPageCollection, (d, e) =>
            {
                var navigator = d as Navigator;
                navigator!.viewModel.PageCollection = navigator.PageCollection;
            }));

        /// <summary>
        /// 获取或设置一个值，该值指示当前页号，值为1到MaxPageNo之间的数；当MaxPageNo为0时，值为1。
        /// </summary>
        public ulong CurrentPageNo
        {
            get { return (ulong)GetValue(CurrentPageNoProperty); }
            set { SetValue(CurrentPageNoProperty, value); }
        }

        /// <summary>
        /// 标识 CurrentPageNo 的依赖属性。
        /// </summary>
        public static readonly DependencyProperty CurrentPageNoProperty =
            DependencyProperty.Register(nameof(CurrentPageNo), typeof(ulong), typeof(Navigator), new PropertyMetadata(1UL, (d, e) =>
            {
                var navigator = d as Navigator;
                navigator!.viewModel.CurrentPageNo = navigator.CurrentPageNo;
            }));

        //public ulong MaxPageNo
        //{
        //    get { return (ulong)GetValue(MaxPageNoProperty); }
        //    set { SetValue(MaxPageNoProperty, value); }
        //}

        //// Using a DependencyProperty as the backing store for MaxPageNo.  This enables animation, styling, binding, etc...
        //public static readonly DependencyProperty MaxPageNoProperty =
        //    DependencyProperty.Register("MaxPageNo", typeof(ulong), typeof(Navigator), new PropertyMetadata(0UL));

        /// <summary>
        /// 获取一个值，该值指示最大页号，无数据时为“0”。
        /// </summary>
        public ulong MaxPageNo
        {
            get
            {
                return viewModel.MaxPageNo;
            }
        }

        /// <summary>
        /// 获取或设置当前页改变中命令。
        /// </summary>
        public ICommand CurrentPageNoChangingCommand
        {
            get { return (ICommand)GetValue(CurrentPageNoChangingCommandProperty); }
            set { SetValue(CurrentPageNoChangingCommandProperty, value); }
        }

        /// <summary>
        /// 标识 CurrentPageNoChangingCommand 的依赖属性。
        /// </summary>
        public static readonly DependencyProperty CurrentPageNoChangingCommandProperty =
            DependencyProperty.Register(nameof(CurrentPageNoChangingCommand), typeof(ICommand), typeof(Navigator), new PropertyMetadata((d, e) =>
            {
                var navigator = d as Navigator;
                navigator!.viewModel.CurrentPageNoChangingCommand = navigator.CurrentPageNoChangingCommand;
            }));

        public object? CurrentPageNoChangingCommandParameter
        {
            get { return (object?)GetValue(CurrentPageNoChangingCommandParameterProperty); }
            set { SetValue(CurrentPageNoChangingCommandParameterProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CurrentPageNoChangingCommandParameter.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CurrentPageNoChangingCommandParameterProperty =
            DependencyProperty.Register(nameof(CurrentPageNoChangingCommandParameter), typeof(object), typeof(Navigator), new PropertyMetadata((d, e) =>
            {
                var navigator = d as Navigator;
                navigator!.viewModel.CurrentPageNoChangingCommandParameter = navigator.CurrentPageNoChangingCommandParameter;
            }));

        /// <summary>
        /// 获取或设置当前页改变后命令。
        /// </summary>
        public ICommand CurrentPageNoChangedCommand
        {
            get { return (ICommand)GetValue(CurrentPageNoChangedCommandProperty); }
            set { SetValue(CurrentPageNoChangedCommandProperty, value); }
        }

        /// <summary>
        /// 标识 CurrentPageNoChangedCommand 的依赖属性。
        /// </summary>
        public static readonly DependencyProperty CurrentPageNoChangedCommandProperty =
            DependencyProperty.Register(nameof(CurrentPageNoChangedCommand), typeof(ICommand), typeof(Navigator), new PropertyMetadata((d, e) =>
            {
                var navigator = d as Navigator;
                navigator!.viewModel.CurrentPageNoChangedCommand = navigator.CurrentPageNoChangedCommand;
            }));

        public object? CurrentPageNoChangedCommandParameter
        {
            get { return (object?)GetValue(CurrentPageNoChangedCommandParameterProperty); }
            set { SetValue(CurrentPageNoChangedCommandParameterProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CurrentPageNoChangedCommandParameter.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CurrentPageNoChangedCommandParameterProperty =
            DependencyProperty.Register(nameof(CurrentPageNoChangedCommandParameter), typeof(object), typeof(Navigator), new PropertyMetadata((d, e) =>
            {
                var navigator = d as Navigator;
                navigator!.viewModel.CurrentPageNoChangedCommandParameter = navigator.CurrentPageNoChangedCommandParameter;
            }));

        /// <summary>
        /// 获取或设置每页的记录数改变中命令。
        /// </summary>
        public ICommand PageSizeChangingCommand
        {
            get { return (ICommand)GetValue(PageSizeChangingCommandProperty); }
            set { SetValue(PageSizeChangingCommandProperty, value); }
        }

        /// <summary>
        /// 标识 PageSizeChangingCommand 的依赖属性。
        /// </summary>
        public static readonly DependencyProperty PageSizeChangingCommandProperty =
            DependencyProperty.Register(nameof(PageSizeChangingCommand), typeof(ICommand), typeof(Navigator), new PropertyMetadata((d, e) =>
            {
                var navigator = d as Navigator;
                navigator!.viewModel.PageSizeChangingCommand = navigator.PageSizeChangingCommand;
            }));

        public object? PageSizeChangingCommandParameter
        {
            get { return (object?)GetValue(PageSizeChangingCommandParameterProperty); }
            set { SetValue(PageSizeChangingCommandParameterProperty, value); }
        }

        // Using a DependencyProperty as the backing store for PageSizeChangingCommandParameter.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PageSizeChangingCommandParameterProperty =
            DependencyProperty.Register(nameof(PageSizeChangingCommandParameter), typeof(object), typeof(Navigator), new PropertyMetadata((d, e) =>
            {
                var navigator = d as Navigator;
                navigator!.viewModel.PageSizeChangingCommandParameter = navigator.PageSizeChangingCommandParameter;
            }));

        public ICommand PageSizeChangedCommand
        {
            get { return (ICommand)GetValue(PageSizeChangedCommandProperty); }
            set { SetValue(PageSizeChangedCommandProperty, value); }
        }

        /// <summary>
        /// 标识 PageSizeChangedCommand 的依赖属性。
        /// </summary>
        public static readonly DependencyProperty PageSizeChangedCommandProperty =
            DependencyProperty.Register(nameof(PageSizeChangedCommand), typeof(ICommand), typeof(Navigator), new PropertyMetadata((d, e) =>
            {
                var navigator = d as Navigator;
                navigator!.viewModel.PageSizeChangedCommand = navigator.PageSizeChangedCommand;
            }));

        public object? PageSizeChangedCommandParameter
        {
            get { return (object?)GetValue(PageSizeChangedCommandParameterProperty); }
            set { SetValue(PageSizeChangedCommandParameterProperty, value); }
        }

        // Using a DependencyProperty as the backing store for PageSizeChangedCommandParameter.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PageSizeChangedCommandParameterProperty =
            DependencyProperty.Register(nameof(PageSizeChangedCommandParameter), typeof(object), typeof(Navigator), new PropertyMetadata((d, e) =>
            {
                var navigator = d as Navigator;
                navigator!.viewModel.PageSizeChangedCommandParameter = navigator.PageSizeChangedCommandParameter;
            }));

        //public ICommand Command
        //{
        //    get { return (ICommand)GetValue(CommandProperty); }
        //    set { SetValue(CommandProperty, value); }
        //}

        //// Using a DependencyProperty as the backing store for Command.  This enables animation, styling, binding, etc...
        //public static readonly DependencyProperty CommandProperty =
        //    DependencyProperty.Register("Command", typeof(ICommand), typeof(Navigator));

        #endregion 依赖属性

        #region 路由事件

        /// <summary>
        /// 标识 CurrentPageNoChanging 路由事件。
        /// </summary>
        public static readonly RoutedEvent CurrentPageNoChangingRoutedEvent =
            EventManager.RegisterRoutedEvent(nameof(CurrentPageNoChanging), RoutingStrategy.Direct, typeof(PropertyChangingRoutedEventHandler), typeof(Navigator));

        /// <summary>
        /// 在当前页号CurrentPageNo改变中发生。
        /// </summary>
        public event PropertyChangingRoutedEventHandler CurrentPageNoChanging
        {
            add { AddHandler(CurrentPageNoChangingRoutedEvent, value); }
            remove { RemoveHandler(CurrentPageNoChangingRoutedEvent, value); }
        }

        /// <summary>
        /// 标识 CurrentPageNoChanged 路由事件。
        /// </summary>
        public static readonly RoutedEvent CurrentPageNoChangedRoutedEvent =
            EventManager.RegisterRoutedEvent(nameof(CurrentPageNoChanged), RoutingStrategy.Direct, typeof(PropertyChangedRoutedEventHandler), typeof(Navigator));

        /// <summary>
        /// 在当前页号CurrentPageNo改变后发生。
        /// </summary>
        public event PropertyChangedRoutedEventHandler CurrentPageNoChanged
        {
            add { AddHandler(CurrentPageNoChangedRoutedEvent, value); }
            remove { RemoveHandler(CurrentPageNoChangedRoutedEvent, value); }
        }

        /// <summary>
        /// 标识 PageSizeChanging 路由事件。
        /// </summary>
        public static readonly RoutedEvent PageSizeChangingRoutedEvent =
            EventManager.RegisterRoutedEvent(nameof(PageSizeChanging), RoutingStrategy.Direct, typeof(PropertyChangingRoutedEventHandler), typeof(Navigator));

        /// <summary>
        /// 在当前页号 PageSizeChanging 改变中发生。
        /// </summary>
        public event PropertyChangingRoutedEventHandler PageSizeChanging
        {
            add { AddHandler(PageSizeChangingRoutedEvent, value); }
            remove { RemoveHandler(PageSizeChangingRoutedEvent, value); }
        }

        /// <summary>
        /// 标识 PageSizeChanged 路由事件。
        /// </summary>
        public static readonly RoutedEvent PageSizeChangedRoutedEvent =
            EventManager.RegisterRoutedEvent(nameof(PageSizeChanged), RoutingStrategy.Direct, typeof(PropertyChangedRoutedEventHandler), typeof(Navigator));

        /// <summary>
        /// 在当前页号CurrentPageNo改变后发生。
        /// </summary>
        public event PropertyChangedRoutedEventHandler PageSizeChanged
        {
            add { AddHandler(PageSizeChangedRoutedEvent, value); }
            remove { RemoveHandler(PageSizeChangedRoutedEvent, value); }
        }

        #endregion 路由事件
    }
}
