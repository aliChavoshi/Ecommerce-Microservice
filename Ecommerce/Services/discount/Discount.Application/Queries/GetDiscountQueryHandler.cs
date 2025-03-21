﻿using AutoMapper;
using Discount.Application.Protos;
using Discount.Core.Interfaces;
using Grpc.Core;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Discount.Application.Queries;

public class GetDiscountQuery(string productName) : IRequest<CouponModel>
{
    public string ProductName { get; set; } = productName;
}

public class GetDiscountQueryHandler(
    IDiscountRepository discountRepository,
    IMapper mapper,
    ILogger<GetDiscountQueryHandler> logger)
    : IRequestHandler<GetDiscountQuery, CouponModel>
{
    public async Task<CouponModel> Handle(GetDiscountQuery request, CancellationToken cancellationToken)
    {
        var coupon = await discountRepository.GetDiscount(request.ProductName);
        if (coupon == null)
            throw new RpcException(new Status(StatusCode.NotFound, $"Discount not found for {request.ProductName}"));
        logger.LogInformation("Discount is retrieved for ProductName : {ProductName}, Amount : {Amount}",
            coupon.ProductName, coupon.Amount);
        return mapper.Map<CouponModel>(coupon);
    }
}