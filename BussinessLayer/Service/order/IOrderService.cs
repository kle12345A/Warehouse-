using BussinessLayer.BaseService;
using BussinessLayer.Service.product;
using DataAccessLayer.Models;
using DataAccessLayer.Repository.product;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WarehouseDTOs;

namespace BussinessLayer.Service.order
{
    public interface IOrderService : IBaseService<Order>
    {
        Task<List<OrderListDTO>> GetAllOrdersAsync();
        Task<OrderDTO> GetOrderByIdAsync(int id);
        Task<OrderDTO> CreateOrderAsync(OrderDTO orderDto);
        Task<OrderDTO> UpdateOrderAsync(int id, OrderDTO orderDto);
        Task<OrderDTO> CreateOrderAsync(OrderCreateDTO orderCreateDTO);
    }
}
