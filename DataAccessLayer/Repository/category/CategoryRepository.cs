using DataAccessLayer.BaseRepository;
using DataAccessLayer.Models;


namespace DataAccessLayer.Repository.category
{
    public class CategoryRepository : BaseRepository<Category>, ICategoryRepository
    {
        public CategoryRepository(WarehouseDbContext context) : base(context)
        {
        }
    }
}
