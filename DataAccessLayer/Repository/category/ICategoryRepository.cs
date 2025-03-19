using DataAccessLayer.BaseRepository;
using DataAccessLayer.Models;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Repository.category
{
    public interface ICategoryRepository : IBaseRepository<Category>
    {
        Task<bool> CategoryExistsAsync(string categoryName);
        IQueryable<Category> GetCategoriesWithProducts();
    }
}
