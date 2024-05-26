using ApparelShoppingAppAPI.Models.DB_Models;

namespace ApparelShoppingAppAPI.Models.DTO_Models
{
    public class LoginReturnDTO
    {
        public int Id { get; set; }
        public string Token { get; set; }
        public string Role { get; set; }
    }
}
