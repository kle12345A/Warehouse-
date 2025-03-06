using DataAccessLayer.BaseRepository;
using DataAccessLayer.Models;


namespace DataAccessLayer.Repository.inventoryhistory
{
    public class InventoryHistoryRepository : BaseRepository<InventoryHistory>, IInventoryHistoryRepository
    {
        public InventoryHistoryRepository(WarehouseDbContext context) : base(context)
        {
        }
    }
}
