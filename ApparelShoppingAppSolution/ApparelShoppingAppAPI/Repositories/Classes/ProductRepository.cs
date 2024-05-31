
using Microsoft.EntityFrameworkCore;
using ApparelShoppingAppAPI.Contexts;
using ApparelShoppingAppAPI.Exceptions;
using ApparelShoppingAppAPI.Models.DB_Models;
using ApparelShoppingAppAPI.Repositories.Interfaces;
using ApparelShoppingAppAPI.Models.DTO_Models;
using System.Xml.Linq;
using System.Globalization;

namespace ApparelShoppingAppAPI.Repositories.Classes
{
    public class ProductRepository: BaseRepository<int, Product>, IProductRepository
    {
        public ProductRepository(ShoppingAppDbContext context) : base(context)
        {
        }
        #region GetById
        public override async Task<Product> GetById(int id)
        {
            var product = await _context.Products
                .Include(p => p.Category)
                .FirstOrDefaultAsync(p => p.ProductId == id);
            if (product == null)
            {
                throw new ProductNotFoundException("Product not found");
            }
            return product;
        }
        #endregion GetById

        #region GetAll
        /// <summary>
        /// Get all products
        /// </summary>
        /// <returns>List<Product></returns>
        public override async Task<IEnumerable<Product>> GetAll()
        {
            var products = await _context.Products
                .Include(p => p.Category)
                .ToListAsync();
            return products;
        }
        #endregion GetAll

        #region AddProductWithCategoryTransaction
        /// <summary>
        /// Add product with category transaction
        /// </summary>
        /// <param name="productDto"></param>
        /// <param name="sellerId"></param>
        /// <returns></returns>
        /// <exception cref="Exception">Product</exception>
        public async Task<Product> AddProductWithCategoryTransaction(ProductDTO productDto, int sellerId)
        {
            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    var category = await _context.Categories
                    .FirstOrDefaultAsync(c => c.Name.ToUpper() == productDto.CategoryName.ToUpper());
                    if(category == null)
                    {
                        category = new Category
                        {
                            Name = CultureInfo.CurrentCulture
                            .TextInfo
                            .ToTitleCase(productDto.CategoryName.ToLower())
                        };

                        await _context.Categories.AddAsync(category);
                        await _context.SaveChangesAsync();
                    }
                    Product product = new Product
                    {
                        Name = productDto.Name,
                        Price = productDto.Price,
                        Description = productDto.Description,
                        Quantity = productDto.Quantity,
                        SellerId = sellerId,
                        CategoryId = category.CategoryId,
                        ImageUrl = productDto.ImageUrl
                    };
                    await _context.Products.AddAsync(product);
                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();
                    return product;
                }
                catch (Exception)
                {
                    await transaction.RollbackAsync();
                    throw new Exception("Error while adding product");
                }
            }
        }
        #endregion AddProductWithCategoryTransaction

        #region UpdateProductWithCategoryTransaction
        /// <summary>
        /// Update product with category transaction
        /// </summary>
        /// <param name="productId"></param>
        /// <param name="productDTO"></param>
        /// <returns>Product</returns>
        /// <exception cref="ProductNotFoundException"></exception>
        public async Task<Product> UpdateProductWithCategoryTransaction(int productId, ProductDTO productDTO)
        {
            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    Product product = await base.GetById(productId);
                    if (product == null)
                    {
                        throw new ProductNotFoundException("Product not found");
                    }
                    var category = await _context.Categories
                        .FirstOrDefaultAsync(c => c.Name.ToUpper() == productDTO.CategoryName.ToUpper());

                    if (category == null)
                    {
                        category = new Category
                        {
                            Name = CultureInfo.CurrentCulture
                            .TextInfo
                            .ToTitleCase(productDTO.CategoryName.ToLower())
                        };

                        await _context.Categories.AddAsync(category);
                        await _context.SaveChangesAsync();
                    }

                    product.Name = productDTO.Name;
                    product.Description = productDTO.Description;
                    product.Quantity = productDTO.Quantity;
                    product.Price = productDTO.Price;
                    product.ImageUrl = productDTO.ImageUrl;
                    product.CategoryId = category.CategoryId;

                    _context.Entry(product).State = EntityState.Modified;
                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();
                    return product;

                }
                catch (Exception)
                {
                    await transaction.RollbackAsync();
                    throw;
                }
            }
        }
        #endregion UpdateProductWithCategoryTransaction

        #region GetProductByName
        /// <summary>
        /// Get Product By Name
        /// </summary>
        /// <param name="name"></param>
        /// <returns>Product</returns>
        /// <exception cref="ProductNotFoundException"></exception>
        public async Task<Product> GetProductByName(string name)
        {
            var product = await _context.Products
            .FirstOrDefaultAsync(p => p.Name.ToUpper() == name.ToUpper());
            if (product == null)
            {
                throw new ProductNotFoundException($"{name} not found.");
            }
            return product;
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
            int? categoryId = null,
            decimal? minPrice = null,
            decimal? maxPrice = null,
            bool? availability = null,
            double? minRating = null,
            double? maxRating = null,
            int? sellerId = null)
        {
            var query = _context.Products.Include(p => p.Reviews).AsQueryable();

            if (categoryId.HasValue)
            {
                query = query.Where(p => p.CategoryId == categoryId.Value);
            }

            if (minPrice.HasValue)
            {
                query = query.Where(p => p.Price >= minPrice.Value);
            }

            if (maxPrice.HasValue)
            {
                query = query.Where(p => p.Price <= maxPrice.Value);
            }

            if (availability.HasValue)
            {
                query = query.Where(p => (availability.Value && p.Quantity > 0) || (!availability.Value && p.Quantity == 0));
            }

            if (minRating.HasValue)
            {
                query = query.Where(p => p.Reviews.Average(r => r.Rating) >= minRating.Value);
            }
            if (maxRating.HasValue)
            {
                query = query.Where(p => p.Reviews.Average(r => r.Rating) <= maxRating.Value);
            }
            if (maxRating.HasValue)
            {
                query = query.Where(p => p.Reviews.Average(r => r.Rating) <= maxRating.Value);
            }

            if (sellerId.HasValue)
            {
                query = query.Where(p => p.SellerId == sellerId.Value);
            }

            return await query.ToListAsync();
        }
        #endregion GetFilteredProducts


    }
}
