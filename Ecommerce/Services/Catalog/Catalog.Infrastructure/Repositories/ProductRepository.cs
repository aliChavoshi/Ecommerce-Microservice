using Catalog.Core.Entities;
using Catalog.Core.Repositories;
using Catalog.Core.Specs;
using Catalog.Infrastructure.Data;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Catalog.Infrastructure.Repositories;

public class ProductRepository(ICatalogContext context) : IProductRepository, IBrandRepository, ITypeRepository
{
    public async Task<Pagination<Product>> GetProducts(CatalogSpecParams specParams)
    {
        var builder = Builders<Product>.Filter;
        var filter = builder.Empty;

        // Search (Case-insensitive)
        if (!string.IsNullOrEmpty(specParams.Search))
        {
            filter &= builder.Regex(x => x.Name, new BsonRegularExpression(specParams.Search, "i"));
        }

        // Brand filter
        if (!string.IsNullOrEmpty(specParams.BrandId))
        {
            filter &= builder.Eq(x => x.Brands.Id, specParams.BrandId);
        }

        // Type filter
        if (!string.IsNullOrEmpty(specParams.TypeId))
        {
            filter &= builder.Eq(x => x.Types.Id, specParams.TypeId);
        }

        // Count total before pagination
        var totalItems = await context.Products.CountDocumentsAsync(filter);

        // Get data with pagination
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

        // Ensure PageIndex is valid
        var pageIndex = specParams.PageIndex < 1 ? 1 : specParams.PageIndex;

        return await context.Products
            .Find(filter)
            .Sort(sort)
            .Skip(specParams.PageSize * (pageIndex - 1))
            .Limit(specParams.PageSize)
            .ToListAsync();
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