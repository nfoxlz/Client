using System;
using System.Collections.Generic;

namespace Compete.Mis.Frame.Services
{
    public interface IFrameService
    {
        IEnumerable<ServiceModels.MenuSetting> GetMenus();

        IEnumerable<Enums.EnumInfo> GetEnums();

#if JAVA_LANGUAGE
        long GetServerDateTime();   // Java

        long? GetAccountingDate();
#else
        DateTime GetServerDateTime();

        DateTime? GetAccountingDate();
#endif

        bool ModifyPassword(string originalPassword, string newPassword);

        IDictionary<string, string> GetSettings();

        bool IsFinanceClosed();

        bool IsFinanceClosedByDate(int periodYearMonth);

#if DEBUG
        void ClearCache();
#endif
    }
}
