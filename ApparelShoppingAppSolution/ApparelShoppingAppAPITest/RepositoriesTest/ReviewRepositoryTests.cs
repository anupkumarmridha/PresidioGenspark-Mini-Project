using ApparelShoppingAppAPI.Contexts;
using ApparelShoppingAppAPI.Models.DB_Models;
using ApparelShoppingAppAPI.Repositories.Classes;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApparelShoppingAppAPITest.RepositoriesTest
{
    public class ReviewRepositoryTests
    {
        private ReviewRepository _reviewRepository;
        private ShoppingAppDbContext _context;

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<ShoppingAppDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDb")
                .Options;

            _context = new ShoppingAppDbContext(options);
            _reviewRepository = new ReviewRepository(_context);
        }

        [TearDown]
        public void TearDown()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }

        [Test]
        public async Task Add_Review_ReturnsAddedReview()
        {
            // Arrange
            var review = new Review
            {
                ReviewId = 1,
                ProductId = 123,
                CustomerId = 456,
                Rating = 5,
                Comment = "This is a test comment for the product. I really liked it!",
                CreatedDate = DateTime.Now,
                UpdateDate = DateTime.Now
            };

            // Act
            var result = await _reviewRepository.Add(review);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(review.ReviewId, result.ReviewId);
        }

        [Test]
        public async Task GetById_ReviewExists_ReturnsReview()
        {
            // Arrange
            var review = new Review
            {
                ReviewId = 1,
                ProductId = 123,
                CustomerId = 456,
                Rating = 5,
                Comment = "This is a test comment for the product. I really liked it!",
                CreatedDate = DateTime.Now,
                UpdateDate = DateTime.Now
            };

            await _reviewRepository.Add(review);

            // Act
            var result = await _reviewRepository.GetById(1);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(review.ReviewId, result.ReviewId);
        }

        [Test]
        public async Task GetById_ReviewDoesNotExist_ReturnsNull()
        {
            // Act
            var result = await _reviewRepository.GetById(1);

            // Assert
            Assert.IsNull(result);
        }

        [Test]
        public async Task GetAll_ReturnsAllReviewes()
        {
            // Arrange
            var review1 = new Review
            {
                ReviewId = 1,
                ProductId = 123,
                CustomerId = 456,
                Rating = 5,
                Comment = "This is a test comment for the product. I really liked it!",
                CreatedDate = DateTime.Now,
                UpdateDate = DateTime.Now
            };
            var review2 = new Review
            {
                ReviewId = 2,
                ProductId = 123,
                CustomerId = 456,
                Rating = 5,
                Comment = "This is a test comment for the product. I really liked it!",
                CreatedDate = DateTime.Now,
                UpdateDate = DateTime.Now
            };
            await _reviewRepository.Add(review1);
            await _reviewRepository.Add(review2);

            // Act
            var result = await _reviewRepository.GetAll();

            // Assert
            Assert.AreEqual(2, result.Count());
        }

        [Test]
        public async Task Update_Review_ReturnsUpdatedReview()
        {
            // Arrange
            var review = new Review
            {
                ReviewId = 1,
                ProductId = 123,
                CustomerId = 456,
                Rating = 5,
                Comment = "This is a test comment for the product. I really liked it!",
                CreatedDate = DateTime.Now,
                UpdateDate = DateTime.Now
            };
            await _reviewRepository.Add(review);

            review.Rating = 3;

            // Act
            var result = await _reviewRepository.Update(review);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(3, result.Rating);
        }

        [Test]
        public async Task Delete_ReviewExists_ReturnsDeletedReview()
        {
            // Arrange
            var review = new Review
            {
                ReviewId = 1,
                ProductId = 123,
                CustomerId = 456,
                Rating = 5,
                Comment = "This is a test comment for the product. I really liked it!",
                CreatedDate = DateTime.Now,
                UpdateDate = DateTime.Now
            };
            await _reviewRepository.Add(review);

            // Act
            var result = await _reviewRepository.Delete(1);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(review.ReviewId, result.ReviewId);
        }

        [Test]
        public void Delete_ReviewDoesNotExist_ThrowsInvalidOperationException()
        {
            // Act & Assert
            var ex = Assert.ThrowsAsync<InvalidOperationException>(() => _reviewRepository.Delete(1));
            Assert.AreEqual("1 not found.", ex.Message);
        }
    }
}
