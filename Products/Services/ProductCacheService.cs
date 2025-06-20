using StackExchange.Redis;
using System.Text.Json;

namespace ProductsApi.Services
{
    public class ProductCacheService
    {
        private readonly IDatabase _cache;
        private readonly TimeSpan _defaultCacheDuration;

        public ProductCacheService(IConnectionMultiplexer redis, IConfiguration config)
        {
            _cache = redis.GetDatabase();
            _defaultCacheDuration = TimeSpan.FromSeconds(config.GetValue("Redis:AbsoluteCacheTTLSeconds", 300));
        }

        public async Task CacheProductAsync(string key, object value)
        {
            var jsonData = JsonSerializer.Serialize(value);
            await _cache.StringSetAsync(key, jsonData, _defaultCacheDuration);
        }

        public async Task<T?> GetCachedProductAsync<T>(string key)
        {
            var data = await _cache.StringGetAsync(key);
            return data.HasValue ? JsonSerializer.Deserialize<T>(data!) : default;
        }

        public async Task RemoveProductFromCacheAsync(string key)
        {
            await _cache.KeyDeleteAsync(key);
        }
    }

}

