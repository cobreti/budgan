using System.ComponentModel.DataAnnotations.Schema;

namespace BudganInfra.DBContext.Tables
{
    [Table("TransactionsFiles")]
    public class TransactionsFile : BaseEntity
    {
        public required Guid AccountId { get; set; }
        public Account Account { get; set; } = null!;

        public required string Content { get; set; }

        public required string Filename { get; set; }

        public required DateOnly InsertionDate { get; set; }
    }
}