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
