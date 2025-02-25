using AutoMapper;
using Basket.Application.Commands;
using Basket.Application.Responses;
using Basket.Core.Entities;
using EventBus.Messages.Events;

namespace Basket.Application.Mappers;

public class BasketMappingProfile : Profile
{
    public BasketMappingProfile()
    {
        CreateMap<ShoppingCartItem, ShoppingCartItemResponse>().ReverseMap();
        CreateMap<ShoppingCart, ShoppingCartResponse>().ReverseMap();
        CreateMap<CreateShoppingCartCommand, ShoppingCart>().ReverseMap();
        //Relations with Rabbit MQ
        CreateMap<BasketCheckout, BasketCheckoutEvent>().ReverseMap();
        CreateMap<BasketCheckoutEventV2, BasketCheckoutV2>().ReverseMap();
    }
}