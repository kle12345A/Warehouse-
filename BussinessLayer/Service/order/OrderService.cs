using AutoMapper;
using BussinessLayer.BaseService;
using DataAccessLayer.Models;
using DataAccessLayer.Repository.order;
using WarehouseDTOs;

namespace BussinessLayer.Service.order
{
    public class OrderService : BaseService<Order>, IOrderService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IMapper _mapper;

        public OrderService(IOrderRepository baseRepository, IMapper mapper) : base(baseRepository)
        {
            _orderRepository = baseRepository;
            _mapper = mapper;
        }

        public async Task<OrderDTO> CreateOrderAsync(OrderDTO orderDto)
        {
           var order = _mapper.Map<Order>(orderDto);
            await AddAsync(order);
            return _mapper.Map<OrderDTO>(order);   
        }

        public async Task<OrderDTO> CreateOrderAsync(OrderCreateDTO orderCreateDTO)
        {
            var order = new Order
            {
                SupplierId = orderCreateDTO.SupplierId,
                OrderDate = orderCreateDTO.OrderDate ?? DateTime.Now,
                OrderType = orderCreateDTO.OrderType,
                Status = orderCreateDTO.Status ?? 0,
                UserId = orderCreateDTO.UserId,
                CreatedAt = orderCreateDTO.CreatedAt,
                UpdatedAt = orderCreateDTO.UpdatedAt
            };

            var orderDetails = orderCreateDTO.OrderDetails.Select(od => new OrderDetail
            {
                ProductId = od.ProductId,
                Quantity = od.Quantity,
                UnitPrice = od.UnitPrice,
                TotalPrice = od.TotalPrice,
                
            }).ToList();

            var createdOrder = await _orderRepository.CreateOrderAsync(order, orderDetails);

            return new OrderDTO
            {
                OrderId = createdOrder.OrderId,
                SupplierId = createdOrder.SupplierId,
                OrderDate = createdOrder.OrderDate,
                OrderType = createdOrder.OrderType,
                Status = createdOrder.Status,
                UserId = createdOrder.UserId,
                CreatedAt = createdOrder.CreatedAt,
                UpdatedAt = createdOrder.UpdatedAt
            };
        }

        public async Task<List<OrderListDTO>> GetAllOrdersAsync()
        {
            return await _orderRepository.GetAllOrdersWithNamesAsync();
        }


        public async Task<OrderDTO> GetOrderByIdAsync(int id)
        {
            var order = await GetByIdAsync(id);
            return _mapper.Map<OrderDTO>(order);

        }

        public async Task<OrderDTO> UpdateOrderAsync(int id, OrderDTO orderDto)
        {
            var existingOrder = await GetByIdAsync(id);
            if (existingOrder != null)
            {
                _mapper.Map(orderDto, existingOrder);
                await UpdateAsync(existingOrder);
                return _mapper.Map<OrderDTO>(existingOrder);
            }
            return null;
        }
    }
}
