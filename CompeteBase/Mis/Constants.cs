using System.Configuration;
using System.Windows.Media;

namespace Compete.Mis
{
    internal static class Constants
    {
        public const ushort DefaultNavigatorPageSize = 30;

        public const string NavigatorMaxPageNoFormat = "/{0}";

        public static readonly ushort[] DefaultNavigatorPageCollection = [10, 30, 50, 100, 200];

        public static readonly Brush RequiredBrush;

        public static readonly Brush CanWriteBrush; // DataGrid

        public static readonly Brush ReadOnlyBrush; // DataPanel

        static Constants()
        {
            var requiredColor = ConfigurationManager.AppSettings["RequiredColor"];
            RequiredBrush = string.IsNullOrWhiteSpace(requiredColor) ? Brushes.DarkRed : new SolidColorBrush((Color)ColorConverter.ConvertFromString(requiredColor));

            var canWriteColor = ConfigurationManager.AppSettings["CanWriteColor"];
            CanWriteBrush = string.IsNullOrWhiteSpace(canWriteColor) ? Brushes.DarkBlue : new SolidColorBrush((Color)ColorConverter.ConvertFromString(canWriteColor));

            var readOnlyColor = ConfigurationManager.AppSettings["ReadOnlyColor"];
            ReadOnlyBrush = string.IsNullOrWhiteSpace(readOnlyColor) ? Brushes.DimGray : new SolidColorBrush((Color)ColorConverter.ConvertFromString(readOnlyColor));
        }
    }
}
