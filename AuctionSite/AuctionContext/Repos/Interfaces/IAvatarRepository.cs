using AuctionPOCOs;
using System.Collections.Generic;

namespace AuctionContext
{
    public interface IAvatarRepository
    {
        List<AvatarImage> GetAvatarsForUser(int id);
        AvatarImage GetAvatar(int id);
        AvatarImage AddAvatar(AvatarImage avatar);
    }
}