using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ApparelShoppingAppAPI.Models.DB_Models
{
    [Index(nameof(Email), IsUnique = true)]
    public class Customer
    {
        [Key]
        [ForeignKey("User")]
        public int CustomerId { get; set; }

        [Required]
        public string Name { get; set; }
        [Required]
        public string Email { get; set; }
        [Required]
        public string Phone { get; set; }
        // Navigation properties
        public virtual Cart? Cart { get; set; }
        public virtual ICollection<Order>? Orders { get; set; }
        public virtual ICollection<Review>? Reviews { get; set; }
        public virtual ICollection<Address>? Addresses { get; set; }
    }
}
