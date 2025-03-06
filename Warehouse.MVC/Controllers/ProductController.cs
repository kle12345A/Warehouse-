using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Security.Policy;
using System.Text;
using Warehouse.MVC.Models;
using WarehouseDTOs;

namespace Warehouse.MVC.Controllers
{
    public class ProductController : Controller
    {
        private string UrlGet = "https://localhost:7200/api/Products";
        private string UrlCreate = "https://localhost:7200/api/Products";
        private string UrlCategory = "https://localhost:7200/api/Category";
        private string UrlDeleteMultiple = "https://localhost:7200/api/Products/DeleteMultiple";
        private string UrlUpdate = "https://localhost:7200/api/Products";

        public async Task<IActionResult> Index()
        {
            var products = await GetProductAsync();
            var view = new ProductView
            {
                Products = products,
            };
            return View(view);
        }
        [HttpPost]
        public async Task<IActionResult> Create(ProductDTO product, IFormFile images)
        {
            var categories = await GetCategoryAsync();

            if (!IsValidImage(images, out var validationError))
            {
                ModelState.AddModelError("images", validationError);
                return View(new ProductView { Categories = categories, Product = product });
            }

            using var client = new HttpClient();
            var content = BuildFormData(product, images);

            var response = await client.PostAsync(UrlCreate, content);

            if (response.IsSuccessStatusCode)
            {
                TempData["SuccessMessage"] = "Tạo sản phẩm thành công!";
                return RedirectToAction("Index");
            }

            var errorMessage = await response.Content.ReadAsStringAsync();
            ModelState.AddModelError("", $"Lỗi khi tạo sản phẩm: {errorMessage}");
            return View(new ProductView { Categories = categories, Product = product });
        }

        [HttpPost]
        public async Task<IActionResult> Update(UpdateProductRequest model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            using (var client = new HttpClient())
            {
                var content = new MultipartFormDataContent();
                content.Add(new StringContent(model.ProductId.ToString()), "ProductId");
                content.Add(new StringContent(model.Name), "Name");
                content.Add(new StringContent(model.Description ?? ""), "Description");
                content.Add(new StringContent(model.Unit ?? ""), "Unit");
                content.Add(new StringContent(model.Quantity?.ToString() ?? ""), "Quantity");
                content.Add(new StringContent(model.AvailableQuantity?.ToString() ?? ""), "AvailableQuantity");
                content.Add(new StringContent(model.Price.ToString()), "Price");
                content.Add(new StringContent(model.CostPrice.ToString()), "CostPrice");
                content.Add(new StringContent(model.CategoryId?.ToString() ?? ""), "CategoryId");
                content.Add(new StringContent(model.Images ?? ""), "Images");

                if (model.ImageFile != null && model.ImageFile.Length > 0)
                {
                    var stream = model.ImageFile.OpenReadStream();
                    var fileContent = new StreamContent(stream);
                    content.Add(fileContent, "ImageFile", model.ImageFile.FileName);
                }

                var response = await client.PutAsync($"{UrlUpdate}/{model.ProductId}", content);

                if (response.IsSuccessStatusCode)
                {
                    TempData["SuccessMessage"] = "Cập nhật sản phẩm thành công!";
                   
                    return RedirectToAction("Index");
                }
                else
                {
                    var errorMessage = await response.Content.ReadAsStringAsync();
                    ModelState.AddModelError("", $"Lỗi khi cập nhật sản phẩm: {errorMessage}");
                    return View(model);
                }
            }
        }

        // GET: Product/Update/{id}
        public async Task<IActionResult> Update(int id)
        {
            var product = await GetProductByIdAsync(id);
            if (product == null)
            {
                return NotFound();
            }

            var categories = await GetCategoryAsync();

            var view = new ProductView
            {
                Product = product,
                Categories = categories
            };

            return View(view);
        }
       
        [HttpPost]
        public async Task<IActionResult> DeleteMultiple(List<int> ids)
        {
            if (ids == null || !ids.Any())
            {
                return Json(new { success = false, message = "Không có sản phẩm nào được chọn." });
            }

            try
            {
                using (HttpClient client = new HttpClient())
                {
                    string json = JsonConvert.SerializeObject(ids);
                    var content = new StringContent(json, Encoding.UTF8, "application/json");
                    HttpResponseMessage response = await client.PostAsync(UrlDeleteMultiple, content);

                    if (response.IsSuccessStatusCode)
                    {
                        return Json(new { success = true });
                    }
                    else
                    {
                        string errorMessage = await response.Content.ReadAsStringAsync();
                        return Json(new { success = false, message = errorMessage });
                    }
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
        public async Task<IActionResult> Create()
        {
            var category = await GetCategoryAsync();
            var view = new ProductView()
            {
                Categories = category,
            };

            return View(view);
        }

        //=======================================================================
        private async Task<ProductDTO> GetProductByIdAsync(int id)
        {
            using (HttpClient client = new HttpClient())
            {
                using (HttpResponseMessage res = await client.GetAsync($"{UrlGet}/{id}"))
                {
                    if (res.IsSuccessStatusCode)
                    {
                        string data = await res.Content.ReadAsStringAsync();
                        return JsonConvert.DeserializeObject<ProductDTO>(data);
                    }
                }
            }
            return null;
        }
        private async Task<List<ProductDTO>> GetProductAsync()
        {
            using (HttpClient client = new HttpClient())
            {
                using (HttpResponseMessage res = await client.GetAsync(UrlGet))
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
        private async Task<List<CategoryDTO>> GetCategoryAsync()
        {
            using (HttpClient client = new HttpClient())
            {
                using (HttpResponseMessage res = await client.GetAsync(UrlCategory))
                {
                    if (res.IsSuccessStatusCode)
                    {
                        string data = await res.Content.ReadAsStringAsync();
                        return JsonConvert.DeserializeObject<List<CategoryDTO>>(data);
                    }
                }
            }
            return new List<CategoryDTO>();
        }

        private bool IsValidImage(IFormFile? image, out string error)
        {
            error = string.Empty;
            if (image == null) return true;

            if (image.Length > 2 * 1024 * 1024)
            {
                error = "Dung lượng ảnh không được vượt quá 2MB.";
                return false;
            }

            return true;
        }

        private MultipartFormDataContent BuildFormData(ProductDTO product, IFormFile? image)
        {
            var content = new MultipartFormDataContent
    {
        { new StringContent(product.Name), "Name" },
        { new StringContent(product.Description ?? ""), "Description" },
        { new StringContent(product.Unit ?? ""), "Unit" },
        { new StringContent(product.Quantity?.ToString() ?? ""), "Quantity" },
        { new StringContent(product.AvailableQuantity?.ToString() ?? ""), "AvailableQuantity" },
        { new StringContent(product.Price.ToString()), "Price" },
        { new StringContent(product.CostPrice.ToString()), "CostPrice" },
        { new StringContent(product.CategoryId?.ToString() ?? ""), "CategoryId" }
    };

            if (image != null && image.Length > 0)
            {
                var streamContent = new StreamContent(image.OpenReadStream());
                content.Add(streamContent, "ImageFile", image.FileName);
            }

            return content;
        }

    }
}
