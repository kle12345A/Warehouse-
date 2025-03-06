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

        public int? Quantity { get; set; }

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
        public int? Quantity { get; set; }
        public int? AvailableQuantity { get; set; }
        public decimal Price { get; set; }
        public decimal CostPrice { get; set; }
        public int? CategoryId { get; set; }
        public IFormFile? ImageFile { get; set; } // File ảnh
    }
    public class UpdateProductRequest
    {
        public int ProductId { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public string? Unit { get; set; }
        public int? Quantity { get; set; }
        public int? AvailableQuantity { get; set; }
        public decimal? Price { get; set; }
        public decimal? CostPrice { get; set; }
        public int? CategoryId { get; set; }
        public IFormFile? ImageFile { get; set; }
        public string? Images { get; set; }
        private DateTime? UpdateAt { get; set; } = DateTime.Now;
        private int? UpdatedBy { get; set; } = 1;
    }
}
