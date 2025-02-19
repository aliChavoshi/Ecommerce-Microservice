using AutoMapper;
using Basket.Application.GrpcService;
using Basket.Application.Responses;
using Basket.Core.Entities;
using Basket.Core.Repositories;
using MediatR;

namespace Basket.Application.Commands;

public class CreateShoppingCartCommand : IRequest<ShoppingCartResponse>
{
    public string UserName { get; set; }
    public List<ShoppingCartItem> Items { get; set; }

    public CreateShoppingCartCommand(string userName, List<ShoppingCartItem> items)
    {
        UserName = userName;
        Items = items;
    }
}

public class CreateShoppingCartCommandHandler(
    IBasketRepository basketRepository,
    IMapper mapper,
    DiscountGrpcService discountGrpcService)
    : IRequestHandler<CreateShoppingCartCommand, ShoppingCartResponse>
{
    public async Task<ShoppingCartResponse> Handle(CreateShoppingCartCommand request,
        CancellationToken cancellationToken)
    {
        foreach (var product in request.Items)
        {
            var coupon = await discountGrpcService.GetDiscount(product.ProductName);
            product.Price -= coupon.Amount;
        }

        var shoppingCart = mapper.Map<ShoppingCart>(request);
        await basketRepository.UpdateBasket(shoppingCart);
        var response = mapper.Map<ShoppingCartResponse>(shoppingCart);
        response.TotalPrice = response.CalculateTotalPrice();
        return response;
    }
}