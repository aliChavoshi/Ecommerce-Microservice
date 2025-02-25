using AutoMapper;
using Basket.Application.Responses;
using Basket.Core.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Basket.Application.Queries;

public class GetBasketByUserNameQuery(string userName) : IRequest<ShoppingCartResponse>
{
    public string UserName { get; set; } = userName;
}

public class GetBasketByUserNameQueryHandler(
    IBasketRepository basketRepository,
    IMapper mapper,
    ILogger<GetBasketByUserNameQueryHandler> logger)
    : IRequestHandler<GetBasketByUserNameQuery, ShoppingCartResponse>
{
    public async Task<ShoppingCartResponse> Handle(GetBasketByUserNameQuery request,
        CancellationToken cancellationToken)
    {
        var basket = await basketRepository.GetBasket(request.UserName);
        logger.LogInformation("Basket is retrieved for {UserName}", request.UserName);
        if (basket != null)
        {
            var response = mapper.Map<ShoppingCartResponse>(basket);
            response.TotalPrice = response.CalculateTotalPrice();
            logger.LogInformation("Basket is retrieved for {UserName}", request.UserName);
            return response;
        }
        logger.LogInformation("Basket is not found for {UserName}", request.UserName);
        return new ShoppingCartResponse(request.UserName);
    }
}