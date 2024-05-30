using ApparelShoppingAppAPI.Models.DB_Models;

namespace ApparelShoppingAppAPI.Repositories.Interfaces
{
    public interface IOrderRepository : IRepository<int,Order>
    {
        public Task<IEnumerable<Order>> GetAllOrdersByCustomer(int CustomerId);
        public Task<Order> CancelOrder(int OrderId);
        public Task<IEnumerable<Order>> GetAllCancelOrdersByCustomer(int CustomerId);
        public Task<IEnumerable<Order>> GetAllActiveOrdersByCustomer(int CustomerId);
        public Task<IEnumerable<Order>> GetAllOrdersBySeller(int SellerId);
        public Task<Order> AddOrderWithTransaction(Order order, Product product, int quantityToDeduct);
        public Task<Order> CheckOutCartWithTransaction(Cart cart, Address address);
    }
}
