using ApparelShoppingAppAPI.Exceptions;
using ApparelShoppingAppAPI.Models.DB_Models;
using ApparelShoppingAppAPI.Models.DTO_Models;
using ApparelShoppingAppAPI.Repositories.Interfaces;
using ApparelShoppingAppAPI.Services.Interfaces;

namespace ApparelShoppingAppAPI.Services.Classes
{
    public class ProductService:IProductService
    {
        private readonly IProductRepository _productRepository;

        public ProductService(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        #region GetProductById
        public async Task<Product> GetProductById(int id)
        {
            try
            {
                var product = await _productRepository.GetById(id);
                if(product == null)
                {
                    throw new ProductNotFoundException("Product not found");
                }
                return product;

            }
            catch (ProductNotFoundException)
            {
                throw;
            }catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        #endregion GetProductById

        #region GetAllProducts
        public async Task<IEnumerable<Product>> GetAllProducts()
        {
            try
            {

                return await _productRepository.GetAll();
            }
            catch (ProductNotFoundException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        #endregion GetAllProducts

        #region AddProduct
        /// <summary>
        /// Add a product to the database
        /// </summary>
        /// <param name="product"></param>
        /// <param name="sellerId"></param>
        /// <returns>A Saved Product</returns>
        /// <exception cref="Exception"></exception>
        public async Task<Product> AddProduct(ProductDTO product, int sellerId)
        {
            try
            {
                return await _productRepository.AddProductWithCategoryTransaction(product, sellerId);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
          
        }
        #endregion AddProduct

        #region UpdateProduct
        public async Task<Product> UpdateProduct(int id, ProductDTO product)
        {
            try
            {
                return await _productRepository.UpdateProductWithCategoryTransaction(id,product);
            }
            catch (ProductNotFoundException)
            {
                throw;
            }
            catch (Exception ex) { 
                throw new Exception(ex.Message);
            }  
        }
        #endregion UpdateProduct

        #region DeleteProduct
        /// <summary>
        /// Delete a product from the database
        /// </summary>
        /// <param name="id">string</param>
        /// <returns></returns>
        /// <exception cref="ProductNotFoundException"></exception>
        /// <exception cref="Exception">Deleted Product</exception>
        public async Task<Product> DeleteProduct(int id)
        {
            try
            {
                return await _productRepository.Delete(id);              
            }
            catch (InvalidOperationException)
            {
                throw new ProductNotFoundException("Product not found");
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        #endregion DeleteProduct

        #region GetProductByName
        /// <summary>
        /// Get product by name
        /// </summary>
        /// <param name="name"></param>
        /// <returns>Product</returns>
        /// <exception cref="Exception"></exception>
        public async Task<Product> GetProductByName(string name)
        {
            try
            {
                return await _productRepository.GetProductByName(name);
            }
            catch (ProductNotFoundException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        #endregion GetProductByName

        #region GetFilteredProducts
        /// <summary>
        /// Get Filtered Products
        /// </summary>
        /// <param name="categoryId"></param>
        /// <param name="minPrice"></param>
        /// <param name="maxPrice"></param>
        /// <param name="availability"></param>
        /// <param name="minRating"></param>
        /// <param name="maxRating"></param>
        /// <param name="sellerId"></param>
        /// <returns></returns>
        public async Task<IEnumerable<Product>> GetFilteredProducts(
            
            int? categoryId,
            
            decimal? minPrice,
            
            decimal? maxPrice,
            
            bool? availability,
            
            double? minRating,
            double? maxRating,
            int? sellerId
            
            )
        {
            return await _productRepository.GetFilteredProducts(categoryId, minPrice, maxPrice, availability, minRating, maxRating, sellerId);
        }
        #endregion GetFilteredProducts
    }
}
