using AutoMapper;
using Basket.Application.Commands;
using Basket.Application.Responses;
using Basket.Core.Entities;

namespace Basket.Application.Mappers;

public class BasketMappingProfile : Profile
{
    public BasketMappingProfile()
    {
        CreateMap<ShoppingCartItem, ShoppingCartItemResponse>().ReverseMap();
        CreateMap<ShoppingCart, ShoppingCartResponse>().ReverseMap();
        CreateMap<CreateShoppingCartCommand, ShoppingCart>().ReverseMap();
    }
}