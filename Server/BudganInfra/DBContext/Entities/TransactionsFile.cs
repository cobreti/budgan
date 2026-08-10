using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BudganInfra.DBContext.Entities
{
    [Table("TransactionsFiles")]
    public class TransactionsFile : BaseEntity
    {
        public required Guid AccountId { get; set; }
        public Account Account { get; set; } = null!;

        public required string Content { get; set; }


        [MaxLength(450)]
        public required string Filename { get; set; }

        public required DateOnly InsertionDate { get; set; }
    }
}