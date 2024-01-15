using System;
using System.Globalization;
using System.Windows.Controls;
using System.Windows.Data;

namespace Compete.Mis.MisControls
{
    internal class YearMonthConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) => DateTimeToString(((Tuple<DatePicker, string>)parameter).Item2, (DateTime?)value) ?? string.Empty;

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var tupleParam = (Tuple<DatePicker, string>)parameter;
            return StringToDateTime(tupleParam.Item1, tupleParam.Item2, (string)value) ?? DateTime.Now;
        }

        public static string? DateTimeToString(string format, DateTime? selectedDate) => selectedDate.HasValue ? selectedDate.Value.ToString(format) : null;

        public static DateTime? StringToDateTime(DatePicker datePicker, string format, string dateString)
        {
            var canParse = DateTime.TryParseExact(dateString, format, CultureInfo.CurrentCulture, DateTimeStyles.None, out DateTime date);

            if (!canParse)
                canParse = DateTime.TryParse(dateString, CultureInfo.CurrentCulture, DateTimeStyles.None, out date);

            return canParse ? date : datePicker.SelectedDate;
        }
    }
}
