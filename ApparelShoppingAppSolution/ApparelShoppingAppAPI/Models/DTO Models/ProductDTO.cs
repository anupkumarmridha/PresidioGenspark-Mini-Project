using System.ComponentModel.DataAnnotations;

namespace ApparelShoppingAppAPI.Models.DTO_Models
{
    public class ProductDTO
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public string ImageUrl { get; set; }
        public string CategoryName { get; set; }
    }
}
