using AutoMapper;
using EventBus.Messages.Events;
using Ordering.Application.Commands;
using Ordering.Application.Responses;
using Ordering.Core.Entities;

namespace Ordering.Application.Mappers;

public class OrderMappingProfile : Profile
{
    public OrderMappingProfile()
    {
        CreateMap<Order, OrderResponse>().ReverseMap();
        CreateMap<CheckoutOrderCommand, Order>().ReverseMap();
        CreateMap<UpdateOrderCommand, Order>().ReverseMap();
        //Mapping From RabbitMQ
        CreateMap<CheckoutOrderCommand, BasketCheckoutEvent>().ReverseMap();
        CreateMap<BasketCheckoutEventV2, CheckoutOrderCommandV2>().ReverseMap();
        CreateMap<CheckoutOrderCommandV2, Order>().ReverseMap();
    }
}