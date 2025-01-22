using Catalog.Application.Mappers;
using Catalog.Application.Responses;
using Catalog.Core.Repositories;
using MediatR;

namespace Catalog.Application.Queries;

public class GetProductByIdQuery(string id) : IRequest<ProductResponse>
{
    public string Id { get; set; } = id;
}

public class GetProductByIdQueryHandler(IProductRepository productRepository) : IRequestHandler<GetProductByIdQuery, ProductResponse>
{
    public async Task<ProductResponse> Handle(GetProductByIdQuery request, CancellationToken cancellationToken)
    {
        var product = await productRepository.GetProduct(request.Id);
        return LazyMapper.Mapper.Map<ProductResponse>(product);
    }
}