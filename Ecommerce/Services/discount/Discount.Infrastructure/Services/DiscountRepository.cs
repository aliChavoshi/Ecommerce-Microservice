using Discount.Core.Entities;
using Discount.Core.Interfaces;

namespace Discount.Infrastructure.Services;

public class DiscountRepository : IDiscountRepository
{
    public Task<Coupon> GetDiscount(string productName)
    {
        throw new NotImplementedException();
    }

    public Task<bool> CreateDiscount(Coupon coupon)
    {
        throw new NotImplementedException();
    }

    public Task<bool> UpdateDiscount(Coupon coupon)
    {
        throw new NotImplementedException();
    }

    public Task<bool> DeleteDiscount(string productName)
    {
        throw new NotImplementedException();
    }
}