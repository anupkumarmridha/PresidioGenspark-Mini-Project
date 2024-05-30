using ApparelShoppingAppAPI.Exceptions;
using ApparelShoppingAppAPI.Models.DB_Models;
using ApparelShoppingAppAPI.Repositories.Interfaces;
using ApparelShoppingAppAPI.Services.Interfaces;

namespace ApparelShoppingAppAPI.Services.Classes
{
    public class CartService : ICartService
    {
        private readonly ICartRepository _cartRepository;
        private readonly IProductRepository _productRepository;


        public CartService(ICartRepository cartRepository, IProductRepository productRepository)
        {
            _cartRepository = cartRepository;
            _productRepository = productRepository;
        }


        #region AddOrUpdateProductToCart

        /// <summary>
        /// Adds a product to the cart, updating the quantity if the product already exists in the cart.
        /// </summary>
        /// <param name="userId">The ID of the user.</param>
        /// <param name="productId">The ID of the product.</param>
        /// <param name="quantity">The quantity to add (can be positive or negative).</param>
        /// <returns>The updated cart.</returns>
        /// <exception cref="ProductNotFoundException">Thrown when the product is not found or has an invalid price.</exception>
        public async Task<Cart> AddOrUpdateProductToCart(int userId, int productId, int quantity)
        {
            try
            {
                var product = await _productRepository.GetById(productId);

                if (product == null)
                {
                    throw new ProductNotFoundException("Product not found or invalid price");
                }
                if (product.Quantity < quantity)
                {
                    throw new InsufficientProductQuantityException($"Insufficient product quantity. Available Quantity = {product.Quantity}");
                }

                // Retrieve the customer's cart, or create a new one if it doesn't exist
                var cart = await _cartRepository.GetCartByUserId(userId);
                if (cart == null)
                {
                    cart = new Cart
                    {
                        CustomerId = userId,
                        Items = new List<CartItem>()
                    };
                    await _cartRepository.Add(cart);
                }

                // Find the cart item for the specified product, or create a new one if it doesn't exist
                var cartItem = cart.Items.FirstOrDefault(ci => ci.ProductId == productId);
                if (cartItem == null)
                {
                    cartItem = new CartItem
                    {
                        ProductId = productId,
                        Quantity = quantity,
                        Price = product.Price * quantity
                    };
                    cart.Items.Add(cartItem);
                }
                else
                {
                    // Update the quantity and price of the existing cart item
                    cartItem.Quantity += quantity;
                    if (cartItem.Quantity <= 0)
                    {
                        cart.Items.Remove(cartItem);
                    }
                    else
                    {
                        // Update the price based on the new quantity
                        cartItem.Price = product.Price * cartItem.Quantity;
                    }
                }

                // Calculate the total price of the cart
                cart.TotalPrice = cart.Items.Sum(item => item.Price);

                // Update the cart's last updated date
                cart.LastUpdatedDate = DateTime.Now;

                await _cartRepository.Update(cart);

                return cart;
            }
            catch(ProductNotFoundException ex)
            {
                throw new ProductNotFoundException(ex.Message);
            }
            catch (InsufficientProductQuantityException ex)
            {
                throw new InsufficientProductQuantityException(ex.Message);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
           
        }

        #endregion AddOrUpdateProductToCart
        #region GetCartByUserId

        public async Task<Cart> GetCartByUserId(int userId)
        {
            try
            {
                return await _cartRepository.GetCartByUserId(userId);
            }
            catch(CartNotFoundException ex)
            {
                throw new CartNotFoundException(ex.Message);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        #endregion GetCartByUserId
        #region RemoveProductFromCart

        /// <summary>
        /// Removes a product from the cart.
        /// </summary>
        /// <param name="userId">The ID of the user.</param>
        /// <param name="productId">The ID of the product.</param>
        /// <returns>The updated cart.</returns>
        /// <exception cref="CartNotFoundException">Thrown when the cart is not found.</exception>
        /// <exception cref="ProductNotFoundException">Thrown when the product is not found in the cart.</exception>
        public async Task<Cart> RemoveProductFromCart(int userId, int productId)
        {
            // Retrieve the customer's cart
            var cart = await _cartRepository.GetCartByUserId(userId);

            if (cart == null)
            {
                throw new CartNotFoundException("Cart Not Found");
            }

            // Find the specific item in the cart
            var cartItem = cart.Items.FirstOrDefault(ci => ci.ProductId == productId);
            if (cartItem == null)
            {
                throw new ProductNotFoundException("Product not found in cart");
            }

            // Remove the item from the cart
            cart.Items.Remove(cartItem);

            // Recalculate the total price of the cart
            cart.TotalPrice = cart.Items.Sum(item => item.Price);

            // Update the cart's last updated date
            cart.LastUpdatedDate = DateTime.Now;

            await _cartRepository.Update(cart);

            return cart;
        }
        #endregion RemoveProductFromCart
    }
}
