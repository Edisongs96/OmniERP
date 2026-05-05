using Microsoft.AspNetCore.Mvc;
using OmniERP.Application.Orders.Dtos;
using OmniERP.Application.Orders.UseCases;

namespace OmniERP.Api.Controllers;

[ApiController]
[Route("api/v1/orders")]
public sealed class OrdersController : ControllerBase
{
    private readonly GetOrderByIdUseCase _getOrderByIdUseCase;
    private readonly UpdateOrderUseCase _updateOrderUseCase;

    public OrdersController(
        GetOrderByIdUseCase getOrderByIdUseCase,
        UpdateOrderUseCase updateOrderUseCase)
    {
        _getOrderByIdUseCase = getOrderByIdUseCase;
        _updateOrderUseCase = updateOrderUseCase;
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<OrderResponse>> GetById(
        int id,
        CancellationToken cancellationToken)
    {
        if (id <= 0)
        {
            return BadRequest(new { message = "El identificador del pedido debe ser mayor que cero." });
        }

        var order = await _getOrderByIdUseCase.ExecuteAsync(id, cancellationToken);

        return order is null
            ? NotFound(new { message = $"No se encontro el pedido {id}." })
            : Ok(order);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(
        int id,
        [FromBody] UpdateOrderRequest? request,
        CancellationToken cancellationToken)
    {
        if (id <= 0)
        {
            return BadRequest(new { message = "El identificador del pedido debe ser mayor que cero." });
        }

        if (request is null)
        {
            return BadRequest(new { message = "La solicitud de actualizacion del pedido es requerida." });
        }

        var result = await _updateOrderUseCase.ExecuteAsync(id, request, cancellationToken);

        return result.Status switch
        {
            UpdateOrderResultStatus.Success => Ok(result.Order),
            UpdateOrderResultStatus.NotFound => NotFound(new { message = result.Message }),
            UpdateOrderResultStatus.ValidationError => BadRequest(new { message = result.Message }),
            UpdateOrderResultStatus.Conflict => Conflict(result.Conflict),
            _ => StatusCode(StatusCodes.Status500InternalServerError)
        };
    }
}
