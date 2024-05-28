using ApparelShoppingAppAPI.Contexts;
using ApparelShoppingAppAPI.Models.DB_Models;
using ApparelShoppingAppAPI.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ApparelShoppingAppAPI.Repositories.Classes
{
    public class AddressRepository:BaseRepository<int, Address>,IAddressRepository
    {
        public AddressRepository(ShoppingAppDbContext context) : base(context)
        {
        }
        #region GetAllAddressByCustomerId
        /// <summary>
        /// Get all address by customer id
        /// </summary>
        /// <param name="customerId"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public async Task<IEnumerable<Address>> GetAllAddressByCustomerId(int customerId)
        {
            return await _context.Addresses.Where(a => a.CustomerId == customerId).ToListAsync();
        }
        #endregion GetAllAddressByCustomerId
    }
}
