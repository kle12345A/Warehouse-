using WarehouseDTOs;

namespace Warehouse.MVC.Models
{
    public class SupplierView
    {
        public List<SupplierDTO> Suppliers { get; set; }
        public SupplierDTO Supplier { get; set; }
    }
}
