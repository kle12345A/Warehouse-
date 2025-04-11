using AutoMapper;
using BussinessLayer.BaseService;
using DataAccessLayer.Models;
using DataAccessLayer.Repository.order;
using DataAccessLayer.Repository.orderdetail;
using DataAccessLayer.Repository.product;
using WarehouseDTOs;

namespace BussinessLayer.Service.order
{
    public class OrderService : BaseService<Order>, IOrderService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IProductRepository _productRepository;
        private readonly IOrderDetailRepository _orderDetailRepository;
        private readonly IMapper _mapper;

        public OrderService(IOrderRepository baseRepository,IProductRepository product, IOrderDetailRepository orderDetail ,IMapper mapper) : base(baseRepository)
        {
            _productRepository = product;
            _orderDetailRepository = orderDetail;
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
                CustomerId = orderCreateDTO.CustomerId,
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

        public async Task<decimal> GetTotalExportAmountForCurrentMonthAsync()
        {
            // Lấy tháng và năm hiện tại
            var currentMonth = DateTime.Now.Month;
            var currentYear = DateTime.Now.Year;

            // Lấy danh sách các đơn hàng xuất kho (OrderType = 2) trong tháng hiện tại
            var exportOrders = await _orderRepository.GetAllAsync();
            exportOrders = exportOrders
                .Where(o => o.OrderType == 2 &&
                            o.Status == 2 &&
                            o.OrderDate.HasValue && // Kiểm tra OrderDate không null
                            o.OrderDate.Value.Month == currentMonth &&
                            o.OrderDate.Value.Year == currentYear)
                .ToList();

            if (!exportOrders.Any())
            {
                return 0; // Nếu không có đơn hàng nào, trả về 0
            }

            // Lấy danh sách OrderID của các đơn hàng xuất kho
            var orderIds = exportOrders.Select(o => o.OrderId).ToList();

            // Lấy tất cả OrderDetails liên quan đến các đơn hàng xuất kho
            var orderDetails = await _orderDetailRepository.GetByOrderIdsAsync(orderIds);

            // Tính tổng TotalPrice
            var totalExportAmount = orderDetails.Sum(od => od.TotalPrice);

            return totalExportAmount;
        }

        public Task<OrderDTO> UpdateOrderAsync(int id, OrderDTO orderDto)
        {
            throw new NotImplementedException();
        }

        public async Task<OrderDTO> UpdateStatusOrder(int id, OrderUpdateStatusDTO orderUpdateStatusDTO)
        {
            var order = await _orderRepository.GetByIdAsync(id);
            if (order == null)
            {
                throw new KeyNotFoundException($"Order with ID {id} not found.");
            }

            if (orderUpdateStatusDTO.Status.HasValue)
            {
                order.Status = orderUpdateStatusDTO.Status.Value;

                var orderDetails = await _orderDetailRepository.GetByOrderIdAsync(id);

                // Nếu trạng thái đơn hàng là 2 (Approved)
                if (order.Status == 2)
                {
                    foreach (var detail in orderDetails)
                    {
                        var product = await _productRepository.GetByIdAsync(detail.ProductId);
                        if (product != null)
                        {
                            if (order.OrderType == 1) // Nhập kho: Cộng số lượng
                            {
                                product.AvailableQuantity += detail.Quantity;
                            }
                            else if (order.OrderType == 2) // Xuất kho: Trừ số lượng
                            {
                                if (product.AvailableQuantity < detail.Quantity)
                                {
                                    throw new InvalidOperationException($"Not enough stock for product {product.ProductId}. Available: {product.AvailableQuantity}, Required: {detail.Quantity}");
                                }
                                product.AvailableQuantity -= detail.Quantity;
                            }
                            product.UpdatedAt = DateTime.UtcNow;
                            await _productRepository.UpdateAsync(product);
                        }
                    }
                }
            }

            order.UpdatedAt = orderUpdateStatusDTO.UpdatedAt ?? DateTime.UtcNow;
            await _orderRepository.UpdateAsync(order);

            return new OrderDTO
            {
                OrderId = order.OrderId,
                SupplierId = order.SupplierId,
                OrderDate = order.OrderDate,
                OrderType = order.OrderType,
                Status = order.Status,
                UserId = order.UserId,
                CreatedAt = order.CreatedAt,
                UpdatedAt = order.UpdatedAt
            };
        }


        public async Task<List<ProductExportDTO>> GetTop5ExportProductsAsync()
        {
            // Lấy danh sách các đơn hàng xuất kho (OrderType = 2) trong tất cả thời gian
            var exportOrders = await _orderRepository.GetAllAsync();
            exportOrders = exportOrders
                .Where(o => o.OrderType == 2) // Chỉ lấy đơn hàng xuất kho
                .ToList();

            if (!exportOrders.Any())
            {
                return new List<ProductExportDTO>(); // Nếu không có đơn hàng, trả về danh sách rỗng
            }

            // Lấy danh sách OrderID của các đơn hàng xuất kho
            var orderIds = exportOrders.Select(o => o.OrderId).ToList();

            // Lấy tất cả OrderDetails liên quan đến các đơn hàng xuất kho
            var orderDetails = await _orderDetailRepository.GetByOrderIdsAsync(orderIds);

            // Nhóm OrderDetails theo ProductId và tính tổng số lượng xuất kho
            var productQuantities = orderDetails
                .GroupBy(od => od.ProductId)
                .Select(g => new
                {
                    ProductId = g.Key,
                    TotalQuantity = g.Sum(od => od.Quantity)
                })
                .OrderByDescending(x => x.TotalQuantity)
                .Take(5) // Mặc định lấy 5 sản phẩm
                .ToList();

            // Lấy thông tin sản phẩm từ ProductRepository
            var topProducts = new List<ProductExportDTO>();
            foreach (var item in productQuantities)
            {
                var product = await _productRepository.GetByIdAsync(item.ProductId);
                if (product != null)
                {
                    topProducts.Add(new ProductExportDTO
                    {
                        ProductId = item.ProductId,
                        ProductName = product.Name,
                        TotalQuantityExported = item.TotalQuantity,
                        ImageUrl = product.Images // Lấy đường dẫn ảnh từ thuộc tính Images của Product
                    });
                }
            }

            return topProducts;
        }

    }
}