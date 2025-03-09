using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Text;
using Warehouse.MVC.Models;
using WarehouseDTOs;

namespace Warehouse.MVC.Controllers
{
    public class OrderDetailController : Controller
    {
        private string Url = "https://localhost:7200/api/OrderDetail";
        private string UrlOrder = "https://localhost:7200/api/Order";
        private string UrlOrderC = "https://localhost:7200/api/OrderDetail/Details";

        public async Task<IActionResult> Index(int id)
        {
            var ord = await GetOrderByIdAsync(id);
            var view = new OrderDetailView()
            {
                OrderDetailWithSupplier = ord
            };
            return View(view);
        }

        public async Task<IActionResult> XuatKho(int id)
        {
            var ord = await GetOrderByIdWithCusAsync(id);
            var view = new OrderDetailView()
            {
                OrderDetailWithCustomer = ord
            };
            return View(view);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ApproveOrder([FromForm] int id)
        {
            using (HttpClient client = new HttpClient())
            {
                // Lấy thông tin đơn hàng để kiểm tra OrderType
                var orderDetailsUrl = $"{Url}/{id}"; // Hoặc sử dụng UrlOrderC nếu phù hợp
                using (HttpResponseMessage getResponse = await client.GetAsync(orderDetailsUrl))
                {
                    if (!getResponse.IsSuccessStatusCode)
                    {
                        TempData["ErrorMessage"] = "Không thể lấy thông tin đơn hàng.";
                        return RedirectToAction("Index", "Order");
                    }

                    string orderData = await getResponse.Content.ReadAsStringAsync();
                    var order = JsonConvert.DeserializeObject<OrderDetailWithSupplierDTO>(orderData); // Hoặc OrderDetailWithCustomerDTO

                    if (order == null || order.OrderTypeEnum == null)
                    {
                        TempData["ErrorMessage"] = "Thông tin đơn hàng không hợp lệ.";
                        return RedirectToAction("Index", "Order");
                    }

                    // Tạo dữ liệu để gửi lên API phê duyệt
                    var requestBody = new OrderUpdateStatusDTO
                    {
                        OrderId = id,
                        Status = (int)OrderStatus.Approved
                    };

                    var jsonContent = new StringContent(JsonConvert.SerializeObject(requestBody), Encoding.UTF8, "application/json");

                    // Gọi API PATCH để phê duyệt
                    using (HttpResponseMessage res = await client.PatchAsync($"{UrlOrder}/{id}/status", jsonContent))
                    {
                        if (res.IsSuccessStatusCode)
                        {
                            TempData["SuccessMessage"] = "Phê duyệt đơn hàng thành công!";
                            // Chuyển hướng dựa trên OrderType
                            if (order.OrderTypeEnum == OrderTypeEnum.NhapKho) // 1
                            {
                                return RedirectToAction("Index", "Order");
                            }
                            else if (order.OrderTypeEnum == OrderTypeEnum.XuatKho) // 2
                            {
                                return RedirectToAction("XuatKho", "OrderDetail", new { id });
                            }
                            else
                            {
                                return RedirectToAction("Index", "Order"); // Default
                            }
                        }
                        else
                        {
                            var errorContent = await res.Content.ReadAsStringAsync();
                            TempData["ErrorMessage"] = "Phê duyệt đơn hàng thất bại: " + errorContent;
                            return RedirectToAction("Index", "Order", new { id });
                        }
                    }
                }
            }
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

        private async Task<OrderDetailWithCustomerDTO> GetOrderByIdWithCusAsync(int id)
        {
            using (HttpClient client = new HttpClient())
            {
                using (HttpResponseMessage res = await client.GetAsync($"{UrlOrderC}/{id}"))
                {
                    if (res.IsSuccessStatusCode)
                    {
                        string data = await res.Content.ReadAsStringAsync();
                        var order = JsonConvert.DeserializeObject<OrderDetailWithCustomerDTO>(data);
                        if (order != null && order.OrderDetails == null)
                        {
                            order.OrderDetails = new List<OrderDetailDTO>();
                        }
                        return order ?? new OrderDetailWithCustomerDTO();
                    }
                }
            }
            return new OrderDetailWithCustomerDTO();
        }


    }
}
