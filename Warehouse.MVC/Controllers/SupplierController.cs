using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Text;
using Warehouse.MVC.Models;
using WarehouseDTOs;

namespace Warehouse.MVC.Controllers
{
    public class SupplierController : Controller
    {
        private string UrlGet = "https://localhost:7200/api/Supplier";


        [HttpGet]
        public  async Task<IActionResult> Index()
        {
            var sup = await GetSupplierAsync();
            var view = new SupplierView()
            {
                Suppliers = sup
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
