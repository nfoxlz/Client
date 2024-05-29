using System;

namespace Compete.Mis.Provider
{
    internal class DateTimeProvider : MemoryData.IServerDateTimeProvider
    {
#if JAVA_LANGUAGE
        public DateTime GetServerDateTime() => Utils.JavaHelper.ToDateTime(Frame.Services.GlobalServices.FrameService.GetServerDateTime()); // Java
        public DateTime GetAccountingDate() => Utils.JavaHelper.ToDateTime(Frame.Services.GlobalServices.FrameService.GetAccountingDate());

#else
        public DateTime GetServerDateTime() => Frame.Services.GlobalServices.FrameService.GetServerDateTime();

        public DateTime GetAccountingDate() => Frame.Services.GlobalServices.FrameService.GetAccountingDate();
#endif
    }
}
