using System;

namespace Compete.Mis.Provider
{
    internal class DateTimeProvider : MemoryData.IServerDateTimeProvider
    {
#if JAVA_LANGUAGE
        public DateTime GetServerDateTime() => Utils.JavaHelper.ToDateTime(Frame.Services.GlobalServices.FrameService.GetServerDateTime()); // Java
#else
        public DateTime GetServerDateTime() => Frame.Services.GlobalServices.FrameService.GetServerDateTime();
#endif
    }
}
