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
    public class PaymentController : ControllerBase
    {
        private readonly IPaymentService _paymentService;
        private readonly ILogger<PaymentController> _logger;

        public PaymentController(IPaymentService paymentService, ILogger<PaymentController> logger)
        {
            _paymentService = paymentService;
            _logger = logger;
        }

        #region GetPaymentDetail
        [HttpGet("{paymentDetailId}")]
        [ProducesResponseType(typeof(PaymentDetail), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetPaymentDetail(int paymentDetailId)
        {
            try
            {
                var paymentDetail = await _paymentService.GetPaymentDetail(paymentDetailId);
                if (paymentDetail == null)
                {
                    return NotFound(new ErrorModel(404, "Payment Detail Not Found"));
                }
                return Ok(paymentDetail);
            }
            catch (PaymentNotFoundException ex)
            {
                return NotFound(new ErrorModel(404, ex.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
        #endregion GetPaymentDetail

        #region GetAllPaymentDetails
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<PaymentDetail>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAllPaymentDetails()
        {
            try
            {
                var customerId = Convert.ToInt32(User.FindFirstValue(ClaimTypes.Name));
                var paymentDetails = await _paymentService.GetAllPaymentDetails(customerId);
                return Ok(paymentDetails);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
        #endregion GetAllPaymentDetails

        #region ProcessPayment
        /// <summary>
        /// Process Payment
        /// </summary>
        /// <param name="paymentDTO"></param>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(typeof(PaymentDetail), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> ProcessPayment([FromBody] PaymentDTO paymentDTO)
        {
            try
            {
                var paymentDetail = await _paymentService.ProcessPayment(paymentDTO);
                return CreatedAtAction(nameof(GetPaymentDetail), new { paymentDetailId = paymentDetail.PaymentDetailId }, paymentDetail);
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
        #endregion ProcessPayment

    }
}
