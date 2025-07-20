using System.Collections.Generic;
using System.Windows.Media;

namespace Compete.Mis.Chart
{
    public sealed class ChartSetting : ViewModels.ViewModelBase
    {
        public ChartTypeSetting? TypeSetting { get; set; } = ChartTypeSetting.DefaultChartTypeSetting;

        public Color? FillColor { get; set; }

        public Color? LineColor { get; set; }

        public IDictionary<string,string> ColumnNames { get; } = new OrderedDictionary<string, string>();

        public IEnumerable<ChartTypeSetting> TypeSettings { get; } = ChartTypeSetting.Settings.Values;
    }
}
