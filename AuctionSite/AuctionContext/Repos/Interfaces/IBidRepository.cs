using AuctionPOCOs;
using System.Collections.Generic;

namespace AuctionContext
{
    public interface IBidRepository
    {
        List<Bid> GetAllBidsForUser(int id);
        List<Bid> GetAllBidsForListing(int id);
        Bid BidOnListing(Bid bid);
        void DismissBid(List<Bid> bids);
    }
}
