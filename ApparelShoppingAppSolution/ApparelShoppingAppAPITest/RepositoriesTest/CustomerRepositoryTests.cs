using ApparelShoppingAppAPI.Contexts;
using ApparelShoppingAppAPI.Models.DB_Models;
using ApparelShoppingAppAPI.Repositories.Classes;
using Microsoft.EntityFrameworkCore;

namespace ApparelShoppingAppAPITest.RepositoriesTest
{
    public class CustomerRepositoryTests
    {
        private CustomerRepository _customerRepository;
        private ShoppingAppDbContext _context;

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<ShoppingAppDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDb")
                .Options;

            _context = new ShoppingAppDbContext(options);
            _customerRepository = new CustomerRepository(_context);
        }

        [TearDown]
        public void TearDown()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }

        [Test]
        public async Task Add_Customer_ReturnsAddedCustomer()
        {
            // Arrange
            var customer = new Customer
            {
                CustomerId = 1,
                Name = "Test",
                Email = "test@gmail.com",
                Phone = "1234567890"
            };

            // Act
            var result = await _customerRepository.Add(customer);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(customer.CustomerId, result.CustomerId);
        }

        [Test]
        public async Task GetById_CustomerExists_ReturnsCustomer()
        {
            // Arrange
            var customer = new Customer
            {
                CustomerId = 1,
                Name = "Test",
                Email = "test@gmail.com",
                Phone = "1234567890"
            };

            await _customerRepository.Add(customer);

            // Act
            var result = await _customerRepository.GetById(1);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(customer.CustomerId, result.CustomerId);
        }

        [Test]
        public async Task GetById_CustomerDoesNotExist_ReturnsNull()
        {
            // Act
            var result = await _customerRepository.GetById(1);

            // Assert
            Assert.IsNull(result);
        }

        [Test]
        public async Task GetAll_ReturnsAllCustomeres()
        {
            // Arrange
            var customer1 = new Customer
            {
                CustomerId = 1,
                Name = "Test",
                Email = "test@gmail.com",
                Phone = "1234567890"
            };
            var customer2 = new Customer
            {
                CustomerId = 2,
                Name = "Test",
                Email = "test@gmail.com",
                Phone = "1234567890"
            };
            await _customerRepository.Add(customer1);
            await _customerRepository.Add(customer2);

            // Act
            var result = await _customerRepository.GetAll();

            // Assert
            Assert.AreEqual(2, result.Count());
        }

        [Test]
        public async Task Update_Customer_ReturnsUpdatedCustomer()
        {
            // Arrange
            var customer = new Customer
            {
                CustomerId = 1,
                Name = "Test",
                Email = "test@gmail.com",
                Phone = "1234567890"
            };
            await _customerRepository.Add(customer);

            customer.Name = "Updated Test";

            // Act
            var result = await _customerRepository.Update(customer);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("Updated Test", result.Name);
        }

        [Test]
        public async Task Delete_CustomerExists_ReturnsDeletedCustomer()
        {
            // Arrange
            var customer = new Customer
            {
                CustomerId = 1,
                Name = "Test",
                Email = "test@gmail.com",
                Phone = "1234567890"
            };
            await _customerRepository.Add(customer);

            // Act
            var result = await _customerRepository.Delete(1);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(customer.CustomerId, result.CustomerId);
        }

        [Test]
        public void Delete_CustomerDoesNotExist_ThrowsInvalidOperationException()
        {
            // Act & Assert
            var ex = Assert.ThrowsAsync<InvalidOperationException>(() => _customerRepository.Delete(1));
            Assert.AreEqual("1 not found.", ex.Message);
        }
    }
}
