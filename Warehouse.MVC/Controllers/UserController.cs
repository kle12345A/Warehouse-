using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Warehouse.MVC.Models;
using WarehouseDTOs;

namespace Warehouse.MVC.Controllers
{
    public class UserController : Controller
    {
        private string UrlGet = "https://localhost:7200/api/User";
        public async Task<IActionResult> Index()
        {
            var user = await GetUserAsync();
            var view = new UserView()
            {
                UserDTOs = user
            };
            return View(view);
        }
        public async Task<IActionResult> Create()
        {
            return View();
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
