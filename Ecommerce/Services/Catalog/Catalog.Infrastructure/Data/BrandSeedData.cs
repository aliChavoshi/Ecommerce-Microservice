using Catalog.Core.Entities;
using MongoDB.Driver;
using System.Text.Json;

namespace Catalog.Infrastructure.Data;

public static class BrandSeedData
{
    public static void SeedData(IMongoCollection<ProductBrand> brandCollection)
    {
        var checkBrands = brandCollection.Find(x => true).Any();
        var pathJson = Path.Combine(AppContext.BaseDirectory, "Data", "SeedData", "brands.json");
        if (checkBrands) return;
        // Read the json file
        if (!File.Exists(pathJson))
        {
            throw new FileNotFoundException($"The seed data file was not found at path: {pathJson}");
        }

        var brandData = File.ReadAllText(pathJson);
        var brands = JsonSerializer.Deserialize<List<ProductBrand>>(brandData, new JsonSerializerOptions()
        {
            PropertyNameCaseInsensitive = false
        });
        if (brands != null) brandCollection.InsertManyAsync(brands);
    }
}