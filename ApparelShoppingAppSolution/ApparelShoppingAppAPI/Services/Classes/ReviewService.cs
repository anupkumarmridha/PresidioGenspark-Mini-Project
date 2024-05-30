using ApparelShoppingAppAPI.Exceptions;
using ApparelShoppingAppAPI.Models.DB_Models;
using ApparelShoppingAppAPI.Models.DTO_Models;
using ApparelShoppingAppAPI.Repositories.Interfaces;
using ApparelShoppingAppAPI.Services.Interfaces;

namespace ApparelShoppingAppAPI.Services.Classes
{
    public class ReviewService : IReviewService
    {
        private readonly IReviewRepository _reviewRepository;
        private readonly IProductRepository _productRepository;
        private readonly ICustomerRepository _customerRepository;

       
        public ReviewService(IReviewRepository reviewRepository, IProductRepository productRepository, ICustomerRepository customerRepository)
        {
            _reviewRepository = reviewRepository;
            _productRepository = productRepository;
            _customerRepository = customerRepository;
        }

        #region AddReview
        public async Task<Review> AddReview(ReviewDTO reviewDTO, int customerId)
        {
            try
            {
                var product = await _productRepository.GetById(reviewDTO.ProductId);
                if (product == null)
                {
                    throw new ProductNotFoundException("Product not found");
                }

                var customer = await _customerRepository.GetById(customerId);
                if (customer == null)
                {
                    throw new CustomerNotFoundException("Customer not found");
                }

                var review = new Review
                {
                    ProductId = reviewDTO.ProductId,
                    CustomerId = customerId,
                    Rating = reviewDTO.Rating,
                    Comment = reviewDTO.Comment,
                };

                return await _reviewRepository.Add(review);
            }
            catch (ProductNotFoundException)
            {
                throw;
            }
            catch (CustomerNotFoundException)
            {
                throw;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }
        #endregion AddReview

        #region GetReviewById
        /// <summary>
        /// Get review by reviewId
        /// </summary>
        /// <param name="reviewId"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task<Review> GetReviewById(int reviewId)
        {
            try
            {
                return await _reviewRepository.GetById(reviewId);
            }
            catch(InvalidOperationException )
            {
                throw new ReviewNotFoundException("Review Not Found");
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }
        #endregion GetReviewById

        #region GetReviewsByProductId
        public async Task<IEnumerable<Review>> GetReviewsByProductId(int productId)
        {
            return await _reviewRepository.GetByProductId(productId);
        }
        #endregion GetReviewsByProductId

        #region GetReviewsByCustomerId
        public async Task<IEnumerable<Review>> GetReviewsByCustomerId(int customerId)
        {
            return await _reviewRepository.GetByCustomerId(customerId);
        }
        #endregion GetReviewsByCustomerId

        #region UpdateReview
        public async Task<Review> UpdateReview(int reviewId, ReviewDTO reviewDTO, int customerId)
        {
            try
            {
                var review = await _reviewRepository.GetById(reviewId);
                if (review == null)
                {
                    throw new ReviewNotFoundException("Review not found");
                }
                if (review.CustomerId != customerId)
                {
                    throw new UnauthorizedAccessException("You are not authorized to update this review");
                }

                review.Rating = reviewDTO.Rating;
                review.Comment = reviewDTO.Comment;
                review.UpdateDate = DateTime.Now;

                return await _reviewRepository.Update(review);
            }
            catch(ReviewNotFoundException)
            {
                throw;
            }
            catch (UnauthorizedAccessException)
            {
                throw;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            
        }
        #endregion UpdateReview

        #region DeleteReview
        public async Task DeleteReview(int reviewId)
        {
            try
            {
                await _reviewRepository.Delete(reviewId);
            }
            catch (InvalidOperationException ex)
            {
                throw new ReviewNotFoundException(ex.Message);
            }
            catch(Exception e)
            {
                throw new Exception(e.Message);
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
        public async Task<IEnumerable<Review>> GetAllProductReviewsByRating(int rating)
        {
            try
            {
                return await _reviewRepository.GetAllProductReviewsByRating(rating);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }
        #endregion GetAllProductReviewsByRating

        #region GetReviewsGroupedByCustomer
        /// <summary>
        /// groups the reviews that have the same CustomerId
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<IGrouping<int, Review>>> GetReviewsGroupedByCustomer()
        {
            try
            {
                return await _reviewRepository.GetReviewsGroupedByCustomer();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }
        #endregion GetReviewsGroupedByCustomer

        #region GetReviewsGroupedByRatingAndProduct
        /// <summary>
        /// groups the reviews that have the same Rating and ProductId
        /// </summary>
        /// <returns></returns>
        public async  Task<IEnumerable<IGrouping<RatingProductKeyDTO, Review>>> GetReviewsGroupedByRatingAndProduct()
        {
            try
            {
                return await _reviewRepository.GetReviewsGroupedByRatingAndProduct();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }
        #endregion GetReviewsGroupedByRatingAndProduct

        #region GetReviewsGroupedByDate
        /// <summary>
        /// groups the reviews that have the same Date
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<IGrouping<DateTime, Review>>> GetReviewsGroupedByDate()
        {
            try
            {
                return await _reviewRepository.GetReviewsGroupedByDate();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }
        #endregion GetReviewsGroupedByDate

        #region GetReviewsGroupedByRating
        /// <summary>
        /// groups the reviews that have the same Rating
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<IGrouping<int, Review>>> GetReviewsGroupedByRating()
        {
            try
            {
                return await _reviewRepository.GetReviewsGroupedByRating();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }
        #endregion GetReviewsGroupedByRating

        #endregion Filters

    }
}
