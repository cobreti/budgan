namespace BudganServices.UseCases.Account.GetList;

public class BOListAccount
{
    public required string Id { get; set; }
    public required string Name { get; set; }
    public required Guid ColumnsMappingId { get; set; }
    public required string AccountType { get; set; }
}
