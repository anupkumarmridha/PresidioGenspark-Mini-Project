using ApparelShoppingAppAPI.Models.DB_Models;

namespace ApparelShoppingAppAPI.Repositories.Interfaces
{
    public interface IOrderRepository : IRepository<int,Order>
    {
        public Task<IEnumerable<Order>> GetAllOrdersByCustomer(int CustomerId);
        public Task<Order> AddOrderWithTransaction(Order order, Product product, int quantityToDeduct);
        public Task<Order> CheckOutCartWithTransaction(Cart cart, Address address);
    }
}
