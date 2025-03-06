using WarehouseDTOs;

namespace Warehouse.MVC.Models
{
    public class OrderDetailView
    {
        public List<OrderDetailDTO> OrderDetailDTOs { get; set; } = new List<OrderDetailDTO>();
        public OrderDetailWithSupplierDTO OrderDetailWithSupplier { get; set; } = new();
    }
}
