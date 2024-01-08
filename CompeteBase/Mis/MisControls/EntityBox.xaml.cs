// ===================================================================
// Compete Management Information System
// ===================================================================
// 版权所有 © Compete software studio 2018 保留所有权利。
// -------------------------------------------------------------------
// 版本    日期时间            作者     说明
// -------------------------------------------------------------------
// 1.0.0.0 2018/3/12 8:36:57 LeeZheng 新建。
//==============================================================
using Compete.Extensions;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

namespace Compete.Mis.MisControls
{
    /// <summary>
    /// EntityBox.xaml 的交互逻辑
    /// </summary>
    public partial class EntityBox : UserControl
    {
        private MethodInfo? formatMethod;

        /// <summary>
        /// 获取选择命令。
        /// </summary>
        public ICommand Select { get; private set; } = new RoutedUICommand();

        public ICommand Clear { get; private set; } = new RoutedUICommand();

        /// <summary>
        /// 初始化 <see cref="EntityBox"/> 类的新实例。
        /// </summary>
        public EntityBox()
        {
            InitializeComponent();

            SelectButton.Command = Select;
            ClearButton.Command = Clear;
            CommandBindings.Add(new CommandBinding(Select, SelectCommandHandler, SelectCanExecuteHandler));
            CommandBindings.Add(new CommandBinding(Clear, ClearCommandHandler));
        }

        private DataRow? GetSourceRow()
        {
            var bindingExpression = GetBindingExpression(ValueProperty);
            if (bindingExpression == null)
                return null;

            var source = bindingExpression.ResolvedSource ?? bindingExpression.ParentBinding.Source;

            if (source is BindingListCollectionView collectionView)
                source = collectionView.CurrentItem ?? collectionView.SourceCollection;

            if (source is DataView view)
                return view.Count > 0 ? view[0].Row : null;
            else if (source is DataTable table)
                return table.Rows.Count > 0 ? table.Rows[0] : null;
            else if (source is DataRowView rowView)
                return rowView.Row;
            else if (source is DataRow row)
                return row;

            return null;
        }

        /// <summary>
        /// Select 命令的Executed事件的处理程序。 
        /// </summary>
        /// <param name="sender">事件处理程序所附加到的对象。</param>
        /// <param name="e">事件数据。</param>
        private void SelectCommandHandler(object sender, ExecutedRoutedEventArgs e)
        {
            IsEnabled = false;

            try
            {
                var dialog = new EntitySelectDialog() { Title = GlobalCommon.GetMessage("EntitySelectDialog.Title", EntityName) };

                var viewModel = (EntitySelectViewModel)dialog.DataContext;
                viewModel.Conditions = GetSourceRow()?.ToDictionary();
                //viewModel.FilterFormat = FilterFormat ?? "(Code LIKE '{0}%' OR Name LIKE '%{0}%' OR MnemonicCode LIKE '%{0}%' OR Barcode = '{0}')";
                viewModel.ServiceParameter = ServiceParameter;
                viewModel.IsRequired = IsRequired;
                viewModel.QueryData();

                _ = new DataGridDecorator(dialog.MainDataGrid);    // 生成DataGrid装饰器。

                if (dialog.ShowDialog() == true)
                    if (viewModel.SelectedItem is DataRowView view)
                        SetEntityValue(view.Row);
                    else if (viewModel.SelectedItem is DataRow row)
                        SetEntityValue(row);
                    else
                        SetEntityValue(viewModel.SelectedItem);
            }
            finally
            {
                IsEnabled = true;
            }
        }

        /// <summary>
        /// Clear 命令的Executed事件的处理程序。 
        /// </summary>
        /// <param name="sender">事件处理程序所附加到的对象。</param>
        /// <param name="e">事件数据。</param>
        private void ClearCommandHandler(object sender, ExecutedRoutedEventArgs e)
        {
            isSetValue = true;
            if (Value is long)
                Value = 0L;
            else
                Value = DBNull.Value;
            isSetValue = false;
            DisplayTextBox.Text = string.Empty;
        }

