using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Warehouse.MVC.Models;
using WarehouseDTOs;

namespace Warehouse.MVC.Controllers
{
    public class OrderDetailController : Controller
    {
        private string Url = "https://localhost:7200/api/OrderDetail";
        public async Task<IActionResult> Index(int id)
        {
            var ord = await GetOrderByIdAsync(id);
            var view = new OrderDetailView()
            {
                OrderDetailWithSupplier = ord
            };
            return View(view);
        }



        //==========================

        private async Task<OrderDetailWithSupplierDTO> GetOrderByIdAsync(int id)
        {
            using (HttpClient client = new HttpClient())
            {
                using (HttpResponseMessage res = await client.GetAsync($"{Url}/{id}"))
                {
                    if (res.IsSuccessStatusCode)
                    {
                        string data = await res.Content.ReadAsStringAsync();
                        var order = JsonConvert.DeserializeObject<OrderDetailWithSupplierDTO>(data);
                        if (order != null && order.OrderDetails == null)
                        {
                            order.OrderDetails = new List<OrderDetailDTO>();
                        }
                        return order ?? new OrderDetailWithSupplierDTO();
                    }
                }
            }
            return new OrderDetailWithSupplierDTO();
        }


    }
}
