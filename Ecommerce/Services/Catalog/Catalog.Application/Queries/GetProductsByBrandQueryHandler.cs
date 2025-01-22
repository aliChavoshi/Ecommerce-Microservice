using Catalog.Application.Mappers;
using Catalog.Application.Responses;
using Catalog.Core.Repositories;
using MediatR;

namespace Catalog.Application.Queries;

public class GetProductsByBrandQuery(string brand) : IRequest<IEnumerable<ProductResponse>>
{
    public string Brand { get; set; } = brand;
}

public class GetProductsByBrandQueryHandler(IProductRepository productRepository) : IRequestHandler<GetProductsByBrandQuery, IEnumerable<ProductResponse>>
{
    public async Task<IEnumerable<ProductResponse>> Handle(GetProductsByBrandQuery request, CancellationToken cancellationToken)
    {
        var products = await productRepository.GetProductsByBrand(request.Brand);
        return LazyMapper.Mapper.Map<List<ProductResponse>>(products);
    }
}