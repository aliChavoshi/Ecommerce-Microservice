﻿using AutoMapper;
using EventBus.Messages.Events;
using MassTransit;
using MediatR;
using Ordering.Application.Commands;

namespace Ordering.API.EventBusConsumer;

public class BasketOrderingConsumer(IMediator mediator, IMapper mapper, ILogger<BasketOrderingConsumer> logger)
    : IConsumer<BasketCheckoutEvent>
{
    public async Task Consume(ConsumeContext<BasketCheckoutEvent> context)
    {
        using var scope = logger.BeginScope("Consuming event: {EventId} {Event}", context.MessageId, context.Message);
        var cmd = mapper.Map<CheckoutOrderCommand>(context.Message);
        var result = await mediator.Send(cmd); // Create a new order
        logger.LogInformation("CheckoutOrderCommand sent to Ordering.API. Result: {Result}", result);
    }
}