using AuctionContext;
using AuctionPOCOs;
using Effort.DataLoaders;
using EntityFramework.MoqHelper;
using Moq;
using NUnit.Framework;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;

namespace UnitTests
{
    [TestFixture]
    class SqlServerUserRepositoryTests
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
            User user = new User();
            user.Id = 1;
            user.EmailAddress = "email@address.com";
            user.Password = "password";
            user.Username = "username";
            user.Address = "address";
            context.Users.Add(user);
            context.SaveChanges();
        }

        [Test]
        public void GetAllUsers_ReturnsAListOfUsersOfLengthZero_WhenCalledAndTheDatabaseIsEmpty()
        {
            using (context = new Context(Effort.DbConnectionFactory.CreateTransient()))
            {
                // Arrange
                int expected = 0;
                SqlServerUserRepository userRepo = new SqlServerUserRepository(context);

                // Act
                int actual = userRepo.GetAllUsers().Count();

                // Assert
                Assert.AreEqual(expected, actual);
            }
        }

        [Test]
        public void GetAllUsers_ReturnsAListOfUsersOfLengthOne_WhenCalledAndTheDatabaseHasOneUserObject()
        {
            // Arrange
            int expected = 1;
            InsertData();
            SqlServerUserRepository userRepo = new SqlServerUserRepository(context);

            // Act
            int actual = userRepo.GetAllUsers().Count();

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void GetUserById_ReturnsAUserOfIdValueOfOne_WhenCalledWithValidIdOfValueOnePresentInDatabase()
        {
            // Arrange
            int expected = 1;
            int Id = 1;
            InsertData();
            SqlServerUserRepository userRepo = new SqlServerUserRepository(context);

            // Act
            int actual = userRepo.GetUserById(Id).Id;

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void GetUserById_ReturnsADefaultUserOfIdValueOfZero_WhenCalledWithValidIdOfValueOneButDatabaseIsEmpty()
        {
            // Arrange
            int expected = 0;
            int Id = 1;
            SqlServerUserRepository userRepo = new SqlServerUserRepository(context);

            // Act
            int actual = userRepo.GetUserById(Id).Id;

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void CheckUsername_ReturnsABoolOfValueFalse_WhenCalledWithValidUsernameAndDatabaseIsEmpty()
        {
            // Arrange
            bool expected = false;
            string Username = "username";
            SqlServerUserRepository userRepo = new SqlServerUserRepository(context);

            // Act
            bool actual = userRepo.CheckUsername(Username);

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void CheckUsername_ReturnsABoolOfValueTrue_WhenCalledWithValidUsernamePresentInDatabase()
        {
            // Arrange
            bool expected = true;
            string Username = "username";
            InsertData();
            SqlServerUserRepository userRepo = new SqlServerUserRepository(context);

            // Act
            bool actual = userRepo.CheckUsername(Username);

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void CheckEmailAddress_ReturnsABoolOfValueFalse_WhenCalledWithValidEmailAddressAndDatabaseIsEmpty()
        {
            // Arrange
            bool expected = false;
            string EmailAddress = "email@address.com";
            SqlServerUserRepository userRepo = new SqlServerUserRepository(context);

            // Act
            bool actual = userRepo.CheckEmailAddress(EmailAddress);

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void CheckEmailAddress_ReturnsABoolOfValueTrue_WhenCalledWithValidEmailAddressPresentInDatabase()
        {
            // Arrange
            bool expected = true;
            string EmailAddress = "email@address.com";
            InsertData();
            SqlServerUserRepository userRepo = new SqlServerUserRepository(context);

            // Act
            bool actual = userRepo.CheckEmailAddress(EmailAddress);

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void AddNewUser_AddsANewUserToDatabaseAndReturnsUserOfIdValueOne_WhenCalledWithUserObjectOfIdValueOne()
        {
            // Arrange
            int expected = 1;
            User user = new User();
            user.Id = 1;
            user.EmailAddress = "email@address.com";
            user.Password = "password";
            user.Username = "username";
            user.Address = "address";
            SqlServerUserRepository userRepo = new SqlServerUserRepository(context);

            // Act
            userRepo.AddNewUser(user);
            int actual = context.Users.ToList().Count();

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void LoginUser_ReturnsADefaultUserOfIdValueOfZero_WhenCalledWithMismatchingEmailAddressAndPassword()
        {
            // Arrange
            int expected = 0;
            InsertData();
            SqlServerUserRepository userRepo = new SqlServerUserRepository(context);

            User loginUser = new User();
            loginUser.Id = 1;
            loginUser.EmailAddress = "other@address.com";
            loginUser.Password = "wrongPassword";
            loginUser.Username = "username";
            loginUser.Address = "address";

            // Act
            int actual = userRepo.LoginUser(loginUser).Id;

            // Assert
            Assert.AreEqual(expected, actual);
        }
    }
}