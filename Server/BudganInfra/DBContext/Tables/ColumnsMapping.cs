using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BudganInfra.DBContext.Tables;

[Table("ColumnsMapping")]
public class ColumnsMapping : BaseEntity
{
    [MaxLength(100)]
    public required string Name { get; set; }

    public required int CardNumberColumnIndex { get; set; }
    
    [MaxLength(100)]
    public required string? CardNumberColumnText { get; set; }

    public required int DateInscriptionColumnIndex { get; set; }
    
    [MaxLength(100)]
    public required string? DateInscriptionColumnText { get; set; }

    public required int AmountColumnIndex { get; set; }
    
    [MaxLength(100)]
    public required string? AmountColumnText { get; set; }

    public required int DescriptionColumnIndex { get; set; }
    
    [MaxLength(100)]
    public required string? DescriptionColumnText { get; set; }
}