        /// <summary>
        /// 是否正在设置 <see cref="Value"/> 属性。
        /// </summary>
        private bool isSetValue;

        /// <summary>
        /// 设置 <see cref="Value"/> 属性，及对应的显示内容。
        /// </summary>
        /// <param name="item">选定的源数据项。</param>
        /// <remarks>
        /// 要求 <see cref="ValuePath"/> 属性与 <see cref="DisplayPath"/> 属性在使用前设置，如未设置，将为无效操作。
        /// </remarks>
        private void SetEntityValue(object? item)
        {
            isSetValue = true;
            if (item == null)
                Value = DBNull.Value;
            else if (item.HasProperty(ValuePath))
                Value = item.GetPropertyValue(ValuePath);
            else if (item.HasProperty("Id"))
                Value = item.GetPropertyValue("Id");
            isSetValue = false;

            if (item == null)
                DisplayTextBox.Text = string.Empty;
            else if (item.HasProperty(DisplayPath))
                DisplayTextBox.Text = item.GetPropertyValue(DisplayPath)!.ToString();
        }

        private static readonly string[] omittedColumns = ["Tenant_Id", "Is_Deleted", "Is_Active", "Creator_User_Id", "Creation_Date_Time", "Last_Modifier_User_Id", "Last_Modification_Date_Time", "Deleter_User_Id", "Deletion_Date_Time", "Version", "Comment"];
        //"No", "Id", "Code", "Name", "Mnemonic_Code", "Barcode", "Description", 

        /// <summary>
        /// 设置 <see cref="Value"/> 属性，及对应的显示内容。
        /// </summary>
        /// <param name="row">选定的源数据记录。</param>
        /// <remarks>
        /// 要求 <see cref="ValuePath"/> 属性与 <see cref="DisplayPath"/> 属性在使用前设置，如未设置，将为无效操作。
        /// </remarks>
        private void SetEntityValue(DataRow row)
        {
            if (IsCopySameName)
            {
                var sourceRow = GetSourceRow();
                if (sourceRow != null)
                    row.CopyTo(sourceRow, omittedColumns.Merge(new string[] { string.IsNullOrWhiteSpace(ValuePath) ? "Id" : ValuePath, string.IsNullOrWhiteSpace(DisplayPath) ? "Name" : DisplayPath }));
            }

            var columns = row.Table.Columns;

            if (!string.IsNullOrWhiteSpace(Format))
                DisplayTextBox.Text = formatMethod?.Invoke(null, new object[] { row })?.ToString();
            else if (columns.Contains(DisplayPath))
                DisplayTextBox.Text = row[DisplayPath].ToString();

            isSetValue = true;
            if (columns.Contains(ValuePath))
                Value = row[ValuePath];
            else if (columns.Contains("Id"))
                Value = row["Id"];
            isSetValue = false;

            if (IsCopySameName && Tag is TagData { Data: DataGrid grid })
            {
                if (!grid.CommitEdit(DataGridEditingUnit.Row, true))
                    throw new Exceptions.PlatformException(GlobalCommon.GetMessage("Exception.DataCannotSubmit"));
                grid.Items.Refresh();

                if (grid is EnhancedDataGrid enhancedDataGrid)
                    enhancedDataGrid.IsEditing = false;
            }

            if (Value == null || Value is long longVal && longVal == 0L || Value is Guid guidVal && guidVal == Guid.Empty || Value is string stringVal && string.IsNullOrWhiteSpace(stringVal))
                DisplayTextBox.Text = string.Empty;

            //var columns = row.Table.Columns;
            //isSetValue = true;
            //if (columns.Contains(ValuePath))
            //    Value = row[ValuePath];
            //else if (columns.Contains("Id"))
            //    Value = row["Id"];
            //isSetValue = false;

            //if (IsCopySameName)
            //{
            //    var sourceRow = GetSourceRow();

            //    if (sourceRow != null)
            //    {
            //        var oldValue = Value;
            //        row.CopyTo(sourceRow, omittedColumns);
            //        //sourceRow.EndEdit();
            //        Value = oldValue;

            //        if ((Tag as TagData)?.Data is DataGrid grid)
            //        {
            //            grid.CommitEdit(DataGridEditingUnit.Row, true);
            //            grid.Items.Refresh();

            //            if (grid is EnhancedDataGrid enhancedDataGrid)
            //                enhancedDataGrid.IsEditing = false;
            //        }
            //    }
            //}
        }

