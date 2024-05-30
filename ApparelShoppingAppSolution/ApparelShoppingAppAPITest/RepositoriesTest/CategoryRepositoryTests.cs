using ApparelShoppingAppAPI.Contexts;
using ApparelShoppingAppAPI.Models.DB_Models;
using ApparelShoppingAppAPI.Repositories.Classes;
using Microsoft.EntityFrameworkCore;

namespace ApparelShoppingAppAPITest.RepositoriesTest
{
    public class CategoryRepositoryTests
    {
        private CategoryRepository _categoryRepository;
        private ShoppingAppDbContext _context;

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<ShoppingAppDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDb")
                .Options;

            _context = new ShoppingAppDbContext(options);
            _categoryRepository = new CategoryRepository(_context);
        }

        [TearDown]
        public void TearDown()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }

        [Test]
        public async Task Add_Category_ReturnsAddedCategory()
        {
            // Arrange
            var category = new Category
            {
                CategoryId = 1,
                Name = "Test",
            };

            // Act
            var result = await _categoryRepository.Add(category);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(category.CategoryId, result.CategoryId);
        }

        [Test]
        public async Task GetById_CategoryExists_ReturnsCategory()
        {
            // Arrange
            var category = new Category
            {
                CategoryId = 1,
                Name = "Test"
            };

            await _categoryRepository.Add(category);

            // Act
            var result = await _categoryRepository.GetById(1);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(category.CategoryId, result.CategoryId);
        }

        [Test]
        public async Task GetById_CategoryDoesNotExist_ReturnsNull()
        {
            // Act
            var result = await _categoryRepository.GetById(1);

            // Assert
            Assert.IsNull(result);
        }

        [Test]
        public async Task GetAll_ReturnsAllCategoryes()
        {
            // Arrange
            var category1 = new Category
            {
                CategoryId = 1,
                Name = "Test1"
            };
            var category2 = new Category
            {
                CategoryId = 2,
                Name = "Test2"
            };
            await _categoryRepository.Add(category1);
            await _categoryRepository.Add(category2);

            // Act
            var result = await _categoryRepository.GetAll();

            // Assert
            Assert.AreEqual(2, result.Count());
        }

        [Test]
        public async Task Update_Category_ReturnsUpdatedCategory()
        {
            // Arrange
            var category = new Category
            {
                CategoryId = 1,
                Name = "Test"
            };
            await _categoryRepository.Add(category);

            category.Name = "Updated Test";

            // Act
            var result = await _categoryRepository.Update(category);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("Updated Test", result.Name);
        }

        [Test]
        public async Task Delete_CategoryExists_ReturnsDeletedCategory()
        {
            // Arrange
            var category = new Category
            {
                CategoryId = 1,
                Name = "Test"
            };
            await _categoryRepository.Add(category);

            // Act
            var result = await _categoryRepository.Delete(1);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(category.CategoryId, result.CategoryId);
        }

        [Test]
        public void Delete_CategoryDoesNotExist_ThrowsInvalidOperationException()
        {
            // Act & Assert
            var ex = Assert.ThrowsAsync<InvalidOperationException>(() => _categoryRepository.Delete(1));
            Assert.AreEqual("1 not found.", ex.Message);
        }

    }
}
