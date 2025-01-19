using MongoDB.Bson.Serialization.Attributes;

namespace Catalog.Core.Entities;

public class ProductType : BaseEntity
{
    [BsonElement(nameof(Name))] public string Name { get; set; }
}