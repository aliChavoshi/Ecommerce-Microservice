using Basket.Application.Commands;
using Basket.Application.GrpcService;
using Basket.Application.Queries;
using Basket.Application.Responses;
using Basket.Core.Repositories;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;

namespace Basket.API.Controllers;

public class BasketController(IMediator mediator) : ApiController
{
    [HttpPost]
    public async Task<ActionResult<ShoppingCartResponse>> CreateBasket([FromBody] CreateShoppingCartCommand command,
        CancellationToken cancellationToken)
    {
        return Ok(await mediator.Send(command, cancellationToken));
    }
    
    [HttpGet("{username}")]
    public async Task<ActionResult<ShoppingCartResponse>> GetBasketByUserName(string username,
        CancellationToken cancellationToken)
    {
        return Ok(await mediator.Send(new GetBasketByUserNameQuery(username), cancellationToken));
    }

    [HttpDelete("{username}")]
    public async Task<ActionResult<bool>> DeleteBasketByUserName(string username, CancellationToken cancellationToken)
    {
        return Ok(await mediator.Send(new DeleteBasketByUserNameCommand(username), cancellationToken));
    }
}