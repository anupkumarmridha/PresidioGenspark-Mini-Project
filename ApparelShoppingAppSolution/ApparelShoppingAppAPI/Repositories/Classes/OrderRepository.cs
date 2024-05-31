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
        public OrderRepository(ShoppingAppDbContext context, IProductRepository productRepository) : base(context)
        {
            _productRepository = productRepository;
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
                 .ThenInclude(od => od.Product)
                  .Include(o => o.Address)
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
                .ThenInclude(od => od.Product)
                  .Include(o => o.Address)
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
        /// <summary>
        /// Checkout the cart with transaction
        /// </summary>
        /// <param name="cart"></param>
        /// <param name="address"></param>
        /// <returns></returns>
        /// <exception cref="InsufficientProductQuantityException"></exception>
        /// <exception cref="Exception"></exception>
        public async Task<Order> CheckOutCartWithTransaction(Cart cart, Address address)
        {
            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    // Create the order details
                    var orderDetails = new List<OrderDetails>();

                    // List to store items to be removed
                    var itemsToRemove = new List<CartItem>();

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
                        _context.Products.Update(product);
                        await _context.SaveChangesAsync();

                        // Add the order details from CartItems
                        orderDetails.Add(new OrderDetails
                        {
                            ProductId = item.ProductId,
                            Quantity = item.Quantity,
                            SubtotalPrice = product.Price * item.Quantity
                        });

                        // Add item to the list of items to be removed
                        itemsToRemove.Add(item);
                    }

                    // Remove the items from the cart outside the foreach loop
                    foreach (var item in itemsToRemove)
                    {
                        cart.Items.Remove(item);
                    }

                    // Create the order
                    var order = new Order
                    {
                        CustomerId = cart.CustomerId,
                        AddressId = address.AddressId,
                        TotalPrice = orderDetails.Sum(od => od.SubtotalPrice),
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
                }catch(InsufficientProductQuantityException)
                {
                    // Rollback transaction
                    await transaction.RollbackAsync();
                    throw;
                }
                catch (InsufficientProductQuantityException)
                {
                    // Rollback transaction
                    await transaction.RollbackAsync();
                    throw;
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

        #region CancelOrder
        /// <summary>
        /// Cancel Order
        /// </summary>
        /// <param name="orderId"></param>
        /// <returns></returns>
        /// <exception cref="OrderNotFoundException"></exception>
        /// <exception cref="Exception"></exception>
        public async Task<Order> CancelOrder(int orderId)
        {
            // Retrieve the order with its details, including products and address
            var order = await _context.Orders
                .Include(o => o.OrderDetails)
                    .ThenInclude(od => od.Product)
                .FirstOrDefaultAsync(o => o.OrderId == orderId);

            if (order == null)
            {
                throw new OrderNotFoundException("Order Not Found");
            }

            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    // Traverse the OrderDetails and update product quantities
                    foreach (var detail in order.OrderDetails)
                    {
                        var product = detail.Product;
                        if (product == null)
                        {
                            throw new ProductNotFoundException($"Product with ID {detail.ProductId} not found");
                        }

                        // Update the product quantity
                        product.Quantity += detail.Quantity;
                        _context.Products.Update(product);
                    }

                    // Mark the order as canceled
                    order.OrderStatus = "Canceled";
                    order.IsCanceled= true;
                    _context.Orders.Update(order);

                    // Save all changes
                    await _context.SaveChangesAsync();

                    // Commit the transaction
                    await transaction.CommitAsync();
                }
                catch (ProductNotFoundException)
                {
                    // Rollback the transaction in case of an error
                    await transaction.RollbackAsync();
                    throw;
                }
                catch (Exception ex)
                {
                    // Rollback the transaction in case of an error
                    await transaction.RollbackAsync();
                    throw new Exception("Error cancelling order", ex);
                }
            }

            return order;
        }
        #endregion CancelOrder

        #region GetAllCancelOrdersByCustomer
        /// <summary>
        /// Get all canceled orders by customer
        /// </summary>
        /// <param name="CustomerId"></param>
        /// <returns></returns>
        public async Task<IEnumerable<Order>> GetAllCancelOrdersByCustomer(int CustomerId)
        {
            return await _context.Orders
                .Include(o => o.OrderDetails)
                .Where(o => o.CustomerId == CustomerId && o.IsCanceled==true && o.OrderStatus == "Canceled")
                .ToListAsync();
        }
        #endregion GetAllCancelOrdersByCustomer

        #region GetAllActiveOrdersByCustomer
        public async Task<IEnumerable<Order>> GetAllActiveOrdersByCustomer(int CustomerId)
        {
            return await _context.Orders
                .Include(o => o.OrderDetails)
                .Where(o => o.CustomerId == CustomerId && o.IsCanceled==false && o.OrderStatus != "Canceled")
                .ToListAsync();
        }

        #endregion GetAllActiveOrdersByCustomer

        #region GetAllOrdersBySeller
        public async Task<IEnumerable<Order>> GetAllOrdersBySeller(int SellerId)
        {
            return await _context.Orders
                .Include(o => o.OrderDetails)
                .ThenInclude(od => od.Product)
                .Where(o => o.OrderDetails.Any(od => od.Product.SellerId == SellerId))
                .ToListAsync();
        }
        #endregion GetAllOrdersBySeller

    }
}
