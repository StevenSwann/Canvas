using AuctionPOCOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;


namespace AuctionContext
{
    public class SqlServerBidRepository : IBidRepository
    {
        Context context;

        public SqlServerBidRepository()
        {
            context = new Context();
        }

        public SqlServerBidRepository(Context _context)
        {
            this.context = _context;
        }

        public List<Bid> GetAllBidsForUser(int id)
        {
            return context.Bids.Where(b => b.User.Id == id).ToList();
        }

        public List<Bid> GetAllBidsForListing(int id)
        {
            return context.Bids.Where(b => b.Listing.Id == id).ToList();
        }

        public Bid BidOnListing(Bid bid)
        {            
            context.Bids.Add(bid);
            context.SaveChanges();
            return bid;
        }

        public void DismissBid(List<Bid> bids)
        {
            foreach (Bid bid in bids)
            {
                Bid bidToEdit = context.Bids.Find(bid.Id);

                bidToEdit.Dismissed = true;  
            }                     
                       
            context.SaveChanges();           
        }
    }
}
