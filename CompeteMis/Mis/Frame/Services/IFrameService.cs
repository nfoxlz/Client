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

        long GetAccountingDate();
#else
        DateTime GetServerDateTime();

        DateTime GetAccountingDate();
#endif

#if DEBUG
        void ClearCache();
#endif

        IDictionary<string, string> GetConfigurations();
    }
}
