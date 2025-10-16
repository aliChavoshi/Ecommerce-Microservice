using Catalog.Core.Entities;
using MongoDB.Driver;
using System.Text.Json;

namespace Catalog.Infrastructure.Data;

public static class TypeSeedData
{
    public static void SeedData(IMongoCollection<ProductType> typeCollection)
    {
        var checkTypes = typeCollection.Find(x => true).Any();
        var pathJson = Path.Combine(AppContext.BaseDirectory, "Data", "SeedData", "types.json");
        if (checkTypes) return;
        // Read the json file
        var typesData = File.ReadAllText(pathJson);
        var types = JsonSerializer.Deserialize<List<ProductType>>(typesData, new JsonSerializerOptions()
        {
            PropertyNameCaseInsensitive = false
        });
        if (types != null) typeCollection.InsertManyAsync(types);
    }
}