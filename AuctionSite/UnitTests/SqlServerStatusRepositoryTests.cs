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
    class SqlServerStatusRepositoryTests
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
            Status status = new Status();
            status.Id = 1;
            status.StatusName = "name";
            context.Statuses.Add(status);
            context.SaveChanges();
        }

        [Test]
        public void GetAllStatuses_ReturnsAListOfStatusesOfLengthZero_WhenCalledAndTheDatabaseIsEmpty()
        {
            // Arrange
            int expected = 0;
            SqlServerStatusRepository statusRepo = new SqlServerStatusRepository(context);

            // Act
            int actual = statusRepo.GetAllStatuses().Count();

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void GetAllStatuses_ReturnsAListOfStatusesOfLengthOne_WhenCalledAndTheDatabaseHasOneStatusObject()
        {
            // Arrange
            int expected = 1;
            InsertData();
            SqlServerStatusRepository statusRepo = new SqlServerStatusRepository(context);

            // Act
            int actual = statusRepo.GetAllStatuses().Count();

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void GetStatusById_ReturnsAStatusOfIdValueOfOne_WhenCalledWithValidIdOfValueOnePresentInDatabase()
        {
            // Arrange
            int expected = 1;
            int id = 1;
            InsertData();
            SqlServerStatusRepository statusRepo = new SqlServerStatusRepository(context);

            // Act
            int actual = statusRepo.GetStatusById(id).Id;

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void GetStatusById_ReturnsADefaultStatusOfIdValueOfZero_WhenCalledWithValidIdOfValueOneButDatabaseIsEmpty()
        {
            // Arrange
            int expected = 0;
            int id = 1;
            SqlServerStatusRepository statusRepo = new SqlServerStatusRepository(context);

            // Act
            int actual = statusRepo.GetStatusById(id).Id;

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void AddStatus_AddsStatusToDatabaseAndReturnsAStatusOfIdValueOfOne_WhenCalledWithValidIdOfValueOne()
        {
            // Arrange
            int expected = 1;
            Status status = new Status();
            status.Id = 1;
            status.StatusName = "name";
            SqlServerStatusRepository statusRepo = new SqlServerStatusRepository(context);

            // Act
            statusRepo.AddStatus(status);
            int actual = context.Statuses.ToList().Count();

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void CheckStatus_ReturnsABoolOfValueFalse_WhenCalledAndStatusNameIsNotInDatabase()
        {
            // Arrange
            bool expected = false;
            string status = "name";
            SqlServerStatusRepository statusRepo = new SqlServerStatusRepository(context);

            // Act
            bool actual = statusRepo.CheckStatus(status);

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void CheckStatus_ReturnsABoolOfValueTrue_WhenCalledAndStatusNamePresentInDatabase()
        {
            // Arrange
            bool expected = true;
            string statusName = "name";
            InsertData();
            SqlServerStatusRepository statusRepo = new SqlServerStatusRepository(context);
            
            // Act
            bool actual = statusRepo.CheckStatus(statusName);

            // Assert
            Assert.AreEqual(expected, actual);
        }
    }
}
