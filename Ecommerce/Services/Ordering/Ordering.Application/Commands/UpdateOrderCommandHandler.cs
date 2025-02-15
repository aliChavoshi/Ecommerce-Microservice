using AutoMapper;
using MediatR;
using Ordering.Application.Exceptions;
using Ordering.Core.Entities;
using Ordering.Core.Repositories;

namespace Ordering.Application.Commands;

public class UpdateOrderCommand : IRequest
{
    public int Id { get; set; }
    public string? UserName { get; set; }
    public decimal? TotalPrice { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? EmailAddress { get; set; }
    public string? AddressLine { get; set; }
    public string? State { get; set; }
    public int? PaymentMethod { get; set; }
}

public class UpdateOrderCommandHandler(IOrderRepository orderRepository, IMapper mapper)
    : IRequestHandler<UpdateOrderCommand>
{
    public async Task Handle(UpdateOrderCommand request, CancellationToken cancellationToken)
    {
        var order = await orderRepository.GetByIdAsync(request.Id);
        if (order == null) throw new OrderNotFoundException(nameof(Order), request.Id);
        mapper.Map(request, order);
        await orderRepository.UpdateAsync(order);
    }
}