using BussinessLayer.BaseService;
using DataAccessLayer.BaseRepository;
using DataAccessLayer.Models;
using DataAccessLayer.Repository.role;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BussinessLayer.Service.role
{
    public class RoleService : BaseService<Role>, IRoleService
    {
        private readonly IRoleRepository _roleRepository;
        public RoleService(IRoleRepository baseRepository) : base(baseRepository)
        {
            _roleRepository = baseRepository;
        }
    }
}
