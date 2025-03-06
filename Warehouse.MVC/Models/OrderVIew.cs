using WarehouseDTOs;

namespace Warehouse.MVC.Models
{
    public class OrderVIew
    {
        public List<OrderListDTO> OrderListDTO { get; set; }
        public List<SupplierDTO> Suppliers { get; set; }
        public List<ProductDTO> Products { get; set; }
        public List<CartItemDTO> CartItems { get; set; }
    }
}
