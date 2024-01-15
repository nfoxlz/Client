// ======================================================
// XXX项目
// ======================================================
// 版权所有 © Compete software studio 2019 保留所有权利。
// ------------------------------------------------------
// 版本    日期时间            作者     说明
// ------------------------------------------------------
// 1.0.0.0 2019/4/19 周五 8:41:22 LeeZheng 新建。
// ======================================================
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;

namespace Compete.Mis.MisControls
{
    ///// <summary>
    ///// 按照步骤 1a 或 1b 操作，然后执行步骤 2 以在 XAML 文件中使用此自定义控件。
    /////
    ///// 步骤 1a) 在当前项目中存在的 XAML 文件中使用该自定义控件。
    ///// 将此 XmlNamespace 特性添加到要使用该特性的标记文件的根 
    ///// 元素中: 
    /////
    /////     xmlns:MyNamespace="clr-namespace:Compete.Mis.Controls"
    /////
    /////
    ///// 步骤 1b) 在其他项目中存在的 XAML 文件中使用该自定义控件。
    ///// 将此 XmlNamespace 特性添加到要使用该特性的标记文件的根 
    ///// 元素中: 
    /////
    /////     xmlns:MyNamespace="clr-namespace:Compete.Mis.Controls;assembly=Compete.Mis.Controls"
    /////
    ///// 您还需要添加一个从 XAML 文件所在的项目到此项目的项目引用，
    ///// 并重新生成以避免编译错误: 
    /////
    /////     在解决方案资源管理器中右击目标项目，然后依次单击
    /////     “添加引用”->“项目”->[浏览查找并选择此项目]
    /////
    /////
    ///// 步骤 2)
    ///// 继续操作并在 XAML 文件中使用控件。
    /////
    /////     <MyNamespace:DataPanelBase/>
    /////
    ///// </summary>

    /// <summary>
    /// DataPanelBase 类。
    /// </summary>
    public abstract class DataPanelBase : Grid
    {
        private static readonly string dateTimeStringFormat = GlobalCommon.GetMessage("DateTimeStringFormat");

        /// <summary>
        /// 静态构造方法。
        /// </summary>
        static DataPanelBase() => DefaultStyleKeyProperty.OverrideMetadata(typeof(DataPanelBase), new FrameworkPropertyMetadata(typeof(Grid)));   // 样式与 Grid 控件一致。

        /// <summary>
        /// 初始化 <see cref="DataPanelBase"/> 类的新实例。
        /// </summary>
        public DataPanelBase() => DataContextChanged += DataPanelBase_DataContextChanged;

        /// <summary>
        /// DataContext 属性改变事件处理方法。
        /// </summary>
        /// <param name="sender">事件源。</param>
        /// <param name="e">事件数据。</param>
        private void DataPanelBase_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e) => CreateItems();

        /// <summary>
        /// 获取或设置一个值，用于指示生成的每个表示题控件的宽度。
        /// </summary>
        public double TitleWidth
        {
            get { return (double)GetValue(TitleWidthProperty); }
            private set { SetValue(TitleWidthProperty, value); }
        }

        // Using a DependencyProperty as the backing store for TitleWidth.  This enables animation, styling, binding, etc...
        /// <summary>
        /// 标识 TitleWidth 的依赖属性。
        /// </summary>
        public static readonly DependencyProperty TitleWidthProperty =
            DependencyProperty.Register(nameof(TitleWidth), typeof(double), typeof(DataPanelBase), new PropertyMetadata(-1D));

        /// <summary>
        /// 获取或设置用于生成 DataPanelBase 的内容的集合。
        /// </summary>
        public object ItemsSource
        {
            get { return GetValue(ItemsSourceProperty); }
            set { SetValue(ItemsSourceProperty, value); }
        }

        /// <summary>
        /// 标识 ItemsSource 的依赖属性。
        /// </summary>
        public static readonly DependencyProperty ItemsSourceProperty =
            DependencyProperty.Register(nameof(ItemsSource), typeof(object), typeof(DataPanelBase), new PropertyMetadata((d, e) => ((DataPanelBase)d).CreateItems()));

