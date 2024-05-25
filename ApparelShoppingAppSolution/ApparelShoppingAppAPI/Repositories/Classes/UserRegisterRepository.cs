using ApparelShoppingAppAPI.Contexts;
using ApparelShoppingAppAPI.Models.DB_Models;
using ApparelShoppingAppAPI.Models.DTO_Models;
using ApparelShoppingAppAPI.Repositories.Interfaces;

namespace ApparelShoppingAppAPI.Repositories.Classes
{
    public class UserRegisterRepository : IUserRegisterRepository
    {
        private readonly ShoppingAppDbContext _context;

        public UserRegisterRepository(ShoppingAppDbContext context)
        {
            _context = context;
        }
        public async Task<(Customer customer, User user)> AddCustomerUserWithTransaction(UserRegisterRepositoryDTO userRegisterDTO)
        {
            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                   var newUser = new User
                   {
                       Role = "Customer",
                       Password = userRegisterDTO.Password,
                       PasswordHashKey = userRegisterDTO.PasswordHashKey
                   };
                    _context.Users.Add(newUser);
                    await _context.SaveChangesAsync();

                    var newCustomer = new Customer
                    {
                        CustomerId = newUser.UserId,
                        Name = userRegisterDTO.Name,
                        Email = userRegisterDTO.Email,
                        Phone = userRegisterDTO.Phone
                    };
                    await _context.Customers.AddAsync(newCustomer);
                    await _context.SaveChangesAsync();

                    await transaction.CommitAsync();

                    return (newCustomer, newUser);
                }
                catch (Exception)
                {
                    await transaction.RollbackAsync();
                    throw new Exception("Error while adding user");
                }
            }
        }

        public async Task<(Seller seller, User user)> AddSellerUserWithTransaction(UserRegisterRepositoryDTO userRegisterDTO)
        {
            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    var newUser = new User
                    {
                        Role = "Seller",
                        Password = userRegisterDTO.Password,
                        PasswordHashKey = userRegisterDTO.PasswordHashKey
                    };
                    _context.Users.Add(newUser);
                    await _context.SaveChangesAsync();

                    var newSeller = new Seller
                    {
                        SellerId = newUser.UserId,
                        Name = userRegisterDTO.Name,
                        Email = userRegisterDTO.Email,
                        Phone = userRegisterDTO.Phone
                    };
                    await _context.Sellers.AddAsync(newSeller);
                    await _context.SaveChangesAsync();

                    await transaction.CommitAsync();

                    return (newSeller, newUser);
                }
                catch (Exception)
                {
                    await transaction.RollbackAsync();
                    throw new Exception("Error while adding user");
                }
            }
        }
    }
}
