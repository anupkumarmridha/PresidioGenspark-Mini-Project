using ApparelShoppingAppAPI.Models.DB_Models;
using ApparelShoppingAppAPI.Models.DTO_Models;

namespace ApparelShoppingAppAPI.Services.Interfaces
{
    public interface IPaymentService
    {
        Task<PaymentDetail> GetPaymentDetail(int paymentDetailId);
        Task<IEnumerable<PaymentDetail>> GetAllPaymentDetails(int customerId);
        Task<PaymentDetail> ProcessPayment(PaymentDTO paymentDTO);
    }
}
