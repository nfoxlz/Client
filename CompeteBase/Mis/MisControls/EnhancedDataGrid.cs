// ======================================================
// Compete Management Information System
// ======================================================
// 版权所有 © Compete software studio 2018 保留所有权利。
// ------------------------------------------------------
// 版本    日期时间               作者     说明
// ------------------------------------------------------
// 1.0.0.0 2018/9/2 周日 14:51:17 LeeZheng 新建。
// ======================================================
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
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
    ///     xmlns:MyNamespace="clr-namespace:Compete.Mis.Controls"
    ///
    ///
    /// 步骤 1b) 在其他项目中存在的 XAML 文件中使用该自定义控件。
    /// 将此 XmlNamespace 特性添加到要使用该特性的标记文件的根 
    /// 元素中: 
    ///
    ///     xmlns:MyNamespace="clr-namespace:Compete.Mis.Controls;assembly=Compete.Mis.Controls"
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
    ///     <MyNamespace:EnhancedDataGrid/>
    ///
    /// </summary>
    public class EnhancedDataGrid : DataGrid
    {
        /// <summary>
        /// 初始化 EnhancedDataGrid 的静态成员。
        /// </summary>
        //static EnhancedDataGrid()
        //{
        //    DefaultStyleKeyProperty.OverrideMetadata(typeof(EnhancedDataGrid), new FrameworkPropertyMetadata(typeof(EnhancedDataGrid)));
        //}

        public EnhancedDataGrid()
        {
            CurrentCellChanged += EnhancedDataGrid_CurrentCellChanged;
            BeginningEdit += EnhancedDataGrid_BeginningEdit;
            PreviewKeyDown += EnhancedDataGrid_PreviewKeyDown;
            MouseDoubleClick += EnhancedDataGrid_MouseDoubleClick;

            _ = new DataGridDecorator(this);
        }

        private void EnhancedDataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e) => Command?.Execute(CommandParameter);

        private void EnhancedDataGrid_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (EditIndexes != null)
            {
                var count = EditIndexes.Count;
                if (count > 0)
                {
                    var displayIndex = EditIndexes.IndexOf(CurrentCell.Column.DisplayIndex);
                    if (displayIndex >= 0)
                    {
                        int selectedIndex;
                        if (e.Key == Key.Tab)
                        {
                            selectedIndex = SelectedIndex;
                            if (displayIndex + 1 < count)
                                displayIndex++;
                            else
                                displayIndex = 0;

                        }
                        else if (e.Key == Key.Enter)
                        {
                            selectedIndex = SelectedIndex + 1;
                            displayIndex = 0;
                        }
                        else
                            return;

                        var row = (DataGridRow)ItemContainerGenerator.ContainerFromIndex(selectedIndex);
                        if (row != null)
                        {
                            DataGridCell? cell = GetCell(row, EditIndexes[displayIndex]); // columnIndex是你想要聚焦的列的索引
                            if (cell != null)
                            {
                                cell.Focus();
                                e.Handled = true;
                            }
                        }
                    }
                }
            }
        }

        private DataGridCell? GetCell(DataGridRow row, int columnIndex)
        {
            if (row != null)
            {
                DataGridCellsPresenter? presenter = FindVisualChild<DataGridCellsPresenter>(row);
                if (presenter == null)
                {
                    row.ApplyTemplate(); // 强制生成模板
                    presenter = FindVisualChild<DataGridCellsPresenter>(row);
                }
                if (presenter != null && presenter.ItemContainerGenerator.Status == GeneratorStatus.ContainersGenerated)
                    return presenter.ItemContainerGenerator.ContainerFromIndex(columnIndex) as DataGridCell;
            }

            return null;
        }

        private T? FindVisualChild<T>(DependencyObject parent) where T : DependencyObject
        {
            var count = VisualTreeHelper.GetChildrenCount(parent);
            for (int i = 0; i < count; i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);
                if (child is T tChild)
                    return tChild;
                var result = FindVisualChild<T>(child);
                if (result != null)
                    return result;
            }
            return null;
        }

        private void EnhancedDataGrid_BeginningEdit(object? sender, DataGridBeginningEditEventArgs e) => IsEditing = true;

        private void EnhancedDataGrid_CurrentCellChanged(object? sender, EventArgs e)
        {
            if (IsEditing)
            {
                IsEditing = false;

                if (!CommitEdit(DataGridEditingUnit.Row, true))
                    throw new Exceptions.PlatformException(GlobalCommon.GetMessage("Exception.DataCannotSubmit"));
                Items.Refresh();
            }
        }

        internal bool IsEditing { get; set; }

        public bool IsPaging
        {
            get { return (bool)GetValue(IsPagingProperty); }
            set { SetValue(IsPagingProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsPaging.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsPagingProperty =
            DependencyProperty.Register(nameof(IsPaging), typeof(bool), typeof(EnhancedDataGrid), new PropertyMetadata(false));

        public ICommand RefreshCommand
        {
            get { return (ICommand)GetValue(RefreshCommandProperty); }
            set { SetValue(RefreshCommandProperty, value); }
        }

        // Using a DependencyProperty as the backing store for RefreshCommand.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty RefreshCommandProperty =
            DependencyProperty.Register(nameof(RefreshCommand), typeof(ICommand), typeof(EnhancedDataGrid));

        public string SortDescription
        {
            get { return (string)GetValue(SortDescriptionProperty); }
            set { SetValue(SortDescriptionProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SortDescription.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SortDescriptionProperty =
            DependencyProperty.Register(nameof(SortDescription), typeof(string), typeof(EnhancedDataGrid));

        public IList<int> EditIndexes
        {
            get { return (IList<int>)GetValue(EditIndexesProperty); }
            set { SetValue(EditIndexesProperty, value); }
        }

        // Using a DependencyProperty as the backing store for EditIndexes.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty EditIndexesProperty =
            DependencyProperty.Register(nameof(EditIndexes), typeof(IList<int>), typeof(EnhancedDataGrid));

        public string EditIndexesString
        {
            get { return (string)GetValue(EditIndexesStringProperty); }
            set { SetValue(EditIndexesStringProperty, value); }
        }

        // Using a DependencyProperty as the backing store for EditIndexesString.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty EditIndexesStringProperty =
            DependencyProperty.Register(nameof(EditIndexesString), typeof(string), typeof(EnhancedDataGrid), new PropertyMetadata((d, e) =>
            {
                var enhancedDataGrid = (EnhancedDataGrid)d;
                if (!string.IsNullOrWhiteSpace(enhancedDataGrid.EditIndexesString))
                {
                    var list = new List<int>();
                    var indexes = enhancedDataGrid.EditIndexesString.Split(',');
                    foreach (var i in indexes)
                        list.Add(int.Parse(i));
                    enhancedDataGrid.EditIndexes = list;
                }
            }));

        public ICommand Command
        {
            get { return (ICommand)GetValue(CommandProperty); }
            set { SetValue(CommandProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Command.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CommandProperty =
            DependencyProperty.Register(nameof(Command), typeof(ICommand), typeof(EnhancedDataGrid));

        public object CommandParameter
        {
            get { return (object)GetValue(CommandParameterProperty); }
            set { SetValue(CommandParameterProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CommandParameter.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CommandParameterProperty =
            DependencyProperty.Register(nameof(CommandParameter), typeof(object), typeof(EnhancedDataGrid));

        public IInputElement CommandTarget
        {
            get { return (IInputElement)GetValue(CommandTargetProperty); }
            set { SetValue(CommandTargetProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CommandTarget.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CommandTargetProperty =
            DependencyProperty.Register(nameof(CommandTarget), typeof(IInputElement), typeof(EnhancedDataGrid));
    }
}
