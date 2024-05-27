using Microsoft.EntityFrameworkCore;
using ApparelShoppingAppAPI.Contexts;
using ApparelShoppingAppAPI.Exceptions;
using ApparelShoppingAppAPI.Models.DB_Models;
using ApparelShoppingAppAPI.Repositories.Interfaces;

namespace ApparelShoppingAppAPI.Repositories.Classes
{
    public class CustomerRepository:BaseRepository<int, Customer>, ICustomerRepository
    {
        public CustomerRepository(ShoppingAppDbContext context) : base(context)
        {
        }

        #region GetCustomerById
        /// <summary>
        /// Get customer by id with cart and orders
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Customer</returns>
        /// <exception cref="CustomerNotFoundException">If Coustomer Not Found </exception>

        public override async Task<Customer> GetById(int id)
        {
            var customer = await _context.Customers
                .Include(c => c.Cart)
                .Include(c => c.Orders)
                .FirstOrDefaultAsync(c => c.CustomerId == id);
            return customer;
        }
        #endregion GetCustomerById

        #region GetAllCustomers
        /// <summary>
        /// Get all customers
        /// </summary>
        /// <returns>List of Customer</returns>

        public override async Task<IEnumerable<Customer>> GetAll()
        {
            var customers = await _context.Customers
                .Include(c => c.Cart)
                .Include(c => c.Orders)
                .ToListAsync();

            return customers;
        }
        #endregion GetAllCustomers

        #region GetCustomerByEmail
        /// <summary>
        /// Get customer by email
        /// </summary>
        /// <param name="email">Email</param>
        /// <returns></returns>
        /// <exception cref="CustomerNotFoundException">If Customer not found </exception>

        public async Task<Customer> GetCustomerByEmail(string email)
        {
            var customer = await _context.Customers
                .Include(c => c.Cart)
                .Include(c => c.Orders)
                .FirstOrDefaultAsync(c => c.Email == email);
            return customer;
        }
        #endregion GetCustomerByEmail
    }

}
