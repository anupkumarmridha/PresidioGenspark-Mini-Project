using ApparelShoppingAppAPI.Models.DB_Models;
using ApparelShoppingAppAPI.Models.DTO_Models;

namespace ApparelShoppingAppAPI.Repositories.Interfaces
{
    public interface IReviewRepository : IRepository<int, Review>
    {
        Task<IEnumerable<Review>> GetByProductId(int productId);
        Task<IEnumerable<Review>> GetByCustomerId(int customerId);
        Task<IEnumerable<Review>> GetAllProductReviewsByRating(int rating);
        Task<IEnumerable<IGrouping<int, Review>>> GetReviewsGroupedByCustomer();
        Task<IEnumerable<IGrouping<RatingProductKeyDTO, Review>>> GetReviewsGroupedByRatingAndProduct();
        Task<IEnumerable<IGrouping<DateTime, Review>>> GetReviewsGroupedByDate();
        Task<IEnumerable<IGrouping<int, Review>>> GetReviewsGroupedByRating();

    }
}
