using ApparelShoppingAppAPI.Models.DB_Models;

namespace ApparelShoppingAppAPI.Repositories.Interfaces
{
    public interface IPaymentRepository : IRepository<int, PaymentDetail>
    {
        public Task<IEnumerable<PaymentDetail>> GetAllPaymentsByCustomer(int customerId);
        public Task<PaymentDetail> ProcessPaymentTransaction(PaymentDetail paymentDetail, Order order);
    }
}
