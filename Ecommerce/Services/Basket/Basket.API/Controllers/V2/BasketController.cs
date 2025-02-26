using Asp.Versioning;
using AutoMapper;
using Basket.API.Controllers.Common;
using Basket.Application.Commands;
using Basket.Application.Queries;
using Basket.Core.Entities;
using EventBus.Messages.Events;
using MassTransit;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Basket.API.Controllers.V2;

[ApiVersion("2.0")]
public class BasketController(
    IMediator mediator,
    IMapper mapper,
    IPublishEndpoint publishEndpoint,
    ILogger<BasketController> logger)
    : ApiController
{
    [HttpPost]
    [MapToApiVersion("2.0")]
    public async Task<IActionResult> Checkout([FromBody] BasketCheckoutV2 basketCheckout)
    {
        var query = new GetBasketByUserNameQuery(basketCheckout.UserName);
        var basket = await mediator.Send(query);
        //Create a basketCheckout event -- Set TotalPrice on basketCheckout eventMessage
        var eventMsg = mapper.Map<BasketCheckoutEventV2>(basketCheckout);
        eventMsg.TotalPrice = basket.TotalPrice;
        await publishEndpoint.Publish(eventMsg); // publish the event
        logger.LogInformation($"Basket published for {basket.UserName} in V2");
        //Remove the basket
        var deleteCommand = new DeleteBasketByUserNameCommand(basket.UserName);
        await mediator.Send(deleteCommand);
        // 202 (Accepted) response
        return Accepted();
    }
}