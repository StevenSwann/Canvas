using AuctionContext;
using AuctionPOCOs;
using EntityFramework.MoqHelper;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;

namespace UnitTests
{
    [TestFixture]
    class SqlServerBidRepositoryTests
    {
        Context context;

        [SetUp]
        public void SetUp()
        {
            DbConnection connection = Effort.DbConnectionFactory.CreateTransient();
            context = new Context(connection);
        }

        public void InsertData()
        {
            Shipping shipping = new Shipping();
            shipping.Id = 1;
            shipping.ShipMode = "mode";
            context.Shippings.Add(shipping);

            ProductCategory category = new ProductCategory();
            category.Id = 1;
            category.ProductCategoryName = "name";
            context.ProductCategories.Add(category);

            Status status = new Status();
            status.Id = 1;
            status.StatusName = "name";
            context.Statuses.Add(status);

            User user = new User();
            user.Id = 1;
            user.EmailAddress = "email@address.com";
            user.Password = "password";
            user.Username = "username";
            user.Address = "address";
            context.Users.Add(user);
            context.SaveChanges();

            Listing listing = new Listing();
            listing.Id = 1;
            listing.ItemName = "name";
            listing.Quantity = 1;
            listing.AuctionStartTime = DateTime.Now.Date;
            listing.AuctionEndTime = DateTime.Now.Date;
            listing.Price = 0;
            listing.Description = "descr";
            listing.ImageUrl = "/not_found";
            listing.User = user;
            listing.ProductCategory = category;
            listing.Shipping = shipping;
            listing.Status = status;
            context.Listings.Add(listing);
            context.SaveChanges();

            Bid bid = new Bid();
            bid.Id = 1;
            bid.BidAmount = 0;
            bid.Listing = listing;
            bid.User = user;
            context.Bids.Add(bid);
            context.SaveChanges();
        }

        [Test]
        public void GetAllBidsForUser_ReturnsABidListOfLengthZero_WhenUserIdIsPassedAndDatabaseIsEmpty()
        {
            // Arrange
            int expected = 0;
            int Id = 1;
            SqlServerBidRepository bidRepo = new SqlServerBidRepository(context);

            // Act
            int actual = bidRepo.GetAllBidsForUser(Id).Count();

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void GetAllBidsForUser_ReturnsABidListOfLengthOne_WhenUserIdOneIsPassedAndIsPresentInDatabase()
        {
            // Arrange            
            int expected = 1;
            int id = 1;
            InsertData();
            SqlServerBidRepository bidRepo = new SqlServerBidRepository(context);

            // Act
            int actual = bidRepo.GetAllBidsForUser(id).Count();

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void GetAllBidsForListing_ReturnsABidListOfLengthZero_WhenListingIdIsPassedAndDatabaseIsEmpty()
        {
            // Arrange
            int expected = 0;
            int id = 1;
            SqlServerBidRepository bidRepo = new SqlServerBidRepository(context);

            // Act
            int actual = bidRepo.GetAllBidsForListing(id).Count();

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void GetAllBidsForListing_ReturnsABidListOfLengthOne_WhenUserIdOneIsPassedAndIsPresentInDatabase()
        {
            // Arrange            
            int expected = 1;
            int id = 1;
            InsertData();
            SqlServerBidRepository bidRepo = new SqlServerBidRepository(context);

            // Act
            int actual = bidRepo.GetAllBidsForListing(id).Count();

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void BidOnListing_AddsANewBidToDatabase_WhenCalledWithBidObject()
        {
            // Arrange
            int expected = 2;
            InsertData();
            Bid bid = new Bid() { Id = 1, ListingId = 1, UserId = 1, BidAmount = 0 };
            SqlServerBidRepository bidRepo = new SqlServerBidRepository(context);

            // Act
            bidRepo.BidOnListing(bid);
            int actual = context.Bids.ToList().Count();

            // Assert
            Assert.AreEqual(expected, actual);
        }
    }
}