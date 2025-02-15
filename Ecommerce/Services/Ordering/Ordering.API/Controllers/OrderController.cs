using MediatR;
using Microsoft.AspNetCore.Mvc;
using Ordering.Application.Commands;
using Ordering.Application.Queries;
using Ordering.Application.Responses;

namespace Ordering.API.Controllers;

public class OrderController(IMediator mediator) : ApiController
{
    [HttpGet("{userName}")]
    public async Task<ActionResult<List<OrderResponse>>> GetOrdersByUserName(string userName,
        CancellationToken cancellationToken)
    {
        var query = new GetOrderListQuery(userName);
        return Ok(await mediator.Send(query, cancellationToken));
    }

    [HttpPost]
    public async Task<ActionResult<int>> CheckoutOrder([FromBody] CheckoutOrderCommand command,
        CancellationToken cancellationToken)
    {
        return Ok(await mediator.Send(command, cancellationToken));
    }

    [HttpPut]
    public async Task<ActionResult<int>> UpdateOrder([FromBody] UpdateOrderCommand command,
        CancellationToken cancellationToken)
    {
        return Ok(await mediator.Send(command, cancellationToken));
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteOrder([FromBody] int id,
        CancellationToken cancellationToken)
    {
        await mediator.Send(new DeleteOrderCommand() { Id = id }, cancellationToken);
        return NoContent();
    }
}