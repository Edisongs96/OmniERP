using Microsoft.Extensions.Caching.Memory;
using OmniERP.Infrastructure.Cache;

namespace OmniERP.Infrastructure.Tests.Cache;

public sealed class MemoryCacheProviderTests
{
    [Fact]
    public async Task MemoryCacheProvider_SetAndGetAsync_ShouldReturnCachedValue()
    {
        using var memoryCache = new MemoryCache(new MemoryCacheOptions());
        var provider = new MemoryCacheProvider(memoryCache);

        await provider.SetAsync("test-key", "cached-value", TimeSpan.FromMinutes(5));
        var value = await provider.GetAsync<string>("test-key");

        Assert.Equal("cached-value", value);
    }

    [Fact]
    public async Task MemoryCacheProvider_RemoveAsync_ShouldRemoveCachedValue()
    {
        using var memoryCache = new MemoryCache(new MemoryCacheOptions());
        var provider = new MemoryCacheProvider(memoryCache);

        await provider.SetAsync("test-key", "cached-value", TimeSpan.FromMinutes(5));
        await provider.RemoveAsync("test-key");
        var value = await provider.GetAsync<string>("test-key");

        Assert.Null(value);
    }
}
