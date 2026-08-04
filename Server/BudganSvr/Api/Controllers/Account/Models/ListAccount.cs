namespace BudganSvr.Api.Controllers.Account.Models;

public class ListAccount
{
    public required string Id { get; set; }
    public required string Name { get; set; }
    public required Guid ColumnsMappingId { get; set; }
    public required string AccountType { get; set; }
}
