using ApparelShoppingAppAPI.Contexts;
using ApparelShoppingAppAPI.Exceptions;
using ApparelShoppingAppAPI.Models.DB_Models;
using ApparelShoppingAppAPI.Models.DTO_Models;
using ApparelShoppingAppAPI.Repositories.Classes;
using ApparelShoppingAppAPI.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApparelShoppingAppAPITest.RepositoriesTest
{
    public class OrderRepositoryTest
    {
        private ShoppingAppDbContext _context;
        private IOrderRepository _orderRepository;
        private IProductRepository _productRepository;
        private ICartRepository _cartRepository;
        private IAddressRepository _addressRepository;
        private IUserRegisterRepository _userRegisterRepository;
        private ICategoryRepository _categoryRepository;
        private Seller _seller;
        private Customer _customer;
        private Category _category;
        private Address _address;
        private Product _product1;
        private Product _product2;
        private Cart _cart;

        #region Setup
        [SetUp]
        public async Task Setup()
        {
            var options = new DbContextOptionsBuilder<ShoppingAppDbContext>()
                .UseSqlite("DataSource=:memory:")
                .Options;

            _context = new ShoppingAppDbContext(options);
            _context.Database.OpenConnection();
            _context.Database.EnsureCreated();

            _productRepository = new ProductRepository(_context);
            _orderRepository = new OrderRepository(_context,_productRepository);
            _productRepository = new ProductRepository(_context);
            _cartRepository = new CartRepository(_context);
            _addressRepository = new AddressRepository(_context);
            _userRegisterRepository = new UserRegisterRepository(_context);
            _categoryRepository = new CategoryRepository(_context);

            #region Register Seller
            // Register Seller and User
            var userRegisterDTO = new UserRegisterRepositoryDTO
            {
                Name = "Test Seller",
                Email = "seller@test.com",
                Phone = "0987654321",
                Password = Encoding.UTF8.GetBytes("password"),
                PasswordHashKey = Encoding.UTF8.GetBytes("hashkey")
            };
            var (seller, user) = await _userRegisterRepository.AddSellerUserWithTransaction(userRegisterDTO);
            _seller = seller;
            #endregion Register Seller

            #region Register Customer
            // Register Customer
            var customerUserDTO = new UserRegisterRepositoryDTO
            {
                Name = "Test Customer",
                Email = "customer@test.com",
                Phone = "1234567890",
                Password = Encoding.UTF8.GetBytes("password"),
                PasswordHashKey = Encoding.UTF8.GetBytes("hashkey")
            };
            var (customer, customerUser) = await _userRegisterRepository.AddCustomerUserWithTransaction(customerUserDTO);
            _customer = customer;
            #endregion Register Customer

            #region Add Category
            // Add Category
            var category = new Category
            {
                Name = "Test Category",
            };
            _category = await _categoryRepository.Add(category);

            #endregion Add Category

            #region Add Address
            // Add Address
            var address = new Address
            {
                AddressId = 1,
                CustomerId = _customer.CustomerId,
                PhoneNumber = "1234567890",
                Street = "123 Test St",
                City = "Test City",
                Country = "Test Country",
                State = "Test State",
                ZipCode = "12345"
            };
            _address = await _addressRepository.Add(address);

            #endregion Add Address

            #region Add Products
            var product1 = new Product
            {
                Name = "Sample Product 1",
                Description = "This is a test product description.",
                Quantity = 100,
                Price = 500.99m,
                CategoryId = _category.CategoryId,
                SellerId = _seller.SellerId,
                CreationDate = DateTime.Now,
                LastUpdatedDate = DateTime.Now
            };

            // Act
            _product1 = await _productRepository.Add(product1);
            
            var product2 = new Product
            {
                Name = "Sample Product 2",
                Description = "This is a test product description.",
                Quantity = 140,
                Price = 299.99m,
                CategoryId = _category.CategoryId,
                SellerId = _seller.SellerId,
                CreationDate = DateTime.Now,
                LastUpdatedDate = DateTime.Now
            };

            // Act
            _product2 = await _productRepository.Add(product2);

            #endregion Add Products

            #region Add Cart
            // Add Cart
            var cart = new Cart
            {
                CustomerId = _customer.CustomerId,
                Items = new List<CartItem>
                {
                    new CartItem
                    {
                        ProductId = _product1.ProductId,
                        Quantity = 40
                    }
                }
            };
            _cart = await _cartRepository.Add(cart);
            #endregion Add Cart
        }

        [TearDown]
        public void TearDown()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }
        #endregion Setup

        #region Test Cases

        [Test]
        public async Task AddOrderWithTransaction_ValidOrder_ReturnsOrder()
        {
            // Arrange
            var order = new Order
            {
                CustomerId = _customer.CustomerId,
                AddressId = _address.AddressId,
                TotalPrice = _product1.Price,
                OrderStatus = "Active",
                OrderDetails = new List<OrderDetails>
                {
                    new OrderDetails
                    {
                        ProductId = _product1.ProductId,
                        Quantity = 10,
                        SubtotalPrice = _product1.Price
                    }
                }
            };

            // Act
            var result = await _orderRepository.AddOrderWithTransaction(order, _product1, 10);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(order.CustomerId, result.CustomerId);
            Assert.AreEqual(order.TotalPrice, result.TotalPrice);
            Assert.AreEqual(order.OrderStatus, result.OrderStatus);
        }

        [Test]
        public async Task GetAllOrdersByCustomer_ValidCustomerId_ReturnsOrders()
        {
            // Arrange
            var order = new Order
            {
                CustomerId = _customer.CustomerId,
                AddressId = _address.AddressId,
                TotalPrice = 100,
                OrderStatus = "Active"
            };
            await _orderRepository.Add(order);

            // Act
            var result = await _orderRepository.GetAllOrdersByCustomer(_customer.CustomerId);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.Count());
        }

        [Test]
        public async Task CancelOrder_ValidOrderId_ReturnsCanceledOrder()
        {
            // Arrange
            var order = new Order
            {
                CustomerId = _customer.CustomerId,
                AddressId = _address.AddressId,
                TotalPrice = 100,
                OrderStatus = "Active"
            };
            var addedOrder = await _orderRepository.Add(order);

            // Act
            var result = await _orderRepository.CancelOrder(addedOrder.OrderId);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("Canceled", result.OrderStatus);
            Assert.IsTrue(result.IsCanceled);
        }

        [Test]
        public async Task GetAllCancelOrdersByCustomer_ValidCustomerId_ReturnsCanceledOrders()
        {
            // Arrange
            var order = new Order
            {
                CustomerId = _customer.CustomerId,
                AddressId = _address.AddressId,
                TotalPrice = _product1.Price,
                OrderStatus = "Active",
                OrderDetails = new List<OrderDetails>
                {
                    new OrderDetails
                    {
                        ProductId = _product1.ProductId,
                        Quantity = 10,
                        SubtotalPrice = _product1.Price
                    }
                }
            };

            // Act
            var addedOrder = await _orderRepository.AddOrderWithTransaction(order, _product1, 10);

            await _orderRepository.CancelOrder(addedOrder.OrderId);
            // Act
            var result = await _orderRepository.GetAllCancelOrdersByCustomer(_customer.CustomerId);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.Count());
        }

        [Test]
        public async Task GetAllActiveOrdersByCustomer_ValidCustomerId_ReturnsActiveOrders()
        {
            // Arrange
            var order = new Order
            {
                CustomerId = _customer.CustomerId,
                AddressId = _address.AddressId,
                TotalPrice = 100,
                OrderStatus = "Active",
                IsCanceled = false
            };
            await _orderRepository.Add(order);

            // Act
            var result = await _orderRepository.GetAllActiveOrdersByCustomer(_customer.CustomerId);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.Count());
        }

        [Test]
        public async Task GetAllOrdersBySeller_ValidSellerId_ReturnsOrders()
        {
            // Arrange
           
            var order = new Order
            {
                CustomerId = _customer.CustomerId,
                AddressId = _address.AddressId,
                TotalPrice = 100,
                OrderStatus = "Active",
                OrderDetails = new List<OrderDetails>
                {
                    new OrderDetails
                    {
                        ProductId = _product1.ProductId,
                        Quantity = 1,
                        SubtotalPrice = 100
                    }
                }
            };
            await _orderRepository.Add(order);

            // Act
            var result = await _orderRepository.GetAllOrdersBySeller(_seller.SellerId);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.Count());
        }

        [Test]
        public async Task CheckOutCartWithTransaction_ValidCart_ReturnsOrder()
        {


            var result = await _orderRepository.CheckOutCartWithTransaction(_cart, _address);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(_customer.CustomerId, result.CustomerId);
            Assert.AreEqual(_address.AddressId, result.AddressId);
            Assert.AreEqual(_product1.Price * 40, result.TotalPrice);
            Assert.AreEqual("Active", result.OrderStatus);
            Assert.AreEqual(1, result.OrderDetails.Count());

        }

        [Test]
        public async Task CheckOutCartWithTransaction_InsufficientProductQuantity_ThrowsException()
        {
            // Arrange
            var cart = new Cart
            {
                CustomerId = _customer.CustomerId,
                Items = new List<CartItem>
                {
                    new CartItem
                    {
                        ProductId = _product1.ProductId,
                        Quantity = 110
                    }
                }
            };
           var addedCart= await _cartRepository.Add(cart);

            // Act
            var ex = Assert.ThrowsAsync<InsufficientProductQuantityException>(() => _orderRepository.CheckOutCartWithTransaction(addedCart, _address));
            // Assert
            Assert.AreEqual($"Insufficient product quantity or product not found for Product ID {_product1.ProductId}", ex.Message);
        }

        #endregion Test Cases
    }
}

