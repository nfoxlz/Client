using System.Collections.Generic;

namespace Compete.Mis.Enums
{
    public interface IEumnDataProvider
    {
        IEnumerable<Enums.EnumInfo> GetEnums();
    }
}
