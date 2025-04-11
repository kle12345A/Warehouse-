using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Diagnostics;
using Warehouse.MVC.Models;
using WarehouseDTOs;

namespace Warehouse.MVC.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly string _totalExportAmountUrl = "https://localhost:7200/api/Order/total-export-amount-current-month";
        private readonly string _totalCustomersUrl = "https://localhost:7200/api/Customer/TotalCustomers";
        private readonly string _totalSuppliersUrl = "https://localhost:7200/api/Supplier/TotalSuppliers";
        private readonly string _totalStockUrl = "https://localhost:7200/api/Products/TotalProduct";
        private readonly string _top5product = "https://localhost:7200/api/Order/top-5-export-products";



        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            // Gọi từng API riêng lẻ
            var totalExportAmount = await GetTotalExportAmountAsync();
            var totalCustomers = await GetTotalCustomersAsync();
            var totalSuppliers = await GetTotalSuppliersAsync();
            var totalStock = await GetTotalStockAsync();
            var top5 = await GetProductexportAsync();
            // Đẩy dữ liệu ra ViewBag
            ViewBag.TotalExportAmount = totalExportAmount;
            ViewBag.TotalCustomers = totalCustomers;
            ViewBag.TotalSuppliers = totalSuppliers;
            ViewBag.TotalStock = totalStock;
            var view = new HomeView
            {
                productExports = top5
            };
            return View(view);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult AccessDenied()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }


        private async Task<List<ProductExportDTO>> GetProductexportAsync()
        {
            using (HttpClient client = new HttpClient())
            {
                using (HttpResponseMessage res = await client.GetAsync(_top5product))
                {
                    if (res.IsSuccessStatusCode)
                    {
                        string data = await res.Content.ReadAsStringAsync();
                        return JsonConvert.DeserializeObject<List<ProductExportDTO>>(data);
                    }
                }
            }
            return new List<ProductExportDTO>();
        }
        private async Task<int> GetTotalStockAsync()
        {
            using (HttpClient client = new HttpClient())
            {
                using (HttpResponseMessage res = await client.GetAsync(_totalStockUrl))
                {
                    if (res.IsSuccessStatusCode)
                    {
                        string data = await res.Content.ReadAsStringAsync();
                        _logger.LogInformation("API Response (Total Stock): {ResponseData}", data);

                        if (int.TryParse(data, out int totalStock))
                        {
                            return totalStock;
                        }
                        else
                        {
                            _logger.LogError("Failed to parse total stock response: {ResponseData}", data);
                            return 0;
                        }
                    }
                    else
                    {
                        string errorData = await res.Content.ReadAsStringAsync();
                        _logger.LogWarning("Failed to fetch total stock. Status code: {StatusCode}, Response: {ErrorData}", res.StatusCode, errorData);
                        return 0;
                    }
                }
            }
        }

        private async Task<decimal> GetTotalExportAmountAsync()
        {
            using (HttpClient client = new HttpClient())
            {
                using (HttpResponseMessage res = await client.GetAsync(_totalExportAmountUrl))
                {
                    if (res.IsSuccessStatusCode)
                    {
                        string data = await res.Content.ReadAsStringAsync();
                        _logger.LogInformation("API Response (Total Export Amount): {ResponseData}", data);

                        try
                        {
                            var response = JsonConvert.DeserializeObject<TotalExportAmountResponse>(data);
                            return response.TotalExportAmount;
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex, "Error deserializing API response: {ResponseData}", data);
                            return 0;
                        }
                    }
                    else
                    {
                        string errorData = await res.Content.ReadAsStringAsync();
                        _logger.LogWarning("Failed to fetch total export amount. Status code: {StatusCode}, Response: {ErrorData}", res.StatusCode, errorData);
                        return 0;
                    }
                }
            }
        }

        private async Task<int> GetTotalCustomersAsync()
        {
            using (HttpClient client = new HttpClient())
            {
                using (HttpResponseMessage res = await client.GetAsync(_totalCustomersUrl))
                {
                    if (res.IsSuccessStatusCode)
                    {
                        string data = await res.Content.ReadAsStringAsync();
                        _logger.LogInformation("API Response (Total Customers): {ResponseData}", data);

                        if (int.TryParse(data, out int totalCustomers))
                        {
                            return totalCustomers;
                        }
                        else
                        {
                            _logger.LogError("Failed to parse total customers response: {ResponseData}", data);
                            return 0;
                        }
                    }
                    else
                    {
                        string errorData = await res.Content.ReadAsStringAsync();
                        _logger.LogWarning("Failed to fetch total customers. Status code: {StatusCode}, Response: {ErrorData}", res.StatusCode, errorData);
                        return 0;
                    }
                }
            }
        }

        private async Task<int> GetTotalSuppliersAsync()
        {
            using (HttpClient client = new HttpClient())
            {
                using (HttpResponseMessage res = await client.GetAsync(_totalSuppliersUrl))
                {
                    if (res.IsSuccessStatusCode)
                    {
                        string data = await res.Content.ReadAsStringAsync();
                        _logger.LogInformation("API Response (Total Suppliers): {ResponseData}", data);

                        if (int.TryParse(data, out int totalSuppliers))
                        {
                            return totalSuppliers;
                        }
                        else
                        {
                            _logger.LogError("Failed to parse total suppliers response: {ResponseData}", data);
                            return 0;
                        }
                    }
                    else
                    {
                        string errorData = await res.Content.ReadAsStringAsync();
                        _logger.LogWarning("Failed to fetch total suppliers. Status code: {StatusCode}, Response: {ErrorData}", res.StatusCode, errorData);
                        return 0;
                    }
                }
            }
        }
    }
}
