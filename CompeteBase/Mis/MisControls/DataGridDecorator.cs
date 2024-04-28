﻿// ===================================================================
// Compete Management Information System
// ===================================================================
// 版权所有 © Compete software studio 2018 保留所有权利。
// -------------------------------------------------------------------
// 版本    日期时间            作者     说明
// -------------------------------------------------------------------
// 1.0.0.0 2018/3/2 10:22:37 LeeZheng  新建。
// ===================================================================
using Compete.Extensions;
using Compete.Mis.Enums;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Media;
using Xceed.Wpf.Toolkit;

namespace Compete.Mis.MisControls
{
    /// <summary>
    /// DataGrid 装饰器类。
    /// </summary>
    public sealed class DataGridDecorator
    {
        //private const string formatString = "string.Format(\"{{0}}({{1}})\", row[\"{0}_Name\"], row[\"{0}_Code\"])";//"{row[\"Name\"]}({row[\"Code\"]})"

        private static readonly string dateTimeStringFormat = GlobalCommon.GetMessage("DateTimeStringFormat");

        private static readonly string dateStringFormat = GlobalCommon.GetMessage("DateStringFormat");

        private readonly DataGrid dataGrid;

        private static readonly bool isWin7Style = Environment.OSVersion.Version.Major == 6;

        /// <summary>
        /// 初始化 <see cref="DataGridDecorator"/> 类的新实例。
        /// </summary>
        /// <param name="grid">要扩充的 <see cref="DataGrid"/> 对象。</param>
        public DataGridDecorator(DataGrid grid)
        {
            dataGrid = grid;

            dataGrid.AutoGeneratingColumn += DataGrid_AutoGeneratingColumn;
            //dataGrid.AutoGeneratedColumns += DataGrid_AutoGeneratedColumns;
            //dataGrid.Sorting += DataGrid_Sorting;
        }

        //private void DataGrid_Sorting(object sender, DataGridSortingEventArgs e)
        //{
        //    DataGrid dataGrid = (DataGrid)sender;
        //    if (e.Column.GetType() == typeof(DataGridTemplateColumn))
        //    {
        //        // 取消默认的排序行为
        //        e.Handled = true;

        //        // 获取要排序的数据项
        //        ICollectionView view = CollectionViewSource.GetDefaultView(dataGrid.ItemsSource);

        //        // 执行自定义排序逻辑
        //        view.SortDescriptions.Clear();
        //        e.Column.SortDirection = e.Column.SortDirection == ListSortDirection.Ascending ? ListSortDirection.Descending : ListSortDirection.Ascending;
        //        view.SortDescriptions.Add(new SortDescription(e.Column.SortMemberPath, e.Column.SortDirection!.Value));

        //        // 刷新DataGrid以应用排序
        //        dataGrid.Items.Refresh();
        //    }
        //}

        //private void DataGrid_AutoGeneratedColumns(object? sender, EventArgs e)
        //{

        //}

        private static double GetMax(int length, int precision)
        {
            Debug.Assert(length >= precision, "长度要大于等于精度。");

            return (Math.Pow(10, length) - 1) / Math.Pow(10, precision);
        }

        private static T? GetMaximum<T>(int length, int precision) where T : struct
        {
            if (length < 0)
                return null;

            return Utils.TypeConvert.ChangeNullableType<T>(GetMax(length, precision));
        }

        private static T? GetMinimum<T>(int length, int precision) where T : struct
        {
            if (length < 0)
                return null;

            return Utils.TypeConvert.ChangeNullableType<T>(-GetMax(length, precision));
        }

