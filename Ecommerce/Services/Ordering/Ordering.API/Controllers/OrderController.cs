using Common.Logging.Correlations;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Ordering.Application.Commands;
using Ordering.Application.Queries;
using Ordering.Application.Responses;

namespace Ordering.API.Controllers;

public class OrderController : ApiController
{
    private readonly IMediator _mediator;

    public OrderController(IMediator mediator, ICorrelationIdGenerator correlation, ILogger<OrderController> logger)
    {
        _mediator = mediator;
        logger.LogInformation("CorrelationId {correlationId}", correlation.Get());
    }

    [HttpGet("{userName}")]
    public async Task<ActionResult<List<OrderResponse>>> GetOrdersByUserName(string userName,
        CancellationToken cancellationToken)
    {
        var query = new GetOrderListQuery()
        {
            UserName = userName
        };
        return Ok(await _mediator.Send(query, cancellationToken));
    }

    [HttpPost]
    public async Task<ActionResult<int>> CheckoutOrder([FromBody] CheckoutOrderCommand command,
        CancellationToken cancellationToken)
    {
        return Ok(await _mediator.Send(command, cancellationToken));
    }

    [HttpPut]
    public async Task<ActionResult<int>> UpdateOrder([FromBody] UpdateOrderCommand command,
        CancellationToken cancellationToken)
    {
        return Ok(await _mediator.Send(command, cancellationToken));
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteOrder(int id,
        CancellationToken cancellationToken)
    {
        await _mediator.Send(new DeleteOrderCommand { Id = id }, cancellationToken);
        return NoContent();
    }
}