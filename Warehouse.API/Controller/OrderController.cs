using BussinessLayer.Service.order;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WarehouseDTOs;

namespace Warehouse.API.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly IOrderService _orderService;

        public OrderController(IOrderService orderService)
        {
            _orderService = orderService;
        }
        [HttpPost]
        public async Task<IActionResult> CreateOrder([FromBody] OrderCreateDTO orderCreateDTO)
        {
            if (orderCreateDTO == null || orderCreateDTO.OrderDetails == null || !orderCreateDTO.OrderDetails.Any())
            {
                return BadRequest("Order data or OrderDetails is missing.");
            }

            var createdOrder = await _orderService.CreateOrderAsync(orderCreateDTO);

            return CreatedAtAction(nameof(GetOrderById), new { id = createdOrder.OrderId }, createdOrder);
        }
        [HttpGet]
        public async Task<ActionResult<List<OrderListDTO>>> GetAllOrders()
        {
            var orders =  await _orderService.GetAllOrdersAsync();
            return Ok(orders);
        }
       [HttpGet("{id}")]
        public async Task<ActionResult<OrderDTO>> GetOrderById(int id)
        {
            var order = await _orderService.GetOrderByIdAsync(id);
            if (order == null)
                return NotFound(new { message = "Đơn hàng không tồn tại." });

            return Ok(order);
        }

        //[HttpPost]
        //public async Task<ActionResult<OrderDTO>> CreateOrder([FromBody] OrderDTO orderDto)
        //{
        //    if (!ModelState.IsValid) return BadRequest(ModelState);
        //    var createdOrder = await _orderService.CreateOrderAsync(orderDto);
        //    return CreatedAtAction(nameof(GetOrderById), new { id = createdOrder.OrderId }, createdOrder);
        //}

        [HttpPut("{id}")]
        public async Task<ActionResult<OrderDTO>> UpdateOrder(int id, [FromBody] OrderUpdateStatusDTO orderUpdateDTO)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            if (id != orderUpdateDTO.OrderId) return BadRequest(new { message = "ID không khớp" });
            var updatedOrder = await _orderService.UpdateStatusOrder(id, orderUpdateDTO);
            if (updatedOrder == null) return NotFound(new { message = "Đơn hàng không tồn tại" });
            return Ok(updatedOrder);
        }


        [HttpPatch("{id}/status")]
        public async Task<ActionResult<OrderDTO>> UpdateOrderStatus(int id, [FromBody] OrderUpdateStatusDTO orderUpdateStatusDTO)
        {
            if (orderUpdateStatusDTO == null)
            {
                return BadRequest(new { message = "Dữ liệu cập nhật không hợp lệ." });
            }

            var updatedOrder = await _orderService.UpdateStatusOrder(id, orderUpdateStatusDTO);

            if (updatedOrder == null)
            {
                return NotFound(new { message = "Đơn hàng không tồn tại." });
            }

            return Ok(updatedOrder);
        }

        [HttpGet("total-export-amount-current-month")]
        public async Task<IActionResult> GetTotalExportAmountForCurrentMonth()
        {
            try
            {
                var totalAmount = await _orderService.GetTotalExportAmountForCurrentMonthAsync();
                return Ok(new { TotalExportAmount = totalAmount });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "An error occurred while calculating the total export amount.", Error = ex.Message });
            }
        }
        [HttpGet("top-5-export-products")]
        public async Task<IActionResult> GetTop5ExportProducts()
        {
            try
            {
                var topProducts = await _orderService.GetTop5ExportProductsAsync();
                return Ok(topProducts);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "An error occurred while fetching top export products.", Error = ex.Message });
            }
        }



    }
}
