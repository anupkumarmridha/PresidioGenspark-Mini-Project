﻿using ApparelShoppingAppAPI.Models.DB_Models;
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

        public async Task<Product> GetProductById(int id)
        {
            try
            {
                return await _productRepository.GetById(id);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<IEnumerable<Product>> GetAllProducts()
        {
            try
            {

                return await _productRepository.GetAll();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

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

        public async Task<Product> UpdateProduct(int id, ProductDTO product)
        {
            try
            {
                return await _productRepository.UpdateProductWithCategoryTransaction(id,product);
            }
            catch(Exception ex) { 
                throw new Exception(ex.Message);
            }
           
        }

        public async Task<Product> DeleteProduct(int id)
        {
            try
            {
                return await _productRepository.Delete(id);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<Product> GetProductByName(string name)
        {
            try
            {
                return await _productRepository.GetProductByName(name);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
