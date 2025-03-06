using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Text;
using Warehouse.MVC.Helpers;
using Warehouse.MVC.Models;
using WarehouseDTOs;

namespace Warehouse.MVC.Controllers
{
    public class OrderController : Controller
    {
        private string UrlGet = "https://localhost:7200/api/Order";
        private string UrlSup = "https://localhost:7200/api/Supplier";
        private string UrlProduct = "https://localhost:7200/api/Products";

        public async Task<IActionResult> Index()
        {
            var ord = await GetOrderAsync();
            var view = new OrderVIew()
            {
                OrderListDTO = ord
            };
            return View(view);
        }

        public async Task<IActionResult> Create()
        {
            var sup = await GetSupplierAsync();
            var pro = await GetProductAsync();
            var cartItems = HttpContext.Session.GetObjectFromJson<List<CartItemDTO>>("CartItems") ?? new List<CartItemDTO>();
            var view = new OrderVIew()
            {
                Products = pro,
                Suppliers = sup,
                CartItems = cartItems
            };
            return View(view);
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
                    return RedirectToAction("Index");
                }
                else
                {
                    TempData["ErrorMessage"] = "Tạo đơn hàng thất bại.";
                    return RedirectToAction("Create");
                }
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddProductsToSession(IFormCollection form)
        {
            // Debug dữ liệu nhận được
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
    }
}