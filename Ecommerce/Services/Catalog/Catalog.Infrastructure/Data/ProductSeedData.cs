using MongoDB.Driver;
using System.Text.Json;
using Product = Catalog.Core.Entities.Product;

namespace Catalog.Infrastructure.Data;

public static class ProductSeedData
{
    public static void SeedData(IMongoCollection<Product> productCollection)
    {
        var exist = productCollection.Find(x => true).Any();
        var pathJson = Path.Combine(AppContext.BaseDirectory, "Data", "SeedData", "products.json");
        if (exist) return;
        // Read the json file
        var stringData = File.ReadAllText(pathJson);
        var products = JsonSerializer.Deserialize<List<Product>>(stringData);
        if (products != null) productCollection.InsertManyAsync(products);
    }
}