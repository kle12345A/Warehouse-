using AutoMapper;
using BussinessLayer.BaseService;
using BussinessLayer.Service.product;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using DataAccessLayer.Models;
using Microsoft.AspNetCore.Http;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using WarehouseDTOs;

namespace BussinessLayer.Service.import
{
    public class ProductImportService : IProductImportService
    {
        private readonly IProductService _productService;
        private readonly IMapper _mapper;
        private readonly Cloudinary _cloudinary;

        public ProductImportService(IProductService productService, IMapper mapper, Cloudinary cloudinary)
        {
            _productService = productService;
            _mapper = mapper;
            _cloudinary = cloudinary;
        }

        public async Task<List<ProductImportResultDTO>> ImportProductsFromExcelAsync(Stream excelStream)
        {
            var results = new List<ProductImportResultDTO>();
            var productRecords = await ReadExcelFileAsync(excelStream);

            foreach (var record in productRecords)
            {
                var result = await ProcessProductImportAsync(record);
                results.Add(result);
            }

            return results;
        }

        public async Task<List<ProductImportDTO>> ReadExcelFileAsync(Stream fileStream)
        {
            var records = new List<ProductImportDTO>();
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            using (var package = new ExcelPackage(fileStream))
            {
                var worksheet = package.Workbook.Worksheets[0];
                var rowCount = worksheet.Dimension.Rows;

                for (int row = 3; row <= rowCount; row++)
                {
                    var localImagePath = worksheet.Cells[row, 8].Text; // Cột chứa đường dẫn ảnh cục bộ
                    string imageUrl = null;

                    if (!string.IsNullOrEmpty(localImagePath) && File.Exists(localImagePath))
                    {
                        try
                        {
                            using (var stream = new FileStream(localImagePath, FileMode.Open, FileAccess.Read))
                            {
                                var fileNameWithoutExt = Path.GetFileNameWithoutExtension(localImagePath);
                                var uploadParams = new ImageUploadParams
                                {
                                    File = new FileDescription(localImagePath, stream),
                                    PublicId = $"{fileNameWithoutExt}_{Guid.NewGuid()}" 
                                };

                                var uploadResult = await _cloudinary.UploadAsync(uploadParams);
                                imageUrl = uploadResult.SecureUrl.ToString();
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Lỗi upload ảnh {localImagePath}: {ex.Message}");
                            imageUrl = null; 
                        }
                    }

                    var record = new ProductImportDTO
                    {
                        Name = worksheet.Cells[row, 1].Text,
                        Description = worksheet.Cells[row, 2].Text,
                        Unit = worksheet.Cells[row, 3].Text,
                        AvailableQuantity = int.TryParse(worksheet.Cells[row, 4].Text, out int qty) ? qty : 0,
                        Price = ParseCurrency(worksheet.Cells[row, 5].Text),
                        CostPrice = ParseCurrency(worksheet.Cells[row, 6].Text),
                        CategoryId = int.TryParse(worksheet.Cells[row, 7].Text, out int catId) ? catId : 0,
                        ImagePath = imageUrl 
                    };
                    Console.WriteLine($"Price: {record.Price}, CostPrice: {record.CostPrice}, ImagePath: {record.ImagePath}");
                    records.Add(record);
                }
            }

            return records;
        }

        private decimal ParseCurrency(string value)
        {
            if (string.IsNullOrEmpty(value))
                return 0;

            Console.WriteLine($"Raw Price: {value}");

            value = value.Replace("đ", "").Trim();

            // Xóa toàn bộ khoảng trắng
            value = value.Replace(" ", "");

            // Nếu có dấu phẩy thập phân (hệ số kiểu Việt Nam)
            if (value.Contains(",") && value.LastIndexOf(",") > value.LastIndexOf("."))
            {
                // Xóa dấu chấm ngăn cách nghìn
                value = value.Replace(".", "");

                // Đổi dấu phẩy thành dấu chấm (chuẩn dạng số thập phân của `decimal`)
                value = value.Replace(",", ".");
            }
            else
            {
                // Xóa dấu phẩy ngăn cách nghìn nếu có
                value = value.Replace(",", "");
            }

            Console.WriteLine($"Processed Price: {value}");

            // Parse số theo chuẩn quốc tế
            if (decimal.TryParse(value, NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out decimal result))
            {
                Console.WriteLine($"✅ Parsed value: {result}");
                return result;
            }

            Console.WriteLine($"❌ Lỗi parse giá trị: {value}");
            return 0;
        }



        private async Task<ProductImportResultDTO> ProcessProductImportAsync(ProductImportDTO importDto)
        {
            var result = new ProductImportResultDTO
            {
                Name = importDto.Name,
                Success = false
            };

            try
            {
                string imageUrl = null;
                if (!string.IsNullOrEmpty(importDto.ImagePath) && File.Exists(importDto.ImagePath))
                {
                    using (var stream = new FileStream(importDto.ImagePath, FileMode.Open, FileAccess.Read))
                    {
                        var fileNameWithoutExt = Path.GetFileNameWithoutExtension(importDto.ImagePath);
                        var uploadParams = new ImageUploadParams
                        {
                            File = new FileDescription(importDto.ImagePath, stream),
                            PublicId = $"{fileNameWithoutExt}_{Guid.NewGuid()}"
                        };

                        var uploadResult = await _cloudinary.UploadAsync(uploadParams);
                        imageUrl = uploadResult.SecureUrl.ToString();
                    }
                }

                var productDto = new ProductDTO
                {
                    Name = importDto.Name,
                    Description = importDto.Description,
                    Unit = importDto.Unit,
                    AvailableQuantity = importDto.AvailableQuantity,
                    Price = importDto.Price,
                    CostPrice = importDto.CostPrice,
                    CategoryId = importDto.CategoryId,
                    Images = imageUrl
                };

                var createdProduct = await _productService.CreateProductAsync(productDto);
                result.ProductID = createdProduct.ProductId;
                result.ImageUrl = imageUrl;
                result.Success = true;
            }
            catch (Exception ex)
            {
                result.ErrorMessage = $"Lỗi khi import sản phẩm {importDto.Name}: {ex.Message}";
            }

            return result;
        }
    }
}