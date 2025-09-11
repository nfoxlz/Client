using Compete.Extensions;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Reflection;
using System.Text.Json;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

namespace Compete.Mis.MisControls
{
    /// <summary>
    /// AbstractEntityBox.xaml 的交互逻辑
    /// </summary>
    public abstract partial class AbstractEntityBox : UserControl
    {
        protected MethodInfo? formatMethod;

        /// <summary>
        /// 获取选择命令。
        /// </summary>
        public ICommand Select { get; private set; } = new RoutedUICommand();

        public ICommand Clear { get; private set; } = new RoutedUICommand();
        
        public AbstractEntityBox()
        {
            InitializeComponent();
            DisplayTextBox.Height = GlobalConstants.TextBoxHeight;

            SelectButton.Command = Select;
            ClearButton.Command = Clear;
            CommandBindings.Add(new CommandBinding(Select, SelectCommandHandler, SelectCanExecuteHandler));
            CommandBindings.Add(new CommandBinding(Clear, ClearCommandHandler, ClearCanExecuteHandler));
        }

        protected DataRow? GetSourceRow()
        {
            var bindingExpression = GetBindingExpression(ValueProperty);
            if (bindingExpression is null)
                return null;

            var source = bindingExpression.ResolvedSource ?? bindingExpression.ParentBinding.Source;

            if (source is BindingListCollectionView collectionView)
                source = collectionView.CurrentItem ?? collectionView.SourceCollection;

            return Utils.DataHelper.GetRow(source);
            //if (source is DataView view)
            //    return view.Count > 0 ? view[0].Row : null;
            //else if (source is DataTable table)
            //    return table.Rows.Count > 0 ? table.Rows[0] : null;
            //else if (source is DataRowView rowView)
            //    return rowView.Row;
            //else if (source is DataRow row)
            //    return row;

            //return null;
        }

        protected abstract object? SelectData();

        //public Action? CallBack { get; set; }

