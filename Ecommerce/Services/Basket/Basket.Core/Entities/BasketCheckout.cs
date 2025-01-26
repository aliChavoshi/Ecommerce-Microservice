namespace Basket.Core.Entities;

public class BasketCheckout
{
    public string UserName { get; set; }
    public decimal TotalPrice { get; set; }

    //
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string EmailAddress { get; set; }
    public string BuyerPhoneNumber { get; set; }
    public string AddressLine { get; set; }
    public string State { get; set; }
    public string City { get; set; }

    //
    public int PaymentMethod { get; set; }
    public string Status { get; set; }

    public string TrackingCode { get; set; }
    //
    // public string CardName { get; set; }
    // public string CardNumber { get; set; }

    //
}