using AutoMapper;
using Discount.Application.Protos;
using Discount.Core.Entities;
using Discount.Core.Interfaces;
using MediatR;

namespace Discount.Application.Commands;

public class UpdateDiscountCommand(CouponModel couponModel) : IRequest<CouponModel>
{
    public CouponModel CouponModel { get; set; } = couponModel;
}

public class UpdateDiscountCommandHandler(IDiscountRepository discountRepository, IMapper mapper)
    : IRequestHandler<UpdateDiscountCommand, CouponModel>
{
    public async Task<CouponModel> Handle(UpdateDiscountCommand request, CancellationToken cancellationToken)
    {
        var coupon = mapper.Map<Coupon>(request.CouponModel);
        await discountRepository.UpdateDiscount(coupon);
        return request.CouponModel;
    }
}