using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;

namespace Surveillance.EventBus.Caching
{
    public class CacheService
    {
        private readonly IDistributedCache _cache;

        public CacheService(IDistributedCache cache)
        {
            _cache = cache;
        }

        public async Task<T?> GetAsync<T>(string key)
        {
            var data = await _cache.GetStringAsync(key);
            return data == null ? default : JsonSerializer.Deserialize<T>(data);
        }

        public async Task SetAsync<T>(string key, T value)
        {
            await _cache.SetStringAsync(key,
                JsonSerializer.Serialize(value),
                new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5)
                });
        }
    }
}
