using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Rotativa;
using Rotativa.AspNetCore;
using System.Text;
using Warehouse.MVC.Models;
using WarehouseDTOs;

namespace Warehouse.MVC.Controllers
{
    [Authorize]

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
        public async Task<IActionResult> ApproveOrder([FromForm] int id)
        {
            using (HttpClient client = new HttpClient())
            {

                var orderDetailsUrl = $"{Url}/{id}";
                using (HttpResponseMessage getResponse = await client.GetAsync(orderDetailsUrl))
                {
                    if (!getResponse.IsSuccessStatusCode)
                    {
                        TempData["ErrorMessage"] = "Không thể lấy thông tin đơn hàng.";
                        return RedirectToAction("Index", "Order");
                    }

                    string orderData = await getResponse.Content.ReadAsStringAsync();
                    var order = JsonConvert.DeserializeObject<OrderDetailWithSupplierDTO>(orderData);

                    if (order == null || order.OrderTypeEnum == null)
                    {
                        TempData["ErrorMessage"] = "Thông tin đơn hàng không hợp lệ.";
                        return RedirectToAction("Index", "Order");
                    }

                    var requestBody = new OrderUpdateStatusDTO
                    {
                        OrderId = id,
                        Status = (int)OrderStatus.Approved
                    };

                    var jsonContent = new StringContent(JsonConvert.SerializeObject(requestBody), Encoding.UTF8, "application/json");

                    using (HttpResponseMessage res = await client.PatchAsync($"{UrlOrder}/{id}/status", jsonContent))
                    {
                        if (res.IsSuccessStatusCode)
                        {
                            TempData["SuccessMessage"] = "Phê duyệt đơn hàng thành công!";
                            return RedirectToAction("Index", "Order");
                        }
                        else
                        {
                            var errorContent = await res.Content.ReadAsStringAsync();
                            TempData["ErrorMessage"] = "Phê duyệt đơn hàng thất bại: " + errorContent;
                            return RedirectToAction("Index", "Order");
                        }


                    }
                }
            }
        }
        [HttpPost]
        public async Task<IActionResult> ApproveExport([FromForm] int id)
        {
            using (HttpClient client = new HttpClient())
            {

                var orderDetailsUrl = $"{UrlOrderC}/{id}";
                using (HttpResponseMessage getResponse = await client.GetAsync(orderDetailsUrl))
                {
                    if (!getResponse.IsSuccessStatusCode)
                    {
                        TempData["ErrorMessage"] = "Không thể lấy thông tin đơn hàng.";
                        return RedirectToAction("XuatKho", "Order");
                    }

                    string orderData = await getResponse.Content.ReadAsStringAsync();
                    var order = JsonConvert.DeserializeObject<OrderDetailWithCustomerDTO>(orderData);

                    if (order == null || order.OrderTypeEnum == null)
                    {
                        TempData["ErrorMessage"] = "Thông tin đơn hàng không hợp lệ.";
                        return RedirectToAction("XuatKho", "Order");
                    }

                    var requestBody = new OrderUpdateStatusDTO
                    {
                        OrderId = id,
                        Status = (int)OrderStatus.Approved
                    };

                    var jsonContent = new StringContent(JsonConvert.SerializeObject(requestBody), Encoding.UTF8, "application/json");

                    using (HttpResponseMessage res = await client.PatchAsync($"{UrlOrder}/{id}/status", jsonContent))
                    {
                        if (res.IsSuccessStatusCode)
                        {
                            TempData["SuccessMessage"] = "Phê duyệt đơn hàng thành công!";
                            return RedirectToAction("XuatKho", "Order");
                        }
                        else
                        {
                            var errorContent = await res.Content.ReadAsStringAsync();
                            TempData["ErrorMessage"] = "Phê duyệt đơn hàng thất bại: " + errorContent;
                            return RedirectToAction("XuatKho", "Order");
                        }


                    }
                }
            }
        }

        public async Task<IActionResult> ExportInvoice(int id)
        {
            // Lấy thông tin đơn hàng từ API
            var order = await GetOrderByIdWithCusAsync(id);

            if (order == null || order.OrderDetails == null)
            {
                TempData["ErrorMessage"] = "Không thể lấy thông tin đơn hàng để xuất hóa đơn.";
                return RedirectToAction("XuatKho", new { id });
            }

            // Tạo view model để truyền vào view PDF
            var viewModel = new OrderDetailView
            {
                OrderDetailWithCustomer = order
            };

            // Tạo PDF từ view
            var pdf = new ViewAsPdf("ExportInvoice", viewModel)
            {
                FileName = $"HoaDon_{id}_{DateTime.Now:yyyyMMddHHmmss}.pdf",
                PageSize = Rotativa.AspNetCore.Options.Size.A4,
                PageOrientation = Rotativa.AspNetCore.Options.Orientation.Portrait
            };

            // Trả về IActionResult
            return pdf;
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
