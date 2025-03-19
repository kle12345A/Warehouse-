using AutoMapper;
using BussinessLayer.BaseService;
using DataAccessLayer.Models;
using DataAccessLayer.Repository.category;
using WarehouseDTOs;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace BussinessLayer.Service.category
{
    public class CategoryService : BaseService<Category>, ICategoryService
    {
        private readonly ICategoryRepository _categoryRepository;
        private readonly IMapper _mapper;

        public CategoryService(ICategoryRepository baseRepository, IMapper mapper) : base(baseRepository)
        {
            _categoryRepository = baseRepository;
            _mapper = mapper;
        }

        public async Task<List<CategoryDTO>> GetAllCategoriesAsync()
        {
            var categories = await GetAllAsync();
            if (categories == null)
            {
                throw new InvalidOperationException("Failed to retrieve categories.");
            }
            return _mapper.Map<List<CategoryDTO>>(categories);
        }

        public async Task<CategoryDTO> GetCategoryByIdAsync(int id)
        {
            if (id <= 0)
            {
                throw new ArgumentException("Category ID must be greater than 0.", nameof(id));
            }

            var category = await GetByIdAsync(id);
            if (category == null)
            {
                throw new KeyNotFoundException($"Category with ID {id} not found.");
            }
            return _mapper.Map<CategoryDTO>(category);
        }

        public async Task<CategoryDTO> CreateCategoryAsync(CategoryDTO categoryDto)
        {
            if (await _categoryRepository.CategoryExistsAsync(categoryDto.Name))
            {
                throw new InvalidOperationException($"Danh mục '{categoryDto.Name}' đã tồn tại.");
            }

            var category = _mapper.Map<Category>(categoryDto);
            category.CreatedAt = DateTime.Now;
            await AddAsync(category);

            return _mapper.Map<CategoryDTO>(category);
        }


        public async Task<CategoryUpdateDTO> UpdateCategoryAsync(int id, CategoryUpdateDTO categoryDto)
        {
            if (id <= 0)
            {
                throw new ArgumentException("Category ID must be greater than 0.", nameof(id));
            }

          
            var existingCategory = await GetByIdAsync(id);
            if (existingCategory == null)
            {
                throw new KeyNotFoundException($"Category with ID {id} not found.");
            }

            _mapper.Map(categoryDto, existingCategory);
            await UpdateAsync(existingCategory);
            return _mapper.Map<CategoryUpdateDTO>(existingCategory);
        }

        public async Task<bool> DeleteCategoryAsync(int id)
        {
            if (id <= 0)
            {
                throw new ArgumentException("Category ID must be greater than 0.", nameof(id));
            }

            return await DeleteAsync(id);
        }

        public async Task<List<CategoryProduct>> GetAllCategoriesProduct()
        {
            var categories = await GetAllQueryable()
                .Select(c => new CategoryProduct
                {
                    CategoryId = c.CategoryId,
                    CategoryName = c.Name,
                    ProductCount = c.Products != null ? c.Products.Count : 0
                })
                .ToListAsync();

            return categories;
        }

        public async Task<List<ProductDTO>> GetProductsByCategoryAsync(int categoryId)
        {
            if (categoryId <= 0)
                throw new ArgumentException("Category ID must be greater than 0.", nameof(categoryId));

            var category = await _categoryRepository.GetCategoriesWithProducts()
                .FirstOrDefaultAsync(c => c.CategoryId == categoryId);

            if (category == null)
                throw new KeyNotFoundException($"Category with ID {categoryId} not found.");

            return category.Products != null ? _mapper.Map<List<ProductDTO>>(category.Products) : new List<ProductDTO>();
        }



    }
}