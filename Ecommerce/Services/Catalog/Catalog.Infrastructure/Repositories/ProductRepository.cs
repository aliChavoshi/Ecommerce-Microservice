using Catalog.Core.Entities;
using Catalog.Core.Repositories;
using Catalog.Core.Specs;
using Catalog.Infrastructure.Data;
using MongoDB.Driver;

namespace Catalog.Infrastructure.Repositories;

public class ProductRepository(ICatalogContext context) : IProductRepository, IBrandRepository, ITypeRepository
{
    public async Task<IEnumerable<Product>> GetProducts()
    {
        return await context.Products.Find(x => true).ToListAsync();
    }

    public async Task<Pagination<Product>> GetProducts(CatalogSpecParams specParams)
    {
        var builder = Builders<Product>.Filter;
        var filter = builder.Empty;
        if (!string.IsNullOrEmpty(specParams.Search))
        {
            filter &= builder.Where(x => x.Name.ToLower().Contains(specParams.Search));
        }

        if (!string.IsNullOrEmpty(specParams.BrandId))
        {
            var brandFiler = builder.Eq(x => x.Brands.Id, specParams.BrandId);
            filter &= brandFiler;
        }

        if (!string.IsNullOrEmpty(specParams.TypeId))
        {
            var typeFilter = builder.Eq(x => x.Types.Id, specParams.TypeId);
            filter &= typeFilter;
        }

        var totalItems = await context.Products.CountDocumentsAsync(filter);
        var data = await FilterData(specParams, filter);
        return new Pagination<Product>(specParams.PageIndex, specParams.PageSize, (int)totalItems, data);
    }

    private async Task<List<Product>> FilterData(CatalogSpecParams specParams, FilterDefinition<Product> filter)
    {
        var sort = Builders<Product>.Sort.Ascending(x => x.Name); // default sort
        if (!string.IsNullOrEmpty(specParams.Sort))
        {
            sort = specParams.Sort switch
            {
                "priceAsc" => Builders<Product>.Sort.Ascending(x => x.Price),
                "priceDesc" => Builders<Product>.Sort.Descending(x => x.Price),
                _ => Builders<Product>.Sort.Ascending(x => x.Name)
            };
        }

        var data = await context.Products
            .Find(filter)
            .Sort(sort)
            .Skip(specParams.PageSize * (specParams.PageIndex - 1))
            .Limit(specParams.PageSize)
            .ToListAsync();
        return data;
    }

    public async Task<Product> GetProduct(string id)
    {
        return await context.Products.Find(p => p.Id == id).FirstOrDefaultAsync();
    }

    public async Task<IEnumerable<Product>> GetProductsByName(string name)
    {
        return await context.Products.Find(x => x.Name.Contains(name)).ToListAsync();
    }

    public async Task<IEnumerable<Product>> GetProductsByType(string type)
    {
        return await context.Products.Find(x => x.Types.Name == type).ToListAsync();
    }

    public async Task<IEnumerable<Product>> GetProductsByBrand(string brand)
    {
        return await context.Products.Find(x => x.Brands.Name == brand).ToListAsync();
    }

    public async Task<bool> UpdateProduct(Product product)
    {
        var result = await context.Products.ReplaceOneAsync(p => p.Id == product.Id, product);
        return result.IsAcknowledged && result.ModifiedCount > 0;
    }

    public async Task<bool> DeleteProduct(string id)
    {
        var result = await context.Products.DeleteOneAsync(x => x.Id == id);
        return result.IsAcknowledged && result.DeletedCount > 0;
    }

    public async Task<Product> CreateProduct(Product product)
    {
        await context.Products.InsertOneAsync(product);
        return product;
    }

    public async Task<List<ProductBrand>> GetBrands()
    {
        return await context.Brands.Find(x => true).ToListAsync();
    }

    public async Task<IEnumerable<ProductType>> GetTypes()
    {
        return await context.Types.Find(x => true).ToListAsync();
    }
}