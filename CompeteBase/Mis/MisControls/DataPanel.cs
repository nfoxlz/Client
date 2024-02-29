// ===================================================================
// Compete Management Information System
// ===================================================================
// 版权所有 © Compete software studio 2018 保留所有权利。
// -------------------------------------------------------------------
// 版本    日期时间            作者     说明
// -------------------------------------------------------------------
// 1.0.0.0 2018/3/7 16:56:15 LeeZheng 新建。
//==============================================================
using Compete.Extensions;
using Compete.MemoryData;
using Compete.Mis.Enums;
using System;
using System.Data;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Media;
using Xceed.Wpf.Toolkit;
using Xceed.Wpf.Toolkit.Primitives;

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
    /////     <MyNamespace:DataPanel/>
    /////
    ///// </summary>

    /// <summary>
    /// 数据面板控件类。
    /// </summary>
    public sealed class DataPanel : DataPanelBase
    {
        private const string beginPrefix = "Begin_";

        private const string endPrefix = "End_";

        public bool IsReadOnly
        {
            get { return (bool)GetValue(IsReadOnlyProperty); }
            set { SetValue(IsReadOnlyProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsReadOnly.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsReadOnlyProperty =
            DependencyProperty.Register(nameof(IsReadOnly), typeof(bool), typeof(DataPanel), new PropertyMetadata(false));

        protected override Brush? GetTitleForeground(DataColumn column)
        {
            if (column.ReadOnly || Convert.ToBoolean(column.ExtendedProperties[ExtendedPropertyNames.IsReadOnly]))
                return Constants.ReadOnlyBrush;
            else if (!column.AllowDBNull || Convert.ToBoolean(column.ExtendedProperties[ExtendedPropertyNames.IsRequired]))
                return Constants.RequiredBrush;
            else
                return base.GetTitleForeground(column);
        }

        private T CreateUpDown<T, N>(string columnName, object? maximum, object? minimum, Binding binding) where T : UpDownBase<N?>, new() where N : struct
        {
            T result = new T()
            {
                Maximum = Utils.TypeConvert.ChangeNullableType<N>(maximum),
                Minimum = Utils.TypeConvert.ChangeNullableType<N>(minimum),
            };

            if (columnName.StartsWith(beginPrefix))
            {
                result.ValueChanged += BeginControlChanged;
                result.Name = columnName;
            }
            else if (columnName.StartsWith(endPrefix))
            {
                result.ValueChanged += EndControlChanged;
                result.Name = columnName;
            }

            result.SetBinding(UpDownBase<N>.ValueProperty, binding);

            return result;
        }

        /// <summary>
        /// 依据数据列创建编辑或显示控件。
        /// </summary>
        /// <param name="column">所依据的数据列。</param>
        /// <returns>生成的编辑或显示控件。</returns>
        protected override FrameworkElement? CreateElement(DataColumn column)
        {
            var isReadOnly = IsReadOnly || column.ReadOnly || Convert.ToBoolean(column.ExtendedProperties[ExtendedPropertyNames.IsReadOnly]);     // 是否只读。
            var isRequired = !isReadOnly && (!column.AllowDBNull || Convert.ToBoolean(column.ExtendedProperties[ExtendedPropertyNames.IsRequired])); // 是否必填。

            var binding = GetBinding(column);

            var control = column.ExtendedProperties[ExtendedPropertyNames.Control];
            var controlType = control == null ? DataControlType.Default : control.ToString()!.ToEnum<DataControlType>();

            FrameworkElement? result;
            var columnName = column.ColumnName;
            var dataType = column.DataType;
            var type = column.ExtendedProperties[ExtendedPropertyNames.DataType];
            var dbType = type == null ? DbType.String : type.ToString()!.ToEnum<DbType>();
            var parameters = DataControlHelper.ConvertParameters((string)column.ExtendedProperties[ExtendedPropertyNames.Parameters]!);

            if (isReadOnly) // 只读处理。
            {
                binding.Mode = BindingMode.OneWay;

                // 生成显示控件。
                if (controlType == DataControlType.Default)
                {
                    //result = new TextBlock();
                    //result.SetBinding(TextBlock.TextProperty, binding);
                    if ((dataType == typeof(long) || dataType == typeof(Guid)) && columnName.EndsWith("_Id") && EntityDataHelper.IsEntityColumn(columnName))
                    {
                        var entityName = columnName[..^3];
                        result = new EntityBox
                        {
                            IsReadOnly = true,
                            ValuePath = parameters == null ? columnName : parameters["ValuePath"],
                            DisplayPath = parameters == null ? $"{entityName}_Name" : parameters["DisplayPath"],
                            ServiceParameter = parameters == null ? entityName : parameters["ServiceParameter"]
                        };
                        result.SetBinding(EntityBox.ValueProperty, binding);
                    }
                    else
                    {
                        result = new TextBox() { IsReadOnly = true };
                        if (dataType.IsNumeric())
                            ((TextBox)result).TextAlignment = TextAlignment.Right;
                        else if (dbType == DbType.Date)
                            binding.StringFormat = "yyyy年M月d日";

                        result.SetBinding(TextBox.TextProperty, binding);
                    }
                }
                else
                    switch (controlType)
                    {
                        case DataControlType.EntityBox:         // 实体框。
                            var entityBoxName = columnName.EndsWith("_Id") ? columnName[..^3] : columnName;
                            result = new EntityBox
                            {
                                IsReadOnly = true,
                                ValuePath = parameters == null ? columnName : parameters["ValuePath"],
                                DisplayPath = parameters == null ? $"{entityBoxName}_Name" : parameters["DisplayPath"],
                                ServiceParameter = parameters == null ? entityBoxName : parameters["ServiceParameter"]
                            };
                            result.SetBinding(EntityBox.ValueProperty, binding);
                            break;
                        case DataControlType.SinglechoiceBox:   // 单选框。
                            result = new SinglechoiceBox
                            {
                                IsReadOnly = true,
                                ItemData = GlobalCommon.EnumDictionary[(column.ExtendedProperties[ExtendedPropertyNames.Parameters] ?? columnName).ToString()!]
                            };
                            result.SetBinding(ChoiceBox.ValueProperty, binding);
                            break;
                        case DataControlType.MultichoiceBox:    // 多选框。
                            result = new MultichoiceBox
                            {
                                IsReadOnly = true,
                                ItemData = GlobalCommon.EnumDictionary[(column.ExtendedProperties[ExtendedPropertyNames.Parameters] ?? columnName).ToString()!]
                            };
                            result.SetBinding(ChoiceBox.ValueProperty, binding);
                            break;
                        default:
                            result = null;
                            break;
                    }
            }
            else
            {
                var maximum = column.ExtendedProperties[ExtendedPropertyNames.Maximum]; // 最大值。
                var minimum = column.ExtendedProperties[ExtendedPropertyNames.Minimum]; // 最小值。

                if (isRequired)  // 是否必须。
                    binding.ValidationRules.Add(new NonnullRule() { DisplayName = column.Caption });

                var regex = column.ExtendedProperties[ExtendedPropertyNames.Regex]?.ToString(); // 正则表达式。
                if (!string.IsNullOrWhiteSpace(regex))
                    binding.ValidationRules.Add(new RegexnRule()
                    {
                        RegularExpression = regex,
                        ErrorText = string.Format(column.ExtendedProperties[ExtendedPropertyNames.ErrorText]!.ToString()!, column.Caption)
                    });

                if (controlType == DataControlType.Default)
                {
                    // 生成编辑控件。
                    if ((dataType == typeof(long) || dataType == typeof(Guid)) && columnName.EndsWith("_Id") && EntityDataHelper.IsEntityColumn(columnName))
                    {
                        result = new EntityBox
                        {
                            IsRequired = isRequired,
                            EntityName = column.Caption,
                            ValuePath = columnName,
                            DisplayPath = $"{columnName[..^3]}_Name",
                            ServiceParameter = parameters == null ? columnName[..^3] : parameters["ServiceParameter"]
                        };
                        result.SetBinding(EntityBox.ValueProperty, binding);
                    }
                    else if (dataType == typeof(long))
                        result = CreateUpDown<LongUpDown, long>(columnName, maximum, minimum, binding);
                    //{
                    //    result = new LongUpDown
                    //    {
                    //        Maximum = Utils.TypeConvert.ChangeNullableType<long>(maximum),
                    //        Minimum = Utils.TypeConvert.ChangeNullableType<long>(minimum)
                    //    };

                    //    if (columnName.StartsWith(beginPrefix))
                    //    {
                    //        ((LongUpDown)result).ValueChanged += BeginControlChanged;
                    //        result.Name = columnName;
                    //    }
                    //    else if (columnName.StartsWith(endPrefix))
                    //    {
                    //        ((LongUpDown)result).ValueChanged += EndControlChanged;
                    //        result.Name = columnName;
                    //    }

                    //    result.SetBinding(LongUpDown.ValueProperty, binding);
                    //}
                    else if (dataType == typeof(int))
                        result = CreateUpDown<IntegerUpDown, int>(columnName, maximum, minimum, binding);
                    else if (dataType == typeof(short))
                        result = CreateUpDown<ShortUpDown, short>(columnName, maximum, minimum, binding);
                    else if (dataType == typeof(byte))
                        result = CreateUpDown<ByteUpDown, byte>(columnName, maximum, minimum, binding);
                    else if (dataType == typeof(double))
                        result = CreateUpDown<DoubleUpDown, double>(columnName, maximum, minimum, binding);
                    else if (dataType == typeof(float))
                        result = CreateUpDown<SingleUpDown, float>(columnName, maximum, minimum, binding);
                    else if (dataType == typeof(decimal))
                        result = dbType == DbType.Currency
                            ? CreateUpDown<CalculatorUpDown, decimal>(columnName, maximum, minimum, binding)
                            : CreateUpDown<DecimalUpDown, decimal>(columnName, maximum, minimum, binding);
                    else if (dataType == typeof(bool))
                    {
                        result = new CheckBox();
                        result.SetBinding(ToggleButton.IsCheckedProperty, binding);
                    }
                    else if (dataType == typeof(DateTime) || dataType == typeof(DateTimeOffset))
                        switch (dbType)
                        {
                            case DbType.Date:
                                var datePicker = columnName.EndsWith("_Year_Month") ? new YearMonthPicker() : new DatePicker();
                                result = datePicker;
                                result.SetBinding(DatePicker.SelectedDateProperty, binding);

                                if (columnName.StartsWith(beginPrefix))
                                {
                                    datePicker.SelectedDateChanged += BeginSelectionChanged;
                                    result.Name = columnName;
                                }
                                else if (columnName.StartsWith(endPrefix))
                                {
                                    datePicker.SelectedDateChanged += EndSelectionChanged;
                                    result.Name = columnName;
                                }

                                break;
                            case DbType.Time:
                                result = CreateUpDown<TimePicker, DateTime>(columnName, maximum, minimum, binding);
                                //result = new TimePicker
                                //{
                                //    Maximum = Utils.TypeConvert.ChangeNullableType<DateTime>(maximum),
                                //    Minimum = Utils.TypeConvert.ChangeNullableType<DateTime>(minimum)
                                //};

                                //if (columnName.StartsWith(beginPrefix))
                                //{
                                //    ((TimePicker)result).ValueChanged += BeginControlChanged;
                                //    result.Name = columnName;
                                //}
                                //else if (columnName.StartsWith(endPrefix))
                                //{
                                //    ((TimePicker)result).ValueChanged += EndControlChanged;
                                //    result.Name = columnName;
                                //}

                                //result.SetBinding(TimePicker.ValueProperty, binding);
                                break;
                            default:
                                result = CreateUpDown<DateTimePicker, DateTime>(columnName, maximum, minimum, binding);
                                break;
                        }
                    else if (dataType == typeof(TimeSpan))
                        result = CreateUpDown<TimeSpanUpDown, TimeSpan>(columnName, maximum, minimum, binding);
                    else if (dataType == typeof(sbyte))
                    {
                        result = new EnumComboBox
                        {
                            Name = columnName,
                            EnumName = columnName,  // .Replace("_", string.Empty)
                            SelectedValuePath = nameof(EnumItem.Value),
                            DisplayMemberPath = nameof(EnumItem.DisplayName),
                        };
                        result.SetBinding(EnumComboBox.SelectedValueProperty, binding);
                    }
                    else if (dataType == typeof(ulong))
                        result = CreateUpDown<ULongUpDown, ulong>(columnName, maximum, minimum, binding);
                    else if (dataType == typeof(uint))
                        result = CreateUpDown<UIntegerUpDown, uint>(columnName, maximum, minimum, binding);
                    else if (dataType == typeof(ushort))
                        result = CreateUpDown<UShortUpDown, ushort>(columnName, maximum, minimum, binding);
                    else if (dataType == typeof(string) && columnName == "Name" && column.Table!.Columns.Contains("MnemonicCode"))
                    {
                        result = new NameBox();
                        if (column.MaxLength >= 0)
                            ((TextBox)result).MaxLength = column.MaxLength;
                        var mnemonicCodeBinding = new Binding("MnemonicCode")
                        {
                            Source = ItemsSource ?? DataContext,
                            Mode = BindingMode.OneWayToSource
                        };  // 创建数据绑定。
                        result.SetBinding(NameBox.MnemonicCodeProperty, mnemonicCodeBinding);

                        result.SetBinding(TextBox.TextProperty, binding);
                    }
                    else
                    {
                        var textBox = new TextBox();
                        result = textBox;
                        if (column.MaxLength >= 0)
                            ((TextBox)result).MaxLength = column.MaxLength;
                        result.SetBinding(TextBox.TextProperty, binding);

                        //if (dataType == typeof(string) && columnName == "Name" && column.Table.Columns.Contains("MnemonicCode"))
                        //    textBox.TextChanged += TextBox_TextChanged;
                    }
                }
                else
                    switch (controlType)
                    {
                        case DataControlType.EntityBox:         // 实体框。
                            result = new EntityBox
                            {
                                EntityName = column.Caption,
                                ValuePath = parameters == null ? "Id" : parameters["ValuePath"],
                                DisplayPath = parameters == null ? "Name" : parameters["DisplayPath"],
                                ServiceParameter = parameters == null ? columnName.EndsWith("_Id") ? columnName[..^3] : columnName : parameters["ServiceParameter"],
                                IsRequired = isRequired
                            };
                            result.SetBinding(EntityBox.ValueProperty, binding);
                            break;
                        case DataControlType.SinglechoiceBox:   // 单选框。
                            result = new SinglechoiceBox
                            {
                                ItemData = GlobalCommon.EnumDictionary[(column.ExtendedProperties[ExtendedPropertyNames.Parameters] ?? columnName).ToString()!]
                            };
                            result.SetBinding(ChoiceBox.ValueProperty, binding);
                            break;
                        case DataControlType.MultichoiceBox:    // 多选框。
                            result = new MultichoiceBox
                            {
                                ItemData = GlobalCommon.EnumDictionary[(column.ExtendedProperties[ExtendedPropertyNames.Parameters] ?? columnName).ToString()!]
                            };
                            result.SetBinding(ChoiceBox.ValueProperty, binding);
                            break;
                        default:
                            result = null;
                            break;
                    }
            }

            if (result is ChoiceBox choiceBox && choiceBox.ItemData.Count > 3)
                result = new ScrollViewer()
                {
                    Content = result,
                    VerticalScrollBarVisibility = ScrollBarVisibility.Auto,
                    Height = 23D
                };

            return result;
        }

        private static void ValueChanged(object? sender, string prefix, string related)
        {
            FrameworkElement frameworkElement = (FrameworkElement)sender!;
            var name = frameworkElement.Name;

            Debug.Assert(name.StartsWith(prefix), $"范围开始控件名【{name}】不是以【{prefix}】开始。");

            var endControl = ((FrameworkElement)(frameworkElement).Parent).FindName($"{related}{name.Remove(5)}");
            if (endControl == null)
                return;

            var beginValue = sender!.GetPropertyValue("Value") as IComparable;
            var endValue = endControl.GetPropertyValue("Value");

            Debug.Assert(beginValue != null, "范围开始控件的值不可比较。");

            if (endValue != null && beginValue.CompareTo(endValue) > 0)
                endControl.SetPropertyValue("Value", beginValue);
        }

        private static void BeginValueChanged(object? sender) => ValueChanged(sender, "Begin", "End");

        private static void EndValueChanged(object? sender) => ValueChanged(sender, "End", "Begin");

        private static void BeginControlChanged(object sender, RoutedPropertyChangedEventArgs<object> e) => BeginValueChanged(sender);

        private static void EndControlChanged(object sender, RoutedPropertyChangedEventArgs<object> e) => EndValueChanged(sender);

        private void BeginSelectionChanged(object? sender, SelectionChangedEventArgs e) => BeginValueChanged(sender);

        private void EndSelectionChanged(object? sender, SelectionChangedEventArgs e) => EndValueChanged(sender);
    }
}
