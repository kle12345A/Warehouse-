using WarehouseDTOs;

namespace Warehouse.MVC.Models
{
    public class CategoryView
    {
        public List<CategoryProduct> Categories { get; set; }
        public CategoryDTO Category { get; set; }
    }
}
