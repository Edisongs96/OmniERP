using OmniERP.Application.Ports;

namespace OmniERP.Application.Tests.Fakes;

internal sealed class FakeCacheProvider : ICacheProvider
{
    private readonly Dictionary<string, object> _items = [];

    public int GetCount { get; private set; }

    public int SetCount { get; private set; }

    public int RemoveCount { get; private set; }

    public TimeSpan? LastTtl { get; private set; }

    public bool ContainsKey(string key)
    {
        return _items.ContainsKey(key);
    }

    public Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default)
    {
        GetCount += 1;

        return Task.FromResult(_items.TryGetValue(key, out var value) ? (T)value : default);
    }

    public Task SetAsync<T>(string key, T value, TimeSpan ttl, CancellationToken cancellationToken = default)
    {
        SetCount += 1;
        LastTtl = ttl;
        _items[key] = value!;

        return Task.CompletedTask;
    }

    public Task RemoveAsync(string key, CancellationToken cancellationToken = default)
    {
        RemoveCount += 1;
        _items.Remove(key);

        return Task.CompletedTask;
    }
}
