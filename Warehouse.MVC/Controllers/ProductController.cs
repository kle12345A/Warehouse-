using Microsoft.AspNetCore.Mvc;
using Microsoft.Build.Framework;
using Microsoft.VisualBasic;
using Newtonsoft.Json;
using System.Net.Http;
using System.Net.Http.Headers;
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
        private string UrlReadFile = "https://localhost:7200/api/Products/read-file-product";
        private string UrlCreate2 = "https://localhost:7200/api/Products/create-product";

        public async Task<IActionResult> Index(int page = 1, int pageSize = 5, int? categoryId = null, string search = null)
        {
            var products = await GetProductAsync();
            var cate = await GetCategoryAsync();

            // Lọc phía client
            if (!string.IsNullOrEmpty(search))
            {
                products = products.Where(p => p.Name.ToLower().Contains(search.ToLower())).ToList();
            }
            if (categoryId.HasValue)
            {
                products = products.Where(p => p.CategoryId == categoryId).ToList();
            }

            int totalItems = products.Count;
            int totalPages = (int)Math.Ceiling((double)totalItems / pageSize);
            var pagedData = products.Skip((page - 1) * pageSize).Take(pageSize).ToList();

            ViewBag.Page = page;
            ViewBag.TotalPages = totalPages;
            ViewBag.CategoryId = categoryId; // Lưu trạng thái dropdown
            ViewBag.Search = search; // Lưu trạng thái tìm kiếm

            var view = new ProductView
            {
                Products = pagedData,
                Categories = cate
            };
            return View(view);
        }

        public IActionResult ImportExcel()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ReadFileAndPreview(IFormFile file, int page = 1, int pageSize = 10)
        {
            if (file == null || file.Length == 0)
            {
                TempData["ErrorMessage"] = "Vui lòng chọn file Excel!";
                return RedirectToAction("Index");
            }

            if (!Path.GetExtension(file.FileName).Equals(".xlsx", StringComparison.OrdinalIgnoreCase) &&
                !Path.GetExtension(file.FileName).Equals(".xls", StringComparison.OrdinalIgnoreCase))
            {
                TempData["ErrorMessage"] = "Vui lòng chọn file Excel (.xls hoặc .xlsx)!";
                return RedirectToAction("Index");
            }

            try
            {
                using (var client = new HttpClient())
                using (var stream = file.OpenReadStream())
                {
                    using (var content = new MultipartFormDataContent())
                    {
                        content.Add(new StreamContent(stream), "file", file.FileName);
                        var response = await client.PostAsync(UrlReadFile, content);

                        if (response.IsSuccessStatusCode)
                        {
                            var data = await response.Content.ReadAsStringAsync();
                            var records = JsonConvert.DeserializeObject<List<ProductImportDTO>>(data);

                            // Phân trang
                            int totalItems = records.Count;
                            int totalPages = (int)Math.Ceiling((double)totalItems / pageSize);
                            var pagedData = records.Skip((page - 1) * pageSize).Take(pageSize).ToList();

                            // Truyền dữ liệu vào ViewBag
                            ViewBag.PreviewData = pagedData;
                            ViewBag.Page = page;
                            ViewBag.PageSize = pageSize;
                            ViewBag.TotalPages = totalPages;
                            ViewBag.TotalItems = totalItems;

                            // Lưu toàn bộ dữ liệu vào TempData để sử dụng khi gửi (nếu cần)
                            TempData["FullPreviewData"] = JsonConvert.SerializeObject(records);

                            return View("Preview");
                        }
                        else
                        {
                            var errorMessage = await response.Content.ReadAsStringAsync();
                            TempData["ErrorMessage"] = $"Lỗi khi đọc file: {errorMessage}";
                            return RedirectToAction("Index");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Lỗi nội bộ: {ex.Message}";
                return RedirectToAction("Index");
            }
        }
        public IActionResult Preview()
        {
            return View();
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


        [HttpPost]
        public async Task<IActionResult> ConfirmImport(string previewData)
        {
            if (string.IsNullOrEmpty(previewData))
            {
                TempData["ErrorMessage"] = "Không có dữ liệu để gửi!";
                return RedirectToAction("Preview");
            }

            try
            {
                Console.WriteLine($"previewData: {previewData}"); // Kiểm tra log
                var importRecords = JsonConvert.DeserializeObject<List<ProductImportDTO>>(previewData);

                if (importRecords == null || !importRecords.Any())
                {
                    TempData["ErrorMessage"] = "Danh sách sản phẩm rỗng!";
                    return RedirectToAction("Preview");
                }

                using (var client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json")); // Định nghĩa JSON format

                    foreach (var importRecord in importRecords)
                    {
                        var productDto = new ProductDTO
                        {
                            Name = importRecord.Name,
                            Description = importRecord.Description,
                            Unit = importRecord.Unit,
                            AvailableQuantity = importRecord.AvailableQuantity,
                            Price = importRecord.Price,
                            CostPrice = importRecord.CostPrice,
                            CategoryId = importRecord.CategoryId,
                            Images = importRecord.ImagePath
                        };

                        var jsonContent = new StringContent(JsonConvert.SerializeObject(productDto), Encoding.UTF8, "application/json");

                        var response = await client.PostAsync(UrlCreate2, jsonContent);

                        if (!response.IsSuccessStatusCode)
                        {
                            var errorMessage = await response.Content.ReadAsStringAsync();
                            TempData["ErrorMessage"] = $"Lỗi khi tạo sản phẩm {importRecord.Name}: {errorMessage}";
                            return RedirectToAction("Preview");
                        }
                    }

                    TempData["SuccessMessage"] = "Tất cả sản phẩm đã được tạo thành công!";
                    return RedirectToAction("Index");
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Lỗi nội bộ: {ex.Message}";
                return RedirectToAction("Preview");
            }
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
