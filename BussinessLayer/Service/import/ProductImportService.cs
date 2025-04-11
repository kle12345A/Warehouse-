using AutoMapper;
using BussinessLayer.BaseService;
using BussinessLayer.Service.product;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using DataAccessLayer.Models;
using DataAccessLayer.Repository.product;
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
        private readonly IProductRepository _productRepository;
        public ProductImportService(IProductService productService, IMapper mapper, Cloudinary cloudinary, IProductRepository productRepository)
        {
            _productRepository = productRepository;
            _productService = productService;
            _mapper = mapper;
            _cloudinary = cloudinary;
        }

        public async Task<List<ProductImportResultDTO>> ImportProductsFromExcelAsync(Stream excelStream)
        {
            //var results = new List<ProductImportResultDTO>();
            //var productRecords = await ReadExcelFileAsync(excelStream);

            //foreach (var record in productRecords)
            //{
            //    var result = await ProcessProductImportAsync(record);
            //    results.Add(result);
            //}

            return null;
        }

        public async Task<List<ProductImportWithErrorsDTO>> ReadExcelFileAsync(Stream fileStream)
        {
            var results = new List<ProductImportWithErrorsDTO>();
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            using (var package = new ExcelPackage(fileStream))
            {
                var worksheet = package.Workbook.Worksheets[0];
                var rowCount = worksheet.Dimension?.Rows ?? 0;

                if (rowCount < 3)
                {
                    var errorResult = new ProductImportWithErrorsDTO
                    {
                        Product = new ProductImportDTO(),
                        Errors = new Dictionary<string, string> { { "General", "File Excel không có dữ liệu hoặc định dạng không đúng (yêu cầu ít nhất 3 dòng, bao gồm tiêu đề)." } },
                        RowNumber = -1
                    };
                    results.Add(errorResult);
                    return results;
                }

                for (int row = 3; row <= rowCount; row++)
                {
                    // Kiểm tra xem dòng có dữ liệu không (cột A đến H)
                    bool isRowEmpty = true;
                    for (int col = 1; col <= 8; col++) // Cột A đến H
                    {
                        if (!string.IsNullOrWhiteSpace(worksheet.Cells[row, col].Text))
                        {
                            isRowEmpty = false;
                            break;
                        }
                    }

                    // Bỏ qua dòng nếu trống
                    if (isRowEmpty)
                    {
                        continue;
                    }

                    var errors = new Dictionary<string, string>();
                    var localImagePath = worksheet.Cells[row, 8].Text; // Cột H (ImagePath)
                    string imageUrl = null;

                    // Validate từng trường
                    string name = worksheet.Cells[row, 1].Text; // Cột A
                    if (string.IsNullOrWhiteSpace(name))
                        errors["Name"] = "Tên sản phẩm không được để trống";
                    else if (name.Length > 100)
                        errors["Name"] = "Tên sản phẩm vượt quá 100 ký tự";
                    else if (await _productRepository.ProductExistsByNameAsync(name))
                    {
                        errors["Name"] = "Tên sản phẩm đã tồn tại trong hệ thống";
                    }
                        string unit = worksheet.Cells[row, 3].Text; // Cột C
                    if (string.IsNullOrWhiteSpace(unit))
                        errors["Unit"] = "Đơn vị không được để trống";

                    int qty = 0;
                    if (!int.TryParse(worksheet.Cells[row, 4].Text, out qty) || qty < 0) // Cột D
                        errors["AvailableQuantity"] = "Số lượng khả dụng phải là số không âm";

                    decimal price = ParseCurrency(worksheet.Cells[row, 5].Text); // Cột E
                    if (price < 0)
                        errors["Price"] = "Giá phải là số không âm";

                    decimal costPrice = ParseCurrency(worksheet.Cells[row, 6].Text); // Cột F
                    if (costPrice < 0)
                        errors["CostPrice"] = "Giá vốn phải là số không âm";

                    int catId = 0;
                    if (!int.TryParse(worksheet.Cells[row, 7].Text, out catId) || catId <= 0) // Cột G
                        errors["CategoryId"] = "CategoryId phải là số nguyên dương";

                    // Xử lý ảnh nếu không có lỗi liên quan đến ảnh
                    if (!errors.ContainsKey("ImagePath") && !string.IsNullOrEmpty(localImagePath))
                    {
                        if (!File.Exists(localImagePath))
                        {
                            errors["ImagePath"] = $"Đường dẫn ảnh '{localImagePath}' không tồn tại";
                        }
                        else
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
                                errors["ImagePath"] = $"Lỗi upload ảnh '{localImagePath}': {ex.Message}";
                            }
                        }
                    }

                    // Tạo bản ghi
                    var record = new ProductImportDTO
                    {
                        Name = name,
                        Description = worksheet.Cells[row, 2].Text, // Cột B
                        Unit = unit,
                        AvailableQuantity = qty,
                        Price = price,
                        CostPrice = costPrice,
                        CategoryId = catId,
                        ImagePath = imageUrl
                    };

                    // Thêm vào danh sách kết quả
                    results.Add(new ProductImportWithErrorsDTO
                    {
                        Product = record,
                        Errors = errors,
                        RowNumber = row
                    });
                }
            }

            return results;
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
                Console.WriteLine($" Parsed value: {result}");
                return result;
            }

            Console.WriteLine($" Lỗi parse giá trị: {value}");
            return 0;
        }



        //private async Task<ProductImportResultDTO> ProcessProductImportAsync(ProductImportDTO importDto)
        //{
        //    var result = new ProductImportResultDTO
        //    {
        //        Name = importDto.Name,
        //        Success = false
        //    };

        //    try
        //    {
        //        string imageUrl = null;
        //        if (!string.IsNullOrEmpty(importDto.ImagePath) && File.Exists(importDto.ImagePath))
        //        {
        //            using (var stream = new FileStream(importDto.ImagePath, FileMode.Open, FileAccess.Read))
        //            {
        //                var fileNameWithoutExt = Path.GetFileNameWithoutExtension(importDto.ImagePath);
        //                var uploadParams = new ImageUploadParams
        //                {
        //                    File = new FileDescription(importDto.ImagePath, stream),
        //                    PublicId = $"{fileNameWithoutExt}_{Guid.NewGuid()}"
        //                };

        //                var uploadResult = await _cloudinary.UploadAsync(uploadParams);
        //                imageUrl = uploadResult.SecureUrl.ToString();
        //            }
        //        }

        //        var productDto = new ProductDTO
        //        {
        //            Name = importDto.Name,
        //            Description = importDto.Description,
        //            Unit = importDto.Unit,
        //            AvailableQuantity = importDto.AvailableQuantity,
        //            Price = importDto.Price,
        //            CostPrice = importDto.CostPrice,
        //            CategoryId = importDto.CategoryId,
        //            Images = imageUrl
        //        };

        //        var createdProduct = await _productService.CreateProductAsync(productDto);
        //        result.ProductID = createdProduct.ProductId;
        //        result.ImageUrl = imageUrl;
        //        result.Success = true;
        //    }
        //    catch (Exception ex)
        //    {
        //        result.ErrorMessage = $"Lỗi khi import sản phẩm {importDto.Name}: {ex.Message}";
        //    }

        //    return result;
        //}
    }
}