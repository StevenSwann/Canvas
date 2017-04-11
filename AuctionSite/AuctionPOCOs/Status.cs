using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AuctionPOCOs
{
    [Table("Statuses")]
    public class Status : BaseEntity 
    {
        [Required, MaxLength(50), Index(IsUnique = true)]
        public string StatusName { get; set; }
    }
}