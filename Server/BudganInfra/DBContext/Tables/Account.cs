using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BudganInfra.DBContext.Tables;

[Table("Account")]
public class Account : BaseEntity
{
    [MaxLength(100)]
    public required string Name { get; set; }
    
    public required Guid ColumnsMappingId { get; set; }
    public ColumnsMapping ColumnsMapping { get; set; }

    [MaxLength(50)]
    public required string AccountType { get; set; }
}
