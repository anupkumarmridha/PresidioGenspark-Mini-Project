using ApparelShoppingAppAPI.Models.DB_Models;

namespace ApparelShoppingAppAPI.Repositories.Interfaces
{
    public interface IAddressRepository:IRepository<int,Address>
    {
        Task<IEnumerable<Address>> GetAllAddressByCustomerId(int customerId);
    }
}
