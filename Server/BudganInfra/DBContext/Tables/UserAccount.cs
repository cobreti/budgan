using System.ComponentModel.DataAnnotations;

namespace BudganInfra.DBContext.Tables;

public class UserAccount
{
    [Key]
    public required Guid Id { get; set; }
    
    [MaxLength(100)]
    public required string Name { get; set; }

    public required bool IsDefault { get; set; }
}
