using ApparelShoppingAppAPI.Models.DB_Models;
using ApparelShoppingAppAPI.Models.DTO_Models;
using ApparelShoppingAppAPI.Repositories.Interfaces;
using ApparelShoppingAppAPI.Services.Interfaces;
using System.Security.Cryptography;
using System.Text;
using ApparelShoppingAppAPI.Exceptions;
using ApparelShoppingAppAPI.Repositories.Classes;

namespace ApparelShoppingAppAPI.Services.Classes
{
    public class UserService : IUserService
    {
        private readonly IUserRegisterRepository _userRegisterRepository;
        private readonly IUserRepository _userRepository;
        private readonly ICustomerRepository _customerRepository;
        private readonly ISellerRepository _sellerRepository;
        private readonly ITokenService _tokenService;

        public UserService(IUserRepository userRepository, 
            ICustomerRepository customerRepository,
            ISellerRepository sellerRepository,
            IUserRegisterRepository userRegisterRepository,
            ITokenService tokenService
            )
        {
            _customerRepository = customerRepository;
            _userRepository = userRepository;
            _userRegisterRepository = userRegisterRepository;
            _tokenService = tokenService;
            _sellerRepository = sellerRepository;
        }

        private byte[] EncryptPassword(string password, byte[] passwordSalt)
        {
            HMACSHA512 hMACSHA = new HMACSHA512(passwordSalt);
            var encrypterPass = hMACSHA.ComputeHash(Encoding.UTF8.GetBytes(password));
            return encrypterPass;
        }

        private bool ComparePassword(byte[] encrypterPass, byte[] password)
        {
            for (int i = 0; i < encrypterPass.Length; i++)
            {
                if (encrypterPass[i] != password[i])
                {
                    return false;
                }
            }
            return true;
        }
        private LoginReturnDTO MapUserToLoginReturn(User user)
        {
            LoginReturnDTO returnDTO = new LoginReturnDTO();
            returnDTO.Id =user.UserId;
            returnDTO.Role = user.Role;
            returnDTO.Token = _tokenService.GenerateToken(user);
            return returnDTO;
        }
        
        public async Task<LoginReturnDTO> CustomerLogin(UserLoginDTO userLoginDTO)
        {
            try
            {
                var user = await _userRepository.GetCustomerUserByEmail(userLoginDTO.Email);
                if (user == null)
                {
                    throw new UnauthorizedUserException("Invalid Email or Password");
                }
                var encryptedPassword = EncryptPassword(userLoginDTO.Password, user.PasswordHashKey);
                bool isPasswordSame = ComparePassword(encryptedPassword, user.Password);
                if (isPasswordSame)
                {
                    if (user.Customer == null)
                    {
                        throw new UnauthorizedUserException("Invalid Email or Password");
                    }
                    LoginReturnDTO loginReturnDTO = MapUserToLoginReturn(user);
                    if(loginReturnDTO == null)
                    {
                        throw new NotAbelToLoginException("Error while generating token");
                    }
                    return loginReturnDTO;
                }
                throw new UnauthorizedUserException("Invalid Email or Password");
            }
            catch (Exception e)
            {
                throw new NotAbelToLoginException(e.Message);
            }
        }

