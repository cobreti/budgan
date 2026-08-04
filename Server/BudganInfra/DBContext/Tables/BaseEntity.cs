using System.ComponentModel.DataAnnotations;

namespace BudganInfra.DBContext.Tables;

public class BaseEntity
{
    [Key]
    public required Guid Id { get; set; }

    public DateTime Timestamp { get; set; }
}
