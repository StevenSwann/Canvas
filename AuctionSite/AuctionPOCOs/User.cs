using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AuctionPOCOs
{
    public class User : BaseEntity
    {
        [Required, MaxLength(100), Index(IsUnique = true)]
        public string Username { get; set; }
        [Required, MaxLength(100), Index(IsUnique = true)]
        public string EmailAddress { get; set; }
        [Required]
        public string Password { get; set; }
        [Required]
        public string Address { get; set; }
        public virtual List<Listing> Listings { get; set; }
        public virtual List<Bid> Bids { get; set; }
        public virtual List<AvatarImage> AvatarImage { get; set; }
    }
}