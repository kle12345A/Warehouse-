using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
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
        public string Name { get; set; }
        public string Description { get; set; }
        public string Unit { get; set; }
        public int AvailableQuantity { get; set; }
        public decimal Price { get; set; }
        public decimal CostPrice { get; set; }
        public int CategoryId { get; set; }
        public string ImagePath { get; set; } // Đường dẫn ảnh cục bộ từ file Excel
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
