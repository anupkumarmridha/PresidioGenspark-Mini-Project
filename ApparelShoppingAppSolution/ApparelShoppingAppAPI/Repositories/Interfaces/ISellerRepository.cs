using ApparelShoppingAppAPI.Models.DB_Models;

namespace ApparelShoppingAppAPI.Repositories.Interfaces
{
    public interface ISellerRepository : IRepository<int, Seller>
    {
        Task<Seller> GetSellerByEmail(string email);
    }
}
