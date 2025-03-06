using WarehouseDTOs;

namespace Warehouse.MVC.Models
{
    public class ProductView
    {
        public List<ProductDTO> Products { get; set; }
        public List<CategoryDTO> Categories { get; set; }
        public ProductDTO Product { get; set; }
    }
}
