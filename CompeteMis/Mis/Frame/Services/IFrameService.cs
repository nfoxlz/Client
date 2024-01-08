using System.Collections.Generic;

namespace Compete.Mis.Frame.Services
{
    internal interface IFrameService
    {
        IEnumerable<ServiceModels.MenuSetting> GetMenus();

        IEnumerable<Enums.EnumInfo> GetEnums();

        long GetServerDateTime();

#if DEBUG
        void ClearCache();
#endif
    }
}
