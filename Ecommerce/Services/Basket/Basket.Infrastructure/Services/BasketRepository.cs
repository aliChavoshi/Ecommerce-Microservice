using Basket.Core.Entities;
using Basket.Core.Repositories;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;

namespace Basket.Infrastructure.Services;

public class BasketRepository(IDistributedCache redisCache) : IBasketRepository
{
    public async Task<ShoppingCart?> GetBasket(string userName)
    {
        var basket = await redisCache.GetStringAsync(userName);
        return string.IsNullOrEmpty(basket) ? null : JsonConvert.DeserializeObject<ShoppingCart>(basket);
    }

    public async Task<ShoppingCart?> UpdateBasket(ShoppingCart shoppingCart)
    {
        await redisCache.SetStringAsync(shoppingCart.UserName, JsonConvert.SerializeObject(shoppingCart));
        return await GetBasket(shoppingCart.UserName);
    }

    public async Task DeleteBasket(string userName)
    {
        await redisCache.RemoveAsync(userName);
    }
}