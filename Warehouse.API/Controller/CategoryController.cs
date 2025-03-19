using BussinessLayer.Service.category;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WarehouseDTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Warehouse.API.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryService _categoryService;

        public CategoryController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        [HttpGet]
        public async Task<ActionResult<List<CategoryDTO>>> GetCategories()
        {
            var categories = await _categoryService.GetAllCategoriesAsync();
            return Ok(categories);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<CategoryDTO>> GetCategoryById(int id)
        {
            var category = await _categoryService.GetCategoryByIdAsync(id);
            if (category == null)
            {
                return NotFound(new { message = "Danh mục không tồn tại" });
            }
            return Ok(category);
        }

        [HttpPost]
        public async Task<IActionResult> CreateCategory([FromBody] CategoryDTO categoryDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var createdCategory = await _categoryService.CreateCategoryAsync(categoryDto);
                return CreatedAtAction(nameof(GetCategoryById), new { id = createdCategory.CategoryId }, createdCategory);
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { message = ex.Message }); 
            }
        }


        [HttpPut("{id}")]
        public async Task<ActionResult<CategoryUpdateDTO>> UpdateCategory(int id, [FromBody] CategoryUpdateDTO categoryDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var updatedCategory = await _categoryService.UpdateCategoryAsync(id, categoryDto);
            if (updatedCategory == null)
            {
                return NotFound(new { message = "Danh mục không tồn tại" });
            }
            return Ok(updatedCategory);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteCategory(int id)
        {
            var result = await _categoryService.DeleteCategoryAsync(id);
            if (!result)
            {
                return NotFound(new { message = "Danh mục không tồn tại" });
            }
            return NoContent();
        }

        [HttpGet("WithProductCount")]
        public async Task<ActionResult<List<CategoryProduct>>> GetAllCategoriesWithProductCount()
        {
            try
            {
                var categories = await _categoryService.GetAllCategoriesProduct();
                return Ok(categories);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"An error occurred: {ex.Message}" });
            }
        }

        [HttpGet("{id}/products")]
        public async Task<ActionResult<List<ProductDTO>>> GetProductsByCategory(int id)
        {
            try
            {
                var products = await _categoryService.GetProductsByCategoryAsync(id);
                return Ok(products);
            }
            catch (KeyNotFoundException)
            {
                return NotFound(new { message = "Danh mục không tồn tại hoặc không có sản phẩm" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Lỗi hệ thống: {ex.Message}" });
            }
        }


    }
}
