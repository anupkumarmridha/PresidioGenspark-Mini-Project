using ApparelShoppingAppAPI.Contexts;
using ApparelShoppingAppAPI.Exceptions;
using ApparelShoppingAppAPI.Models.DB_Models;
using ApparelShoppingAppAPI.Models.DTO_Models;
using ApparelShoppingAppAPI.Repositories.Classes;
using ApparelShoppingAppAPI.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApparelShoppingAppAPITest.RepositoriesTest
{
    public class ProductRepositoryTests
    {
        private ShoppingAppDbContext _context;
        private ProductRepository _productRepository;
        private CategoryRepository _categoryRepository;
        private SellerRepository _sellerRepository;
        private UserRegisterRepository _userRegisterRepository;
        private Category _category;
        private Seller _seller;
        private User _user;
        
        #region Setup
        [SetUp]
        public async Task Setup()
        {
            var options = new DbContextOptionsBuilder<ShoppingAppDbContext>()
                .UseSqlite("DataSource=:memory:")
                .Options;

            _context = new ShoppingAppDbContext(options);
            _context.Database.OpenConnection();
            _context.Database.EnsureCreated();

            _sellerRepository = new SellerRepository(_context);
            _productRepository = new ProductRepository(_context);
            _categoryRepository = new CategoryRepository(_context);
            _userRegisterRepository = new UserRegisterRepository(_context);

            var userRegisterDTO = new UserRegisterRepositoryDTO
            {
                Name = "Test Seller",
                Email = "seller@test.com",
                Phone = "0987654321",
                Password = Encoding.UTF8.GetBytes("password"),
                PasswordHashKey = Encoding.UTF8.GetBytes("hashkey")
            };

            var (seller, user) = await _userRegisterRepository.AddSellerUserWithTransaction(userRegisterDTO);

            _seller = seller;
            _user = user;

            var category = new Category
            {
                Name = "Test",
            };
            var categoryResult = await _categoryRepository.Add(category);
            _category = categoryResult;
        }

        [TearDown]
        public void TearDown()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }

        #endregion Setup

        [Test]
        public async Task Add_Product_ReturnsAddedProduct()
        {
            // Arrange
            var product = new Product
            {
                ProductId = 1,
                Name = "Sample Product",
                Description = "This is a test product description.",
                Quantity = 10,
                Price = 29.99m,
                CategoryId = _category.CategoryId,
                SellerId = _seller.SellerId,
                CreationDate = DateTime.Now,
                LastUpdatedDate = DateTime.Now
            };

            // Act
            var result = await _productRepository.Add(product);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(product.ProductId, result.ProductId);
        }
      
        [Test]
        public async Task GetById_ProductExists_ReturnsProduct()
        {
            // Arrange
            var product = new Product
            {
                ProductId = 1,
                Name = "Sample Product",
                Description = "This is a test product description.",
                Quantity = 10,
                Price = 29.99m,
                CategoryId = _category.CategoryId,
                SellerId = _seller.SellerId,
                CreationDate = DateTime.Now,
                LastUpdatedDate = DateTime.Now
            };


            var res =await _productRepository.Add(product);
            await Console.Out.WriteLineAsync((res.ProductId).ToString());
            // Act
            var result = await _productRepository.GetById(product.ProductId);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(product.ProductId, result.ProductId);
        }

        [Test]
        public async Task GetById_ProductDoesNotExist_ThrowsProductNotFoundException()
        {
            var ex = Assert.ThrowsAsync<ProductNotFoundException>(() => _productRepository.GetById(1));
            Assert.AreEqual("Product not found", ex.Message);
        }

        [Test]
        public async Task GetAll_ReturnsAllProductes()
        {
            // Arrange
            var product1 = new Product
            {
                ProductId = 1,
                Name = "Sample Product",
                Description = "This is a test product description.",
                Quantity = 10,
                Price = 29.99m,
                CategoryId = _category.CategoryId,
                SellerId = _seller.SellerId,
                CreationDate = DateTime.Now,
                LastUpdatedDate = DateTime.Now
            };

            var product2 = new Product
            {
                ProductId = 2,
                Name = "Sample Product",
                Description = "This is a test product description.",
                Quantity = 10,
                Price = 29.99m,
                CategoryId = _category.CategoryId,
                SellerId = _seller.SellerId,
                CreationDate = DateTime.Now,
                LastUpdatedDate = DateTime.Now
            };
            await _productRepository.Add(product1);
            await _productRepository.Add(product2);

            // Act
            var result = await _productRepository.GetAll();

            // Assert
            Assert.AreEqual(2, result.Count());
        }

        [Test]
        public async Task Update_Product_ReturnsUpdatedProduct()
        {
            // Arrange
            var product = new Product
            {
                ProductId = 1,
                Name = "Sample Product",
                Description = "This is a test product description.",
                Quantity = 10,
                Price = 29.99m,
                CategoryId = _category.CategoryId,
                SellerId = _seller.SellerId,
                CreationDate = DateTime.Now,
                LastUpdatedDate = DateTime.Now
            };
            await _productRepository.Add(product);

            product.Quantity = 30;

            // Act
            var result = await _productRepository.Update(product);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(30, result.Quantity);
        }

        [Test]
        public async Task Delete_ProductExists_ReturnsDeletedProduct()
        {
            // Arrange

            var product = new Product
            {
                ProductId = 1,
                Name = "Sample Product",
                Description = "This is a test product description.",
                Quantity = 10,
                Price = 29.99m,
                CategoryId = _category.CategoryId,
                SellerId = _seller.SellerId,
                CreationDate = DateTime.Now,
                LastUpdatedDate = DateTime.Now
            };
            await _productRepository.Add(product);

            // Act
            var result = await _productRepository.Delete(1);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(product.ProductId, result.ProductId);
        }

        [Test]
        public void Delete_ProductDoesNotExist_ThrowsInvalidOperationException()
        {
            // Act & Assert
            var ex = Assert.ThrowsAsync<InvalidOperationException>(() => _productRepository.Delete(1));
            Assert.AreEqual("1 not found.", ex.Message);
        }

        [Test]
        public async Task AddProductWithCategoryTransaction_ReturnsProduct()
        {
            // Arrange
            var productDTO = new ProductDTO
            {
                Name = "Sample Product",
                Description = "This is a test product description.",
                Quantity = 10,
                Price = 29.99m,
                CategoryName = "Test Category"
            };

            // Act
            var result = await _productRepository.AddProductWithCategoryTransaction(productDTO, _seller.SellerId);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(productDTO.Name, result.Name);
        }

        [Test]
        public async Task AddProductWithCategoryTransaction_CategoryExists_ReturnsProduct()
        {
            // Arrange
            var category = new Category
            {
                Name = "Test Category"
            };
            await _categoryRepository.Add(category);

            var productDTO = new ProductDTO
            {
                Name = "Sample Product",
                Description = "This is a test product description.",
                Quantity = 10,
                Price = 29.99m,
                CategoryName = "Test Category"
            };

            // Act
            var result = await _productRepository.AddProductWithCategoryTransaction(productDTO, _seller.SellerId);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(productDTO.Name, result.Name);
        }
        [Test]
        public async Task AddProductWithCategoryTransaction_CategoryDoesNotExist_ReturnsProduct()
        {
            // Arrange
            var productDTO = new ProductDTO
            {
                Name = "Sample Product",
                Description = "This is a test product description.",
                Quantity = 10,
                Price = 29.99m,
                CategoryName = "Test Category"
            };

            // Act
            var result = await _productRepository.AddProductWithCategoryTransaction(productDTO, _seller.SellerId);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(productDTO.Name, result.Name);
        }

        [Test]
        public async Task UpdateProductWithCategoryTransaction_ReturnsProduct()
        {
            // Arrange
            var productDTO = new ProductDTO
            {
                Name = "Sample Product",
                Description = "This is a test product description.",
                Quantity = 10,
                Price = 29.99m,
                CategoryName = "Test Category"
            };

            // Act
            var product = await _productRepository.AddProductWithCategoryTransaction(productDTO, _seller.SellerId);

            productDTO.CategoryName= "Updated Category";

            var result = await _productRepository.UpdateProductWithCategoryTransaction(product.ProductId, productDTO);
            Assert.IsNotNull(result);
            Assert.AreEqual(productDTO.CategoryName, result.Category.Name);
        }

        [Test]
        public async Task UpdateProductWithCategoryTransaction_CategoryExists_ReturnsProduct()
        {
            // Arrange
            var category = new Category
            {
                Name = "Test Category"
            };
            await _categoryRepository.Add(category);

            var productDTO = new ProductDTO
            {
                Name = "Sample Product",
                Description = "This is a test product description.",
                Quantity = 10,
                Price = 29.99m,
                CategoryName = "Test Category"
            };

            // Act
            var product = await _productRepository.AddProductWithCategoryTransaction(productDTO, _seller.SellerId);

            productDTO.CategoryName = "Updated Category";

            var result = await _productRepository.UpdateProductWithCategoryTransaction(product.ProductId, productDTO);
            Assert.IsNotNull(result);
            Assert.AreEqual(productDTO.CategoryName, result.Category.Name);
        }

        [Test]
        public async Task UpdateProductWithCategoryTransaction_CategoryDoesNotExist_ReturnsProduct()
        {
            // Arrange
            var productDTO = new ProductDTO
            {
                Name = "Sample Product",
                Description = "This is a test product description.",
                Quantity = 10,
                Price = 29.99m,
                CategoryName = "Test Category"
            };

            // Act
            var product = await _productRepository.AddProductWithCategoryTransaction(productDTO, _seller.SellerId);

            productDTO.CategoryName = "Updated Category";

            var result = await _productRepository.UpdateProductWithCategoryTransaction(product.ProductId, productDTO);
            Assert.IsNotNull(result);
            Assert.AreEqual(productDTO.CategoryName, result.Category.Name);
        }

        [Test]
        public async Task UpdateProductWithCategoryTransaction_ProductDoesNotExist_ThrowsProductNotFoundException()
        {
            // Arrange
            var productDTO = new ProductDTO
            {
                Name = "Sample Product",
                Description = "This is a test product description.",
                Quantity = 10,
                Price = 29.99m,
                CategoryName = "Test Category"
            };

            // Act & Assert
            var ex = Assert.ThrowsAsync<ProductNotFoundException>(() => _productRepository.UpdateProductWithCategoryTransaction(1, productDTO));
            Assert.AreEqual("Product not found", ex.Message);
        }

        [Test]
        public async Task GetProductByName_ReturnsProduct()
        {
            // Arrange
            var productDTO = new ProductDTO
            {
                Name = "Sample Product",
                Description = "This is a test product description.",
                Quantity = 10,
                Price = 29.99m,
                CategoryName = "Test Category"
            };

            // Act
            var result = await _productRepository.AddProductWithCategoryTransaction(productDTO, _seller.SellerId);
            Assert.IsNotNull(result);

            var product = await _productRepository.GetProductByName("Sample Product");
            Assert.IsNotNull(product);
            Assert.AreEqual("Sample Product", product.Name);
        }

        [Test]
        public async Task GetProductByName_ProductDoesNotExist_ThrowsProductNotFoundException()
        {
            // Arrange
            var productDTO = new ProductDTO
            {
                Name = "Sample Product",
                Description = "This is a test product description.",
                Quantity = 10,
                Price = 29.99m,
                CategoryName = "Test Category"
            };

            // Act
            var result = await _productRepository.AddProductWithCategoryTransaction(productDTO, _seller.SellerId);
            Assert.IsNotNull(result);

            // Act & Assert
            var ex = Assert.ThrowsAsync<ProductNotFoundException>(() => _productRepository.GetProductByName("Test Product"));
            Assert.AreEqual("Test Product not found.", ex.Message);
        }

        [Test]
        public async Task GetProductsByCategory_ReturnsListOfProducts()
        {
            // Arrange
            var productDTO = new ProductDTO
            {
                Name = "Sample Product",
                Description = "This is a test product description.",
                Quantity = 10,
                Price = 29.99m,
                CategoryName = "Test Category"
            };

            // Act
            var result = await _productRepository.AddProductWithCategoryTransaction(productDTO, _seller.SellerId);
            Assert.IsNotNull(result);

            var product = await _productRepository.GetFilteredProducts(categoryId: result.CategoryId);
            Assert.IsNotNull(product);
            Assert.AreEqual("Test Category", product.First().Category.Name);
        }
        
        [Test]
        public async Task GetProductsByCategory_ReturnsEmptyListOfProductsWhenCategoryDoesNotExist()
        {
            // Arrange
            var productDTO = new ProductDTO
            {
                Name = "Sample Product",
                Description = "This is a test product description.",
                Quantity = 10,
                Price = 29.99m,
                CategoryName = "Test Category"
            };

            // Act
            var result = await _productRepository.AddProductWithCategoryTransaction(productDTO, _seller.SellerId);

            var product = await _productRepository.GetFilteredProducts(categoryId: 2);
            Assert.IsNotNull(product);
        }

        [Test]
        public async Task GetProductsByMinPrice_ReturnsListOfProducts()
        {
            // Arrange
            var productDTO = new ProductDTO
            {
                Name = "Sample Product",
                Description = "This is a test product description.",
                Quantity = 10,
                Price = 29.99m,
                CategoryName = "Test Category"
            };

            // Act
            var result = await _productRepository.AddProductWithCategoryTransaction(productDTO, _seller.SellerId);
            Assert.IsNotNull(result);

            var product = await _productRepository.GetFilteredProducts(minPrice: 29.99m);
            Assert.IsNotNull(product);
            Assert.AreEqual(1, product.Count());
        }

        [Test]
        public async Task GetProductsByMaxPrice_ReturnsListOfProducts()
        {
            // Arrange
            var productDTO = new ProductDTO
            {
                Name = "Sample Product",
                Description = "This is a test product description.",
                Quantity = 10,
                Price = 29.99m,
                CategoryName = "Test Category"
            };

            // Act
            var result = await _productRepository.AddProductWithCategoryTransaction(productDTO, _seller.SellerId);
            Assert.IsNotNull(result);

            var product = await _productRepository.GetFilteredProducts(maxPrice: 29.99m);
            Assert.IsNotNull(product);
            Assert.AreEqual(1, product.Count());
        }

        [Test]
        public async Task GetProductsByMinAndMaxPrice_ReturnsListOfProducts()
        {
            // Arrange
            var productDTO = new ProductDTO
            {
                Name = "Sample Product",
                Description = "This is a test product description.",
                Quantity = 10,
                Price = 29.99m,
                CategoryName = "Test Category"
            };

            // Act
            var result = await _productRepository.AddProductWithCategoryTransaction(productDTO, _seller.SellerId);
            Assert.IsNotNull(result);

            var product = await _productRepository.GetFilteredProducts(minPrice: 29.99m, maxPrice: 29.99m);
            Assert.IsNotNull(product);
            Assert.AreEqual(1, product.Count());
        }

        [Test]
        public async Task GetProductsByMinAndMaxPrice_ReturnsEmptyListOfProductsWhenPriceDoesNotExist()
        {
            // Arrange
            var productDTO = new ProductDTO
            {
                Name = "Sample Product",
                Description = "This is a test product description.",
                Quantity = 10,
                Price = 29.99m,
                CategoryName = "Test Category"
            };

            // Act
            var result = await _productRepository.AddProductWithCategoryTransaction(productDTO, _seller.SellerId);
            Assert.IsNotNull(result);

            var product = await _productRepository.GetFilteredProducts(minPrice: 30.00m, maxPrice: 30.00m);
            Assert.IsNotNull(product);
            Assert.AreEqual(0, product.Count());
        }

        [Test]
        public async Task GetProductsByAvailability_ReturnsListOfAvailableProducts()
        {
            // Arrange
            var productDTO = new ProductDTO
            {
                Name = "Sample Product",
                Description = "This is a test product description.",
                Quantity = 40,
                Price = 29.99m,
                CategoryName = "Test Category"
            };

            // Act
            var result = await _productRepository.AddProductWithCategoryTransaction(productDTO, _seller.SellerId);
            Assert.IsNotNull(result);

            var product = await _productRepository.GetFilteredProducts(availability: true);
            Assert.IsNotNull(product);
            Assert.AreEqual(1, product.Count());
        }

        [Test]
        public async Task GetProductsByAvailability_ReturnsListOfUnavailableProducts()
        {
            // Arrange
            var productDTO = new ProductDTO
            {
                Name = "Sample Product",
                Description = "This is a test product description.",
                Quantity = 0,
                Price = 29.99m,
                CategoryName = "Test Category"
            };

            // Act
            var result = await _productRepository.AddProductWithCategoryTransaction(productDTO, _seller.SellerId);
            Assert.IsNotNull(result);

            var product = await _productRepository.GetFilteredProducts(availability: true);
            Assert.IsNotNull(product);
            Assert.AreEqual(0, product.Count());
        }

        [Test]
        public async Task GetProductsByAvailability_ReturnsEmptyListOfProductsWhenNoProductsAvailable()
        {
            // Arrange
            var productDTO = new ProductDTO
            {
                Name = "Sample Product",
                Description = "This is a test product description.",
                Quantity = 0,
                Price = 29.99m,
                CategoryName = "Test Category"
            };

            // Act
            var result = await _productRepository.AddProductWithCategoryTransaction(productDTO, _seller.SellerId);
            Assert.IsNotNull(result);

            var product = await _productRepository.GetFilteredProducts(availability: true);
            Assert.IsNotNull(product);
            Assert.AreEqual(0, product.Count());
        }

        [Test]
        public async Task GetProductsByMinRating_ReturnsListOfProducts()
        {
            //Arrange
            var productDTO = new ProductDTO
            {
                Name = "Sample Product",
                Description = "This is a test product description.",
                Quantity = 40,
                Price = 29.99m,
                CategoryName = "Test Category"
            };

            //Act
            var result = await _productRepository.AddProductWithCategoryTransaction(productDTO, _seller.SellerId);
            Assert.IsNotNull(result);

            var product = await _productRepository.GetFilteredProducts(minRating: 0);
            Assert.IsNotNull(product);
            Assert.AreEqual(0, product.Count());
        }

        [Test]
        public async Task GetProductsByMaxRating_ReturnsListOfProducts()
        {
            //Arrange
            var productDTO = new ProductDTO
            {
                Name = "Sample Product",
                Description = "This is a test product description.",
                Quantity = 40,
                Price = 29.99m,
                CategoryName = "Test Category"
            };

            //Act
            var result = await _productRepository.AddProductWithCategoryTransaction(productDTO, _seller.SellerId);
            Assert.IsNotNull(result);

            var product = await _productRepository.GetFilteredProducts(maxRating: 5);
            Assert.IsNotNull(product);
            Assert.AreEqual(0, product.Count());
        }

        [Test]
        public async Task GetProductsByMinAndMaxRating_ReturnsListOfProducts()
        {
            //Arrange
            var productDTO = new ProductDTO
            {
                Name = "Sample Product",
                Description = "This is a test product description.",
                Quantity = 40,
                Price = 29.99m,
                CategoryName = "Test Category"
            };

            //Act
            var result = await _productRepository.AddProductWithCategoryTransaction(productDTO, _seller.SellerId);
            Assert.IsNotNull(result);

            var product = await _productRepository.GetFilteredProducts(minRating: 0, maxRating: 5);
            Assert.IsNotNull(product);
            Assert.AreEqual(0, product.Count());
        }

        [Test]
        public async Task GetProductsByMinAndMaxRating_ReturnsEmptyListOfProductsWhenRatingDoesNotExist()
        {
            //Arrange
            var productDTO = new ProductDTO
            {
                Name = "Sample Product",
                Description = "This is a test product description.",
                Quantity = 40,
                Price = 29.99m,
                CategoryName = "Test Category"
            };

            //Act
            var result = await _productRepository.AddProductWithCategoryTransaction(productDTO, _seller.SellerId);
            Assert.IsNotNull(result);

            var product = await _productRepository.GetFilteredProducts(minRating: 1, maxRating: 5);
            Assert.IsNotNull(product);
            Assert.AreEqual(0, product.Count());
        }

        [Test]
        public async Task GetProductsBySeller_ReturnsListOfProducts()
        {
            //Arrange
            var productDTO = new ProductDTO
            {
                Name = "Sample Product",
                Description = "This is a test product description.",
                Quantity = 40,
                Price = 29.99m,
                CategoryName = "Test Category"
            };

            //Act
            var result = await _productRepository.AddProductWithCategoryTransaction(productDTO, _seller.SellerId);
            Assert.IsNotNull(result);

            var product = await _productRepository.GetFilteredProducts(sellerId: _seller.SellerId);
            Assert.IsNotNull(product);
            Assert.AreEqual(1, product.Count());
        }

        [Test]
        public async Task GetProductsBySeller_ReturnsEmptyListOfProductsWhenSellerDoesNotExist()
        {
            //Arrange
            var productDTO = new ProductDTO
            {
                Name = "Sample Product",
                Description = "This is a test product description.",
                Quantity = 40,
                Price = 29.99m,
                CategoryName = "Test Category"
            };

            //Act
            var result = await _productRepository.AddProductWithCategoryTransaction(productDTO, _seller.SellerId);
            Assert.IsNotNull(result);

            var product = await _productRepository.GetFilteredProducts(sellerId: 2);
            Assert.IsNotNull(product);
            Assert.AreEqual(0, product.Count());
        }

        [Test]
        public async Task GetProductsBySellerAndCategory_ReturnsListOfProducts()
        {
            //Arrange
            var productDTO = new ProductDTO
            {
                Name = "Sample Product",
                Description = "This is a test product description.",
                Quantity = 40,
                Price = 29.99m,
                CategoryName = "Test Category"
            };

            //Act
            var result = await _productRepository.AddProductWithCategoryTransaction(productDTO, _seller.SellerId);
            Assert.IsNotNull(result);

            var product = await _productRepository.GetFilteredProducts(sellerId: _seller.SellerId, categoryId: result.CategoryId);
            Assert.IsNotNull(product);
            Assert.AreEqual(1, product.Count());
        }
        [Test]
        public async Task GetProductsBySellerAndCategory_ReturnsEmptyListOfProductsWhenSellerAndCategoryDoesNotExist()
        {
            //Arrange
            var productDTO = new ProductDTO
            {
                Name = "Sample Product",
                Description = "This is a test product description.",
                Quantity = 40,
                Price = 29.99m,
                CategoryName = "Test Category"
            };

            //Act
            var result = await _productRepository.AddProductWithCategoryTransaction(productDTO, _seller.SellerId);
            Assert.IsNotNull(result);

            var product = await _productRepository.GetFilteredProducts(sellerId: 2, categoryId: 2);
            Assert.IsNotNull(product);
            Assert.AreEqual(0, product.Count());
        }
    }
}
