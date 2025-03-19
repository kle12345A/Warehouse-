using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Text;
using Warehouse.MVC.Helpers;
using Warehouse.MVC.Models;
using WarehouseDTOs;

namespace Warehouse.MVC.Controllers
{
    [Authorize]

    public class OrderController : Controller
    {
        private string UrlGet = "https://localhost:7200/api/Order";
        private string UrlSup = "https://localhost:7200/api/Supplier";
        private string UrlCus = "https://localhost:7200/api/Customer";
        private string UrlProduct = "https://localhost:7200/api/Products";
        public async Task<IActionResult> Index(int page = 1, int pageSize = 5, int? status = null, string dateFilter = null, string search = null)
        {
            var orders = await GetOrderAsync();
            var filteredOrders = orders?.Where(o => o.OrderType == OrderTypeEnum.NhapKho).ToList() ?? new List<OrderListDTO>();

            // Tìm kiếm
            if (!string.IsNullOrEmpty(search))
            {
                filteredOrders = filteredOrders
                    .Where(o => o.OrderId.ToString().Contains(search, StringComparison.OrdinalIgnoreCase) ||
                                (o.SupplierName?.Contains(search, StringComparison.OrdinalIgnoreCase) ?? false))
                    .ToList();
            }

            // Lọc theo trạng thái
            if (status.HasValue)
            {
                filteredOrders = filteredOrders.Where(o => (int?)o.Status == status).ToList();
            }

            // Lọc theo ngày
            if (!string.IsNullOrEmpty(dateFilter))
            {
                switch (dateFilter.ToLower())
                {
                    case "hôm nay":
                        filteredOrders = filteredOrders.Where(o => o.OrderDate.HasValue && o.OrderDate.Value.Date == DateTime.Today).ToList();
                        break;
                    case "7 ngày qua":
                        filteredOrders = filteredOrders.Where(o => o.OrderDate.HasValue && o.OrderDate.Value.Date >= DateTime.Today.AddDays(-7)).ToList();
                        break;
                    case "tháng này":
                        filteredOrders = filteredOrders.Where(o => o.OrderDate.HasValue && o.OrderDate.Value.Month == DateTime.Today.Month && o.OrderDate.Value.Year == DateTime.Today.Year).ToList();
                        break;
                }
            }

            // **Sắp xếp theo OrderId giảm dần (mới nhất trước)**
            filteredOrders = filteredOrders.OrderByDescending(o => o.OrderId).ToList();

            // Phân trang
            int totalItems = filteredOrders.Count;
            int totalPages = (int)Math.Ceiling((double)totalItems / pageSize);
            var pagedData = filteredOrders.Skip((page - 1) * pageSize).Take(pageSize).ToList();

            ViewBag.Page = page;
            ViewBag.TotalPages = totalPages;
            ViewBag.Status = status;
            ViewBag.DateFilter = dateFilter;
            ViewBag.Search = search;

            var view = new OrderVIew
            {
                OrderListDTO = pagedData
            };

            return View(view);
        }

        public async Task<IActionResult> XuatKho(int page = 1, int pageSize = 5, int? status = null, string dateFilter = null, string search = null)
        {
            var orders = await GetOrderAsync();
            var filteredOrders = orders?.Where(o => o.OrderType == OrderTypeEnum.XuatKho).ToList() ?? new List<OrderListDTO>();

            // Tìm kiếm
            if (!string.IsNullOrEmpty(search))
            {
                filteredOrders = filteredOrders
                    .Where(o => o.OrderId.ToString().Contains(search, StringComparison.OrdinalIgnoreCase) ||
                                (o.Customer?.Contains(search, StringComparison.OrdinalIgnoreCase) ?? false))
                    .ToList();
            }

            // Lọc theo trạng thái
            if (status.HasValue)
            {
                filteredOrders = filteredOrders.Where(o => (int?)o.Status == status).ToList();
            }

            // Lọc theo ngày
            if (!string.IsNullOrEmpty(dateFilter))
            {
                switch (dateFilter.ToLower())
                {
                    case "hôm nay":
                        filteredOrders = filteredOrders.Where(o => o.OrderDate.HasValue && o.OrderDate.Value.Date == DateTime.Today).ToList();
                        break;
                    case "7 ngày qua":
                        filteredOrders = filteredOrders.Where(o => o.OrderDate.HasValue && o.OrderDate.Value.Date >= DateTime.Today.AddDays(-7)).ToList();
                        break;
                    case "tháng này":
                        filteredOrders = filteredOrders.Where(o => o.OrderDate.HasValue && o.OrderDate.Value.Month == DateTime.Today.Month && o.OrderDate.Value.Year == DateTime.Today.Year).ToList();
                        break;
                }
            }

           
            filteredOrders = filteredOrders.OrderByDescending(o => o.OrderId).ToList();

            // Phân trang
            int totalItems = filteredOrders.Count;
            int totalPages = (int)Math.Ceiling((double)totalItems / pageSize);
            var pagedData = filteredOrders.Skip((page - 1) * pageSize).Take(pageSize).ToList();

            ViewBag.Page = page;
            ViewBag.TotalPages = totalPages;
            ViewBag.Status = status;
            ViewBag.DateFilter = dateFilter;
            ViewBag.Search = search;

            var view = new OrderVIew
            {
                OrderListDTO = pagedData
            };

            return View(view);
        }


