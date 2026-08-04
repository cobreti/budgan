namespace BudganSvr.Api.Controllers.Account.Models;

public class AddOrUpdateAccount
{
    public string? Id { get; set; }
    public DateTime? Timestamp { get; set; }
    public required string Name { get; set; }
    public required Guid ColumnsMappingId { get; set; }
    public required string AccountType { get; set; }
}
