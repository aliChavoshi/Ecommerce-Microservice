using Basket.Core.Entities;

namespace Basket.Application.Responses;

public class ShoppingCartResponse
{
    public string UserName { get; set; }
    public List<ShoppingCartItemResponse> Items { get; set; } = new();
    public decimal TotalPrice { get; set; }
    public ShoppingCartResponse()
    {
    }

    public ShoppingCartResponse(string userName)
    {
        UserName = userName;
    }

    public decimal CalculateOriginalPrice()
    {
        return Items.Sum(x => x.Price * x.Quantity);
    }
}