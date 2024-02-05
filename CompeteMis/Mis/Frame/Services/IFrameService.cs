using System;
using System.Collections.Generic;

namespace Compete.Mis.Frame.Services
{
    internal interface IFrameService
    {
        IEnumerable<ServiceModels.MenuSetting> GetMenus();

        IEnumerable<Enums.EnumInfo> GetEnums();

#if JAVA_LANGUAGE
        long GetServerDateTime();   // Java
#else
        DateTime GetServerDateTime();
#endif


#if DEBUG
        void ClearCache();
#endif
    }
}
