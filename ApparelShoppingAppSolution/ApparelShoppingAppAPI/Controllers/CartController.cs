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
    public class CartController : ControllerBase
    {
        private readonly ICartService _cartService;
        private readonly ILogger<CartController> _logger;

        public CartController(ICartService cartService, ILogger<CartController> logger)
        {
            _cartService = cartService;
            _logger = logger;
        }

        #region GetCartByUserId

        /// <summary>
        /// Get the cart of the user.
        /// </summary>
        /// <returns>Cart Associated with User</returns>

        [Authorize(Roles = "Customer")]
        [HttpGet]
        [ProducesResponseType(typeof(Cart), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<Cart>> GetCartByUserId()
        {
            try
            {
                var userId = Convert.ToInt32(User.FindFirstValue(ClaimTypes.Name));
                if(userId == 0)
                {
                    return NotFound(new ErrorModel(404, "Invalid User"));
                }
                var cart = await _cartService.GetCartByUserId(userId);
                return Ok(cart);
            }
            catch (CartNotFoundException ex)
            {
                return NotFound(new ErrorModel(404, ex.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(500, new ErrorModel(500, "An error occurred while retrieving the cart"));
            }
        }
        #endregion

        #region AddProductToCart
        /// <summary>
        /// Adds a product to the cart.
        /// </summary>
        /// <param name="cartItemDTO">productId and quantity</param>
        /// <returns>Cart</returns>
        [Authorize(Roles = "Customer")]
        [HttpPost]
        [ProducesResponseType(typeof(Cart), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<Cart>> AddProductToCart(CartItemDTO cartItemDTO)
        {
            try
            {
                var userId = Convert.ToInt32(User.FindFirstValue(ClaimTypes.Name));
                if (userId == 0)
                {
                    return NotFound(new ErrorModel(404, "Invalid User"));
                }
                var cart = await _cartService.AddOrUpdateProductToCart(userId, cartItemDTO.ProductId, cartItemDTO.Quantity);
                return Ok(cart);
            }
            catch (CartNotFoundException ex)
            {
                return NotFound(new ErrorModel(404, ex.Message));
            }
            catch (ProductNotFoundException ex)
            {
                return NotFound(new ErrorModel(404, ex.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(500, new ErrorModel(500, "An error occurred while adding the product to the cart"));
            }
        }
        #endregion AddProductToCart

        #region RemoveProductFromCart
        /// <summary>
        /// Removes a product from the cart.
        /// </summary>
        /// <param name="productId"></param>
        /// <returns></returns>

        [Authorize(Roles = "Customer")]
        [HttpDelete("{productId}")]
        [ProducesResponseType(typeof(Cart), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<Cart>> RemoveProductFromCart(int productId)
        {
            try
            {
                var userId = Convert.ToInt32(User.FindFirstValue(ClaimTypes.Name));
                if (userId == 0)
                {
                    return NotFound(new ErrorModel(404, "Invalid User"));
                }
                var cart = await _cartService.RemoveProductFromCart(userId, productId);
                return Ok(cart);
            }
            catch (CartNotFoundException ex)
            {
                return NotFound(new ErrorModel(404, ex.Message));
            }
            catch (ProductNotFoundException ex)
            {
                return NotFound(new ErrorModel(404, ex.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(500, new ErrorModel(500, "An error occurred while removing the product from the cart"));
            }
        }
        #endregion RemoveProductFromCart
    }
}
