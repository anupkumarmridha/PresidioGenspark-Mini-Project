using ApparelShoppingAppAPI.Models.DB_Models;
using ApparelShoppingAppAPI.Models.DTO_Models;

namespace ApparelShoppingAppAPI.Services.Interfaces
{
    public interface IOrderService
    {
        public Task<Order> AddOrder(int CustomerId, OrderDTO orderDTO);
        public Task<Order> GetOrder(int OrderId);
        public Task<IEnumerable<Order>> GetAllOrdersByCustomer(int CustomerId);
        public Task<IEnumerable<Order>> GetAllActiveOrdersByCustomer(int customerId);
        public Task<IEnumerable<Order>> GetAllCancelOrdersByCustomer(int customerId);
        public Task<IEnumerable<Order>> GetAllOrdersBySeller(int SellerId);
        public Task<Order> CancelOrder(int OrderId);
        public Task<Order> CartCheckOut(int customerId, int addressId);

    }
}
