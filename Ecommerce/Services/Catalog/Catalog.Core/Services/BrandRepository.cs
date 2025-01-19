using Catalog.Core.Entities;
using Catalog.Core.Repositories;

namespace Catalog.Core.Services;

public class BrandRepository : IBrandRepository
{
    public Task<IEnumerable<ProductBrand>> GetBrands()
    {
        throw new NotImplementedException();
    }
}