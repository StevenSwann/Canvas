using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AuctionPOCOs
{
    public class AvatarImage : BaseEntity
    {
        public byte[] AvatarImageBytes { get; set; }

        public virtual User User { get; set; }
        public int UserId { get; set; }
    }
}
