using ApparelShoppingAppAPI.Contexts;
using ApparelShoppingAppAPI.Models.DB_Models;
using ApparelShoppingAppAPI.Repositories.Classes;
using Microsoft.EntityFrameworkCore;

namespace ApparelShoppingAppAPITest.RepositoriesTest
{
    public class SellerRepositoryTests
    {
        private SellerRepository _sellerRepository;
        private ShoppingAppDbContext _context;

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<ShoppingAppDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDb")
                .Options;

            _context = new ShoppingAppDbContext(options);
            _sellerRepository = new SellerRepository(_context);
        }

        [TearDown]
        public void TearDown()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }

        [Test]
        public async Task Add_Seller_ReturnsAddedSeller()
        {
            // Arrange
            var seller = new Seller
            {
                SellerId = 1,
                Name = "Test",
                Email = "test@gmail.com",
                Phone = "1234567890"
            };

            // Act
            var result = await _sellerRepository.Add(seller);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(seller.SellerId, result.SellerId);
        }

        [Test]
        public async Task GetById_SellerExists_ReturnsSeller()
        {
            // Arrange
            var seller = new Seller
            {
                SellerId = 1,
                Name = "Test",
                Email = "test@gmail.com",
                Phone = "1234567890"
            };

            await _sellerRepository.Add(seller);

            // Act
            var result = await _sellerRepository.GetById(1);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(seller.SellerId, result.SellerId);
        }

        [Test]
        public async Task GetById_SellerDoesNotExist_ReturnsNull()
        {
            // Act
            var result = await _sellerRepository.GetById(1);

            // Assert
            Assert.IsNull(result);
        }

        [Test]
        public async Task GetAll_ReturnsAllSelleres()
        {
            // Arrange
            var seller1 = new Seller
            {
                SellerId = 1,
                Name = "Test",
                Email = "test@gmail.com",
                Phone = "1234567890"
            };
            var seller2 = new Seller
            {
                SellerId = 2,
                Name = "Test",
                Email = "test@gmail.com",
                Phone = "1234567890"
            };
            await _sellerRepository.Add(seller1);
            await _sellerRepository.Add(seller2);

            // Act
            var result = await _sellerRepository.GetAll();

            // Assert
            Assert.AreEqual(2, result.Count());
        }

        [Test]
        public async Task Update_Seller_ReturnsUpdatedSeller()
        {
            // Arrange
            var seller = new Seller
            {
                SellerId = 1,
                Name = "Test",
                Email = "test@gmail.com",
                Phone = "1234567890"
            };
            await _sellerRepository.Add(seller);

            seller.Name = "Updated Test";

            // Act
            var result = await _sellerRepository.Update(seller);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("Updated Test", result.Name);
        }

        [Test]
        public async Task Delete_SellerExists_ReturnsDeletedSeller()
        {
            // Arrange
            var seller = new Seller
            {
                SellerId = 1,
                Name = "Test",
                Email = "test@gmail.com",
                Phone = "1234567890"
            };
            await _sellerRepository.Add(seller);

            // Act
            var result = await _sellerRepository.Delete(1);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(seller.SellerId, result.SellerId);
        }

        [Test]
        public void Delete_SellerDoesNotExist_ThrowsInvalidOperationException()
        {
            // Act & Assert
            var ex = Assert.ThrowsAsync<InvalidOperationException>(() => _sellerRepository.Delete(1));
            Assert.AreEqual("1 not found.", ex.Message);
        }
    }
}
