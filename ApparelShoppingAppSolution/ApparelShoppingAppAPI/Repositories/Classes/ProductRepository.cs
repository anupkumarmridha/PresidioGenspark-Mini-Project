
using Microsoft.EntityFrameworkCore;
using ApparelShoppingAppAPI.Contexts;
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

        public override async Task<Product> GetById(int id)
        {
            var product = await _context.Products
                .Include(p => p.Category)
                .FirstOrDefaultAsync(p => p.ProductId == id);
            return product;
        }

        public override async Task<IEnumerable<Product>> GetAll()
        {
            var products = await _context.Products
                .Include(p => p.Category)
                .ToListAsync();
            return products;
        }

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
        public async Task<Product> UpdateProductWithCategoryTransaction(int productId, ProductDTO productDTO)
        {
            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    Product product = await base.GetById(productId);
                    if (product == null)
                    {
                        throw new Exception("Product not found");
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
                    throw new Exception("Error while adding product");
                }
            }
        }
        public async Task<Product> GetProductByName(string name)
        {
            var product = await _context.Products
            .FirstOrDefaultAsync(p => p.Name.ToUpper() == name.ToUpper());
            if (product == null)
            {
                throw new InvalidOperationException($"{name} not found.");
            }
            return product;
        }
    }
}
