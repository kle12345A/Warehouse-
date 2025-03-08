using WarehouseDTOs;

namespace Warehouse.MVC.Models
{
    public class OrderDetailView
    {
        public List<OrderDetailDTO> OrderDetailDTOs { get; set; } = new List<OrderDetailDTO>();
        public OrderDetailWithSupplierDTO OrderDetailWithSupplier { get; set; } = new();
        public OrderDetailWithCustomerDTO OrderDetailWithCustomer { get; set; } = new();

        public List<OrderCreateDTO> OrderCreateDTOs { get; set;} 

    }
}
