using BudganInfra.Repositories.Models;

namespace BudganInfra.Repositories.Account.Save;

public class DaoSaveAccount : DaoBaseUpdateModel
{
    public required string Name { get; set; }
    public required Guid ColumnsMappingId { get; set; }
    public required string AccountType { get; set; }
}
