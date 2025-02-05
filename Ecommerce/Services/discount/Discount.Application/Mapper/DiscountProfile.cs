using AutoMapper;
using Discount.Application.Protos;
using Discount.Core.Entities;

namespace Discount.Application.Mapper;

public class DiscountProfile : Profile
{
    public DiscountProfile()
    {
        CreateMap<Coupon, CouponModel>().ReverseMap();
    }
}