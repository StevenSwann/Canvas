using System.ComponentModel.DataAnnotations;

namespace AuctionPOCOs
{
    public class Comment : BaseEntity
    {
        [Required, MaxLength(500)]
        public string Content { get; set; }

        [Required]
        public virtual Listing Listing { get; set; }
        public int ListingId { get; set; }
        [Required]
        public virtual User User { get; set; }
        public int UserId { get; set; }
    }
}