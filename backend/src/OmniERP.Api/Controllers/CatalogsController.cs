using Microsoft.AspNetCore.Mvc;
using OmniERP.Application.Catalogs.Dtos;
using OmniERP.Application.Catalogs.UseCases;
using OmniERP.Application.Common;
using OmniERP.Application.Ports;

namespace OmniERP.Api.Controllers;

[ApiController]
[Route("api/v1/catalogs")]
public sealed class CatalogsController : ControllerBase
{
    private readonly GetOrderCatalogsUseCase _getOrderCatalogsUseCase;
    private readonly ICacheProvider _cacheProvider;

    public CatalogsController(
        GetOrderCatalogsUseCase getOrderCatalogsUseCase,
        ICacheProvider cacheProvider)
    {
        _getOrderCatalogsUseCase = getOrderCatalogsUseCase;
        _cacheProvider = cacheProvider;
    }

    [HttpGet("order-form")]
    public async Task<ActionResult<OrderFormCatalogsResponse>> GetOrderFormCatalogs(
        CancellationToken cancellationToken)
    {
        var catalogs = await _getOrderCatalogsUseCase.ExecuteAsync(cancellationToken);

        return Ok(catalogs);
    }

    [HttpPost("cache/invalidate")]
    public async Task<IActionResult> InvalidateCache(CancellationToken cancellationToken)
    {
        await _cacheProvider.RemoveAsync(CacheKeys.OrderFormCatalogs, cancellationToken);

        return Ok(new
        {
            message = "Order form catalogs cache invalidated successfully."
        });
    }
}
