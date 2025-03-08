using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WarehouseDTOs
{
    public class OrderDTO
    {
        public int OrderId { get; set; }

        public int? SupplierId { get; set; }

        public DateTime? OrderDate { get; set; }

        public int? OrderType { get; set; }
        public int? CustomerId { get; set; }
        public int? Status { get; set; }

        public int UserId { get; set; }


        public List<OrderDetailsDTO> OrderDetails { get; set; } = new();

        public DateTime CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }
    }
    public class OrderDetailsDTO
    {
        public int OrderDetailId { get; set; }
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal TotalPrice { get; set; }
    }


    public class OrderListDTO
    {
        public int OrderId { get; set; }
        public string? SupplierName { get; set; }
        public OrderTypeEnum? OrderType { get; set; }
        public string? UserName { get; set; }
        public string? Customer {  get; set; }
        public DateTime? OrderDate { get; set; }
        public OrderStatus? Status {  get; set; }
    }
    public class OrderCreateDTO
    {
        public int? SupplierId { get; set; }
        public DateTime? OrderDate { get; set; }
        public int? OrderType { get; set; }
        public int UserId { get; set; }
        public int? CustomerId {  get; set; }
        public int? Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        public List<OrderDetailCreateDTO> OrderDetails { get; set; } = new List<OrderDetailCreateDTO>();
    }
    public class OrderUpdateDTO
    {
        public int OrderId { get; set; }
        public int? SupplierId { get; set; }
        public DateTime? OrderDate { get; set; }
        public int? OrderType { get; set; }
        public int UserId { get; set; }
        public int? Status { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public List<OrderDetailUpdateDTO> OrderDetails { get; set; } = new List<OrderDetailUpdateDTO>();
    }
    public class OrderUpdateStatusDTO
    {
        public int OrderId { get; set; }
       
        public int? Status { get; set; }
    
        public DateTime? UpdatedAt { get; set; }
       
    }
    public class OrderDetailCreateDTO
    {
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal TotalPrice { get; set; }
      
    }
    public class OrderDetailUpdateDTO
    {
        public int? OrderDetailId { get; set; }
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal TotalPrice { get; set; }
    }
    public class SelectedProductDto
    {
        public int ProductId { get; set; }
        public int Quantity { get; set; }
    }
    public enum OrderTypeEnum
    {
        NhapKho = 1,
        XuatKho = 2,
        TraHang = 3
    }
    public enum OrderStatus
    {
        Pending = 1,        // Chờ xử lý
        Approved = 2,       // Đã duyệt
       
    }
}
