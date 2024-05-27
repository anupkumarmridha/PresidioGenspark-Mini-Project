using ApparelShoppingAppAPI.Models.DB_Models;
using ApparelShoppingAppAPI.Repositories.Interfaces;
using ApparelShoppingAppAPI.Contexts;
using Microsoft.EntityFrameworkCore;
using ApparelShoppingAppAPI.Exceptions;

namespace ApparelShoppingAppAPI.Repositories.Classes
{
    public class CartRepository : BaseRepository<int, Cart>, ICartRepository
    {
        public CartRepository(ShoppingAppDbContext context):base(context)
        {
        }

        #region GetCartByUserId
        /// <summary>
        /// Get cart by user id
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        /// <exception cref="CartNotFoundException"></exception>
        public async Task<Cart> GetCartByUserId(int userId)
        {
            var cart = await _context.Carts.Include(c => c.Items)
                .ThenInclude(i => i.Product)
                .FirstOrDefaultAsync(c => c.CustomerId == userId);
            if (cart == null)
            {
                throw new CartNotFoundException("Cart Not Found");
            }
            return cart;
        }
        #endregion GetCartByUserId

    }
}
