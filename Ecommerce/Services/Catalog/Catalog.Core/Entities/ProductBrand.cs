using MongoDB.Bson.Serialization.Attributes;

namespace Catalog.Core.Entities;

public class ProductBrand : BaseEntity
{
    [BsonElement(nameof(Name))] public string Name { get; set; }
}