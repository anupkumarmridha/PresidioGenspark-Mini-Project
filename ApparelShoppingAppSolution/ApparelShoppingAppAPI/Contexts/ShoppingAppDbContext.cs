using Microsoft.EntityFrameworkCore;
using ApparelShoppingAppAPI.Models.DB_Models;
using System;

namespace ApparelShoppingAppAPI.Contexts
{
    public class ShoppingAppDbContext : DbContext
    {
        public ShoppingAppDbContext(DbContextOptions<ShoppingAppDbContext> options) : base(options) { }
        public DbSet<User> Users { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Seller> Sellers { get; set; }
        public DbSet<Cart> Carts { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<Review> Reviews { get; set; }
        public DbSet<Address> Addresses { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<PaymentDetail> PaymentDetails { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // User to Customer one-to-one relationship
            modelBuilder.Entity<User>()
                .HasOne(u => u.Customer)
                .WithOne()
                .HasForeignKey<Customer>(c => c.CustomerId)
                .OnDelete(DeleteBehavior.Cascade); // Ensure cascading delete

            // User to Seller one-to-one relationship
            modelBuilder.Entity<User>()
                .HasOne(u => u.Seller)
                .WithOne()
                .HasForeignKey<Seller>(s => s.SellerId)
                .OnDelete(DeleteBehavior.Cascade); // Ensure cascading delete

            // Ensuring the primary keys are the same
            modelBuilder.Entity<Customer>()
                .Property(c => c.CustomerId)
                .ValueGeneratedNever();

            modelBuilder.Entity<Seller>()
                .Property(s => s.SellerId)
                .ValueGeneratedNever();


            // Order to OrderDetail one-to-many relationship
            modelBuilder.Entity<Order>()
                .HasMany(o => o.OrderDetails)
                .WithOne()
                .HasForeignKey(od => od.OrderId);

            // Product to Review one-to-many relationship
            modelBuilder.Entity<Product>()
                .HasMany(p => p.Reviews)
                .WithOne(r => r.Product)
                .HasForeignKey(r => r.ProductId);

            // Seller to Products one-to-many relationship
            modelBuilder.Entity<Product>()
                .HasOne(p => p.Seller)
                .WithMany(s => s.Products)
                .HasForeignKey(p => p.SellerId)
                .OnDelete(DeleteBehavior.Restrict);

            // Product to Category many-to-one relationship
            modelBuilder.Entity<Product>()
                .HasOne(p => p.Category)
                .WithMany()
                .HasForeignKey(p => p.CategoryId)
                .OnDelete(DeleteBehavior.SetNull);

            // Customer to Review one-to-many relationship
            modelBuilder.Entity<Customer>()
                .HasMany(c => c.Reviews)
                .WithOne(r => r.Customer)
                .HasForeignKey(r => r.CustomerId);

            // Customer to Address one-to-many relationship
            modelBuilder.Entity<Customer>()
                .HasMany(c => c.Addresses)
                .WithOne(a => a.Customer)
                .HasForeignKey(a => a.CustomerId);
            
            // Configure the one-to-many relationship between Order and Address
            modelBuilder.Entity<Order>()
                .HasOne(o => o.Address)
                .WithMany()
                .HasForeignKey(o => o.AddressId)
                .OnDelete(DeleteBehavior.Restrict);

            // Customer to Cart one-to-one relationship
            modelBuilder.Entity<Customer>()
                .HasOne(c => c.Cart)
                .WithOne(cart => cart.Customer)
                .HasForeignKey<Cart>(cart => cart.CustomerId);
        }
    }
}
