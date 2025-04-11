using DataAccessLayer.BaseRepository;
using DataAccessLayer.Models;
using Microsoft.EntityFrameworkCore;
using WarehouseDTOs;


namespace DataAccessLayer.Repository.user
{
    public class UserRepository : BaseRepository<User>, IUserRepository
    {
        private WarehouseDbContext _context;
        public UserRepository(WarehouseDbContext context) : base(context)
        {
            _context = context;
        }

       
    }
}
