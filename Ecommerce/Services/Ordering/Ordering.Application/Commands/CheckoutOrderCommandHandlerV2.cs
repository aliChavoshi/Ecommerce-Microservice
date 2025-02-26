using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using Ordering.Core.Entities;
using Ordering.Core.Repositories;

namespace Ordering.Application.Commands;

public class CheckoutOrderCommandV2 : IRequest<int>
{
    public string? UserName { get; set; }
    public decimal? TotalPrice { get; set; }
}

public class CheckoutOrderCommandHandlerV2(
    IMapper mapper,
    IOrderRepository orderRepository,
    ILogger<CheckoutOrderCommandHandlerV2> logger) : IRequestHandler<CheckoutOrderCommandV2, int>
{
    public async Task<int> Handle(CheckoutOrderCommandV2 request, CancellationToken cancellationToken)
    {
        var order = mapper.Map<Order>(request);
        var result = await orderRepository.AddAsync(order);
        logger.LogInformation("CheckoutOrderCommandV2 handled. Result: {Result}", result);
        return result.Id;
    }
}