        public async Task<LoginReturnDTO> SellerLogin(UserLoginDTO userLoginDTO)
        {
            try
            {
                var user = await _userRepository.GetSellerUserByEmail(userLoginDTO.Email);
                if (user == null)
                {
                    throw new UnauthorizedUserException("Invalid Email or Password");
                }
                var encryptedPassword = EncryptPassword(userLoginDTO.Password, user.PasswordHashKey);
                bool isPasswordSame = ComparePassword(encryptedPassword, user.Password);
                if (isPasswordSame)
                {
                    if (user.Seller == null)
                    {
                        throw new UnauthorizedUserException("Invalid Email or Password");
                    }
                    LoginReturnDTO loginReturnDTO = MapUserToLoginReturn(user);
                    if (loginReturnDTO == null)
                    {
                        throw new NotAbelToLoginException("Error while generating token");
                    }
                    return loginReturnDTO;
                }
                throw new UnauthorizedUserException("Invalid Email or Password");
            }
            catch (Exception e)
            {
                throw new NotAbelToLoginException(e.Message);
            }

        }
        private RegisterReturnDTO MapCustomerToRegisterReturn(User user, Customer customer)
        {
            RegisterReturnDTO returnDTO = new RegisterReturnDTO();
            returnDTO.Name = customer.Name;
            returnDTO.Email = customer.Email;
            returnDTO.Phone = customer.Phone;
            returnDTO.Role = user.Role;
            returnDTO.Token = _tokenService.GenerateToken(user);
            return returnDTO;
        }
        private RegisterReturnDTO MapSellerToRegisterReturn(User user, Seller seller)
        {
            RegisterReturnDTO returnDTO = new RegisterReturnDTO();
            returnDTO.Name = seller.Name;
            returnDTO.Email = seller.Email;
            returnDTO.Phone = seller.Phone;
            returnDTO.Role = user.Role;
            returnDTO.Token = _tokenService.GenerateToken(user);
            return returnDTO;
        }

        private UserRegisterRepositoryDTO MapUserRegisterRepositoryDTO(UserRegisterDTO userRegisterDTO)
        {
            UserRegisterRepositoryDTO userRegisterRepositoryDTO = new UserRegisterRepositoryDTO();
            HMACSHA512 hMACSHA = new HMACSHA512();
            userRegisterRepositoryDTO.PasswordHashKey = hMACSHA.Key;
            userRegisterRepositoryDTO.Password = hMACSHA.ComputeHash(Encoding.UTF8.GetBytes(userRegisterDTO.Password));
            userRegisterRepositoryDTO.Name = userRegisterDTO.Name;
            userRegisterRepositoryDTO.Email = userRegisterDTO.Email;
            userRegisterRepositoryDTO.Phone = userRegisterDTO.Phone;
            return userRegisterRepositoryDTO;
        }
       

        public async Task<RegisterReturnDTO> CustomerRegister(UserRegisterDTO userRegisterDTO)
        {
            try
            {
                var existingCustomer = await _customerRepository.GetCustomerByEmail(userRegisterDTO.Email);
                if (existingCustomer != null)
                {
                    throw new UserAlreadyExistsException("Email already exists");
                }
                if (userRegisterDTO.Password != userRegisterDTO.ConfirmPassword)
                {
                    throw new PasswordMismatchException("Password and Confirm Password do not match");
                }
                UserRegisterRepositoryDTO userRegisterRepositoryDTO = MapUserRegisterRepositoryDTO(userRegisterDTO);
                var (customer, user) = await _userRegisterRepository.AddCustomerUserWithTransaction(userRegisterRepositoryDTO);
                
                if (customer == null || user == null)
                {
                    throw new UnableToRegisterException("Error while adding user");
                }
                RegisterReturnDTO registerReturnDTO = MapCustomerToRegisterReturn(user, customer);
                return registerReturnDTO;
            }
            catch (Exception e)
            {
                throw new UnableToRegisterException(e.Message);
            }
        }

        public async Task<RegisterReturnDTO> SellerRegister(UserRegisterDTO userRegisterDTO)
        {
            try
            {
                var existingSeller = await _sellerRepository.GetSellerByEmail(userRegisterDTO.Email);
                if (existingSeller != null)
                {
                    throw new UserAlreadyExistsException("Email already exists");
                }
                if (userRegisterDTO.Password != userRegisterDTO.ConfirmPassword)
                {
                    throw new PasswordMismatchException("Password and Confirm Password do not match");
                }
                UserRegisterRepositoryDTO userRegisterRepositoryDTO = MapUserRegisterRepositoryDTO(userRegisterDTO);
                var (seller, user) = await _userRegisterRepository.AddSellerUserWithTransaction(userRegisterRepositoryDTO);

                if (seller == null || user == null)
                {
                    throw new UnableToRegisterException("Error while adding user");
                }
                RegisterReturnDTO registerReturnDTO = MapSellerToRegisterReturn(user, seller);
                return registerReturnDTO;
            }
            catch (Exception e)
            {
                throw new UnableToRegisterException(e.Message);
            }
        }

    }
   

}
