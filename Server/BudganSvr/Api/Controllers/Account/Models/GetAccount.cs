namespace BudganSvr.Api.Controllers.Account.Models;

public class GetAccount
{
    public required string Id { get; set; }
    public required DateTime Timestamp { get; set; }
    public required string Name { get; set; }
    public required Guid ColumnsMappingId { get; set; }
    public required string AccountType { get; set; }
}
