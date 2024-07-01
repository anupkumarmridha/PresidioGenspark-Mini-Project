using ApparelShoppingAppAPI.Models.DB_Models;
using ApparelShoppingAppAPI.Models.DTO_Models;

namespace ApparelShoppingAppAPI.Services.Interfaces
{
    public interface IProductService
    {
        Task<IEnumerable<Product>> GetAllProducts();
        Task<Product> GetProductById(int id);
        Task<Product> GetProductByName(string name);
        Task<Product> AddProduct(ProductDTO product, int sellerId);
        Task<Product> UpdateProduct(int id, ProductDTO product);
        Task<Product> DeleteProduct(int id);
        Task<IEnumerable<Category>> GetAllCategories();
        Task<IEnumerable<Product>> GetFilteredProducts(
            
            int? categoryId,
            
            decimal? minPrice,
            
            decimal? maxPrice,
            
            bool? availability,
            
            double? minRating,
            double? maxRating,
            int? sellerId
            
            );
    }
}
