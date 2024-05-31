using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ApparelShoppingAppAPI.Models.DB_Models;
using ApparelShoppingAppAPI.Models.DTO_Models;
using ApparelShoppingAppAPI.Services.Interfaces;
using System.Security.Claims;
using ApparelShoppingAppAPI.Exceptions;

namespace ApparelShoppingAppAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IProductService _productService;
        private readonly ILogger<ProductController> _logger;
        public ProductController(IProductService productService, ILogger<ProductController> logger)
        {
            _productService = productService;
            _logger = logger;
        }

        #region GetProducts
        /// <summary>
        /// Get all products
        /// </summary>
        /// <returns>All Products</returns>
        [HttpGet]
        [ProducesResponseType(typeof(IList<Product>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<IList<Product>>> GetProducts()
        {
            try
            {
                var result = await _productService.GetAllProducts();
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return BadRequest(new ErrorModel(400, ex.Message));
            }
        }
        #endregion GetProducts

        #region GetProduct
        /// <summary>
        /// Get product by id or name
        /// </summary>
        /// <param name="idOrName"></param>
        /// <returns>A Product</returns>
        [HttpGet("{idOrName}")]
        [ProducesResponseType(typeof(Product), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<Product>> GetProduct(string idOrName)
        {
            try
            {
                Product result;
                if (int.TryParse(idOrName, out int id))
                {
                    result = await _productService.GetProductById(id);
                }
                else
                {
                    result = await _productService.GetProductByName(idOrName);
                }

                if (result == null)
                {
                    return NotFound(new ErrorModel(404, "Product not found."));
                }

                return Ok(result);
            }
            catch(ProductNotFoundException ex)
            {
                return NotFound(new ErrorModel(404, ex.Message));
            }

            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(500, new ErrorModel(500, "An error occurred while retrieving the product"));
            }
        }
        #endregion GetProduct

        #region AddProduct
        /// <summary>
        /// Add a product to the database
        /// </summary>
        /// <param name="productDTO"></param>
        /// <returns></returns>
        [Authorize(Roles = "Admin,Seller")]
        [HttpPost]
        [ProducesResponseType(typeof(Product), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<Product>> AddProduct(ProductDTO productDTO)
        {
            try
            {
                var Id = User.FindFirstValue(ClaimTypes.Name);
                if (Id == null)
                {
                    return BadRequest(new ErrorModel(400, "Invalid User"));
                }
                var sellerId = Convert.ToInt32(Id);


                var result = await _productService.AddProduct(productDTO, sellerId);
                return CreatedAtAction(nameof(GetProduct), new { idOrName = result.ProductId }, result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(500, new ErrorModel(500, "An error occurred while Adding Product"));
            }
        }
        #endregion AddProduct

        #region UpdateProduct
        /// <summary>
        /// Update a product
        /// </summary>
        /// <param name="id"></param>
        /// <param name="productDTO"></param>
        /// <returns>Updated Product</returns>
        [Authorize(Roles = "Admin,Seller")]
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(Product), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<Product>> UpdateProduct(int id, ProductDTO productDTO)
        {
            try
            {
                var result = await _productService.UpdateProduct(id, productDTO);
                return Ok(result);
            }
            catch(ProductNotFoundException ex)
            {
                return NotFound(new ErrorModel(404, ex.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(500, new ErrorModel(500, "An error occurred while Updateing the product"));
            }
        }
        #endregion UpdateProduct

        #region DeleteProduct
        /// <summary>
        /// Delete a product
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Deleted Product</returns>
        [Authorize(Roles = "Admin,Seller")]
        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(Product), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<Product>> DeleteProduct(int id)
        {
            try
            {
                var result = await _productService.DeleteProduct(id);
                return Ok(result);
            }
            catch (ProductNotFoundException ex)
            {
                return NotFound(new ErrorModel(404, ex.Message));
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                return StatusCode(500, new ErrorModel(500, "An error occurred while deleting the product"));
            }
        }
        #endregion DeleteProduct

        #region GetFilteredProducts
        /// <summary>
        /// Get filtered products
        /// </summary>
        /// <param name="categoryId"></param>
        /// <param name="minPrice"></param>
        /// <param name="maxPrice"></param>
        /// <param name="availability"></param>
        /// <param name="minRating"></param>
        /// <param name="maxRating"></param>
        /// <param name="sellerId"></param>
        /// <returns></returns>
        [HttpGet("filter")]
        [ProducesResponseType(typeof(Product), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetFilteredProducts(
            
            [FromQuery] int? categoryId, [FromQuery] decimal? minPrice,
            
            [FromQuery] decimal? maxPrice, [FromQuery] bool? availability,
            
            [FromQuery] double? minRating, [FromQuery] double? maxRating,
            [FromQuery] int? sellerId)
        {
            try
            {
                var products = await _productService.GetFilteredProducts(
                    
                    categoryId,
                    
                    minPrice,
                    
                    maxPrice,
                    
                    availability,
                    
                    minRating,                  
                    maxRating,
                    sellerId);
                return Ok(products);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = ex.Message });
            }
        }
        #endregion GetFilteredProducts
    }
}
