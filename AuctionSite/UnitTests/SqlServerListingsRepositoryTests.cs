using AuctionContext;
using AuctionPOCOs;
using EntityFramework.MoqHelper;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;

namespace UnitTests
{
    [TestFixture]
    class SqlServerListingsRepositoryTests
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
            listing.AuctionEndTime = DateTime.Now.AddDays(1);
            listing.Price = 0;
            listing.Removed = false;
            listing.Description = "descr";
            listing.ImageUrl = "/not_found";
            listing.User = user;
            listing.ProductCategory = category;
            listing.Shipping = shipping;
            listing.Status = status;
            context.Listings.Add(listing);
            context.SaveChanges();
        }

        [Test]
        public void GetAllListings_ReturnsAListingListOfValueZero_WhenCalledAndDatabaseIsEmpty()
        {
            // Arrange
            int expected = 0;
            SqlServerListingRepository listingRepo = new SqlServerListingRepository(context);

            // Act
            int actual = listingRepo.GetAllListings().Count();

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void GetAllListings_ReturnsAListingListOfLengthOne_WhenCalledAndDatabaseHasOneListing()
        {
            // Arrange
            int expected = 1;
            InsertData();                 
            SqlServerListingRepository listingRepo = new SqlServerListingRepository(context);

            // Act
            int actual = listingRepo.GetAllListings().Count();

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void GetListingById_ReturnsADefaultListingIdOfValueZero_WhenCalledAndDatabaseIsEmpty()
        {
            // Arrange
            int expected = 0;
            int id = 1;
            SqlServerListingRepository listingRepo = new SqlServerListingRepository(context);

            // Act
            int actual = listingRepo.GetListingById(id).Id;

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void GetListingById_ReturnsAListingIdOfValueOne_WhenCalledAndDatabaseHasOneListingOfIdOne()
        {
            // Arrange
            int expected = 1;
            int id = 1;
            InsertData();
            SqlServerListingRepository listingRepo = new SqlServerListingRepository(context);

            // Act
            int actual = listingRepo.GetListingById(id).Id;

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void AddListing_AddsListingToDatabaseAndReturnsListingOfIdValueOne_WhenCalledWithListingObjectOfIdValueOne()
        {
            // Arrange
            int expected = 1;
            Shipping shipping = new Shipping() { Id = 1, ShipMode = "mode" };
            ProductCategory category = new ProductCategory() { Id = 1, ProductCategoryName = "name" };
            Status status = new Status() { Id = 1, StatusName = "name" };
            User user = new User() { Id = 1, EmailAddress = "email@address.com", Password = "password", Username = "username",
                                        Address = "address"};
            Listing listing = new Listing();
            listing.Id = 1;
            listing.ItemName = "name";
            listing.Quantity = 1;
            listing.AuctionStartTime = DateTime.Now.Date;
            listing.AuctionEndTime = DateTime.Now.Date;
            listing.Price = 0;
            listing.Description = "descr";
            listing.ImageUrl = "/image";
            listing.Shipping = shipping;
            listing.ProductCategory = category;
            listing.Status = status;
            listing.User = user;
            SqlServerListingRepository listingRepo = new SqlServerListingRepository(context);

            // Act
            listingRepo.AddListing(listing);
            int actual = context.Listings.Count();

            // Assert
            Assert.AreEqual(expected, actual);
        }
    }
}