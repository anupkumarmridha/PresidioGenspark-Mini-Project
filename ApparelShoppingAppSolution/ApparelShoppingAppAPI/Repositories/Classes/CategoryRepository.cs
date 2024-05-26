using ApparelShoppingAppAPI.Contexts;
using ApparelShoppingAppAPI.Models.DB_Models;
using ApparelShoppingAppAPI.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ApparelShoppingAppAPI.Repositories.Classes
{
    public class CategoryRepository:BaseRepository<int, Category>, ICategoryRepository
    {
        public CategoryRepository(ShoppingAppDbContext context) : base(context)
        {
        }

        public async Task<Category> GetCategoryByName(string name)
        {
            var category = await _context.Categories
            .FirstOrDefaultAsync(c => c.Name.ToUpper() == name.ToUpper());
            if (category == null)
            {
                throw new InvalidOperationException($"{name} not found.");
            }
            return category;
        }
    }
}
