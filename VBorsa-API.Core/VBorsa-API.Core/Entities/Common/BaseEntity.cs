namespace VBorsa_API.Core.Entities.Common;

public class BaseEntity
{
    public Guid Id { get; set; }
    public DateTime CreateDateTime { get; set; } 
    public virtual DateTime UpdateDateTime { get; set; } 
}