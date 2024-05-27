using ApparelShoppingAppAPI.Contexts;
using ApparelShoppingAppAPI.Exceptions;
using ApparelShoppingAppAPI.Models.DB_Models;
using ApparelShoppingAppAPI.Models.DTO_Models;
using ApparelShoppingAppAPI.Repositories.Classes;
using ApparelShoppingAppAPI.Repositories.Interfaces;
using ApparelShoppingAppAPI.Services.Classes;
using ApparelShoppingAppAPI.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApparelShoppingAppAPITest.ServicesTest
{
    public class CartServiceTest
    {
        private ShoppingAppDbContext _context;
        private ICartService _cartService;
        private ICartRepository _cartRepository;
        private IProductRepository _productRepository;

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<ShoppingAppDbContext>()
                .UseInMemoryDatabase("TestDatabase")
                .Options;

            _context = new ShoppingAppDbContext(options);
            _cartRepository = new CartRepository(_context);
            _productRepository = new ProductRepository(_context); // Assuming you have a ProductRepository
            _cartService = new CartService(_cartRepository, _productRepository);
        }

        [Test]
        public async Task AddOrUpdateProductToCart_AddNewProduct()
        {
            var product = new Product { ProductId = 1, Name = "Product1", Price = 100 };
            await _context.Products.AddAsync(product);
            await _context.SaveChangesAsync();

            var cart = await _cartService.AddOrUpdateProductToCart(1, 1, 1);

            Assert.IsNotNull(cart);
            Assert.AreEqual(1, cart.Items.Count); 
            var cartItem = cart.Items.FirstOrDefault(); 
            Assert.IsNotNull(cartItem); 
            Assert.AreEqual(1, cartItem.ProductId); 
            Assert.AreEqual(1, cartItem.Quantity);
        }

        [Test]
        public async Task AddOrUpdateProductToCart_UpdateExistingProduct()
        {
            var product = new Product { ProductId = 1, Name = "Product1", Price = 100 };
            await _context.Products.AddAsync(product);
            await _context.SaveChangesAsync();

            var cart = await _cartService.AddOrUpdateProductToCart(1, 1, 1);
            cart = await _cartService.AddOrUpdateProductToCart(1, 1, 1);

            Assert.IsNotNull(cart);
            Assert.AreEqual(1, cart.Items.Count);
            var cartItem = cart.Items.FirstOrDefault();
            Assert.IsNotNull(cartItem);
            Assert.AreEqual(1, cartItem.ProductId);
            Assert.AreEqual(2, cartItem.Quantity);
        }

        [Test]
        public void AddOrUpdateProductToCart_ProductNotFound()
        {
            Assert.ThrowsAsync<ProductNotFoundException>(async () =>
            {
                await _cartService.AddOrUpdateProductToCart(1, 999, 1);
            });
        }

        [Test]
        public async Task RemoveProductFromCart_RemoveExistingProduct()
        {
            var product = new Product { ProductId = 1, Name = "Product1", Price = 100 };
            await _context.Products.AddAsync(product);
            await _context.SaveChangesAsync();

            var cart = await _cartService.AddOrUpdateProductToCart(1, 1, 1);
            cart = await _cartService.RemoveProductFromCart(1, 1);

            Assert.IsNotNull(cart);
            Assert.AreEqual(0, cart.Items.Count);
        }

        [Test]
        public void RemoveProductFromCart_ProductNotFoundInCart()
        {
            Assert.ThrowsAsync<ProductNotFoundException>(async () =>
            {
                var product = new Product { ProductId = 1, Name = "Product1", Price = 100 };
                await _context.Products.AddAsync(product);
                await _context.SaveChangesAsync();

                await _cartService.RemoveProductFromCart(1, 1);
            });
        }

        [Test]
        public void RemoveProductFromCart_CartNotFound()
        {
            Assert.ThrowsAsync<CartNotFoundException>(async () =>
            {
                await _cartService.RemoveProductFromCart(1, 1);
            });
        }

        [Test]
        public async Task GetCartByUserId_CartExists()
        {
            var cart = new Cart
            {
                CustomerId = 1,
                Items = new List<CartItem>
                {
                    new CartItem { ProductId = 1, Quantity = 1, Price = 100 }
                }
            };
            await _context.Carts.AddAsync(cart);
            await _context.SaveChangesAsync();

            var result = await _cartService.GetCartByUserId(1);

            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.CustomerId);
        }

        [Test]
        public void GetCartByUserId_CartNotFound()
        {
            Assert.ThrowsAsync<CartNotFoundException>(async () =>
            {
                await _cartService.GetCartByUserId(1);
            });
        }
    }
}
