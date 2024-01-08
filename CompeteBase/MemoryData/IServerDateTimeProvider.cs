using System;

namespace Compete.MemoryData
{
    public interface IServerDateTimeProvider
    {
        DateTime GetServerDateTime();

        DateTime GetServerDate() => GetServerDateTime().Date;

        TimeSpan GetServerTime() => GetServerDateTime().TimeOfDay;
    }
}
