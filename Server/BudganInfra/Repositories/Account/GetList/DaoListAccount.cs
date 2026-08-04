namespace BudganInfra.Repositories.Account.GetList;

public class DaoListAccount
{
    public required Guid Id { get; set; }
    public required string Name { get; set; }
    public required Guid ColumnsMappingId { get; set; }
    public required string AccountType { get; set; }
}
