using DataAccessLayer.BaseRepository;
using DataAccessLayer.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WarehouseDTOs;

namespace DataAccessLayer.Repository.order
{
    public class OrderRepository : BaseRepository<Order>, IOrderRepository
    {
        private readonly WarehouseDbContext _context;
        public OrderRepository(WarehouseDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<Order> CreateOrderAsync(Order order, List<OrderDetail> orderDetails)
        {
            await _context.Orders.AddAsync(order);
            await _context.SaveChangesAsync(); 

            foreach (var detail in orderDetails)
            {
                detail.OrderId = order.OrderId;
            }

            await _context.OrderDetails.AddRangeAsync(orderDetails);
            await _context.SaveChangesAsync();

            return order;
        }

        public async Task<List<OrderListDTO>> GetAllOrdersWithNamesAsync()
        {
            var orders = await _context.Orders
                .Select(o => new OrderListDTO
                {
                    OrderId = o.OrderId,
                    SupplierName = o.Supplier.Name,
                    OrderType = (OrderTypeEnum?)o.OrderType,
                    Customer =  o.Customer.FullName,
                    UserName = o.User.FullName,
                    OrderDate = o.OrderDate,
                    Status = (OrderStatus)o.Status,
                    
                })
                .ToListAsync();

            return orders;
        }

        public async Task<Order> GetByIdWithDetailsAsync(int id)
        {
            return await _context.Orders
                .Include(o => o.OrderDetails)
                .FirstOrDefaultAsync(o => o.OrderId == id);
        }

        public async Task<List<Order>> GetExportOrdersForCurrentMonthAsync()
        {
            var currentMonth = DateTime.Now.Month;
            var currentYear = DateTime.Now.Year;

            return await _context.Orders
                .Where(o => o.OrderType == 2 &&
                            o.OrderDate.HasValue &&
                            o.OrderDate.Value.Month == currentMonth &&
                            o.OrderDate.Value.Year == currentYear)
                .ToListAsync();
        }

        public Task<bool> UpdateOrderStatusAsync(int orderId, OrderStatus newStatus)
        {
            throw new NotImplementedException();
        }
    }
}
