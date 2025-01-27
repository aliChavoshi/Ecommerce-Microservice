using Basket.Core.Repositories;
using MediatR;

namespace Basket.Application.Commands;

public class DeleteBasketByUserNameCommand : IRequest<bool>
{
    public string UserName { get; set; }

    public DeleteBasketByUserNameCommand(string userName)
    {
        UserName = userName;
    }
}
public class DeleteBasketByUserNameCommandHandler(IBasketRepository basketRepository) : IRequestHandler<DeleteBasketByUserNameCommand,bool>
{
    public async Task<bool> Handle(DeleteBasketByUserNameCommand request, CancellationToken cancellationToken)
    {
        await basketRepository.DeleteBasket(request.UserName);
        return true;
    }
}