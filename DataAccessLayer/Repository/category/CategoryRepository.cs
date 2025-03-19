using DataAccessLayer.BaseRepository;
using DataAccessLayer.Models;
using Microsoft.EntityFrameworkCore;

namespace DataAccessLayer.Repository.category
{
    public class CategoryRepository : BaseRepository<Category>, ICategoryRepository
    {
        private readonly WarehouseDbContext _context;

        public CategoryRepository(WarehouseDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<bool> CategoryExistsAsync(string categoryName)
        {
            return await _context.Categories.AnyAsync(c => c.Name.ToLower() == categoryName.ToLower());
        }

        public IQueryable<Category> GetCategoriesWithProducts()
        {
            return _context.Categories.Include(c => c.Products).AsQueryable();
        }
    }

}
