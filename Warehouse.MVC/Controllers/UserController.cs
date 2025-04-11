using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Text;
using System.Threading.Tasks;
using Warehouse.MVC.Models;
using WarehouseDTOs;

namespace Warehouse.MVC.Controllers
{
    [Authorize(Roles = "Admin")]

    public class UserController : Controller
    {
        private string UrlGet = "https://localhost:7200/api/User";
        private string UrlGetRole = "https://localhost:7200/api/Role/GetAllRole";

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
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, UserDTO user)
        {
            // Kiểm tra id từ URL có khớp với userId từ form không
            if (id != user.UserId)
            {
                TempData["ErrorMessage"] = "Dữ liệu không hợp lệ.";
                return RedirectToAction("Index");
            }

            // Kiểm tra tính hợp lệ của model
            if (!ModelState.IsValid)
            {
                var roles = await GetRole();
                var view = new UserView
                {
                    roleDTO = roles,
                    userDTO = user
                };
                return View(view);
            }

            // Chỉ lấy các trường cần thiết và gán giá trị
            var userToUpdate = new UserDTO
            {
                UserId = id,
                Username = user.Username,
                Password = user.Password,
                FullName = user.FullName,
                Phone = user.Phone,
                Email = user.Email,
                Address = user.Address,
                RoleId = user.RoleId,
                UpdatedAt = DateTime.UtcNow // Tự động cập nhật thời gian
            };

            // Gọi API để cập nhật người dùng
            using (HttpClient client = new HttpClient())
            {
                var json = JsonConvert.SerializeObject(userToUpdate);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                HttpResponseMessage response = await client.PutAsync($"{UrlGet}/{id}", content);

                if (response.IsSuccessStatusCode)
                {
                    TempData["SuccessMessage"] = "Cập nhật người dùng thành công!";
                    return RedirectToAction("Index");
                }
                else
                {
                    // Đọc thông báo lỗi từ API
                    var errorMessage = await response.Content.ReadAsStringAsync();
                    TempData["ErrorMessage"] = $"Cập nhật thất bại: {errorMessage}";
                    var roles = await GetRole();
                    var view = new UserView
                    {
                        roleDTO = roles,
                        userDTO = user
                    };
                    return View(view);
                }
            }
        }
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            // Lấy thông tin người dùng theo id
            var user = await GetUserByIdAsync(id);
            if (user == null)
            {
                TempData["ErrorMessage"] = "Không tìm thấy người dùng.";
                return RedirectToAction("Index");
            }

            // Lấy danh sách vai trò
            var roles = await GetRole();

            // Tạo view model để truyền vào view
            var view = new UserView
            {
                roleDTO = roles,
                userDTO = user
            };

            return View(view);
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
        private async Task<UserDTO> GetUserByIdAsync(int id)
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
        private async Task<List<RoleDTO>> GetRole()
        {
            using (HttpClient client = new HttpClient())
            {
                using (HttpResponseMessage res = await client.GetAsync(UrlGetRole))
                {

                    if (res.IsSuccessStatusCode)
                    {
                        string data = await res.Content.ReadAsStringAsync();
                        return JsonConvert.DeserializeObject<List<RoleDTO>>(data);
                    }


                }

            }
            return new List<RoleDTO>();

        }
    }
}
