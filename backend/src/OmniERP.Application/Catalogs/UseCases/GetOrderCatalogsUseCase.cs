using System.Diagnostics;
using OmniERP.Application.Catalogs.Dtos;
using OmniERP.Application.Common;
using OmniERP.Application.Ports;

namespace OmniERP.Application.Catalogs.UseCases;

public sealed class GetOrderCatalogsUseCase
{
    private const string CacheSource = "cache";
    private const string SlowSource = "slow-source";

    private readonly ICacheProvider _cacheProvider;
    private readonly ICatalogRepository _catalogRepository;

    public GetOrderCatalogsUseCase(
        ICacheProvider cacheProvider,
        ICatalogRepository catalogRepository)
    {
        _cacheProvider = cacheProvider;
        _catalogRepository = catalogRepository;
    }

    public async Task<OrderFormCatalogsResponse> ExecuteAsync(CancellationToken cancellationToken = default)
    {
        var stopwatch = Stopwatch.StartNew();
        var cachedCatalogs = await _cacheProvider.GetAsync<OrderFormCatalogsResponse>(
            CacheKeys.OrderFormCatalogs,
            cancellationToken);

        if (cachedCatalogs is not null)
        {
            stopwatch.Stop();

            return WithMetadata(cachedCatalogs, CacheSource, stopwatch.ElapsedMilliseconds);
        }

        var orderStatusesTask = _catalogRepository.GetOrderStatusesAsync(cancellationToken);
        var shippingMethodsTask = _catalogRepository.GetShippingMethodsAsync(cancellationToken);

        await Task.WhenAll(orderStatusesTask, shippingMethodsTask);

        var response = new OrderFormCatalogsResponse
        {
            OrderStatuses = orderStatusesTask.Result.Select(CatalogMapper.ToResponse).ToArray(),
            ShippingMethods = shippingMethodsTask.Result.Select(CatalogMapper.ToResponse).ToArray(),
            Metadata = BuildMetadata(SlowSource, durationMs: 0)
        };

        await _cacheProvider.SetAsync(
            CacheKeys.OrderFormCatalogs,
            response,
            CacheDurations.OrderFormCatalogsTtl,
            cancellationToken);

        stopwatch.Stop();

        return WithMetadata(response, SlowSource, stopwatch.ElapsedMilliseconds);
    }

    private static OrderFormCatalogsResponse WithMetadata(
        OrderFormCatalogsResponse response,
        string source,
        long durationMs)
    {
        return response with
        {
            Metadata = BuildMetadata(source, durationMs)
        };
    }

    private static CatalogResponseMetadata BuildMetadata(string source, long durationMs)
    {
        return new CatalogResponseMetadata
        {
            Source = source,
            DurationMs = durationMs,
            CacheKey = CacheKeys.OrderFormCatalogs
        };
    }
}
