using Catalog.Application.Mappers;
using Catalog.Application.Responses;
using Catalog.Core.Repositories;
using Catalog.Core.Specs;
using MediatR;

namespace Catalog.Application.Queries;

public class GetAllProductsQuery : CatalogSpecParams, IRequest<Pagination<ProductResponse>>
{
};

public class GetAllProductsQueryHandler(IProductRepository productRepository)
    : IRequestHandler<GetAllProductsQuery, Pagination<ProductResponse>>
{
    public async Task<Pagination<ProductResponse>> Handle(GetAllProductsQuery request,
        CancellationToken cancellationToken)
    {
        var products = await productRepository.GetProducts(request);
        return LazyMapper.Mapper.Map<Pagination<ProductResponse>>(products);
    }
}