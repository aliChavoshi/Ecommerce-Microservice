namespace Basket.Core.Entities;

public class ShoppingCart(string userName)
{
    public string UserName { get; set; } = userName;
    public List<ShoppingCartItem> Items { get; set; } = new();

    public decimal CalculateOriginalPrice()
    {
        return Items.Sum(x => x.Price * x.Quantity);
    }
}