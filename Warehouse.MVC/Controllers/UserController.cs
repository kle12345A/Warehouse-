using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Text;
using Warehouse.MVC.Models;
using WarehouseDTOs;

namespace Warehouse.MVC.Controllers
{
    [Authorize(Roles = "Admin")]

    public class UserController : Controller
    {
        private string UrlGet = "https://localhost:7200/api/User";

        public async Task<IActionResult> Index(int page = 1, int pageSize = 5, string search = null)
        {
            var users = await GetUserAsync();
            var filteredUsers = users?.ToList() ?? new List<UserDTO>();

            // Lọc phía client
            if (!string.IsNullOrEmpty(search))
            {
                filteredUsers = filteredUsers
                    .Where(u => (u.Username?.Contains(search, StringComparison.OrdinalIgnoreCase) ?? false) ||
                                (u.FullName?.Contains(search, StringComparison.OrdinalIgnoreCase) ?? false) ||
                                (u.Phone?.Contains(search, StringComparison.OrdinalIgnoreCase) ?? false) ||
                                (u.Email?.Contains(search, StringComparison.OrdinalIgnoreCase) ?? false) )
                              
                    .ToList();
            }

            int totalItems = filteredUsers.Count;
            int totalPages = (int)Math.Ceiling((double)totalItems / pageSize);
            var pagedData = filteredUsers.Skip((page - 1) * pageSize).Take(pageSize).ToList();

            ViewBag.Page = page;
            ViewBag.TotalPages = totalPages;
            ViewBag.Search = search;

            var view = new UserView
            {
                UserDTOs = pagedData
            };
            return View(view);
        }
        public async Task<IActionResult> Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(UserDTO user)
        {


            string json = JsonConvert.SerializeObject(user);
            using (HttpClient client = new HttpClient())
            {
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                HttpResponseMessage response = await client.PostAsync(UrlGet, content);

                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction("Index");
                }
            }

            return RedirectToAction("Index");
        }


        //=======================
        private async Task<UserDTO> GetCategoryByIdAsync(int id)
        {
            using (HttpClient client = new HttpClient())
            {
                using (HttpResponseMessage res = await client.GetAsync($"{UrlGet}/{id}"))
                {
                    if (res.IsSuccessStatusCode)
                    {
                        string data = await res.Content.ReadAsStringAsync();
                        return JsonConvert.DeserializeObject<UserDTO>(data);
                    }
                }
            }
            return null;
        }
        private async Task<List<UserDTO>> GetUserAsync()
        {
            using (HttpClient client = new HttpClient())
            {
                using (HttpResponseMessage res = await client.GetAsync(UrlGet)) {

                    if (res.IsSuccessStatusCode)
                    {
                        string data = await res.Content.ReadAsStringAsync();
                        return JsonConvert.DeserializeObject<List<UserDTO>>(data);
                    }


                }

            }
            return new List<UserDTO>();

        }
    }
}
