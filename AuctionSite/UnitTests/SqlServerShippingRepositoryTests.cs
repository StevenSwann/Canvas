using AuctionContext;
using AuctionPOCOs;
using EntityFramework.MoqHelper;
using Moq;
using NUnit.Framework;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;

namespace UnitTests
{
    [TestFixture]
    class SqlServerShippingRepositoryTests
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
            context.SaveChanges();
        }

        [Test]
        public void GetAllShippingStatuses_ReturnsAListOfShippingsOfLengthZero_WhenCalledAndTheDatabaseIsEmpty()
        {
            // Arrange
            int expected = 0;
            SqlServerShippingRepository shippingRepo = new SqlServerShippingRepository(context);

            // Act
            int actual = shippingRepo.GetAllShippingStatuses().Count();

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void GetAllShippingStatuses_ReturnsAListOfShippingsOfLengthOne_WhenCalledAndTheDatabaseHasOneShippingObject()
        {
            // Arrange
            int expected = 1;
            InsertData();
            SqlServerShippingRepository shippingRepo = new SqlServerShippingRepository(context);

            // Act
            int actual = shippingRepo.GetAllShippingStatuses().Count();

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void GetShippingStatusById_ReturnsAShippingOfIdValueOfOne_WhenCalledWithValidIdOfValueOnePresentInDatabase()
        {
            // Arrange
            int expected = 1;
            int id = 1;
            InsertData();
            SqlServerShippingRepository shippingRepo = new SqlServerShippingRepository(context);

            // Act
            int actual = shippingRepo.GetShippingStatusById(id).Id;

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void GetShippingStatusById_ReturnsADefaultShippingOfIdValueOfZero_WhenCalledWithValidIdOfValueOneButDatabaseIsEmpty()
        {
            // Arrange
            int expected = 0;
            int id = 1;
            SqlServerShippingRepository shippingRepo = new SqlServerShippingRepository(context);

            // Act
            int actual = shippingRepo.GetShippingStatusById(id).Id;

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void AddShipping_AddAShippingToDatabaseAndReturnsAShippingOfIdValueOfOne_WhenCalledWithValidIdOfValueOne()
        {
            // Arrange
            int expected = 1;
            Shipping shipping = new Shipping();
            shipping.Id = 1;
            shipping.ShipMode = "mode";
            SqlServerShippingRepository shippingRepo = new SqlServerShippingRepository(context);

            // Act
            shippingRepo.AddShipping(shipping);
            int actual = context.Shippings.Count();

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void CheckShippingMode_ReturnsABoolOfValueFalse_WhenCalledAndProductCategoryNameIsNotInDatabase()
        {
            // Arrange
            bool expected = false;
            string shipMode = "mode";
            SqlServerShippingRepository shippingRepo = new SqlServerShippingRepository(context);

            // Act
            bool actual = shippingRepo.CheckShippingMode(shipMode);

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void CheckShippingMode_ReturnsABoolOfValueTrue_WhenCalledAndProductCategoryNameIsInDatabase()
        {
            // Arrange
            bool expected = true;
            string shipMode = "mode";
            InsertData();
            SqlServerShippingRepository shippingRepo = new SqlServerShippingRepository(context);

            // Act
            bool actual = shippingRepo.CheckShippingMode(shipMode);

            // Assert
            Assert.AreEqual(expected, actual);
        }
    }
}
