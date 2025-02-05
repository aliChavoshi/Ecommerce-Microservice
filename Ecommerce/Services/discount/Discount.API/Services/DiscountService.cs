using Discount.Application.Commands;
using Discount.Application.Protos;
using Discount.Application.Queries;
using Grpc.Core;
using MediatR;

namespace Discount.API.Services;

public class DiscountService(IMediator mediator) : DiscountProtoService.DiscountProtoServiceBase
{
    public override async Task<CouponModel> GetDiscount(GetDiscountRequest request, ServerCallContext context)
    {
        var query = new GetDiscountQuery(request.ProductName);
        var result = await mediator.Send(query);
        return result;
    }

    public override async Task<CouponModel> UpdateDiscount(UpdateDiscountRequest request, ServerCallContext context)
    {
        var command = new UpdateDiscountCommand(request.Coupon);
        var result = await mediator.Send(command);
        return result;
    }

    public override async Task<DeleteDiscountResponse> DeleteDiscount(DeleteDiscountRequest request,
        ServerCallContext context)
    {
        var command = new DeleteDiscountCommand(request.ProductName);
        var result = await mediator.Send(command);
        return new DeleteDiscountResponse()
        {
            Success = result
        };
    }

    public override async Task<CouponModel> CreateDiscount(CreateDiscountRequest request, ServerCallContext context)
    {
        var command = new CreateDiscountCommand(request.Coupon);
        return await mediator.Send(command);
    }
}