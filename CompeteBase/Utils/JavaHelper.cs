#if JAVA_LANGUAGE

using System;
using System.Collections.Generic;

namespace Compete.Utils
{
    public static class JavaHelper    // Java
    {
        private static readonly DateTime startTime = DateTime.SpecifyKind(new DateTime(1970, 1, 1), DateTimeKind.Utc);

        public static DateTime ToDateTime(long timeStamp) => startTime.Add(new TimeSpan(timeStamp * 10000L)).ToLocalTime();

        public static long ConvertDateTime(DateTime dateTime) => (long)(TimeZoneInfo.ConvertTime(dateTime, TimeZoneInfo.Utc) - startTime).TotalMilliseconds;

        public static DateTimeOffset ToDateTimeOffset(long timeStamp) => ToDateTime(timeStamp);

        public static long ConvertDateTime(DateTimeOffset dateTimeOffset) => ConvertDateTime(dateTimeOffset.DateTime);

        //private static readonly DateTimeOffset startTimeOffset = TimeZoneInfo.ConvertTime(new DateTime(1970, 1, 1), TimeZoneInfo.Utc);

        //public static DateTimeOffset ToDateTimeOffset(long timeStamp) => startTimeOffset.Add(new TimeSpan(timeStamp * 10000L));// TimeZoneInfo.ConvertTime(ToDateTime(timeStamp), TimeZoneInfo.Local);

        //public static DateTime ToDateTime(long timeStamp) => TimeZoneInfo.ConvertTime(ToDateTimeOffset(timeStamp).DateTime, TimeZoneInfo.Local);

        //public static long ConvertDateTime(DateTimeOffset dateTime) => (long)(dateTime - startTimeOffset).TotalMilliseconds;

        ////public static long ConvertDateTime(DateTime dateTime) => ConvertDateTime((DateTimeOffset)TimeZoneInfo.ConvertTime(dateTime, TimeZoneInfo.Local));

        //private static readonly DateTime startTime = TimeZoneInfo.ConvertTime(new DateTime(1970, 1, 1), TimeZoneInfo.Utc);

        //public static long ConvertDateTime(DateTime dateTime) => (long)(dateTime - startTime).TotalMilliseconds;


        public static IDictionary<string, object>? Convert(IDictionary<string, object>? data)
        {
            if (null == data)
                return null;

            var result = new Dictionary<string, object>();
            foreach (var item in data)
                if (item.Value is DateTime dateTime)
                    result.Add(item.Key, ConvertDateTime(dateTime));
                else if (item.Value is DateTimeOffset dateTimeOffset)
                    result.Add(item.Key, ConvertDateTime(dateTimeOffset));
                else
                    result.Add(item.Key, item.Value);
            return result;
        }
    }
}

#endif