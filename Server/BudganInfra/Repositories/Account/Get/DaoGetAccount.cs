using BudganInfra.Repositories.Models;

namespace BudganInfra.Repositories.Account.Get;

public class DaoGetAccount : DaoBaseUpdateModel
{
    public required string Name { get; set; }
    public required Guid ColumnsMappingId { get; set; }
    public required string AccountType { get; set; }
}
