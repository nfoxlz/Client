using System.Collections.Generic;

namespace Compete.Mis.Provider
{
    internal class EumnDataProvider : Enums.IEumnDataProvider
    {
        public IEnumerable<Enums.EnumInfo> GetEnums() => Frame.Services.GlobalServices.FrameService.GetEnums();
    }
}
