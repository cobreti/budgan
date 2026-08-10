using System.ComponentModel.DataAnnotations;

namespace BudganInfra.DBContext.Entities;

public class BaseEntity
{
    [Key]
    public required Guid Id { get; set; }

    public DateTime Timestamp { get; set; }
}