        public async Task<IActionResult> Update(int id)
        {
            var existOrder = await GetOrderByIdAsync(id);
            if (existOrder == null)
            {
                TempData["ErrorMessage"] = "Đơn hàng không tồn tại.";
                return RedirectToAction("Index");
            }
            var sessionKey = $"UpdateOrderDetails_{id}";
            var orderDetailsSession = HttpContext.Session.GetObjectFromJson<List<OrderDetailsDTO>>(sessionKey);
            if (orderDetailsSession == null)
            {
                orderDetailsSession = existOrder.OrderDetails.ToList();
                HttpContext.Session.SetObjectAsJson(sessionKey, orderDetailsSession);
            }
            var sup = await GetSupplierAsync();
            var produc = await GetProductAsync();
           
            var view = new OrderVIew()
            {
                OrderDTOs = existOrder,
                OrderDetailsDTO = orderDetailsSession,
                Suppliers = sup,
                Products = produc,
                StatusList = Enum.GetValues(typeof(OrderStatus)).Cast<OrderStatus>().Select(s => new StatusDTO
        {
            StatusId = (int)s,
            StatusName = s.ToString()
        }).ToList()
            };
            return View(view); 
        }



        public async Task<IActionResult> Create()
        {
            var sup = await GetSupplierAsync();
            var pro = await GetProductAsync();
            var cus = await GetCustomerAsync();
            var cartItems = HttpContext.Session.GetObjectFromJson<List<CartItemDTO>>("CartItems") ?? new List<CartItemDTO>();
            var view = new OrderVIew()
            {
                CustomerDTOs= cus,
                Products = pro,
                Suppliers = sup,
                CartItems = cartItems
            };
            return View(view);
        }

        public async Task<IActionResult> CreateExportWare(int page = 1, int pageSize = 5)
        {
            var sup = await GetSupplierAsync();
            var pro = await GetProductAsync();
            var cus = await GetCustomerAsync();
            var cartItems = HttpContext.Session.GetObjectFromJson<List<CartItemDTO>>("CartItems") ?? new List<CartItemDTO>();

            int totalItems = pro.Count;
            int totalPages = (int)Math.Ceiling((double)totalItems / pageSize);
            var pagedProducts = pro.Skip((page - 1) * pageSize).Take(pageSize).ToList();

            var view = new OrderVIew()
            {
                CustomerDTOs = cus,
                Products = pagedProducts,
                Suppliers = sup,
                CartItems = cartItems,
                CurrentPage = page,
                TotalPages = totalPages,
                PageSize = pageSize
            };
            return View(view);
        }

        public async Task<IActionResult> GetProductList(int page = 1, int pageSize = 5)
        {
            var pro = await GetProductAsync();
            int totalItems = pro.Count;
            int totalPages = (int)Math.Ceiling((double)totalItems / pageSize);
            var pagedProducts = pro.Skip((page - 1) * pageSize).Take(pageSize).ToList();

            var view = new OrderVIew()
            {
                Products = pagedProducts,
                CurrentPage = page,
                TotalPages = totalPages,
                PageSize = pageSize
            };
            return PartialView("_ProductList", view);
        }


