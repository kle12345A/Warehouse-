using BussinessLayer.BaseService;
using DataAccessLayer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WarehouseDTOs;

namespace BussinessLayer.Service.category
{
    public interface ICategoryService : IBaseService<Category>
    {
        Task<List<CategoryDTO>> GetAllCategoriesAsync();
        Task<CategoryDTO> GetCategoryByIdAsync(int id);
        Task<CategoryDTO> CreateCategoryAsync(CategoryDTO categoryDto);
        Task<CategoryUpdateDTO> UpdateCategoryAsync(int id, CategoryUpdateDTO categoryDto);
        Task<bool> DeleteCategoryAsync(int id);
        Task<List<CategoryProduct>> GetAllCategoriesProduct();
        Task<List<ProductDTO>> GetProductsByCategoryAsync(int categoryId);


    }
}
