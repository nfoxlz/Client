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
using System.Windows.Controls;

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

            _ = new DataGridDecorator(this);
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
    }
}
