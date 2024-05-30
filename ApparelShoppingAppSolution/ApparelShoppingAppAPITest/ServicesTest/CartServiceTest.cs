using ApparelShoppingAppAPI.Exceptions;
using ApparelShoppingAppAPI.Models.DB_Models;
using ApparelShoppingAppAPI.Repositories.Interfaces;
using ApparelShoppingAppAPI.Services.Classes;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApparelShoppingAppAPITest.ServicesTest
{
    public class CartServiceTests
    {
        private Mock<ICartRepository> _cartRepositoryMock;
        private Mock<IProductRepository> _productRepositoryMock;
        private CartService _cartService;

        [SetUp]
        public void Setup()
        {
            _cartRepositoryMock = new Mock<ICartRepository>();
            _productRepositoryMock = new Mock<IProductRepository>();
            _cartService = new CartService(_cartRepositoryMock.Object, _productRepositoryMock.Object);
        }

        [Test]
        public async Task AddOrUpdateProductToCart_ProductNotFound_ThrowsProductNotFoundException()
        {
            // Arrange
            var userId = 1;
            var productId = 1;
            var quantity = 1;
            _productRepositoryMock.Setup(repo => repo.GetById(productId)).ReturnsAsync((Product)null);

            // Act & Assert
            var ex = Assert.ThrowsAsync<ProductNotFoundException>(() => _cartService.AddOrUpdateProductToCart(userId, productId, quantity));
            Assert.AreEqual("Product not found or invalid price", ex.Message);
        }

        [Test]
        public async Task AddOrUpdateProductToCart_InsufficientQuantity_ThrowsInsufficientProductQuantityException()
        {
            // Arrange
            var userId = 1;
            var productId = 1;
            var quantity = 10;
            var product = new Product { ProductId = productId, Quantity = 5, Price = 100 };
            _productRepositoryMock.Setup(repo => repo.GetById(productId)).ReturnsAsync(product);

            // Act & Assert
            var ex = Assert.ThrowsAsync<InsufficientProductQuantityException>(() => _cartService.AddOrUpdateProductToCart(userId, productId, quantity));
            Assert.AreEqual("Insufficient product quantity. Available Quantity = 5", ex.Message);
        }

        [Test]
        public async Task AddOrUpdateProductToCart_NewProduct_AddsToCart()
        {
            // Arrange
            var userId = 1;
            var productId = 1;
            var quantity = 2;
            var product = new Product { ProductId = productId, Quantity = 10, Price = 50 };
            var cart = new Cart { CustomerId = userId, Items = new List<CartItem>() };

            _productRepositoryMock.Setup(repo => repo.GetById(productId)).ReturnsAsync(product);
            _cartRepositoryMock.Setup(repo => repo.GetCartByUserId(userId)).ReturnsAsync(cart);

            // Act
            var updatedCart = await _cartService.AddOrUpdateProductToCart(userId, productId, quantity);

            // Assert
            Assert.AreEqual(1, updatedCart.Items.Count);
            Assert.AreEqual(productId, updatedCart.Items.First().ProductId);
            Assert.AreEqual(quantity, updatedCart.Items.First().Quantity);
            Assert.AreEqual(100, updatedCart.Items.First().Price);
            Assert.AreEqual(100, updatedCart.TotalPrice);
        }

        [Test]
        public async Task AddOrUpdateProductToCart_ExistingProduct_UpdatesQuantity()
        {
            // Arrange
            var userId = 1;
            var productId = 1;
            var quantity = 2;
            var product = new Product { ProductId = productId, Quantity = 10, Price = 50 };
            var cartItem = new CartItem { ProductId = productId, Quantity = 1, Price = 50 };
            var cart = new Cart { CustomerId = userId, Items = new List<CartItem> { cartItem } };

            _productRepositoryMock.Setup(repo => repo.GetById(productId)).ReturnsAsync(product);
            _cartRepositoryMock.Setup(repo => repo.GetCartByUserId(userId)).ReturnsAsync(cart);

            // Act
            var updatedCart = await _cartService.AddOrUpdateProductToCart(userId, productId, quantity);

            // Assert
            Assert.AreEqual(1, updatedCart.Items.Count);
            Assert.AreEqual(productId, updatedCart.Items.First().ProductId);
            Assert.AreEqual(3, updatedCart.Items.First().Quantity); // 1 + 2
            Assert.AreEqual(150, updatedCart.Items.First().Price); // 3 * 50
            Assert.AreEqual(150, updatedCart.TotalPrice);
        }

        [Test]
        public async Task RemoveProductFromCart_ProductNotFoundInCart_ThrowsProductNotFoundException()
        {
            // Arrange
            var userId = 1;
            var productId = 1;
            var cart = new Cart { CustomerId = userId, Items = new List<CartItem>() };

            _cartRepositoryMock.Setup(repo => repo.GetCartByUserId(userId)).ReturnsAsync(cart);

            // Act & Assert
            var ex = Assert.ThrowsAsync<ProductNotFoundException>(() => _cartService.RemoveProductFromCart(userId, productId));
            Assert.AreEqual("Product not found in cart", ex.Message);
        }

        [Test]
        public async Task RemoveProductFromCart_ProductExists_RemovesProductFromCart()
        {
            // Arrange
            var userId = 1;
            var productId = 1;
            var cartItem = new CartItem { ProductId = productId, Quantity = 1, Price = 50 };
            var cart = new Cart { CustomerId = userId, Items = new List<CartItem> { cartItem } };

            _cartRepositoryMock.Setup(repo => repo.GetCartByUserId(userId)).ReturnsAsync(cart);

            // Act
            var updatedCart = await _cartService.RemoveProductFromCart(userId, productId);

            // Assert
            Assert.AreEqual(0, updatedCart.Items.Count);
            Assert.AreEqual(0, updatedCart.TotalPrice);
        }

        [Test]
        public async Task GetCartByUserId_CartNotFound_ThrowsCartNotFoundException()
        {
            // Arrange
            var userId = 1;
            _cartRepositoryMock.Setup(repo => repo.GetCartByUserId(userId)).ThrowsAsync(new CartNotFoundException("Cart Not Found"));
            // Act & Assert
            var ex = Assert.ThrowsAsync<CartNotFoundException>(() => _cartService.GetCartByUserId(userId));
            Assert.AreEqual("Cart Not Found", ex.Message);
        }

        [Test]
        public async Task GetCartByUserId_CartExists_ReturnsCart()
        {
            // Arrange
            var userId = 1;
            var cart = new Cart { CustomerId = userId, Items = new List<CartItem>() };
            _cartRepositoryMock.Setup(repo => repo.GetCartByUserId(userId)).ReturnsAsync(cart);

            // Act
            var result = await _cartService.GetCartByUserId(userId);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(userId, result.CustomerId);
        }
    }
}
