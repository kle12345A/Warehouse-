using DataAccessLayer.BaseRepository;
using DataAccessLayer.Models;
using Microsoft.EntityFrameworkCore;


namespace DataAccessLayer.Repository.supplier
{
    public class SupplierRepository : BaseRepository<Supplier>, ISupplierRepository
    {
        readonly WarehouseDbContext _dbContext;
        public SupplierRepository(WarehouseDbContext context) : base(context)
        {
            _dbContext = context;
        }
        public async Task<int> GetTotalSuppliersAsync()
        {
            return await _dbContext.Suppliers.CountAsync();
        }
    }
}
