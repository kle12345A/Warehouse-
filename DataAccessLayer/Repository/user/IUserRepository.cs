using DataAccessLayer.BaseRepository;
using DataAccessLayer.Models;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Repository.user
{
    public interface IUserRepository : IBaseRepository<User>
    {
    }
}
