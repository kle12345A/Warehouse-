using DataAccessLayer.BaseRepository;
using DataAccessLayer.Models;
using Microsoft.EntityFrameworkCore;
using WarehouseDTOs;

namespace DataAccessLayer.Repository.orderdetail
{
    public class OrderDetailRepository : BaseRepository<OrderDetail>, IOrderDetailRepository
    {
        private readonly WarehouseDbContext _context;
        public OrderDetailRepository(WarehouseDbContext context) : base(context)
        {
            _context = context;
        }

       public async Task<List<OrderDetail>> GetByOrderIdAsync(int orderId)
    {
        return await _context.OrderDetails
            .Where(od => od.OrderId == orderId)
            .ToListAsync();
    }

        public async Task<OrderDetailWithCustomerDTO> GetOrderDetailsWithCutsomerByOrderIdAsync(int orderId)
        {
            var order = await _context.Orders
                .Include(o => o.Customer)
                .Include(o => o.OrderDetails)
                    .ThenInclude(od => od.Product)
                .FirstOrDefaultAsync(o => o.OrderId == orderId);

            if (order == null)
            {
                return null;
            }

            var result = new OrderDetailWithCustomerDTO()
            {
                OrderId = order.OrderId,
                FullName = order.Customer.FullName,
                Phone = order.Customer.Phone,
                 Email = order.Customer.Email,
                OrderStatus = (OrderStatus)order.Status,
                Address = order.Customer.Address,
                OrderDetails = order.OrderDetails.Select(od => new OrderDetailDTO
                {
                    OrderDetailID = od.OrderDetailId,
                    Image = od.Product.Images,
                    ProductName = od.Product.Name,
                    Quantity = od.Quantity,
                    UnitPrice = od.UnitPrice,
                    TotalPrice = od.TotalPrice,

                }).ToList()
            };

            return result;
        }

        public async Task<OrderDetailWithSupplierDTO> GetOrderDetailsWithSupplierByOrderIdAsync(int orderId)
        {
            var order = await _context.Orders
                .Include(o => o.Supplier)
                .Include(o => o.OrderDetails)
                    .ThenInclude(od => od.Product)
                .FirstOrDefaultAsync(o => o.OrderId == orderId);

            if (order == null)
            {
                return null;
            }

            var result = new OrderDetailWithSupplierDTO
            {
                OrderId = order.OrderId,
                SupplierName = order.Supplier.Name,
                SupplierPhone = order.Supplier.Phone,
                SupplierEmail = order.Supplier.Email,
                OrderStatus = (OrderStatus)order.Status,
                OrderTypeEnum = (OrderTypeEnum)order.OrderType,
                SupplierAddress = order.Supplier.Address,
                OrderDetails = order.OrderDetails.Select(od => new OrderDetailDTO
                {
                    OrderDetailID = od.OrderDetailId,
                    Image = od.Product.Images,
                    ProductName = od.Product.Name,
                    Quantity = od.Quantity,
                    UnitPrice = od.UnitPrice,
                    TotalPrice = od.TotalPrice,
                    
                }).ToList()
            };

            return result;
        }

    }
}
