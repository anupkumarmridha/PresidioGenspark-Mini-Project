using ApparelShoppingAppAPI.Contexts;
using ApparelShoppingAppAPI.Models.DB_Models;
using ApparelShoppingAppAPI.Repositories.Classes;
using Microsoft.EntityFrameworkCore;

namespace ApparelShoppingAppAPITest.RepositoriesTest
{
    public class AddressRepositoryTests
    {
        private AddressRepository _addressRepository;
        private ShoppingAppDbContext _context;

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<ShoppingAppDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDb")
                .Options;

            _context = new ShoppingAppDbContext(options);
            _addressRepository = new AddressRepository(_context);
        }

        [TearDown]
        public void TearDown()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }

        [Test]
        public async Task Add_Address_ReturnsAddedAddress()
        {
            // Arrange
            var address = new Address { 
                AddressId = 1, 
                CustomerId = 1, 
                PhoneNumber = "1234567890",
                Street = "123 Test St", 
                City = "Test City", 
                Country="Test Country",
                State = "Test State",
                ZipCode = "12345" 
            };

            // Act
            var result = await _addressRepository.Add(address);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(address.AddressId, result.AddressId);
        }

        [Test]
        public async Task GetById_AddressExists_ReturnsAddress()
        {
            // Arrange
            var address = new Address
            {
                AddressId = 1,
                CustomerId = 1,
                PhoneNumber = "1234567890",
                Street = "123 Test St",
                City = "Test City",
                Country = "Test Country",
                State = "Test State",
                ZipCode = "12345"
            };
            
            await _addressRepository.Add(address);

            // Act
            var result = await _addressRepository.GetById(1);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(address.AddressId, result.AddressId);
        }

        [Test]
        public async Task GetById_AddressDoesNotExist_ReturnsNull()
        {
            // Act
            var result = await _addressRepository.GetById(1);

            // Assert
            Assert.IsNull(result);
        }

        [Test]
        public async Task GetAll_ReturnsAllAddresses()
        {
            // Arrange
            var address1 = new Address
            {
                AddressId = 1,
                CustomerId = 1,
                PhoneNumber = "1234567890",
                Street = "123 Test St",
                City = "Test City",
                Country = "Test Country",
                State = "Test State",
                ZipCode = "12345"
            };
            var address2 = new Address
            {
                AddressId = 2,
                CustomerId = 2,
                PhoneNumber = "1234567890",
                Street = "123 Test St",
                City = "Test City",
                Country = "Test Country",
                State = "Test State",
                ZipCode = "12345"
            };
            await _addressRepository.Add(address1);
            await _addressRepository.Add(address2);

            // Act
            var result = await _addressRepository.GetAll();

            // Assert
            Assert.AreEqual(2, result.Count());
        }

        [Test]
        public async Task Update_Address_ReturnsUpdatedAddress()
        {
            // Arrange
            var address = new Address
            {
                AddressId = 1,
                CustomerId = 1,
                PhoneNumber = "1234567890",
                Street = "123 Test St",
                City = "Test City",
                Country = "Test Country",
                State = "Test State",
                ZipCode = "12345"
            };
            await _addressRepository.Add(address);

            address.City = "Updated City";

            // Act
            var result = await _addressRepository.Update(address);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("Updated City", result.City);
        }

        [Test]
        public async Task Delete_AddressExists_ReturnsDeletedAddress()
        {
            // Arrange
            var address = new Address
            {
                AddressId = 1,
                CustomerId = 1,
                PhoneNumber = "1234567890",
                Street = "123 Test St",
                City = "Test City",
                Country = "Test Country",
                State = "Test State",
                ZipCode = "12345"
            };
            await _addressRepository.Add(address);

            // Act
            var result = await _addressRepository.Delete(1);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(address.AddressId, result.AddressId);
        }

        [Test]
        public void Delete_AddressDoesNotExist_ThrowsInvalidOperationException()
        {
            // Act & Assert
            var ex = Assert.ThrowsAsync<InvalidOperationException>(() => _addressRepository.Delete(1));
            Assert.AreEqual("1 not found.", ex.Message);
        }

        [Test]
        public async Task GetAllAddressByCustomerId_CustomerHasAddresses_ReturnsAddresses()
        {
            // Arrange
            var customerId = 1;
            var address1 = new Address
            {
                AddressId = 1,
                CustomerId = customerId,
                PhoneNumber = "1234567890",
                Street = "123 Test St",
                City = "Test City",
                Country = "Test Country",
                State = "Test State",
                ZipCode = "12345"
            };
            var address2 = new Address
            {
                AddressId = 2,
                CustomerId = customerId,
                PhoneNumber = "1234567890",
                Street = "123 Test St",
                City = "Test City",
                Country = "Test Country",
                State = "Test State",
                ZipCode = "12345"
            };
            await _addressRepository.Add(address1);
            await _addressRepository.Add(address2);

            // Act
            var result = await _addressRepository.GetAllAddressByCustomerId(customerId);

            // Assert
            Assert.AreEqual(2, result.Count());
            Assert.IsTrue(result.All(a => a.CustomerId == customerId));
        }

        [Test]
        public async Task GetAllAddressByCustomerId_CustomerHasNoAddresses_ReturnsEmpty()
        {
            // Act
            var result = await _addressRepository.GetAllAddressByCustomerId(1);

            // Assert
            Assert.IsEmpty(result);
        }
    }
}
