using DataAccessLayer.BaseRepository;
using DataAccessLayer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Repository.role
{
    public class RoleRepository : BaseRepository<Role>, IRoleRepository
    {
        private readonly WarehouseDbContext _context;
        public RoleRepository(WarehouseDbContext context) : base(context)
        {
            _context = context;
        }
    }
}
