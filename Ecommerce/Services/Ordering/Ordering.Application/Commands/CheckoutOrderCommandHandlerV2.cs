using MediatR;

namespace Ordering.Application.Commands;

public class CheckoutOrderCommandV2 : IRequest<int>
{
    public string? UserName { get; set; }
    public decimal? TotalPrice { get; set; }
}
public class CheckoutOrderCommandHandlerV2 : IRequestHandler<CheckoutOrderCommandV2,int>
{
    public Task<int> Handle(CheckoutOrderCommandV2 request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}