using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Security.Claims;
using System.Security.Principal;
using System.Text;
using WarehouseDTOs;

namespace Warehouse.MVC.Controllers
{
    public class AuthController : Controller
    {
        private string UrlLogin = "https://localhost:7200/api/User/Login";
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("", "Auth");
        }
        public async Task<IActionResult> Login(UserLogin userLogin)
        {
            string json = JsonConvert.SerializeObject(userLogin);
            using (HttpClient client = new HttpClient())
            {
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                HttpResponseMessage response = await client.PostAsync(UrlLogin, content);

                string responseContent = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    var userInfo = JsonConvert.DeserializeObject<UserInfoDTO>(responseContent);

                    var claims = new List<Claim>
                        {
                            new Claim(ClaimTypes.Name, userInfo.UserName),
                            new Claim(ClaimTypes.Email, userInfo.EmailAddress),
                            new Claim(ClaimTypes.Role, userInfo.RoleName) 
                        };

                    var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                    var authProperties = new AuthenticationProperties
                    {
                        IsPersistent = false,
                        ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(5)
                    };

                    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity), authProperties);

                    HttpContext.Session.SetString("UserId", userInfo.UserId.ToString());
                    HttpContext.Session.SetString("UserName", userInfo.UserName);
                    HttpContext.Session.SetString("EmailAddress", userInfo.EmailAddress);
                    HttpContext.Session.SetString("RoleId", userInfo.RoleId.ToString());

                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    ViewBag.ErrorMessage = responseContent;
                    return View("Index");
                }
            }
        }
    }
}
