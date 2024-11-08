using System;

namespace Compete.Runtime.Caching
{
    internal struct CacheItem<T>//class record struct
    {
        public T? Value { get; set; }

        public DateTimeOffset AbsoluteExpiration { get; set; }
    }
}
