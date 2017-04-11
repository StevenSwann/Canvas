using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AuctionDTOs
{
    public class CommentDTO
    {
        public int Id { get; set; }
        public string Content { get; set; }
        public ListingDTO Listing { get; set; }
        public UserDTO User { get; set; }
    }
}
