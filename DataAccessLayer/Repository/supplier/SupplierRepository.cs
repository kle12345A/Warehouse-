using DataAccessLayer.BaseRepository;
using DataAccessLayer.Models;


namespace DataAccessLayer.Repository.supplier
{
    public class SupplierRepository : BaseRepository<Supplier>, ISupplierRepository
    {
        public SupplierRepository(WarehouseDbContext context) : base(context)
        {
        }
    }
}
