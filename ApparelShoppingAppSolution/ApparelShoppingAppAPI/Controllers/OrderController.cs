using ApparelShoppingAppAPI.Exceptions;
using ApparelShoppingAppAPI.Models.DB_Models;
using ApparelShoppingAppAPI.Models.DTO_Models;
using ApparelShoppingAppAPI.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ApparelShoppingAppAPI.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly IOrderService _orderService;
        private readonly ILogger<OrderController> _logger;

        public OrderController(IOrderService orderService, ILogger<OrderController> logger)
        {
            _orderService = orderService;
            _logger = logger;
        }

        #region AddOrderForSingleProduct
        /// <summary>
        /// Add an Order by Customer for Single Product
        /// </summary>
        /// <param name="orderDTO"></param>
        /// <returns></returns>
        [Authorize(Roles = "Customer")]
        [HttpPost]
        [ProducesResponseType(typeof(Order), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> AddOrder([FromBody] OrderDTO orderDTO)
        {
            try
            {
                int customerId = Convert.ToInt32(User.FindFirstValue(ClaimTypes.Name));
                Order newOrder = await _orderService.AddOrder(customerId, orderDTO);
                return Ok(newOrder);
            }
            catch (ProductNotFoundException ex)
            {
                return NotFound(new ErrorModel(404, ex.Message));
            }
            catch (AddressNotFoundException ex)
            {
                return NotFound(new ErrorModel(404, ex.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
        #endregion AddOrderForSingleProduct

        #region GetOrder
        [Authorize(Roles = "Customer")]
        [HttpGet("{orderId}")]
        [ProducesResponseType(typeof(Order), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetOrder(int orderId)
        {
            try
            {
                Order order = await _orderService.GetOrder(orderId);
                return Ok(order);
            }
            catch (OrderNotFoundException ex)
            {
                return NotFound(new ErrorModel(404, ex.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
        #endregion GetOrder

        #region GetAllOrdersByCustomer
        /// <summary>
        /// Get All Orders By Customer
        /// </summary>
        /// <returns></returns>
        [Authorize(Roles = "Customer")]
        [HttpGet("Customer")]
        [ProducesResponseType(typeof(IEnumerable<Order>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAllOrdersByCustomer()
        {
            try
            {
                int customerId = Convert.ToInt32(User.FindFirstValue(ClaimTypes.Name));
                IEnumerable<Order> orders = await _orderService.GetAllOrdersByCustomer(customerId);
                return Ok(orders);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
        #endregion GetAllOrdersByCustomer

        #region GetAllActiveOrdersByCustomer
        /// <summary>
        /// Get All Active Orders By Customer
        /// </summary>
        /// <returns></returns>
        [Authorize(Roles = "Customer")]
        [HttpGet("active/customer")]
        [ProducesResponseType(typeof(IEnumerable<Order>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAllActiveOrdersByCustomer()
        {
            try
            {
                int customerId = Convert.ToInt32(User.FindFirstValue(ClaimTypes.Name));
                IEnumerable<Order> orders = await _orderService.GetAllActiveOrdersByCustomer(customerId);
                return Ok(orders);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
        #endregion GetAllActiveOrdersByCustomer

        #region GetAllCancelOrdersByCustomer
        /// <summary>
        /// Get All Cancel Orders By Customer
        /// </summary>
        /// <returns></returns>
        [Authorize(Roles = "Customer")]
        [HttpGet("cancelled/customer")]
        [ProducesResponseType(typeof(IEnumerable<Order>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAllCancelOrdersByCustomer()
        {
            try
            {
                int customerId = Convert.ToInt32(User.FindFirstValue(ClaimTypes.Name));
                IEnumerable<Order> orders = await _orderService.GetAllCancelOrdersByCustomer(customerId);
                return Ok(orders);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
        #endregion GetAllCancelOrdersByCustomer

        #region GetAllOrdersBySeller
        /// <summary>
        /// Get All Orders By Seller
        /// </summary>
        /// <returns></returns>
        [HttpGet("seller")]
        [Authorize(Roles = "Seller")]
        [ProducesResponseType(typeof(IEnumerable<Order>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAllOrdersBySeller()
        {
            try
            {
                int sellerId = Convert.ToInt32(User.FindFirstValue(ClaimTypes.Name));
                IEnumerable<Order> orders = await _orderService.GetAllOrdersBySeller(sellerId);
                return Ok(orders);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
        #endregion GetAllOrdersBySeller

        #region CancelOrder
        /// <summary>
        /// Cancel Order
        /// </summary>
        /// <param name="orderId"></param>
        /// <returns></returns>
        [Authorize(Roles = "Customer")]
        [HttpPut("{orderId}")]
        [ProducesResponseType(typeof(Order), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> CancelOrder(int orderId)
        {
            try
            {
                Order canceledOrder = await _orderService.CancelOrder(orderId);
                return Ok(canceledOrder);
            }
            catch (OrderNotFoundException ex)
            {
                return NotFound(new ErrorModel(404, ex.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
        #endregion CancelOrder

        #region CartCheckOut
        [Authorize(Roles = "Customer")]
        [HttpPost("Checkout")]
        [ProducesResponseType(typeof(Order), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> CartCheckOut(CheckOutDTO checkOutDTO)
        {
            try
            {
                var customerId = Convert.ToInt32(User.FindFirstValue(ClaimTypes.Name));
                Order order = await _orderService.CartCheckOut(customerId, checkOutDTO.AddressId);
                return Ok(order);
            }
            catch (CartNotFoundException ex)
            {
                return NotFound(new ErrorModel(404, ex.Message));
            }
            catch (CartEmptyException ex)
            {
                return NotFound(new ErrorModel(404, ex.Message));
            }
            catch (AddressNotFoundException ex)
            {
                return NotFound(new ErrorModel(404, ex.Message));
            }
            catch (InsufficientProductQuantityException ex)
            {
                return BadRequest(new ErrorModel(400, ex.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
        #endregion CartCheckOut
    }
}