        [HttpPost]
        public async Task<IActionResult> Create(OrderCreateDTO order)
        {
            var cartItems = HttpContext.Session.GetObjectFromJson<List<CartItemDTO>>("CartItems") ?? new List<CartItemDTO>();

            if (!cartItems.Any())
            {
                TempData["ErrorMessage"] = "Giỏ hàng trống. Vui lòng thêm sản phẩm.";
                return RedirectToAction("Index");
            }

            order.OrderDetails = cartItems.Select(c => new OrderDetailCreateDTO
            {
                ProductId = c.ProductId,
                Quantity = c.Quantity,
                UnitPrice = c.Price,
                TotalPrice = c.Price * c.Quantity,
            }).ToList();

            order.CreatedAt = order.OrderDate ?? DateTime.Now;
            order.UserId = 1;
            order.Status = 1;

            string jsonData = JsonConvert.SerializeObject(order, Formatting.Indented);
            Console.WriteLine("Dữ liệu gửi lên API:");
            Console.WriteLine(jsonData);

            using (HttpClient client = new HttpClient())
            {
                var content = new StringContent(jsonData, Encoding.UTF8, "application/json");
                HttpResponseMessage response = await client.PostAsync(UrlGet, content);

                if (response.IsSuccessStatusCode)
                {
                    HttpContext.Session.Remove("CartItems");
                    TempData["SuccessMessage"] = "Tạo đơn hàng thành công!";

                    return order.OrderType == 1 ?
                        RedirectToAction("Index") :
                        RedirectToAction("XuatKho", "Order");
                }
                else
                {
                    TempData["ErrorMessage"] = "Tạo đơn hàng thất bại.";
                    return RedirectToAction("Create");
                }
            }

            return BadRequest("Lỗi không mong muốn xảy ra.");
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddToSessionForUpdate(int orderId, IFormCollection form)
        {
            Console.WriteLine("Đã vào AddToSessionForUpdate");
            Console.WriteLine("OrderId: " + orderId);
            Console.WriteLine("Dữ liệu form (Update): " + JsonConvert.SerializeObject(form));

            var selectedProducts = new List<SelectedProductDto>();
            var productIds = form["SelectedProducts"].Where(x => !string.IsNullOrEmpty(x)).Select(int.Parse).ToList();

            foreach (var productId in productIds)
            {
                var quantityStr = form[$"SelectedProducts[{productId}].Quantity"].FirstOrDefault();
                if (int.TryParse(quantityStr, out int quantity) && quantity > 0)
                {
                    selectedProducts.Add(new SelectedProductDto { ProductId = productId, Quantity = quantity });
                }
            }

            Console.WriteLine("Dữ liệu SelectedProducts (Update): " + JsonConvert.SerializeObject(selectedProducts));

            if (!selectedProducts.Any())
            {
                TempData["ErrorMessage"] = "Không có sản phẩm nào được chọn.";
                return RedirectToAction("Update", new { id = orderId });
            }

            var allProducts = await GetProductAsync();
            var sessionKey = $"UpdateOrderDetails_{orderId}";
            var orderDetailsSession = HttpContext.Session.GetObjectFromJson<List<OrderDetailsDTO>>(sessionKey) ?? new List<OrderDetailsDTO>();

            foreach (var selected in selectedProducts)
            {
                var product = allProducts.FirstOrDefault(p => p.ProductId == selected.ProductId);
                if (product != null)
                {
                    var existingItem = orderDetailsSession.FirstOrDefault(c => c.ProductId == selected.ProductId);
                    if (existingItem != null)
                    {
                        existingItem.Quantity += selected.Quantity;
                        existingItem.TotalPrice = existingItem.UnitPrice * existingItem.Quantity;
                    }
                    else
                    {
                        orderDetailsSession.Add(new OrderDetailsDTO
                        {
                            OrderDetailId = 0, 
                            ProductId = product.ProductId,
                            Quantity = selected.Quantity,
                            UnitPrice = product.Price,
                            TotalPrice = product.Price * selected.Quantity
                        });
                    }
                }
            }

            HttpContext.Session.SetObjectAsJson(sessionKey, orderDetailsSession);
            TempData["SuccessMessage"] = "Cập nhật sản phẩm trong đơn hàng thành công!";
            return RedirectToAction("Update", new { id = orderId });
        }





        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddProductsToSession(IFormCollection form)
        {
      
            Console.WriteLine("Dữ liệu form: " + JsonConvert.SerializeObject(form));

            var selectedProducts = new List<SelectedProductDto>();
            var productIds = form["SelectedProducts"].Where(x => !string.IsNullOrEmpty(x)).Select(int.Parse).ToList();
            foreach (var productId in productIds)
            {
                var quantityStr = form[$"SelectedProducts[{productId}].Quantity"].FirstOrDefault();
                if (int.TryParse(quantityStr, out int quantity) && quantity > 0)
                {
                    selectedProducts.Add(new SelectedProductDto { ProductId = productId, Quantity = quantity });
                }
            }

            Console.WriteLine("Dữ liệu SelectedProducts: " + JsonConvert.SerializeObject(selectedProducts));

            if (selectedProducts == null || !selectedProducts.Any())
            {
                return Json(new { success = false, message = "Không có sản phẩm nào được chọn." });
            }

            var allProducts = await GetProductAsync();
            var cartItems = HttpContext.Session.GetObjectFromJson<List<CartItemDTO>>("CartItems") ?? new List<CartItemDTO>();

            foreach (var selected in selectedProducts)
            {
                var product = allProducts.FirstOrDefault(p => p.ProductId == selected.ProductId);
                if (product != null)
                {
                    var existingItem = cartItems.FirstOrDefault(c => c.ProductId == selected.ProductId);
                    if (existingItem != null)
                    {
                        existingItem.Quantity += selected.Quantity;
                    }
                    else
                    {
                        cartItems.Add(new CartItemDTO
                        {
                            ProductId = product.ProductId,
                            ProductName = product.Name,
                            ImageUrl = product.Images,
                            Price = product.Price,
                            Quantity = selected.Quantity
                        });
                    }
                }
            }

            HttpContext.Session.SetObjectAsJson("CartItems", cartItems);
            return Json(new { success = true, message = "Thêm sản phẩm vào giỏ hàng thành công!" });
        }

        [HttpGet]
        public IActionResult RemoveFromCart(int id)
        {
            var cartItems = HttpContext.Session.GetObjectFromJson<List<CartItemDTO>>("CartItems") ?? new List<CartItemDTO>();
            cartItems.RemoveAll(c => c.ProductId == id);
            HttpContext.Session.SetObjectAsJson("CartItems", cartItems);
            return RedirectToAction("Create");
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult RemoveSessionForUpdate(int orderId, int productId)
        {
            var sessionKey = $"UpdateOrderDetails_{orderId}";
            var orderDetailsSession = HttpContext.Session.GetObjectFromJson<List<OrderDetailsDTO>>(sessionKey) ?? new List<OrderDetailsDTO>();
            orderDetailsSession.RemoveAll(item => item.ProductId == productId);
            HttpContext.Session.SetObjectAsJson(sessionKey, orderDetailsSession);
            return RedirectToAction("Update", new { id = orderId });
        }


        //====================================================
        private async Task<List<ProductDTO>> GetProductAsync()
        {
            using (HttpClient client = new HttpClient())
            {
                using (HttpResponseMessage res = await client.GetAsync(UrlProduct))
                {
                    if (res.IsSuccessStatusCode)
                    {
                        string data = await res.Content.ReadAsStringAsync();
                        return JsonConvert.DeserializeObject<List<ProductDTO>>(data);
                    }
                }
            }
            return new List<ProductDTO>();
        }
        private async Task<List<CustomerDTO>> GetCustomerAsync()
        {
            using (HttpClient client = new HttpClient())
            {
                using (HttpResponseMessage res = await client.GetAsync(UrlCus))
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

        private async Task<List<SupplierDTO>> GetSupplierAsync()
        {
            using (HttpClient client = new HttpClient())
            {
                using (HttpResponseMessage res = await client.GetAsync(UrlSup))
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

        private async Task<List<OrderListDTO>> GetOrderAsync()
        {
            using (HttpClient client = new HttpClient())
            {
                using (HttpResponseMessage res = await client.GetAsync(UrlGet))
                {
                    if (res.IsSuccessStatusCode)
                    {
                        string data = await res.Content.ReadAsStringAsync();
                        return JsonConvert.DeserializeObject<List<OrderListDTO>>(data);
                    }
                }
            }
            return new List<OrderListDTO>();
        }


        private async Task<OrderDTO> GetOrderByIdAsync(int id)
        {
            using (HttpClient client = new HttpClient())
            {
                using (HttpResponseMessage res = await client.GetAsync($"{UrlGet}/{id}"))
                {
                    if (res.IsSuccessStatusCode)
                    {
                        string data = await res.Content.ReadAsStringAsync();
                        return JsonConvert.DeserializeObject<OrderDTO>(data);
                    }
                }
            }
            return null;
        }
    }
}