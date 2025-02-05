using Discount.Core.Interfaces;
using MediatR;

namespace Discount.Application.Commands;

public class DeleteDiscountCommand(string productName) : IRequest<bool>
{
    public string ProductName { get; set; } = productName;
}

public class DeleteDiscountCommandHandler(IDiscountRepository discountRepository)
    : IRequestHandler<DeleteDiscountCommand, bool>
{
    public async Task<bool> Handle(DeleteDiscountCommand request, CancellationToken cancellationToken)
    {
        return await discountRepository.DeleteDiscount(request.ProductName);
    }
}