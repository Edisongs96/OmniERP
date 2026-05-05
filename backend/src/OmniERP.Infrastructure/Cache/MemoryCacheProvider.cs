using Microsoft.Extensions.Caching.Memory;
using OmniERP.Application.Ports;

namespace OmniERP.Infrastructure.Cache;

public sealed class MemoryCacheProvider : ICacheProvider
{
    private readonly IMemoryCache _memoryCache;

    public MemoryCacheProvider(IMemoryCache memoryCache)
    {
        _memoryCache = memoryCache;
    }

    public Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default)
    {
        _memoryCache.TryGetValue(key, out T? value);

        return Task.FromResult(value);
    }

    public Task SetAsync<T>(string key, T value, TimeSpan ttl, CancellationToken cancellationToken = default)
    {
        _memoryCache.Set(key, value, ttl);

        return Task.CompletedTask;
    }

    public Task RemoveAsync(string key, CancellationToken cancellationToken = default)
    {
        _memoryCache.Remove(key);

        return Task.CompletedTask;
    }
}
