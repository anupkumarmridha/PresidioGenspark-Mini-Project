using ApparelShoppingAppAPI.Models.DTO_Models;

namespace ApparelShoppingAppAPI.Services.Interfaces
{
    public interface IUserService
    {
        Task<RegisterReturnDTO> CustomerRegister(UserRegisterDTO userRegisterDTO);
        Task<CustomerLoginReturnDTO> CustomerLogin(UserLoginDTO userLoginDTO);
        Task<RegisterReturnDTO> SellerRegister(UserRegisterDTO userRegisterDTO);
        Task<SellerLoginReturnDTO> SellerLogin(UserLoginDTO userLoginDTO);
    }
}
