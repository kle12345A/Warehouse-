using DataAccessLayer.BaseRepository;
using DataAccessLayer.Models;


namespace DataAccessLayer.Repository.product
{
    public class ProductRepository : BaseRepository<Product>, IProductRepository
    {
        public ProductRepository(WarehouseDbContext context) : base(context)
        {
        }
    }
}
