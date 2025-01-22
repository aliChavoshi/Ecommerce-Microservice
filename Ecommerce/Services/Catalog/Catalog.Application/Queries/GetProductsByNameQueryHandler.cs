using Catalog.Application.Mappers;
using Catalog.Application.Responses;
using Catalog.Core.Repositories;
using MediatR;

namespace Catalog.Application.Queries;

public class GetProductsByNameQuery(string name) : IRequest<List<ProductResponse>>
{
    public string Name { get; set; } = name;
}

public class GetProductsByNameQueryHandler(IProductRepository productRepository)
    : IRequestHandler<GetProductsByNameQuery, List<ProductResponse>>
{
    public async Task<List<ProductResponse>> Handle(GetProductsByNameQuery request, CancellationToken cancellationToken)
    {
        var products = await productRepository.GetProductsByName(request.Name);
        return LazyMapper.Mapper.Map<List<ProductResponse>>(products);
    }
}