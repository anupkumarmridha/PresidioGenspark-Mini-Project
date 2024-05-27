using ApparelShoppingAppAPI.Contexts;
using ApparelShoppingAppAPI.Exceptions;
using ApparelShoppingAppAPI.Models.DB_Models;
using ApparelShoppingAppAPI.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ApparelShoppingAppAPI.Repositories.Classes
{
    public class SellerRepository: BaseRepository<int, Seller>, ISellerRepository
    {
        public SellerRepository(ShoppingAppDbContext context) : base(context)
        {
        }

        #region GetSellerByEmail
        /// <summary>
        /// Get seller by email
        /// </summary>
        /// <param name="email"></param>
        /// <returns>Seller</returns>
        /// <exception cref="SellerNotFoundException">If Seller Not Found</exception>
        public async Task<Seller> GetSellerByEmail(string email)
        {
            var seller = await _context.Sellers
                    .Include(c => c.Products)
                    .FirstOrDefaultAsync(c => c.Email == email);
            return seller;
        }
        #endregion GetSellerByEmail
    }
}
