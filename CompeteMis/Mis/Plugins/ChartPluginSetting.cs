using System.Collections.Generic;

namespace Compete.Mis.Plugins
{
    public sealed class ChartPluginSetting
    {
        public Chart.ChartType Type { get; set; }

        public uint? FillColor { get; set; }

        public uint? LineColor { get; set; }

        public IEnumerable<string>? ColumnNames { get; set; }
    }
}
