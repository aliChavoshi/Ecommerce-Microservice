using Catalog.Application.Mappers;
using Catalog.Application.Responses;
using Catalog.Core.Repositories;
using MediatR;

namespace Catalog.Application.Queries;

public class GetAllTypesQuery : IRequest<IEnumerable<TypeResponse>>
{
}

public class GetAllTypesQueryHandler(ITypeRepository productRepository)
    : IRequestHandler<GetAllTypesQuery, IEnumerable<TypeResponse>>
{
    public async Task<IEnumerable<TypeResponse>> Handle(GetAllTypesQuery request, CancellationToken cancellationToken)
    {
        var types = await productRepository.GetTypes();
        return LazyMapper.Mapper.Map<IEnumerable<TypeResponse>>(types);
    }
}