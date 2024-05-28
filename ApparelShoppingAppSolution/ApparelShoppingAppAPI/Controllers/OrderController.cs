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
    [Authorize(Roles = "Customer")]
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

        #region CancelOrder
        [HttpDelete("{orderId}")]
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
