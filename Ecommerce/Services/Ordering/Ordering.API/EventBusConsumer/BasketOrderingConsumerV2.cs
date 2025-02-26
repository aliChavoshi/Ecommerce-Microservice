using AutoMapper;
using EventBus.Messages.Events;
using MassTransit;
using MediatR;
using Ordering.Application.Commands;

namespace Ordering.API.EventBusConsumer;

public class BasketOrderingConsumerV2(IMediator mediator, IMapper mapper, ILogger<BasketOrderingConsumerV2> logger)
    : IConsumer<BasketCheckoutEventV2>
{
    public async Task Consume(ConsumeContext<BasketCheckoutEventV2> context)
    {
        using var scope =
            logger.BeginScope("Consuming event V2: {EventId} {Event}", context.MessageId, context.Message);
        var cmd = mapper.Map<CheckoutOrderCommandV2>(context.Message);
        var result = await mediator.Send(cmd); // Create a new order
        logger.LogInformation("CheckoutOrderCommandV2 sent to Ordering.API. Result: {Result}", result);
    }
}