﻿namespace Basket.Core.Entities;

public class ShoppingCart
{
    public string UserName { get; set; }
    public List<ShoppingCartItem> Items { get; set; } = new();

    public ShoppingCart()
    {
    }

    public ShoppingCart(string userName)
    {
        UserName = userName;
    }
    public decimal CalculateOriginalPrice()
    {
        return Items.Sum(x => x.Price * x.Quantity);
    }
}