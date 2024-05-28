using ApparelShoppingAppAPI.Models.DB_Models;
using ApparelShoppingAppAPI.Models.DTO_Models;

namespace ApparelShoppingAppAPI.Services.Interfaces
{
    public interface IOrderService
    {
        public Task<Order> AddOrder(int CustomerId, OrderDTO orderDTO);
        public Task<Order> GetOrder(int OrderId);
        public Task<IEnumerable<Order>> GetAllOrdersByCustomer(int CustomerId);
        public Task<Order> CancelOrder(int OrderId);
        public Task<Order> CartCheckOut(int customerId, int addressId);
    }
}
