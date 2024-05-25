using ApparelShoppingAppAPI.Models.DB_Models;

namespace ApparelShoppingAppAPI.Models.DTO_Models
{
    public class CustomerLoginReturnDTO
    {
        public int CustomerId { get; set; }
        public string Token { get; set; }
        public string Role { get; set; }
    }
}
