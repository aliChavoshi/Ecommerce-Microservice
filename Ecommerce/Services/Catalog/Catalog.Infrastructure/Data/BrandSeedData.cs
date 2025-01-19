using Catalog.Core.Entities;
using MongoDB.Driver;
using System.Text.Json;

namespace Catalog.Infrastructure.Data;

public static class BrandSeedData
{
    public static void SeedData(IMongoCollection<ProductBrand> brandCollection)
    {
        var checkBrands = brandCollection.Find(x => true).Any();
        var pathJson = Path.Combine("Data", "SeedData", "brands.json");
        if (checkBrands) return;
        // Read the json file
        var brandData = File.ReadAllText(pathJson);
        var brands = JsonSerializer.Deserialize<List<ProductBrand>>(brandData);
        if (brands != null) brandCollection.InsertManyAsync(brands);
    }
}