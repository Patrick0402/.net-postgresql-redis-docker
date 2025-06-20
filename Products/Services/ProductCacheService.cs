using StackExchange.Redis;
using System.Text.Json;

namespace ProductsApi.Services
{
    public class ProductCacheService
    {
        private readonly IDatabase _cache;

        public ProductCacheService(IConnectionMultiplexer redis)
        {
            _cache = redis.GetDatabase();
        }

        public async Task CacheProductAsync(string key, object value)
        {
            var jsonData = JsonSerializer.Serialize(value);
            await _cache.StringSetAsync(key, jsonData, TimeSpan.FromSeconds(5));
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
