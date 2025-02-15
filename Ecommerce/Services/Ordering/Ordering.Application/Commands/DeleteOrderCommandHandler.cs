using MediatR;
using Ordering.Core.Entities;
using Ordering.Core.Repositories;

namespace Ordering.Application.Commands;

public class DeleteOrderCommand : IRequest
{
    public int Id { get; set; }
}
public class DeleteOrderCommandHandler(IOrderRepository orderRepository) : IRequestHandler<DeleteOrderCommand>
{
    public async  Task Handle(DeleteOrderCommand request, CancellationToken cancellationToken)
    {
        var order = await orderRepository.GetByIdAsync(request.Id);
        if(order==null) throw new OrderNotFoundException(nameof(Order), request.Id);
        await orderRepository.DeleteAsync(order);
    }
}