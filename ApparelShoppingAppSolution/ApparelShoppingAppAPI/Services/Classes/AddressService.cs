using ApparelShoppingAppAPI.Exceptions;
using ApparelShoppingAppAPI.Models.DB_Models;
using ApparelShoppingAppAPI.Models.DTO_Models;
using ApparelShoppingAppAPI.Repositories.Interfaces;
using ApparelShoppingAppAPI.Services.Interfaces;

namespace ApparelShoppingAppAPI.Services.Classes
{
    public class AddressService : IAddressService
    {
        private readonly IAddressRepository _addressRepository;
        public AddressService(IAddressRepository addressRepository)
        {
            _addressRepository = addressRepository;
        }

        #region Add Address
        /// <summary>
        /// Add address for a customer
        /// </summary>
        /// <param name="customerId"></param>
        /// <param name="address"></param>
        /// <returns>Added Address</returns>
        public async Task<Address> AddCustomerAddress(int customerId, AddressDTO address)
        {
            try
            {
                Address newAddress = new Address
                {
                    CustomerId = customerId,
                    Street = address.Street,
                    City = address.City,
                    State = address.State,
                    ZipCode = address.ZipCode,
                    Country = address.Country
                };
                return await _addressRepository.Add(newAddress);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion Add Address

        #region Delete Address
        /// <summary>
        /// Delete address for a customer
        /// </summary>
        /// <param name="addressId"></param>
        /// <returns>Deleted Address</returns>
        public async Task<Address> DeleteCustomerAddress(int addressId)
        {
            try
            {
                return await _addressRepository.Delete(addressId);
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }
        #endregion Delete Address

        #region Get All Address of A Customer
        /// <summary>
        /// Get all address of a customer
        /// </summary>
        /// <param name="customerId"></param>
        /// <returns>List of Address of A Customer</returns>
        public async Task<IEnumerable<Address>> GetAllAddressByCustomerId(int customerId)
        {
            try
            {
                return await _addressRepository.GetAllAddressByCustomerId(customerId);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion Get All Address of A Customer

        #region Get Address
        /// <summary>
        /// Get Address by addressId
        /// </summary>
        /// <param name="addressId"></param>
        /// <returns>Address</returns>
        /// <exception cref="NotImplementedException"></exception>
        public async Task<Address> GetCustomerAddress(int addressId)
        {
            try
            {
                var address=await _addressRepository.GetById(addressId);
                if (address == null)
                {
                    throw new AddressNotFoundException("Address Not Found");
                }
                return address;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion Get Address

        #region Update Address
        /// <summary>
        /// Update address for a customer
        /// </summary>
        /// <param name="address"></param>
        /// <returns>Updated Address</returns>
        /// <exception cref="AddressNotFoundException"></exception>
        public async Task<Address> UpdateCustomerAddress(int addressId, AddressDTO addressDTO)
        {
            try
            {
                var addressToUpdate = await _addressRepository.GetById(addressId);
                if (addressToUpdate == null)
                {
                    throw new AddressNotFoundException("Address Not Found");
                }

                // Update the properties of the retrieved address
                addressToUpdate.Street = addressDTO.Street;
                addressToUpdate.City = addressDTO.City;
                addressToUpdate.State = addressDTO.State;
                addressToUpdate.Country = addressDTO.Country;
                addressToUpdate.ZipCode = addressDTO.ZipCode;

                await _addressRepository.Update(addressToUpdate);
                return addressToUpdate;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion Update Address
    }
}
