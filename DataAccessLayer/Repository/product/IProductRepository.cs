using DataAccessLayer.BaseRepository;
using DataAccessLayer.Models;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Repository.product
{
    public interface IProductRepository : IBaseRepository<Product>
    {
        Task<Product?> GetByNameAsync(string name);
        Task<Product?> GetByIdAsync(int id);
    }
 
}
