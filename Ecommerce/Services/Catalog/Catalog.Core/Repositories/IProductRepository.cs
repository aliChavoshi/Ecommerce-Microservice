using Catalog.Core.Entities;
using Catalog.Core.Specs;

namespace Catalog.Core.Repositories;

public interface IProductRepository
{
    Task<Pagination<Product>> GetProducts(CatalogSpecParams specParams);
    Task<Product> GetProduct(string id);
    Task<IEnumerable<Product>> GetProductsByName(string name);
    Task<IEnumerable<Product>> GetProductsByType(string type);
    Task<IEnumerable<Product>> GetProductsByBrand(string brand);
    Task<bool> UpdateProduct(Product product);
    Task<bool> DeleteProduct(string id);
    Task<Product> CreateProduct(Product product);
}