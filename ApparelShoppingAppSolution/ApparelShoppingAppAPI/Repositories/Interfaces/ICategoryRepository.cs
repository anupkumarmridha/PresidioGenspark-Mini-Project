using ApparelShoppingAppAPI.Models.DB_Models;

namespace ApparelShoppingAppAPI.Repositories.Interfaces
{
    public interface ICategoryRepository : IRepository<int, Category>
    {
        public Task<Category> GetCategoryByName(string name);
    }
}
