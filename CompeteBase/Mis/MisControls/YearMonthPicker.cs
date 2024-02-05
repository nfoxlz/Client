using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
using Calendar = System.Windows.Controls.Calendar;

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
    ///     <MyNamespace:YearMonthPicker/>
    ///
    /// </summary>
    public class YearMonthPicker : DatePicker
    {
        //static YearMonthPicker()
        //{
        //    DefaultStyleKeyProperty.OverrideMetadata(typeof(DatePicker), new FrameworkPropertyMetadata(typeof(DatePicker)));
        //}

        private TextBox GetTemplateTextBox()
        {
            ApplyTemplate();
            return (TextBox)Template.FindName("PART_TextBox", this);
        }

        private static Calendar GetDatePickerCalendar(object sender)
        {
            var datePicker = (DatePicker)sender;
            return (Calendar)((Popup)datePicker.Template.FindName("PART_Popup", datePicker)).Child;
        }

        private static YearMonthPicker GetCalendarsDatePicker(FrameworkElement child)
        {
            var parent = (FrameworkElement)child.Parent;
            if (parent.Name == "PART_Root")
                return (YearMonthPicker)parent.TemplatedParent;
            return GetCalendarsDatePicker(parent);
        }

        private static DateTime? GetSelectedCalendarDate(DateTime? selectedDate)
        {
            if (!selectedDate.HasValue)
                return null;
            return new DateTime(selectedDate.Value.Year, selectedDate.Value.Month, 1);
        }

        public YearMonthPicker()
        {
            Loaded += (sender, e) => BindingTextBox();

            CalendarOpened += (sender, e) =>
            {
                /* When DatePicker's TextBox is not focused and its Calendar is opened by clicking its calendar button 
                 * its text will be the result of its internal date parsing until its TextBox is focused and another 
                 * date is selected. A workaround is to set this string when it is opened. */

                var textBox = GetTemplateTextBox();
                textBox.Text = YearMonthConverter.DateTimeToString(DateForamt, SelectedDate);

                var calendar = GetDatePickerCalendar(this);
                calendar.DisplayMode = CalendarMode.Year;
                calendar.DisplayModeChanged += Calendar_DisplayModeChanged;
            };

            CalendarClosed += (sender, e) =>
            {
                var datePicker = (YearMonthPicker)sender;
                var calendar = GetDatePickerCalendar(sender);
                datePicker.SelectedDate = calendar.SelectedDate;

                calendar.DisplayModeChanged -= Calendar_DisplayModeChanged;
            };
        }

        public string DateForamt
        {
            get { return (string)GetValue(DateForamtProperty); }
            set { SetValue(DateForamtProperty, value); }
        }

        // Using a DependencyProperty as the backing store for DateForamt.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DateForamtProperty =
            DependencyProperty.Register(nameof(DateForamt), typeof(string), typeof(YearMonthPicker), new PropertyMetadata(GlobalCommon.GetMessageOrDefault("YearMonthStringForamt", "yyyy/MM"), (d, e) => ((YearMonthPicker)d).BindingTextBox()));

        private void BindingTextBox()
        {
            var textBox = GetTemplateTextBox();
            textBox.SetBinding(TextBox.TextProperty, new Binding(nameof(SelectedDate))
            {
                RelativeSource = new RelativeSource { AncestorType = typeof(DatePicker) },
                Converter = new YearMonthConverter(),
                ConverterParameter = new Tuple<DatePicker, string>(this, DateForamt)
            });

            textBox.PreviewKeyDown -= TextBox_PreviewKeyDown;
            textBox.PreviewKeyDown += TextBox_PreviewKeyDown;
        }

        private static void TextBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key != Key.Return)
                return;

            /* DatePicker subscribes to its TextBox's KeyDown event to set its SelectedDate if Key.Return was 
             * pressed. When this happens its text will be the result of its internal date parsing until it 
             * loses focus or another date is selected. A workaround is to stop the KeyDown event bubbling up 
             * and handling setting the DatePicker.SelectedDate. */

            e.Handled = true;

            var textBox = (TextBox)sender;
            var yearMonthPicker = (YearMonthPicker)textBox.TemplatedParent;
            var dateStr = textBox.Text;
            yearMonthPicker.SelectedDate = YearMonthConverter.StringToDateTime(yearMonthPicker, yearMonthPicker.DateForamt, dateStr);
        }

        private static void Calendar_DisplayModeChanged(object? sender, CalendarModeChangedEventArgs e)
        {
            var calendar = (Calendar)sender!;
            if (calendar.DisplayMode != CalendarMode.Month)
                return;

            calendar.SelectedDate = GetSelectedCalendarDate(calendar.DisplayDate);

            var datePicker = GetCalendarsDatePicker(calendar);
            datePicker.IsDropDownOpen = false;
        }
    }
}
