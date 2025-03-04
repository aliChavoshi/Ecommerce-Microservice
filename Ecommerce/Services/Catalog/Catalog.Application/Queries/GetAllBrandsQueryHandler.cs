using AutoMapper;
using Catalog.Application.Responses;
using Catalog.Core.Repositories;
using MediatR;

namespace Catalog.Application.Queries;

public class GetAllBrandsQuery : IRequest<List<BrandResponse>>;

public class GetAllBrandsQueryHandler(IBrandRepository brandRepository, IMapper mapper)
    : IRequestHandler<GetAllBrandsQuery, List<BrandResponse>>
{
    public async Task<List<BrandResponse>> Handle(GetAllBrandsQuery request, CancellationToken cancellationToken)
    {
        var result = await brandRepository.GetBrands();
        return mapper.Map<List<BrandResponse>>(result);
    }
}