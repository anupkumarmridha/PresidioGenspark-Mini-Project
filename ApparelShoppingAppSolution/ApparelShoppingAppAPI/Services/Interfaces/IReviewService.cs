using ApparelShoppingAppAPI.Models.DB_Models;
using ApparelShoppingAppAPI.Models.DTO_Models;

namespace ApparelShoppingAppAPI.Services.Interfaces
{
    public interface IReviewService
    {
        Task<Review> AddReview(ReviewDTO reviewDTO, int customerId);
        Task<Review> GetReviewById(int reviewId);
        Task<IEnumerable<Review>> GetReviewsByProductId(int productId);
        Task<IEnumerable<Review>> GetReviewsByCustomerId(int customerId);
        Task<Review> UpdateReview(int reviewId, ReviewDTO reviewDTO, int customerId);
        Task DeleteReview(int reviewId);

        //Filters
        Task<IEnumerable<Review>> GetAllProductReviewsByRating(int rating);
        Task<IEnumerable<IGrouping<int, Review>>> GetReviewsGroupedByCustomer();
        Task<IEnumerable<IGrouping<RatingProductKeyDTO, Review>>> GetReviewsGroupedByRatingAndProduct();
        Task<IEnumerable<IGrouping<DateTime, Review>>> GetReviewsGroupedByDate();
        Task<IEnumerable<IGrouping<int, Review>>> GetReviewsGroupedByRating();
    }
}
