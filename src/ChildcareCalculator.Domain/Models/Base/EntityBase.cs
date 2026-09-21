using System.ComponentModel.DataAnnotations;

namespace ChildcareCalculator.Domain.Models.Base;

public abstract class EntityBase
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAtUtc { get; set; }
    public string? CreatedBy { get; set; }
    public string? UpdatedBy { get; set; }
    public int Version { get; set; } = 1;
}
