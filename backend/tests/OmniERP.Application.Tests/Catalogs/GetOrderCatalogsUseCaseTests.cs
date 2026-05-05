using OmniERP.Application.Catalogs.UseCases;
using OmniERP.Application.Common;
using OmniERP.Application.Tests.Fakes;

namespace OmniERP.Application.Tests.Catalogs;

public sealed class GetOrderCatalogsUseCaseTests
{
    [Fact]
    public async Task GetOrderCatalogs_FirstCall_ShouldUseRepositoryAndStoreCache()
    {
        var cacheProvider = new FakeCacheProvider();
        var catalogRepository = new FakeCatalogRepository();
        var useCase = new GetOrderCatalogsUseCase(cacheProvider, catalogRepository);

        var result = await useCase.ExecuteAsync();

        Assert.Equal("slow-source", result.Metadata.Source);
        Assert.Equal(CacheKeys.OrderFormCatalogs, result.Metadata.CacheKey);
        Assert.Equal(1, catalogRepository.OrderStatusesCallCount);
        Assert.Equal(1, catalogRepository.ShippingMethodsCallCount);
        Assert.Equal(1, cacheProvider.SetCount);
        Assert.True(cacheProvider.ContainsKey(CacheKeys.OrderFormCatalogs));
        Assert.Equal(CacheDurations.OrderFormCatalogsTtl, cacheProvider.LastTtl);
    }

    [Fact]
    public async Task GetOrderCatalogs_SecondCall_ShouldUseCache()
    {
        var cacheProvider = new FakeCacheProvider();
        var catalogRepository = new FakeCatalogRepository();
        var useCase = new GetOrderCatalogsUseCase(cacheProvider, catalogRepository);

        await useCase.ExecuteAsync();
        var result = await useCase.ExecuteAsync();

        Assert.Equal("cache", result.Metadata.Source);
        Assert.Equal(1, catalogRepository.OrderStatusesCallCount);
        Assert.Equal(1, catalogRepository.ShippingMethodsCallCount);
        Assert.Equal(1, cacheProvider.SetCount);
        Assert.Equal(2, cacheProvider.GetCount);
    }

    [Fact]
    public async Task GetOrderCatalogs_ShouldReturnMetadataWithCacheKeyAndDuration()
    {
        var useCase = new GetOrderCatalogsUseCase(
            new FakeCacheProvider(),
            new FakeCatalogRepository());

        var result = await useCase.ExecuteAsync();

        Assert.Equal(CacheKeys.OrderFormCatalogs, result.Metadata.CacheKey);
        Assert.True(result.Metadata.DurationMs >= 0);
        Assert.False(string.IsNullOrWhiteSpace(result.Metadata.Source));
    }
}
