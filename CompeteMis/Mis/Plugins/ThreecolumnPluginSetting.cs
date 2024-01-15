using System.Collections.Generic;

namespace Compete.Mis.Plugins
{
    public class ThreecolumnPluginSetting : DataPluginSetting
    {
        public IDictionary<string, IEnumerable<ThreecolumnSetting>>? CalculatedColumns { get; set; }
    }
}