        public Action? CallBack
        {
            get { return (Action?)GetValue(CallBackProperty); }
            set { SetValue(CallBackProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CallBack.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CallBackProperty =
            DependencyProperty.Register(nameof(CallBack), typeof(Action), typeof(AbstractEntityBox));

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
                var selectedItem = SelectData();
                if (selectedItem is null)
                    return;

                if (selectedItem is DataRowView view)
                    SetEntityValue(view.Row);
                else if (selectedItem is DataRow row)
                    SetEntityValue(row);
                else
                    SetEntityValue(selectedItem);

                CallBack?.Invoke();
            }
            catch(Exception exception)
            {
                MessageDialog.Exception(exception);
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
            else if(Value is ulong)
                Value = 0UL;
            else if (Value is Guid)
                Value = Guid.Empty;
            else
                Value = DBNull.Value;
            isSetValue = false;
            DisplayTextBox.Text = string.Empty;
        }

        /// <summary>
        /// Clear 命令的CanExecute事件的处理程序。 
        /// </summary>
        /// <param name="sender">调用处理程序的命令目标。</param>
        /// <param name="e">事件数据。</param>
        private void ClearCanExecuteHandler(object sender, CanExecuteRoutedEventArgs e) => e.CanExecute = Value != DBNull.Value && ((Value is long && (long)Value != 0L) || (Value is ulong && (ulong)Value != 0UL) || (Value is Guid && (Guid)Value != Guid.Empty));

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
            if (item is null)
                Value = DBNull.Value;
            else if (item.HasProperty(ValuePath))
                Value = item.GetPropertyValue(ValuePath);
            else if (item.HasProperty("Id"))
                Value = item.GetPropertyValue("Id");
            else
            {
                var idName = EntityName + "_Id";
                if (item.HasProperty(idName))
                    Value = item.GetPropertyValue(idName);
            }
            isSetValue = false;

            if (item is null)
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
                if (sourceRow is not null)
                    row.CopyTo(sourceRow, omittedColumns.Merge([string.IsNullOrWhiteSpace(ValuePath) ? "Id" : ValuePath]));
                    //row.CopyTo(sourceRow, omittedColumns.Merge([string.IsNullOrWhiteSpace(ValuePath) ? "Id" : ValuePath, string.IsNullOrWhiteSpace(DisplayPath) ? "Name" : DisplayPath]));
                    //sourceRow.EndEdit();
            }

            var columns = row.Table.Columns;

            if (!string.IsNullOrWhiteSpace(Format))
                DisplayTextBox.Text = formatMethod?.Invoke(null, [row])?.ToString();
            else if (columns.Contains(DisplayPath))
                DisplayTextBox.Text = row[DisplayPath].ToString();

            isSetValue = true;
            if (columns.Contains(ValuePath))
                Value = row[ValuePath];
            else if (columns.Contains("Id"))
                Value = row["Id"];
            else
            {
                var idName = EntityName + "_Id";
                if (columns.Contains(idName))
                    Value = row[idName];
            }
            isSetValue = false;

            if (IsCopySameName && Tag is TagData { Data: DataGrid grid })
            {
                if (!grid.CommitEdit(DataGridEditingUnit.Row, true))
                {
                    grid.CancelEdit();
                    throw new Exceptions.PlatformException(GlobalCommon.GetMessage("Exception.DataCannotSubmit"));
                }
                grid.Items.Refresh();

                if (grid is EnhancedDataGrid enhancedDataGrid)
                    enhancedDataGrid.IsEditing = false;
            }

            if (Value is null || Value is long longVal && longVal == 0L || Value is Guid guidVal && Guid.Empty == guidVal || Value is string stringVal && string.IsNullOrWhiteSpace(stringVal))
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

            //    if (sourceRow  is not null)
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

        protected abstract string? GetDisplay(DataTable entities);

        /// <summary>
        /// 标识 Value 的依赖属性。
        /// </summary>
        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register(nameof(Value), typeof(object), typeof(AbstractEntityBox), new PropertyMetadata((d, e) =>
            {
                var abstractEntityBox = (AbstractEntityBox)d;
                if (abstractEntityBox.isSetValue || abstractEntityBox.Value is not null && abstractEntityBox.Value.Equals(abstractEntityBox.oldValue) || string.IsNullOrWhiteSpace(abstractEntityBox.ServiceParameter) || string.IsNullOrWhiteSpace(abstractEntityBox.DisplayPath))
                    return;

                // 取得需要从UI线程向新线程传递的数据。
                var entityBoxValue = abstractEntityBox.Value;               // 控件的值。

                if (entityBoxValue is null || string.IsNullOrWhiteSpace(entityBoxValue.ToString()) || entityBoxValue is long val && 0L == val)
                {
                    abstractEntityBox.DisplayTextBox.Text = string.Empty;
                    return;
                }

                var parameter = abstractEntityBox.ServiceParameter;
                var entities = MisThreading.ThreadingHelper.Invoke(() => GlobalCommon.GetEntity(parameter, entityBoxValue), "Query");
                if (entities is null || entities.Rows.Count == 0 || !entities.Columns.Contains(abstractEntityBox.DisplayPath))
                    return;

                abstractEntityBox.DisplayTextBox.Text = abstractEntityBox.GetDisplay(entities);

                abstractEntityBox.oldValue = abstractEntityBox.Value;
            }));

        private object? oldValue;

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
            DependencyProperty.Register(nameof(ValuePath), typeof(string), typeof(AbstractEntityBox));

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
            DependencyProperty.Register(nameof(DisplayPath), typeof(string), typeof(AbstractEntityBox));

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
            DependencyProperty.Register(nameof(EntityName), typeof(string), typeof(AbstractEntityBox));

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
            DependencyProperty.Register(nameof(ServiceParameter), typeof(string), typeof(AbstractEntityBox), new PropertyMetadata((d, e) =>
            {
                var abstractEntityBox = (AbstractEntityBox)d;
                var path = Path.ChangeExtension(Path.Combine(Utils.PathHelper.PluginPath, "EntitySelector", abstractEntityBox.ServiceParameter), "json");
                if (File.Exists(path))
                {
                    var setting = JsonSerializer.Deserialize<EntitySelectorSetting>(File.ReadAllText(path));
                    if (string.IsNullOrWhiteSpace(abstractEntityBox.Format) && !string.IsNullOrWhiteSpace(setting?.Format))
                        abstractEntityBox.Format = setting.Format;
                    if (string.IsNullOrWhiteSpace(abstractEntityBox.FilterFormat) && !string.IsNullOrWhiteSpace(setting?.FilterFormat))
                        abstractEntityBox.FilterFormat = setting.FilterFormat;
                }
            }));

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
            DependencyProperty.Register(nameof(IsReadOnly), typeof(bool), typeof(AbstractEntityBox), new PropertyMetadata((d, e) =>
            {
                var abstractEntityBox = (AbstractEntityBox)d;
                abstractEntityBox.SelectButton.Visibility = abstractEntityBox.IsReadOnly ? Visibility.Collapsed : Visibility.Visible;
                abstractEntityBox.ClearButton.Visibility = abstractEntityBox.IsRequired || abstractEntityBox.IsReadOnly ? Visibility.Collapsed : Visibility.Visible;
            }));

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
            DependencyProperty.Register(nameof(IsRequired), typeof(bool), typeof(AbstractEntityBox), new PropertyMetadata(false, (d, e) =>
            {
                var abstractEntityBox = (AbstractEntityBox)d;
                abstractEntityBox.ClearButton.Visibility = abstractEntityBox.IsRequired || abstractEntityBox.IsReadOnly ? Visibility.Collapsed : Visibility.Visible;
            }));

        //public bool IsSmallSize
        //{
        //    get { return (bool)GetValue(IsSmallSizeProperty); }
        //    set { SetValue(IsSmallSizeProperty, value); }
        //}

        //// Using a DependencyProperty as the backing store for IsSmallSize.  This enables animation, styling, binding, etc...
        //public static readonly DependencyProperty IsSmallSizeProperty =
        //    DependencyProperty.Register(nameof(IsSmallSize), typeof(bool), typeof(AbstractEntityBox), new PropertyMetadata(false, (d, e) =>
        //    {
        //        var abstractEntityBox = (AbstractEntityBox)d;
        //        if (abstractEntityBox.IsSmallSize)
        //            abstractEntityBox.DisplayTextBox.Height = Constants.SmallTextBoxHeight;
        //        else
        //            abstractEntityBox.DisplayTextBox.Height = Constants.TextBoxHeight;
        //    }));

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
            DependencyProperty.Register(nameof(IsCopySameName), typeof(bool), typeof(AbstractEntityBox), new PropertyMetadata(true));

        public string Format
        {
            get { return (string)GetValue(FormatProperty); }
            set { SetValue(FormatProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Format.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty FormatProperty =
            DependencyProperty.Register(nameof(Format), typeof(string), typeof(AbstractEntityBox), new PropertyMetadata((d, e) =>
            {
                var abstractEntityBox = (AbstractEntityBox)d;
                if (!string.IsNullOrWhiteSpace(abstractEntityBox.Format))
                    abstractEntityBox.formatMethod = Scripts.ScriptBuilder.GetMethod(Scripts.ScriptTemplates.FormatTemplate, abstractEntityBox.Format, "Compete.Mis.Scripts.Formater", "GetString");
            }));

        public string FilterFormat
        {
            get { return (string)GetValue(FilterFormatProperty); }
            set { SetValue(FilterFormatProperty, value); }
        }

        // Using a DependencyProperty as the backing store for FilterFormat.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty FilterFormatProperty =
            DependencyProperty.Register(nameof(FilterFormat), typeof(string), typeof(AbstractEntityBox));

        #endregion 依赖属性

        private static string GetEntityName(string columnName)
        {
            var entityName = columnName.EndsWith("_Id") ? columnName[..^3] : columnName;
            if (entityName.EndsWith("_User"))
                entityName = "Operator";
            return entityName;
        }

        //public static IDictionary<DependencyProperty, object?> GeneratePropertyDictionary(DataColumn column, IDictionary<string, string>? parameters)
        //{
        //    var entityName = GetEntityName(column.ColumnName);

        //    return new Dictionary<DependencyProperty, object?>
        //    {
        //        { EntityNameProperty, column.Caption },
        //        { ValuePathProperty, parameters is null || !parameters.TryGetValue(Constants.EntityBoxParameterValuePath, out string? valuePath) ? $"{entityName}_Id" : valuePath },
        //        { DisplayPathProperty, parameters is null || !parameters.TryGetValue(Constants.EntityBoxParameterDisplayPath, out string? displayPath) ? $"{entityName}_Name" : displayPath },
        //        { ServiceParameterProperty, parameters is null || !parameters.TryGetValue(Constants.EntityBoxParameterServiceParameter, out string? serviceParameter) ? entityName : serviceParameter }
        //    };
        //}

        public static IDictionary<DependencyProperty, object?> GenerateGridPropertyDictionary(DataColumn column, IDictionary<string, string>? parameters)
        {
            var entityName = GetEntityName(column.ColumnName);

            return new Dictionary<DependencyProperty, object?>
            {
                { EntityNameProperty, column.Caption },
                { ValuePathProperty, parameters is null || !parameters.TryGetValue(GlobalConstants.EntityBoxParameterValuePath, out string? valuePath) ? $"{entityName}_Id" : valuePath },
                { DisplayPathProperty, parameters is null || !parameters.TryGetValue(GlobalConstants.EntityBoxParameterDisplayPath, out string? displayPath) ? $"{entityName}_Name" : displayPath },
                { ServiceParameterProperty, parameters is null || !parameters.TryGetValue(GlobalConstants.EntityBoxParameterServiceParameter, out string? serviceParameter) ? entityName : serviceParameter }
            };
        }
    }
}
