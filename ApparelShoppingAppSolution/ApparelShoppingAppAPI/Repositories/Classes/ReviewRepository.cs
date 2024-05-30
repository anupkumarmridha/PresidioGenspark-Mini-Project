using ApparelShoppingAppAPI.Contexts;
using ApparelShoppingAppAPI.Models.DB_Models;
using ApparelShoppingAppAPI.Models.DTO_Models;
using ApparelShoppingAppAPI.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ApparelShoppingAppAPI.Repositories.Classes
{
    public class ReviewRepository:BaseRepository<int, Review>, IReviewRepository
    {
        public ReviewRepository(ShoppingAppDbContext context) : base(context)
        {
        }

        #region GetByProductId
        public async Task<IEnumerable<Review>> GetByProductId(int productId)
        {
            return await _context.Reviews.Where(r => r.ProductId == productId).ToListAsync();
        }
        #endregion GetByProductId

        #region GetByCustomerId
        public async Task<IEnumerable<Review>> GetByCustomerId(int customerId)
        {
            return await _context.Reviews.Where(r => r.CustomerId == customerId).ToListAsync();
        }
        #endregion GetByCustomerId

        #region GetAllProductReviewsByRating
        /// <summary>
        /// gets all the reviews that have the same ratingId
        /// </summary>
        /// <param name="rating"></param>
        /// <returns></returns>
        public async Task<IEnumerable<Review>> GetAllProductReviewsByRating(int rating)
        {
            return await _context.Reviews.Where(r => r.Rating == rating).ToListAsync();
        }
        #endregion GetAllProductReviewsByRating

        #region GetReviewsGroupedByCustomer
        /// <summary>
        /// groups the reviews that have the same CustomerId
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<IGrouping<int, Review>>> GetReviewsGroupedByCustomer()
        {
            var reviews = await _context.Reviews.ToListAsync();

            return reviews.GroupBy(r => r.CustomerId);
        }
        #endregion GetReviewsGroupedByCustomer

        #region GetReviewsGroupedByRatingAndProduct
        /// <summary>
        /// groups the reviews that have the same Rating and ProductId
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<IGrouping<RatingProductKeyDTO, Review>>> GetReviewsGroupedByRatingAndProduct()
        {
            var reviews = await _context.Reviews.ToListAsync();
            return reviews.GroupBy(r => new RatingProductKeyDTO { Rating = r.Rating, ProductId = r.ProductId });
        }
        #endregion GetReviewsGroupedByRatingAndProduct

        #region GetReviewsGroupedByDate
        /// <summary>
        /// groups the reviews that have the same Date
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<IGrouping<DateTime, Review>>> GetReviewsGroupedByDate()
        {
            var reviews = await _context.Reviews.ToListAsync();

            return reviews.GroupBy(r => r.UpdateDate.Date);
        }
        #endregion GetReviewsGroupedByDate

        #region GetReviewsGroupedByRating
        /// <summary>
        /// groups the reviews that have the same Rating
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<IGrouping<int, Review>>> GetReviewsGroupedByRating()
        {
            var reviews = await _context.Reviews.ToListAsync();

            return reviews.GroupBy(r => r.Rating);
        }
        #endregion GetReviewsGroupedByRating

    }
}
