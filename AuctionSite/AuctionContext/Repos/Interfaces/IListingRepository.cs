using AuctionPOCOs;
using System;
using System.Collections.Generic;

namespace AuctionContext
{
    public interface IListingRepository
    {
        List<Listing> GetAllListings();
        Listing GetListingById(int id);
        Listing AddListing(Listing listing);
        List<Listing> GetAllListingsForUser(int id);
        void RemoveListing(Listing listing);
        Listing EditListing(Listing listing);
        List<Listing> GetListingsByString(string searchTerm, string category);
        List<Listing> GetListingsByStringAdvanced(string searchTerm, string category, string maximumPrice, string minimumPrice, DateTime auctionEndTime);
    }
}
