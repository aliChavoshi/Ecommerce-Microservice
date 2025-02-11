namespace Ordering.Core.Common;

public abstract class EntityBase
{
    public int Id { get; protected set; }
    public string? CreatedBy { get; set; }
    public DateTime CreatedDate { get; set; } = DateTime.Now;
    public string? LastModifiedBy { get; set; }
    public DateTime? LastModifiedDate { get; set; }
}