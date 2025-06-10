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
using Common.Logging.Correlations;

namespace Basket.API.Controllers.V1;

[ApiVersion("1.0")]
public class BasketController : ApiController
{
    private readonly IMediator _mediator;
    private readonly IMapper _mapper;
    private readonly IPublishEndpoint _publishEndpoint;
    private readonly ILogger<BasketController> _logger;
    private readonly ICorrelationIdGenerator _correlation;

    public BasketController(IMediator mediator,
        IMapper mapper,
        IPublishEndpoint publishEndpoint,
        ILogger<BasketController> logger, ICorrelationIdGenerator correlation)
    {
        _mediator = mediator;
        _mapper = mapper;
        _publishEndpoint = publishEndpoint;
        _logger = logger;
        _correlation = correlation;
        _logger.LogInformation("CorrelationId {correlationId}", correlation.Get());
    }

    [HttpPost]
    [MapToApiVersion("1.0")]
    public async Task<ActionResult<ShoppingCartResponse>> CreateBasket([FromBody] CreateShoppingCartCommand command,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("CreateBasketCommand: {UserName} {Items}", command.UserName, command.Items);
        return Ok(await _mediator.Send(command, cancellationToken));
    }

    [HttpGet("{username}")]
    [MapToApiVersion("1.0")]
    public async Task<ActionResult<ShoppingCartResponse>> GetBasketByUserName(string username,
        CancellationToken cancellationToken)
    {
        return Ok(await _mediator.Send(new GetBasketByUserNameQuery(username), cancellationToken));
    }

    [HttpDelete("{username}")]
    [MapToApiVersion("1.0")]
    public async Task<ActionResult<bool>> DeleteBasketByUserName(string username, CancellationToken cancellationToken)
    {
        return Ok(await _mediator.Send(new DeleteBasketByUserNameCommand(username), cancellationToken));
    }

    [HttpPost]
    [MapToApiVersion("1.0")]
    public async Task<IActionResult> Checkout([FromBody] BasketCheckout basketCheckout)
    {
        var query = new GetBasketByUserNameQuery(basketCheckout.UserName);
        var basket = await _mediator.Send(query);
        //Create a basketCheckout event -- Set TotalPrice on basketCheckout eventMessage
        var eventMsg = _mapper.Map<BasketCheckoutEvent>(basketCheckout);
        eventMsg.TotalPrice = basket.TotalPrice;
        // Set the correlation ID
        eventMsg.CorrelationId = _correlation.Get();
        await _publishEndpoint.Publish(eventMsg); // publish the event
        _logger.LogInformation("BasketCheckoutEvent published successfully: {correlationId} {DateTime} {UserName}",
            eventMsg.CorrelationId, eventMsg.CreationDate, eventMsg.UserName);
        //Remove the basket
        var deleteCommand = new DeleteBasketByUserNameCommand(basket.UserName);
        await _mediator.Send(deleteCommand);
        // 202 (Accepted) response
        return Accepted();
    }
}