        /// <summary>
        /// Select 命令的CanExecute事件的处理程序。 
        /// </summary>
        /// <param name="sender">调用处理程序的命令目标。</param>
        /// <param name="e">事件数据。</param>
        private void SelectCanExecuteHandler(object sender, CanExecuteRoutedEventArgs e) => e.CanExecute = !string.IsNullOrWhiteSpace(ServiceParameter);

        #region 依赖属性

        /// <summary>
        /// 获取或设置控件的值。
        /// </summary>
        public object? Value
        {
            get { return GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }

        /// <summary>
        /// 标识 Value 的依赖属性。
        /// </summary>
        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register("Value", typeof(object), typeof(EntityBox), new PropertyMetadata(new PropertyChangedCallback(OnValueChanged)));

        private object? oldValue;

        /// <summary>
        /// Value 依赖项属性更变的回调方法。
        /// </summary>
        /// <param name="d">属性已更改值的 DependencyObject 。</param>
        /// <param name="node">由所有事件跟踪问题到该属性的有效值的更改事件数据。</param>
        private static void OnValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var entityBox = (EntityBox)d;
            if (entityBox.isSetValue || entityBox.Value != null && entityBox.Value.Equals(entityBox.oldValue) || string.IsNullOrWhiteSpace(entityBox.ServiceParameter) || string.IsNullOrWhiteSpace(entityBox.DisplayPath))
                return;

            // 取得需要从UI线程向新线程传递的数据。
            var serviceParameter = entityBox.ServiceParameter;  // 服务参数。
            var entityBoxValue = entityBox.Value;               // 控件的值。

            if (entityBoxValue == null || string.IsNullOrWhiteSpace(entityBoxValue.ToString()) || entityBoxValue is long val && val == 0L)
            {
                entityBox.DisplayTextBox.Text = string.Empty;
                return;
            }

            var entity = MisThreading.ThreadingHelper.Invoke(() => GlobalCommon.EntityDataProvider!.GetEntity(serviceParameter, entityBoxValue), "Query");
            if (entity == null || entity.Rows.Count == 0 || !entity.Columns.Contains(entityBox.DisplayPath))
                return;

            if (string.IsNullOrWhiteSpace(entityBox.Format) || entityBox.formatMethod == null)
                entityBox.DisplayTextBox.Text = entity.Rows[0][entityBox.DisplayPath].ToString();
            else
                entityBox.DisplayTextBox.Text = entityBox.formatMethod?.Invoke(null, new object[] { entity.Rows[0] })?.ToString();

            entityBox.oldValue = entityBox.Value;
        }

        /// <summary>
        /// 获取或设置源对象上的值的路径，以用作对象的值。
        /// </summary>
        public string ValuePath
        {
            get { return (string)GetValue(ValuePathProperty); }
            set { SetValue(ValuePathProperty, value); }
        }

        /// <summary>
        /// 标识 ValuePath 的依赖属性。
        /// </summary>
        public static readonly DependencyProperty ValuePathProperty =
            DependencyProperty.Register("ValuePath", typeof(string), typeof(EntityBox));

        /// <summary>
        /// 获取或设置源对象上的值的路径，以用作对象的可视表示形式。
        /// </summary>
        public string DisplayPath
        {
            get { return (string)GetValue(DisplayPathProperty); }
            set { SetValue(DisplayPathProperty, value); }
        }

        /// <summary>
        /// 标识 DisplayPath 的依赖属性。
        /// </summary>
        public static readonly DependencyProperty DisplayPathProperty =
            DependencyProperty.Register("DisplayPath", typeof(string), typeof(EntityBox));

