using ApparelShoppingAppAPI.Models.DB_Models;

namespace ApparelShoppingAppAPI.Repositories.Interfaces
{
    public interface IUserRepository:IRepository<int, User>
    {
        Task<User> GetCustomerUserByEmail(string email);
        Task<User> GetSellerUserByEmail(string email);
    }
}
