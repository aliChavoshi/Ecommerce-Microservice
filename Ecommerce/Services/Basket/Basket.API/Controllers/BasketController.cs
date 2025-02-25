using AutoMapper;
using Basket.Application.Commands;
using Basket.Application.Queries;
using Basket.Application.Responses;
using Basket.Core.Entities;
using EventBus.Messages.Events;
using MassTransit;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Basket.API.Controllers;

[ApiVersion("1")]
public class BasketController(
    IMediator mediator,
    IMapper mapper,
    IPublishEndpoint publishEndpoint,
    ILogger<BasketController> logger) : ApiController
{
    [HttpPost]
    public async Task<ActionResult<ShoppingCartResponse>> CreateBasket([FromBody] CreateShoppingCartCommand command,
        CancellationToken cancellationToken)
    {
        logger.LogInformation("CreateBasketCommand: {UserName} {Items}", command.UserName, command.Items);
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

    [HttpPost]
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