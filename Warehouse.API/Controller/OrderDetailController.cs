using BussinessLayer.Service.orderdetail;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WarehouseDTOs;

namespace Warehouse.API.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderDetailController : ControllerBase
    {
        private readonly IOrderDetailService _orderDetailService;

        public OrderDetailController(IOrderDetailService orderDetailService)
        {
            _orderDetailService = orderDetailService;
        }
        [HttpGet("{id}")]
        public async Task<ActionResult<List<OrderDetailDTO>>> GetAllOrders(int id)
        {
            var orders = await _orderDetailService.GetAllAsync(id);
            return Ok(orders);
        }

    }
}
