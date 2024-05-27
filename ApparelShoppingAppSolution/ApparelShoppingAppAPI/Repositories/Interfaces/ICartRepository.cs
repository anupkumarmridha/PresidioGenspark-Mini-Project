using ApparelShoppingAppAPI.Models.DB_Models;

namespace ApparelShoppingAppAPI.Repositories.Interfaces
{
    public interface ICartRepository:IRepository<int, Cart>
    {
        public Task<Cart> GetCartByUserId(int userId);
    }
}
