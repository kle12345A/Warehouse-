using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Text;
using Warehouse.MVC.Models;
using WarehouseDTOs;

namespace Warehouse.MVC.Controllers
{
    [Authorize]

    public class SupplierController : Controller
    {
        private string UrlGet = "https://localhost:7200/api/Supplier";


        [HttpGet]
        public async Task<IActionResult> Index(int page = 1, int pageSize = 5, string search = null)
        {
            var suppliers = await GetSupplierAsync();
            var filteredSuppliers = suppliers?.ToList() ?? new List<SupplierDTO>();

            if (!string.IsNullOrEmpty(search))
            {
                filteredSuppliers = filteredSuppliers
                    .Where(s => (s.Name?.Contains(search, StringComparison.OrdinalIgnoreCase) ?? false) ||
                                (s.Phone?.Contains(search, StringComparison.OrdinalIgnoreCase) ?? false) ||
                                (s.Email?.Contains(search, StringComparison.OrdinalIgnoreCase) ?? false))
                    .ToList();
            }

            int totalItems = filteredSuppliers.Count;
            int totalPages = (int)Math.Ceiling((double)totalItems / pageSize);
            var pagedData = filteredSuppliers.Skip((page - 1) * pageSize).Take(pageSize).ToList();

            ViewBag.Page = page;
            ViewBag.TotalPages = totalPages;
            ViewBag.Search = search; 

            var view = new SupplierView
            {
                Suppliers = pagedData
            };
            return View(view);
        }
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }


        [HttpPost]
        public async Task<IActionResult> Create(SupplierDTO supplier)
        {


            string json = JsonConvert.SerializeObject(supplier);
            using (HttpClient client = new HttpClient())
            {
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                HttpResponseMessage response = await client.PostAsync(UrlGet, content);

                if (response.IsSuccessStatusCode)
                {
                    TempData["SuccessMessage"] = "Thêm NCC thành công!";
                    return RedirectToAction("Index");
                }
            }

            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> Update(int id)
        {
            var sup = await GetCategoryByIdAsync(id);
            var view = new SupplierView
            {
                Supplier = sup

            };
            return View(view);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(int id, SupplierUpdateDTO supplier)
        {
          
            using (HttpClient client = new HttpClient())
            {
                var json = JsonConvert.SerializeObject(supplier);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                HttpResponseMessage response = await client.PutAsync($"{UrlGet}/{id}", content);

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

            return View(supplier);
        }




        //=====================================
        private async Task<List<SupplierDTO>> GetSupplierAsync()
        {
            using (HttpClient client = new HttpClient())
            {
                using (HttpResponseMessage res = await client.GetAsync(UrlGet))
                {
                    if (res.IsSuccessStatusCode)
                    {
                        string data = await res.Content.ReadAsStringAsync();
                        return JsonConvert.DeserializeObject<List<SupplierDTO>>(data);
                    }
                }
            }
            return new List<SupplierDTO>();
        }


        private async Task<SupplierDTO> GetCategoryByIdAsync(int id)
        {
            using (HttpClient client = new HttpClient())
            {
                using (HttpResponseMessage res = await client.GetAsync($"{UrlGet}/{id}"))
                {
                    if (res.IsSuccessStatusCode)
                    {
                        string data = await res.Content.ReadAsStringAsync();
                        return JsonConvert.DeserializeObject<SupplierDTO>(data);
                    }
                }
            }
            return null;
        }
    }
}
