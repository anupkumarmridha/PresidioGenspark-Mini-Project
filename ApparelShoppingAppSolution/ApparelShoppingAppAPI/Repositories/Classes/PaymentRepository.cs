using ApparelShoppingAppAPI.Contexts;
using ApparelShoppingAppAPI.Exceptions;
using ApparelShoppingAppAPI.Models.DB_Models;
using ApparelShoppingAppAPI.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ApparelShoppingAppAPI.Repositories.Classes
{
    public class PaymentRepository : BaseRepository<int, PaymentDetail>, IPaymentRepository
    {
        public PaymentRepository(ShoppingAppDbContext context) : base(context)
        {

        }
        #region GetAllPaymentsByCustomer
        public async Task<IEnumerable<PaymentDetail>> GetAllPaymentsByCustomer(int customerId)
        {
            return await _context.PaymentDetails
                .Include(p => p.Order)
                .Where(p => p.Order.CustomerId == customerId)
                .ToListAsync();
        }
        #endregion GetAllPaymentsByCustomer

        #region ProcessPaymentTransaction
        /// <summary>
        /// Process payment transaction
        /// </summary>
        /// <param name="paymentDetail"></param>
        /// <param name="order"></param>
        /// <returns></returns>
        public async Task<PaymentDetail> ProcessPaymentTransaction(PaymentDetail paymentDetail, Order order)
        {
            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {

                    await _context.PaymentDetails.AddAsync(paymentDetail);
                    await _context.SaveChangesAsync();

                    // Update order status to Paid
                    order.isPaid = true;
                    _context.Update(order);

                    
                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();

                    return paymentDetail;
                }
                catch (Exception)
                {
                    await transaction.RollbackAsync();
                    throw;
                }
            }
        }
        #endregion ProcessPaymentTransaction
    }
}
