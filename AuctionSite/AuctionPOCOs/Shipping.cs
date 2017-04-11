using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AuctionPOCOs
{
    public class Shipping : BaseEntity
    {
        [Required, MaxLength(50), Index(IsUnique = true)]
        public string ShipMode { get; set; }
    }
}