        /// <summary>
        /// 获取或设置实体的名称。
        /// </summary>
        public string EntityName
        {
            get { return (string)GetValue(EntityNameProperty); }
            set { SetValue(EntityNameProperty, value); }
        }

        /// <summary>
        /// 标识 EntityName 的依赖属性。
        /// </summary>
        public static readonly DependencyProperty EntityNameProperty =
            DependencyProperty.Register(nameof(EntityName), typeof(string), typeof(EntityBox));

        /// <summary>
        /// 获取或设置服务参数。
        /// </summary>
        public string ServiceParameter
        {
            get { return (string)GetValue(ServiceParameterProperty); }
            set { SetValue(ServiceParameterProperty, value); }
        }

        /// <summary>
        /// 标识 ServiceParameter 的依赖属性。
        /// </summary>
        public static readonly DependencyProperty ServiceParameterProperty =
            DependencyProperty.Register(nameof(ServiceParameter), typeof(string), typeof(EntityBox), new PropertyMetadata(new PropertyChangedCallback(OnServiceParameterChanged)));

        /// <summary>
        /// ServiceParameter 依赖项属性更变的回调方法。
        /// </summary>
        /// <param name="d">属性已更改值的 DependencyObject 。</param>
        /// <param name="node">由所有事件跟踪问题到该属性的有效值的更改事件数据。</param>
        private static void OnServiceParameterChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var entityBox = (EntityBox)d;
            var path = Path.ChangeExtension(Path.Combine(Utils.PathHelper.PluginPath, "EntitySelector", entityBox.ServiceParameter), "json");
            if (File.Exists(path))
            {
                var setting = JsonSerializer.Deserialize<EntitySelectorSetting>(File.ReadAllText(path)); 
                if (string.IsNullOrWhiteSpace(entityBox.Format) && !string.IsNullOrWhiteSpace(setting?.Format))
                    entityBox.Format = setting.Format;
                if (string.IsNullOrWhiteSpace(entityBox.FilterFormat) && !string.IsNullOrWhiteSpace(setting?.FilterFormat))
                    entityBox.FilterFormat = setting.FilterFormat;
            }
        }

        /// <summary>
        /// 获取或设置一个值，指示控件是否只读。
        /// </summary>
        public bool IsReadOnly
        {
            get { return (bool)GetValue(IsReadOnlyProperty); }
            set { SetValue(IsReadOnlyProperty, value); }
        }

        /// <summary>
        /// 标识 IsReadOnly 的依赖属性。
        /// </summary>
        public static readonly DependencyProperty IsReadOnlyProperty =
            DependencyProperty.Register("IsReadOnly", typeof(bool), typeof(EntityBox), new PropertyMetadata(new PropertyChangedCallback(OnIsReadOnlyChanged)));

