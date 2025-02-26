using Asp.Versioning;
using AutoMapper;
using Basket.API.Controllers.Common;
using Basket.Application.Commands;
using Basket.Application.Queries;
using Basket.Application.Responses;
using Basket.Core.Entities;
using EventBus.Messages.Events;
using MassTransit;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Basket.API.Controllers.V1;

[ApiVersion("1.0")]
public class BasketController(
    IMediator mediator,
    IMapper mapper,
    IPublishEndpoint publishEndpoint,
    ILogger<BasketController> logger)
    : ApiController
{
    [HttpPost]
    [MapToApiVersion("1.0")]
    public async Task<ActionResult<ShoppingCartResponse>> CreateBasket([FromBody] CreateShoppingCartCommand command,
        CancellationToken cancellationToken)
    {
        logger.LogInformation("CreateBasketCommand: {UserName} {Items}", command.UserName, command.Items);
        return Ok(await mediator.Send(command, cancellationToken));
    }

    [HttpGet("{username}")]
    [MapToApiVersion("1.0")]
    public async Task<ActionResult<ShoppingCartResponse>> GetBasketByUserName(string username,
        CancellationToken cancellationToken)
    {
        return Ok(await mediator.Send(new GetBasketByUserNameQuery(username), cancellationToken));
    }

    [HttpDelete("{username}")]
    [MapToApiVersion("1.0")]
    public async Task<ActionResult<bool>> DeleteBasketByUserName(string username, CancellationToken cancellationToken)
    {
        return Ok(await mediator.Send(new DeleteBasketByUserNameCommand(username), cancellationToken));
    }

    [HttpPost]
    [MapToApiVersion("1.0")]
    public async Task<IActionResult> Checkout([FromBody] BasketCheckout basketCheckout)
    {
        var query = new GetBasketByUserNameQuery(basketCheckout.UserName);
        var basket = await mediator.Send(query);
        //Create a basketCheckout event -- Set TotalPrice on basketCheckout eventMessage
        var eventMsg = mapper.Map<BasketCheckoutEvent>(basketCheckout);
        eventMsg.TotalPrice = basket.TotalPrice;
        await publishEndpoint.Publish(eventMsg); // publish the event
        logger.LogInformation("BasketCheckoutEvent published successfully. EventId: {EventId} {DateTime} {UserName}",
            eventMsg.CorrelationId, eventMsg.CreationDate, eventMsg.UserName);
        //Remove the basket
        var deleteCommand = new DeleteBasketByUserNameCommand(basket.UserName);
        await mediator.Send(deleteCommand);
        // 202 (Accepted) response
        return Accepted();
    }
}
