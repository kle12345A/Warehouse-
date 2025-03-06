using BussinessLayer.Service.shipping;
using Microsoft.AspNetCore.Mvc;
using WarehouseDTOs;

namespace Warehouse.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ShippingController : ControllerBase
    {
        private readonly IShippingService _shippingService;

        public ShippingController(IShippingService shippingService)
        {
            _shippingService = shippingService;
        }

        [HttpGet]
        public async Task<ActionResult<List<ShippingDTO>>> GetAllShippings()
        {
            var shippings = await _shippingService.GetAllShippingsAsync();
            return Ok(shippings);
        }


        [HttpGet("{id}")]
        public async Task<ActionResult<ShippingDTO>> GetShippingById(int id)
        {
            var shipping = await _shippingService.GetShippingByIdAsync(id);
            return Ok(shipping);
        }


        [HttpPost]
        public async Task<ActionResult<ShippingDTO>> CreateShipping([FromBody] ShippingDTO shippingDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var createdShipping = await _shippingService.CreateShippingAsync(shippingDto);
            return CreatedAtAction(nameof(GetShippingById), new { id = createdShipping.ShippingId }, createdShipping);
        }


        [HttpPut("{id}")]
        public async Task<ActionResult<ShippingDTO>> UpdateShipping(int id, [FromBody] ShippingDTO shippingDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var updatedShipping = await _shippingService.UpdateShippingAsync(id, shippingDto);
            return Ok(updatedShipping);
        }


        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteShipping(int id)
        {
            var result = await _shippingService.DeleteShippingAsync(id);
            if (!result)
            {
                return NotFound(new { message = "Vận chuyển không tồn tại" });
            }
            return NoContent();
        }
    }
}