using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore.Metadata;
using Ordering.Application.Responses;
using Ordering.Core.Entities;
using Ordering.Core.Repositories;

namespace Ordering.Application.Queries;

public class GetOrderListQuery(string userName) : IRequest<List<OrderResponse>>
{
    public string UserName { get; set; } = userName;
}

public class GetOrderListQueryHandler(IOrderRepository orderRepository,IMapper mapper)
    : IRequestHandler<GetOrderListQuery, List<OrderResponse>>
{
    public async Task<List<OrderResponse>> Handle(GetOrderListQuery request, CancellationToken cancellationToken)
    {
        var result = await orderRepository.GetOrdersByUserName(request.UserName);
        return mapper.Map<List<OrderResponse>>(result);
    }
}