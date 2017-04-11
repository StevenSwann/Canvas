using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace AuctionDTOs
{
    public class BidDTO
    {
        public int Id { get; set; }
        public bool Dismissed { get; set; }
        [Required(ErrorMessage = "A bid amount is required.")]
        [DataType(DataType.Currency)]  
        [Range(0.01, 10000000)]
        public decimal BidAmount { get; set; }
        public UserDTO User { get; set; }
        public ListingDTO Listing { get; set; }
    }
}
