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
                    {
                        result = new LongUpDown
                        {
                            Maximum = Utils.TypeConvert.ChangeNullableType<long>(maximum),
                            Minimum = Utils.TypeConvert.ChangeNullableType<long>(minimum)
                        };

                        if (columnName.StartsWith(beginPrefix))
                        {
                            ((LongUpDown)result).ValueChanged += BeginControlChanged;
                            result.Name = columnName;
                        }
                        else if (columnName.StartsWith(endPrefix))
                        {
                            ((LongUpDown)result).ValueChanged += EndControlChanged;
                            result.Name = columnName;
                        }

                        result.SetBinding(LongUpDown.ValueProperty, binding);
                    }
                    else if (dataType == typeof(int))
                    {
                        result = new IntegerUpDown
                        {
                            Maximum = Utils.TypeConvert.ChangeNullableType<int>(maximum),
                            Minimum = Utils.TypeConvert.ChangeNullableType<int>(minimum)
                        };

                        if (columnName.StartsWith(beginPrefix))
                        {
                            ((IntegerUpDown)result).ValueChanged += BeginControlChanged;
                            result.Name = columnName;
                        }
                        else if (columnName.StartsWith(endPrefix))
                        {
                            ((IntegerUpDown)result).ValueChanged += EndControlChanged;
                            result.Name = columnName;
                        }

                        result.SetBinding(IntegerUpDown.ValueProperty, binding);
                    }
                    else if (dataType == typeof(short))
                    {
                        result = new ShortUpDown
                        {
                            Maximum = Utils.TypeConvert.ChangeNullableType<short>(maximum),
                            Minimum = Utils.TypeConvert.ChangeNullableType<short>(minimum)
                        };

                        if (columnName.StartsWith(beginPrefix))
                        {
                            ((ShortUpDown)result).ValueChanged += BeginControlChanged;
                            result.Name = columnName;
                        }
                        else if (columnName.StartsWith(endPrefix))
                        {
                            ((ShortUpDown)result).ValueChanged += EndControlChanged;
                            result.Name = columnName;
                        }

                        result.SetBinding(ShortUpDown.ValueProperty, binding);
                    }
                    else if (dataType == typeof(byte))
                    {
                        result = new ByteUpDown
                        {
                            Maximum = Utils.TypeConvert.ChangeNullableType<byte>(maximum),
                            Minimum = Utils.TypeConvert.ChangeNullableType<byte>(minimum)
                        };

                        if (columnName.StartsWith(beginPrefix))
                        {
                            ((ByteUpDown)result).ValueChanged += BeginControlChanged;
                            result.Name = columnName;
                        }
                        else if (columnName.StartsWith(endPrefix))
                        {
                            ((ByteUpDown)result).ValueChanged += EndControlChanged;
                            result.Name = columnName;
                        }

                        result.SetBinding(ByteUpDown.ValueProperty, binding);
                    }
                    else if (dataType == typeof(double))
                    {
                        result = new DoubleUpDown
                        {
                            Maximum = Utils.TypeConvert.ChangeNullableType<double>(maximum),
                            Minimum = Utils.TypeConvert.ChangeNullableType<double>(minimum)
                        };

                        if (columnName.StartsWith(beginPrefix))
                        {
                            ((DoubleUpDown)result).ValueChanged += BeginControlChanged;
                            result.Name = columnName;
                        }
                        else if (columnName.StartsWith(endPrefix))
                        {
                            ((DoubleUpDown)result).ValueChanged += EndControlChanged;
                            result.Name = columnName;
                        }

                        result.SetBinding(DoubleUpDown.ValueProperty, binding);
                    }
                    else if (dataType == typeof(float))
                    {
                        result = new SingleUpDown
                        {
                            Maximum = Utils.TypeConvert.ChangeNullableType<float>(maximum),
                            Minimum = Utils.TypeConvert.ChangeNullableType<float>(minimum)
                        };

                        if (columnName.StartsWith(beginPrefix))
                        {
                            ((SingleUpDown)result).ValueChanged += BeginControlChanged;
                            result.Name = columnName;
                        }
                        else if (columnName.StartsWith(endPrefix))
                        {
                            ((SingleUpDown)result).ValueChanged += EndControlChanged;
                            result.Name = columnName;
                        }

                        result.SetBinding(SingleUpDown.ValueProperty, binding);
                    }
                    else if (dataType == typeof(decimal))
                    {
                        if (dbType == DbType.Currency)
                        {
                            result = new CalculatorUpDown();
                            result.SetBinding(CalculatorUpDown.ValueProperty, binding);
                        }
                        else
                        {
                            result = new DecimalUpDown();
                            result.SetBinding(DecimalUpDown.ValueProperty, binding);
                        }
                        var decimalUpDown = (DecimalUpDown)result;
                        decimalUpDown.Maximum = Utils.TypeConvert.ChangeNullableType<decimal>(maximum);
                        decimalUpDown.Minimum = Utils.TypeConvert.ChangeNullableType<decimal>(minimum);

                        if (columnName.StartsWith(beginPrefix))
                        {
                            decimalUpDown.ValueChanged += BeginControlChanged;
                            result.Name = columnName;
                        }
                        else if (columnName.StartsWith(endPrefix))
                        {
                            decimalUpDown.ValueChanged += EndControlChanged;
                            result.Name = columnName;
                        }
                    }
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
                                result = new TimePicker
                                {
                                    Maximum = Utils.TypeConvert.ChangeNullableType<DateTime>(maximum),
                                    Minimum = Utils.TypeConvert.ChangeNullableType<DateTime>(minimum)
                                };

                                if (columnName.StartsWith(beginPrefix))
                                {
                                    ((TimePicker)result).ValueChanged += BeginControlChanged;
                                    result.Name = columnName;
                                }
                                else if (columnName.StartsWith(endPrefix))
                                {
                                    ((TimePicker)result).ValueChanged += EndControlChanged;
                                    result.Name = columnName;
                                }

                                result.SetBinding(TimePicker.ValueProperty, binding);
                                break;
                            default:
                                result = new DateTimePicker
                                {
                                    Maximum = Utils.TypeConvert.ChangeNullableType<DateTime>(maximum),
                                    Minimum = Utils.TypeConvert.ChangeNullableType<DateTime>(minimum)
                                };

                                if (columnName.StartsWith(beginPrefix))
                                {
                                    ((DateTimePicker)result).ValueChanged += BeginControlChanged;
                                    result.Name = columnName;
                                }
                                else if (columnName.StartsWith(endPrefix))
                                {
                                    ((DateTimePicker)result).ValueChanged += EndControlChanged;
                                    result.Name = columnName;
                                }

                                result.SetBinding(DateTimePicker.ValueProperty, binding);
                                break;
                        }
                    else if (dataType == typeof(TimeSpan))
                    {
                        result = new TimeSpanUpDown
                        {
                            Maximum = Utils.TypeConvert.ChangeNullableType<TimeSpan>(maximum),
                            Minimum = Utils.TypeConvert.ChangeNullableType<TimeSpan>(minimum)
                        };

                        if (columnName.StartsWith(beginPrefix))
                        {
                            ((TimeSpanUpDown)result).ValueChanged += BeginControlChanged;
                            result.Name = columnName;
                        }
                        else if (columnName.StartsWith(endPrefix))
                        {
                            ((TimeSpanUpDown)result).ValueChanged += EndControlChanged;
                            result.Name = columnName;
                        }

                        result.SetBinding(TimeSpanUpDown.ValueProperty, binding);
                    }
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
