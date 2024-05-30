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
    public class ReviewsController : ControllerBase
    {
        private readonly IReviewService _reviewService;
        private readonly ILogger<ReviewsController> _logger;

        public ReviewsController(IReviewService reviewService, ILogger<ReviewsController> logger)
        {
            _reviewService = reviewService;
            _logger = logger;
        }


        #region AddReview
        /// <summary>
        /// Add a review for a product by Customer
        /// </summary>
        /// <param name="reviewDTO"></param>
        /// <returns></returns>
        [Authorize(Roles = "Customer")]
        [HttpPost]
        [ProducesResponseType(typeof(Review), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> AddReview([FromBody] ReviewDTO reviewDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                int customerId = Convert.ToInt32(User.FindFirstValue(ClaimTypes.Name));
                var review = await _reviewService.AddReview(reviewDTO, customerId);
                return CreatedAtAction(nameof(GetReviewById), new { reviewId = review.ReviewId }, review);
            }
            catch (ProductNotFoundException ex)
            {
                return NotFound(new ErrorModel(404, ex.Message));
            }
            catch (CustomerNotFoundException ex)
            {
                return NotFound(new ErrorModel(404, ex.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
        #endregion AddReview

        #region GetReviewById
        /// <summary>
        /// Get review by reviewId
        /// </summary>
        /// <param name="reviewId"></param>
        /// <returns></returns>
        [HttpGet("{reviewId}")]
        [ProducesResponseType(typeof(Review), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetReviewById(int reviewId)
        {
            try
            {
                var review = await _reviewService.GetReviewById(reviewId);
                if (review == null)
                {
                    return NotFound(new ErrorModel(404, "Review not found"));
                }
                return Ok(review);
            }
            catch(ReviewNotFoundException ex)
            {
                return NotFound(new ErrorModel(404, ex.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
        #endregion GetReviewById

        #region GetReviewsByProductId
        /// <summary>
        /// Get reviews by productId
        /// </summary>
        /// <param name="productId"></param>
        /// <returns></returns>
        [HttpGet("product/{productId}")]
        [ProducesResponseType(typeof(IEnumerable<Review>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetReviewsByProductId(int productId)
        {
            try
            {
                var reviews = await _reviewService.GetReviewsByProductId(productId);
                return Ok(reviews);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
        #endregion GetReviewsByProductId

        #region GetReviewsByCustomerId
        /// <summary>
        /// Get reviews by customerId
        /// </summary>
        /// <param name="customerId"></param>
        /// <returns></returns>
        [HttpGet("customer/{customerId}")]
        [ProducesResponseType(typeof(IEnumerable<Review>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetReviewsByCustomerId(int customerId)
        {
            try
            {
                var reviews = await _reviewService.GetReviewsByCustomerId(customerId);
                return Ok(reviews);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
        #endregion GetReviewsByCustomerId

        #region UpdateReview
        /// <summary>
        /// Update a review by reviewId
        /// </summary>
        /// <param name="reviewId"></param>
        /// <param name="reviewDTO"></param>
        /// <returns></returns>
        [Authorize(Roles = "Customer")]
        [HttpPut("{reviewId}")]
        [ProducesResponseType(typeof(Review), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdateReview(int reviewId, [FromBody] ReviewDTO reviewDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                int customerId = Convert.ToInt32(User.FindFirstValue(ClaimTypes.Name));
                var review = await _reviewService.UpdateReview(reviewId, reviewDTO, customerId);
                return Ok(review);
            }
            catch (ReviewNotFoundException ex)
            {
                return NotFound(new ErrorModel(404, ex.Message));
            }
            catch (UnauthorizedAccessException ex)
            {
                return StatusCode(StatusCodes.Status401Unauthorized, ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
        #endregion UpdateReview

        #region DeleteReview
        /// <summary>
        /// Delete a review by reviewId
        /// </summary>
        /// <param name="reviewId"></param>
        /// <returns></returns>
        [Authorize(Roles = "Customer")]
        [HttpDelete("{reviewId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteReview(int reviewId)
        {
            try
            {
              await _reviewService.DeleteReview(reviewId);
              return Ok();
            }
            catch (ReviewNotFoundException ex)
            {
                return NotFound(new ErrorModel(404, ex.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
        #endregion DeleteReview

        #region Filters

        #region GetAllProductReviewsByRating
        /// <summary>
        /// gets all the reviews that have the same ratingId
        /// </summary>
        /// <param name="rating"></param>
        /// <returns></returns>
        [HttpGet("rating/{rating}")]
        [ProducesResponseType(typeof(IEnumerable<Review>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAllProductReviewsByRating(int rating)
        {
            try
            {
                var reviews = await _reviewService.GetAllProductReviewsByRating(rating);
                return Ok(reviews);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ErrorModel(500, ex.Message));
            }
        }
        #endregion GetAllProductReviewsByRating

        #region GetReviewsGroupedByCustomer
        /// <summary>
        /// groups the reviews that have the same CustomerId
        /// </summary>
        /// <returns></returns>
        [HttpGet("customer-group")]
        [ProducesResponseType(typeof(IEnumerable<IGrouping<int, Review>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetReviewsGroupedByCustomer()
        {
            try
            {
                var reviews = await _reviewService.GetReviewsGroupedByCustomer();
                return Ok(reviews);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ErrorModel(500, ex.Message));
            }
        }
        #endregion GetReviewsGroupedByCustomer

        #region GetReviewsGroupedByRatingAndProduct
        /// <summary>
        /// groups the reviews that have the same Rating and ProductId
        /// </summary>
        /// <returns></returns>
        [HttpGet("rating-product-group")]
        [ProducesResponseType(typeof(IEnumerable<IGrouping<RatingProductKeyDTO, Review>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetReviewsGroupedByRatingAndProduct()
        {
            try
            {
                var reviews = await _reviewService.GetReviewsGroupedByRatingAndProduct();
                return Ok(reviews);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ErrorModel(500, ex.Message));
            }
        }
        #endregion GetReviewsGroupedByRatingAndProduct

        #region GetReviewsGroupedByDate
        /// <summary>
        /// groups the reviews that have the same Date
        /// </summary>
        /// <returns></returns>
        [HttpGet("date-group")]
        [ProducesResponseType(typeof(IEnumerable<IGrouping<DateTime, Review>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetReviewsGroupedByDate()
        {
            try
            {
                var reviews = await _reviewService.GetReviewsGroupedByDate();
                return Ok(reviews);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ErrorModel(500, ex.Message));
            }
        }
        #endregion GetReviewsGroupedByDate

        #region GetReviewsGroupedByRating
        /// <summary>
        /// groups the reviews that have the same Rating
        /// </summary>
        /// <returns></returns>
        [HttpGet("rating-group")]
        [ProducesResponseType(typeof(IEnumerable<IGrouping<DateTime, Review>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetReviewsGroupedByRating()
        {
            try
            {
                var reviews = await _reviewService.GetReviewsGroupedByRating();
                return Ok(reviews);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ErrorModel(500, ex.Message));
            }
        }
        #endregion GetReviewsGroupedByRating

        #endregion Filters
    }
}
