using ApparelShoppingAppAPI.Models.DB_Models;
using ApparelShoppingAppAPI.Models.DTO_Models;

namespace ApparelShoppingAppAPI.Services.Interfaces
{
    public interface IAddressService
    {
        Task<Address> AddCustomerAddress(int customerId, AddressDTO address);
        Task<Address> GetCustomerAddress(int addressId);
        Task<IEnumerable<Address>> GetAllAddressByCustomerId(int customerId);
        Task<Address> UpdateCustomerAddress(int addressId, AddressDTO address);
        Task<Address> DeleteCustomerAddress(int addressId);
    }
}
