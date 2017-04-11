using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AuctionDTOs
{
    public class AvatarImageIdDTO
    {
        public int Id { get; set; }
    }

    public class AvatarImageDTO : AvatarImageIdDTO
    {
        public byte[] AvatarImageBytes { get; set; }

        public UserDTO User { get; set; }
    }
}
