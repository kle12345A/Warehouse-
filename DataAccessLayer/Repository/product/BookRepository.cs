using DataAccessLayer.BaseRepository;
using DataAccessLayer.Models;


namespace DataAccessLayer.Repository.product
{
    public class BookRepository : BaseRepository<Product>, IProductRepository
    {
        public BookRepository(WarehouseDbContext context) : base(context)
        {

        }
    }
}
