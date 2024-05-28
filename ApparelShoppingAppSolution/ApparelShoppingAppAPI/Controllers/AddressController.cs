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
    public class AddressController : ControllerBase
    {
        private readonly IAddressService _addressService;
        private readonly ILogger<AddressController> _logger;
        public AddressController(IAddressService addressService, ILogger<AddressController> logger)
        {
            _addressService = addressService;
            _logger = logger;
        }

        #region Add Address
        /// <summary>
        /// Add address for a customer
        /// </summary>
        /// <param name="address"></param>
        /// <returns></returns>
        [Authorize(Roles = "Customer")]
        [HttpPost]
        [ProducesResponseType(typeof(Address), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> AddCustomerAddress([FromBody] AddressDTO address)
        {
            try
            {
                int customerId = Convert.ToInt32(User.FindFirstValue(ClaimTypes.Name));
                Address newAddress = await _addressService.AddCustomerAddress(customerId, address);
                return Ok(newAddress);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
        #endregion Add Address

        #region Get Address
        /// <summary>
        /// Get address for a customer
        /// </summary>
        /// <param name="addressId"></param>
        /// <returns></returns>
        [Authorize(Roles="Customer")]
        [HttpGet("{addressId}")]
        [ProducesResponseType(typeof(Address), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetCustomerAddress(int addressId)
        {
            try
            {
                Address address = await _addressService.GetCustomerAddress(addressId);
                return Ok(address);
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
        #endregion Get Address

        #region Get All Address of Customer
        [Authorize(Roles="Customer")]
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<Address>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAllAddressByCustomerId()
        {
            try
            {
                int customerId = Convert.ToInt32(User.FindFirstValue(ClaimTypes.Name));
                IEnumerable<Address> addresses = await _addressService.GetAllAddressByCustomerId(customerId);
                return Ok(addresses);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
        #endregion Get All Address of Customer

        #region Update Address
        [Authorize(Roles = "Customer")]
        [HttpPut("{addressId}")]
        [ProducesResponseType(typeof(Address), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdateCustomerAddress(int addressId, [FromBody] AddressDTO addressDTO)
        {
            try
            {
                Address updatedAddress = await _addressService.UpdateCustomerAddress(addressId, addressDTO);
                return Ok(updatedAddress);
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
        #endregion Update Address

        #region Delete Address
        /// <summary>
        /// Delete address for a customer
        /// </summary>
        /// <param name="addressId"></param>
        /// <returns></returns>
        [Authorize(Roles = "Customer")]
        [HttpDelete("{addressId}")]
        [ProducesResponseType(typeof(Address), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Address), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteCustomerAddress(int addressId)
        {
            try
            {
                Address deletedAddress = await _addressService.DeleteCustomerAddress(addressId);
                return Ok(deletedAddress);
            }
            catch(InvalidOperationException ex)
            {
                return NotFound(new ErrorModel(404, ex.Message));
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
        #endregion Delete Address
    }
}
