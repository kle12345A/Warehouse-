using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WarehouseDTOs
{
    public class CategoryDTO
    {
        public int CategoryId { get; set; }

        public string Name { get; set; } = null!;

        public string? Description { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }
    }
    public class CategoryUpdateDTO
    {
        

        public string Name { get; set; } = null!;

        public string? Description { get; set; }

        public DateTime? UpdatedAt { get; set; }
    }
    public class CategoryProduct
    {
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }
        public int ProductCount { get; set; }
    }
}
