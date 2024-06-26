﻿using ApparelShoppingAppAPI.Models.DB_Models;
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

        #region EncryptPassword
        /// <summary>
        /// Generate encrypted password
        /// </summary>
        /// <param name="password"></param>
        /// <param name="passwordSalt"></param>
        /// <returns></returns>
        private byte[] EncryptPassword(string password, byte[] passwordSalt)
        {
            HMACSHA512 hMACSHA = new HMACSHA512(passwordSalt);
            var encrypterPass = hMACSHA.ComputeHash(Encoding.UTF8.GetBytes(password));
            return encrypterPass;
        }
        #endregion EncryptPassword

        #region ComparePassword
        /// <summary>
        /// Compare password
        /// </summary>
        /// <param name="encrypterPass"></param>
        /// <param name="password"></param>
        /// <returns>If Match True else False</returns>
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
        #endregion ComparePassword

        #region MapUserToLoginReturn
        /// <summary>
        /// Map User to LoginReturnDTO
        /// </summary>
        /// <param name="user"></param>
        /// <returns>Return LoginReturnDTO</returns>
        private LoginReturnDTO MapUserToLoginReturn(User user)
        {
            LoginReturnDTO returnDTO = new LoginReturnDTO();
            returnDTO.Id =user.UserId;
            returnDTO.Role = user.Role;
            returnDTO.Token = _tokenService.GenerateToken(user);
            return returnDTO;
        }
        #endregion MapUserToLoginReturn

        #region CustomerLogin
        /// <summary>
        /// Customer Login
        /// </summary>
        /// <param name="userLoginDTO"></param>
        /// <returns></returns>
        /// <exception cref="UnauthorizedUserException"></exception>
        /// <exception cref="NotAbelToLoginException"></exception>
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
            catch (CustomerNotFoundException)
            {
                throw new UnauthorizedUserException("Invalid Email or Password");
            }
            catch (Exception e)
            {
                throw new NotAbelToLoginException(e.Message);
            }
        }
        #endregion CustomerLogin

        #region SellerLogin
        /// <summary>
        /// 
        /// </summary>
        /// <param name="userLoginDTO"></param>
        /// <returns></returns>
        /// <exception cref="UnauthorizedUserException"></exception>
        /// <exception cref="NotAbelToLoginException"></exception>
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
            catch (SellerNotFoundException)
            {
                throw new UnauthorizedUserException("Invalid Email or Password");
            }
            catch (Exception e)
            {
                throw new NotAbelToLoginException(e.Message);
            }

        }
        #endregion SellerLogin

        #region MapCustomerToRegisterReturn
        /// <summary>
        /// Map Customer to RegisterReturnDTO
        /// </summary>
        /// <param name="user"></param>
        /// <param name="customer"></param>
        /// <returns></returns>
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
        #endregion MapCustomerToRegisterReturn

        #region MapSellerToRegisterReturn
        /// <summary>
        /// Map Seller to RegisterReturnDTO
        /// </summary>
        /// <param name="user"></param>
        /// <param name="seller"></param>
        /// <returns></returns>
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
        #endregion MapSellerToRegisterReturn

        #region MapUserRegisterRepositoryDTO
        /// <summary>
        /// Map UserRegisterDTO to UserRegisterRepositoryDTO
        /// </summary>
        /// <param name="userRegisterDTO"></param>
        /// <returns></returns>
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
        #endregion MapUserRegisterRepositoryDTO

        #region CustomerRegister
        /// <summary>
        /// Customer Register
        /// </summary>
        /// <param name="userRegisterDTO"></param>
        /// <returns></returns>
        /// <exception cref="UserAlreadyExistsException"></exception>
        /// <exception cref="PasswordMismatchException"></exception>
        /// <exception cref="UnableToRegisterException"></exception>
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
        #endregion CustomerRegister

        #region SellerRegister
        /// <summary>
        /// Seller Register
        /// </summary>
        /// <param name="userRegisterDTO"></param>
        /// <returns></returns>
        /// <exception cref="UserAlreadyExistsException"></exception>
        /// <exception cref="PasswordMismatchException"></exception>
        /// <exception cref="UnableToRegisterException"></exception>
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
        #endregion SellerRegister

    }


}
