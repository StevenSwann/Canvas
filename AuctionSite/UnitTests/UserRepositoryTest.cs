using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Moq;
using EntityFramework.MoqHelper;
using AuctionPOCOs;
using AuctionContext;
using AuctionContext.Repos.SqlServer;

namespace UnitTests
{
    [TestFixture]
    public class UserRepositoryTest
    {
        [Test]
        public void GetAllUsers_ReturnAnEmptyList_WhenCalled()
        {
            //Arrange
            List<User> listOfUsers = new List<User>();

            var mockDbSet = EntityFrameworkMoqHelper.CreateMockForDbSet<User>()
                .SetupForQueryOn(listOfUsers);
            var mockContext = EntityFrameworkMoqHelper.CreateMockForDbContext<Context, User>(mockDbSet);

            UserRepository userRepository = new UserRepository(mockContext.Object);
            int expected = 0;

            //Act
            List<User> actualresult = userRepository.GetAllUsers();

            //Assert
            Assert.AreEqual(expected, actualresult.Count);
        }

        [Test]
        public void GetAllUsers_ReturnAListContainingUsersFromDatabase_WhenMethodCalled()
        {
            //Arrange
            List<User> listOfUsers = new List<User>();
            Mock<User> mockUser = new Mock<User>();

            listOfUsers.Add(mockUser.Object);

            var mockDbSet = EntityFrameworkMoqHelper.CreateMockForDbSet<User>()
                .SetupForQueryOn(listOfUsers);
            var mockContext = EntityFrameworkMoqHelper.CreateMockForDbContext<Context, User>(mockDbSet);

            UserRepository userRepository = new UserRepository(mockContext.Object);

            //Act
            List<User> actualResult = userRepository.GetAllUsers();

            //Assert
            CollectionAssert.Contains(actualResult, mockUser.Object);
        }

        [Test]
        public void GetUserByUsername_ReturnUsersFormDatabaseWithUsernameMatchingStringPassedIntoAMethod_WhenPassedUsername()
        {
            //Arrange
            List<User> listOfUsers = new List<User>();
            string username = "Johny";
            Mock<User> mockUser = new Mock<User>();
            mockUser.Object.Username = username;
            listOfUsers.Add(mockUser.Object);

            var mockDbSet = EntityFrameworkMoqHelper.CreateMockForDbSet<User>()
                .SetupForQueryOn(listOfUsers)
                .WithFind(listOfUsers, "Username");
            var mockContext = EntityFrameworkMoqHelper.CreateMockForDbContext<Context, User>(mockDbSet);
            UserRepository userRepository = new UserRepository(mockContext.Object);

            //Act
            User actualResult = userRepository.GetUserByUsername(username);

            //Assert
            Assert.AreSame(mockUser.Object, actualResult);
        }

        [Test]
        public void AddNewUser_AddsNewUserToDatabase_WhenPassedInAUserObject()
        {
            //Arrange
            List<User> listOfUsers = new List<User>();
            Mock<User> mockUser = new Mock<User>();

            var mockDbSet = EntityFrameworkMoqHelper.CreateMockForDbSet<User>()
                .SetupForQueryOn(listOfUsers)
                .WithAdd(listOfUsers);
            var mockContext = EntityFrameworkMoqHelper.CreateMockForDbContext<Context, User>(mockDbSet);
            UserRepository userRepository = new UserRepository(mockContext.Object);

            //Act
            userRepository.AddNewUser(mockUser.Object);

            //Assert
            Assert.IsTrue(mockDbSet.Object.Contains(mockUser.Object));
        }
    }
}
