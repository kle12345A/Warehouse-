using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Text;
using Warehouse.MVC.Models;
using WarehouseDTOs;

namespace Warehouse.MVC.Controllers
{
    [Authorize]

    public class CategoryController : Controller
    {
        private string UrlGet = "https://localhost:7200/api/Category/WithProductCount";
        private string UrlGetId = "https://localhost:7200/api/Category";
        private string UrlCreate = "https://localhost:7200/api/Category";
        private string UrlUpdate = "https://localhost:7200/api/Category";

        public async Task<IActionResult> Index(int page = 1, int pageSize = 5, string search = null)
        {
            var categories = await GetCategoryAsync();
            var filteredCategories = categories?.ToList() ?? new List<CategoryProduct>();

            if (!string.IsNullOrEmpty(search))
            {
                filteredCategories = filteredCategories
                    .Where(c => c.CategoryName.Contains(search, StringComparison.OrdinalIgnoreCase))
                    .ToList();
            }

            int totalItems = filteredCategories.Count;
            int totalPages = (int)Math.Ceiling((double)totalItems / pageSize);
            var pagedData = filteredCategories.Skip((page - 1) * pageSize).Take(pageSize).ToList();

            ViewBag.Page = page;
            ViewBag.TotalPages = totalPages;
            ViewBag.Search = search; // Lưu trạng thái tìm kiếm

            var view = new CategoryView
            {
                Categories = pagedData
            };
            return View(view);
        }
        public async Task<IActionResult> Update(int id)
        {
            var cat = await GetCategoryByIdAsync(id);
            var prodwc = await GetProductsByCategoryAsync(id);
            var view = new CategoryView
            {
                Products = prodwc,

                Category = cat
            };
            return View(view);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(int id, CategoryUpdateDTO category)
        {

            using (HttpClient client = new HttpClient())
            {
                var json = JsonConvert.SerializeObject(category);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                HttpResponseMessage response = await client.PutAsync($"{UrlUpdate}/{id}", content);

                if (response.IsSuccessStatusCode)
                {
                    TempData["SuccessMessage"] = "Cập nhật thành công!!";
                    return RedirectToAction("Index");
                }
                else
                {
                    TempData["ErrorMessage"] = "Cập nhật thất bại, vui lòng thử lại!";
                    return RedirectToAction("Index");
                }
            }

           
        }

        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(CategoryDTO category)
        {
         

            string json = JsonConvert.SerializeObject(category);
            using (HttpClient client = new HttpClient())
            {
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                HttpResponseMessage response = await client.PostAsync(UrlCreate, content);

                if (response.IsSuccessStatusCode)
                {
                    TempData["SuccessMessage"] = "Tạo danh mục thành công!!";
                    return RedirectToAction("Index");
                }
                else
                {
                    TempData["ErrorMessage"] = "Danh mục đã tồn tại vu lòng thử lại!";
                    return RedirectToAction("Index");
                }
            }

            
        }
        private async Task<CategoryDTO> GetCategoryByIdAsync(int id)
        {
            using (HttpClient client = new HttpClient())
            {
                using (HttpResponseMessage res = await client.GetAsync($"{UrlGetId}/{id}"))
                {
                    if (res.IsSuccessStatusCode)
                    {
                        string data = await res.Content.ReadAsStringAsync();
                        return JsonConvert.DeserializeObject<CategoryDTO>(data);
                    }
                }
            }
            return null;
        }

        private async Task<List<CategoryProduct>> GetCategoryAsync()
        {
            using (HttpClient client = new HttpClient())
            {
                using (HttpResponseMessage res = await client.GetAsync(UrlGet))
                {
                    if (res.IsSuccessStatusCode)
                    {
                        string data = await res.Content.ReadAsStringAsync();
                        return JsonConvert.DeserializeObject<List<CategoryProduct>>(data);
                    }
                }
            }
            return new List<CategoryProduct>();
        }
        private async Task<List<ProductDTO>> GetProductsByCategoryAsync(int categoryId)
        {
            using (HttpClient client = new HttpClient())
            {
                using (HttpResponseMessage res = await client.GetAsync($"{UrlGetId}/{categoryId}/products"))
                {
                    if (res.IsSuccessStatusCode)
                    {
                        string data = await res.Content.ReadAsStringAsync();
                        return JsonConvert.DeserializeObject<List<ProductDTO>>(data);
                    }
                }
            }
            return new List<ProductDTO>();
        }

    }
}
