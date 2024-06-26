﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ApparelShoppingAppAPI.Models.DB_Models
{
    public class Order
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int OrderId { get; set; }

        [Required]
        public int CustomerId { get; set; }

        [Required]
        public int AddressId { get; set; }

        [Required]
        public decimal TotalPrice { get; set; }

        [Required]
        public DateTime OrderDate { get; set; }= DateTime.Now;
        
        [Required]
        public DateTime OrderUpdatedDate { get; set; }= DateTime.Now;

        [Required]
        public string OrderStatus { get; set; } = "Active";
        
        [Required]
        public bool IsDelivered { get; set; } = false;
        
        [Required]
        public bool IsCanceled { get; set; } = false;

        [Required]
        public bool IsPaid { get; set; } = false;

        // Navigation properties
        public virtual Customer Customer { get; set; }
        public virtual Address Address { get; set; }
        public virtual ICollection<OrderDetails> OrderDetails { get; set; }
    }
}
