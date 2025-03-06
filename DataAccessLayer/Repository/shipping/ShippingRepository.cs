using DataAccessLayer.BaseRepository;
using DataAccessLayer.Models;

namespace DataAccessLayer.Repository.shipping
{
    public class ShippingRepository : BaseRepository<Shipping>, IShippingRepository
    {
        public ShippingRepository(WarehouseDbContext context) : base(context)
        {
        }
    }
}
