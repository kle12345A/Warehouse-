using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WarehouseDTOs
{
    
    
    public class ProductDTO
    {
        public int ProductId { get; set; }

        public string Name { get; set; } = null!;

        public string? Description { get; set; }

        public string? Images { get; set; }

        public string? Unit { get; set; }

       
        public int? AvailableQuantity { get; set; }
        public string? CategoryName { get; set; }

        public decimal Price { get; set; }

        public decimal CostPrice { get; set; }

        public int? CategoryId { get; set; }

        public DateTime CreatedAt { get; set; }

        public int CreatedBy { get; set; }

        public DateTime? UpdatedAt { get; set; }

        public int? UpdatedBy { get; set; }
    }
    public class CreateProductRequest
    {
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public string? Unit { get; set; }
        public int? AvailableQuantity { get; set; }
        public decimal Price { get; set; }
        public decimal CostPrice { get; set; }
        public int? CategoryId { get; set; }
        public int CreatedBy { get; set; }
        public IFormFile? ImageFile { get; set; } // File ảnh
    }
    public class CreateProductRequest2
    {
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public string? Unit { get; set; }
        public int? AvailableQuantity { get; set; }
        public decimal Price { get; set; }
        public decimal CostPrice { get; set; }
        public int? CategoryId { get; set; }
        public int CreatedBy { get; set; }
       public string? Images {  get; set; }
    }
    public class UpdateProductRequest
    {
        public int ProductId { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public string? Unit { get; set; }
        public int? AvailableQuantity { get; set; }
        public decimal? Price { get; set; }
        public decimal? CostPrice { get; set; }
        public int? CategoryId { get; set; }
        public IFormFile? ImageFile { get; set; }
        public string? Images { get; set; }
        private DateTime? UpdateAt { get; set; } = DateTime.Now;
        private int? UpdatedBy { get; set; } = 1;
    }


    //============================
    public class ProductImportDTO
    {
        [Required(ErrorMessage = "Tên sản phẩm không được để trống")]
        [StringLength(100, ErrorMessage = "Tên sản phẩm không được vượt quá 100 ký tự")]
        public string Name { get; set; }

        public string Description { get; set; }

        [Required(ErrorMessage = "Đơn vị không được để trống")]
        public string Unit { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Số lượng khả dụng phải là số không âm")]
        public int AvailableQuantity { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Giá phải là số không âm")]
        public decimal Price { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Giá vốn phải là số không âm")]
        public decimal CostPrice { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "CategoryId phải là số nguyên dương")]
        public int CategoryId { get; set; }

        public string ImagePath { get; set; }
    }
    public class ProductImportWithErrorsDTO
    {
        public ProductImportDTO Product { get; set; }
        public Dictionary<string, string> Errors { get; set; } = new Dictionary<string, string>(); // Lưu lỗi theo tên trường
        public int RowNumber { get; set; }
    }
    public class ProductExportDTO
    {
        public int ProductId { get; set; }
        public string ImageUrl { get; set; }
        public string ProductName { get; set; }
        public int TotalQuantityExported { get; set; }
    }
    public class ProductImportResultDTO
    {
        public int ProductID { get; set; }
        public string Name { get; set; }
        public string ImageUrl { get; set; } // URL từ Cloudinary
        public bool Success { get; set; }
        public string ErrorMessage { get; set; }
    }
}