        /// <summary>
        /// 自动生成单列事件处理方法。
        /// </summary>
        /// <param name="sender">事件源。</param>
        /// <param name="e">事件数据。</param>
        private void DataGrid_AutoGeneratingColumn(object? sender, DataGridAutoGeneratingColumnEventArgs e)
        {
            DataView dataView;
            if (dataGrid.ItemsSource is CollectionView collectionView)
                dataView = (DataView)collectionView.SourceCollection;
            else
                dataView = (DataView)dataGrid.ItemsSource;
            DataColumn column = dataView.Table!.Columns[e.PropertyName]!;
            var columnName = column.ColumnName;

            //if (columnName.EndsWith("_Id") && column.DataType == typeof(long))
            //{
            //    try
            //    {
            //        var entityName = columnName.Substring(0, columnName.Length - 3);
            //        if (entityName == "Tenant")
            //            return;

            //        var nameColumnName = entityName + "Name";
            //        var table = column.Table;
            //        if (!table.Columns.Contains(nameColumnName))
            //        {
            //            table.Columns.Add(nameColumnName, typeof(string));
            //            foreach (DataRow row in table.Rows)
            //                if (row[column] != DBNull.Value)
            //                    row[nameColumnName] = Global.EntityDataProvider.GetEntity(entityName, row[column]);
            //        }

            //        if (table.Columns.Contains(entityName))
            //            table.Columns[entityName].SetOrdinal(column.Ordinal + 1);

            //        return;
            //    }
            //    finally
            //    {
            //        e.Cancel = true;
            //    }
            //}

            if (column.ExtendedProperties.ContainsKey(MemoryData.ExtendedPropertyNames.IsVisible) && !Convert.ToBoolean(column.ExtendedProperties[MemoryData.ExtendedPropertyNames.IsVisible]))   // 不可视。
            {
                e.Cancel = true;
                return;
            }

            e.Column.IsReadOnly = column.ExtendedProperties.ContainsKey(MemoryData.ExtendedPropertyNames.IsReadOnly) && Convert.ToBoolean(column.ExtendedProperties[MemoryData.ExtendedPropertyNames.IsReadOnly]);
            var isRequired = !e.Column.IsReadOnly && column.ExtendedProperties.ContainsKey(MemoryData.ExtendedPropertyNames.IsRequired) && Convert.ToBoolean(column.ExtendedProperties[MemoryData.ExtendedPropertyNames.IsRequired]);

            var control = column.ExtendedProperties[MemoryData.ExtendedPropertyNames.Control];
            var controlType = control == null ? DataControlType.Default : control.ToString()!.ToEnum<DataControlType>();

            BindingBase binding;
            if ((dataGrid.IsReadOnly || e.Column.IsReadOnly) && controlType == DataControlType.Default && e.Column is DataGridBoundColumn dataGridBoundColumn)
                binding = dataGridBoundColumn.Binding;
            else
                binding = new Binding(e.PropertyName);

            //binding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;

            var dbType = column.ExtendedProperties.Contains(MemoryData.ExtendedPropertyNames.DataType) ? column.ExtendedProperties[MemoryData.ExtendedPropertyNames.DataType]!.ToString()!.ToEnum<DbType>() : DbType.String;

            var format = column.ExtendedProperties[MemoryData.ExtendedPropertyNames.Format]?.ToString();   // 绑定格式。
            if (string.IsNullOrWhiteSpace(format))
                switch (dbType)
                {
                    case DbType.Date:
                        binding.StringFormat = dateStringFormat;
                        break;
                    case DbType.DateTime:
                    case DbType.DateTime2:
                    case DbType.DateTimeOffset:
                        binding.StringFormat = dateTimeStringFormat;
                        break;
                }
            else
                binding.StringFormat = format;

            //if (binding.StringFormat == "C2")
            //    binding.StringFormat = "¥{0:N2}";

            if (column.ExtendedProperties.Contains(MemoryData.ExtendedPropertyNames.TargetNullValue) && column.ExtendedProperties[MemoryData.ExtendedPropertyNames.TargetNullValue] != null)
                binding.TargetNullValue = column.ExtendedProperties[MemoryData.ExtendedPropertyNames.TargetNullValue];

            var parameters = DataControlHelper.ConvertParameters((string)column!.ExtendedProperties[MemoryData.ExtendedPropertyNames.Parameters]!)!;

            var dataType = column.DataType;
            if (dataGrid.IsReadOnly || e.Column.IsReadOnly)
            {
                // 只读列设置。
                if (controlType == DataControlType.Default)
                {
                    if ((dataType == typeof(long) || dataType == typeof(Guid)) && columnName.EndsWith("_Id") && EntityDataHelper.IsEntityColumn(columnName))
                        e.Column = CreateColumn(binding, typeof(EntityTextBlock), EntityTextBlock.ValueProperty,
                            EntityTextBlock.GeneratePropertyDictionary(column, parameters), false, true);
                    else if (column.DataType.IsNumeric() && e.Column is DataGridBoundColumn gridBoundColumn)
                    {
                        gridBoundColumn.ElementStyle = new Style { TargetType = typeof(TextBlock) };
                        gridBoundColumn.ElementStyle.Setters.Add(new Setter { Property = TextBlock.TextAlignmentProperty, Value = TextAlignment.Right });
                    }
                    //new Dictionary<DependencyProperty, object>
                    //        {
                    //            { EntityTextBlock.ValuePathProperty, "Id" },
                    //            { EntityTextBlock.DisplayPathProperty, "Name" },
                    //            { EntityTextBlock.ServiceParameterProperty, columnName.Substring(0, columnName.Length - 3) }
                    //        }
                }
                else
                {
                    switch (controlType)
                    {
                        case DataControlType.EntityBox:         // 实体框。
                            e.Column = CreateColumn(binding, typeof(EntityTextBlock), EntityTextBlock.ValueProperty,
                                EntityTextBlock.GeneratePropertyDictionary(column, parameters), false, true);
                            break;
                        case DataControlType.SinglechoiceBox:   // 单选框。
                            e.Column = CreateColumn(binding, typeof(SinglechoiceBox), ChoiceBox.ValueProperty,
                                new Dictionary<DependencyProperty, object?>
                                {
                                    { ChoiceBox.ItemDataProperty, GlobalCommon.EnumDictionary[column.ExtendedProperties[MemoryData.ExtendedPropertyNames.Parameters]!.ToString()!] }
                                }, false, true,
                                new Dictionary<DependencyProperty, object?>
                                {
                                    { ChoiceBox.IsReadOnlyProperty, true }
                                });
                            break;
                        case DataControlType.MultichoiceBox:    // 多选框。
                            e.Column = CreateColumn(binding, typeof(MultichoiceBox), ChoiceBox.ValueProperty,
                                new Dictionary<DependencyProperty, object?>
                                {
                                    { ChoiceBox.ItemDataProperty, GlobalCommon.EnumDictionary[column.ExtendedProperties[MemoryData.ExtendedPropertyNames.Parameters]!.ToString()!] }
                                }, false, true,
                                new Dictionary<DependencyProperty, object?>
                                {
                                    { ChoiceBox.IsReadOnlyProperty, true }
                                });
                            break;
                        default:
                            //throw new Exceptions.PlatformException($"不能使用不支持的数据控件类型【{controlType}】。");
                            Debug.Assert(false, $"不能使用不支持的数据控件类型【{controlType}】。");
                            return;
                    }
                }

                e.Column.IsReadOnly = true;
            }
            else if (controlType == DataControlType.Default)
            {
                // 非只读、默认控件列设置。
                var maximum = column.ExtendedProperties[MemoryData.ExtendedPropertyNames.Maximum];     // 最大值。
                var minimum = column.ExtendedProperties[MemoryData.ExtendedPropertyNames.Minimum];     // 最小值。
                var length = (column.ExtendedProperties[MemoryData.ExtendedPropertyNames.Length] as int?) ?? -1;       // 长度。
                var precision = (column.ExtendedProperties[MemoryData.ExtendedPropertyNames.Precision] as int?) ?? 0; // 长度。

                if ((dataType == typeof(long) || dataType == typeof(Guid)) && columnName.EndsWith("_Id") && EntityDataHelper.IsEntityColumn(columnName))
                {
                    var showFormat = column.ExtendedProperties[MemoryData.ExtendedPropertyNames.ShowFormat]?.ToString();// ?? string.Format(formatString, columnName[..^3]);   // 绑定格式。

                    var propertyDictionary = EntityBox.GenerateGridPropertyDictionary(column, parameters);
                    propertyDictionary.Add(EntityBox.IsRequiredProperty, isRequired);
                    propertyDictionary.Add(EntityBox.FormatProperty, showFormat);

                    var displayPropertyDictionary = EntityTextBlock.GenerateGridPropertyDictionary(column, parameters);
                    displayPropertyDictionary.Add(EntityTextBlock.FormatProperty, showFormat);

                    e.Column = CreateColumn(binding, typeof(EntityBox), EntityBox.ValueProperty,
                        propertyDictionary,
                        false, false,
                        displayPropertyDictionary);
                    //new Dictionary<DependencyProperty, object>
                    //    {
                    //        { EntityBox.EntityNameProperty, column.Caption },
                    //        { EntityBox.ValuePathProperty, "Id" },
                    //        { EntityBox.DisplayPathProperty, "Name" },
                    //        { EntityBox.IsRequiredProperty, isRequired },
                    //        { EntityBox.FormatProperty, showFormat ?? defaultFormat },
                    //        { EntityBox.ServiceParameterProperty, columnName.Substring(0, columnName.Length - 2) }
                    //    }
                    //new Dictionary<DependencyProperty, object>
                    //    {
                    //        { EntityTextBlock.ValuePathProperty, "Id" },
                    //        { EntityTextBlock.DisplayPathProperty, "Name" },
                    //        { EntityTextBlock.FormatProperty, showFormat ?? defaultFormat },
                    //        { EntityTextBlock.ServiceParameterProperty, columnName.Substring(0, columnName.Length - 2) }
                    //    }
                }
                else if (dataType == typeof(long))
                    e.Column = CreateUpDownColumn<LongUpDown, long>(binding, new Dictionary<DependencyProperty, object?>
                        {
                            { LongUpDown.MaximumProperty, Utils.MathHelper.Min(Utils.TypeConvert.ChangeNullableType<long>(maximum), GetMaximum<long>(length, precision)) },
                            { LongUpDown.MinimumProperty, Utils.MathHelper.Max(Utils.TypeConvert.ChangeNullableType<long>(minimum), GetMinimum<long>(length, precision)) },
                        });
                    //e.Column = CreateColumn(binding, typeof(LongUpDown), LongUpDown.ValueProperty,
                    //    new Dictionary<DependencyProperty, object?>
                    //    {
                    //        { LongUpDown.MaximumProperty, Utils.MathHelper.Min(Utils.TypeConvert.ChangeNullableType<long>(maximum), GetMaximum<long>(length, precision)) },
                    //        { LongUpDown.MinimumProperty, Utils.MathHelper.Max(Utils.TypeConvert.ChangeNullableType<long>(minimum), GetMinimum<long>(length, precision)) },
                    //    },
                    //    true);
                else if (dataType == typeof(int))
                    e.Column = CreateUpDownColumn<IntegerUpDown, int>(binding, new Dictionary<DependencyProperty, object?>
                        {
                            { IntegerUpDown.MaximumProperty, Utils.MathHelper.Min(Utils.TypeConvert.ChangeNullableType<int>(maximum), GetMaximum<int>(length, precision)) },
                            { IntegerUpDown.MinimumProperty, Utils.MathHelper.Max(Utils.TypeConvert.ChangeNullableType<int>(minimum), GetMinimum<int>(length, precision)) },
                        });
                else if (dataType == typeof(short))
                    e.Column = CreateUpDownColumn<ShortUpDown, short>(binding, new Dictionary<DependencyProperty, object?>
                        {
                            { ShortUpDown.MaximumProperty, Utils.MathHelper.Min(Utils.TypeConvert.ChangeNullableType<short>(maximum), GetMaximum<short>(length, precision)) },
                            { ShortUpDown.MinimumProperty, Utils.MathHelper.Max(Utils.TypeConvert.ChangeNullableType<short>(minimum), GetMinimum<short>(length, precision)) },
                        });
                else if (dataType == typeof(byte))
                    e.Column = CreateUpDownColumn<ByteUpDown, byte>(binding, new Dictionary<DependencyProperty, object?>
                        {
                            { ByteUpDown.MaximumProperty, Utils.MathHelper.Min(Utils.TypeConvert.ChangeNullableType<byte>(maximum), GetMaximum<byte>(length, precision)) },
                            { ByteUpDown.MinimumProperty, Utils.MathHelper.Max(Utils.TypeConvert.ChangeNullableType<byte>(minimum), GetMinimum<byte>(length, precision)) },
                        });
                else if (dataType == typeof(decimal))
                {
                    if (dbType == DbType.Currency)
                        e.Column = CreateUpDownColumn<CalculatorUpDown, decimal>(binding, new Dictionary<DependencyProperty, object?>
                        {
                            { CalculatorUpDown.MaximumProperty, Utils.MathHelper.Min(Utils.TypeConvert.ChangeNullableType<decimal>(maximum), GetMaximum<decimal>(length, precision)) },
                            { CalculatorUpDown.MinimumProperty, Utils.MathHelper.Max(Utils.TypeConvert.ChangeNullableType<decimal>(minimum), GetMinimum<decimal>(length, precision)) },
                        });
                    else
                        e.Column = CreateUpDownColumn<DecimalUpDown, decimal>(binding, new Dictionary<DependencyProperty, object?>
                        {
                            { DecimalUpDown.MaximumProperty, Utils.MathHelper.Min(Utils.TypeConvert.ChangeNullableType<decimal>(maximum), GetMaximum<decimal>(length, precision)) },
                            { DecimalUpDown.MinimumProperty, Utils.MathHelper.Max(Utils.TypeConvert.ChangeNullableType<decimal>(minimum), GetMinimum<decimal>(length, precision)) },
                        });
                }
                else if (dataType == typeof(DateTime) || dataType == typeof(DateTimeOffset))
                {
                    e.Column = dbType switch
                    {
                        DbType.Date => CreateColumn(binding, typeof(DatePicker), DatePicker.SelectedDateProperty),
                        DbType.Time => CreateUpDownColumn<TimePicker, DateTime>(binding, new Dictionary<DependencyProperty, object?>
                                {
                                    { TimePicker.MaximumProperty, Utils.TypeConvert.ChangeNullableType<DateTime>(maximum) },
                                    { TimePicker.MinimumProperty, Utils.TypeConvert.ChangeNullableType<DateTime>(minimum) },
                                }, false),
                        _ => CreateUpDownColumn<DateTimePicker, DateTime>(binding, new Dictionary<DependencyProperty, object?>
                                {
                                    { DateTimePicker.MaximumProperty, Utils.TypeConvert.ChangeNullableType<DateTime>(maximum) },
                                    { DateTimePicker.MinimumProperty, Utils.TypeConvert.ChangeNullableType<DateTime>(minimum) },
                                }, false),
                    };
                }
                else if (dataType == typeof(TimeSpan))
                    e.Column = CreateUpDownColumn<TimeSpanUpDown, TimeSpan>(binding, new Dictionary<DependencyProperty, object?>
                        {
                            { TimeSpanUpDown.MaximumProperty, Utils.TypeConvert.ChangeNullableType<TimeSpan>(maximum) },
                            { TimeSpanUpDown.MinimumProperty, Utils.TypeConvert.ChangeNullableType<TimeSpan>(minimum) },
                        }, false);
                else if (dataType == typeof(sbyte))
                    e.Column = CreateColumn(binding, typeof(EnumComboBox), Selector.SelectedValueProperty,
                        new Dictionary<DependencyProperty, object?>
                        {
                            { EnumComboBox.EnumNameProperty, columnName },
                            { Selector.SelectedValuePathProperty, nameof(EnumItem.Value) },
                            { ItemsControl.DisplayMemberPathProperty, nameof(EnumItem.DisplayName) },
                        });
            }
            else
            {
                // 非只读、非默认控件列设置。
                //var parameters = DataControlHelper.ConvertParameters(column.ExtendedProperties[ExtendedPropertyNames.Parameters] as string);
                switch (controlType)
                {
                    case DataControlType.EntityBox:         // 实体框。
                        e.Column = CreateColumn(binding, typeof(EntityBox), EntityBox.ValueProperty,
                            EntityBox.GeneratePropertyDictionary(column, parameters),
                            false, false, EntityTextBlock.GeneratePropertyDictionary(column, parameters));
                        break;
                    case DataControlType.SinglechoiceBox:   // 单选框。
                        var enumName = column.ExtendedProperties.ContainsKey(MemoryData.ExtendedPropertyNames.Parameters) && column.ExtendedProperties[MemoryData.ExtendedPropertyNames.Parameters] != null
                                    ? column.ExtendedProperties[MemoryData.ExtendedPropertyNames.Parameters]!.ToString()
                                    : columnName;
                        Debug.Assert(GlobalCommon.EnumDictionary.ContainsKey(enumName!), $"枚举{enumName}未定义。");

                        e.Column = CreateColumn(binding, typeof(SinglechoiceBox), ChoiceBox.ValueProperty,
                            new Dictionary<DependencyProperty, object?>
                            {
                                {
                                    ChoiceBox.ItemDataProperty,
                                    GlobalCommon.EnumDictionary[enumName!]
                                }
                            });
                        break;
                    case DataControlType.MultichoiceBox:    // 多选框。
                        e.Column = CreateColumn(binding, typeof(MultichoiceBox), ChoiceBox.ValueProperty,
                            new Dictionary<DependencyProperty, object?>
                            {
                                { ChoiceBox.ItemDataProperty, GlobalCommon.EnumDictionary[column.ExtendedProperties[MemoryData.ExtendedPropertyNames.Parameters]!.ToString()!] }
                            });
                        break;
                }
            }

            e.Column.Header = column.Caption;   // 设置列标题。

            if (e.Column is DataGridTemplateColumn)
                e.Column.SortMemberPath = e.PropertyName;

            if (!((DataGrid)sender!).IsReadOnly)
            {
                var foregroundBrush = e.Column.IsReadOnly ? null : isRequired ? Constants.RequiredBrush : Constants.CanWriteBrush;
                if (foregroundBrush != null)
                {
                    e.Column.HeaderStyle = new Style { TargetType = typeof(DataGridColumnHeader) };
                    e.Column.HeaderStyle.Setters.Add(new Setter { Property = Control.ForegroundProperty, Value = foregroundBrush });
                    if (isWin7Style)
                        e.Column.HeaderStyle.Setters.Add(new Setter { Property = Control.BackgroundProperty, Value = Brushes.Transparent });
                }
            }

            // 将列头设为剧中显示。
            //e.Column.HeaderStyle = new Style { TargetType = typeof(DataGridColumnHeader) };
            //e.Column.HeaderStyle.Setters.Add(new Setter { Property = FrameworkElement.HorizontalAlignmentProperty, Value = HorizontalAlignment.Center });
            //if (e.Column is DataGridBoundColumn boundColumn)
            //{
            //    boundColumn.ElementStyle = new Style { TargetType = typeof(TextBlock) };
            //    boundColumn.ElementStyle.Setters.Add(new Setter { Property = TextBlock.TextAlignmentProperty, Value = TextAlignment.Center });
            //}

            if (binding is Binding columnBinding)
                columnBinding.Mode = e.Column.IsReadOnly ? BindingMode.OneWay : BindingMode.TwoWay;

            // 设置列宽度。
            //if (!e.Column.IsReadOnly)
            //{
            if (dataGrid.ColumnWidth.UnitType != DataGridLengthUnitType.Auto)
            {
                CharType charType = CharType.English;
                var columnLength = column.MaxLength;

                if (columnLength < 0)
                {
                    if (column.DataType == typeof(DateTime))
                    {
                        charType = CharType.Number;
                        columnLength = dbType switch
                        {
                            DbType.Date => 14,
                            DbType.Time => 15,
                            _ => 30,
                        };
                    }
                    else if (column.DataType == typeof(TimeSpan))
                        columnLength = 11;
                    //else if (EntityDataHelper.IsEntityColumn(columnName))
                    //    if (column.DataType == typeof(Guid))
                    //        columnLength = 36;
                    else if (column.DataType == typeof(long) && column.ColumnName.EndsWith("_Id"))
                        columnLength = 13;
                    else if (column.DataType.IsNumeric())
                    {
                        charType = CharType.Number;
                        columnLength = (column.ExtendedProperties[MemoryData.ExtendedPropertyNames.Length] as int?) ?? 0;

                        if (Convert.ToInt32(column.ExtendedProperties[MemoryData.ExtendedPropertyNames.Precision]) > 0)
                            columnLength++;

                        if ((column.ExtendedProperties[MemoryData.ExtendedPropertyNames.DataType] as DbType?) == DbType.Currency)
                            columnLength++;
                    }
                    else if (column.ColumnName.EndsWith("Code"))
                        charType = CharType.English;
                    else
                        charType = CharType.Chinese;
                }

                e.Column.Width = Math.Max(CharWidthHelper.GetStringWidth(column.Caption, dataGrid.FontFamily, dataGrid.FontSize, dataGrid.FontStyle, dataGrid.FontWeight, dataGrid.FontStretch) + 10D,
                    CharWidthHelper.GetStringWidth(charType, columnLength, dataGrid.FontFamily, dataGrid.FontSize, dataGrid.FontStyle, dataGrid.FontWeight, dataGrid.FontStretch)) + 8D;
            }
            //}

            // 调整顺序。
            var displayIndex = column.ExtendedProperties.ContainsKey(MemoryData.ExtendedPropertyNames.DisplayIndex) ? Convert.ToInt32(column.ExtendedProperties[MemoryData.ExtendedPropertyNames.DisplayIndex]) : -1;
            if (displayIndex >= 0 && displayIndex < dataGrid.Columns.Count)
                e.Column.DisplayIndex = displayIndex;
        }

