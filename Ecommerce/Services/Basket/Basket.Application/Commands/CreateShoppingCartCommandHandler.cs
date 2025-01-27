using AutoMapper;
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

public class CreateShoppingCartCommandHandler(IBasketRepository basketRepository, IMapper mapper)
    : IRequestHandler<CreateShoppingCartCommand, ShoppingCartResponse>
{
    public async Task<ShoppingCartResponse> Handle(CreateShoppingCartCommand request,
        CancellationToken cancellationToken)
    {
        // TODO : will be integrating Discount service
        var shoppingCart = mapper.Map<ShoppingCart>(request);
        await basketRepository.UpdateBasket(shoppingCart);
        return mapper.Map<ShoppingCartResponse>(shoppingCart);
    }
}