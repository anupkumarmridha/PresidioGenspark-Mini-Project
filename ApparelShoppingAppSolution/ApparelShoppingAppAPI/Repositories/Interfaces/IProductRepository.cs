using ApparelShoppingAppAPI.Models.DB_Models;
using ApparelShoppingAppAPI.Models.DTO_Models;

namespace ApparelShoppingAppAPI.Repositories.Interfaces
{
    public interface IProductRepository:IRepository<int, Product>
    {
        public Task<Product> AddProductWithCategoryTransaction(ProductDTO productDto, int sellerId);
        public Task<Product> UpdateProductWithCategoryTransaction(int productId, ProductDTO productDto);
        public Task<Product> GetProductByName(string name);
    }
}
