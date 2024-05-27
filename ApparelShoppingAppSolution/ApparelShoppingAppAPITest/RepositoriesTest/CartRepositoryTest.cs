using ApparelShoppingAppAPI.Contexts;
using ApparelShoppingAppAPI.Exceptions;
using ApparelShoppingAppAPI.Models.DB_Models;
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
    public class CartRepositoryTest
    {
        private ShoppingAppDbContext _context;
        private ICartRepository _cartRepository;

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<ShoppingAppDbContext>()
                .UseInMemoryDatabase("DummyDB")
                .Options;
            _context = new ShoppingAppDbContext(options);
            _cartRepository = new CartRepository(_context);
        }
        [Test]
        public async Task GetCartByUserId()
        {
            var cart = new Cart
            {
                CustomerId = 1,
                Items = new List<CartItem>
                {
                    new CartItem
                    {
                        ProductId = 1,
                        Quantity = 1
                    }
                }
            };
            _context.Carts.Add(cart);
            _context.SaveChanges();

            var result = await _cartRepository.GetCartByUserId(1);

            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.CustomerId);
            Assert.AreEqual(1, result.Items.Count);
        }
        [Test]
        public async Task GetCartByUserId_CartNotFound()
        {
            int userId = 2;
            var result = _cartRepository.GetCartByUserId(userId);
            Assert.IsNotNull(result);
        }

        [Test]
        public async Task GetCartByUserId_CartNotFoundException()
        {
            int userId = 2;
            var ex = Assert.ThrowsAsync<CartNotFoundException>(() => _cartRepository.GetCartByUserId(userId));
            Assert.AreEqual("Cart Not Found", ex.Message);
        }
        [Test]
        public async Task DeleteCart()
        {
            var cart = new Cart
            {
                CustomerId = 1,
                Items = new List<CartItem>
                {
                    new CartItem
                    {
                        ProductId = 1,
                        Quantity = 1
                    }
                }
            };
            await _cartRepository.Add(cart);

            await _cartRepository.Delete(cart.CartId);
            var ex = Assert.ThrowsAsync<CartNotFoundException>(() => _cartRepository.GetCartByUserId(1));
            Assert.AreEqual("Cart Not Found", ex.Message);
        }
    }
}
