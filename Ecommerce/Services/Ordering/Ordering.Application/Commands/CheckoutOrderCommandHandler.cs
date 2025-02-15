using AutoMapper;
using MediatR;
using Ordering.Application.Responses;
using Ordering.Core.Entities;
using Ordering.Core.Repositories;

namespace Ordering.Application.Commands;

public class CheckoutOrderCommand : IRequest<int>
{
    public string? UserName { get; set; }
    public decimal? TotalPrice { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? EmailAddress { get; set; }
    public string? AddressLine { get; set; }
    public string? State { get; set; }
    public int? PaymentMethod { get; set; }
}

public class CheckoutOrderCommandHandler(IOrderRepository orderRepository, IMapper mapper)
    : IRequestHandler<CheckoutOrderCommand, int>
{
    public async Task<int> Handle(CheckoutOrderCommand request, CancellationToken cancellationToken)
    {
        var order = mapper.Map<Order>(request);
        var result = await orderRepository.AddAsync(order);
        return result.Id;
    }
}