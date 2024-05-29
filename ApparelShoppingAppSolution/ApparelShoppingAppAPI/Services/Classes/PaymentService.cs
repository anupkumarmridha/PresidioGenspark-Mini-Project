using ApparelShoppingAppAPI.Exceptions;
using ApparelShoppingAppAPI.Models.DB_Models;
using ApparelShoppingAppAPI.Models.DTO_Models;
using ApparelShoppingAppAPI.Repositories.Interfaces;
using ApparelShoppingAppAPI.Services.Interfaces;

namespace ApparelShoppingAppAPI.Services.Classes
{
    public class PaymentService : IPaymentService
    {
        private readonly IPaymentRepository _paymentRepository;
        private readonly IOrderRepository _orderRepository;
        public PaymentService(IPaymentRepository paymentRepository, IOrderRepository orderRepository)
        {
            _paymentRepository = paymentRepository;
            _orderRepository = orderRepository;
        }

        #region GetAllPaymentDetails
        public Task<IEnumerable<PaymentDetail>> GetAllPaymentDetails(int customerId)
        {
            try
            {
                return _paymentRepository.GetAllPaymentsByCustomer(customerId);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        #endregion GetAllPaymentDetails

        #region GetPaymentDetail
        /// <summary>
        /// Get Payment Detail by Payment Id
        /// </summary>
        /// <param name="paymentDetailId"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task<PaymentDetail> GetPaymentDetail(int paymentDetailId)
        {
            try
            {
                var payment = await _paymentRepository.GetById(paymentDetailId);
                if (payment == null)
                {
                    throw new PaymentNotFoundException("Payment Not Found");
                }
                return payment;
            }
            catch (PaymentNotFoundException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        #endregion GetPaymentDetail

        #region ProcessPayment
        /// <summary>
        /// Process Payment
        /// </summary>
        /// <param name="paymentDTO"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task<PaymentDetail> ProcessPayment(PaymentDTO paymentDTO)
        {
            try
            {
                var order = await _orderRepository.GetById(paymentDTO.OrderId);
                if (order == null)
                {
                    throw new OrderNotFoundException("Order Not Found");
                }
                var payment = new PaymentDetail
                {
                    OrderId = paymentDTO.OrderId,
                    PaymentDate = DateTime.Now,
                    PaymentMethod = paymentDTO.PaymentMethod,
                    Status = "Success",
                    Amount = order.TotalPrice
                };
                await _paymentRepository.ProcessPaymentTransaction(payment, order);
                return payment;
            }
            catch (OrderNotFoundException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        #endregion ProcessPayment
    }
}
