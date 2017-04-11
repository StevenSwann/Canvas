using System.ComponentModel.DataAnnotations;

namespace AuctionPOCOs
{
    public class Bid : BaseEntity
    {
        [Required]
        public decimal BidAmount { get; set; }

        [Required]
        public bool Dismissed { get; set; }
                
        public virtual Listing Listing { get; set; }
        public int ListingId { get; set; }

        public virtual User User { get; set; }
        public int UserId { get; set; }
    }
}