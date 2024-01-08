using System;

namespace Compete.Mis.Provider
{
    internal class DateTimeProvider : MemoryData.IServerDateTimeProvider
    {
        public DateTime GetServerDateTime() => Utils.JavaHelper.ToDateTime(Frame.Services.GlobalServices.FrameService.GetServerDateTime());
    }
}
