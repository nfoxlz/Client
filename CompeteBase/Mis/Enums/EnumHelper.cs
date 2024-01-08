using System.Collections.Generic;
using System.Linq;

namespace Compete.Mis.Enums
{
    public static class EnumHelper
    {
        private static IEnumerable<EnumInfo>? enums;

        public static void Initialize(IEumnDataProvider provider) => enums = provider.GetEnums();

        internal static IList<EnumItem> GetEnum(string name) => (from info in enums
                                                                 where info.Name == name
                                                                 orderby info.Sn, info.Value
                                                                 select new EnumItem { Value = info.Value, DisplayName = info.DisplayName }).ToList();
    }
}
