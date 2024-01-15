using Compete.Extensions;
using System;
using System.Collections.Generic;
using System.Data;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;

namespace Compete.Mis.MisControls
{
    /// <summary>
    /// 按照步骤 1a 或 1b 操作，然后执行步骤 2 以在 XAML 文件中使用此自定义控件。
    ///
    /// 步骤 1a) 在当前项目中存在的 XAML 文件中使用该自定义控件。
    /// 将此 XmlNamespace 特性添加到要使用该特性的标记文件的根
    /// 元素中:
    ///
    ///     xmlns:MyNamespace="clr-namespace:Compete.Mis.MisControls"
    ///
    ///
    /// 步骤 1b) 在其他项目中存在的 XAML 文件中使用该自定义控件。
    /// 将此 XmlNamespace 特性添加到要使用该特性的标记文件的根
    /// 元素中:
    ///
    ///     xmlns:MyNamespace="clr-namespace:Compete.Mis.MisControls;assembly=Compete.Mis.MisControls"
    ///
    /// 您还需要添加一个从 XAML 文件所在的项目到此项目的项目引用，
    /// 并重新生成以避免编译错误:
    ///
    ///     在解决方案资源管理器中右击目标项目，然后依次单击
    ///     “添加引用”->“项目”->[浏览查找并选择此项目]
    ///
    ///
    /// 步骤 2)
    /// 继续操作并在 XAML 文件中使用控件。
    ///
    ///     <MyNamespace:DataTableRowGroup/>
    ///
    /// </summary>
    public class DataTableRowGroup : TableRowGroup
    {
        //static DataTableRowGroup()
        //{
        //    DefaultStyleKeyProperty.OverrideMetadata(typeof(DataTableRowGroup), new FrameworkPropertyMetadata(typeof(DataTableRowGroup)));
        //}

        public IEnumerable<string> ShowColumns
        {
            get { return (IEnumerable<string>)GetValue(ShowColumnsProperty); }
            set { SetValue(ShowColumnsProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ShowColumns.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ShowColumnsProperty =
            DependencyProperty.Register("ShowColumns", typeof(IEnumerable<string>), typeof(DataTableRowGroup), new PropertyMetadata((d, e) => ((DataTableRowGroup)d).CreateRow()));

        public object ItemsSource
        {
            get { return (object)GetValue(ItemsSourceProperty); }
            set { SetValue(ItemsSourceProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ItemsSource.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ItemsSourceProperty =
            DependencyProperty.Register("ItemsSource", typeof(object), typeof(DataTableRowGroup), new PropertyMetadata((d, e) => ((DataTableRowGroup)d).CreateRow()));

        //private static void CreateRow(DependencyObject d, DependencyPropertyChangedEventArgs e) => ((DataTableRowGroup)d).CreateRow();

        private void CreateRow()
        {
            if (ShowColumns == null)
                return;

            var source = ItemsSource ?? DataContext;
            if (source == null)
                return;

            DataView view;
            if (source is DataTable table)
                view = table.DefaultView;
            else if (source is DataView dataView)
                view = dataView;
            else
                return;

            var columns = view.Table!.Columns;

            string? format;
            string? val;
            TextAlignment textAlignment;
            DataColumn dataColumn;
            Rows.Clear();
            foreach (DataRowView rowView in view)
            {
                var row = new TableRow();
                foreach (var column in ShowColumns)
                {
                    if (!columns.Contains(column))
                        continue;

                    dataColumn = columns[column]!;

                    format = dataColumn.ExtendedProperties.ContainsKey(MemoryData.ExtendedPropertyNames.ShowFormat) ? dataColumn.ExtendedProperties[MemoryData.ExtendedPropertyNames.ShowFormat]?.ToString()
                        : dataColumn.ExtendedProperties.ContainsKey(MemoryData.ExtendedPropertyNames.Format) ? dataColumn.ExtendedProperties[MemoryData.ExtendedPropertyNames.Format]?.ToString()
                        : null;

                    textAlignment = dataColumn.DataType.IsNumeric() ? TextAlignment.Right : TextAlignment.Left;
                    if (rowView.Row[column] is DBNull)
                        val = string.Empty;
                    else if (format == null)
                        val = rowView.Row[column].ToString();
                    else if (textAlignment == TextAlignment.Right)
                        val = Convert.ToDecimal(rowView.Row[column]).ToString(format);
                    else if (dataColumn.DataType == typeof(DateTime))
                        val = Convert.ToDateTime(rowView.Row[column]).ToString(format);
                    else
                        val = rowView.Row[column].ToString();

                    row.Cells.Add(new TableCell(new Paragraph(new Run(val)))
                    {
                        BorderThickness = new Thickness(0.5),
                        BorderBrush = Brushes.Black,
                        Padding = new Thickness(3),
                        TextAlignment = textAlignment,
                    });
                }
                Rows.Add(row);
            }
        }
    }
}
