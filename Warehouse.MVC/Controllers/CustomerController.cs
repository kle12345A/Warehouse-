using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Text;
using Warehouse.MVC.Models;
using WarehouseDTOs;

namespace Warehouse.MVC.Controllers
{
    public class CustomerController : Controller
    {
        private readonly string url = "https://localhost:7200/api/Customer";

        public async Task<IActionResult> Index(int page = 1, int pageSize = 5, string search = null)
        {
            var customers = await GetCustomerAsync();
            var filteredCustomers = customers?.ToList() ?? new List<CustomerDTO>();

           
            if (!string.IsNullOrEmpty(search))
            {
                filteredCustomers = filteredCustomers
                    .Where(c => (c.FullName?.Contains(search, StringComparison.OrdinalIgnoreCase) ?? false) ||
                                (c.Phone?.Contains(search, StringComparison.OrdinalIgnoreCase) ?? false) ||
                                (c.Email?.Contains(search, StringComparison.OrdinalIgnoreCase) ?? false) ||
                                (c.Address?.Contains(search, StringComparison.OrdinalIgnoreCase) ?? false))
                    .ToList();
            }

            int totalItems = filteredCustomers.Count;
            int totalPages = (int)Math.Ceiling((double)totalItems / pageSize);
            var pagedData = filteredCustomers.Skip((page - 1) * pageSize).Take(pageSize).ToList();

            ViewBag.Page = page;
            ViewBag.TotalPages = totalPages;
            ViewBag.Search = search;

            var view = new CustomerView
            {
                Customers = pagedData
            };
            return View(view);
        }


        private async Task<List<CustomerDTO>> GetCustomerAsync()
        {
            using (HttpClient client = new HttpClient())
            {
                using (HttpResponseMessage res = await client.GetAsync(url))
                {
                    if (res.IsSuccessStatusCode)
                    {
                        string data = await res.Content.ReadAsStringAsync();
                        return JsonConvert.DeserializeObject<List<CustomerDTO>>(data);
                    }
                }
            }
            return new List<CustomerDTO>();
        }
        [HttpGet]
        public async Task<IActionResult> Update(int id)
        {
            var customer = await GetCustomerByIdAsync(id);
            if (customer == null)
            {
                return NotFound();
            }

            var viewModel = new CustomerView()
            {
                Customer = customer
            };

            return View(viewModel);
        }
        [HttpGet]
        [HttpGet]
        public IActionResult Create()
        {
            return View(new CustomerView { Customer = new CustomerDTO() });
        }

        [HttpPost]
        public async Task<IActionResult> Create(CustomerDTO customer)
        {


            string json = JsonConvert.SerializeObject(customer);
            using (HttpClient client = new HttpClient())
            {
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                HttpResponseMessage response = await client.PostAsync(url, content);

                if (response.IsSuccessStatusCode)
                {
                    TempData["SuccessMessage"] = "Tạo khchs hàng thành công!!";
                    return RedirectToAction("Index");
                }
                else
                {
                    TempData["ErrorMessage"] = "Tạo thất bại, vui lòng thử lại!";
                    return RedirectToAction("Index");
                }
            }

           
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(int id, CustomerUpdateDTO category)
        {

            using (HttpClient client = new HttpClient())
            {
                var json = JsonConvert.SerializeObject(category);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                HttpResponseMessage response = await client.PutAsync($"{url}/{id}", content);

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
        [HttpPost]
        public async Task<IActionResult> DeleteMultiple([FromBody] List<int> customerIds)
        {
            if (customerIds == null || !customerIds.Any())
            {
                return BadRequest("Danh sách khách hàng cần xóa không hợp lệ.");
            }

            using (HttpClient client = new HttpClient())
            {
                var json = JsonConvert.SerializeObject(customerIds);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                HttpResponseMessage response = await client.PostAsync($"{url}/delete-multiple", content);

                if (response.IsSuccessStatusCode)
                {
                    TempData["SuccessMessage"] = "Xóa thành công!";
                    return Ok();
                }
                else
                {
                    TempData["ErrorMessage"] = "Xóa thất bại.";
                    return StatusCode((int)response.StatusCode);
                }
            }
        }

        private async Task<CustomerDTO?> GetCustomerByIdAsync(int id)
        {
            using (HttpClient client = new HttpClient())
            {
                using (HttpResponseMessage res = await client.GetAsync($"{url}/{id}"))
                {
                    if (res.IsSuccessStatusCode)
                    {
                        string data = await res.Content.ReadAsStringAsync();
                        return JsonConvert.DeserializeObject<CustomerDTO>(data);
                    }
                }
            }
            return null;
        }

    }
}
