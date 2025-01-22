using AutoMapper;
using Catalog.Application.Responses;
using Catalog.Core.Entities;
using Catalog.Core.Repositories;
using MediatR;

namespace Catalog.Application.Queries;

public class GetAllBrandsQuery : IRequest<IEnumerable<BrandResponse>>
{
}

public class GetAllBrandsQueryHandler(IBrandRepository brandRepository, IMapper mapper)
    : IRequestHandler<GetAllBrandsQuery, IEnumerable<BrandResponse>>
{
    public async Task<IEnumerable<BrandResponse>> Handle(GetAllBrandsQuery request, CancellationToken cancellationToken)
    {
        var result = await brandRepository.GetBrands();
        return mapper.Map<IEnumerable<BrandResponse>>(result);
    }
}