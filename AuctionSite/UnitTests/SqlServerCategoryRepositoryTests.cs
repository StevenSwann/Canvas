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
    class SqlServerCategoryRepositoryTests
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
            ProductCategory category = new ProductCategory();
            category.Id = 1;
            category.ProductCategoryName = "name";
            context.ProductCategories.Add(category);
            context.SaveChanges();
        }

        [Test]
        public void GetAllCategories_ReturnsAListOfProductCategoriesOfLengthZero_WhenCalledAndTheDatabaseIsEmpty()
        {
            // Arrange
            int expected = 0;
            SqlServerCategoryRepository categoryRepo = new SqlServerCategoryRepository(context);

            // Act
            int actual = categoryRepo.GetAllCategories().Count();

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void GetAllCategories_ReturnsAListOfProductCategoriesOfLengthOne_WhenCalledAndTheDatabaseHasOneProductCategoryObject()
        {
            // Arrange
            int expected = 1;
            InsertData();
            SqlServerCategoryRepository categoryRepo = new SqlServerCategoryRepository(context);

            // Act
            int actual = categoryRepo.GetAllCategories().Count();

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void GetCategoryById_ReturnsAProductCategoriyOfIdValueOfOne_WhenCalledWithValidIdOfValueOnePresentInDatabase()
        {
            // Arrange
            int expected = 1;
            int id = 1;
            InsertData();
            SqlServerCategoryRepository categoryRepo = new SqlServerCategoryRepository(context);

            // Act
            int actual = categoryRepo.GetCategoryById(id).Id;

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void GetCategoryById_ReturnsADefaultProductCategoriyOfIdValueOfZero_WhenCalledWithValidIdOfValueOneButDatabaseIsEmpty()
        {
            // Arrange
            int expected = 0;
            int id = 1;
            SqlServerCategoryRepository categoryRepo = new SqlServerCategoryRepository(context);

            // Act
            int actual = categoryRepo.GetCategoryById(id).Id;

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void AddNewCategory_ReturnsAProductCategoriyOfIdValueOfOne_WhenCalledWithValidIdOfValueOnePresentInDatabase()
        {
            // Arrange
            int expected = 1;
            ProductCategory category = new ProductCategory();
            category.Id = 1;
            category.ProductCategoryName = "name";
            SqlServerCategoryRepository categoryRepo = new SqlServerCategoryRepository(context);

            // Act
            categoryRepo.AddNewCategory(category);
            int actual = context.ProductCategories.ToList().Count();

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void CheckCategoryName_ReturnsABoolOfValueFalse_WhenCalledAndProductCategoryNameIsNotInDatabase()
        {
            // Arrange
            bool expected = false;
            string name = "name";
            SqlServerCategoryRepository categoryRepo = new SqlServerCategoryRepository(context);

            // Act
            bool actual = categoryRepo.CheckCategoryName(name);

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void CheckCategoryName_ReturnsABoolOfValueTrue_WhenCalledAndProductCategoryNamePresentInDatabase()
        {
            // Arrange
            bool expected = true;
            string name = "name";
            ProductCategory category = new ProductCategory();
            category.Id = 1;
            category.ProductCategoryName = "name";
            context.ProductCategories.Add(category);
            context.SaveChanges();
            SqlServerCategoryRepository categoryRepo = new SqlServerCategoryRepository(context);

            // Act
            bool actual = categoryRepo.CheckCategoryName(name);

            // Assert
            Assert.AreEqual(expected, actual);
        }
    }
}