using ApparelShoppingAppAPI.Contexts;
using ApparelShoppingAppAPI.Models.DTO_Models;
using ApparelShoppingAppAPI.Repositories.Classes;
using ApparelShoppingAppAPI.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Text;

namespace ApparelShoppingAppAPITest.RepositoriesTest
{
    public class Tests
    {
        private ShoppingAppDbContext _context;
        private IUserRegisterRepository _userRegisterRepository;

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<ShoppingAppDbContext>()
                .UseInMemoryDatabase("DummyDB")
                .Options;
            _context = new ShoppingAppDbContext(options);
            _userRegisterRepository = new UserRegisterRepository(_context);
        }

        //[Test]
        //public async Task CustomerUserRegister()
        //{
        //    var userRegisterDTO = new UserRegisterRepositoryDTO
        //    {
        //        Name = "Test Customer",
        //        Email = "customer@test.com",
        //        Phone = "1234567890",
        //        Password = Encoding.UTF8.GetBytes("password"),
        //        PasswordHashKey = Encoding.UTF8.GetBytes("hashkey")
        //    };

        //    var (customer, user) = await _userRegisterRepository.AddCustomerUserWithTransaction(userRegisterDTO);

        //    Assert.IsNotNull(customer);
        //    Assert.IsNotNull(user);
        //    Assert.AreEqual("Customer", user.Role);
        //    Assert.AreEqual(user.UserId, customer.CustomerId);
        //}

        //[Test]
        //public async Task SellerUserRegister()
        //{
        //    var userRegisterDTO = new UserRegisterRepositoryDTO
        //    {
        //        Name = "Test Seller",
        //        Email = "seller@test.com",
        //        Phone = "0987654321",
        //        Password = Encoding.UTF8.GetBytes("password"),
        //        PasswordHashKey = Encoding.UTF8.GetBytes("hashkey")
        //    };

        //    var (seller, user) = await _userRegisterRepository.AddSellerUserWithTransaction(userRegisterDTO);

        //    Assert.IsNotNull(seller);
        //    Assert.IsNotNull(user);
        //    Assert.AreEqual("Seller", user.Role);
        //    Assert.AreEqual(user.UserId, seller.SellerId);
        //}
    }
}