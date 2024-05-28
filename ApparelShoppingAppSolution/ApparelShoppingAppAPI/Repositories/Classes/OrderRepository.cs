using ApparelShoppingAppAPI.Contexts;
using ApparelShoppingAppAPI.Exceptions;
using ApparelShoppingAppAPI.Models.DB_Models;
using ApparelShoppingAppAPI.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ApparelShoppingAppAPI.Repositories.Classes
{
    public class OrderRepository : BaseRepository<int, Order>, IOrderRepository
    {
        private readonly IProductRepository _productRepository;
        private readonly ICartRepository _cartRepository;
        public OrderRepository(ShoppingAppDbContext context, IProductRepository productRepository, ICartRepository cartRepository) : base(context)
        {
            _productRepository = productRepository;
            _cartRepository = cartRepository;
        }

        #region GetById
        /// <summary>
        /// Get an order by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        /// <exception cref="OrderNotFoundException"></exception>
        public override async Task<Order> GetById(int id)
        {
            var order = await _context.Orders
                .Include(o => o.OrderDetails)
                .FirstOrDefaultAsync(o => o.OrderId == id);
            if (order == null)
            {
                throw new OrderNotFoundException("Order Not Found");
            }
            return order;
        }
        #endregion GetById

        #region GetAll
        /// <summary>
        /// Get all orders
        /// </summary>
        /// <returns></returns>
        public override async Task<IEnumerable<Order>> GetAll()
        {
            return await _context.Orders
                .Include(o => o.OrderDetails)
                .ToListAsync();
        }
        #endregion GetAll

        #region GetAllOrdersByCustomer
        /// <summary>
        /// Get all orders by customer
        /// </summary>
        /// <param name="CustomerId"></param>
        /// <returns></returns>
        public async Task<IEnumerable<Order>> GetAllOrdersByCustomer(int CustomerId)
        {
            return await _context.Orders
                .Include(o => o.OrderDetails)
                .Where(o => o.CustomerId == CustomerId)
                .ToListAsync();
        }
        #endregion GetAllOrdersByCustomer

        #region AddOrderWithTransaction
        /// <summary>
        /// Add an Order with Transaction
        /// </summary>
        /// <param name="order"></param>
        /// <param name="product"></param>
        /// <param name="quantityToDeduct"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task<Order> AddOrderWithTransaction(Order order, Product product, int quantityToDeduct)
        {
            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    // Add the order
                    await _context.Orders.AddAsync(order);
                    await _context.SaveChangesAsync();

                    // Update the product quantity
                    product.Quantity -= quantityToDeduct;
                    await _productRepository.Update(product);
                    await _context.SaveChangesAsync();

                    // Commit transaction
                    await transaction.CommitAsync();
                    return order;
                }
                catch (Exception e)
                {
                    // Rollback transaction
                    await transaction.RollbackAsync();
                    throw new Exception(e.Message);
                }
            }

        }
        #endregion AddOrderWithTransaction

        #region CheckOutCartWithTransaction
        public async Task<Order> CheckOutCartWithTransaction(Cart cart, Address address)
        {
            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    // Create the order details
                    var orderDetails = new List<OrderDetails>();

                    // Traverse through the cart items
                    foreach (var item in cart.Items)
                    {
                        // Check if the product exists and has sufficient quantity
                        var product = await _productRepository.GetById(item.ProductId);
                        if (product == null || product.Quantity < item.Quantity)
                        {
                            throw new InsufficientProductQuantityException($"Insufficient product quantity or product not found for Product ID {item.ProductId}");
                        }

                        // Update the product quantity
                        product.Quantity -= item.Quantity;
                        await _productRepository.Update(product);

                        // Add the order details from CartItems
                        orderDetails.Add(new OrderDetails
                        {
                            ProductId = item.ProductId,
                            Quantity = item.Quantity,
                            SubtotalPrice = product.Price * item.Quantity
                        });

                        //Remove the item from the cart
                        cart.Items.Remove(item);
                    }

                    // Create the order
                    var order = new Order
                    {
                        CustomerId = cart.CustomerId,
                        AddressId = address.AddressId,
                        TotalPrice = cart.TotalPrice,
                        OrderDetails = orderDetails
                    };

                    // Add the order
                    await _context.Orders.AddAsync(order);
                    await _context.SaveChangesAsync();

                    // Update the cart after removing the cart items
                    _context.Carts.Update(cart);
                    await _context.SaveChangesAsync();

                    // Commit transaction
                    await transaction.CommitAsync();

                    return order;
                }
                catch (Exception e)
                {
                    // Rollback transaction
                    await transaction.RollbackAsync();
                    throw new Exception("Error during checkout transaction", e);
                }
            }
        }
        #endregion CheckOutCartWithTransaction

    }
}
