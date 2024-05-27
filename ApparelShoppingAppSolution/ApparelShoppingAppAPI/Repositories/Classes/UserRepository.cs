using Microsoft.EntityFrameworkCore;
using ApparelShoppingAppAPI.Contexts;
using ApparelShoppingAppAPI.Exceptions;
using ApparelShoppingAppAPI.Models.DB_Models;
using ApparelShoppingAppAPI.Repositories.Interfaces;

namespace ApparelShoppingAppAPI.Repositories.Classes
{
    public class UserRepository: BaseRepository<int, User>, IUserRepository
    {
        public UserRepository(ShoppingAppDbContext context) : base(context)
        {
        }

        #region GetCustomerUserByEmail
        /// <summary>
        /// Get customer user by email
        /// </summary>
        /// <param name="email"></param>
        /// <returns>User, Customer</returns>
        public async Task<User> GetCustomerUserByEmail(string email)
        {
            var user = await _context.Users
                 .Include(u => u.Customer)
                 .FirstOrDefaultAsync(u => u.Customer.Email == email);
            return user;
        }
        #endregion GetCustomerUserByEmail

        #region GetSellerUserByEmail
        /// <summary>
        /// Get seller user by email
        /// </summary>
        /// <param name="email"></param>
        /// <returns>User, Seller</returns>
        public async Task<User> GetSellerUserByEmail(string email)
        {
            var user = await _context.Users
                 .Include(u => u.Seller)
                 .FirstOrDefaultAsync(u => u.Seller.Email == email);
            return user;
        }
        #endregion GetSellerUserByEmail
    }
}
