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
    class SqlServerCommentRepositoryTests
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

            Comment comment = new Comment();
            comment.Id = 1;
            comment.Content = "comment";
            comment.Listing = listing;
            comment.User = user;
            context.Comments.Add(comment);
            context.SaveChanges();
        }

        [Test]
        public void GetAllCommentsForListing_ReturnsACommentListOfLengthZero_WhenListingIdIsPassedAndDatabaseIsEmpty()
        {
            // Arrange
            int expected = 0;
            int Id = 1;
            SqlServerCommentRepository commentRepo = new SqlServerCommentRepository(context);

            // Act
            int actual = commentRepo.GetAllCommentsForListing(Id).Count();

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void GetAllCommentsForListing_ReturnsACommentListOfLengthOne_WhenListingIdIsPassedAndPresentInDatabase()
        {
            // Arrange
            int expected = 1;
            int Id = 1;
            InsertData();
            SqlServerCommentRepository commentRepo = new SqlServerCommentRepository(context);

            // Act
            int actual = commentRepo.GetAllCommentsForListing(Id).Count();

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void AddComment_AddsCommentToDatabaseAndReturnsACommentOfIdValueOne_WhenCalledWithCommentObjectOfIdValueOne()
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
            Comment comment = new Comment() { Id = 1, Content = "comment", Listing = listing, User = user };
            SqlServerCommentRepository commentRepo = new SqlServerCommentRepository(context);

            // Act
            commentRepo.AddComment(comment);
            int actual = context.Comments.Count();

            // Assert
            Assert.AreEqual(expected, actual);
        }
    }
}