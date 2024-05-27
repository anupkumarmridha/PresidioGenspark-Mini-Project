using ApparelShoppingAppAPI.Models.DB_Models;

namespace ApparelShoppingAppAPI.Services.Interfaces
{
    public interface ICartService
    {
        Task<Cart> GetCartByUserId(int userId);
        Task<Cart> AddOrUpdateProductToCart(int userId, int productId, int quantity);
        Task<Cart> RemoveProductFromCart(int userId, int productId);
    }
}