        /// <summary>
        /// 创建项。每一项包括一个标题控件与一个编辑或显示控件。
        /// </summary>
        private void CreateItems()
        {
            var source = ItemsSource ?? DataContext;
            if (source is CollectionView collectionView)
                source = collectionView.SourceCollection;

            DataColumnCollection columns;

            if (source is DataTable table)
                columns = table.Columns;
            else if (source is DataView view)
                columns = view.Table!.Columns;
            else if (source is DataRowView rowView)
                columns = rowView.DataView.Table!.Columns;
            else if (source is DataRow row)
                columns = row.Table.Columns;
            else
                return;

            TitleWidth = 0D;
            foreach (DataColumn column in columns)
                if (!column.ExtendedProperties.ContainsKey(MemoryData.ExtendedPropertyNames.IsVisible) || Convert.ToBoolean(column.ExtendedProperties[MemoryData.ExtendedPropertyNames.IsVisible]))
                    TitleWidth = Math.Max(TitleWidth, CharWidthHelper.GetStringWidth(column.Caption));
            TitleWidth += 10D;

            CreateDataItems(columns);
        }

        /// <summary>
        /// 依据数据列集合创建数据项。每一项包括一个标题控件与一个编辑或显示控件。
        /// </summary>
        /// <param name="columns">所依据的数据列集合。</param>
        private void CreateDataItems(DataColumnCollection columns)
        {
            Children.Clear();           // 清除原有控件。

            FrameworkElement? element;   // 编辑或显示控件。
            TextBlock titleControl;     // 标题控件。
            double top = 2D;            // 数据项显示的顶部位置。
            int columnIndex = 0;
            var columnList = new List<DataColumn>();
            int displayIndex;
            foreach(DataColumn column in columns)
            {
                displayIndex = Convert.ToInt32(column.ExtendedProperties[MemoryData.ExtendedPropertyNames.DisplayIndex] ?? -1);
                if (displayIndex < 0)
                    columnList.Add(column);
                else
                    columnList.Insert(displayIndex, column);
            }
            foreach (DataColumn column in columnList)
            {
                if (column.ExtendedProperties.ContainsKey(MemoryData.ExtendedPropertyNames.IsVisible) && !Convert.ToBoolean(column.ExtendedProperties[MemoryData.ExtendedPropertyNames.IsVisible]))
                    continue;   // 不可视，跳过。

                // 生成标题控件。
                titleControl = new TextBlock
                {
                    Text = column.Caption,
                    Margin = new Thickness(0D, top + 3D, 0D, 0D),
                    HorizontalAlignment = HorizontalAlignment.Left,
                    VerticalAlignment = VerticalAlignment.Top,
                    Width = TitleWidth - 5D,
                    TextAlignment = TextAlignment.Right
                };
                var foreground = GetTitleForeground(column);
                if (foreground != null)
                    titleControl.Foreground = foreground;
                SetColumn(titleControl, columnIndex);
                Children.Add(titleControl);

                // 生成编辑或显示控件。
                element = CreateElement(column);
                if (element == null)
                    continue;
                element.Margin = new Thickness(TitleWidth, top, 0D, 0D);
                element.VerticalAlignment = VerticalAlignment.Top;

                SetColumn(element, columnIndex);
                Children.Add(element);

                // 计算位置。
                columnIndex++;
                if (columnIndex >= ColumnDefinitions.Count)
                {
                    columnIndex = 0;
                    top += 28D;
                }
            }
        }

        protected virtual Brush? GetTitleForeground(DataColumn column) => null;

        protected Binding GetBinding(DataColumn column)
        {
            var binding = new Binding(column.ColumnName)
            {
                Source = ItemsSource ?? DataContext,
                Mode = column.ReadOnly || Convert.ToBoolean(column.ExtendedProperties[MemoryData.ExtendedPropertyNames.IsReadOnly]) ? BindingMode.OneWay : BindingMode.TwoWay
            };  // 创建数据绑定。

            var dataType = column.DataType;
            var format = column.ExtendedProperties[MemoryData.ExtendedPropertyNames.Format]?.ToString();   // 绑定格式。
            if (!string.IsNullOrWhiteSpace(format))
                binding.StringFormat = format;
            else if (dataType == typeof(DateTime) || dataType == typeof(DateTimeOffset))
                binding.StringFormat = dateTimeStringFormat;

            return binding;
        }

        protected abstract FrameworkElement? CreateElement(DataColumn column);
    }
}
