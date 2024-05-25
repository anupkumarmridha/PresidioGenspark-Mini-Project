using ApparelShoppingAppAPI.Models.DB_Models;

namespace ApparelShoppingAppAPI.Services.Interfaces
{
    public interface ITokenService
    {
        public string GenerateCustomerToken(Customer customer);
        public string GenerateSellerToken(Seller seller);
    }
}