        /// <summary>
        /// IsReadOnly 依赖项属性更变的回调方法。
        /// </summary>
        /// <param name="d">属性已更改值的 DependencyObject 。</param>
        /// <param name="node">由所有事件跟踪问题到该属性的有效值的更改事件数据。</param>
        private static void OnIsReadOnlyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var entityBox = (EntityBox)d;
            entityBox.SelectButton.Visibility = entityBox.IsReadOnly ? Visibility.Collapsed : Visibility.Visible;
            entityBox.ClearButton.Visibility = entityBox.IsRequired || entityBox.IsReadOnly ? Visibility.Collapsed : Visibility.Visible;
        }

        /// <summary>
        /// 获取或设置一个值，指示是否是必填项。
        /// </summary>
        public bool IsRequired
        {
            get { return (bool)GetValue(IsRequiredProperty); }
            set { SetValue(IsRequiredProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsRequired.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsRequiredProperty =
            DependencyProperty.Register(nameof(IsRequired), typeof(bool), typeof(EntityBox), new PropertyMetadata(false, new PropertyChangedCallback(OnIsRequiredChanged)));

        private static void OnIsRequiredChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var entityBox = (EntityBox)d;
            entityBox.ClearButton.Visibility = entityBox.IsRequired || entityBox.IsReadOnly ? Visibility.Collapsed : Visibility.Visible;
        }

        /// <summary>
        /// 获取或设置一个值，指示取得实体后是否还进行同名复制。
        /// </summary>
        public bool IsCopySameName
        {
            get { return (bool)GetValue(IsCopySameNameProperty); }
            set { SetValue(IsCopySameNameProperty, value); }
        }

        /// <summary>
        /// 标识 IsCopySameName 的依赖属性。
        /// </summary>
        public static readonly DependencyProperty IsCopySameNameProperty =
            DependencyProperty.Register(nameof(IsCopySameName), typeof(bool), typeof(EntityBox), new PropertyMetadata(true));

        public string Format
        {
            get { return (string)GetValue(FormatProperty); }
            set { SetValue(FormatProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Format.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty FormatProperty =
            DependencyProperty.Register(nameof(Format), typeof(string), typeof(EntityBox), new PropertyMetadata(new PropertyChangedCallback(OnFormatChanged)));

        /// <summary>
        /// Format 依赖项属性更变的回调方法。
        /// </summary>
        /// <param name="d">属性已更改值的 DependencyObject 。</param>
        /// <param name="node">由所有事件跟踪问题到该属性的有效值的更改事件数据。</param>
        private static void OnFormatChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var entityBox = (EntityBox)d;
            if (!string.IsNullOrWhiteSpace(entityBox.Format))
                entityBox.formatMethod = Scripts.ScriptBuilder.GetMethod(Scripts.ScriptTemplates.FormatTemplate, entityBox.Format, "Compete.Mis.Scripts.Formater", "GetString");
        }

        public string FilterFormat
        {
            get { return (string)GetValue(FilterFormatProperty); }
            set { SetValue(FilterFormatProperty, value); }
        }

        // Using a DependencyProperty as the backing store for FilterFormat.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty FilterFormatProperty =
            DependencyProperty.Register(nameof(FilterFormat), typeof(string), typeof(EntityBox));

        #endregion 依赖属性

        private static string GetEntityName(string columnName)
        {
            var entityName = columnName.EndsWith("_Id") ? columnName[..^3] : columnName;
            if (entityName.EndsWith("_User"))
                entityName = "Operator";
            return entityName;
        }
        
        public static IDictionary<DependencyProperty, object?> GeneratePropertyDictionary(DataColumn column, IDictionary<string, string> parameters)
        {
            var entityName = GetEntityName(column.ColumnName);

            return new Dictionary<DependencyProperty, object?>
            {
                { EntityNameProperty, column.Caption },
                { ValuePathProperty, parameters == null || !parameters.TryGetValue("ValuePath", out string? valuePath) ? $"{entityName}_Id" : valuePath },
                { DisplayPathProperty, parameters == null || !parameters.TryGetValue("DisplayPath", out string? displayPath) ? $"{entityName}_Name" : displayPath },
                { ServiceParameterProperty, parameters == null || !parameters.TryGetValue("ServiceParameter", out string? serviceParameter) ? entityName : serviceParameter }
            };
        }

        public static IDictionary<DependencyProperty, object?> GenerateGridPropertyDictionary(DataColumn column, IDictionary<string, string> parameters)
        {
            var entityName = GetEntityName(column.ColumnName);

            return new Dictionary<DependencyProperty, object?>
            {
                { EntityNameProperty, column.Caption },
                { ValuePathProperty, parameters == null || !parameters.TryGetValue("ValuePath", out string? valuePath) ? $"{entityName}_Id" : valuePath },
                { DisplayPathProperty, parameters == null || !parameters.TryGetValue("DisplayPath", out string? displayPath) ? $"{entityName}_Code" : displayPath },
                { ServiceParameterProperty, parameters == null || !parameters.TryGetValue("ServiceParameter", out string? serviceParameter) ? entityName : serviceParameter }
            };
        }
    }
}
