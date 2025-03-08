using DataAccessLayer.BaseRepository;
using DataAccessLayer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WarehouseDTOs;

namespace DataAccessLayer.Repository.order
{
    public interface IOrderRepository : IBaseRepository<Order>
    {
        Task<List<OrderListDTO>> GetAllOrdersWithNamesAsync();
        Task<Order> CreateOrderAsync(Order order, List<OrderDetail> orderDetails);
        Task<Order> GetByIdWithDetailsAsync(int id);
        Task<bool> UpdateOrderStatusAsync(int orderId, OrderStatus newStatus);


    }
}
