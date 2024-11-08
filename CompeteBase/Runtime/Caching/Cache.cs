using System;
using System.Configuration;
using System.Runtime.Caching;

namespace Compete.Runtime.Caching
{
    public sealed class Cache<K, V>
    {
        //private readonly ConcurrentDictionary<K, CacheItem<V>> dictionary = new ConcurrentDictionary<K, CacheItem<V>>();
        private static readonly TimeSpan DefaultExpiration;

        static Cache()
        {
            var cacheExpiration = ConfigurationManager.AppSettings["CacheExpiration"];
            DefaultExpiration = !string.IsNullOrWhiteSpace(cacheExpiration) && double.TryParse(cacheExpiration, out double expiration)
                ? TimeSpan.FromMicroseconds(expiration)
                : TimeSpan.FromSeconds(10D);
        }

        private readonly MemoryCache memoryCache;

        private readonly Func<K, V> getFunc;

        public Cache(string name, Func<K, V> func)
        {
            memoryCache = new MemoryCache(name);
            getFunc = func;

            var cacheExpiration = ConfigurationManager.AppSettings["CacheExpiration"];
            if (!string.IsNullOrWhiteSpace(cacheExpiration) && double.TryParse(cacheExpiration, out double expiration))
                Expiration = TimeSpan.FromMicroseconds(expiration);
        }

        public TimeSpan Expiration { get; set; } = DefaultExpiration;

        public V GetValue(K key)
        {
            var keyString = key!.ToString();
            if (memoryCache.Contains(keyString))
                return (V)memoryCache[keyString];

            var result = getFunc(key);
            memoryCache.Add(keyString, result,
                new CacheItemPolicy
                {
                    AbsoluteExpiration = DateTimeOffset.Now.Add(Expiration) // 设置缓存项的过期时间
                });

            return result;
        }

        public void Remove(K key) => memoryCache.Remove(key!.ToString());
    }
}
