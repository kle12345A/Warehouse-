using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WarehouseDTOs
{
    public class OrderDetailDTO
    {
        public int OrderDetailID { get; set; }
        public string? Image { get; set; }
        public int OrderID { get; set; }
        public int ProductID { get; set; }
        public string ProductName { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal TotalPrice { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
    public class OrderDetailWithSupplierDTO
    {
        public int OrderId { get; set; }
        public string SupplierName { get; set; }
        public string SupplierPhone { get; set; }
        public OrderStatus OrderStatus { get; set; }
        public OrderTypeEnum OrderTypeEnum { get; set; }
        public string SupplierEmail { get; set; }
        public string SupplierAddress { get; set; }
        public List<OrderDetailDTO> OrderDetails { get; set; } = new List<OrderDetailDTO>();
    }

    public class OrderDetailWithCustomerDTO
    {
        public int OrderId { get; set; }
        public string FullName { get; set; }
        public string Phone { get; set; }
        public OrderStatus OrderStatus { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
        public List<OrderDetailDTO> OrderDetails { get; set; } = new List<OrderDetailDTO>();
    }


}
