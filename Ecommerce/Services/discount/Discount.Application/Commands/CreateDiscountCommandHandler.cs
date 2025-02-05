using AutoMapper;
using Discount.Application.Protos;
using Discount.Core.Entities;
using Discount.Core.Interfaces;
using Grpc.Core;
using MediatR;

namespace Discount.Application.Commands;

public class CreateDiscountCommand(CouponModel couponModel) : IRequest<CouponModel>
{
    public CouponModel CouponModel { get; set; } = couponModel;
}

public class CreateDiscountCommandHandler(IDiscountRepository discountRepository, IMapper mapper)
    : IRequestHandler<CreateDiscountCommand, CouponModel>
{
    public async Task<CouponModel> Handle(CreateDiscountCommand request, CancellationToken cancellationToken)
    {
        var coupon = mapper.Map<Coupon>(request.CouponModel);
        if (coupon == null)
            throw new RpcException(new Status(StatusCode.Unknown, " Create of the coupon had some errors"));
        if (await discountRepository.CreateDiscount(coupon))
            return request.CouponModel;

        throw new RpcException(new Status(StatusCode.Unknown, " Create of the coupon had some errors"));
    }
}