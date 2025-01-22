using Catalog.Core.Repositories;
using MediatR;

namespace Catalog.Application.Commands;

public class DeleteProductCommand(string id) : IRequest<bool>
{
    public string Id { get; set; } = id;
}

public class DeleteProductCommandHandler(IProductRepository productRepository)
    : IRequestHandler<DeleteProductCommand, bool>
{
    public async Task<bool> Handle(DeleteProductCommand request, CancellationToken cancellationToken)
    {
        return await productRepository.DeleteProduct(request.Id);
    }
}