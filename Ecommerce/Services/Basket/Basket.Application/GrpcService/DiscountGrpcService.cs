using Discount.Application.Protos;

namespace Basket.Application.GrpcService;

public class DiscountGrpcService(DiscountProtoService.DiscountProtoServiceClient client)
{
    public async Task<CouponModel> GetDiscount(string productName)
    {
        var discountRequest = new GetDiscountRequest { ProductName = productName };
        return await client.GetDiscountAsync(discountRequest);
    }
}