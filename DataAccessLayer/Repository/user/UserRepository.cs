using DataAccessLayer.BaseRepository;
using DataAccessLayer.Models;


namespace DataAccessLayer.Repository.user
{
    public class UserRepository : BaseRepository<User>, IUserRepository
    {
        public UserRepository(WarehouseDbContext context) : base(context)
        {
        }
    }
}
