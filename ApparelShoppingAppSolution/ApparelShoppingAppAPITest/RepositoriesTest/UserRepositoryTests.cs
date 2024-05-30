using ApparelShoppingAppAPI.Contexts;
using ApparelShoppingAppAPI.Models.DB_Models;
using ApparelShoppingAppAPI.Repositories.Classes;
using Microsoft.EntityFrameworkCore;
using System.Text;

namespace ApparelShoppingAppAPITest.RepositoriesTest
{
    public class UserRepositoryTests
    {
        private UserRepository _userRepository;
        private ShoppingAppDbContext _context;

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<ShoppingAppDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDb")
                .Options;

            _context = new ShoppingAppDbContext(options);
            _userRepository = new UserRepository(_context);
        }

        [TearDown]
        public void TearDown()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }

        [Test]
        public async Task Add_User_ReturnsAddedUser()
        {
            // Arrange
            var user = new User
            {
                UserId = 1,
                Role = "Test",
                Password = Encoding.UTF8.GetBytes("password"),
                PasswordHashKey = Encoding.UTF8.GetBytes("hashkey")
            };

            // Act
            var result = await _userRepository.Add(user);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(user.UserId, result.UserId);
        }

        [Test]
        public async Task GetById_UserExists_ReturnsUser()
        {
            // Arrange
            var user = new User
            {
                UserId = 1,
                Role = "Test",
                Password = Encoding.UTF8.GetBytes("password"),
                PasswordHashKey = Encoding.UTF8.GetBytes("hashkey")
            };

            await _userRepository.Add(user);

            // Act
            var result = await _userRepository.GetById(1);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(user.UserId, result.UserId);
        }

        [Test]
        public async Task GetById_UserDoesNotExist_ReturnsNull()
        {
            // Act
            var result = await _userRepository.GetById(1);

            // Assert
            Assert.IsNull(result);
        }

        [Test]
        public async Task GetAll_ReturnsAllUseres()
        {
            // Arrange
            var user1 = new User
            {
                UserId = 1,
                Role = "Test",
                Password = Encoding.UTF8.GetBytes("password"),
                PasswordHashKey = Encoding.UTF8.GetBytes("hashkey")
            };
            var user2 = new User
            {
                UserId = 2,
                Role = "Test",
                Password = Encoding.UTF8.GetBytes("password"),
                PasswordHashKey = Encoding.UTF8.GetBytes("hashkey")
            };
            await _userRepository.Add(user1);
            await _userRepository.Add(user2);

            // Act
            var result = await _userRepository.GetAll();

            // Assert
            Assert.AreEqual(2, result.Count());
        }

        [Test]
        public async Task Update_User_ReturnsUpdatedUser()
        {
            // Arrange
            var user = new User
            {
                UserId = 1,
                Role = "Test",
                Password = Encoding.UTF8.GetBytes("password"),
                PasswordHashKey = Encoding.UTF8.GetBytes("hashkey")
            };
            await _userRepository.Add(user);

            user.Role = "Updated Test";

            // Act
            var result = await _userRepository.Update(user);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("Updated Test", result.Role);
        }

        [Test]
        public async Task Delete_UserExists_ReturnsDeletedUser()
        {
            // Arrange
            var user = new User
            {
                UserId = 1,
                Role = "Test",
                Password = Encoding.UTF8.GetBytes("password"),
                PasswordHashKey = Encoding.UTF8.GetBytes("hashkey")
            };
            await _userRepository.Add(user);

            // Act
            var result = await _userRepository.Delete(1);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(user.UserId, result.UserId);
        }

        [Test]
        public void Delete_UserDoesNotExist_ThrowsInvalidOperationException()
        {
            // Act & Assert
            var ex = Assert.ThrowsAsync<InvalidOperationException>(() => _userRepository.Delete(1));
            Assert.AreEqual("1 not found.", ex.Message);
        }
    }
}
