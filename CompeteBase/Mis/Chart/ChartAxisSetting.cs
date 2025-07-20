using System.Collections.Generic;

namespace Compete.Mis.Chart
{
    public sealed class ChartAxisSetting
    {
        public IList<string> DataPath { get; set; } = new List<string>();

        public uint? FillColor { get; set; }

        public uint? LineColor { get; set; }
    }
}
