using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace UITemplate
{
    /// <summary>
    /// Bill.xaml 的交互逻辑
    /// </summary>
    public partial class Bill : UserControl
    {
        public Bill()
        {
            InitializeComponent();
        }

        private void DatePicker_CalendarOpened(object sender, RoutedEventArgs e)
        {
            var datePicker = (DatePicker)sender;
            var popup = (Popup)datePicker.Template.FindName("PART_Popup", datePicker);
            var calendar = ((Calendar)popup.Child);
            calendar.DisplayMode = CalendarMode.Year;

            calendar.DisplayModeChanged += Calendar_DisplayModeChanged;

            datePicker.ApplyTemplate();
            var textBox = (TextBox)datePicker.Template.FindName("PART_TextBox", datePicker);
            //textBox.Text = DatePickerDateTimeConverter.DateTimeToString("yyyy-MM", datePicker.SelectedDate);
            textBox.Text = datePicker.SelectedDate.HasValue? datePicker.SelectedDate.Value.ToString("yyyy-MM") : null;
        }

        private void Calendar_DisplayModeChanged(object? sender, CalendarModeChangedEventArgs e)
        {
            var calendar = (Calendar)sender!;
            if (calendar.DisplayMode != CalendarMode.Month)
                return;

            calendar.SelectedDate = GetSelectedCalendarDate(calendar.DisplayDate);
            //calendar.SelectedDate = calendar.DisplayDate;

            //var datePicker = GetCalendarsDatePicker(calendar);
            mainDatePicker.IsDropDownOpen = false;
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {

            //var popup = (Popup)mainDatePicker.Template.FindName("PART_Popup", mainDatePicker);
            //var calendar = ((Calendar)popup.Child);
            //calendar.DisplayMode = CalendarMode.Month;

        }

        private void MainDatePicker_CalendarClosed(object sender, RoutedEventArgs e)
        {
            var datePicker = (DatePicker)sender;
            var popup = (Popup)datePicker.Template.FindName("PART_Popup", datePicker);
            var calendar = ((Calendar)popup.Child);

            mainDatePicker.SelectedDate = calendar.SelectedDate;

            calendar.DisplayModeChanged -= Calendar_DisplayModeChanged;
        }

        private static DateTime? GetSelectedCalendarDate(DateTime? selectedDate)
        {
            if (!selectedDate.HasValue)
                return null;
            return new DateTime(selectedDate.Value.Year, selectedDate.Value.Month, 1);
        }
    }
}
