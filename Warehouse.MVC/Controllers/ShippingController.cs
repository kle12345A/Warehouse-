using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Warehouse.MVC.Models;
using WarehouseDTOs;

namespace Warehouse.MVC.Controllers
{
    public class ShippingController : Controller
    {
        private string UrlGet = "https://localhost:7200/api/Shipping";
        public async Task<IActionResult> Index()
        {
            var ship = await GetShippingAsync();
            var view = new ShippingView()
            {
                shippingDTOs = ship
            };
            return View(view);
        }


        //==============================================
        private async Task<List<ShippingDTO>> GetShippingAsync()
        {
            using (HttpClient client = new HttpClient())
            {
                using (HttpResponseMessage res = await client.GetAsync(UrlGet))
                {
                    if (res.IsSuccessStatusCode)
                    {
                        string data = await res.Content.ReadAsStringAsync();
                        return JsonConvert.DeserializeObject<List<ShippingDTO>>(data);
                    }
                }
            }
            return new List<ShippingDTO>();
        }
    }
}