        private DataGridTemplateColumn CreateUpDownColumn<T, N>(BindingBase binding, IDictionary<DependencyProperty, object?>? propertyDictionary = null, bool isNumeric = true)
            where T : Xceed.Wpf.Toolkit.Primitives.UpDownBase<N?> where N : struct
            => CreateColumn(binding, typeof(T), Xceed.Wpf.Toolkit.Primitives.UpDownBase<N?>.ValueProperty, propertyDictionary, isNumeric);


        /// <summary>
        /// 创建 <see cref="DataGridTemplateColumn"/> 型的数据列。
        /// </summary>
        /// <param name="binding">数据绑定。</param>
        /// <param name="editType">编辑用控件的数据类型。</param>
        /// <param name="editBindingProperty">编辑用控件绑定数据的依赖属性。</param>
        /// <param name="propertyDictionary">编辑用控件的依赖属性设置字典。</param>
        /// <param name="isNumeric">是否为数值型数据。</param>
        /// <param name="isSame">显示控件是否与编辑控件相同。</param>
        /// <param name="displayPropertyDictionary">显示用控件的依赖属性设置字典。</param>
        /// <returns><see cref="DataGridTemplateColumn"/> 型的数据列。</returns>
        private DataGridTemplateColumn CreateColumn(BindingBase binding, Type editType, DependencyProperty editBindingProperty, IDictionary<DependencyProperty, object?>? propertyDictionary = null,
            bool isNumeric = false, bool isSame = false, IDictionary<DependencyProperty, object?>? displayPropertyDictionary = null)//DataGridColumn
        {
            var column = new DataGridTemplateColumn();

            // 创建单元格显示工厂。
            FrameworkElementFactory cellFactory;
            if (isSame)
            {
                cellFactory = new FrameworkElementFactory(editType);
                cellFactory.SetBinding(editBindingProperty, binding);

                cellFactory.SetValue(Control.BorderThicknessProperty, new Thickness(0D));
            }
            else if (editType == typeof(EntityBox))
            {
                cellFactory = new FrameworkElementFactory(typeof(EntityTextBlock));
                cellFactory.SetBinding(EntityTextBlock.ValueProperty, binding);

                //binding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
            }
            else
            {
                cellFactory = new FrameworkElementFactory(typeof(TextBlock));
                cellFactory.SetBinding(TextBlock.TextProperty, binding);

                if (isNumeric)
                    cellFactory.SetValue(TextBlock.TextAlignmentProperty, TextAlignment.Right);
            }

            if (propertyDictionary != null)
                foreach (var pair in propertyDictionary)
                    cellFactory.SetValue(pair.Key, pair.Value);

            if (displayPropertyDictionary != null && displayPropertyDictionary.Count > 0)
                foreach (var pair in displayPropertyDictionary)
                    cellFactory.SetValue(pair.Key, pair.Value);

            column.CellTemplate = new DataTemplate
            {
                VisualTree = cellFactory
            };

            // 创建单元格编辑工厂。
            var cellEditingFactory = new FrameworkElementFactory(editType);
            cellEditingFactory.SetBinding(editBindingProperty, binding);

            cellEditingFactory.SetValue(Control.BorderThicknessProperty, new Thickness(0D));
            cellEditingFactory.SetValue(FrameworkElement.TagProperty, new TagData { Data = dataGrid });

            if (propertyDictionary != null)
                foreach (var pair in propertyDictionary)
                    cellEditingFactory.SetValue(pair.Key, pair.Value);

            column.CellEditingTemplate = new DataTemplate
            {
                VisualTree = cellEditingFactory
            };

            return column;
        }
    }
}
