using System.ComponentModel.DataAnnotations;

namespace ApparelShoppingAppAPI.Models.DB_Models
{
    public class User
    {
        [Key]
        public int UserId { get; set; }

        [Required]
        public byte[] Password { get; set; }

        [Required]
        public byte[] PasswordHashKey { get; set; }

        [Required]
        public string Role { get; set; }

        public string Status { get; set; } = "Not Activate";

        // Navigation property for customer
        public virtual Customer? Customer { get; set; }

        // Navigation property for seller
        public virtual Seller? Seller { get; set; }

    }
}
