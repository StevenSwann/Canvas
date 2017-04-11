using AuctionPOCOs;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AuctionContext
{
    public class SqlServerListingRepository : IListingRepository
    {
        Context context;

        public SqlServerListingRepository()
        {
            context = new Context();
        }

        public SqlServerListingRepository(Context _context)
        {
            this.context = _context;
        }

        public List<Listing> GetAllListings()
        {
            return context.Listings.Where(l => l.Removed == false && System.DateTime.Now < l.AuctionEndTime ).ToList();
        }

        public Listing GetListingById(int id)
        {
            var listing = context.Listings.FirstOrDefault(l => l.Id == id);
            if (listing == null)
            {
                return new Listing();
            }
            return listing;
        }

        public Listing AddListing(Listing listing)
        {
            context.Listings.Add(listing);
            context.SaveChanges();
            return listing;
        }

        public List<Listing> GetAllListingsForUser(int id)
        {
            List<Listing> userListings = context.Listings.Where(l => l.UserId == id && l.Removed == false).ToList();
            if (userListings == null)
            {
                return new List<Listing>();
            }
            return userListings;
        }

        public void RemoveListing(Listing listing)
        {
            Listing listingToEdit = context.Listings.First(l => l.Id == listing.Id);

            listingToEdit.Removed = true;

            context.SaveChanges();
        }

        public Listing EditListing(Listing listing)
        {
            Listing listingToUpdate = context.Listings.First(l => l.Id == listing.Id);

            if (listingToUpdate != null)
            {
                listingToUpdate.ItemName = listing.ItemName;
                listingToUpdate.Description = listing.Description;
                listingToUpdate.ProductCategory = listing.ProductCategory;
                listingToUpdate.Price = listing.Price;
                listingToUpdate.Quantity = listing.Quantity;
                listingToUpdate.User = listing.User;
                listingToUpdate.Shipping = listing.Shipping;
                listingToUpdate.Status = listing.Status;
            }
            
            context.SaveChanges();

            return listingToUpdate;
        }

        public List<Listing> GetListingsByString(string searchTerm, string category)
        {
            List<Listing> listingsList = new List<Listing>();
            if (category == null)
            {
                IQueryable<Listing> listings = context.Listings.Where(s => s.ItemName.Contains(searchTerm) && s.Removed == false );
                IQueryable<Listing> listings2 = context.Listings.Where(s => s.Description.Contains(searchTerm) && s.Removed == false);
                IQueryable<Listing> listings3 = context.Listings.Where(s => s.User.Username.Contains(searchTerm) && s.Removed == false);

                if (listings.Count() > 0)
                {
                    foreach (var listing in listings)
                    {
                        listingsList.Add(listing);
                    }
                }

                if (listings2.Count() > 0)
                {
                    foreach (var listing in listings2)
                    {
                        listingsList.Add(listing);
                    }
                }

                if (listings3.Count() > 0)
                {
                    foreach (var listing in listings3)
                    {
                        listingsList.Add(listing);
                    }
                }

                return listingsList.Distinct().ToList();
            }
            else
            {
                IQueryable<Listing> listings = context.Listings.Where(s => s.ItemName.Contains(searchTerm) && s.ProductCategory.ProductCategoryName.Contains(category) && s.Removed == false);
                IQueryable<Listing> listings2 = context.Listings.Where(s => s.Description.Contains(searchTerm) && s.ProductCategory.ProductCategoryName.Contains(category) && s.Removed == false);
                IQueryable<Listing> listings3 = context.Listings.Where(s => s.User.Username.Contains(searchTerm) && s.ProductCategory.ProductCategoryName.Contains(category) && s.Removed == false);

                if (listings.Count() > 0)
                {
                    foreach (var listing in listings)
                    {
                        listingsList.Add(listing);
                    }
                }

                if (listings2.Count() > 0)
                {
                    foreach (var listing in listings2)
                    {
                        listingsList.Add(listing);
                    }
                }

                if (listings3.Count() > 0)
                {
                    foreach (var listing in listings3)
                    {
                        listingsList.Add(listing);
                    }
                }
                
                return listingsList.Distinct().ToList();
            }
        }

        public List<Listing> GetListingsByStringAdvanced(string searchTerm, string category, string maximumPrice, string minimumPrice, DateTime auctionEndTime)
        {
            List<Listing> listingsList = new List<Listing>();
            List<Listing> tempList = new List<Listing>();
            List<Listing> resultList = new List<Listing>();
            decimal maxPrice = 100000000;
            decimal minPrice = 0;

            if (searchTerm == null)
            {
                searchTerm = "";
            }

            if (maximumPrice != null)
            {
                maxPrice = decimal.Parse(maximumPrice);
            }            

            if (minimumPrice != null)
            {
                minPrice = decimal.Parse(minimumPrice);
            }            
            
            if (maximumPrice == null && minimumPrice == null)
            {
                IQueryable<Listing> listings = context.Listings.Where(s => s.Bids.Max(b => b.BidAmount) <= maxPrice && s.Bids.Max(b => b.BidAmount) >= minPrice && s.Removed == false);
                IQueryable<Listing> listings2 = context.Listings.Where(s => s.Bids.Count == 0 && s.Price <= maxPrice && s.Price >= minPrice && s.Removed == false);

                if (listings.Count() > 0)
                {
                    foreach (var listing in listings)
                    {
                        listingsList.Add(listing);
                    }
                }

                if (listings2.Count() > 0)
                {
                    foreach (var listing in listings2)
                    {
                        listingsList.Add(listing);
                    }
                }
            }
            else
            {
                IQueryable<Listing> listings = context.Listings.Where(s => s.Bids.Max(b => b.BidAmount) <= maxPrice && s.Bids.Max(b => b.BidAmount) >= minPrice && s.Removed == false);
                IQueryable<Listing> listings2 = context.Listings.Where(s => s.Bids.Count == 0 && s.Price <= maxPrice && s.Price >= minPrice && s.Removed == false);

                if (listings.Count() > 0)
                {
                    foreach (var listing in listings)
                    {
                        listingsList.Add(listing);
                    }
                }

                if (listings2.Count() > 0)
                {
                    foreach (var listing in listings2)
                    {
                        listingsList.Add(listing);
                    }
                }

            }           
            
            if (auctionEndTime == DateTime.MinValue || auctionEndTime == null)
            {
                tempList = new List<Listing>(listingsList);
                listingsList.Clear();
            }
            else
            {
                if (listingsList.Count == 0)
                {
                    tempList = new List<Listing>(listingsList);
                    listingsList.Clear();                
                }
                else
                {
                    IEnumerable<Listing> listings = listingsList.Where(s => s.AuctionEndTime.Ticks <= auctionEndTime.Ticks);
                    
                    if (listings.Count() > 0)
                    {
                        foreach (var listing in listings)
                        {
                            tempList.Add(listing);
                        }
                    }
                    listingsList.Clear();
                }              
            }

            if (searchTerm == "")
            {
                listingsList = new List<Listing>(tempList);
                tempList.Clear();
            }
            else
            {
                if (tempList.Count == 0)
                {
                    listingsList = new List<Listing>(tempList);
                    tempList.Clear();
                }
                else
                {
                    IEnumerable<Listing> listings = tempList.Where(s => s.ItemName.Contains(searchTerm));
                    IEnumerable<Listing> listings2 = tempList.Where(s => s.Description.Contains(searchTerm));
                    IEnumerable<Listing> listings3 = tempList.Where(s => s.User.Username.Contains(searchTerm));

                        if (listings.Count() > 0)
                        {
                            foreach (var listing in listings)
                            {
                                listingsList.Add(listing);
                            }
                        }

                        if (listings2.Count() > 0)
                        {
                            foreach (var listing in listings2)
                            {
                                listingsList.Add(listing);
                            }
                        }

                        if (listings3.Count() > 0)
                        {
                            foreach (var listing in listings3)
                            {
                                listingsList.Add(listing);
                            }
                        }

                        tempList.Clear();
                }
            }

            if (category == null)
            {
                tempList = new List<Listing>(listingsList);
                listingsList.Clear();
            }
            else
            {
                if (listingsList.Count() == 0)
                {
                    tempList = new List<Listing>(listingsList);
                    listingsList.Clear();
                }
                else
                {
                        IEnumerable<Listing> listings = listingsList.Where(s => s.ProductCategory.ProductCategoryName.Contains(category));
                        IEnumerable<Listing> listings2 = listingsList.Where(s => s.ProductCategory.ProductCategoryName.Contains(category));
                        IEnumerable<Listing> listings3 = listingsList.Where(s => s.ProductCategory.ProductCategoryName.Contains(category));

                        if (listings.Count() > 0)
                        {
                            foreach (var listing in listings)
                            {
                                tempList.Add(listing);
                            }
                        }

                        if (listings2.Count() > 0)
                        {
                            foreach (var listing in listings2)
                            {
                                tempList.Add(listing);
                            }
                        }

                        if (listings3.Count() > 0)
                        {
                            foreach (var listing in listings3)
                            {
                                tempList.Add(listing);
                            }
                        }

                        listingsList.Clear();
                }
            }
            
            if (listingsList.Count != 0)
            {               
                resultList = listingsList;
            }

            if (tempList.Count != 0)
            {                
                resultList = tempList;
            }

            return resultList.Distinct().ToList();
        }

    }
}
