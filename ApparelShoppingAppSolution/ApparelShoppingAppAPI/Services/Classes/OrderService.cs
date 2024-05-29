using ApparelShoppingAppAPI.Exceptions;
using ApparelShoppingAppAPI.Models.DB_Models;
using ApparelShoppingAppAPI.Models.DTO_Models;
using ApparelShoppingAppAPI.Repositories.Interfaces;
using ApparelShoppingAppAPI.Services.Interfaces;
using System.Linq;
namespace ApparelShoppingAppAPI.Services.Classes
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly ICartRepository _cartRepository;
        private readonly IProductRepository _productRepository;
        private readonly IAddressRepository _addressRepository;

        public OrderService(IOrderRepository orderRepository, ICartRepository cartRepository, IProductRepository productRepository, IAddressRepository addressRepository)
        {
            _orderRepository = orderRepository;
            _cartRepository = cartRepository;
            _productRepository = productRepository;
            _addressRepository = addressRepository;
        }

        #region AddOrder
        /// <summary>
        /// Add an Order by Customer for Single Product
        /// </summary>
        /// <param name="customerId"></param>
        /// <param name="orderDTO"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task<Order> AddOrder(int customerId, OrderDTO orderDTO)
        {
            try
            {
                var product = await _productRepository.GetById(orderDTO.ProductId);
                if (product == null)
                {
                    throw new ProductNotFoundException("Product Not Found");
                }
                if (product.Quantity < orderDTO.Quantity)
                {
                    throw new InsufficientProductQuantityException($"Insufficient product quantity. Available Quantity {product.Quantity}");
                }

                var address = await _addressRepository.GetById(orderDTO.AddressId);
                if (address == null || address.CustomerId != customerId)
                {
                    throw new AddressNotFoundException("Invalid address");
                }

                if (product.Quantity < orderDTO.Quantity)
                {
                    throw new Exception("Insufficient product quantity");
                }

                var order = new Order
                {
                    CustomerId = customerId,
                    AddressId = orderDTO.AddressId,
                    TotalPrice = product.Price * orderDTO.Quantity,
                    OrderDetails = new List<OrderDetails>
            {
                new OrderDetails
                {
                    ProductId = orderDTO.ProductId,
                    Quantity = orderDTO.Quantity,
                    SubtotalPrice = product.Price * orderDTO.Quantity
                }
            }
                };

                await _orderRepository.AddOrderWithTransaction(order, product, orderDTO.Quantity);
                return order;
            }
            catch (ProductNotFoundException)
            {
                throw;
            }
            catch (InsufficientProductQuantityException)
            {
                throw;
            }
            catch (AddressNotFoundException)
            {
                throw;
            }
            catch (Exception e)
            {
                throw new Exception("Error adding order", e);
            }
        }

        #endregion AddOrder

        #region GetOrder
        public async Task<Order> GetOrder(int orderId)
        {
            try
            {
                var order = await _orderRepository.GetById(orderId);
                if (order == null)
                {
                    throw new OrderNotFoundException("Order Not Found");
                }
                return order;
            }
            catch (OrderNotFoundException)
            {
                throw;
            }
            catch(Exception e)
            {
                throw new Exception("Error getting order", e);
            }
        }
        #endregion GetOrder

        #region GetAllOrdersByCustomer
        public async Task<IEnumerable<Order>> GetAllOrdersByCustomer(int customerId)
        {
            try
            {
                return await _orderRepository.GetAllOrdersByCustomer(customerId);
            }
            catch (Exception e)
            {
                throw new Exception("Error getting orders", e);
            }
        }
        #endregion GetAllOrdersByCustomer

        #region CancelOrder
        /// <summary>
        /// Cancel an Order
        /// </summary>
        /// <param name="orderId"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task<Order> CancelOrder(int orderId)
        {
            try
            {
                var order = await _orderRepository.GetById(orderId);
                if (order == null)
                {
                    throw new OrderNotFoundException("Order Not Found");
                }
                await _orderRepository.CancelOrder(order.OrderId);
                return order;
            }
            catch (OrderNotFoundException)
            {
                throw;
            }catch (ProductNotFoundException)
            {
                throw;
            }catch (InvalidOperationException)
            {
                throw;
            }
            catch (Exception e)
            {
                throw new Exception("Error cancelling order", e);
            }
        }
        #endregion CancelOrder

        #region CartCheckOut
        public async Task<Order> CartCheckOut(int customerId, int addressId)
        {
            try
            {
                // Fetch cart and address
                var cart = await _cartRepository.GetCartByUserId(customerId);
                if (cart == null)
                {
                    throw new CartNotFoundException("Cart Not Found");
                }

                var address = await _addressRepository.GetById(addressId);
                if (address == null)
                {
                    throw new AddressNotFoundException("Customer Address Not Found");
                }

                // Create order and delegate the transaction to the repository
                var order = await _orderRepository.CheckOutCartWithTransaction(cart, address);

                return order;
            }
            catch (CartNotFoundException)
            {
                throw;
            }
            catch (AddressNotFoundException)
            {
                throw;
            }
            catch (InsufficientProductQuantityException)
            {
                throw;
            }
            catch (Exception e)
            {
                throw new Exception("Error checking out cart", e);
            }
        }
        #endregion CartCheckOut
    }
}
