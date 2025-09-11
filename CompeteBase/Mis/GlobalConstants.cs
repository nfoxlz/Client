using System.Configuration;
using System.Windows.Media;

namespace Compete.Mis
{
    public static class GlobalConstants
    {
        public const ushort DefaultNavigatorPageSize = 30;

        public const double TextBoxHeight = 24D;

        //public const double SmallTextBoxHeight = 18D;

        public const string NavigatorMaxPageNoFormat = "/{0}";

        public const string EntityBoxParameterValuePath = "ValuePath";

        public const string EntityBoxParameterDisplayPath = "DisplayPath";

        public const string EntityBoxParameterServiceParameter = "ServiceParameter";

        public static readonly ushort[] DefaultNavigatorPageCollection = [10, 20, 30, 50, 100, 200];

        public static readonly Brush RequiredBrush;

        public static readonly Brush CanWriteBrush; // DataGrid

        public static readonly Brush ReadOnlyBrush; // DataPanel

        static GlobalConstants()
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
