using WarehouseDTOs;

namespace Warehouse.MVC.Models
{
    public class OrderVIew
    {
        public List<OrderListDTO> OrderListDTO { get; set; }
        public List<OrderDetailsDTO> OrderDetailsDTO { get; set; }

        public List<SupplierDTO> Suppliers { get; set; }
        public List<ProductDTO> Products { get; set; }
        public List<CartItemDTO> CartItems { get; set; }
        public OrderDTO OrderDTOs { get; set; }
        public List<StatusDTO> StatusList { get; set; }
        public List<CustomerDTO> CustomerDTOs { get; set; }
        public List<OrderTypeEnum> OrderTypeList { get; set; }
    }
}
